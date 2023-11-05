using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{

    [SerializeField] private ParticleSystem explosionPrefab;

    private static AnimationSystem _animationSystem;

    // Start is called before the first frame update
    void Start()
    {
        _animationSystem = GetComponent<AnimationSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public static AnimationSystem GetInstance()
    {
        return _animationSystem;
    }

    public void PlayExplosionAt(Vector3 position)
    {
        var explosion = Instantiate(explosionPrefab, position, Quaternion.identity, gameObject.transform);
        explosion.Play();
    }
}
