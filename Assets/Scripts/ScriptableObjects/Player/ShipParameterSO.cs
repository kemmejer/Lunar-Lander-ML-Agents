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
            _shipParameterSO = Instantiate(AssetDatabase.LoadAssetAtPath<ShipParameterSO>("Assets/Scripts/ScriptableObjects/Player/ShipParameterSO.asset"));

        return _shipParameterSO;
    }

    public static ShipParameterSO GetInstanceCopy()
    {
        return Instantiate(GetInstance());
    }

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
}

