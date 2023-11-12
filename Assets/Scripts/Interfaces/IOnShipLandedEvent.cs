using UnityEngine;

public interface IOnShipLandedEvent
{
    public event OnShipLandedDelegate OnShipLandedEvent;

    public delegate void OnShipLandedDelegate(Vector2 landingPosition);
}

