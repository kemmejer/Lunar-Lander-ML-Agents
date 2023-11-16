using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Policies;
using UnityEngine;

public class PlayerSpawnerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _spaceShip;

    private List<GameObject> _spaceShips;
    private PlayerSpawnerSO _playerSpawnerSO;

    private static PlayerSpawnerBehaviour _playerSpawnerBehaviour;

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

    public GameObject InstantiateShip(bool userControllable = false)
    {
        var ship = Instantiate(_spaceShip, gameObject.transform.parent);
        _spaceShips.Add(ship);

        ResetShip(ship);

        if (userControllable)
        {
            var shipBehaviour = ship.GetComponent<ShipBehaviour>();
            PlayerInput.GetInstance().SetPlayer(shipBehaviour);
        }
        else
        {
            var shipAgent = ship.GetComponent<ShipAgent>();
            shipAgent.enabled = true;
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
        ship.transform.position = spawnPos;
        ship.transform.rotation = Quaternion.identity;

        // Rigidbody
        var rigidBody = ship.GetComponent<Rigidbody2D>();
        ResetHelper.ResetRigidBody(rigidBody);
        rigidBody.position = spawnPos;
        var startingVelocity = spawnPos.x > bounds.center.x ? Vector3.left : Vector3.right;
        rigidBody.AddForce(startingVelocity * _playerSpawnerSO.horizontalStartingVelocity.RndValue);

        // Trail
        var trail = ship.GetComponentInChildren<TrailRenderer>();
        trail.Clear();
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
