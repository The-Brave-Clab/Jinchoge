using System;
using System.IO;
using System.Threading;

namespace Yuyuyui.PrivateServer.Desktop;

internal static class FileSystemData
{
    public const string PLAYER_DATA_FOLDER = "PlayerData";
    public const string PLAYER_DATA_FILE = "players.dat";

    public const string LOCAL_DATA_FOLDER = "Resources";
    public const string LOCAL_DATA_VERSION_FILE = "master_data.version.json";

    public static string BASE_DIR => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YuyuyuiPrivateServer");

    public static readonly string dataFolder;
    public static readonly SemaphoreSlim dataFileLock = new(1, 1);

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