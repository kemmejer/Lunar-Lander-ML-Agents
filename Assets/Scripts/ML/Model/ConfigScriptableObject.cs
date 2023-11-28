using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class ConfigScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance;

    public static void Load(string folderPath)
    {
        InitInstance();

        var filePath = Path.Combine(folderPath, typeof(T).Name + ".json");

        if (!File.Exists(filePath))
        {
            Save(folderPath);
            return;
        }

        using var inputFile = new StreamReader(filePath);
        var data = inputFile.ReadToEnd();

        JsonUtility.FromJsonOverwrite(data, _instance);

        GenerateRandomValues();
    }

    public static void Save(string folderPath)
    {
        var filePath = Path.Combine(folderPath, typeof(T).Name + ".json");
        var data = JsonUtility.ToJson(_instance);

        Directory.CreateDirectory(folderPath);
        using var outputFile = new StreamWriter(filePath);
        outputFile.Write(data);
    }

    public static T GetInstance()
    {
        return _instance;
    }

    public static T GetInstanceCopy()
    {
        var instance = Instantiate(_instance);
        IRandomValue.GenerateValuesForAllFields(instance);

        return instance;
    }

    public static void GenerateRandomValues()
    {
        IRandomValue.GenerateValuesForAllFields(_instance);
    }

    private static void InitInstance()
    {
        if (_instance)
            return;

        var scriptableObjectPath = Path.Combine(typeof(T).Name);
        var scriptableObject = Resources.Load<T>(scriptableObjectPath);

        _instance = Instantiate(scriptableObject);
    }
}
