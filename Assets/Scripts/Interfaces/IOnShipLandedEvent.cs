using UnityEngine;

public interface IOnShipLandedEvent
{
    public enum LandingType
    {
        Success, Crash, OutOfBounds
    }

    public event OnShipLandedDelegate OnShipLandedEvent;

    public delegate void OnShipLandedDelegate(Vector2 landingPosition, LandingType landingType);
}

