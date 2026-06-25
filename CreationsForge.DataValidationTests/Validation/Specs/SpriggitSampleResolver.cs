using System.Globalization;
using CreationsForge.Core.Enums;
using CreationsForge.DataValidationTests.Validation.Environment;
using CreationsForge.DataValidationTests.Validation.Parsing;

namespace CreationsForge.DataValidationTests.Validation.Specs;

public class SpriggitSampleResolver
{
    private readonly SpriggitEnvironmentLoader environmentLoader = new();
    private readonly Lazy<SpriggitEnvironmentConfiguration> spriggitEnvironment;

    public SpriggitSampleResolver()
    {
        spriggitEnvironment = new Lazy<SpriggitEnvironmentConfiguration>(() => environmentLoader.Load());
    }

    public IReadOnlyDictionary<string, string> LoadFields(ValidationSpec spec)
    {
        var path = FindSpriggitFile(spec.Game, spec.RecordType.TableName, spec.SampleName);
        var document = SpriggitYamlDocument.Load(path);
        return AddRootScalarLists(path, document.FlattenScalars());
    }

    private string FindSpriggitFile(SupportedGame game, string folder, string sampleName)
    {
        var root = environmentLoader.GetExtractionRoot(game, spriggitEnvironment.Value);
        var directory = Path.Combine(root, folder);
        if (!Directory.Exists(directory))
        {
            return FindSpriggitFile(root, sampleName);
        }

        var exactFileName = sampleName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase)
            ? sampleName
            : sampleName + ".yaml";
        var exactPath = Path.Combine(directory, exactFileName);
        if (File.Exists(exactPath))
        {
            return exactPath;
        }

        var matchingPath = Directory.GetFiles(directory, "*.yaml", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(path =>
                string.Equals(Path.GetFileNameWithoutExtension(path), sampleName, StringComparison.OrdinalIgnoreCase) ||
                Path.GetFileNameWithoutExtension(path).StartsWith(sampleName + " - ", StringComparison.OrdinalIgnoreCase));

        return matchingPath ?? FindSpriggitFile(root, sampleName);
    }

    private static string FindSpriggitFile(string root, string sampleName)
    {
        if (!Directory.Exists(root))
        {
            throw new DirectoryNotFoundException("Spriggit extraction root should exist: " + root);
        }

        var matchingPath = Directory.GetFiles(root, "*.yaml", SearchOption.AllDirectories)
            .FirstOrDefault(path =>
                string.Equals(Path.GetFileNameWithoutExtension(path), sampleName, StringComparison.OrdinalIgnoreCase) ||
                Path.GetFileNameWithoutExtension(path).StartsWith(sampleName + " - ", StringComparison.OrdinalIgnoreCase));

        if (matchingPath == null)
        {
            throw new FileNotFoundException("Unable to find Spriggit sample '" + sampleName + "' under " + root + ".");
        }

        return matchingPath;
    }

    private static IReadOnlyDictionary<string, string> AddRootScalarLists(string path, IReadOnlyDictionary<string, string> fields)
    {
        var mergedFields = new Dictionary<string, string>(fields, StringComparer.OrdinalIgnoreCase);
        var lines = File.ReadAllLines(path);

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            if (string.IsNullOrWhiteSpace(line) || GetIndent(line) != 0)
            {
                continue;
            }

            var trimmed = line.Trim();
            if (!trimmed.EndsWith(":", StringComparison.Ordinal) || trimmed.StartsWith("- ", StringComparison.Ordinal))
            {
                continue;
            }

            var fieldName = trimmed[..^1];
            var values = new List<string>();
            for (var valueIndex = lineIndex + 1; valueIndex < lines.Length; valueIndex++)
            {
                var valueLine = lines[valueIndex];
                if (string.IsNullOrWhiteSpace(valueLine))
                {
                    continue;
                }

                if (GetIndent(valueLine) != 0 || !valueLine.TrimStart().StartsWith("- ", StringComparison.Ordinal))
                {
                    break;
                }

                var value = valueLine.Trim()[2..].Trim();
                if (!IsRootScalarListValue(value))
                {
                    values.Clear();
                    break;
                }

                values.Add(NormalizeScalar(value));
            }

            if (values.Count == 0)
            {
                continue;
            }

            mergedFields[fieldName + ".Count"] = values.Count.ToString(CultureInfo.InvariantCulture);
            for (var valueIndex = 0; valueIndex < values.Count; valueIndex++)
            {
                mergedFields[fieldName + "[" + valueIndex.ToString(CultureInfo.InvariantCulture) + "]"] = values[valueIndex];
            }
        }

        return mergedFields;
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

    /// <summary>
    /// Determines whether a top-level YAML list row is a scalar alias candidate rather than an object wrapper.
    /// </summary>
    /// <param name="value">The raw list item text after the leading dash.</param>
    /// <returns><c>true</c> when the row can be safely projected as a scalar list item.</returns>
    private static bool IsRootScalarListValue(string value)
    {
        var isQuoted = value.Length >= 2 &&
                       ((value.StartsWith('\'') && value.EndsWith('\'')) ||
                        (value.StartsWith('"') && value.EndsWith('"')));
        return isQuoted ||
               (!value.EndsWith(":", StringComparison.Ordinal) &&
                !value.Contains(": ", StringComparison.Ordinal));
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
}
