using ImGuiNET;
using ImPlotNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using UnityEngine;

public static class VisualizationLogger
{
    private static StatsRecorder _statsRecorder;
    private static ImageVisualizationData[] _imageVisualizationData;
    private const int ImageBufferSize = 32768;

    public enum GraphName
    {
        SuccessRate,
    }

    public enum ImageGraphName
    {
        WorldBounds, // float minX, float minY, float maxX, float maxY
        Position,    // float x, float y
        Rotation,    // float rotation (euler)
        Velocity,    // float velocityX, float velocity
        Reward,      // float reward
        Thrust       // float thrust (0 = false, 1 = true)
    }

    public static void Init()
    {
        _statsRecorder = Academy.Instance.StatsRecorder;

        int imageGraphCount = Enum.GetNames(typeof(ImageGraphName)).Length;
        _imageVisualizationData = new ImageVisualizationData[imageGraphCount];
        for (int i = 0; i < imageGraphCount; i++)
        {
            _imageVisualizationData[i] = new ImageVisualizationData((ImageGraphName)i);
        }

        SendImageWorldBounds();
    }


    public static void AddValue(ImageGraphName name, float value, StatAggregationMethod method = StatAggregationMethod.Average)
    {
        _statsRecorder.Add(name.ToString(), value, method);
    }

    public static void AddImageValue(ImageGraphName name, float value)
    {
        CollectImageValue(name, value);
    }

    public static void AddImageValue(ImageGraphName name, in Vector2 value)
    {
        CollectImageValue(name, value.x);
        CollectImageValue(name, value.y);
    }

    public static void SendImageWorldBounds()
    {
        var bounds = CameraHelper.GetScreenBounds();
        var worldBoundsData = _imageVisualizationData[(int)ImageGraphName.WorldBounds];
        worldBoundsData.Data.AddRange(new float[] { bounds.min.x, bounds.min.y, bounds.max.x, bounds.max.y });

        CollectImageValue(ImageGraphName.WorldBounds, bounds.min.x);
        CollectImageValue(ImageGraphName.WorldBounds, bounds.min.y);
        CollectImageValue(ImageGraphName.WorldBounds, bounds.max.x);
        CollectImageValue(ImageGraphName.WorldBounds, bounds.max.y);
    }

    private static void CollectImageValue(ImageGraphName name, float value, StatAggregationMethod method = StatAggregationMethod.Average)
    {
        _statsRecorder.Add(name.ToString(), value, method);
    }
}
