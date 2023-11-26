using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.VisualScripting;
using static IOnShipLandedEvent;
using TMPro;

public class ShipAgent : Agent
{
    public event IOnEndEpisode.OnEndEpisodeDelegate OnEndEpisode;

    public TrainingSO TrainingSO { get; set; }

    private TextMeshProUGUI _rewardText;

    private ShipBehaviour _shipBehaviour;
    private RayCasterBehaviour _rayCasterBehaviour;
    private BehaviorParameters _behaviorParameters;
    private ShipDecisionRequester _shipDecisionRequester;

    private bool _hasEpisodeEnded;

    private readonly Color _rewardPositiveColor = new Color(0.8f, 1.0f, 0.8f);
    private readonly Color _rewardNegativeColor = new Color(1.0f, 0.8f, 0.8f);

    private const int ObservationParameterCount = 2 + 2 + 1; // Position + Velocity + Rotation

    private void Awake()
    {
        _shipBehaviour = GetComponent<ShipBehaviour>();
        _behaviorParameters = GetComponent<BehaviorParameters>();
        _shipDecisionRequester = GetComponent<ShipDecisionRequester>();

        _shipBehaviour.OnShipLandedEvent += OnShipLanded;

        var rayCount = RayCasterSO.GetInstance().RayCount;
        _behaviorParameters.BrainParameters.VectorObservationSize = ObservationParameterCount + rayCount;
    }

    private void Start()
    {
        _rayCasterBehaviour = GetComponentInChildren<RayCasterBehaviour>();
        _rewardText = GetComponentInChildren<TextMeshProUGUI>();

        _shipDecisionRequester.DecisionPeriod = TrainingSO.decisionInterval;
    }

    /// <summary>
    /// Override OnDisable to prevent the base class OnDisable call of the Agent class.
    /// This is made to prevent deinitialization of the agent and its rewards while hiding the ship.
    /// </summary>
    protected override void OnDisable()
    {

    }

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        _hasEpisodeEnded = false;
        ResetShip();
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int thrustAction = actions.DiscreteActions[0];
        int rotateAction = actions.DiscreteActions[1];

        if (_hasEpisodeEnded)
            Debug.LogError("WHAT?");

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

        // Reward 
        if (_hasEpisodeEnded)
            return; // Don't reward an already landed ship

        float groundDistance = ObservationNormalizer.NormalizeRayCastDistance(rayHits[rayHits.Length / 2].distance);
        bool isShipMovingDown = normalizedVelocity.y < 0.0f;
        bool isShipThrusting = _shipBehaviour.IsShipThrusting();
        bool isShipTooFast = _shipBehaviour.IsShipTooFast();

        float reward = 0.0f;
        if (groundDistance < 0.3)
        {
            if (isShipTooFast)
            {
                if (isShipThrusting)
                    reward += 0.02f;
                else
                    reward -= 0.05f;
            }
            else if (isShipMovingDown)
            {
                reward += 0.1f;
            }
        }

        float shipRotation = Mathf.Abs(_shipBehaviour.GetEulerRotation());
        if (shipRotation > _shipBehaviour.ShipParameterSO.landing.maxAngle.value * 2)
            reward -= 0.02f;

        float groundDistanceFactor = Mathf.Max(1 - groundDistance, 0.0f) * 0.05f;
        if (isShipMovingDown)
            reward += groundDistanceFactor;
        else
            reward -= 0.1f;

        SetReward(reward);

        UpdateRewardText();
    }

    public void EnableAgent()
    {
        _shipDecisionRequester.Enabled = true;
    }

    public void DisableAgent()
    {
        _shipDecisionRequester.Enabled = false;
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
            case LandingType.Success: RewardSuccessfulLanding(landingData); break;
            case LandingType.Crash: RewardCrash(landingData); break;
            case LandingType.OutOfBounds: SetReward(-10.0f); break;
        }
    }

    private void RewardSuccessfulLanding(in LandingData landingData)
    {
        SetReward(50.0f);
    }

    private void RewardCrash(in LandingData landingData)
    {
        const float penaltyFactor = 2.0f;
        const float successReward = 0.2f;
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
        _hasEpisodeEnded = true;
        DisableAgent();

        OnEndEpisode?.Invoke(this);

        UpdateRewardText();
    }

    private void ResetShip()
    {
        PlayerSpawnerBehaviour.GetInstance().ResetShip(gameObject);

        SetRandomComponentColor();
        UpdateRewardText();

        var trail = gameObject.GetComponentInChildren<TrailRenderer>();
        trail.Clear();
    }

    private void UpdateRewardText()
    {
        float cumulativeReward = GetCumulativeReward();
        _rewardText.SetText(cumulativeReward.ToString("0.000"));
        _rewardText.color = cumulativeReward >= 0 ? _rewardPositiveColor : _rewardNegativeColor;
    }

    private void SetRandomComponentColor()
    {
        var hue = Random.value;
        Color trailColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
        var shipTrail = gameObject.GetComponentInChildren<ShipTrailBehaviour>();
        shipTrail.SetColor(trailColor);

        Color rayColor = Color.HSVToRGB(hue, 1.0f, 1.0f);
        rayColor.a = 0.1f;
        var rayCaster = gameObject.GetComponentInChildren<RayCasterBehaviour>();
        rayCaster.SetColor(rayColor);
    }
}
