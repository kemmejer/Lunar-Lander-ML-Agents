using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Barracuda;
using Unity.Barracuda.Editor;
using Unity.Barracuda.ONNX;
using UnityEngine;

public class AgentModel
{
    public string Name { get; private set; }
    public NNModel Model { get; private set; } = null;

    const string ModelFileExtension = ".onnx";

    public AgentModel(string name)
    {
        Name = name;
    }

    public bool Load()
    {
        UnityEngine.Object.Destroy(Model);
        Model = null;

        var modelPath = GetModelPath();
        if (modelPath == null)
            return false;

        var converter = new ONNXModelConverter(true);
        var model = converter.Convert(modelPath);
        var modelData = ScriptableObject.CreateInstance<NNModelData>();

        using (var memoryStream = new MemoryStream())
        using (var writer = new BinaryWriter(memoryStream))
        {
            ModelWriter.Save(writer, model);
            modelData.Value = memoryStream.ToArray();
        }

        modelData.name = "Data";
        modelData.hideFlags = HideFlags.HideInHierarchy;

        Model = ScriptableObject.CreateInstance<NNModel>();
        Model.modelData = modelData;
        Model.name = Constants.AgentName;

        return true;
    }

    private string GetModelPath()
    {
        var modelFolderPath = Path.Combine(Constants.ModelPath, Name, Constants.AgentName);
        if (!Directory.Exists(modelFolderPath)) 
            return null;

        var files = Directory.GetFiles(modelFolderPath, "*" + ModelFileExtension);
        if (!files.Any())
            return null;

        var numbers = files.Select(file => int.Parse(Regex.Match(file, @"(\d+)\" + ModelFileExtension).Groups[1].Value)).ToArray();
        var maxIndex = Array.IndexOf(numbers, numbers.Max());

        return files[maxIndex];
    }
}
