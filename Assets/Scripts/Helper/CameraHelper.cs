using UnityEngine;


public static class CameraHelper
{
    /// <summary>
    /// Returns the screen bounds in world coordinates
    /// </summary>
    /// <returns>Screen bounds in world coordinates</returns>
    public static Bounds GetScreenBounds()
    {
        var lowerLeft = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, 0.0f));
        var upperRight = Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 1.0f, 0.0f));

        var bounds = new Bounds();
        bounds.SetMinMax(lowerLeft, upperRight);

        return bounds;
    }
}
