using ImGuiNET;
using ImPlotNET;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;

public static class VisualizationLogger
{
    private static StatsRecorder _statsRecorder;

    public enum GraphName
    {
        Reward
    }

    static VisualizationLogger()
    {
        _statsRecorder = Academy.Instance.StatsRecorder;
    }

    public static void AddValue(GraphName name, float value, StatAggregationMethod method = StatAggregationMethod.Average)
    {
        _statsRecorder.Add(name.ToString(), value, method);
    }
}
