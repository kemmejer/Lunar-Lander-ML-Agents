
using System.Linq;

public interface IRandomValue
{
    public void GenerateRandomValue();

    /// <summary>
    /// Recursively iterates over all fields and generates a random value for all IRandomValue fields
    /// </summary>
    /// <param name="obj">Instance of the class / struct</param>
    public static void GenerateValuesForAllFields(object obj)
    {
        var fields = obj.GetType().GetFields().Where(field => typeof(IRandomValue).IsAssignableFrom(field.FieldType));
        foreach (var field in fields)
        {
            var randomValue = field.GetValue(obj) as IRandomValue;
            randomValue.GenerateRandomValue();
        }

        var structs = obj.GetType().GetFields().Where(field => field.FieldType.IsValueType && !field.FieldType.IsPrimitive);
        foreach (var structField in structs)
        {
            var field = structField.GetValue(obj);
            GenerateValuesForAllFields(field);
        }
    }
}

