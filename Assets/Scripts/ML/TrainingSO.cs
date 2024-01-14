using UnityEngine;

[CreateAssetMenu(fileName = "_trainingSO", menuName = "ScriptableObjects/_trainingSO")]
public class TrainingSO : ConfigScriptableObject<TrainingSO>
{
    public int shipCount;
    public int decisionInterval;
}
