using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public static class VisualizationLogger
{
    private static int _successfulLandings;
    private static int _failedLandings;
    private static StatsRecorder _statsRecorder;
    private static readonly Dictionary<GraphName, string> GraphNameStrings = new Dictionary<GraphName, string> {
        { GraphName.SuccessRate, "Environment/Landing Success Rate" },
        { GraphName.SuccessfulLandings, "Environment/Successful Landings" },
        { GraphName.FailedLandings, "Environment/Failed Landings" }};

    public enum GraphName
    {
        SuccessRate,
        SuccessfulLandings,
        FailedLandings
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

    /// <summary>
    /// Initializes the logger and starts tge communication by sending the world bounds
    /// </summary>
    public static void Init()
    {
        _statsRecorder = Academy.Instance.StatsRecorder;

        SendImageWorldBounds();
        ResetLandingCounters();
    }

    /// <summary>
    /// Resets the success and fail landing counters
    /// </summary>
    public static void ResetLandingCounters()
    {
        _successfulLandings = 0;
        _failedLandings = 0;
    }

    /// <summary>
    /// Adds a successful landing
    /// </summary>
    public static void AddSuccessfulLanding()
    {
        _successfulLandings++;

        AddValue(GraphName.SuccessfulLandings, _successfulLandings, StatAggregationMethod.MostRecent);
    }

    /// <summary>
    /// Adds a failed landing
    /// </summary>
    public static void AddFailedLanding()
    {
        _failedLandings++;

        AddValue(GraphName.FailedLandings, _failedLandings, StatAggregationMethod.MostRecent);
    }

    /// <summary>
    /// Adds a value to a tensorboard graph
    /// </summary>
    /// <param name="name">Name of the graph</param>
    /// <param name="value">Value to add to the graph</param>
    /// <param name="method">Aggregation method for the values</param>
    public static void AddValue(GraphName name, float value, StatAggregationMethod method = StatAggregationMethod.Average)
    {
        var graphName = GraphNameStrings[name];
        _statsRecorder?.Add(graphName, value, method);
    }

    /// <summary>
    /// Adds a value to a tensorboard image
    /// </summary>
    /// <param name="name">Name of the image</param>
    /// <param name="value">Value to add to the image</param>
    public static void AddImageValue(ImageGraphName name, float value)
    {
        CollectImageValue(name, value);
    }

    /// <summary>
    /// Adds a value to a tensorboard image
    /// </summary>
    /// <param name="name">Name of the image</param>
    /// <param name="value">Value to add to the image</param>
    public static void AddImageValue(ImageGraphName name, in Vector2 value)
    {
        CollectImageValue(name, value.x);
        CollectImageValue(name, value.y);
    }

    /// <summary>
    /// Sends the world bounds to the training server for generating images
    /// </summary>
    public static void SendImageWorldBounds()
    {
        var bounds = CameraHelper.GetScreenBounds();
        CollectImageValue(ImageGraphName.WorldBounds, bounds.min.x);
        CollectImageValue(ImageGraphName.WorldBounds, bounds.min.y);
        CollectImageValue(ImageGraphName.WorldBounds, bounds.max.x);
        CollectImageValue(ImageGraphName.WorldBounds, bounds.max.y);
    }

    /// <summary>
    /// Sends an image value to the training server and tensorboard
    /// </summary>
    /// <param name="name">Name of the image</param>
    /// <param name="value">Value to add to the image</param>
    /// <param name="method">Aggregation method for the values</param>
    private static void CollectImageValue(ImageGraphName name, float value, StatAggregationMethod method = StatAggregationMethod.Average)
    {
        _statsRecorder?.Add(name.ToString(), value, method);
    }
}
