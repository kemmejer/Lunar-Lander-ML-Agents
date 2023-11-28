using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrainingSO", menuName = "ScriptableObjects/TrainingSO")]
public class TrainingSO : ConfigScriptableObject<TrainingSO>
{
    public int shipCount;
    public int decisionInterval;
}
