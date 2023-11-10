using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundGeneratorSO", menuName = "ScriptableObjects/GroundGeneratorSO")]
public class GroundGeneratorSO : ScriptableObject
{
    public RandomFloat baseHeight;
    public RandomFloat noiseHeight;
    public RandomFloat noiseScale;
    public RandomInt seed;
    public RandomInt resolution;

    private static GroundGeneratorSO _groundGeneratorSO;

    public static GroundGeneratorSO GetInstance()
    {
        if (_groundGeneratorSO == null)
            _groundGeneratorSO = Instantiate(Resources.Load<GroundGeneratorSO>("GroundGeneratorSO"));

        IRandomValue.GenerateValuesForAllFields(_groundGeneratorSO);

        return _groundGeneratorSO;
    }
}
