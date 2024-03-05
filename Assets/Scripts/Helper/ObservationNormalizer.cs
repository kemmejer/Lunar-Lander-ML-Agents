using UnityEngine;

public static class ObservationNormalizer
{
    private static Vector2 _minScreenPos;
    private static Vector2 _maxScreenPos;
    private static Vector2 _screenDimensions;

    private const float MaxVelocity = 6.0f;

    /// <summary>
    /// Initializes the screen bounds for normalization
    /// </summary>
    public static void Init()
    {
        var screenBounds = CameraHelper.GetScreenBounds();
        _minScreenPos = screenBounds.min;
        _maxScreenPos = screenBounds.max;
        _screenDimensions = screenBounds.max - screenBounds.min;
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
    /// <param name="angle">Euler angle between a [-Constants.MaxShipAngle,Constants.MaxShipAngle] range</param>
    /// <returns>Normalized angle in a [-1,1] range </returns>
    public static float NormalizeEulerAngle(float angle)
    {
        if(Mathf.Abs(angle) > 90.0f)
        {
            Debug.Log(angle);
            int a = 0;
        }
        return angle / Constants.MaxShipAngle;
    }

    /// <summary>
    /// Normalizes the provided distance in a [0,1] range using the screen height
    /// </summary>
    /// <param name="distance">Distance inside the screen bounds</param>
    /// <returns>Normalized distance in a [0,1] range</returns>
    public static float NormalizeRayCastDistance(float distance)
    {
        return distance / _screenDimensions.y;
    }
}
