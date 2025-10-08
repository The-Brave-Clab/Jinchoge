using System;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer.AWS;

public class DynamoDBPlayerProfileSessionProvider : IPlayerProfileSessionProvider
{
    public Task<PlayerProfile?> GetPlayerProfileFromUUID(string playerUUID)
    {
        throw new NotImplementedException();
    }

    public Task<PlayerProfile?> GetPlayerProfileFromCode(string playerCode)
    {
        throw new NotImplementedException();
    }

    public Task AddNewPlayer(PlayerProfile player)
    {
        throw new NotImplementedException();
    }

    public Task RemovePlayer(PlayerProfile player)
    {
        throw new NotImplementedException();
    }

    public Task<IPlayerProfileSessionProvider.PlayerSession> GetOrAddSessionFromUUID(string playerUUID, Func<IPlayerProfileSessionProvider.SessionInfo> createSessionInfo, Func<string, Task<PlayerProfile>> registerNewPlayer)
    {
        throw new NotImplementedException();
    }

    public Task<IPlayerProfileSessionProvider.PlayerSession?> GetSessionFromSessionID(string sessionID)
    {
        throw new NotImplementedException();
    }
}