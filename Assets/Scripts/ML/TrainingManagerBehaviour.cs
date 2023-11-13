using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainingManagerBehaviour : MonoBehaviour
{
    private TrainingSO _trainingSO;

    private static TrainingManagerBehaviour _instance;

    private List<ShipAgent> _agents;
    private bool _isRunning;

    // Start is called before the first frame update
    void Start()
    {
        _instance = GetComponent<TrainingManagerBehaviour>();
        _trainingSO = TrainingSO.GetInstance();
        StartTraining();
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

        StartBatch();
    }

    public void StopTraining()
    {
        if (!_isRunning)
            return;

        _isRunning = false;

        EndBatch();
    }

    private void StartBatch()
    {
        _agents = new List<ShipAgent>();
        var playerSpawner = PlayerSpawnerBehaviour.GetInstance();

        for (int i = 0; i < _trainingSO.shipCount; i++)
        {
            var ship = playerSpawner.InstantiateShip();

            var shipAgent = ship.GetComponent<ShipAgent>();
            shipAgent.OnEndEpisode += OnShipEpisodeEnded;
            _agents.Add(shipAgent);
        }
    }

    private void EndBatch()
    {
        PlayerSpawnerBehaviour.GetInstance().DestroyShips();
        TrailManager.GetInstance().DestoryTrails();

        if(_isRunning)
            StartBatch();
    }

    private void OnShipEpisodeEnded(ShipAgent shipAgent)
    {
    }
}
