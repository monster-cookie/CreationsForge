using System.Globalization;

namespace CreationsForge.DataValidationTests.Validation.Parsing;

public class SpriggitYamlDocument
{
    private readonly IReadOnlyDictionary<string, IReadOnlyList<string>> valuesByPath;
    private readonly IReadOnlyDictionary<string, int> listCountsByPath;

    public SpriggitYamlDocument(
        string filePath,
        IReadOnlyDictionary<string, IReadOnlyList<string>> valuesByPath,
        IReadOnlyDictionary<string, int> listCountsByPath)
    {
        FilePath = filePath;
        this.valuesByPath = valuesByPath;
        this.listCountsByPath = listCountsByPath;
    }

    public string FilePath { get; }

    public IReadOnlyDictionary<string, IReadOnlyList<string>> ValuesByPath => valuesByPath;

    public IReadOnlyDictionary<string, int> ListCountsByPath => listCountsByPath;

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

            var isListLine = trimmed.StartsWith("- ", StringComparison.Ordinal);
            while (frames.Count > 0 && ShouldPopFrame(frames[^1], indent, isListLine))
            {
                frames.RemoveAt(frames.Count - 1);
            }

            if (isListLine)
            {
                ReadListLine(lines, valuesByPath, listCountsByPath, frames, ref lineIndex, indent, trimmed);
                continue;
            }

            if (!TrySplitKeyValue(trimmed, out var key, out var scalar))
            {
                continue;
            }

            var path = frames.Count == 0
                ? key
                : frames[^1].Path + "." + key;

            ReadValue(lines, valuesByPath, frames, ref lineIndex, indent, path, scalar);
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

    public bool TryGetFormKey(out string? value)
    {
        return TryGetScalar("FormKey", out value);
    }

    public IReadOnlyDictionary<string, string> FlattenScalars()
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in valuesByPath)
        {
            if (pair.Value.Count == 1)
            {
                values[pair.Key] = pair.Value[0];
                continue;
            }

            for (var index = 0; index < pair.Value.Count; index++)
            {
                values[ExpandListIndex(pair.Key, index)] = pair.Value[index];
            }
        }

        foreach (var pair in listCountsByPath)
        {
            values[pair.Key + ".Count"] = pair.Value.ToString(CultureInfo.InvariantCulture);
        }

        return values;
    }

    private static void ReadListLine(
        IReadOnlyList<string> lines,
        IDictionary<string, List<string>> valuesByPath,
        IDictionary<string, int> listCountsByPath,
        IList<YamlFrame> frames,
        ref int lineIndex,
        int indent,
        string trimmed)
    {
        if (frames.Count == 0)
        {
            return;
        }

        if (!frames[^1].ListIndent.HasValue)
        {
            frames[^1] = frames[^1].WithListIndent(indent);
        }

        var listPath = frames[^1].Path;
        var itemIndex = listCountsByPath.TryGetValue(listPath, out var currentListCount)
            ? currentListCount
            : 0;
        listCountsByPath[listPath] = itemIndex + 1;
        var itemPath = listPath + "[" + itemIndex.ToString(CultureInfo.InvariantCulture) + "]";
        frames.Add(new YamlFrame(indent, itemPath, true));

        var listValue = trimmed[2..].Trim();
        if (TrySplitKeyValue(listValue, out var listKey, out var listScalar))
        {
            if (string.IsNullOrWhiteSpace(listScalar))
            {
                frames.Add(new YamlFrame(indent + 2, itemPath + "." + listKey, false));
                return;
            }

            ReadValue(lines, valuesByPath, frames, ref lineIndex, indent, itemPath + "." + listKey, listScalar);
        }
        else if (!string.IsNullOrWhiteSpace(listValue))
        {
            AddValue(valuesByPath, itemPath, NormalizeScalar(listValue));
        }
    }

    private static void ReadValue(
        IReadOnlyList<string> lines,
        IDictionary<string, List<string>> valuesByPath,
        IList<YamlFrame> frames,
        ref int lineIndex,
        int indent,
        string path,
        string scalar)
    {
        if (TryGetBlockScalar(scalar, out var blockScalarStyle, out var blockScalarChomping))
        {
            AddValue(valuesByPath, path, ReadBlockScalar(lines, ref lineIndex, indent, blockScalarStyle, blockScalarChomping));
            return;
        }

        if (string.IsNullOrWhiteSpace(scalar))
        {
            frames.Add(new YamlFrame(indent, path, false));
            return;
        }

        AddValue(valuesByPath, path, NormalizeScalar(scalar));
    }

    private static string ExpandListIndex(string path, int index)
    {
        var markerIndex = path.IndexOf("[]", StringComparison.Ordinal);
        return markerIndex < 0
            ? path + "[" + index.ToString(CultureInfo.InvariantCulture) + "]"
            : path[..markerIndex] + "[" + index.ToString(CultureInfo.InvariantCulture) + "]" + path[(markerIndex + 2)..];
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

    private static bool TryGetBlockScalar(string value, out char style, out char chomping)
    {
        style = '\0';
        chomping = '\0';

        if (string.IsNullOrWhiteSpace(value) || value[0] is not ('>' or '|'))
        {
            return false;
        }

        style = value[0];
        if (value.Length > 1 && value[1] is '-' or '+')
        {
            chomping = value[1];
        }

        return true;
    }

    private static string ReadBlockScalar(IReadOnlyList<string> lines, ref int lineIndex, int parentIndent, char style, char chomping)
    {
        var buffer = new List<string>();
        var firstLineIndent = int.MaxValue;

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

        if (buffer.Count == 0)
        {
            return string.Empty;
        }

        if (firstLineIndent == int.MaxValue)
        {
            firstLineIndent = 0;
        }

        var normalizedLines = buffer
            .Select(line => string.IsNullOrWhiteSpace(line)
                ? string.Empty
                : line[Math.Min(firstLineIndent, line.Length)..])
            .ToList();
        var value = style == '>'
            ? FoldBlockScalar(normalizedLines)
            : string.Join("\r\n", normalizedLines);

        return chomping == '-'
            ? value
            : value + "\r\n";
    }

    private static string FoldBlockScalar(IReadOnlyList<string> lines)
    {
        var builder = new System.Text.StringBuilder();
        var pendingBlankLines = 0;

        foreach (var line in lines)
        {
            if (line.Length == 0)
            {
                if (builder.Length > 0)
                {
                    pendingBlankLines++;
                }

                continue;
            }

            if (builder.Length == 0)
            {
                builder.Append(line);
                continue;
            }

            if (pendingBlankLines == 0)
            {
                builder.Append(' ');
                builder.Append(line);
                continue;
            }

            for (var index = 0; index < pendingBlankLines; index++)
            {
                builder.Append("\r\n");
            }

            builder.Append(line);
            pendingBlankLines = 0;
        }

        return builder.ToString();
    }

    private static string NormalizeScalar(string value)
    {
        if (value.Length >= 2 &&
            ((value.StartsWith('\'') && value.EndsWith('\'')) || (value.StartsWith('"') && value.EndsWith('"'))))
        {
            return value[1..^1];
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

    private static bool ShouldPopFrame(YamlFrame frame, int indent, bool isListLine)
    {
        if (indent > frame.Indent)
        {
            return false;
        }

        if (isListLine && frame.ListIndent.HasValue)
        {
            return indent != frame.ListIndent.Value || frame.IsListItem;
        }

        return !isListLine ||
               indent != frame.Indent ||
               frame.IsListItem ||
               IsTransparentSpriggitListWrapper(frame.Path);
    }

    private static bool IsTransparentSpriggitListWrapper(string path)
    {
        return path.EndsWith(".Values", StringComparison.OrdinalIgnoreCase);
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
        public YamlFrame(int indent, string path, bool isListItem, int? listIndent = null)
        {
            Indent = indent;
            Path = path;
            IsListItem = isListItem;
            ListIndent = listIndent;
        }

        public int Indent { get; }

        public string Path { get; }

        public bool IsListItem { get; }

        public int? ListIndent { get; }

        public YamlFrame WithListIndent(int listIndent)
        {
            return new YamlFrame(Indent, Path, IsListItem, listIndent);
        }
    }
}
