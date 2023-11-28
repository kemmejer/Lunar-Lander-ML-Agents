using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Config
{
    public string Name {  get; private set; }

    private string ConfigResourceBasePath => Path.Combine(Constants.ConfigPath, Name);

    public Config(string name)
    {
        Name = name;
    }

    public void Load()
    {
        TrainingSO.Load(ConfigResourceBasePath);
        RayCasterSO.Load(ConfigResourceBasePath);
        PlayerSpawnerSO.Load(ConfigResourceBasePath);
        ShipParameterSO.Load(ConfigResourceBasePath);
        GroundGeneratorSO.Load(ConfigResourceBasePath);
    }

    public void Save()
    {
        TrainingSO.Save(ConfigResourceBasePath);
        RayCasterSO.Save(ConfigResourceBasePath);
        PlayerSpawnerSO.Save(ConfigResourceBasePath);
        ShipParameterSO.Save(ConfigResourceBasePath);
        GroundGeneratorSO.Save(ConfigResourceBasePath);
    }
}
