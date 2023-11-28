using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConfigManager
{
    public static List<Config> Configs { get; private set; }
    public static Config CurrentConfig { get; private set; }

    private static bool _initialized;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        if (_initialized)
            return;

        _initialized = true;

        var defaultConfig = new Config("Default");
        defaultConfig.Load();

        Configs = new List<Config>
        {
            defaultConfig
        };
    }

    public static void LoadConfig(string name)
    {
        var config = Configs.Find(conf => conf.Name == name);

        if (config == null)
            return;

        config.Load();
        CurrentConfig = config;
    }

    public static void SaveConfig(string name)
    {
        var config = Configs.Find(conf => conf.Name == name);

        if (config == null)
        {
            config = new Config(name);
            Configs.Add(config);
        }

        config.Save();
    }
}
