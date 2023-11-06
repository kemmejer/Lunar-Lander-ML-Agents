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
        RandomValue.GenerateValuesForAllFields(physics);
        RandomValue.GenerateValuesForAllFields(controlParameter);
        RandomValue.GenerateValuesForAllFields(fuel);
        RandomValue.GenerateValuesForAllFields(landing);
    }

    [Serializable]
    public struct ShipPhysics
    {
        public RandomValue mass;
        public RandomValue drag;
        public RandomValue angularDrag;
        public RandomValue gravityScale;
    }

    [Serializable]
    public struct ControlParameter
    {
        public RandomValue rotationSpeed;
        public RandomValue thrustAmount;
    }

    [Serializable]
    public struct Fuel
    {
        public RandomValue maxFuel;
        public RandomValue remainingFuel;
        public RandomValue fuelConsumption;

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
        public RandomValue maxVelocity;
        public RandomValue maxAngle;
    }
}

