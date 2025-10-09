using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer.Desktop;

public class InMemoryPlayerProfileSessionProvider : IPlayerProfileSessionProvider
{
    private Dictionary<string, PlayerProfile> playerUUID = new();
    private Dictionary<string, PlayerProfile> playerCode = new();
    private Dictionary<string, IPlayerProfileSessionProvider.PlayerSession> playerSessions = new();

    private bool isInitialized = false;

    private async Task Init()
    {
        if (isInitialized) return;

        playerUUID = new Dictionary<string, PlayerProfile>();
        playerCode = new Dictionary<string, PlayerProfile>();
        playerSessions = new Dictionary<string, IPlayerProfileSessionProvider.PlayerSession>();

        var playerDataFile = Path.Combine(FileSystemData.dataFolder, FileSystemData.PLAYER_DATA_FILE);

        await FileSystemData.dataFileLock.WaitAsync();
        try
        {
            if (!File.Exists(playerDataFile))
            {
                await File.AppendAllTextAsync(playerDataFile, null);
            }
        }
        finally
        {
            FileSystemData.dataFileLock.Release();
        }

        string[] players;
        await FileSystemData.dataFileLock.WaitAsync();
        try
        {
            players = await File.ReadAllLinesAsync(playerDataFile);
        }
        finally
        {
            FileSystemData.dataFileLock.Release();
        }
            
        foreach (var s in players)
        {
            var split = s.Split(',');
            string uuid = split[0];
            string code = split[1];
            PlayerProfile player = await PlayerProfile.Load(code);
            playerUUID.Add(player!.id.uuid, player);
            playerCode.Add(player.id.code, player);
        }

        isInitialized = true;
    }

    public async Task<PlayerProfile?> GetPlayerProfileFromUUID(string playerUUID)
    {
        await Init();
        return this.playerUUID.TryGetValue(playerUUID, out var profile) ? profile : null;
    }

    public async Task<PlayerProfile?> GetPlayerProfileFromCode(string playerCode)
    {
        await Init();
        return this.playerCode.TryGetValue(playerCode, out var profile) ? profile : null;
    }

    public async Task AddNewPlayer(PlayerProfile player)
    {
        await Init();
        playerUUID.Add(player.id.uuid, player);
        playerCode.Add(player.id.code, player);

        var playerDataFile = Path.Combine(FileSystemData.dataFolder, FileSystemData.PLAYER_DATA_FILE);
        await FileSystemData.dataFileLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(playerDataFile, $"{player.id.uuid},{player.id.code}\n");
        }
        finally
        {
            FileSystemData.dataFileLock.Release();
        }
    }

    public async Task RemovePlayer(PlayerProfile player)
    {
        var playerDataFile = Path.Combine(FileSystemData.dataFolder, FileSystemData.PLAYER_DATA_FILE);
        await FileSystemData.dataFileLock.WaitAsync();
        try
        {
            string[] lines;
            using (StreamReader sr = new(playerDataFile))
            {
                var content = await sr.ReadToEndAsync();
                lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            }

            var newLines = lines.Where(line => !line.StartsWith(player.id.uuid));
            await using (StreamWriter sw = new(playerDataFile, false))
            {
                foreach (var line in newLines)
                {
                    await sw.WriteLineAsync(line);
                }
            }
        }
        finally
        {
            FileSystemData.dataFileLock.Release();
        }
    }

    public async Task<IPlayerProfileSessionProvider.PlayerSession> GetOrAddSessionFromUUID(string playerUUID,
        Func<IPlayerProfileSessionProvider.SessionInfo> createSessionInfo,
        Func<string /* uuid */, Task<PlayerProfile>> registerNewPlayer)
    {
        await Init();

        if (playerSessions.TryGetValue(playerUUID, out var playerSession))
            return playerSession;


        playerSession = new IPlayerProfileSessionProvider.PlayerSession
        {
            session = createSessionInfo(),
            player = this.playerUUID.TryGetValue(playerUUID, out var value) ? value : await registerNewPlayer(playerUUID),
        };

        playerSessions.Add(playerSession.session.id, playerSession);

        return playerSession;
    }

    public async Task<IPlayerProfileSessionProvider.PlayerSession?> GetSessionFromSessionID(string sessionID)
    {
        await Init();
        return playerSessions.TryGetValue(sessionID, out var playerSession) ? playerSession : null;
    }
}