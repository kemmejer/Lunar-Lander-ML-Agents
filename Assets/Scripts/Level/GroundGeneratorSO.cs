using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GroundGeneratorSO", menuName = "ScriptableObjects/GroundGeneratorSO")]
public class GroundGeneratorSO : ScriptableObject
{
    public RandomValue baseHeight;
    public RandomValue noiseHeight;
    public RandomValue noiseScale;
    public int seed;
    public int resolution;

    private static GroundGeneratorSO _groundGeneratorSO;

    public static GroundGeneratorSO GetInstance()
    {
        if (_groundGeneratorSO == null)
            _groundGeneratorSO = Instantiate(Resources.Load<GroundGeneratorSO>("GroundGeneratorSO"));

        RandomValue.GenerateValuesForAllFields(_groundGeneratorSO);

        return _groundGeneratorSO;
    }
}
