
using System.Linq;

public interface IRandomValue
{
    public void GenerateRandomValue();

    /// <summary>
    /// Iterates over all RandomFloat fields of the provided object and generates random values
    /// </summary>
    /// <typeparam name="T">Type of the class</typeparam>
    /// <param name="obj">Instance of the class</param>
    public static void GenerateValuesForAllFields<T>(T obj)
    {
        var fields = typeof(T).GetFields().Where(field => typeof(IRandomValue).IsAssignableFrom(field.FieldType));
        foreach (var field in fields)
        {
            var randomValue = field.GetValue(obj) as IRandomValue;
            randomValue.GenerateRandomValue();
        }
    }
}

