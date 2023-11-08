using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSpawnerSO", menuName = "ScriptableObjects/PlayerSpawnerSO")]
public class PlayerSpawnerSO : ScriptableObject
{
    public RandomValue horizontalStartingVelocity;

    private static PlayerSpawnerSO _playerSpawnerSO;

    public static PlayerSpawnerSO GetInstance()
    {
        if (_playerSpawnerSO == null)
            _playerSpawnerSO = Instantiate(Resources.Load<PlayerSpawnerSO>("PlayerSpawnerSO"));

        RandomValue.GenerateValuesForAllFields(_playerSpawnerSO);

        return _playerSpawnerSO;
    }
}
