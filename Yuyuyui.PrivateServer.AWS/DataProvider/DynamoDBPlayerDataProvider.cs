using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Yuyuyui.PrivateServer.AWS;

public class DynamoDBPlayerDataProvider<TPlayerData, TIdentifier> : IPlayerDataProvider<TPlayerData, TIdentifier>
    where TPlayerData : BasePlayerData<TPlayerData, TIdentifier>
    where TIdentifier : notnull
{
    private readonly IAmazonDynamoDB client;
    private const string PLAYER_DATA_TABLE_NAME = "YuyuyuiPlayerData";
    private const string GSI2_NAME = "GSI2";

    // Table Design
    // ┌──────────────────────────┬─────────┬────────────┬──────────────┬──────────┐
    // │ PK                       │ version │ data       │ GSI2PK       │ GSI2SK   │
    // ├──────────────────────────┼─────────┼────────────┼──────────────┼──────────┤
    // │ PLAYERPROFILE#0000000001 │ 5       │ {...}      │ UUID#000...1 │ PROFILE  │ ← Has GSI
    // │ CARD#123456789           │ 12      │ {...}      │ (no attr)    │          │ ← No GSI
    // │ CARD#987654321           │ 8       │ {...}      │ (no attr)    │          │ ← No GSI
    // │ DECK#555555555           │ 3       │ {...}      │ (no attr)    │          │ ← No GSI
    // │ UNIT#111111111           │ 1       │ {...}      │ (no attr)    │          │ ← No GSI
    // │ PLAYERPROFILE#0000000002 │ 2       │ {...}      │ UUID#000...2 │ PROFILE  │ ← Has GSI
    // └──────────────────────────┴─────────┴────────────┴──────────────┴──────────┘

    public DynamoDBPlayerDataProvider()
    {
        client = new AmazonDynamoDBClient(Amazon.RegionEndpoint.APNortheast1);
    }

    public async Task<TPlayerData> Load(TIdentifier id)
    {
        var getRequest = new GetItemRequest
        {
            TableName = PLAYER_DATA_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue { S = GetPrimaryKey(id) } }
            }
        };

        var getResponse = await client.GetItemAsync(getRequest);

        if (!getResponse.IsItemSet || getResponse.Item.Count == 0)
        {
            throw new Exception($"PlayerData not found: {typeof(TPlayerData).Name} with ID {id}");
        }

        var result = DeserializeEntity(getResponse.Item);

        Utils.LogTrace($"{typeof(TPlayerData).Name} #{id} loaded with version {result._version}");

        return result;
    }

    public async Task<IEnumerable<TPlayerData>> LoadMany(IEnumerable<TIdentifier> ids)
    {
        var identifiers = ids.ToList();
        if (identifiers.Count == 0)
        {
            return new List<TPlayerData>();
        }

        // DynamoDB BatchGetItem has a limit of 100 items per request
        const int batchSize = 100;
        var results = new List<TPlayerData>();

        for (int i = 0; i < identifiers.Count; i += batchSize)
        {
            var batch = identifiers.Skip(i).Take(batchSize).ToList();
            var keys = batch.Select(id => new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue { S = GetPrimaryKey(id) } }
            }).ToList();

            var batchRequest = new BatchGetItemRequest
            {
                RequestItems = new Dictionary<string, KeysAndAttributes>
                {
                    {
                        PLAYER_DATA_TABLE_NAME, new KeysAndAttributes
                        {
                            Keys = keys
                        }
                    }
                }
            };

            var batchResponse = await client.BatchGetItemAsync(batchRequest);

            if (batchResponse.Responses.TryGetValue(PLAYER_DATA_TABLE_NAME, out var items))
            {
                results.AddRange(items.Select(DeserializeEntity));
            }
        }

        if (results.Count != identifiers.Count)
        {
            Utils.LogWarning($"Requested to load {identifiers.Count} PlayerData of type {typeof(TPlayerData).Name}, " +
                             $"but only {results.Count} PlayerData were found");
        }

        foreach (var result in results)
        {
            Utils.LogTrace($"{typeof(TPlayerData).Name} #{result.Identifier} loaded with version {result._version}");
        }

        return results;
    }

    public async Task Save(TPlayerData entity, TIdentifier id)
    {
        string dataJson = JsonSerializer.Serialize(entity);
        string pk = GetPrimaryKey(id);

        var item = new Dictionary<string, AttributeValue>
        {
            { "PK", new AttributeValue { S = pk } },
            { "version", new AttributeValue { N = (entity._version + 1).ToString() } },
            { "data", new AttributeValue { S = dataJson } }
        };

        // Special handling for PlayerProfile: Add GSI2 attributes
        if (typeof(TPlayerData) == typeof(PlayerProfile) && entity is PlayerProfile profile)
        {
            item["GSI2PK"] = new AttributeValue { S = $"UUID#{profile.id.uuid}" };
            item["GSI2SK"] = new AttributeValue { S = "PROFILE" };
        }

        var putRequest = new PutItemRequest
        {
            TableName = PLAYER_DATA_TABLE_NAME,
            Item = item,
            ConditionExpression = "attribute_not_exists(PK) OR version = :currentVersion",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":currentVersion", new AttributeValue { N = entity._version.ToString() } }
            }
        };

        try
        {
            await client.PutItemAsync(putRequest);
            // Update version to match what was written to DB
            entity._version++;
            Utils.LogTrace($"{typeof(TPlayerData).Name} #{id} saved with version {entity._version}");
        }
        catch (ConditionalCheckFailedException)
        {
            throw new Exception($"Concurrency conflict: Entity {typeof(TPlayerData).Name} with ID {id} was modified by another request! " +
                                $"Expected version: {entity._version}");
        }
    }

    public async Task<bool> Exists(TIdentifier id)
    {
        var getRequest = new GetItemRequest
        {
            TableName = PLAYER_DATA_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue { S = GetPrimaryKey(id) } }
            },
            ProjectionExpression = "PK" // Only fetch the key to minimize data transfer
        };

        var getResponse = await client.GetItemAsync(getRequest);

        return getResponse.IsItemSet && getResponse.Item.Count > 0;
    }

    public async Task Delete(TIdentifier id)
    {
        var deleteRequest = new DeleteItemRequest
        {
            TableName = PLAYER_DATA_TABLE_NAME,
            Key = new Dictionary<string, AttributeValue>
            {
                { "PK", new AttributeValue { S = GetPrimaryKey(id) } }
            }
        };

        await client.DeleteItemAsync(deleteRequest);
    }

    private string GetPrimaryKey(TIdentifier id)
    {
        // Format: {TYPE}#{id}
        // Example: PLAYERPROFILE#0000000001, CARD#123456789, DECK#555555555
        return $"{typeof(TPlayerData).Name.ToUpperInvariant()}#{id}";
    }

    private TPlayerData DeserializeEntity(Dictionary<string, AttributeValue> item)
    {
        string dataJson = item["data"].S;
        int version = int.Parse(item["version"].N);

        var entity = JsonSerializer.Deserialize<TPlayerData>(dataJson)!;
        entity._version = version;

        return entity;
    }

    /// <summary>
    /// Query GSI2 to find PlayerProfile by UUID (used during login/registration)
    /// </summary>
    internal async Task<PlayerProfile?> LoadPlayerProfileByUUID(string playerUUID)
    {
        var queryRequest = new QueryRequest
        {
            TableName = PLAYER_DATA_TABLE_NAME,
            IndexName = GSI2_NAME,
            KeyConditionExpression = "GSI2PK = :uuid AND GSI2SK = :sk",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":uuid", new AttributeValue { S = $"UUID#{playerUUID}" } },
                { ":sk", new AttributeValue { S = "PROFILE" } }
            }
        };

        var queryResponse = await client.QueryAsync(queryRequest);

        if (queryResponse.Items.Count == 0)
        {
            return null;
        }

        var item = queryResponse.Items[0];
        return DeserializeEntity(item) as PlayerProfile;
    }
}