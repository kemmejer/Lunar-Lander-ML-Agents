using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using UnityEngine;
using ImageGraphName = VisualizationLogger.ImageGraphName;


public class ImageVisualizationData
{
    public ImageGraphName GraphName { get; private set; }
    public List<float> Data { get; private set; } = new List<float>();
    public int Count { get; private set; }

    public ImageVisualizationData(ImageGraphName graphName)
    {
        GraphName = graphName;
    }

    public int AddFloat(float value)
    {
        Data.Add(value);

        return ++Count;
    }

    public int AddVec(in Vector2 value)
    {
        Data.AddRange(new float[] { value.x, value.y });

        return ++Count;
    }

    public void Clear()
    {
        Data.Clear();
        Count = 0;
    }
}

