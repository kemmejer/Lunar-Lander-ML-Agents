using System.Collections.Generic;
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

    /// <summary>
    /// Moves the trails gameobject from the ships gameobject to the trail manager object.
    /// This ensures that the trail stays visible even when the ship becomes invisible
    /// </summary>
    /// <param name="trail">Trail of the ship to move to the trail manager</param>
    public void MoveTrailToTrailManager(GameObject trail)
    {
        // Add a new trail to the trails parent object
        AddTrailToObject(trail.transform.parent);

        // Move the trail to the trail manager
        trail.transform.SetParent(transform, true);
        _trails.Add(trail);
    }

    /// <summary>
    /// Destroys all trails managed by the trail manager
    /// </summary>
    public void DestroyTrails()
    {
        foreach (var trail in _trails)
        {
            Destroy(trail);
        }

        _trails.Clear();
    }

    /// <summary>
    /// Adds trail to the specified object
    /// </summary>
    /// <param name="target">Target to add the trail to</param>
    private void AddTrailToObject(Transform target)
    {
        var trail = Instantiate(_trailPrefab, target);
        trail.name = TrailName;
    }

}
