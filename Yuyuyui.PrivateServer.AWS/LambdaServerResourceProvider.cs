using System;

namespace Yuyuyui.PrivateServer.AWS;

public class LambdaServerResourceProvider : IPrivateServerResourceProvider
{
    private DynamoDBPlayerDataProviderFactory playerDataProviderFactory = new();
    private DynamoDBPlayerProfileSessionProvider lambdaSessionProvider = new();
    private LambdaMasterDataProvider lambdaMasterDataProvider = new();
    private ConfigPlayerInGameConfigProvider lambdaInGameConfigProvider = new();
    private DynamoDBLockProvider lambdaDistributedLockProvider = new();

    public IPlayerDataProvider<TPlayerData, TIdentifier> GetDataProvider<TPlayerData, TIdentifier>() where TPlayerData : BasePlayerData<TPlayerData, TIdentifier> where TIdentifier : notnull
    {
        return playerDataProviderFactory.Get<TPlayerData, TIdentifier>();
    }

    public IPlayerProfileSessionProvider sessionProvider => lambdaSessionProvider;
    public IMasterDataProvider masterDataProvider => lambdaMasterDataProvider;
    public IInGameConfigProvider inGameConfigProvider => lambdaInGameConfigProvider;
    public IDistributedLockProvider distributedLockProvider => lambdaDistributedLockProvider;

    public Uri RewriteRequestUri(Uri originalRequestUri)
    {
        return originalRequestUri;
    }
}