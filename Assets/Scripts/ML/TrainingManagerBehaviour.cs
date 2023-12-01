using System.Collections;
using System.Collections.Generic;
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

    private int finishedShipCount;

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

        CreateAgents();
    }

    public void StopTraining()
    {
        if (!_isRunning)
            return;

        _isRunning = false;

        PlayerSpawnerBehaviour.GetInstance().DestroyShips();
        TrailManager.GetInstance().DestoryTrails();
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
        finishedShipCount = 0;
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
        finishedShipCount++;

        if (finishedShipCount == _agents.Count)
            StartBatch();
    }
}
