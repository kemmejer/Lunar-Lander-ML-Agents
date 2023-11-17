using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;
using static IOnShipLandedEvent;

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
        Vector2 normalizedPosition = ObservationNormalizer.NormalizeScreenPosition(_shipBehaviour.GetPosition());
        sensor.AddObservation(normalizedPosition);

        // Rotation
        float normalizedRotation = ObservationNormalizer.NormalizeEulerAngle(_shipBehaviour.GetEulerRotation());
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

    private void OnShipLanded(in LandingData landingData)
    {
        RewardLanding(landingData);
        EndTraining();
    }

    private void RewardLanding(in LandingData landingData)
    {
        switch (landingData.type)
        {
            case LandingType.Success: RewardSuccessfullLanding(landingData); break;
            case LandingType.Crash: RewardCrash(landingData); break;
            case LandingType.OutOfBounds: SetReward(-1.0f); break;
        }
    }

    private void RewardSuccessfullLanding(in LandingData landingData)
    {
        float reward = 1.0f;
        SetReward(reward);
    }

    private void RewardCrash(in LandingData landingData)
    {
        const float penaltyFactor = 0.5f;
        const float successReward = 0.3f;
        float reward = 0.0f;

        // Angle
        float groundDeltaAngle = Mathf.Abs(landingData.groundDeltaAngle);
        if (groundDeltaAngle < _shipBehaviour.ShipParameterSO.landing.maxAngle.value)
            reward += successReward;
        else
            reward -= ObservationNormalizer.NormalizeEulerAngle(groundDeltaAngle) * penaltyFactor;

        // Velocity

        if (landingData.velocity.magnitude < _shipBehaviour.ShipParameterSO.landing.maxVelocity.value)
            reward += successReward;
        else
            reward -= ObservationNormalizer.NormalizeVelocity(landingData.velocity).magnitude * penaltyFactor;

        SetReward(reward);
    }

    private void EndTraining()
    {
        decisionIteration = 1;
        OnEndEpisode?.Invoke(this);
    }
}
