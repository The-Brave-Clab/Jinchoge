using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Yuyuyui.PrivateServer.AWS;

/// <summary>
/// DynamoDB-based distributed lock provider for PlayerProfile operations.
/// Uses a separate YuyuyuiLocks table to coordinate locks across Lambda instances.
/// Supports Lambda-context-wise reentrancy using ILambdaContext.AwsRequestId.
/// </summary>
public class DynamoDBLockProvider : IDistributedLockProvider
{
    private readonly IAmazonDynamoDB client;
    private const string LOCKS_TABLE_NAME = "YuyuyuiLocks";
    private const int LOCK_TTL_SECONDS = 30; // Lock expires after 30 seconds (handles crashes/timeouts)
    private const int MAX_RETRY_ATTEMPTS = 20; // Max ~10 seconds of retries
    
    /// <summary>
    /// Set by Lambda handler at the start of each request. Used as lock ID for better traceability
    /// and to enable Lambda-context-wise reentrancy (same request can acquire same lock multiple times).
    /// MUST be cleared at the end of each request to prevent issues with Lambda container reuse.
    /// </summary>
    public static string? CurrentLambdaRequestId { get; set; }
    
    /// <summary>
    /// Tracks which locks have been acquired in the current Lambda request for reentrancy support.
    /// MUST be cleared at the end of each request.
    /// </summary>
    private static readonly HashSet<string> acquiredLocksInThisRequest = new();
    private static readonly object reentrantLockObject = new();
    
    // Table Design:
    // ┌─────────────────────────┬────────────────────────┬──────────────┬──────────────┬──────┐
    // │ PK                      │ lockId                 │ lockExpiry   │ acquiredAt   │ ttl  │
    // ├─────────────────────────┼────────────────────────┼──────────────┼──────────────┼──────┤
    // │ PLAYERPROFILE#00000001  │ abc-123-lambda-req-id  │ 1704123456   │ 1704123426   │ ...  │
    // │ PLAYERPROFILE#00000002  │ def-456-lambda-req-id  │ 1704123457   │ 1704123427   │ ...  │
    // └─────────────────────────┴────────────────────────┴──────────────┴──────────────┴──────┘
    //
    // Notes:
    // - PK: Primary key, format "PLAYERPROFILE#{playerCode}"
    // - lockId: Lambda request ID (ILambdaContext.AwsRequestId) for traceability and reentrancy
    // - lockExpiry: Unix timestamp when lock expires (now + 30s)
    // - acquiredAt: Unix timestamp when lock was acquired
    // - ttl: DynamoDB TTL attribute for automatic cleanup (lockExpiry + 60s grace period)

    public DynamoDBLockProvider()
    {
        client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.APNortheast1);
    }

    /// <summary>
    /// Clears the acquired locks tracking. MUST be called at the end of each Lambda request
    /// to prevent issues with Lambda container reuse.
    /// </summary>
    public static void ClearRequestContext()
    {
        lock (reentrantLockObject)
        {
            acquiredLocksInThisRequest.Clear();
        }
        CurrentLambdaRequestId = null;
    }

    public async Task<IAsyncDisposable> AcquirePlayerProfileLock(string playerCode)
    {
        string lockKey = $"PLAYERPROFILE#{playerCode}";
        
        // Check if we already own this lock in this Lambda request (reentrant lock)
        lock (reentrantLockObject)
        {
            if (acquiredLocksInThisRequest.Contains(lockKey))
            {
                // We already own this lock - return a no-op handle
                Utils.LogTrace($"Reentrant lock detected for {playerCode} in request {CurrentLambdaRequestId}");
                return new NoOpLockHandle();
            }
        }
        
        // Use Lambda request ID if available (for AWS), fallback to GUID (for desktop compatibility during testing)
        string lockId = CurrentLambdaRequestId ?? Guid.NewGuid().ToString();
        
        int attempt = 0;
        while (attempt < MAX_RETRY_ATTEMPTS)
        {
            try
            {
                await AcquireLock(lockKey, lockId);
                
                // Mark as acquired for reentrancy tracking
                lock (reentrantLockObject)
                {
                    acquiredLocksInThisRequest.Add(lockKey);
                }
                
                return new LockHandle(client, lockKey, lockId, () => {
                    // Cleanup callback: remove from acquired locks when released
                    lock (reentrantLockObject)
                    {
                        acquiredLocksInThisRequest.Remove(lockKey);
                    }
                });
            }
            catch (ConditionalCheckFailedException)
            {
                attempt++;
                
                if (attempt >= MAX_RETRY_ATTEMPTS)
                {
                    throw new Exception($"Failed to acquire PlayerProfile lock for {playerCode} after {MAX_RETRY_ATTEMPTS} attempts (~10 seconds). " +
                                      "This usually indicates high contention or a stuck lock. Check for long-running requests.");
                }
                
                // Exponential backoff with jitter
                int delayMs = CalculateBackoffDelay(attempt);
                await Task.Delay(delayMs);
            }
        }
        
        // Should never reach here due to throw in loop
        throw new Exception($"Unexpected error acquiring lock for {playerCode}");
    }

    private async Task AcquireLock(string lockKey, string lockId)
    {
        long currentTime = Utils.FromDateTime(DateTime.UtcNow);
        long lockExpiry = currentTime + LOCK_TTL_SECONDS;
        long ttl = lockExpiry + 60; // Grace period for DynamoDB TTL cleanup
        
        var putRequest = new PutItemRequest
        {
            TableName = LOCKS_TABLE_NAME,
            Item = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue { S = lockKey } },
                { "lockId", new AttributeValue { S = lockId } },
                { "lockExpiry", new AttributeValue { N = lockExpiry.ToString() } },
                { "acquiredAt", new AttributeValue { N = currentTime.ToString() } },
                { "ttl", new AttributeValue { N = ttl.ToString() } }
            },
            ConditionExpression = "attribute_not_exists(PK) OR lockExpiry < :now",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":now", new AttributeValue { N = currentTime.ToString() } }
            }
        };
        
        await client.PutItemAsync(putRequest);
    }

    private static int CalculateBackoffDelay(int attempt)
    {
        // Exponential backoff: 50ms, 100ms, 200ms, 400ms, 800ms, then cap at 1000ms
        int baseDelay = Math.Min(50 * (1 << (attempt - 1)), 1000);
        
        // Add jitter (±25%) to prevent thundering herd
        Random random = new Random();
        int jitter = random.Next(-baseDelay / 4, baseDelay / 4);
        
        return baseDelay + jitter;
    }

    /// <summary>
    /// No-op lock handle for reentrant lock cases (when same request acquires same lock multiple times).
    /// </summary>
    private class NoOpLockHandle : IAsyncDisposable
    {
        public void Dispose()
        {
            // No-op: Lock is already held by this request, will be released by the real handle
        }

        public ValueTask DisposeAsync()
        {
            // No-op: Lock is already held by this request, will be released by the real handle
            return ValueTask.CompletedTask;
        }
    }

    /// <summary>
    /// Lock handle that automatically releases the lock when disposed.
    /// </summary>
    private class LockHandle : IAsyncDisposable
    {
        private readonly IAmazonDynamoDB client;
        private readonly string lockKey;
        private readonly string lockId;
        private readonly Action? onDispose;
        private bool disposed = false;

        public LockHandle(IAmazonDynamoDB client, string lockKey, string lockId, Action? onDispose = null)
        {
            this.client = client;
            this.lockKey = lockKey;
            this.lockId = lockId;
            this.onDispose = onDispose;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            
            // Call cleanup callback (remove from acquired locks)
            onDispose?.Invoke();
            
            // Release lock asynchronously (fire-and-forget)
            // We don't want to block the response on lock release
            _ = ReleaseLockAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (disposed) return;
            disposed = true;
            onDispose?.Invoke();
            await ReleaseLockAsync();
        }

        private async Task ReleaseLockAsync()
        {
            try
            {
                // Only delete if we still own the lock (prevent deleting someone else's lock)
                var deleteRequest = new DeleteItemRequest
                {
                    TableName = LOCKS_TABLE_NAME,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        { "PK", new AttributeValue { S = lockKey } }
                    },
                    ConditionExpression = "lockId = :lockId",
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        { ":lockId", new AttributeValue { S = lockId } }
                    }
                };
                
                await client.DeleteItemAsync(deleteRequest);
            }
            catch (ConditionalCheckFailedException)
            {
                // Lock already expired and was acquired by someone else - this is fine
                Utils.LogTrace($"Lock {lockKey} was already expired and potentially reacquired");
            }
            catch (Exception ex)
            {
                // Best effort release - log but don't throw
                // Lock will expire naturally via TTL if delete fails
                Utils.LogWarning($"Failed to release lock {lockKey}: {ex.Message}");
            }
        }
    }
}

