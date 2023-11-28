using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.WSA;

public static class ConfigManager
{
    public static List<Config> Configs { get; private set; }
    public static Config CurrentConfig { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        var defaultConfig = new Config(Constants.DefaultConfigName);
        defaultConfig.Load();
        CurrentConfig = defaultConfig;

        Configs = new List<Config>
        {
            defaultConfig
        };

        LoadConfigsFromDisk();
    }

    public static void LoadConfig(string name)
    {
        var config = Configs.Find(conf => conf.Name == name);

        if (config == null)
        {
            if (!Configs.Any())
                Init();

            return;
        }

        config.Load();
        CurrentConfig = config;
    }

    public static void SaveConfig(string name)
    {
        if (name == Constants.DefaultConfigName)
            return;

        var config = Configs.Find(conf => conf.Name == name);

        if (config == null)
        {
            config = new Config(name);
            Configs.Add(config);
        }

        config.Save();
    }

    public static void DeleteConfig(string name)
    {
        if (name == Constants.DefaultConfigName)
            return;

        string configFolderPath = Path.Combine(Constants.ConfigPath, name);
        if (Directory.Exists(configFolderPath))
            Directory.Delete(configFolderPath, true);

        Configs.RemoveAll(config => config.Name == name);
    }

    private static void LoadConfigsFromDisk()
    {
        if (!Directory.Exists(Constants.ConfigPath))
            return;

        var configDirs = Directory.GetDirectories(Constants.ConfigPath);
        foreach (var configDir in configDirs)
        {
            var configName = Path.GetFileName(configDir);

            if (Configs.Any(config => config.Name == configName))
                continue;

            var config = new Config(configName);
            Configs.Add(config);
        }
    }
}
