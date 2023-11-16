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

    public TrainingSO TrainingSO { get; set; }

    private ShipBehaviour _shipBehaviour;
    private RayCasterBehaviour _rayCasterBehaviour;
    private BehaviorParameters _behaviorParameters;
    private int decisionIteration = 1;

    private const int ObervationParameterCount = 2 + 2 + 1; // Position + Velocity + Rotation

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
        if (decisionIteration >= TrainingSO.decisionInterval)
        {
            RequestDecision();
            decisionIteration = 1;
        }
        else
        {
            RequestAction();
            decisionIteration++;
        }
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
        // Position
        Vector2 normalizedPosition = ObservationNormalizer.NormalizeScreenPosition(gameObject.transform.position);
        sensor.AddObservation(normalizedPosition);

        // Rotation
        float normalizedRotation = ObservationNormalizer.NormalizeEulerAngle(gameObject.transform.eulerAngles.z);
        sensor.AddObservation(normalizedRotation);

        //Velocity
        var normalizedVelocity = ObservationNormalizer.NormalizeVelocity(_shipBehaviour.GetVelocity());
        sensor.AddObservation(normalizedVelocity);

        // RayCast distance
        var rayHits = _rayCasterBehaviour.CastRays();
        foreach (var rayHit in rayHits)
        {
            if (rayHit.collider)
            {
                float normalizedDistance = ObservationNormalizer.NormalizeRayCastDistance(rayHit.distance);
                sensor.AddObservation(normalizedDistance);
            }
            else
            {
                sensor.AddObservation(-1.0f);
            }
        }
    }

    private void OnShipLanded(Vector2 landingPosition, IOnShipLandedEvent.LandingType landingType)
    {
        switch (landingType)
        {
            case IOnShipLandedEvent.LandingType.Success: SetReward(1.0f); break;
            case IOnShipLandedEvent.LandingType.Crash: SetReward(-0.2f); break;
            case IOnShipLandedEvent.LandingType.OutOfBounds: SetReward(-1.0f); break;
        }

        EndTraining();
    }

    private void EndTraining()
    {
        decisionIteration = 1;
        OnEndEpisode?.Invoke(this);
    }
}
