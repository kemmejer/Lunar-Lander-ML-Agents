using UnityEngine;


public static class CameraHelper
{
    public static Bounds GetScreenBounds()
    {
        var lowerLeft = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
        var upperRight = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));

        var bounds = new Bounds();
        bounds.SetMinMax(lowerLeft, upperRight);

        return bounds;
    }
}
