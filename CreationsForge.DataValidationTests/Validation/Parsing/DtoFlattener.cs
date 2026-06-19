using System.Collections;
using System.Globalization;
using System.Reflection;
using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.DataValidationTests.Validation.Parsing;

public class DtoFlattener
{
    private static readonly HashSet<string> IgnoredRootProperties = new(StringComparer.Ordinal)
    {
        "Game",
        "ModKey",
        "ImportedAtUTC"
    };

    public IReadOnlyDictionary<string, string> Flatten(object instance)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        FlattenValue(values, string.Empty, instance);
        return values;
    }

    private static void FlattenValue(IDictionary<string, string> values, string path, object? value)
    {
        if (string.IsNullOrWhiteSpace(path) && value is not null)
        {
            foreach (var property in GetReadableProperties(value.GetType()))
            {
                if (IgnoredRootProperties.Contains(property.Name))
                {
                    continue;
                }

                FlattenValue(values, property.Name, property.GetValue(value));
            }

            return;
        }

        if (value is null)
        {
            return;
        }

        if (value is FormKeyDTO formKey)
        {
            values[path] = FormatFormKey(formKey);
            return;
        }

        if (value is ModKeyDTO modKey)
        {
            values[path] = modKey.FileName;
            return;
        }

        if (IsScalar(value))
        {
            values[path] = ConvertScalar(value);
            return;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var index = 0;
            foreach (var item in enumerable)
            {
                FlattenValue(values, path + "[" + index.ToString(CultureInfo.InvariantCulture) + "]", item);
                index++;
            }

            values[path + ".Count"] = index.ToString(CultureInfo.InvariantCulture);
            return;
        }

        foreach (var property in GetReadableProperties(value.GetType()))
        {
            FlattenValue(values, path + "." + property.Name, property.GetValue(value));
        }
    }

    private static IReadOnlyList<PropertyInfo> GetReadableProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetIndexParameters().Length == 0)
            .OrderBy(property => property.Name, StringComparer.Ordinal)
            .ToList();
    }

    private static bool IsScalar(object value)
    {
        var type = value.GetType();
        return type.IsPrimitive ||
               type.IsEnum ||
               value is string or decimal or DateTime or Guid;
    }

    private static string ConvertScalar(object value)
    {
        return value switch
        {
            bool boolean => boolean.ToString(CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };
    }

    private static string FormatFormKey(FormKeyDTO formKey)
    {
        return formKey.Id.ToString("X6", CultureInfo.InvariantCulture) + ":" + formKey.ModKey.FileName;
    }
}
