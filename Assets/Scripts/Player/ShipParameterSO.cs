using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ShipParameterSO", menuName = "ScriptableObjects/ShipParameterSO")]
public class ShipParameterSO : ScriptableObject
{
    public ShipPhysics physics;
    public ControlParameter controlParameter;
    public Fuel fuel;
    public Landing landing;

    private static ShipParameterSO _shipParameterSO;

    public static ShipParameterSO GetInstance()
    {
        if (_shipParameterSO == null)
            _shipParameterSO = Instantiate(Resources.Load<ShipParameterSO>("ShipParameterSO"));

        _shipParameterSO.GenerateRandomValues();

        return _shipParameterSO;
    }

    public static ShipParameterSO GetInstanceCopy()
    {
        return Instantiate(GetInstance());
    }

    public void GenerateRandomValues()
    {
        IRandomValue.GenerateValuesForAllFields(physics);
        IRandomValue.GenerateValuesForAllFields(controlParameter);
        IRandomValue.GenerateValuesForAllFields(fuel);
        IRandomValue.GenerateValuesForAllFields(landing);
    }

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

