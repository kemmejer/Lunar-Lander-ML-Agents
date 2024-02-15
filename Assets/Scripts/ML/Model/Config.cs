using System;
using System.IO;

public class Config
{
    public string Name { get; private set; }

    private string ConfigBasePath => Path.Combine(Constants.ConfigPath, Name);

    public Config(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Loads all configurations
    /// </summary>
    public void Load()
    {
        try
        {
            TrainingSO.Load(ConfigBasePath);
            RayCasterSO.Load(ConfigBasePath);
            PlayerSpawnerSO.Load(ConfigBasePath);
            ShipParameterSO.Load(ConfigBasePath);
            GroundGeneratorSO.Load(ConfigBasePath);
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// Saves all configurations
    /// </summary>
    public void Save()
    {
        TrainingSO.Save(ConfigBasePath);
        RayCasterSO.Save(ConfigBasePath);
        PlayerSpawnerSO.Save(ConfigBasePath);
        ShipParameterSO.Save(ConfigBasePath);
        GroundGeneratorSO.Save(ConfigBasePath);
    }
}
