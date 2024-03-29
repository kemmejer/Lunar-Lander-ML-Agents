using UnityEngine;

[CreateAssetMenu(fileName = "GroundGeneratorSO", menuName = "ScriptableObjects/GroundGeneratorSO")]
public class GroundGeneratorSO : ConfigScriptableObject<GroundGeneratorSO>
{
    public RandomFloat baseHeight;
    public RandomFloat noiseHeight;
    public RandomFloat noiseScale;
    public RandomInt seed;
    public RandomInt resolution;
    public RandomInt regenerateInterval;
    public bool regenerateGroundWhileTraining;
}
