using UnityEngine;

public class ShipTrailBehaviour : MonoBehaviour
{
    private TrailRenderer _trailRenderer;

    public void SetColor(Color color)
    {
        if (!_trailRenderer)
            _trailRenderer = GetComponent<TrailRenderer>();

        _trailRenderer.startColor = color;
        _trailRenderer.endColor = color;
    }
}
