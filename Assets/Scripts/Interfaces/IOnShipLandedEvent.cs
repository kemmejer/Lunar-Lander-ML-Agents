using UnityEngine;

public interface IOnShipLandedEvent
{
    public enum LandingType
    {
        Success, Crash
    }

    public event OnShipLandedDelegate OnShipLandedEvent;

    public delegate void OnShipLandedDelegate(Vector2 landingPosition, LandingType landingType);
}

