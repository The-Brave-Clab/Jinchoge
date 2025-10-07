using System;
using System.Collections.Concurrent;

namespace Yuyuyui.PrivateServer;

public abstract class PlayerDataProviderFactory
{
    public static PlayerDataProviderFactory? ActiveFactory { get; set; } = null;

    private static readonly ConcurrentDictionary<(Type playerDataType, Type identifierType), object> _providers = new();

    public IPlayerDataProvider<TPlayerData, TIdentifier> Get<TPlayerData, TIdentifier>()
        where TPlayerData : PlayerDataBase
        where TIdentifier : notnull
    {
        return (IPlayerDataProvider<TPlayerData, TIdentifier>)
            _providers.GetOrAdd(
                (typeof(TPlayerData), typeof(TIdentifier)),
                _ => Create<TPlayerData, TIdentifier>());
    }

    protected abstract IPlayerDataProvider<TPlayerData, TIdentifier> Create<TPlayerData, TIdentifier>()
        where TPlayerData : PlayerDataBase
        where TIdentifier : notnull;
}