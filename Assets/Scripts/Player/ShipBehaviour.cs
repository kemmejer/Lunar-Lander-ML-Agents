using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _shipThruster;
    [SerializeField] private GameObject _fuelBar;
    [SerializeField] private GameObject _velocityIndicator;

    private ShipParameterSO _shipParameter;
    private Rigidbody2D _rigidBody;
    private bool _isDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        _shipParameter = ShipParameterSO.GetInstanceCopy();
        _rigidBody = gameObject.GetComponent<Rigidbody2D>();
        UpdateShipPhysics();
        _shipThruster.SetActive(false);
        ResetFuel();
    }

    // Update is called once per frame
    void Update()
    {
        var scale = _fuelBar.transform.localScale;
        var xScale = _shipParameter.fuel.remainingFuel / _shipParameter.fuel.maxFuel;
        _fuelBar.transform.localScale = new Vector3(xScale, scale.y, scale.z);
    }

    void FixedUpdate()
    {
        UpdateVelocityIndicator();
    }

    private void OnBecameInvisible()
    {
        DestroyShip();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        bool angleOk = IsCollisionAngleSmallEnough(collision);
        bool velocityOk = IsCollisionVelocitySmallEnough(collision);

        if (angleOk && velocityOk)
        {

        }
        else
        {
            DestroyShip();
        }

        Debug.Log(string.Format("AngleOk: {0}, VelocityOk: {1}", angleOk, velocityOk));
    }

    public void RotateRight()
    {
        gameObject.transform.Rotate(0.0f, 0.0f, -_shipParameter.controlParameter.rotationSpeed);

        var rotation = gameObject.transform.localEulerAngles;
        var zRotation = rotation.z > 180 ? rotation.z - 360 : rotation.z;
        if (zRotation < -90.0f)
            gameObject.transform.localEulerAngles = new Vector3(rotation.x, rotation.y, 270.0f);
    }

    public void RotateLeft()
    {
        gameObject.transform.Rotate(0.0f, 0.0f, _shipParameter.controlParameter.rotationSpeed);

        var rotation = gameObject.transform.localEulerAngles;
        var zRotation = rotation.z > 180 ? rotation.z - 360 : rotation.z;
        if (zRotation > 90.0f)
            gameObject.transform.localEulerAngles = new Vector3(rotation.x, rotation.y, 90.0f);
    }

    public void Thrust()
    {
        if (_shipParameter.fuel.remainingFuel == 0)
        {
            StopThrust();
            return;
        }

        _rigidBody.AddForce(transform.up * _shipParameter.controlParameter.thrustAmount, ForceMode2D.Force);
        _shipThruster.SetActive(true);
        UseFuel();
    }

    public void StopThrust()
    {
        _shipThruster.SetActive(false);
    }

    private void DestroyShip(bool explode = true)
    {
        if (_isDestroyed)
            return;

        _isDestroyed = true;

        if (explode)
            AnimationSystem.animationSystem.PlayExplosionAt(GetPosition());

        var trail = gameObject.transform.Find("Trail").gameObject;
        TrailManager.GetInstance().MoveTrailToTrailManager(trail);

        Destroy(_shipParameter);
        Destroy(gameObject);
    }

    private void UpdateShipPhysics()
    {
        var shipPhysics = _shipParameter.physics;

        _rigidBody.mass = shipPhysics.mass;
        _rigidBody.drag = shipPhysics.drag;
        _rigidBody.angularDrag = shipPhysics.angularDrag;
        _rigidBody.gravityScale = shipPhysics.gravityScale;
    }

    private void UseFuel()
    {
        _shipParameter.fuel.UseFuel();

        var scale = _fuelBar.transform.localScale;
        var xScale = _shipParameter.fuel.remainingFuel / _shipParameter.fuel.maxFuel;
        _fuelBar.transform.localScale = new Vector3(xScale, scale.y, scale.z);
    }

    private void UpdateVelocityIndicator()
    {
        bool isVelocityOk = GetVelocity().magnitude < _shipParameter.landing.maxVelocity;

        if (isVelocityOk)
            _velocityIndicator.GetComponent<Renderer>().material.color = Color.green;
        else
            _velocityIndicator.GetComponent<Renderer>().material.color = Color.red;
    }

    private void ResetFuel()
    {
        _shipParameter.fuel.remainingFuel = _shipParameter.fuel.maxFuel;

        var scale = _fuelBar.transform.localScale;
        _fuelBar.transform.localScale = new Vector3(1.0f, scale.y, scale.z);
    }

    private bool IsCollisionAngleSmallEnough(Collision2D collision)
    {
        var surfaceNormal = collision.GetContact(0).normal;
        var shipNormal = GetUpFacingShipNormal();
        var angle = Vector2.Angle(surfaceNormal, shipNormal);

        Debug.Log(string.Format("Landing angle: {0}", angle));

        return Mathf.Abs(angle) < _shipParameter.landing.maxAngle;
    }

    private bool IsCollisionVelocitySmallEnough(Collision2D collision)
    {
        var velocity2D = collision.GetContact(0).relativeVelocity;
        var velocity = velocity2D.magnitude;

        Debug.Log(string.Format("Landing velocity: {0}", velocity));

        return velocity < _shipParameter.landing.maxVelocity;
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

}
