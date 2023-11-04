using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
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
        trail.transform.parent = gameObject.transform;
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

}
