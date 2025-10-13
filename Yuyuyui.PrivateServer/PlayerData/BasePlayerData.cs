using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public abstract class BasePlayerData<TSelf, TIdentifier> : PlayerDataBase
    where TSelf : BasePlayerData<TSelf, TIdentifier>
    where TIdentifier : notnull
{
    public abstract TIdentifier Identifier { get; }
    protected override string DataType => typeof(TSelf).Name;

    public virtual async Task Save()
    {
        await PrivateServer.ResourceProvider.GetDataProvider<TSelf, TIdentifier>().Save((TSelf)this, Identifier);
    }

    public static async Task<TSelf> Load(TIdentifier identifier)
    {
        return await PrivateServer.ResourceProvider.GetDataProvider<TSelf, TIdentifier>().Load(identifier);
    }

    public static async Task<IEnumerable<TSelf>> LoadMany(IEnumerable<TIdentifier> identifiers)
    {
        return await PrivateServer.ResourceProvider.GetDataProvider<TSelf, TIdentifier>().LoadMany(identifiers);
    }

    public static async Task Delete(TIdentifier identifier)
    {
        await PrivateServer.ResourceProvider.GetDataProvider<TSelf, TIdentifier>().Delete(identifier);
    }

    public async Task Delete()
    {
        await Delete(Identifier);
    }

    public static async Task<bool> Exists(TIdentifier identifier)
    {
        return await PrivateServer.ResourceProvider.GetDataProvider<TSelf, TIdentifier>().Exists(identifier);
    }
}