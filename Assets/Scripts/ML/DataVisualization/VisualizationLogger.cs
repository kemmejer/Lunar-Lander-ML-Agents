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
    private static ImageVisualizationChannel _imageChannel;
    private static ImageVisualizationData[] _imageVisualizationData;
    private const int ImageBufferSize = 32768;

    public enum GraphName
    {
        SuccessRate,
    }

    public enum ImageGraphName
    {
        PositionImage, // float x, float y
        RotationImage, // float rotation (euler)
        VelocityImage, // float velocityX, float velocity
        RewardImage,   // float reward
        ThrustImage    // float thrust (0 = false, 1 = true)
    }

    public static void Init()
    {
        _statsRecorder = Academy.Instance.StatsRecorder;
        _imageChannel = new ImageVisualizationChannel();
        SideChannelManager.RegisterSideChannel(_imageChannel);

        int imageGraphCount = Enum.GetNames(typeof(ImageGraphName)).Length;
        _imageVisualizationData = new ImageVisualizationData[imageGraphCount];
        for (int i = 0; i < imageGraphCount; i++)
        {
            _imageVisualizationData[i] = new ImageVisualizationData((ImageGraphName)i);
        }
    }

    public static void UnInit()
    {
        SideChannelManager.UnregisterSideChannel(_imageChannel);
        _imageChannel = null;
    }

    public static void AddValue(ImageGraphName name, float value, StatAggregationMethod method = StatAggregationMethod.Average)
    {
        _statsRecorder.Add(name.ToString(), value, method);
    }

    public static void AddImageValue(ImageGraphName name, float value)
    {
        if (_imageVisualizationData[(int)name].AddFloat(value) > ImageBufferSize)
            SendImageData(name);
    }

    public static void AddImageValue(ImageGraphName name, in Vector2 value)
    {
        if (_imageVisualizationData[(int)name].AddVec(value) > ImageBufferSize)
            SendImageData(name);
    }

    public static void SendImageData(ImageGraphName graphName)
    {
        var imageData = _imageVisualizationData[(int)graphName];
        _imageChannel.SendData(imageData);
        imageData.Clear();
    }
}
