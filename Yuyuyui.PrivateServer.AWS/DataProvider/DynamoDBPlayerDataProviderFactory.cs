namespace Yuyuyui.PrivateServer.AWS;

public class DynamoDBPlayerDataProviderFactory : PlayerDataProviderFactory
{
    protected override IPlayerDataProvider<TPlayerData, TIdentifier> Create<TPlayerData, TIdentifier>()
    {
        return new DynamoDBPlayerDataProvider<TPlayerData, TIdentifier>();
    }
}