using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.SideChannels;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class ImageVisualizationChannel : SideChannel
{
    public struct ImageDataFloat
    {
        float posX;
        float posY;
        float value;
    }

    public struct ImageDataVec
    {
        float posX;
        float posY;
        float vecX;
        float vecY;
    }

    public ImageVisualizationChannel()
    {
        ChannelId = new Guid("E6FC6161-7938-4C11-825B-56870D25E80F");
    }

    protected override void OnMessageReceived(IncomingMessage msg)
    {
        Debug.Log(msg);
    }

    public void SendData()
    {
        using var message = new OutgoingMessage();
        message.WriteString("Hallo");
        QueueMessageToSend(message);
    }
}
