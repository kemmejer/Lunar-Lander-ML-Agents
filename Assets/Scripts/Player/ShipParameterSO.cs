using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipParameterSO", menuName = "ScriptableObjects/ShipParameterSO")]
public class ShipParameterSO : ConfigScriptableObject<ShipParameterSO>
{
    public ShipPhysics physics;
    public ControlParameter controlParameter;
    public Fuel fuel;
    public Landing landing;

    [Serializable]
    public struct ShipPhysics
    {
        public RandomFloat mass;
        public RandomFloat drag;
        public RandomFloat gravityScale;
    }

    [Serializable]
    public struct ControlParameter
    {
        public RandomFloat rotationSpeed;
        public RandomFloat thrustAmount;
    }

    [Serializable]
    public struct Fuel
    {
        public RandomFloat maxFuel;
        public RandomFloat remainingFuel;
        public RandomFloat fuelConsumption;

        public void UseFuel()
        {
            remainingFuel.value -= fuelConsumption.value;

            if (remainingFuel.value < 0)
                remainingFuel.value = 0;
        }
    }

    [Serializable]
    public struct Landing
    {
        public RandomFloat maxVelocity;
        public RandomFloat maxAngle;
    }
}

