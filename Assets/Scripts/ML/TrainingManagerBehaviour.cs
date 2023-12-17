using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.MLAgents;
using UnityEngine;

public class TrainingManagerBehaviour : MonoBehaviour
{
    private TrainingSO _trainingSO;

    private static TrainingManagerBehaviour _instance;

    private List<ShipAgent> _agents;
    private bool _isRunning;

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
        if (_isRunning)
            return;

        _isRunning = true;
        _trainingSO = TrainingSO.GetInstanceCopy();

        StartCoroutine(StartTrainingServer());
    }

    public void StopTraining()
    {
        if (!_isRunning)
            return;

        _isRunning = false;
        _trainingServerStarted = false;

        PlayerSpawnerBehaviour.GetInstance().DestroyShips();
        TrailManager.GetInstance().DestoryTrails();
        VisualizationLogger.UnInit();

        _agents.Clear();
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

    private void TrainingServerOutputHandler(object sender, DataReceivedEventArgs e)
    {
        Logger.Log(e.Data);
        if (e.Data.Contains("Listening on port"))
            _trainingServerStarted = true;
    }

    private void OnTrainingServerStarted()
    {
        VisualizationLogger.Init();

        CreateAgents();
    }
}
