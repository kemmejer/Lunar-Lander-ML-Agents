
using UnityEngine;

[CreateAssetMenu(fileName = "RayCasterSO", menuName = "ScriptableObjects/RayCasterSO")]
public class RayCasterSO : ConfigScriptableObject<RayCasterSO>
{
    public float angle;
    public int raysPerDirection;
    public float horizontalRayDistribution;
    public static bool drawRays;

    public int RayCount => raysPerDirection * 2 + 1;
}
