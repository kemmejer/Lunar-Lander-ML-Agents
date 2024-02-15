using UnityEngine;

public class AnimationSystem : MonoBehaviour
{
    [SerializeField] private ParticleSystem explosionPrefab;
    private ParticleSystem _explosion;
    private const int ExplosionEmitCount = 3;

    private static AnimationSystem _animationSystem;

    void Start()
    {
        _animationSystem = GetComponent<AnimationSystem>();
        _explosion = Instantiate(explosionPrefab, gameObject.transform);
    }

    public static AnimationSystem GetInstance()
    {
        return _animationSystem;
    }

    /// <summary>
    /// Plays an explosion animation at the specified position
    /// </summary>
    /// <param name="position">Position to spawn the explosion at</param>
    public void PlayExplosionAt(Vector3 position)
    {
        _explosion.transform.position = position;
        _explosion.Emit(ExplosionEmitCount);
    }
}

