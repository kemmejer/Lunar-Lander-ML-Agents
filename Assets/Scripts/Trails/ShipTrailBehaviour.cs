using UnityEngine;

public class ShipTrailBehaviour : MonoBehaviour
{
    private TrailRenderer _trailRenderer;

    /// <summary>
    /// Sets the color of the ships trail
    /// </summary>
    /// <param name="color">Color of the trail</param>
    public void SetColor(Color color)
    {
        if (!_trailRenderer)
            _trailRenderer = GetComponent<TrailRenderer>();

        _trailRenderer.startColor = color;
        _trailRenderer.endColor = color;
    }
}
