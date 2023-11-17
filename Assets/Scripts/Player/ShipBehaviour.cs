using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static IOnShipLandedEvent;

public class ShipBehaviour : MonoBehaviour, IOnShipLandedEvent
{
    public event OnShipLandedDelegate OnShipLandedEvent;

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
        if (gameObject.activeInHierarchy)
            OnShipLanded(LandingType.OutOfBounds, 0.0f, Vector2.zero);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        float landingDeltaAngle;
        Vector2 landingVelocity;
        bool angleOk = IsCollisionAngleSmallEnough(collision, out landingDeltaAngle);
        bool velocityOk = IsCollisionVelocitySmallEnough(collision, out landingVelocity);

        if (angleOk && velocityOk)
        {
            OnShipLanded(LandingType.Success, landingDeltaAngle, landingVelocity);
        }
        else
        {
            OnShipLanded(LandingType.Crash, landingDeltaAngle, landingVelocity);
        }

        Debug.Log(string.Format("AngleOk: {0}, VelocityOk: {1}, Angle: {2}, Velocity: {3}", angleOk, velocityOk, landingDeltaAngle, landingVelocity));
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

        if (GetEulerRotation() < -Constants.MaxShipAngle)
        {
            var rotation = gameObject.transform.eulerAngles;
            gameObject.transform.eulerAngles = new Vector3(rotation.x, rotation.y, -Constants.MaxShipAngle);
        }
    }

    public void RotateLeft()
    {
        gameObject.transform.Rotate(0.0f, 0.0f, ShipParameterSO.controlParameter.rotationSpeed.value);

        if (GetEulerRotation() > Constants.MaxShipAngle)
        {
            var rotation = gameObject.transform.eulerAngles;
            gameObject.transform.eulerAngles = new Vector3(rotation.x, rotation.y, Constants.MaxShipAngle);
        }
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

    public Vector2 GetPosition()
    {
        return gameObject.transform.position;
    }

    public float GetEulerRotation()
    {
        var eulerAngle = gameObject.transform.eulerAngles.z;
        return eulerAngle > 180.0f ? eulerAngle - 360.0f : eulerAngle;
    }

    public Vector2 GetUpFacingShipNormal()
    {
        return gameObject.transform.rotation * Vector2.up;
    }

    public Vector2 GetVelocity()
    {
        return _rigidBody.velocity;
    }

    private void OnShipLanded(LandingType landingType, float landingDeltaAngle, in Vector2 landingVelocity)
    {
        if (landingType == LandingType.Crash)
            AnimationSystem.GetInstance().PlayExplosionAt(GetPosition());

        if (landingType != LandingType.Success)
            gameObject.SetActive(false);

        var trail = gameObject.transform.Find(TrailManager.TrailName).gameObject;
        TrailManager.GetInstance().MoveTrailToTrailManager(trail);

        var landingData = new LandingData() {
            type = landingType,
            position = GetPosition(),
            velocity = landingVelocity,
            groundDeltaAngle = landingDeltaAngle };

        OnShipLandedEvent?.Invoke(landingData);
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

    private bool IsCollisionAngleSmallEnough(Collision2D collision, out float landingDeltaAngle)
    {
        var surfaceNormal = collision.GetContact(0).normal;
        var shipNormal = GetUpFacingShipNormal();
        landingDeltaAngle = Vector2.Angle(surfaceNormal, shipNormal);

        return Mathf.Abs(landingDeltaAngle) < ShipParameterSO.landing.maxAngle.value;
    }

    private bool IsCollisionVelocitySmallEnough(Collision2D collision, out Vector2 landingVelocity)
    {
        landingVelocity = collision.GetContact(0).relativeVelocity;
        float velocity = landingVelocity.magnitude;

        return velocity < ShipParameterSO.landing.maxVelocity.value;
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
