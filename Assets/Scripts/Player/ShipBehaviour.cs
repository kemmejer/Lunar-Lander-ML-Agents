using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour
{

    [SerializeField] private ShipParameterSO _shipParameter;
    [SerializeField] private GameObject _shipThruster;

    private Rigidbody2D _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody2D>();
        UpdateShipPhysics();
        _shipThruster.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

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
        _rigidBody.AddForce(transform.up * _shipParameter.controlParameter.thrustAmount, ForceMode2D.Force);
        _shipThruster.SetActive(true);
    }

    public void StopThrust()
    {
        _shipThruster.SetActive(false);
    }

    private void UpdateShipPhysics()
    {
        var shipPhysics = _shipParameter.physics;

        _rigidBody.mass = shipPhysics.mass;
        _rigidBody.drag = shipPhysics.drag;
        _rigidBody.angularDrag = shipPhysics.angularDrag;
        _rigidBody.gravityScale = shipPhysics.gravityScale;
    }
}
