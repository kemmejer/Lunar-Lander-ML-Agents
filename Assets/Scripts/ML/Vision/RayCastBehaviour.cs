using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastBehaviour : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    [SerializeField] private LayerMask _layerMask;
    private Vector2 _direction = Vector2.down;

    void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;

        HideRay();
    }

    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
    }

    public void CastRay(bool drawRay = false)
    {
        var origin = gameObject.transform.position;
        var rayHit = Physics2D.Raycast(origin, _direction, 100, _layerMask);

        if (drawRay && rayHit.collider)
            DrawRay(origin, rayHit.point);
        else
            HideRay();
    }

    public void SetColor(Color color)
    {
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    private void DrawRay(in Vector3 origin, in Vector3 rayHit)
    {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, origin);
        _lineRenderer.SetPosition(1, rayHit);
    }

    private void HideRay()
    {
        _lineRenderer.enabled = false;
    }
}
