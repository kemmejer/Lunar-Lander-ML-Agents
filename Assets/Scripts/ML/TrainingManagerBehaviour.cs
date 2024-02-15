using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.MLAgents;
using UnityEngine;

public class TrainingManagerBehaviour : MonoBehaviour
{
    public uint TrainingIteration { get; private set; }
    public bool IsTraining { get; private set; }
    public bool IsStarting { get; private set; }
    public bool IsStopping { get; private set; }

    private TrainingSO _trainingSO;
    private GroundGeneratorSO _groundGeneratorSO;
    private GroundGeneratorBehaviour _groundGeneratorBehaviour;

    private static TrainingManagerBehaviour _instance;

    private List<ShipAgent> _agents;

    private int _finishedShipCount;
    private int _regenerateGroundInterval;

    private bool _trainingServerStarted;
    private Process _trainingServerProcess;

    private bool _autoStartTrainingInApplication = true;
    private bool _loadConfigInMainThread = false;

    void Awake()
    {
        _instance = GetComponent<TrainingManagerBehaviour>();
    }

    /// <summary>
    /// Returns the current instance of the training manager
    /// </summary>
    /// <returns>Current instance of the training manager</returns>
    public static TrainingManagerBehaviour GetInstance()
    {
        return _instance;
    }

    private void FixedUpdate()
    {
        if (_autoStartTrainingInApplication)
        {
            _autoStartTrainingInApplication = false;

            if (CommandLineHelper.IsTrainingApplicationInstance)
            {
                ConfigManager.LoadConfig(CommandLineHelper.SelectedConfig);
                _instance.StartTraining();
            }
        }

        if (_loadConfigInMainThread)
        {
            _loadConfigInMainThread = false;
            ConfigManager.LoadConfig(ConfigManager.CurrentConfig.Name);
        }
    }

    /// <summary>
    /// Starts the training by initializing the ships and starting the training server
    /// </summary>
    public void StartTraining()
    {
        if (IsTraining || IsStarting || IsStopping)
            return;

        IsStarting = true;
        _trainingSO = TrainingSO.GetInstanceCopy();
        _groundGeneratorSO = GroundGeneratorSO.GetInstance();
        _groundGeneratorBehaviour = GroundGeneratorBehaviour.GetInstance();

        PlayerSpawnerBehaviour.GetInstance().DestroyShips();
        TrailManager.GetInstance().DestroyTrails();

        ConfigManager.UnloadModel();

        _regenerateGroundInterval = _groundGeneratorSO.regenerateInterval.RndValue;
        TrainingIteration = 0;

        StartCoroutine(StartTrainingServer());
    }

    /// <summary>
    /// Stops the training by destroying the ships, stopping the server and saving the model
    /// </summary>
    public void StopTraining()
    {
        if (!IsTraining || IsStarting || IsStopping)
            return;

        IsStopping = true;

        PlayerSpawnerBehaviour.GetInstance().DestroyShips();
        TrailManager.GetInstance().DestroyTrails();

        _agents?.Clear();

        StartCoroutine(StopTrainingServer());
    }

    /// <summary>
    /// Spawns the ship agents for training
    /// </summary>
    private void CreateAgents()
    {
        _agents = new List<ShipAgent>();
        var playerSpawner = PlayerSpawnerBehaviour.GetInstance();

        for (int i = 0; i < _trainingSO.shipCount; i++)
        {
            var ship = playerSpawner.InstantiateShip(PlayerSpawnerBehaviour.ShipType.TrainingAgent);

            var shipAgent = ship.GetComponent<ShipAgent>();
            shipAgent.OnEndEpisode += OnShipEpisodeEnded;
            shipAgent.EnableAgent();
            _agents.Add(shipAgent);
        }
    }

    /// <summary>
    /// Starts a training batch by resetting all ships and enabling the agents
    /// </summary>
    private void StartBatch()
    {
        _finishedShipCount = 0;
        TrainingIteration++;
        TrailManager.GetInstance().DestroyTrails();

        if (_groundGeneratorSO.regenerateGroundWhileTraining)
        {
            if (_regenerateGroundInterval == 0)
            {
                _groundGeneratorBehaviour.GenerateGround();
                _regenerateGroundInterval = _groundGeneratorSO.regenerateInterval.RndValue;
            }
            else
            {
                _regenerateGroundInterval--;
            }
        }

        foreach (var agent in _agents)
        {
            agent.EndEpisode();
            agent.GetComponent<ShipBehaviour>().SetShipActive(true);
            agent.EnableAgent();
        }
    }

    /// <summary>
    /// Is called when a ship has ended its training episode
    /// </summary>
    /// <param name="shipAgent"></param>
    private void OnShipEpisodeEnded(ShipAgent shipAgent)
    {
        _finishedShipCount++;

        if (_finishedShipCount == _agents.Count)
            StartBatch();
    }

    /// <summary>
    /// Starts the training server asynchronously
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartTrainingServer()
    {
        if (CommandLineHelper.IsTrainingApplicationInstance)
        {
            OnTrainingServerStarted();
            yield break;
        }

        string arguments = ConfigManager.CurrentConfig.Name;
        if (Application.isEditor)
            arguments += " true";
        else
            arguments += " false";

        _trainingServerProcess = new Process();
        _trainingServerProcess.StartInfo.FileName = Constants.TrainingBatPath;
        _trainingServerProcess.StartInfo.Arguments = arguments;
        //_trainingServerProcess.StartInfo.CreateNoWindow = true;
        _trainingServerProcess.StartInfo.UseShellExecute = false;
        _trainingServerProcess.StartInfo.RedirectStandardOutput = true;
        _trainingServerProcess.StartInfo.RedirectStandardError = true;

        _trainingServerProcess.OutputDataReceived += new DataReceivedEventHandler(TrainingServerOutputHandler);
        _trainingServerProcess.ErrorDataReceived += new DataReceivedEventHandler(TrainingServerOutputHandler);

        _trainingServerProcess.Start();
        _trainingServerProcess.BeginOutputReadLine();
        _trainingServerProcess.BeginErrorReadLine();

        if (CommandLineHelper.IsTrainingApplicationHost)
        {
            _trainingServerStarted = true;
            IsTraining = true;
            IsStarting = false;
        }
        else
        {
            yield return new WaitUntil(() => _trainingServerStarted);
            OnTrainingServerStarted();
        }
    }

    /// <summary>
    /// Stops the training server asynchronously
    /// </summary>
    /// <returns></returns>
    private IEnumerator StopTrainingServer()
    {
        // Disposing the Academy results in the communicator to close.
        // This sends a UnityCommunicatorStoppedException to the server, which results in the server shutting gracefully down
        Academy.Instance.Dispose();

        yield return new WaitUntil(() => !_trainingServerStarted);

        _trainingServerProcess?.WaitForExit();
        OnTrainingServerStopped();
    }

    /// <summary>
    /// Listens to the training server output messages and handles them
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">Output messages</param>
    private void TrainingServerOutputHandler(object sender, DataReceivedEventArgs e)
    {
        Logger.Log(e.Data);
        if (e.Data.Contains("Listening on port"))
        {
            _trainingServerStarted = true;
        }
        else if (e.Data.Contains("Stopped Training Server"))
        {
            _trainingServerStarted = false;
            if (!IsStopping)
                OnTrainingServerStopped();
        }
    }

    /// <summary>
    /// Is called, when the training server has started.
    /// Finishes the training startup
    /// </summary>
    private void OnTrainingServerStarted()
    {
        VisualizationLogger.Init();
        CreateAgents();
        IsTraining = true;
        IsStarting = false;
    }

    /// <summary>
    /// Is called, when the training server has stopped.
    /// Finished the stopping of the training
    /// </summary>
    private void OnTrainingServerStopped()
    {
        IsTraining = false;
        IsStopping = false;

        // Reset the time scale to 1 because the training server adjusts the value while training
        if (!CommandLineHelper.IsTrainingApplicationHost)
            Time.timeScale = 1.0f;

        _loadConfigInMainThread = true;
    }
}
