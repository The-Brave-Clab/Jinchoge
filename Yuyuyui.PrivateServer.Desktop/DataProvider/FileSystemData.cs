using System;
using System.IO;
using System.Threading;

namespace Yuyuyui.PrivateServer.Desktop;

internal static class FileSystemData
{
    public const string PLAYER_DATA_FOLDER = "PlayerData";
    public const string PLAYER_DATA_FILE = "players.dat";

    public static string BASE_DIR => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YuyuyuiPrivateServer");

    public static string dataFolder = "";
    public static SemaphoreSlim dataFileLock = new(1, 1);

    static FileSystemData()
    {
        dataFileLock.Wait();
        try
        {
            dataFolder = Utils.EnsureDirectory(Path.Combine(BASE_DIR, PLAYER_DATA_FOLDER));
        }
        finally
        {
            dataFileLock.Release();
        }
    }
}