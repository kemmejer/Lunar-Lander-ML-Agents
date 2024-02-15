using UnityEngine;

public static class RandomHelper
{
    /// <summary>
    /// Returns a random vector with its values in range [min, max]
    /// </summary>
    /// <param name="min">Min value (inclusive)</param>
    /// <param name="max">Max value (inclusive)</param>
    /// <returns>Random vector in range [min, max]</returns>
    public static Vector2 RandomInRange(in Vector2 min, in Vector2 max)
    {
        return new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
    }

    /// <summary>
    /// Returns a random vector with its values in range [min, max]
    /// </summary>
    /// <param name="min">Min value (inclusive)</param>
    /// <param name="max">Max value (inclusive)</param>
    /// <returns>Random vector in range [min, max]</returns>
    public static Vector3 RandomInRange(in Vector3 min, in Vector3 max)
    {
        return new Vector3(Random.Range(min.x, max.x), Random.Range(min.y, max.y), Random.Range(min.z, max.z));
    }

    /// <summary>
    /// Returns a random value using the provided seed
    /// </summary>
    /// <param name="seed">Seed for the random generator</param>
    /// <returns>Random double between 0.0 and 1.0</returns>
    public static float GetSeededRandom(int seed)
    {
        return (float)new System.Random(seed).NextDouble();
    }

    public static int GetSeededRandomInRange(int seed, int min, int max)
    {
        return new System.Random(seed).Next(min, max);
    }
}
