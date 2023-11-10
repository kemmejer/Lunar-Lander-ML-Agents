using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSpawnerSO", menuName = "ScriptableObjects/PlayerSpawnerSO")]
public class PlayerSpawnerSO : ScriptableObject
{
    public RandomFloat horizontalStartingVelocity;

    private static PlayerSpawnerSO _playerSpawnerSO;

    public static PlayerSpawnerSO GetInstance()
    {
        if (_playerSpawnerSO == null)
            _playerSpawnerSO = Instantiate(Resources.Load<PlayerSpawnerSO>("PlayerSpawnerSO"));

        IRandomValue.GenerateValuesForAllFields(_playerSpawnerSO);

        return _playerSpawnerSO;
    }
}
