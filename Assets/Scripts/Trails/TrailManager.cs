using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
    public const string TrailName = "Trail";

    [SerializeField] private GameObject _trailPrefab;

    private List<GameObject> _trails;
    private static TrailManager _instance;

    // Start is called before the first frame update
    void Start()
    {
        _trails = new List<GameObject>();
        _instance = gameObject.GetComponent<TrailManager>();
    }

    public static TrailManager GetInstance()
    {
        return _instance;
    }

    public void MoveTrailToTrailManager(GameObject trail)
    {
        // Add a new trail to the trails parent object
        AddTrailToObject(trail.transform.parent);

        // Move the trail to the trail manager
        trail.transform.SetParent(transform, true);
        _trails.Add(trail);
    }

    public void DestoryTrails()
    {
        foreach (var trail in _trails)
        {
            Destroy(trail);
        }

        _trails.Clear();
    }

    private void AddTrailToObject(Transform target)
    {
        var trail = Instantiate(_trailPrefab, target);
        trail.name = TrailName;
    }

}
