using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayCasterBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _rayCasterObject;

    private RayCasterSO _rayCasterSO;
    private RayCastBehaviour[] _casters;

    // Start is called before the first frame update
    void Awake()
    {
        _rayCasterSO = RayCasterSO.GetInstanceCopy();
        GenerateCaster();
    }

    void FixedUpdate()
    {
        CastRays();
    }

    private void OnDestroy()
    {
        foreach (var caster in _casters)
        {
            Destroy(caster);
        }
    }

    public void CastRays()
    {
        foreach (var caster in _casters)
        {
            caster.CastRay(_rayCasterSO.drawRays);
        }
    }

    public void SetColor(Color color)
    {
        foreach(var caster in _casters)
        {
            caster.SetColor(color);
        }
    }

    private void GenerateCaster()
    {
        var rayCount = _rayCasterSO.rayCount.value;
        var rayAngle = _rayCasterSO.angle.value;

        _casters = new RayCastBehaviour[rayCount];

        float angle = rayCount == 1 ? 0 : -rayAngle / 2.0f;
        float angleStep = rayAngle / (rayCount - 1);

        for(int i = 0; i < rayCount; i++)
        {
            var casterObject = Instantiate(_rayCasterObject, gameObject.transform, false);
            var caster = casterObject.GetComponent<RayCastBehaviour>();

            Vector2 direction =  Quaternion.Euler(0.0f, 0.0f, angle) * Vector2.down;
            caster.SetDirection(direction);

            _casters[i] = caster.GetComponent<RayCastBehaviour>();

            angle += angleStep;
        }
    }
}
