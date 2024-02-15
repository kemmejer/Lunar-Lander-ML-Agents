using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class ConfigManager
{
    public static List<Config> Configs { get; private set; }
    public static Config CurrentConfig { get; private set; }
    public static AgentModel CurrentModel { get; private set; }

    /// <summary>
    /// Initializes the config manager and tries to load the default config
    /// </summary>
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

    /// <summary>
    /// Tries to load the specified config by name from the config folder
    /// </summary>
    /// <param name="name">Name of the config to load</param>
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

        var model = new AgentModel(CurrentConfig.Name);
        CurrentModel = model.Load() ? model : null;
    }

    /// <summary>
    /// Save the current config in the config folder
    /// </summary>
    /// <param name="name">Name of the config to save</param>
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

    /// <summary>
    /// Deletes the config using the provided name
    /// </summary>
    /// <param name="name">Name of the config to delete</param>
    public static void DeleteConfig(string name)
    {
        if (name == Constants.DefaultConfigName)
            return;

        string configFolderPath = Path.Combine(Constants.ConfigPath, name);
        if (Directory.Exists(configFolderPath))
            Directory.Delete(configFolderPath, true);

        Configs.RemoveAll(config => config.Name == name);
    }

    /// <summary>
    /// Unloads the currently loaded model
    /// </summary>
    public static void UnloadModel()
    {
        CurrentModel = null;
    }

    /// <summary>
    /// Loads the current model from the config folder
    /// </summary>
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
