using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipParameterSO", menuName = "ScriptableObjects/ShipParameterSO")]
public class ShipParameterSO : ScriptableObject
{
    [Serializable]
    public class ShipPhysics
    {
        public float mass;
        public float drag;
        public float angularDrag;
        public float gravityScale;
    }

    [Serializable]
    public class ControlParameter
    {
        public float rotationSpeed;
        public float thrustAmount;
    }

    public float fuel;
    public float fuelConsumption;

    public ShipPhysics physics;
    public ControlParameter controlParameter;
}
