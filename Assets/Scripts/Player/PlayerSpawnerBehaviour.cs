using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _spaceShip;

    private List<GameObject> _spaceShips;
    private PlayerSpawnerSO _playerSpawnerSO;

    private static PlayerSpawnerBehaviour _playerSpawnerBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        _playerSpawnerSO = PlayerSpawnerSO.GetInstance();
        _playerSpawnerBehaviour = GetComponent<PlayerSpawnerBehaviour>();
        _spaceShips = new List<GameObject>();

        SpawnShip(true);
    }

    private void OnDisable()
    {
        DestroyShips();
    }

    public static PlayerSpawnerBehaviour GetInstance()
    {
        return _playerSpawnerBehaviour;
    }

    public void SpawnShip(bool userControllable = false)
    {
        var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        var bounds = spriteRenderer.bounds;
        var spawnPos = RandomHelper.RandomInRange(bounds.min, bounds.max);

        var ship = Instantiate(_spaceShip, spawnPos, Quaternion.identity);
        _spaceShips.Add(ship);

        var startingVelocity = spawnPos.x > bounds.center.x ? Vector3.left : Vector3.right;
        var rigidBody = ship.GetComponent<Rigidbody2D>();
        rigidBody.AddForce(startingVelocity * _playerSpawnerSO.horizontalStartingVelocity);

        var shipBehaviour = ship.GetComponent<ShipBehaviour>();
        shipBehaviour.OnDestroyEvent += OnDestroyShip;

        if (userControllable)
        {
            PlayerInput.GetInstance().SetPlayer(shipBehaviour);
        }
    }

    private void DestroyShips()
    {
        foreach (var ship in _spaceShips)
        {
            Destroy(ship);
        }

        _spaceShips.Clear();
    }

    private void OnDestroyShip(GameObject ship)
    {
        var shipBehaviour = ship.GetComponent<ShipBehaviour>();
        shipBehaviour.OnDestroyEvent -= OnDestroyShip;

        _spaceShips.Remove(ship);
    }
}
