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
        if (!trail.gameObject.activeInHierarchy)
            return;

        var trailCopy = Instantiate(trail, gameObject.transform);

        _trails.Add(trailCopy);
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
