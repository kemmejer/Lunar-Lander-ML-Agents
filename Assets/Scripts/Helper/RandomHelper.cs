using UnityEngine;

public static class RandomHelper
{
    public static Vector2 RandomInRange(in Vector2 min, in Vector2 max)
    {
        return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }

    public static Vector3 RandomInRange(in Vector3 min, in Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

    public static float GetSeededRandom(int seed)
    {
        return (float)new System.Random(seed).NextDouble();
    }

    public static int GetSeededRandomInRange(int seed, int min, int max)
    {
        return new System.Random(seed).Next(min, max);
    }
}
