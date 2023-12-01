using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Config
{
    public string Name {  get; private set; }

    private string ConfigBasePath => Path.Combine(Constants.ConfigPath, Name);

    public Config(string name)
    {
        Name = name;
    }

    public void Load()
    {
        TrainingSO.Load(ConfigBasePath);
        RayCasterSO.Load(ConfigBasePath);
        PlayerSpawnerSO.Load(ConfigBasePath);
        ShipParameterSO.Load(ConfigBasePath);
        GroundGeneratorSO.Load(ConfigBasePath);
    }

    public void Save()
    {
        TrainingSO.Save(ConfigBasePath);
        RayCasterSO.Save(ConfigBasePath);
        PlayerSpawnerSO.Save(ConfigBasePath);
        ShipParameterSO.Save(ConfigBasePath);
        GroundGeneratorSO.Save(ConfigBasePath);
    }
}
