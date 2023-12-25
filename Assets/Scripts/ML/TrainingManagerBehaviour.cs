using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.MLAgents;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TrainingManagerBehaviour : MonoBehaviour
{
    public bool IsTraining { get; private set; }
    public bool IsStarting { get; private set; }
    public bool IsStopping { get; private set; }

    private TrainingSO _trainingSO;

    private static TrainingManagerBehaviour _instance;

    private List<ShipAgent> _agents;

    private int _finishedShipCount;

    private bool _trainingServerStarted;
    private Process _trainingServerProcess;

    void Awake()
    {
        _instance = GetComponent<TrainingManagerBehaviour>();
    }

    public static TrainingManagerBehaviour GetInstance()
    {
        return _instance;
    }

    public void StartTraining()
    {
        if (IsTraining || IsStarting || IsStopping)
            return;

        IsStarting = true;
        _trainingSO = TrainingSO.GetInstanceCopy();

        StartCoroutine(StartTrainingServer());
    }

    public void StopTraining()
    {
        if (!IsTraining || IsStarting || IsStopping)
            return;

        IsStopping = true;

        PlayerSpawnerBehaviour.GetInstance().DestroyShips();
        TrailManager.GetInstance().DestoryTrails();

        _agents.Clear();

        StartCoroutine(StopTrainingServer());
    }

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

    private void StartBatch()
    {
        _finishedShipCount = 0;
        TrailManager.GetInstance().DestoryTrails();

        foreach (var agent in _agents)
        {
            agent.EndEpisode();
            agent.GetComponent<ShipBehaviour>().SetShipActive(true);
            agent.EnableAgent();
        }
    }

    private void OnShipEpisodeEnded(ShipAgent shipAgent)
    {
        _finishedShipCount++;

        if (_finishedShipCount == _agents.Count)
            StartBatch();
    }

    private IEnumerator StartTrainingServer()
    {
        _trainingServerProcess = new Process();
        _trainingServerProcess.StartInfo.FileName = Constants.TrainingBatPath;
        _trainingServerProcess.StartInfo.Arguments = ConfigManager.CurrentConfig.Name;
        //_trainingServerProcess.StartInfo.CreateNoWindow = true;
        _trainingServerProcess.StartInfo.UseShellExecute = false;
        _trainingServerProcess.StartInfo.RedirectStandardOutput = true;
        _trainingServerProcess.StartInfo.RedirectStandardError = true;

        _trainingServerProcess.OutputDataReceived += new DataReceivedEventHandler(TrainingServerOutputHandler);
        _trainingServerProcess.ErrorDataReceived += new DataReceivedEventHandler(TrainingServerOutputHandler);

        _trainingServerProcess.Start();
        _trainingServerProcess.BeginOutputReadLine();
        _trainingServerProcess.BeginErrorReadLine();

        yield return new WaitUntil(() => _trainingServerStarted);

        OnTrainingServerStarted();
    }

    private IEnumerator StopTrainingServer()
    {
        // Disposing the Academy results in the communicator to close.
        // This sends a UnityCommunicatorStoppedException to the server, which results in the server shutting gracefully down
        Academy.Instance.Dispose();

        yield return new WaitUntil(() => !_trainingServerStarted);

        _trainingServerProcess.WaitForExit();
        OnTrainingServerStopped();
    }

    private void TrainingServerOutputHandler(object sender, DataReceivedEventArgs e)
    {
        Logger.Log(e.Data);
        if (e.Data.Contains("Listening on port"))
            _trainingServerStarted = true;
        else if (e.Data.Contains("Stopped Training Server"))
            _trainingServerStarted = false;
    }

    private void OnTrainingServerStarted()
    {
        VisualizationLogger.Init();
        CreateAgents();
        IsTraining = true;
        IsStarting = false;
    }

    private void OnTrainingServerStopped()
    {
        IsTraining = false;
        IsStopping = false;
    }
}
