using System.Globalization;
using System.Text.Json;

namespace CreationsForge.Console.Mcp;

/// <summary>
/// Builds stateless immediate-child JSON Pointer pages from detached engine JSON values.
/// </summary>
internal static class McpJsonPager
{
    /// <summary>The stable transport policy bound into every inspect and compare cursor.</summary>
    internal const string Policy = "immediate_children_v1";

    /// <summary>Validates a canonical RFC 6901 JSON Pointer without resolving it.</summary>
    /// <param name="path">The empty root pointer or slash-prefixed pointer.</param>
    /// <param name="error">Receives a human-readable validation error.</param>
    /// <returns><see langword="true"/> when every escape sequence is canonical.</returns>
    internal static bool TryValidatePath(string path, out string error)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (path.Length > McpInput.MaximumJsonPointerLength)
        {
            error = $"Argument 'path' cannot exceed {McpInput.MaximumJsonPointerLength} characters.";
            return false;
        }

        if (path.Length == 0)
        {
            error = string.Empty;
            return true;
        }

        if (path[0] != '/')
        {
            error = "Argument 'path' must be an empty root path or a canonical slash-prefixed JSON Pointer.";
            return false;
        }

        for (var index = 0; index < path.Length; index++)
        {
            if (path[index] != '~')
            {
                continue;
            }

            if (++index >= path.Length || path[index] is not '0' and not '1')
            {
                error = "Argument 'path' contains an invalid JSON Pointer escape.";
                return false;
            }
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Creates one bounded page at an exact JSON Pointer path.</summary>
    /// <param name="root">The complete detached view, or <see langword="null"/> for an absent value.</param>
    /// <param name="path">The validated JSON Pointer path.</param>
    /// <param name="position">The next zero-based child or UTF-16 string position.</param>
    /// <param name="maximumResults">The maximum immediate children or string units requested.</param>
    /// <param name="cursorFactory">Creates a cursor for the next position.</param>
    /// <param name="page">Receives the detached page.</param>
    /// <param name="error">Receives a path or position error.</param>
    /// <returns><see langword="true"/> when the path and position identify a valid page.</returns>
    internal static bool TryCreatePage(
        JsonElement? root,
        string path,
        int position,
        int maximumResults,
        Func<int, string> cursorFactory,
        out JsonElement page,
        out string error)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(cursorFactory);
        page = default;
        if (!TryResolve(root, path, out var value, out var absent) || position < 0)
        {
            error = $"JSON Pointer path '{path}' does not identify a value in the selected view.";
            return false;
        }

        if (absent)
        {
            if (position != 0)
            {
                error = "The cursor position is outside the absent value.";
                return false;
            }

            page = JsonSerializer.SerializeToElement(new
            {
                path,
                kind = "absent",
                count = 0,
            });
            error = string.Empty;
            return true;
        }

        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                return TryCreateObjectPage(value, path, position, maximumResults, cursorFactory, out page, out error);
            case JsonValueKind.Array:
                return TryCreateArrayPage(value, path, position, maximumResults, cursorFactory, out page, out error);
            case JsonValueKind.String:
                return TryCreateStringPage(value.GetString() ?? string.Empty, path, position, maximumResults, cursorFactory, out page, out error);
            case JsonValueKind.Number:
                if (position != 0)
                {
                    error = "The cursor position is outside the scalar value.";
                    return false;
                }

                page = JsonSerializer.SerializeToElement(new
                {
                    path,
                    kind = "number",
                    rawValue = value.GetRawText(),
                    count = 1,
                });
                error = string.Empty;
                return true;
            case JsonValueKind.True:
            case JsonValueKind.False:
                if (position != 0)
                {
                    error = "The cursor position is outside the scalar value.";
                    return false;
                }

                page = JsonSerializer.SerializeToElement(new
                {
                    path,
                    kind = "boolean",
                    value = value.GetBoolean(),
                    count = 1,
                });
                error = string.Empty;
                return true;
            case JsonValueKind.Null:
                if (position != 0)
                {
                    error = "The cursor position is outside the null value.";
                    return false;
                }

                page = JsonSerializer.SerializeToElement(new
                {
                    path,
                    kind = "null",
                    count = 0,
                });
                error = string.Empty;
                return true;
            default:
                error = "The selected view contains an undefined JSON value.";
                return false;
        }
    }

    /// <summary>Creates an immediate-property page without embedding container values.</summary>
    /// <param name="value">The selected object.</param>
    /// <param name="path">The object's JSON Pointer.</param>
    /// <param name="position">The first property ordinal.</param>
    /// <param name="maximumResults">The maximum returned property descriptors.</param>
    /// <param name="cursorFactory">Creates a continuation cursor.</param>
    /// <param name="page">Receives the object page.</param>
    /// <param name="error">Receives a position error.</param>
    /// <returns><see langword="true"/> when the position is valid.</returns>
    private static bool TryCreateObjectPage(
        JsonElement value,
        string path,
        int position,
        int maximumResults,
        Func<int, string> cursorFactory,
        out JsonElement page,
        out string error)
    {
        var properties = value.EnumerateObject().ToArray();
        if (position > properties.Length)
        {
            page = default;
            error = "The cursor position is outside the selected object.";
            return false;
        }

        var children = properties
            .Skip(position)
            .Take(maximumResults)
            .Select(property => new
            {
                property = property.Name,
                path = AppendPath(path, property.Name),
                kind = Kind(property.Value),
                count = Count(property.Value),
            })
            .ToArray();
        var nextPosition = position + children.Length;
        page = JsonSerializer.SerializeToElement(new
        {
            path,
            kind = "object",
            offset = position,
            count = children.Length,
            totalCount = properties.Length,
            children,
            cursor = nextPosition < properties.Length ? cursorFactory(nextPosition) : null,
        });
        error = string.Empty;
        return true;
    }

    /// <summary>Creates an immediate-index page without embedding container values.</summary>
    /// <param name="value">The selected array.</param>
    /// <param name="path">The array's JSON Pointer.</param>
    /// <param name="position">The first array index.</param>
    /// <param name="maximumResults">The maximum returned index descriptors.</param>
    /// <param name="cursorFactory">Creates a continuation cursor.</param>
    /// <param name="page">Receives the array page.</param>
    /// <param name="error">Receives a position error.</param>
    /// <returns><see langword="true"/> when the position is valid.</returns>
    private static bool TryCreateArrayPage(
        JsonElement value,
        string path,
        int position,
        int maximumResults,
        Func<int, string> cursorFactory,
        out JsonElement page,
        out string error)
    {
        var values = value.EnumerateArray().ToArray();
        if (position > values.Length)
        {
            page = default;
            error = "The cursor position is outside the selected array.";
            return false;
        }

        var children = values
            .Skip(position)
            .Take(maximumResults)
            .Select((child, offset) => new
            {
                index = position + offset,
                path = AppendPath(path, (position + offset).ToString(CultureInfo.InvariantCulture)),
                kind = Kind(child),
                count = Count(child),
            })
            .ToArray();
        var nextPosition = position + children.Length;
        page = JsonSerializer.SerializeToElement(new
        {
            path,
            kind = "array",
            offset = position,
            count = children.Length,
            totalCount = values.Length,
            children,
            cursor = nextPosition < values.Length ? cursorFactory(nextPosition) : null,
        });
        error = string.Empty;
        return true;
    }

    /// <summary>Creates a Unicode-safe string chunk without splitting a UTF-16 surrogate pair.</summary>
    /// <param name="value">The complete selected string.</param>
    /// <param name="path">The string's JSON Pointer.</param>
    /// <param name="position">The first UTF-16 code-unit offset.</param>
    /// <param name="maximumResults">The desired maximum UTF-16 units.</param>
    /// <param name="cursorFactory">Creates a continuation cursor.</param>
    /// <param name="page">Receives the string page.</param>
    /// <param name="error">Receives a position error.</param>
    /// <returns><see langword="true"/> when the position is a Unicode boundary.</returns>
    private static bool TryCreateStringPage(
        string value,
        string path,
        int position,
        int maximumResults,
        Func<int, string> cursorFactory,
        out JsonElement page,
        out string error)
    {
        if (position > value.Length ||
            (position < value.Length && position > 0 && char.IsLowSurrogate(value[position])))
        {
            page = default;
            error = "The cursor position is outside the selected string or splits a Unicode surrogate pair.";
            return false;
        }

        var length = Math.Min(maximumResults, value.Length - position);
        if (length > 0 &&
            position + length < value.Length &&
            char.IsHighSurrogate(value[position + length - 1]) &&
            char.IsLowSurrogate(value[position + length]))
        {
            length++;
        }

        var chunk = value.Substring(position, length);
        var nextPosition = position + length;
        page = JsonSerializer.SerializeToElement(new
        {
            path,
            kind = "string",
            offset = position,
            length,
            totalLength = value.Length,
            value = chunk,
            cursor = nextPosition < value.Length ? cursorFactory(nextPosition) : null,
        });
        error = string.Empty;
        return true;
    }

    /// <summary>Resolves a canonical pointer against a present or absent root.</summary>
    /// <param name="root">The root view, or <see langword="null"/> when absent.</param>
    /// <param name="path">The canonical JSON Pointer.</param>
    /// <param name="value">Receives the selected JSON value.</param>
    /// <param name="absent">Receives whether the selected root is absent.</param>
    /// <returns><see langword="true"/> when the path resolves.</returns>
    private static bool TryResolve(
        JsonElement? root,
        string path,
        out JsonElement value,
        out bool absent)
    {
        value = default;
        absent = !root.HasValue;
        if (path.Length == 0)
        {
            if (root.HasValue)
            {
                value = root.Value;
            }

            return true;
        }

        if (!root.HasValue)
        {
            return false;
        }

        absent = false;
        value = root.Value;
        foreach (var encodedToken in path[1..].Split('/'))
        {
            var token = DecodeToken(encodedToken);
            if (value.ValueKind == JsonValueKind.Object)
            {
                if (!value.TryGetProperty(token, out value))
                {
                    return false;
                }
            }
            else if (value.ValueKind == JsonValueKind.Array)
            {
                if (!TryParseArrayIndex(token, out var index) || index >= value.GetArrayLength())
                {
                    return false;
                }

                value = value[index];
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Parses one canonical non-negative JSON Pointer array index.</summary>
    /// <param name="token">The decoded pointer token.</param>
    /// <param name="index">Receives the parsed index.</param>
    /// <returns><see langword="true"/> when the token has no sign or leading zero.</returns>
    private static bool TryParseArrayIndex(string token, out int index)
    {
        index = -1;
        return token.Length != 0 &&
            (token.Length == 1 || token[0] != '0') &&
            int.TryParse(token, NumberStyles.None, CultureInfo.InvariantCulture, out index) &&
            index >= 0;
    }

    /// <summary>Appends and escapes one token to a canonical JSON Pointer.</summary>
    /// <param name="path">The parent pointer.</param>
    /// <param name="token">The decoded child token.</param>
    /// <returns>The child pointer.</returns>
    private static string AppendPath(string path, string token)
    {
        return $"{path}/{token.Replace("~", "~0", StringComparison.Ordinal).Replace("/", "~1", StringComparison.Ordinal)}";
    }

    /// <summary>Decodes one already-validated JSON Pointer token.</summary>
    /// <param name="token">The encoded token.</param>
    /// <returns>The decoded property name or index text.</returns>
    private static string DecodeToken(string token)
    {
        return token.Replace("~1", "/", StringComparison.Ordinal).Replace("~0", "~", StringComparison.Ordinal);
    }

    /// <summary>Returns the stable JSON value-kind name.</summary>
    /// <param name="value">The JSON value.</param>
    /// <returns>The MCP kind name.</returns>
    private static string Kind(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Object => "object",
            JsonValueKind.Array => "array",
            JsonValueKind.String => "string",
            JsonValueKind.Number => "number",
            JsonValueKind.True or JsonValueKind.False => "boolean",
            JsonValueKind.Null => "null",
            _ => "undefined",
        };
    }

    /// <summary>Returns an immediate child, UTF-16 unit, scalar-presence, or zero-null count.</summary>
    /// <param name="value">The JSON value to describe.</param>
    /// <returns>The value's bounded-navigation count.</returns>
    private static int Count(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.Object => value.EnumerateObject().Count(),
            JsonValueKind.Array => value.GetArrayLength(),
            JsonValueKind.String => value.GetString()?.Length ?? 0,
            JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False => 1,
            _ => 0,
        };
    }
}
