using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundGeneratorSO", menuName = "ScriptableObjects/GroundGeneratorSO")]
public class GroundGeneratorSO : ConfigScriptableObject<GroundGeneratorSO>
{
    public RandomFloat baseHeight;
    public RandomFloat noiseHeight;
    public RandomFloat noiseScale;
    public RandomInt seed;
    public RandomInt resolution;
}
