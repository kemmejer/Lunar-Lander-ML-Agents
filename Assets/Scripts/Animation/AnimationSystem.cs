using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    public static AnimationSystem animationSystem;

    [SerializeField] private ParticleSystem explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        if (animationSystem != null)
            Destroy(animationSystem);
        else
            animationSystem = this;

        DontDestroyOnLoad(this);
    }

    public void PlayExplosionAt(Vector3 position)
    {
        var explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosion.Play();
    }
}
