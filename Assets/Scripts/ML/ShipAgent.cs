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

    private const int ObervationParameterCount = 2 + 1;

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

        _shipBehaviour.OnShipLandedEvent += OnShipLanded;
    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        PlayerSpawnerBehaviour.GetInstance().ResetShip(gameObject);
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
        sensor.AddObservation(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
        sensor.AddObservation(gameObject.transform.eulerAngles.z);

        var rayHits = _rayCasterBehaviour.CastRays();
        foreach (var rayHit in rayHits)
        {
            if(rayHit.collider)
                sensor.AddObservation(rayHit.distance);
            else
                sensor.AddObservation(-1.0f);
        }
    }

    private void OnShipLanded(Vector2 landingPosition, IOnShipLandedEvent.LandingType landingType)
    {
        if (landingType == IOnShipLandedEvent.LandingType.Success)
            SetReward(1.0f);
        else
            SetReward(-1.0f);

        EndTraining();
    }

    private void EndTraining()
    {
        OnEndEpisode?.Invoke(this);

        EndEpisode();
    }
}
