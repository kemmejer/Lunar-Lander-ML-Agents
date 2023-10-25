using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour
{

    [SerializeField] private ShipParameterSO shipParameter;

    // Start is called before the first frame update
    void Start()
    {
        UpdateShipPhysics();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateShipPhysics()
    {
        var rigidBody = gameObject.GetComponent<Rigidbody2D>();
        var shipPhysics = shipParameter.physics;

        rigidBody.mass = shipPhysics.mass;
        rigidBody.drag = shipPhysics.drag;
        rigidBody.angularDrag = shipPhysics.angularDrag;
        rigidBody.gravityScale = shipPhysics.gravityScale;
    }
}
