using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSpawnerSO", menuName = "ScriptableObjects/PlayerSpawnerSO")]
public class PlayerSpawnerSO : ConfigScriptableObject<PlayerSpawnerSO>
{
    public RandomFloat horizontalStartingVelocity;
}
