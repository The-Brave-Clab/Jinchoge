using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer.Desktop;

public class InMemoryPlayerProfileSessionProvider : IPlayerProfileSessionProvider
{
    private List<PlayerProfile.ID> playerIds = new();
    private Dictionary<string, IPlayerProfileSessionProvider.PlayerSession> playerSessions = new();

    private bool isInitialized = false;

    private async Task Init()
    {
        if (isInitialized) return;

        playerIds = new List<PlayerProfile.ID>();
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
            playerIds.Add(new()
            {
                uuid = split[0],
                code = split[1]
            });
        }

        isInitialized = true;
    }

    public async Task AddNewPlayer(PlayerProfile player)
    {
        await Init();
        playerIds.Add(player.id);

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

        IPlayerProfileSessionProvider.PlayerSession session;
        try
        {
            session = playerSessions.First(p => p.Value.playerId.uuid == playerUUID).Value;
        }
        catch (InvalidOperationException)
        {
            var playerId = playerIds.Any(id => id.uuid == playerUUID)
                ? playerIds.First(id => id.uuid == playerUUID)
                : (await registerNewPlayer(playerUUID)).id;
            session = new IPlayerProfileSessionProvider.PlayerSession
            {
                session = createSessionInfo(),
                playerId = playerId,
            };

            playerSessions.Add(session.session.id, session);
        }

        return session;
    }

    public async Task<IPlayerProfileSessionProvider.PlayerSession?> GetSessionFromSessionID(string sessionID)
    {
        await Init();
        return playerSessions.TryGetValue(sessionID, out var playerSession) ? playerSession : null;
    }
}