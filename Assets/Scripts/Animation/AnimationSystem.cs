using UnityEngine;

public class AnimationSystem : MonoBehaviour
{

    [SerializeField] private ParticleSystem explosionPrefab;
    private ParticleSystem _explosion;
    private const int ExplosionEmitCount = 3;

    private static AnimationSystem _animationSystem;

    // Start is called before the first frame update
    void Start()
    {
        _animationSystem = GetComponent<AnimationSystem>();
        _explosion = Instantiate(explosionPrefab, gameObject.transform);
    }

    public static AnimationSystem GetInstance()
    {
        return _animationSystem;
    }

    public void PlayExplosionAt(Vector3 position)
    {
        _explosion.transform.position = position;
        _explosion.Emit(ExplosionEmitCount);
    }
}

