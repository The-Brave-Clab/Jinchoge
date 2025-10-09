using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer
{
    public abstract class BasePlayerData<TSelf, TIdentifier> : PlayerDataBase
        where TSelf : BasePlayerData<TSelf, TIdentifier>
        where TIdentifier : notnull
    {
        protected abstract TIdentifier Identifier { get; }
        protected override string DataType => typeof(TSelf).Name;

        public async Task Save()
        {
            await PlayerDataProviderFactory.ActiveFactory!.Get<TSelf, TIdentifier>().Save((TSelf)this, Identifier);
        }

        public static async Task<TSelf> Load(TIdentifier identifier)
        {
            return await PlayerDataProviderFactory.ActiveFactory!.Get<TSelf, TIdentifier>().Load(identifier);
        }

        public static async Task<IEnumerable<TSelf>> LoadMany(IEnumerable<TIdentifier> identifiers)
        {
            return await PlayerDataProviderFactory.ActiveFactory!.Get<TSelf, TIdentifier>().LoadMany(identifiers);
        }

        public static async Task Delete(TIdentifier identifier)
        {
            await PlayerDataProviderFactory.ActiveFactory!.Get<TSelf, TIdentifier>().Delete(identifier);
        }

        public async Task Delete()
        {
            await Delete(Identifier);
        }

        public static async Task<bool> Exists(TIdentifier identifier)
        {
            return await PlayerDataProviderFactory.ActiveFactory!.Get<TSelf, TIdentifier>().Exists(identifier);
        }
    }
}