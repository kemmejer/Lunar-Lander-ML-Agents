using UnityEngine;

public interface IOnShipLandedEvent
{
    public enum LandingType
    {
        Success, Crash, OutOfBounds
    }

    struct LandingData
    {
        public LandingType type; // Landing type
        public Vector2 position; // Position of the landing
        public Vector2 velocity; // Velocity of the landing
        public float groundDeltaAngle; // Angle between the ground and the ship rotation
    }

    public event OnShipLandedDelegate OnShipLandedEvent;

    public delegate void OnShipLandedDelegate(in LandingData landingData);
}

