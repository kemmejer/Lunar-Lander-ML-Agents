using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class RayCasterBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject _rayCastPrefab;

    private RayCasterSO _rayCasterSO;
    private RayCastBehaviour[] _casters;

    void Awake()
    {
        _rayCasterSO = RayCasterSO.GetInstanceCopy();
        GenerateCaster();
    }

    private void OnDestroy()
    {
        foreach (var caster in _casters)
        {
            Destroy(caster);
        }
    }

    public RaycastHit2D[] CastRays()
    {
        var rayHits = new RaycastHit2D[_casters.Length];
        for (int i = 0; i < _casters.Length; i++)
        {
            rayHits[i] = _casters[i].CastRay();
        }

        return rayHits;
    }

    public void SetColor(Color color)
    {
        foreach (var caster in _casters)
        {
            caster.SetColor(color);
        }
    }

    private void GenerateCaster()
    {
        var rayCount = _rayCasterSO.RayCount;
        var rayAngle = _rayCasterSO.angle;
        var rayDistance = CameraHelper.GetScreenBounds().size.y;

        _casters = new RayCastBehaviour[rayCount];

        float angle = rayCount == 1 ? 0.0f : -rayAngle / 2.0f;
        float angleStep = rayAngle / (rayCount - 1);

        var shipWidth = gameObject.transform.parent.GetComponent<BoxCollider2D>().size.x * _rayCasterSO.horizontalRayDistribution;
        var rayTranslation = rayCount  == 1 ? 0.0f : -shipWidth / 2.0f;
        var rayTranslationStep = rayCount == 1 ? 0.0f : shipWidth / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            var casterObject = Instantiate(_rayCastPrefab, gameObject.transform, false);
            var caster = casterObject.GetComponent<RayCastBehaviour>();
            _casters[i] = caster;

            Vector2 direction = Quaternion.Euler(0.0f, 0.0f, angle) * Vector2.down;
            caster.SetDirection(direction);
            caster.SetCastDistance(rayDistance);
            casterObject.transform.Translate(new Vector3(rayTranslation, 0.0f, 0.0f));

            rayTranslation += rayTranslationStep;
            angle += angleStep;
        }
    }
}
