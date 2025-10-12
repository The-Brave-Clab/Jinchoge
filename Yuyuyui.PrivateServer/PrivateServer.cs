using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer
{
    public static class PrivateServer
    {
        public const string YUYUYUI_APP_VERSION = "3.27.0";

        public const string OFFICIAL_API_SERVER = "app.yuyuyui.jp";
        public const string PRIVATE_PUBLIC_API_SERVER = "936fkiz1v2.execute-api.ap-northeast-1.amazonaws.com";

        private static IPrivateServerResourceProvider? resourceProvider = null;

        public static IPrivateServerResourceProvider ResourceProvider =>
            resourceProvider ??
            throw new InvalidOperationException(
                "PrivateServer is not initialized. Call PrivateServer.Init() first.");

        public static async Task Init(IPrivateServerResourceProvider privateServerResourceProvider)
        {
            resourceProvider = privateServerResourceProvider;

            DataModel.Config.BaseDir = ResourceProvider.masterDataProvider.Directory;
            await ResourceProvider.masterDataProvider.Initialize();
        }

        public async static Task<PlayerProfile> RegisterNewPlayer(string uuid, string? code = null)
        {
            string newCode = Utils.GenerateRandomDigit(10);
            while (await PlayerProfile.Exists(newCode))
            {
                newCode = Utils.GenerateRandomDigit(10);
            }
            
            var player = new PlayerProfile
            {
                id = new()
                {
                    uuid = uuid, 
                    code = code ?? newCode
                }
            };

            await using (await ResourceProvider.distributedLockProvider.AcquirePlayerProfileLock(player.id.code))
            {
                await ResourceProvider.sessionProvider.AddNewPlayer(player);
                await player.Save();
            }

            Utils.Log(string.Format(Resources.LOG_PS_REGISTER_NEW_PLAYER, player.id.code));

            return player;
        }

        public static async Task<PlayerProfile?> EnsureDummyPlayer()
        {
            const string DUMMY_CODE = "0000000001";
            const string DUMMY_UUID = "0000000000000000000000000000000000000000000000000000000000000001";

            if (await PlayerProfile.Exists(DUMMY_CODE))
                return await PlayerProfile.Load(DUMMY_CODE);

            var dummyPlayer = await RegisterNewPlayer(DUMMY_UUID, DUMMY_CODE);
            dummyPlayer.profile.nickname = "無名な勇者さん";
            dummyPlayer.profile.comment = "無名な勇者さん";
            
            var yuuna = await Card.DefaultYuuna();
            yuuna.id = 1;

            dummyPlayer.cards.Add(100010, yuuna.id);
            await yuuna.Save();

            var yuunaUnit = await yuuna.CreateUnit();
            yuunaUnit.id = 1;
            
            await yuunaUnit.Save();
            
            var firstDeck = new Deck
            {
                id = 1,
                leaderUnitID = yuunaUnit.id,
                name = null,
                units = new List<long> {yuunaUnit.id}
            };
            dummyPlayer.decks.Add(firstDeck.id);
            await firstDeck.Save();
            
            await dummyPlayer.Save();

            Utils.Log(Resources.LOG_PS_CREATED_DUMMY_PLAYER);

            return dummyPlayer;
        }

        public static async Task<IPlayerProfileSessionProvider.PlayerSession> CreateSessionForPlayer(string uuid, EntityBase entity)
        {
            IPlayerProfileSessionProvider.PlayerSession playerSession =
                await ResourceProvider.sessionProvider.GetOrAddSessionFromUUID(uuid,
                    () => new IPlayerProfileSessionProvider.SessionInfo
                    {
                        id = Utils.GenerateRandomHexString(32),
                        key = Utils.GenerateRandomHexString(16),
                    },
                    async u => await RegisterNewPlayer(u));

            Utils.Log(string.Format(Resources.LOG_PS_CREATE_SESSION,
                playerSession.playerId.code, playerSession.session.id, playerSession.session.key));

            playerSession.deviceInfo = new IPlayerProfileSessionProvider.DeviceInfo
            {
                os = entity.GetRequestHeaderValue("X-APP-PLATFORM").Split(' ')[0] == "Android"
                    ? IPlayerProfileSessionProvider.DeviceInfo.OS.Android
                    : IPlayerProfileSessionProvider.DeviceInfo.OS.iOS,
                platformName = entity.GetRequestHeaderValue("X-APP-PLATFORM"),
                unityVersion = entity.GetRequestHeaderValue("X-Unity-Version"),
                appVersion = entity.GetRequestHeaderValue("X-APP-VERSION"),
                deviceName = entity.GetRequestHeaderValue("X-APP-DEVICE"),
                userAgent = entity.GetRequestHeaderValue("User-Agent"),
            };

            return playerSession;
        }

        public static async Task<IPlayerProfileSessionProvider.PlayerSession?> GetSessionFromCookie(this EntityBase entity)
        {
            string cookie = entity.GetRequestHeaderValue("Cookie");
            var cookies = cookie.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Split('='))
                .ToDictionary(e => e[0], e => e.Length > 1 ? e[1] : "");

            if (cookies.TryGetValue("_session_id", out var c))
            {
                return await ResourceProvider.sessionProvider.GetSessionFromSessionID(c);
            }

            return null;
        }

        public static async Task RemovePlayerProfile(PlayerProfile player)
        {
            await ResourceProvider.sessionProvider.RemovePlayer(player);
        }
    }
}