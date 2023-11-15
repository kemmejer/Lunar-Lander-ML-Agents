using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainingSO", menuName = "ScriptableObjects/TrainingSO")]
public class TrainingSO : ScriptableObject
{
    public int shipCount;
    public int decisionInterval;

    private static TrainingSO _trainingSO;

    public static TrainingSO GetInstance()
    {
        if (_trainingSO == null)
            _trainingSO = Instantiate(Resources.Load<TrainingSO>("TrainingSO"));

        IRandomValue.GenerateValuesForAllFields(_trainingSO);

        return _trainingSO;
    }

    public static TrainingSO GetInstanceCopy()
    {
        return Instantiate(GetInstance());
    }
}
