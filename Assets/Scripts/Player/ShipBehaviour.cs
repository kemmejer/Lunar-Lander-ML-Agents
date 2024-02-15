using UnityEngine;
using static IOnShipLandedEvent;

public class ShipBehaviour : MonoBehaviour, IOnShipLandedEvent
{
    public event OnShipLandedDelegate OnShipLandedEvent;

    public ShipParameterSO ShipParameterSO { get; private set; }
    public bool IsInitialized { get; private set; }

    [SerializeField] private GameObject _shipThruster;
    [SerializeField] private GameObject _fuelBar;
    [SerializeField] private GameObject _velocityIndicator;

    private bool _isThrusting;

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
        bool angleOk = IsCollisionAngleSmallEnough(collision, out float landingDeltaAngle);
        bool velocityOk = IsCollisionVelocitySmallEnough(collision, out Vector2 landingVelocity);

        if (angleOk && velocityOk)
        {
            OnShipLanded(LandingType.Success, landingDeltaAngle, landingVelocity);
        }
        else
        {
            OnShipLanded(LandingType.Crash, landingDeltaAngle, landingVelocity);
        }
    }

    /// <summary>
    /// Initializes the ships parameter
    /// </summary>
    public void InitShip()
    {
        ShipParameterSO = ShipParameterSO.GetInstanceCopy();
        UpdateShipPhysics();
        _shipThruster.SetActive(false);
        ResetFuel();
        SetShipActive(true);
        IsInitialized = true;
    }

    /// <summary>
    /// Rotates the ship to the right
    /// </summary>
    public void RotateRight()
    {
        gameObject.transform.Rotate(0.0f, 0.0f, -ShipParameterSO.controlParameter.rotationSpeed.value);

        if (GetEulerRotation() < -Constants.MaxShipAngle)
        {
            var rotation = gameObject.transform.eulerAngles;
            gameObject.transform.eulerAngles = new Vector3(rotation.x, rotation.y, -Constants.MaxShipAngle);
        }
    }

    /// <summary>
    /// Rotates the ship to the left
    /// </summary>
    public void RotateLeft()
    {
        gameObject.transform.Rotate(0.0f, 0.0f, ShipParameterSO.controlParameter.rotationSpeed.value);

        if (GetEulerRotation() > Constants.MaxShipAngle)
        {
            var rotation = gameObject.transform.eulerAngles;
            gameObject.transform.eulerAngles = new Vector3(rotation.x, rotation.y, Constants.MaxShipAngle);
        }
    }

    /// <summary>
    /// Enables the thrust of the ship
    /// </summary>
    public void Thrust()
    {
        if (ShipParameterSO.fuel.remainingFuel.value == 0)
        {
            StopThrust();
            return;
        }

        _isThrusting = true;

        _rigidBody.AddForce(transform.up * ShipParameterSO.controlParameter.thrustAmount.value, ForceMode2D.Force);
        _shipThruster.SetActive(true);
        UseFuel();
    }

    /// <summary>
    /// Stops the thrust of the ship
    /// </summary>
    public void StopThrust()
    {
        _isThrusting = false;
        _shipThruster.SetActive(false);
    }

    /// <summary>
    /// Returns the position of the ship
    /// </summary>
    /// <returns>Position of the ship</returns>
    public Vector2 GetPosition()
    {
        return gameObject.transform.position;
    }

    /// <summary>
    /// Rotation the rotation of the ship in euler angles
    /// </summary>
    /// <returns>Rotation in euler angles</returns>
    public float GetEulerRotation()
    {
        var eulerAngle = gameObject.transform.eulerAngles.z;
        return eulerAngle > 180.0f ? eulerAngle - 360.0f : eulerAngle;
    }

    /// <summary>
    /// Returns the up facing normal of the ship depending on the rotation of the ship
    /// </summary>
    /// <returns></returns>
    public Vector2 GetUpFacingShipNormal()
    {
        return gameObject.transform.rotation * Vector2.up;
    }

    /// <summary>
    /// Returns the velocity of the ship
    /// </summary>
    /// <returns>Velocity of the ship</returns>
    public Vector2 GetVelocity()
    {
        return _rigidBody.velocity;
    }

    /// <summary>
    /// Returns whether the ships speed exceeds the max velocity threshold or not
    /// </summary>
    /// <returns>True if the ship is too fast</returns>
    public bool IsShipTooFast()
    {
        return GetVelocity().magnitude >= ShipParameterSO.landing.maxVelocity.value;
    }

    /// <summary>
    /// Returns whether the ship is thrusting or not
    /// </summary>
    /// <returns>True if the ship is thrusting</returns>
    public bool IsShipThrusting()
    {
        return _isThrusting;
    }

    /// <summary>
    /// Enables or disables the ship gameobject
    /// </summary>
    /// <param name="active"></param>
    public void SetShipActive(bool active)
    {
        gameObject.SetActive(active);
    }

    /// <summary>
    /// Called when the ship has landed.
    /// Handles the landing of the ship and invokes the OnShipLandedEvent
    /// </summary>
    /// <param name="landingType">Type of the landing</param>
    /// <param name="landingDeltaAngle">Angle between the ground and the ship</param>
    /// <param name="landingVelocity">Velocity of the landing</param>
    private void OnShipLanded(LandingType landingType, float landingDeltaAngle, in Vector2 landingVelocity)
    {
        if (landingType == LandingType.Crash)
            AnimationSystem.GetInstance().PlayExplosionAt(GetPosition());

        if (landingType != LandingType.Success)
            SetShipActive(false);

        StopThrust();
        _rigidBody.velocity = Vector2.zero;

        var landingData = new LandingData()
        {
            type = landingType,
            position = GetPosition(),
            velocity = landingVelocity,
            groundDeltaAngle = landingDeltaAngle
        };

        OnShipLandedEvent?.Invoke(landingData);
    }

    /// <summary>
    /// Updates the ridigbody physics of the ship
    /// </summary>
    private void UpdateShipPhysics()
    {
        var shipPhysics = ShipParameterSO.physics;

        _rigidBody.mass = shipPhysics.mass.value;
        _rigidBody.drag = shipPhysics.drag.value;
        _rigidBody.gravityScale = shipPhysics.gravityScale.value;
        _rigidBody.freezeRotation = true;
    }

    /// <summary>
    /// Reduces the fuel of the ship
    /// </summary>
    private void UseFuel()
    {
        ShipParameterSO.fuel.UseFuel();

        var scale = _fuelBar.transform.localScale;
        var xScale = ShipParameterSO.fuel.remainingFuel.value / ShipParameterSO.fuel.maxFuel.value;
        _fuelBar.transform.localScale = new Vector3(xScale, scale.y, scale.z);
    }

    /// <summary>
    /// Updates the indicator whether the ship is too fast
    /// </summary>
    private void UpdateVelocityIndicator()
    {
        bool isShipTooFast = IsShipTooFast();

        if (isShipTooFast)
            _velocityIndicator.GetComponent<Renderer>().material.color = Color.red;
        else
            _velocityIndicator.GetComponent<Renderer>().material.color = Color.green;
    }

    /// <summary>
    /// Resets the fuel of the ship
    /// </summary>
    private void ResetFuel()
    {
        ShipParameterSO.fuel.remainingFuel.value = ShipParameterSO.fuel.maxFuel.value;

        var scale = _fuelBar.transform.localScale;
        _fuelBar.transform.localScale = new Vector3(1.0f, scale.y, scale.z);
    }

    /// <summary>
    /// Checks whether the landing angle is small enough for a save landing
    /// </summary>
    /// <param name="collision">Collision information of the 2d collision between the ship and the ground</param>
    /// <param name="landingDeltaAngle">[out] Angle between the ground and the ship on landing</param>
    /// <returns>True if the landing angle is small enough</returns>
    private bool IsCollisionAngleSmallEnough(Collision2D collision, out float landingDeltaAngle)
    {
        var surfaceNormal = collision.GetContact(0).normal;
        var shipNormal = GetUpFacingShipNormal();
        landingDeltaAngle = Vector2.Angle(surfaceNormal, shipNormal);

        return Mathf.Abs(landingDeltaAngle) < ShipParameterSO.landing.maxAngle.value;
    }

    /// <summary>
    /// Checks whether the landing velocity is small enough for a save landing
    /// </summary>
    /// <param name="collision">Collision information of the 2d collision between the ship and the ground</param>
    /// <param name="landingVelocity">[out] Velocity of the landing</param>
    /// <returns>True of the landing velocity is small enough</returns>
    private bool IsCollisionVelocitySmallEnough(Collision2D collision, out Vector2 landingVelocity)
    {
        landingVelocity = collision.GetContact(0).relativeVelocity;
        float velocity = landingVelocity.magnitude;

        return velocity < ShipParameterSO.landing.maxVelocity.value;
    }
}
