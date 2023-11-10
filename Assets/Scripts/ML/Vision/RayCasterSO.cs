
using UnityEngine;

[CreateAssetMenu(fileName = "RayCasterSO", menuName = "ScriptableObjects/RayCasterSO")]
public class RayCasterSO : ScriptableObject
{
    public RandomFloat angle;
    public RandomInt rayCount;
    public bool drawRays;

    private static RayCasterSO _rayCasterSO;

    public static RayCasterSO GetInstance()
    {
        if (_rayCasterSO == null)
            _rayCasterSO = Instantiate(Resources.Load<RayCasterSO>("RayCasterSO"));

        IRandomValue.GenerateValuesForAllFields(_rayCasterSO);

        return _rayCasterSO;
    }

    public static RayCasterSO GetInstanceCopy()
    {
        return Instantiate(GetInstance());
    }
}
