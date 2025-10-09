using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public interface IPlayerDataProvider<TPlayerData, TIdentifier> where TPlayerData : PlayerDataBase where TIdentifier : notnull
{
    Task<TPlayerData> Load(TIdentifier id);
    Task<IEnumerable<TPlayerData>> LoadMany(IEnumerable<TIdentifier> ids);
    Task Save(TPlayerData entity, TIdentifier id);
    Task<bool> Exists(TIdentifier id);
    Task Delete(TIdentifier id);
}