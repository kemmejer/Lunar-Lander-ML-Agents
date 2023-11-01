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

    [Serializable]
    public class Fuel
    {
        public float maxFuel;
        public float remainingFuel;
        public float fuelConsumption;

        public void UseFuel()
        {
            remainingFuel -= fuelConsumption;

            if(remainingFuel < 0)
                remainingFuel = 0;
        }
    }

    [Serializable]
    public class Landing
    {
        public float maxVelocity;
        public float maxAngle;
    }

    public ShipPhysics physics;
    public ControlParameter controlParameter;
    public Fuel fuel;
    public Landing landing;
}
