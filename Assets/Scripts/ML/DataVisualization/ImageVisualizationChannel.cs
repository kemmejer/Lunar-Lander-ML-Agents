﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using ImageGraphName = VisualizationLogger.ImageGraphName;


public class ImageVisualizationChannel : SideChannel
{
    public ImageVisualizationChannel()
    {
        ChannelId = new Guid("E6FC6161-7938-4C11-825B-56870D25E80F");
    }

    protected override void OnMessageReceived(IncomingMessage msg)
    {
        Debug.Log(msg);
    }

    public void SendData(in ImageVisualizationData data)
    {
        using var message = new OutgoingMessage();

        message.WriteInt32((int)data.GraphName);
        message.WriteFloatList(data.Data);

        QueueMessageToSend(message);
    }
}