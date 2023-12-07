using ImGuiNET;
using ImPlotNET;
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

    public enum GraphName
    {
        SuccessRate,
        PositionImage,
        RotationImage,
        VelocityImage,
        RewardImage
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        _statsRecorder = Academy.Instance.StatsRecorder;
        _imageChannel = new ImageVisualizationChannel();
        SideChannelManager.RegisterSideChannel(_imageChannel);
        _imageChannel.SendData();
    }

    public static void UnInit()
    {
        SideChannelManager.UnregisterSideChannel(_imageChannel);
        _imageChannel = null;
    }

    public static void AddValue(GraphName name, float value, StatAggregationMethod method = StatAggregationMethod.Average)
    {
        _statsRecorder.Add(name.ToString(), value, method);
    }

    public static void AddImageValue(GraphName name, in Vector2 position, float value)
    {
    }

    public static void AddImageValue(GraphName name, in Vector2 position, Vector2 value)
    {

    }
}
