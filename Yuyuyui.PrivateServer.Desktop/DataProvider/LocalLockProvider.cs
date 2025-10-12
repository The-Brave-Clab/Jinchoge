using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer.Desktop;

/// <summary>
/// In-memory lock provider for desktop single-process environment.
/// Uses SemaphoreSlim for thread-safe locking within the same process.
/// </summary>
public class LocalLockProvider : IDistributedLockProvider
{
    // In-memory locks per player code
    private static readonly Dictionary<string, SemaphoreSlim> locks = new();
    private static readonly object lockObject = new();

    public async Task<IAsyncDisposable> AcquirePlayerProfileLock(string playerCode)
    {
        SemaphoreSlim semaphore = GetOrCreateSemaphore(playerCode);
        
        // Wait for lock with timeout (30 seconds, same as AWS)
        bool acquired = await semaphore.WaitAsync(TimeSpan.FromSeconds(30));
        
        if (!acquired)
        {
            throw new Exception($"Failed to acquire PlayerProfile lock for {playerCode} after 30 seconds. " +
                              "This usually indicates a deadlock or very long-running request.");
        }
        
        return new LockHandle(semaphore);
    }

    private static SemaphoreSlim GetOrCreateSemaphore(string playerCode)
    {
        lock (lockObject)
        {
            if (!locks.TryGetValue(playerCode, out var semaphore))
            {
                // Create new semaphore for this player (max count = 1, ensuring mutual exclusion)
                semaphore = new SemaphoreSlim(1, 1);
                locks[playerCode] = semaphore;
            }
            return semaphore;
        }
    }

    /// <summary>
    /// Lock handle that automatically releases the semaphore when disposed.
    /// </summary>
    private class LockHandle : IAsyncDisposable
    {
        private readonly SemaphoreSlim semaphore;
        private bool disposed = false;

        public LockHandle(SemaphoreSlim semaphore)
        {
            this.semaphore = semaphore;
        }

        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            
            semaphore.Release();
        }

        public ValueTask DisposeAsync()
        {
            if (disposed) return ValueTask.CompletedTask;
            disposed = true;
            
            semaphore.Release();
            return ValueTask.CompletedTask;
        }
    }
}

