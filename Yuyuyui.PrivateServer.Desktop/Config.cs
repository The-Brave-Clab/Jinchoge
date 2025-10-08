using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using YamlDotNet.Serialization;

namespace Yuyuyui.PrivateServer.Desktop;

public static class Config
{
    public class ConfigObject
    {
        public General General = new();
        public InGame InGame = new();
    }

    public class General
    {
        public string Language { get; set; } = "";
        public bool AutoCheckUpdate { get; set; } = true;
        public string UpdateBranch { get; set; } = 
            SupportedUpdateChannel.Contains(Update.LocalVersion.version_info.branch) ?
                Update.LocalVersion.version_info.branch : "master";
    }

    public class InGame
    {
        public string ScenarioLanguage { get; set; } = IInGameConfigProvider.SupportedInGameScenarioLanguage[0];
        public bool InfiniteItems { get; set; } = true;
        public bool UnlockAllDifficulties { get; set; } = false;
    }


    public static ConfigObject Get()
    {
        rwLock.EnterReadLock();
        try
        {
            return instance;
        }
        finally
        {
            rwLock.ExitReadLock();
        }
    }


    static Config()
    {
        instance = new ConfigObject();

        if (File.Exists(GetFileName()))
        {
            var deserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();
            instance = deserializer.Deserialize<ConfigObject>(File.ReadAllText(GetFileName(), Encoding.UTF8));
        }
        var serializer = new Serializer();
        File.WriteAllText(GetFileName(), serializer.Serialize(instance), Encoding.UTF8);
    }

    public static void Load()
    {
        rwLock.EnterWriteLock();
        try
        {
            string content = File.ReadAllText(GetFileName(), Encoding.UTF8);
            var deserializer = new Deserializer();
            instance = deserializer.Deserialize<ConfigObject>(content);
        }
        finally
        {
            rwLock.ExitWriteLock();
        }
    }

    public static void Save()
    {
        rwLock.EnterWriteLock();
        try
        {
            var serializer = new Serializer();
            File.WriteAllText(GetFileName(), serializer.Serialize(instance), Encoding.UTF8);
        }
        finally
        {
            rwLock.ExitWriteLock();
        }
    }


    private static ConfigObject instance;

    private const string FILE_NAME = "config.yaml";

    private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

    private static string GetFileName()
    {
        // To be deprecated
        string BASE_DIR = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
            "YuyuyuiPrivateServer");
        return Path.Combine(BASE_DIR, FILE_NAME);
    }

    public static readonly List<string> SupportedInterfaceLocale =
    [
        "",
        "en",
        "zh"
    ];

    public static readonly List<string> SupportedUpdateChannel =
    [
        "master",
        "release"
    ];
}