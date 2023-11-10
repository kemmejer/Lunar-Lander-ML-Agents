using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTrailBehaviour : MonoBehaviour
{

    TrailRenderer _trailRenderer;

    void Awake()
    {
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    public void SetColor(Color color)
    {
        _trailRenderer.startColor = color;
        _trailRenderer.endColor = color;
    }
}
