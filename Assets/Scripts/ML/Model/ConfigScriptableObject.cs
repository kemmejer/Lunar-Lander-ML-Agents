using System;
using System.IO;
using UnityEngine;
///
[Serializable]
public class ConfigScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T _instance;

    /// <summary>
    /// Loads a single config object from the specified folder and randomly initializes the object
    /// </summary>
    /// <param name="folderPath">Path of the config folder</param>
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

    /// <summary>
    /// Saves the config object in the specified folder
    /// </summary>
    /// <param name="folderPath">Folder to save the config in</param>
    public static void Save(string folderPath)
    {
        var filePath = Path.Combine(folderPath, typeof(T).Name + ".json");
        var data = JsonUtility.ToJson(_instance);

        Directory.CreateDirectory(folderPath);
        using var outputFile = new StreamWriter(filePath);
        outputFile.Write(data);
    }

    /// <summary>
    /// Returns the current instance of the config
    /// </summary>
    /// <returns>Instance of the current config</returns>
    public static T GetInstance()
    {
        return _instance;
    }

    /// <summary>
    /// Creates a copy of the current instance of the config and returns the copy
    /// </summary>
    /// <returns>Copy of the instance of the current config</returns>
    public static T GetInstanceCopy()
    {
        var instance = Instantiate(_instance);
        IRandomValue.GenerateValuesForAllFields(instance);

        return instance;
    }

    /// <summary>
    /// Generates random values for all IRandomValue fields in the config
    /// </summary>
    public static void GenerateRandomValues()
    {
        IRandomValue.GenerateValuesForAllFields(_instance);
    }

    /// <summary>
    /// Initializes the instance of the config scriptable object
    /// </summary>
    private static void InitInstance()
    {
        if (_instance)
            return;

        var scriptableObjectPath = Path.Combine(typeof(T).Name);
        var scriptableObject = Resources.Load<T>(scriptableObjectPath);

        _instance = Instantiate(scriptableObject);
    }
}
