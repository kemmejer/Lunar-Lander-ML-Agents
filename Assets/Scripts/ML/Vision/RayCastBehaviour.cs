using UnityEngine;

public class RayCastBehaviour : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField] private LayerMask _layerMask;
    private Vector2 _direction = Vector2.down;
    private float _castDistance;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;

        HideRay();
    }

    /// <summary>
    /// Sets the ray cast direction
    /// </summary>
    /// <param name="direction">Direction of the ray cast</param>
    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
    }

    /// <summary>
    /// Sets the maximum ray cast distance
    /// </summary>
    /// <param name="distance">Maximum ray cast distance</param>
    public void SetCastDistance(float distance)
    {
        _castDistance = distance;
    }

    /// <summary>
    /// Casts a ray and checks for collision
    /// </summary>
    /// <returns>Result of the ray cast including the distance</returns>
    public RaycastHit2D CastRay()
    {
        var origin = gameObject.transform.position;
        var rayHit = Physics2D.Raycast(origin, _direction, _castDistance, _layerMask);

        if (RayCasterSO.drawRays && rayHit.collider)
            DrawRay(origin, rayHit.point);
        else
            HideRay();

        return rayHit;
    }

    /// <summary>
    /// Sets the color of the ray cast when rendering the ray
    /// </summary>
    /// <param name="color">Color of the ray cast</param>
    public void SetColor(Color color)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    /// <summary>
    /// Draws a line for the ray cast
    /// </summary>
    /// <param name="origin">Startpoint of the ray cast</param>
    /// <param name="rayHit">Endpoint of the ray cast</param>
    private void DrawRay(in Vector3 origin, in Vector3 rayHit)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, origin);
        _lineRenderer.SetPosition(1, rayHit);
    }

    /// <summary>
    /// Disables the rendering of the ray cast
    /// </summary>
    private void HideRay()
    {
        _lineRenderer.enabled = false;
    }
}
