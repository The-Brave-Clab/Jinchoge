using System;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

public interface IPlayerProfileSessionProvider
{
    public class PlayerSession
    {
        public PlayerProfile.ID playerId = new();
        public SessionInfo session = new();
        public DeviceInfo deviceInfo = new();
    }

    public struct SessionInfo
    {
        public string id;
        public string key;
    }

    public struct DeviceInfo
    {
        public enum OS
        {
            Android,
            iOS
        }

        public OS os;
        public string platformName;
        public string unityVersion;
        public string appVersion;
        public string deviceName;
        public string userAgent;
    }

    Task AddNewPlayer(PlayerProfile player);
    Task RemovePlayer(PlayerProfile player);
    Task<PlayerSession> GetOrAddSessionFromUUID(string playerUUID, Func<SessionInfo> createSessionInfo, Func<string /* uuid */, Task<PlayerProfile>> registerNewPlayer);
    Task<PlayerSession?> GetSessionFromSessionID(string sessionID);
}