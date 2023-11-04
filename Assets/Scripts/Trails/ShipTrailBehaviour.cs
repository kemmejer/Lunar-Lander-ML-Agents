using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipTrailBehaviour : MonoBehaviour
{

    TrailRenderer _trailRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _trailRenderer = GetComponent<TrailRenderer>();

        SetRandomColor();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetRandomColor()
    {
        Color color = Color.HSVToRGB(Random.value, 1.0f, 1.0f);

        _trailRenderer.startColor = color;
        _trailRenderer.endColor = color;
    }
}
