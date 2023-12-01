using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

public class PlayerSpawnerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _playerShipPrefab;
    [SerializeField] private GameObject _agentShipPrefab;

    private List<GameObject> _spaceShips;
    private PlayerSpawnerSO _playerSpawnerSO;

    private static PlayerSpawnerBehaviour _playerSpawnerBehaviour;

    public enum ShipType { Player, TrainedAgent, TrainingAgent }

    void Awake()
    {
        _playerSpawnerSO = PlayerSpawnerSO.GetInstance();
        _playerSpawnerBehaviour = GetComponent<PlayerSpawnerBehaviour>();
        _spaceShips = new List<GameObject>();
    }

    private void OnDisable()
    {
        DestroyShips();
    }

    public static PlayerSpawnerBehaviour GetInstance()
    {
        return _playerSpawnerBehaviour;
    }

    public GameObject InstantiateShip(ShipType shipType)
    {
        Func<GameObject, GameObject> CreateShip = shipPrefab =>
        {
            var shipInstance = Instantiate(shipPrefab, gameObject.transform.parent);
            ResetShip(shipInstance);
            _spaceShips.Add(shipInstance);
            return shipInstance;
        };

        GameObject ship = null;
        switch (shipType)
        {
            case ShipType.Player:
                ship = CreateShip(_playerShipPrefab);
                var shipBehaviour = ship.GetComponent<ShipBehaviour>();
                PlayerInput.GetInstance().SetPlayer(shipBehaviour);
                break;

            case ShipType.TrainedAgent:
                if (ConfigManager.CurrentModel == null) return null;
                ship = CreateShip(_agentShipPrefab);
                ship.GetComponent<ShipAgent>().SetAgentModel(ConfigManager.CurrentModel);
                break;

            case ShipType.TrainingAgent:
                ship = CreateShip(_agentShipPrefab);
                break;
        }

        return ship;
    }

    public void ResetShip(GameObject ship)
    {
        // Ship parameter
        var shipBehaviour = ship.GetComponent<ShipBehaviour>();
        shipBehaviour.InitShip();

        // Transform
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        var bounds = spriteRenderer.bounds;
        var spawnPos = RandomHelper.RandomInRange(bounds.min, bounds.max);
        ship.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);

        // Rigidbody
        var rigidBody = ship.GetComponent<Rigidbody2D>();
        ResetHelper.ResetRigidBody(rigidBody);
        rigidBody.position = spawnPos;
        var startingVelocity = spawnPos.x > bounds.center.x ? Vector3.left : Vector3.right;
        rigidBody.AddForce(startingVelocity * _playerSpawnerSO.horizontalStartingVelocity.RndValue);
    }

    public void DestroyShips()
    {
        foreach (var ship in _spaceShips)
        {
            Destroy(ship);
        }

        _spaceShips.Clear();
    }
}
