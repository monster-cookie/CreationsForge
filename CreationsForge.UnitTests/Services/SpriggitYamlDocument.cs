using System.Globalization;

namespace CreationsForge.UnitTests.Services;

public class SpriggitYamlDocument
{
    private readonly IReadOnlyDictionary<string, IReadOnlyList<string>> valuesByPath;
    private readonly IReadOnlyDictionary<string, int> listCountsByPath;

    public SpriggitYamlDocument(string filePath, IReadOnlyDictionary<string, IReadOnlyList<string>> valuesByPath, IReadOnlyDictionary<string, int> listCountsByPath)
    {
        FilePath = filePath;
        this.valuesByPath = valuesByPath;
        this.listCountsByPath = listCountsByPath;
    }

    public string FilePath { get; }

    public static SpriggitYamlDocument Load(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var valuesByPath = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        var listCountsByPath = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var frames = new List<YamlFrame>();

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var rawLine = lines[lineIndex];
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                continue;
            }

            var indent = GetIndent(rawLine);
            var trimmed = rawLine.Trim();
            if (trimmed.StartsWith('#'))
            {
                continue;
            }

            while (frames.Count > 0 && indent <= frames[^1].Indent)
            {
                frames.RemoveAt(frames.Count - 1);
            }

            if (trimmed.StartsWith("- ", StringComparison.Ordinal))
            {
                if (frames.Count == 0)
                {
                    continue;
                }

                var listPath = frames[^1].Path;
                listCountsByPath[listPath] = listCountsByPath.GetValueOrDefault(listPath) + 1;
                var itemPath = listPath + "[]";
                frames.Add(new YamlFrame(indent, itemPath));

                var listValue = trimmed[2..].Trim();
                if (TrySplitKeyValue(listValue, out var listKey, out var listScalar))
                {
                    var propertyPath = itemPath + "." + listKey;
                    if (IsBlockScalar(listScalar))
                    {
                        AddValue(valuesByPath, propertyPath, ReadBlockScalar(lines, ref lineIndex, indent));
                    }
                    else if (string.IsNullOrWhiteSpace(listScalar))
                    {
                        frames.Add(new YamlFrame(indent + 1, propertyPath));
                    }
                    else
                    {
                        AddValue(valuesByPath, propertyPath, NormalizeScalar(listScalar));
                    }
                }
                else if (!string.IsNullOrWhiteSpace(listValue))
                {
                    AddValue(valuesByPath, itemPath, NormalizeScalar(listValue));
                }

                continue;
            }

            if (!TrySplitKeyValue(trimmed, out var key, out var scalar))
            {
                continue;
            }

            var path = frames.Count == 0
                ? key
                : frames[^1].Path + "." + key;

            if (IsBlockScalar(scalar))
            {
                AddValue(valuesByPath, path, ReadBlockScalar(lines, ref lineIndex, indent));
                continue;
            }

            if (string.IsNullOrWhiteSpace(scalar))
            {
                frames.Add(new YamlFrame(indent, path));
                continue;
            }

            AddValue(valuesByPath, path, NormalizeScalar(scalar));
        }

        return new SpriggitYamlDocument(
            filePath,
            valuesByPath.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<string>)pair.Value, StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, int>(listCountsByPath, StringComparer.OrdinalIgnoreCase));
    }

    public bool HasPath(string path)
    {
        return valuesByPath.ContainsKey(path) || listCountsByPath.ContainsKey(path);
    }

    public int GetListItemCount(string path)
    {
        return listCountsByPath.GetValueOrDefault(path);
    }

    public bool TryGetScalar(string path, out string? value)
    {
        if (valuesByPath.TryGetValue(path, out var values) && values.Count > 0)
        {
            value = values[0];
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetLocalizedEnglishString(string path, out string? value)
    {
        value = null;
        if (!valuesByPath.TryGetValue(path + ".Values[].Language", out var languages) ||
            !valuesByPath.TryGetValue(path + ".Values[].String", out var strings))
        {
            return false;
        }

        for (var index = 0; index < languages.Count && index < strings.Count; index++)
        {
            if (!string.Equals(languages[index], "English", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            value = strings[index];
            return !string.IsNullOrWhiteSpace(value);
        }

        return false;
    }

    public bool TryGetFormKey(out string? value)
    {
        return TryGetScalar("FormKey", out value);
    }

    public bool TryGetInt32(string path, out int value)
    {
        value = default;
        return TryGetScalar(path, out var scalar) &&
               int.TryParse(scalar, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    public bool TryGetInt64(string path, out long value)
    {
        value = default;
        return TryGetScalar(path, out var scalar) &&
               long.TryParse(scalar, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    public bool TryGetDouble(string path, out double value)
    {
        value = default;
        return TryGetScalar(path, out var scalar) &&
               double.TryParse(scalar, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out value);
    }

    private static bool TrySplitKeyValue(string line, out string key, out string value)
    {
        var separatorIndex = line.IndexOf(':', StringComparison.Ordinal);
        if (separatorIndex <= 0)
        {
            key = string.Empty;
            value = string.Empty;
            return false;
        }

        key = line[..separatorIndex].Trim();
        value = line[(separatorIndex + 1)..].Trim();
        return true;
    }

    private static bool IsBlockScalar(string value)
    {
        return value is ">" or ">-" or "|" or "|-";
    }

    private static string ReadBlockScalar(IReadOnlyList<string> lines, ref int lineIndex, int parentIndent)
    {
        var buffer = new List<string>();
        var firstLineIndent = parentIndent + 1;

        for (var nextIndex = lineIndex + 1; nextIndex < lines.Count; nextIndex++)
        {
            var nextLine = lines[nextIndex];
            if (string.IsNullOrWhiteSpace(nextLine))
            {
                buffer.Add(string.Empty);
                lineIndex = nextIndex;
                continue;
            }

            var nextIndent = GetIndent(nextLine);
            if (nextIndent <= parentIndent)
            {
                break;
            }

            firstLineIndent = Math.Min(firstLineIndent, nextIndent);
            buffer.Add(nextLine);
            lineIndex = nextIndex;
        }

        return string.Join(
            "\n",
            buffer.Select(line => string.IsNullOrWhiteSpace(line)
                ? string.Empty
                : line[Math.Min(firstLineIndent, line.Length)..].TrimEnd()));
    }

    private static string NormalizeScalar(string value)
    {
        if (value.Length >= 2)
        {
            if ((value.StartsWith('\'') && value.EndsWith('\'')) || (value.StartsWith('"') && value.EndsWith('"')))
            {
                return value[1..^1];
            }
        }

        return value;
    }

    private static int GetIndent(string line)
    {
        var index = 0;
        while (index < line.Length && char.IsWhiteSpace(line[index]))
        {
            index++;
        }

        return index;
    }

    private static void AddValue(IDictionary<string, List<string>> valuesByPath, string path, string value)
    {
        if (!valuesByPath.TryGetValue(path, out var values))
        {
            values = new List<string>();
            valuesByPath[path] = values;
        }

        values.Add(value);
    }

    private sealed class YamlFrame
    {
        public YamlFrame(int indent, string path)
        {
            Indent = indent;
            Path = path;
        }

        public int Indent { get; }

        public string Path { get; }
    }
}
