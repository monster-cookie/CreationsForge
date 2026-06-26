using System.Collections;
using System.Globalization;
using System.Reflection;
using CreationsForge.Core.DTOs.Plugins;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Utilities;

public static class SpriggitValueFormatter
{
    private const int MaxDepth = 3;

    public static string? Format(object? value)
    {
        return FormatValue(value, 0);
    }

    public static string? FormatFormKey(FormKeyDTO? formKey)
    {
        return formKey == null
            ? null
            : formKey.Id.ToString("X6", CultureInfo.InvariantCulture) + ":" + formKey.ModKey.FileName;
    }

    private static string? FormatValue(object? value, int depth)
    {
        if (value == null || IsNullLink(value))
        {
            return null;
        }

        if (value is string stringValue)
        {
            return string.IsNullOrWhiteSpace(stringValue) ? null : stringValue;
        }

        if (value is FormKey formKey)
        {
            return FormatFormKey(MapFormKey(formKey));
        }

        if (GetFormKeyProperty(value) is { } linkedFormKey)
        {
            return FormatFormKey(MapFormKey(linkedFormKey));
        }

        if (value is bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        var type = value.GetType();
        if (type.IsEnum || type == typeof(Guid) || type == typeof(DateTime))
        {
            return value.ToString();
        }

        if (value is IEnumerable enumerable)
        {
            var items = enumerable
                .Cast<object>()
                .Select(item => FormatValue(item, depth + 1))
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();
            return items.Count == 0 ? null : string.Join("; ", items);
        }

        if (depth >= MaxDepth)
        {
            return value.ToString();
        }

        var parts = new List<string>();
        foreach (var property in type.GetProperties())
        {
            if (property.GetIndexParameters().Length > 0 || !property.CanRead)
            {
                continue;
            }

            var propertyValue = GetPropertyValue(property, value);
            var formattedValue = FormatValue(propertyValue, depth + 1);
            if (!string.IsNullOrWhiteSpace(formattedValue))
            {
                parts.Add(property.Name + "=" + formattedValue);
            }
        }

        return parts.Count == 0 ? null : string.Join("; ", parts);
    }

    private static bool IsNullLink(object value)
    {
        var isNullProperty = value.GetType().GetProperty("IsNull");
        return isNullProperty?.GetValue(value) is true;
    }

    private static FormKey? GetFormKeyProperty(object value)
    {
        var formKeyProperty = value.GetType().GetProperty("FormKey");
        if (formKeyProperty?.GetValue(value) is FormKey formKey)
        {
            return formKey;
        }

        var nullableFormKeyProperty = value.GetType().GetProperty("FormKeyNullable");
        return nullableFormKeyProperty?.GetValue(value) is FormKey nullableFormKey ? nullableFormKey : null;
    }

    private static object? GetPropertyValue(PropertyInfo property, object instance)
    {
        try
        {
            return property.GetValue(instance);
        }
        catch (TargetInvocationException)
        {
            return null;
        }
    }

    private static FormKeyDTO MapFormKey(FormKey formKey)
    {
        return new FormKeyDTO
        {
            ModKey = ModKeyDTOMapper.FromModKey(formKey.ModKey),
            Id = formKey.ID
        };
    }
}
