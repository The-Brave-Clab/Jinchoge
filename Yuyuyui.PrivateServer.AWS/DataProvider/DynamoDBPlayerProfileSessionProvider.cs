using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Yuyuyui.PrivateServer.AWS;

public class DynamoDBPlayerProfileSessionProvider : IPlayerProfileSessionProvider
{
    private readonly IAmazonDynamoDB client;
    private const string SESSIONS_TABLE_NAME = "YuyuyuiSessions";
    private const string GSI_NAME = "GSI1";
    
    // TTL constants
    private const long TTL_DURATION = 7200; // 2 hours in seconds
    private const long TTL_REFRESH_THRESHOLD = 600; // 10 minutes in seconds

    // Table Design:
    // ┌─────────────────┬─────────────────┬──────────┬─────────────┬────────────┬──────────┬─────────────┐
    // │ PK              │ GSI1PK          │ GSI1SK   │ playerCode  │ sessionKey │ ttl      │ lastActive  │
    // ├─────────────────┼─────────────────┼──────────┼─────────────┼────────────┼──────────┼─────────────┤
    // │ abc123def456... │ UUID#00000...01 │ SESSION  │ 0000000001  │ f3a2b1...  │ 17041... │ 17041...    │
    // │ 789xyz012...    │ UUID#00000...02 │ SESSION  │ 0000000002  │ c4d5e6...  │ 17041... │ 17041...    │
    // └─────────────────┴─────────────────┴──────────┴─────────────┴────────────┴──────────┴─────────────┘

    public DynamoDBPlayerProfileSessionProvider()
    {
        client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.APNortheast1);
    }

    public Task AddNewPlayer(PlayerProfile player)
    {
        // No-op for DynamoDB: Player profiles are stored in PlayerData table via PlayerProfile.Save()
        // Desktop version tracks players in a file, but DynamoDB doesn't need separate tracking
        return Task.CompletedTask;
    }

    public Task RemovePlayer(PlayerProfile player)
    {
        // No-op for DynamoDB: Player deletion is handled by PlayerProfile.Delete()
        // This method is only for cleaning up desktop tracking files
        return Task.CompletedTask;
    }

    public async Task<IPlayerProfileSessionProvider.PlayerSession> GetOrAddSessionFromUUID(
        string playerUUID, 
        Func<IPlayerProfileSessionProvider.SessionInfo> createSessionInfo, 
        Func<string, Task<PlayerProfile>> registerNewPlayer)
    {
        // Query GSI to find existing session by UUID
        var queryRequest = new QueryRequest
        {
            TableName = SESSIONS_TABLE_NAME,
            IndexName = GSI_NAME,
            KeyConditionExpression = "GSI1PK = :uuid",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":uuid", new AttributeValue { S = $"UUID#{playerUUID}" } }
            }
        };

        var queryResponse = await client.QueryAsync(queryRequest);

        // If session exists, parse it directly from GSI query result
        if (queryResponse.Items.Count > 0)
        {
            var sessionItem = queryResponse.Items[0];
            return await ParseSessionFromItem(sessionItem);
        }

        // No session exists, create new one
        var sessionInfo = createSessionInfo();
        
        // Try to load existing player by UUID, or register new one
        var player = await LoadPlayerProfileByUUID(playerUUID) 
                     ?? await registerNewPlayer(playerUUID);

        // Create and save new session
        var session = new IPlayerProfileSessionProvider.PlayerSession
        {
            session = sessionInfo,
            player = player
        };

        await SaveSession(session);
        
        return session;
    }

    public async Task<IPlayerProfileSessionProvider.PlayerSession?> GetSessionFromSessionID(string sessionID)
    {
        // Get session from DynamoDB
        var getRequest = new GetItemRequest
        {
            TableName = SESSIONS_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue { S = sessionID } }
            }
        };

        var getResponse = await client.GetItemAsync(getRequest);

        if (!getResponse.IsItemSet || getResponse.Item.Count == 0)
        {
            return null;
        }

        return await ParseSessionFromItem(getResponse.Item);
    }

    private async Task<IPlayerProfileSessionProvider.PlayerSession> ParseSessionFromItem(
        Dictionary<string, AttributeValue> item)
    {
        // Extract session data from DynamoDB item
        string sessionId = item["PK"].S;
        string playerCode = item["playerCode"].S;
        string sessionKey = item["sessionKey"].S;
        long ttl = long.Parse(item["ttl"].N);

        // Check if TTL needs refresh (less than 10 minutes remaining)
        long currentTime = Utils.FromDateTime(DateTime.UtcNow);
        long timeUntilExpiry = ttl - currentTime;

        if (timeUntilExpiry is < TTL_REFRESH_THRESHOLD and > 0)
        {
            // Update TTL to extend session
            await UpdateSessionTTL(sessionId, currentTime);
        }

        // Load player profile
        var player = await PlayerProfile.Load(playerCode);

        // Deserialize device info

        return new IPlayerProfileSessionProvider.PlayerSession
        {
            player = player,
            session = new IPlayerProfileSessionProvider.SessionInfo
            {
                id = sessionId,
                key = sessionKey
            },
            deviceInfo = new() // Will be filled later
        };
    }

    private async Task<PlayerProfile?> LoadPlayerProfileByUUID(string playerUUID)
    {
        // Use GSI2 on PlayerData table to find profile by UUID
        // This is an O(1) query operation that scales efficiently
        var dynamoDBPlayerDataProvider = PlayerDataProviderFactory.ActiveFactory!.Get<PlayerProfile, string>()
            as DynamoDBPlayerDataProvider<PlayerProfile, string>;
        return await dynamoDBPlayerDataProvider!.LoadPlayerProfileByUUID(playerUUID);
    }

    private async Task SaveSession(IPlayerProfileSessionProvider.PlayerSession session)
    {
        long currentTime = Utils.FromDateTime(DateTime.UtcNow);
        long ttl = currentTime + TTL_DURATION;

        var putRequest = new PutItemRequest
        {
            TableName = SESSIONS_TABLE_NAME,
            Item = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue { S = session.session.id } },
                { "GSI1PK", new AttributeValue { S = $"UUID#{session.player.id.uuid}" } },
                { "GSI1SK", new AttributeValue { S = "SESSION" } },
                { "playerCode", new AttributeValue { S = session.player.id.code } },
                { "sessionKey", new AttributeValue { S = session.session.key } },
                { "ttl", new AttributeValue { N = ttl.ToString() } },
                { "lastActive", new AttributeValue { N = currentTime.ToString() } },
            }
        };

        await client.PutItemAsync(putRequest);
    }

    private async Task UpdateSessionTTL(string sessionID, long currentTime)
    {
        long newTtl = currentTime + TTL_DURATION;

        var updateRequest = new UpdateItemRequest
        {
            TableName = SESSIONS_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue { S = sessionID } }
            },
            UpdateExpression = "SET #ttl = :ttl, #lastActive = :lastActive",
            ExpressionAttributeNames = new Dictionary<string, string>
            {
                { "#ttl", "ttl" },
                { "#lastActive", "lastActive" }
            },
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":ttl", new AttributeValue { N = newTtl.ToString() } },
                { ":lastActive", new AttributeValue { N = currentTime.ToString() } }
            }
        };

        await client.UpdateItemAsync(updateRequest);
    }
}