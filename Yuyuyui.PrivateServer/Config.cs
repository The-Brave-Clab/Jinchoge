﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace Yuyuyui.PrivateServer;

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
        public string UpdateBranch { get; set; } = "master";
    }

    public class InGame
    {
        public string ScenarioLanguage { get; set; } = SupportedInGameScenarioLanguage[0];
    }


    public static ConfigObject Get()
    {
        return instance;
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
        string content = File.ReadAllText(GetFileName(), Encoding.UTF8);
        var deserializer = new Deserializer();
        instance = deserializer.Deserialize<ConfigObject>(content);
    }

    public static void Save()
    {
        var serializer = new Serializer();
        File.WriteAllText(GetFileName(), serializer.Serialize(instance), Encoding.UTF8);
    }


    private static ConfigObject instance;

    private const string FILE_NAME = "config.yaml";

    private static string GetFileName()
    {
        return Path.Combine(PrivateServer.BASE_DIR, FILE_NAME);
    }

    public static readonly List<string> SupportedInterfaceLocale = new()
    {
        "",
        "en",
        "zh"
    };

    public static readonly List<string> SupportedUpdateChannel = new()
    {
        "master",
    };

    public static readonly List<string> SupportedInGameScenarioLanguage = new()
    {
        "ja",
        "zh",
        "en"
    };
}