using TMPro;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using static IOnShipLandedEvent;
using ImageGraphName = VisualizationLogger.ImageGraphName;

public class ShipAgent : Agent
{
    public event IOnEndEpisode.OnEndEpisodeDelegate OnEndEpisode;

    private TextMeshProUGUI _rewardText;
    private ShipBehaviour _shipBehaviour;
    private RayCasterBehaviour _rayCasterBehaviour;
    private BehaviorParameters _behaviorParameters;
    private ShipDecisionRequester _shipDecisionRequester;

    private bool _hasEpisodeEnded;

    private readonly Color _rewardPositiveColor = new Color(0.8f, 1.0f, 0.8f);
    private readonly Color _rewardNegativeColor = new Color(1.0f, 0.8f, 0.8f);

    private bool _hasAgentModel;

    private const int ObservationParameterCount = 2 + 2 + 1; // Position + Velocity + Rotation

    private void Awake()
    {
        ObservationNormalizer.Init();

        _shipBehaviour = GetComponent<ShipBehaviour>();
        _behaviorParameters = GetComponent<BehaviorParameters>();
        _shipDecisionRequester = GetComponent<ShipDecisionRequester>();
        _rayCasterBehaviour = GetComponentInChildren<RayCasterBehaviour>();
        _rewardText = GetComponentInChildren<TextMeshProUGUI>();

        _shipBehaviour.OnShipLandedEvent += OnShipLanded;

        var rayCount = RayCasterSO.GetInstance().RayCount;
        _behaviorParameters.BrainParameters.VectorObservationSize = ObservationParameterCount + rayCount;
        _behaviorParameters.BehaviorName = Constants.AgentName;
        _shipDecisionRequester.DecisionPeriod = TrainingSO.GetInstance().decisionInterval;
    }

    /// <summary>
    /// Override OnDisable to prevent the base class OnDisable call of the TrainedAgent class.
    /// This is made to prevent deinitialization of the agent and its rewards while hiding the ship.
    /// </summary>
    protected override void OnDisable()
    {

    }

    /// <summary>
    /// Called every time when an episode starts.
    /// Resets the ship to its default state and position
    /// </summary>
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
        _hasEpisodeEnded = false;

        ResetShip();
    }

    /// <summary>
    /// Called every time when the model makes a decision.
    /// Handles the behaviour of the ship.
    /// </summary>
    /// <param name="actions">Actions containing the decisions of the model</param>
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

    /// <summary>
    /// Sends the observations to the model to make a decision
    /// </summary>
    /// <param name="sensor">Sensor to add the observations to</param>
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
        if (groundDistance < 0.2)
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
        if (shipRotation > Constants.MaxShipAngle / 2.0f)
            reward -= 0.02f;

        float groundDistanceFactor = Mathf.Max(1 - groundDistance, 0.0f) * 0.05f;
        if (isShipMovingDown)
            reward += groundDistanceFactor;
        else
            reward -= 0.1f;

        SetReward(reward);

        CollectImageData();
        UpdateRewardText();
    }

    /// <summary>
    /// Enables the agent and its training behaviour
    /// </summary>
    public void EnableAgent()
    {
        _shipDecisionRequester.Enabled = true;
    }

    /// <summary>
    /// Disables the agent and its training behaviour
    /// </summary>
    public void DisableAgent()
    {
        _shipDecisionRequester.Enabled = false;
    }

    /// <summary>
    /// Applies a trained model to the ship
    /// </summary>
    /// <param name="model"></param>
    public void SetAgentModel(in AgentModel model)
    {
        _hasAgentModel = true;
        SetModel(Constants.AgentName, model.Model);
        ResetShip();
        _behaviorParameters.BehaviorType = BehaviorType.InferenceOnly;
        EnableAgent();
    }

    /// <summary>
    /// Called, when the ship has landed or crashed
    /// </summary>
    /// <param name="landingData">Information about the landing</param>
    private void OnShipLanded(in LandingData landingData)
    {
        var trail = gameObject.transform.Find(TrailManager.TrailName).gameObject;
        TrailManager.GetInstance().MoveTrailToTrailManager(trail);

        RewardLanding(landingData);
        EndTraining();
    }

    /// <summary>
    /// Rewards the landing depending on the success of the landing
    /// </summary>
    /// <param name="landingData">Information about the landing</param>
    private void RewardLanding(in LandingData landingData)
    {
        switch (landingData.type)
        {
            case LandingType.Success: RewardSuccessfulLanding(landingData); break;
            case LandingType.Crash: RewardCrash(landingData); break;
            case LandingType.OutOfBounds: RewardOutOfBounds(landingData); break;
        }
    }

    /// <summary>
    /// Rewards a successful landing on the ground
    /// </summary>
    /// <param name="landingData">Information about the landing</param>
    private void RewardSuccessfulLanding(in LandingData landingData)
    {
        SetReward(50.0f);

        VisualizationLogger.AddSuccessfulLanding();
        VisualizationLogger.AddValue(VisualizationLogger.GraphName.SuccessRate, 1.0f, StatAggregationMethod.Average);
    }

    /// <summary>
    /// Rewards a failed landing on the ground
    /// </summary>
    /// <param name="landingData">Information about the landing</param>
    private void RewardCrash(in LandingData landingData)
    {
        const float penaltyFactor = 2.0f;
        const float successReward = 2.0f;
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
            reward -= ObservationNormalizer.NormalizeVelocity(landingData.velocity).magnitude * penaltyFactor * 2;

        SetReward(reward);

        VisualizationLogger.AddFailedLanding();
        VisualizationLogger.AddValue(VisualizationLogger.GraphName.SuccessRate, 0.0f, StatAggregationMethod.Average);
    }

    /// <summary>
    /// Rewards a out of bounds failed landing
    /// </summary>
    /// <param name="landingData">Information about the landing</param>
    private void RewardOutOfBounds(in LandingData landingData)
    {
        SetReward(-10.0f);

        VisualizationLogger.AddFailedLanding();
        VisualizationLogger.AddValue(VisualizationLogger.GraphName.SuccessRate, 0.0f, StatAggregationMethod.Average);
    }

    /// <summary>
    /// Ends the current training iteration of the ship 
    /// </summary>
    private void EndTraining()
    {
        _hasEpisodeEnded = true;
        DisableAgent();

        OnEndEpisode?.Invoke(this);

        UpdateRewardText();
    }

    /// <summary>
    /// Resets the ship to begin a new training iteration
    /// </summary>
    private void ResetShip()
    {
        if (!_shipBehaviour.IsInitialized)
            return;

        PlayerSpawnerBehaviour.GetInstance().ResetShip(gameObject);

        SetRandomComponentColor();
        UpdateRewardText();

        var trail = gameObject.GetComponentInChildren<TrailRenderer>();
        trail.Clear();
    }

    /// <summary>
    /// Sets the text above the ship to the current agent reward
    /// </summary>
    private void UpdateRewardText()
    {
        float cumulativeReward = GetCumulativeReward();
        _rewardText.SetText(cumulativeReward.ToString("0.000"));
        _rewardText.color = cumulativeReward >= 0 ? _rewardPositiveColor : _rewardNegativeColor;
    }

    /// <summary>
    /// Sets the trail and ray cast color of the ship to a random color
    /// </summary>
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

    /// <summary>
    /// Collects data about the ships state for image analysis using tensorboard
    /// </summary>
    private void CollectImageData()
    {
        if (_hasAgentModel)
            return;

        var position = _shipBehaviour.GetPosition();
        var rotation = _shipBehaviour.GetEulerRotation();
        var velocity = _shipBehaviour.GetVelocity();
        var reward = GetCumulativeReward();
        var isBoosting = _shipBehaviour.IsShipThrusting() ? 1.0f : 0.0f;

        VisualizationLogger.AddImageValue(ImageGraphName.Position, position);
        VisualizationLogger.AddImageValue(ImageGraphName.Rotation, rotation);
        VisualizationLogger.AddImageValue(ImageGraphName.Velocity, velocity);
        VisualizationLogger.AddImageValue(ImageGraphName.Reward, reward);
        VisualizationLogger.AddImageValue(ImageGraphName.Thrust, isBoosting);
    }
}
