using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ObservationNormalizer
{
    private static Vector2 _minScreenPos;
    private static Vector2 _maxScreenPos;
    private static Vector2 _screenDimensions;
    private static float _screenDiagonal;

    private const float MaxVelocity = 6.0f;

    public static void Init()
    {
        var screenBounds = CameraHelper.GetScreenBounds();
        _minScreenPos = screenBounds.min;
        _maxScreenPos = screenBounds.max;
        _screenDimensions = screenBounds.max - screenBounds.min;
        _screenDiagonal = Mathf.Sqrt(Mathf.Pow(_screenDimensions.x, 2.0f) + Mathf.Pow(_screenDimensions.y, 2.0f));
    }

    /// <summary>
    /// Normalizes the provided position relative to the screen dimensions in a [0,1] range
    /// </summary>
    /// <param name="position">Position inside the screen dimensions</param>
    /// <returns>Normalized position in a [0,1] range</returns>
    public static Vector2 NormalizeScreenPosition(in Vector2 position)
    {
        return (position - _minScreenPos) / _screenDimensions;
    }

    /// <summary>
    /// Normalizes the provided velocity relative to a experimentally determined constant
    /// </summary>
    /// <param name="velocity">Velocity, which should not be higher than MaxVelocity</param>
    /// <returns>Normalized velocity in a [-1,1] range if MaxVelocity is not exceeded</returns>
    public static Vector2 NormalizeVelocity(in Vector2 velocity)
    {
        return velocity / MaxVelocity;
    }

    /// <summary>
    /// Normalizes the provided euler angle in a [-1,1] range
    /// </summary>
    /// <param name="angle">Euler angle bettween range [0,360]</param>
    /// <returns>Normalized angle in a [-1,1] range </returns>
    public static float NormalizeEulerAngle(float angle)
    {
        return angle > 180.0f ? (angle - 360.0f) / 180.0f : angle / 180.0f;
    }

    /// <summary>
    /// Normalizes the provided distance in a [0,1] range using the screen diagonal
    /// </summary>
    /// <param name="distance">Distance inside the screen bounds</param>
    /// <returns>Normalized distance in a [0,1] range</returns>
    public static float NormalizeRayCastDistance(float distance)
    {
        return distance / _screenDiagonal;
    }
}
