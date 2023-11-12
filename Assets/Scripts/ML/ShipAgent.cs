using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;

public class ShipAgent : Agent
{
    public event IOnEndEpisode.OnEndEpisodeDelegate OnEndEpisode;

    private ShipBehaviour _shipBehaviour;
    private RayCasterBehaviour _rayCasterBehaviour;
    private BehaviorParameters _behaviorParameters;

    private const int ObervationParameterCount = 3 + 4;

    private void Awake()
    {
        _behaviorParameters = GetComponent<BehaviorParameters>();

        var rayCount = RayCasterSO.GetInstance().rayCount;
        _behaviorParameters.BrainParameters.VectorObservationSize = ObervationParameterCount + rayCount;
    }

    private void Start()
    {
        _shipBehaviour = GetComponent<ShipBehaviour>();
        _rayCasterBehaviour = GetComponentInChildren<RayCasterBehaviour>();

        _shipBehaviour.OnDestroyEvent += OnDestroyShip;
        _shipBehaviour.OnShipLandedEvent += OnShipLanded;
    }

    private void FixedUpdate()
    {
        RequestDecision();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int thrustAction = actions.DiscreteActions[0];
        int rotateAction = actions.DiscreteActions[1];

        switch (rotateAction)
        {
            case 0: break;
            case 1: _shipBehaviour.RotateLeft(); break;
            case 2: _shipBehaviour.RotateRight(); break;
        }

        switch (thrustAction)
        {
            case 0: _shipBehaviour.StopThrust(); break;
            case 1: _shipBehaviour.Thrust(); break;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(gameObject.transform.position);
        sensor.AddObservation(gameObject.transform.rotation);

        var rayHits = _rayCasterBehaviour.CastRays();
        foreach (var rayHit in rayHits)
        {
            sensor.AddObservation(rayHit.distance);
        }
    }

    private void OnShipLanded(Vector2 landingPosition)
    {
        SetReward(1.0f);
        EndTraining();
    }

    private void OnDestroyShip(GameObject ship)
    {
        SetReward(-1.0f);
        EndTraining();
    }

    private void EndTraining()
    {
        _shipBehaviour.OnDestroyEvent -= OnDestroyShip;
        _shipBehaviour.OnShipLandedEvent -= OnShipLanded;

        OnEndEpisode?.Invoke(this);

        Destroy(this);
    }
}
