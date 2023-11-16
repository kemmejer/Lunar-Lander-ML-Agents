using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour, IOnShipLandedEvent
{
    public event IOnShipLandedEvent.OnShipLandedDelegate OnShipLandedEvent;

    public ShipParameterSO ShipParameterSO { get; private set; }

    [SerializeField] private GameObject _shipThruster;
    [SerializeField] private GameObject _fuelBar;
    [SerializeField] private GameObject _velocityIndicator;

    private Rigidbody2D _rigidBody;

    void Awake()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        UpdateVelocityIndicator();
    }

    private void OnBecameInvisible()
    {
        if(gameObject.activeInHierarchy)
            OnShipLanded(IOnShipLandedEvent.LandingType.OutOfBounds);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        bool angleOk = IsCollisionAngleSmallEnough(collision);
        bool velocityOk = IsCollisionVelocitySmallEnough(collision);

        if (angleOk && velocityOk)
        {
            OnShipLanded(IOnShipLandedEvent.LandingType.Success);
        }
        else
        {
            OnShipLanded(IOnShipLandedEvent.LandingType.Crash);
        }

        Debug.Log(string.Format("AngleOk: {0}, VelocityOk: {1}", angleOk, velocityOk));
    }

    public void InitShip()
    {
        ShipParameterSO = ShipParameterSO.GetInstanceCopy();
        UpdateShipPhysics();
        _shipThruster.SetActive(false);
        ResetFuel();
        SetRandomComponentColor();
    }

    public void RotateRight()
    {
        gameObject.transform.Rotate(0.0f, 0.0f, -ShipParameterSO.controlParameter.rotationSpeed.value);

        var rotation = gameObject.transform.localEulerAngles;
        var zRotation = rotation.z > 180 ? rotation.z - 360 : rotation.z;
        if (zRotation < -90.0f)
            gameObject.transform.localEulerAngles = new Vector3(rotation.x, rotation.y, 270.0f);
    }

    public void RotateLeft()
    {
        gameObject.transform.Rotate(0.0f, 0.0f, ShipParameterSO.controlParameter.rotationSpeed.value);

        var rotation = gameObject.transform.localEulerAngles;
        var zRotation = rotation.z > 180 ? rotation.z - 360 : rotation.z;
        if (zRotation > 90.0f)
            gameObject.transform.localEulerAngles = new Vector3(rotation.x, rotation.y, 90.0f);
    }

    public void Thrust()
    {
        if (ShipParameterSO.fuel.remainingFuel.value == 0)
        {
            StopThrust();
            return;
        }

        _rigidBody.AddForce(transform.up * ShipParameterSO.controlParameter.thrustAmount.value, ForceMode2D.Force);
        _shipThruster.SetActive(true);
        UseFuel();
    }

    public void StopThrust()
    {
        _shipThruster.SetActive(false);
    }

    private void OnShipLanded(IOnShipLandedEvent.LandingType landingType)
    {
        if (landingType == IOnShipLandedEvent.LandingType.Crash)
            AnimationSystem.GetInstance().PlayExplosionAt(GetPosition());

        if(landingType != IOnShipLandedEvent.LandingType.Success)
            gameObject.SetActive(false);

        var trail = gameObject.transform.Find(TrailManager.TrailName).gameObject;
        TrailManager.GetInstance().MoveTrailToTrailManager(trail);

        OnShipLandedEvent?.Invoke(gameObject.transform.position, landingType);
    }

    private void UpdateShipPhysics()
    {
        var shipPhysics = ShipParameterSO.physics;

        _rigidBody.mass = shipPhysics.mass.value;
        _rigidBody.drag = shipPhysics.drag.value;
        _rigidBody.angularDrag = shipPhysics.angularDrag.value;
        _rigidBody.gravityScale = shipPhysics.gravityScale.value;
    }

    private void UseFuel()
    {
        ShipParameterSO.fuel.UseFuel();

        var scale = _fuelBar.transform.localScale;
        var xScale = ShipParameterSO.fuel.remainingFuel.value / ShipParameterSO.fuel.maxFuel.value;
        _fuelBar.transform.localScale = new Vector3(xScale, scale.y, scale.z);
    }

    private void UpdateVelocityIndicator()
    {
        bool isVelocityOk = GetVelocity().magnitude < ShipParameterSO.landing.maxVelocity.value;

        if (isVelocityOk)
            _velocityIndicator.GetComponent<Renderer>().material.color = Color.green;
        else
            _velocityIndicator.GetComponent<Renderer>().material.color = Color.red;
    }

    private void ResetFuel()
    {
        ShipParameterSO.fuel.remainingFuel.value = ShipParameterSO.fuel.maxFuel.value;

        var scale = _fuelBar.transform.localScale;
        _fuelBar.transform.localScale = new Vector3(1.0f, scale.y, scale.z);
    }

    private bool IsCollisionAngleSmallEnough(Collision2D collision)
    {
        var surfaceNormal = collision.GetContact(0).normal;
        var shipNormal = GetUpFacingShipNormal();
        var angle = Vector2.Angle(surfaceNormal, shipNormal);

        Debug.Log(string.Format("Landing angle: {0}", angle));

        return Mathf.Abs(angle) < ShipParameterSO.landing.maxAngle.value;
    }

    private bool IsCollisionVelocitySmallEnough(Collision2D collision)
    {
        var velocity2D = collision.GetContact(0).relativeVelocity;
        var velocity = velocity2D.magnitude;

        Debug.Log(string.Format("Landing velocity: {0}", velocity));

        return velocity < ShipParameterSO.landing.maxVelocity.value;
    }

    private Vector2 GetPosition()
    {
        return gameObject.transform.position;
    }

    private Vector2 GetUpFacingShipNormal()
    {
        return gameObject.transform.rotation * Vector2.up;
    }

    private Vector2 GetVelocity()
    {
        return _rigidBody.velocity;
    }

    private void SetRandomComponentColor()
    {
        var hue = Random.value;
        Color trailColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
        var shipTrail = gameObject.GetComponentInChildren<ShipTrailBehaviour>();
        shipTrail.SetColor(trailColor);

        Color rayColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
        rayColor.a = 0.1f;
        var rayCaster = gameObject.GetComponentInChildren<RayCasterBehaviour>();
        rayCaster.SetColor(rayColor);
    }

}
