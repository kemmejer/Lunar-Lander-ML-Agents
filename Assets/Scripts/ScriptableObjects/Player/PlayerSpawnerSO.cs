using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSpawnerSO", menuName = "ScriptableObjects/PlayerSpawnerSO")]
public class PlayerSpawnerSO : ScriptableObject
{
    public float horizontalStartingVelocity;

    private static PlayerSpawnerSO _shipParameterSO;

    public static PlayerSpawnerSO GetInstance()
    {
        if (_shipParameterSO == null)
            _shipParameterSO = Instantiate(AssetDatabase.LoadAssetAtPath<PlayerSpawnerSO>("Assets/Scripts/ScriptableObjects/Player/PlayerSpawnerSO.asset"));

        return _shipParameterSO;
    }

}
