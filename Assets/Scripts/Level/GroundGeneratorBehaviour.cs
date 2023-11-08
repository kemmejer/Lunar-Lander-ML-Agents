using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class GroundGeneratorBehaviour : MonoBehaviour
{
    private GroundGeneratorSO _groundGeneratorSO;
    private Mesh _mesh;
    private List<Vector3> _vertices;
    private List<int> _triangles;
    private List<Vector2> _collider;

    private static GroundGeneratorBehaviour _instance;

    public static GroundGeneratorBehaviour GetInstance()
    {
        return _instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        _instance = GetComponent<GroundGeneratorBehaviour>();
        _groundGeneratorSO = GroundGeneratorSO.GetInstance();

        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        GenerateGround();
    }

    public void GenerateGround()
    {
        RandomValue.GenerateValuesForAllFields(_groundGeneratorSO);

        _vertices = new List<Vector3>();
        _triangles = new List<int>();
        _collider = new List<Vector2>();


        GenerateBase();
        GenerateTop();

        UpdateMesh();
    }

    private void GenerateTop()
    {
        var screenBounds = CameraHelper.GetScreenBounds();
        float baseY = screenBounds.min.y + _groundGeneratorSO.baseHeight.value;

        if(_groundGeneratorSO.noiseHeight.value == 0)
        {
            _collider.Add(screenBounds.min);
            _collider.Add(new Vector2(screenBounds.min.x, baseY));
            _collider.Add(new Vector2(screenBounds.max.x, baseY));
            _collider.Add(new Vector2(screenBounds.max.x, screenBounds.min.y));

            return;
        }

        float x = screenBounds.min.x;
        float stepSize = screenBounds.size.x / _groundGeneratorSO.resolution;

        float seed = _groundGeneratorSO.seed.GetHashCode();
        int triangleIndex = _vertices.Count;

        _collider.Add(screenBounds.min);

        // Triangle vertex order
        //1-3-5
        //|\|\|...
        //0-2-4
        for (int i = 0; i <= _groundGeneratorSO.resolution; i++)
        {
            float noise = Mathf.PerlinNoise1D(x * _groundGeneratorSO.noiseScale.value + seed) * _groundGeneratorSO.noiseHeight.value;
            float y = baseY + noise;

            _vertices.Add(new Vector3(x, baseY, 0.0f));
            _vertices.Add(new Vector3(x, y, 0.0f));
            _collider.Add(new Vector2(x, y));

            x += stepSize;
        }

        _collider.Add(new Vector2(screenBounds.max.x, screenBounds.min.y));

        // Clockwise triangle indices
        // 0-1-2 | 2-1-3 | 2-3-4 | 4-3-5 ...
        for (int i = 0; i < _groundGeneratorSO.resolution; i++)
        {
            _triangles.AddRange(new int[] {
                triangleIndex    , triangleIndex + 1, triangleIndex + 2,
                triangleIndex + 2, triangleIndex + 1, triangleIndex + 3});

            triangleIndex += 2;
        }
    }

    private void GenerateBase()
    {
        var screenBounds = CameraHelper.GetScreenBounds();
        var triangleIndex = _vertices.Count;

        //1-3
        //|\|
        //0-2
        _vertices.Add(new Vector3(screenBounds.min.x, screenBounds.min.y, 0.0f));
        _vertices.Add(new Vector3(screenBounds.min.x, screenBounds.min.y + _groundGeneratorSO.baseHeight.value, 0.0f));
        _vertices.Add(new Vector3(screenBounds.max.x, screenBounds.min.y, 0.0f));
        _vertices.Add(new Vector3(screenBounds.max.x, screenBounds.min.y + _groundGeneratorSO.baseHeight.value, 0.0f));

        // Clockwise triangle indices
        // 0-1-2 | 2-1-3
        _triangles.AddRange(new int[]{
            triangleIndex    , triangleIndex + 1, triangleIndex + 2,
            triangleIndex + 2, triangleIndex + 1, triangleIndex + 3 });
    }

    private void UpdateMesh()
    {
        _mesh.Clear();
        _mesh.vertices = _vertices.ToArray();
        _mesh.triangles = _triangles.ToArray();
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        var collider = GetComponent<PolygonCollider2D>();
        collider.SetPath(0, _collider);
    }
}
