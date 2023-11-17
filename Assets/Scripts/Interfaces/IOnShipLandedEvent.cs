using UnityEngine;

public interface IOnShipLandedEvent
{
    public enum LandingType
    {
        Success, Crash, OutOfBounds
    }

    struct LandingData
    {
        public LandingType type;
        public Vector2 position;
        public Vector2 velocity;
        public float groundDeltaAngle;
    }

    public event OnShipLandedDelegate OnShipLandedEvent;

    public delegate void OnShipLandedDelegate(in LandingData landingData);
}

