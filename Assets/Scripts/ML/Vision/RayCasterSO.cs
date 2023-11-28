
using UnityEngine;

[CreateAssetMenu(fileName = "RayCasterSO", menuName = "ScriptableObjects/RayCasterSO")]
public class RayCasterSO : ConfigScriptableObject<RayCasterSO>
{
    public RandomFloat angle;
    public int raysPerDirection;
    public static bool drawRays;

    public int RayCount => raysPerDirection * 2 + 1;
}
