﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace Yuyuyui.PrivateServer;

public static class Config
{
    public class ConfigObject
    {
        public readonly General General = new();
    }
    public class General
    {
        public string Language { get; set; } = "";
    }


    public static ConfigObject Get()
    {
        return instance;
    }


    static Config()
    {
        instance = new ConfigObject();

        if (File.Exists(GetFileName())) return;
        var serializer = new Serializer();
        File.WriteAllText(GetFileName(), serializer.Serialize(instance));
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
        File.WriteAllText(GetFileName(), serializer.Serialize(instance));
    }


    private static ConfigObject instance;

    private const string FILE_NAME = "config.yaml";

    private static string GetFileName()
    {
        return FILE_NAME;
    }

    public static readonly List<string> SupportedInterfaceLocale = new()
    {
        "en",
        "zh"
    };

    public static readonly List<string> SupportedInGameScenarioLanguage = new()
    {
        "jp",
        "zh",
        "en"
    };
}