using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
//[DebuggerDisplay("Value = {value}")]
public class RandomValue
{
    /// <summary>
    /// Random value generated by a GenerateRandomValue call
    /// Can be used for read and write operations
    /// </summary>
    [HideInInspector]
    public float value;

    /// <summary>
    /// x = Mean
    /// y = Deviation
    /// </summary>
    public Vector2 parameter; // x = Mean, y = Deviation

    /// <summary>
    /// Returns a random value each time is is beeing accessed
    /// </summary>
    public float RndValue
    {
        get
        {
            GenerateRandomValue();
            return value;
        }
    }

    public ref float Mean => ref parameter.x;
    public ref float Deviation => ref parameter.y;

    public float Max => Mean + Deviation;
    public float Min => Mean - Deviation;

    /// <summary>
    /// Generates a random value around the mean value using: mean +- deviation
    /// Cannot generate negative numbers
    /// </summary>
    public void GenerateRandomValue()
    {
        value = Math.Max(UnityEngine.Random.Range(Min, Max), 0.0f);
    }

    /// <summary>
    /// Iterates over all RandomValue fields of the provided object and generates random values
    /// </summary>
    /// <typeparam name="T">Type of the class</typeparam>
    /// <param name="obj">Instance of the class</param>
    public static void GenerateValuesForAllFields<T>(T obj)
    {
        var fields = typeof(T).GetFields().Where(field => field.FieldType == typeof(RandomValue));
        foreach (var field in fields)
        {
            var randomValue = field.GetValue(obj) as RandomValue;
            randomValue.GenerateRandomValue();
        }
    }
}
