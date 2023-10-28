using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour
{

    [SerializeField] private ShipParameterSO shipParameter;

    private Rigidbody2D _rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody2D>();
        UpdateShipPhysics();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RotateRight()
    {
        gameObject.transform.Rotate(0.0f, 0.0f, -shipParameter.controlParameter.rotationSpeed);

        var rotation = gameObject.transform.localEulerAngles;
        var zRotation = rotation.z > 180 ? rotation.z - 360 : rotation.z;
        if (zRotation < -90.0f)
            gameObject.transform.localEulerAngles = new Vector3(rotation.x, rotation.y, 270.0f);
    }

    public void RotateLeft()
    {
        gameObject.transform.Rotate(0.0f, 0.0f, shipParameter.controlParameter.rotationSpeed);

        var rotation = gameObject.transform.localEulerAngles;
        var zRotation = rotation.z > 180 ? rotation.z - 360 : rotation.z;
        if (zRotation > 90.0f)
            gameObject.transform.localEulerAngles = new Vector3(rotation.x, rotation.y, 90.0f);
    }

    public void Thrust()
    {
        _rigidBody.AddForce(transform.up * shipParameter.controlParameter.thrustAmount, ForceMode2D.Force);
    }

    private void UpdateShipPhysics()
    {
        var shipPhysics = shipParameter.physics;

        _rigidBody.mass = shipPhysics.mass;
        _rigidBody.drag = shipPhysics.drag;
        _rigidBody.angularDrag = shipPhysics.angularDrag;
        _rigidBody.gravityScale = shipPhysics.gravityScale;
    }
}
