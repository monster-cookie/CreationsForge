using System.Text.Json;
using System.Text.Json.Serialization;
using CreationsForge.DataValidationTests.Validation.Models;

namespace CreationsForge.DataValidationTests.Validation.Reports;

public class SpriggitValidationReportWriter
{
    private const int FailedRecordFindingPreviewCount = 8;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public string Write(IReadOnlyList<SpriggitSampleValidationResult> results, string? game = null, string? recordType = null)
    {
        var outputDirectory = CreateOutputDirectory(game, recordType);
        var jsonPath = Path.Combine(outputDirectory, "SpriggitValidationReport.json");
        var markdownPath = Path.Combine(outputDirectory, "SpriggitValidationReport.md");
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(results, SerializerOptions));
        File.WriteAllText(markdownPath, CreateMarkdown(results, jsonPath, game, recordType));
        return markdownPath;
    }

    private static string CreateMarkdown(IReadOnlyList<SpriggitSampleValidationResult> results, string jsonPath, string? game, string? recordType)
    {
        var comparedRecords = results.Count;
        var comparedFields = results.Sum(result => result.Comparisons.Count);
        var categoryCounts = results
            .SelectMany(result => result.Comparisons)
            .GroupBy(comparison => comparison.Category)
            .OrderBy(group => group.Key.ToString(), StringComparer.Ordinal)
            .ToList();
        var failedRecords = results.Where(result => result.HasFailingFindings).ToList();
        var lines = new List<string>
        {
            "# Spriggit Data Validation Report",
            string.Empty,
            "- Run date UTC: " + DateTime.UtcNow.ToString("O"),
            "- Scope: " + CreateScopeLabel(game, recordType),
            "- Records compared: " + comparedRecords,
            "- Field comparisons: " + comparedFields,
            "- JSON details: " + jsonPath,
            string.Empty,
            "## Category Counts",
            string.Empty
        };

        foreach (var categoryCount in categoryCounts)
        {
            lines.Add("- " + categoryCount.Key + ": " + categoryCount.Count());
        }

        lines.Add(string.Empty);
        lines.Add("## Failed Records");
        lines.Add(string.Empty);
        if (failedRecords.Count == 0)
        {
            lines.Add("None.");
        }
        else
        {
            foreach (var result in failedRecords)
            {
                AddFailedRecord(lines, result);
            }
        }

        lines.Add(string.Empty);
        lines.Add("## Highest Priority Findings");
        lines.Add(string.Empty);
        var priorityFindings = results
            .SelectMany(result => result.Comparisons.Select(comparison => new { result.Sample, Comparison = comparison }))
            .Where(item => item.Comparison.Category is SpriggitValidationCategory.MissingInCreationsForge or SpriggitValidationCategory.ValueMismatch)
            .Take(100)
            .ToList();
        if (priorityFindings.Count == 0)
        {
            lines.Add("None.");
        }
        else
        {
            foreach (var finding in priorityFindings)
            {
                lines.Add("- " + finding.Sample.Game + " " + finding.Sample.RecordType + " " + finding.Sample.FormKey + " `" + finding.Comparison.FieldPath + "` " + finding.Comparison.Category);
            }
        }

        return string.Join(System.Environment.NewLine, lines) + System.Environment.NewLine;
    }

    private static void AddFailedRecord(IList<string> lines, SpriggitSampleValidationResult result)
    {
        var failingComparisons = GetFailingComparisons(result)
            .ToList();
        var failingCategoryCounts = failingComparisons
            .GroupBy(comparison => comparison.Category)
            .OrderBy(group => group.Key.ToString(), StringComparer.Ordinal)
            .Select(group => group.Key + ": " + group.Count())
            .ToList();

        lines.Add("### " + result.Sample.Game + " " + result.Sample.RecordType + " " + result.Sample.FormKey);
        lines.Add(string.Empty);
        lines.Add("- EditorID: " + (string.IsNullOrWhiteSpace(result.Sample.EditorId) ? "(none)" : result.Sample.EditorId));
        lines.Add("- Spriggit file: `" + result.Sample.SpriggitFile + "`");
        lines.Add("- Failing counts: " + string.Join(", ", failingCategoryCounts));
        lines.Add(string.Empty);

        if (result.Errors.Count > 0)
        {
            lines.Add("Errors:");
            foreach (var error in result.Errors)
            {
                lines.Add("- " + error);
            }

            lines.Add(string.Empty);
        }

        lines.Add("Top failing fields:");
        foreach (var comparison in failingComparisons.Take(FailedRecordFindingPreviewCount))
        {
            lines.Add("- `" + comparison.FieldPath + "` " + comparison.Category + " - " + CreateFindingSummary(comparison));
        }

        if (failingComparisons.Count > FailedRecordFindingPreviewCount)
        {
            lines.Add("- " + (failingComparisons.Count - FailedRecordFindingPreviewCount) + " more failing fields in the JSON report.");
        }

        lines.Add(string.Empty);
    }

    private static IEnumerable<SpriggitFieldComparison> GetFailingComparisons(SpriggitSampleValidationResult result)
    {
        return result.Comparisons
            .Where(comparison => comparison.Category is SpriggitValidationCategory.MissingInCreationsForge
                or SpriggitValidationCategory.ValueMismatch
                or SpriggitValidationCategory.TypeMismatch
                or SpriggitValidationCategory.CollectionCountMismatch
                or SpriggitValidationCategory.CollectionOrderMismatch
                or SpriggitValidationCategory.Error)
            .OrderBy(comparison => GetCategoryPriority(comparison.Category))
            .ThenBy(comparison => comparison.FieldPath, StringComparer.OrdinalIgnoreCase);
    }

    private static int GetCategoryPriority(SpriggitValidationCategory category)
    {
        return category switch
        {
            SpriggitValidationCategory.Error => 0,
            SpriggitValidationCategory.MissingInCreationsForge => 1,
            SpriggitValidationCategory.ValueMismatch => 2,
            SpriggitValidationCategory.TypeMismatch => 3,
            SpriggitValidationCategory.CollectionCountMismatch => 4,
            SpriggitValidationCategory.CollectionOrderMismatch => 5,
            _ => 9
        };
    }

    private static string CreateFindingSummary(SpriggitFieldComparison comparison)
    {
        if (!string.IsNullOrWhiteSpace(comparison.Notes))
        {
            return comparison.Notes;
        }

        return comparison.Category switch
        {
            SpriggitValidationCategory.MissingInCreationsForge => "Spriggit value `" + Truncate(comparison.SpriggitValue) + "` had no mapped DTO value.",
            SpriggitValidationCategory.ValueMismatch => "Spriggit `" + Truncate(comparison.SpriggitValue) + "` vs DTO `" + Truncate(comparison.CreationsForgeValue) + "`.",
            SpriggitValidationCategory.TypeMismatch => "Spriggit `" + Truncate(comparison.SpriggitValue) + "` vs DTO `" + Truncate(comparison.CreationsForgeValue) + "`.",
            SpriggitValidationCategory.CollectionCountMismatch => "Spriggit `" + Truncate(comparison.SpriggitValue) + "` vs DTO `" + Truncate(comparison.CreationsForgeValue) + "`.",
            SpriggitValidationCategory.CollectionOrderMismatch => "Spriggit `" + Truncate(comparison.SpriggitValue) + "` vs DTO `" + Truncate(comparison.CreationsForgeValue) + "`.",
            SpriggitValidationCategory.Error => "Validation error.",
            _ => "See JSON report for details."
        };
    }

    private static string Truncate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "(none)";
        }

        return value.Length <= 80
            ? value
            : value[..77] + "...";
    }

    private static string CreateScopeLabel(string? game, string? recordType)
    {
        return string.IsNullOrWhiteSpace(game) || string.IsNullOrWhiteSpace(recordType)
            ? "All configured samples"
            : game + " " + recordType;
    }

    private static string CreateOutputDirectory(string? game, string? recordType)
    {
        var repositoryRoot = FindRepositoryRoot();
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
        var outputDirectory = Path.Combine(repositoryRoot, "TestResults", "SpriggitValidation", timestamp);
        if (!string.IsNullOrWhiteSpace(game) && !string.IsNullOrWhiteSpace(recordType))
        {
            outputDirectory = Path.Combine(outputDirectory, SanitizePathSegment(game + "-" + recordType));
        }

        Directory.CreateDirectory(outputDirectory);
        return outputDirectory;
    }

    private static string SanitizePathSegment(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(value.Select(character => invalidChars.Contains(character) ? '-' : character).ToArray());
        return sanitized.Replace(' ', '-');
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CreationsForge.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Unable to locate repository root from test output directory.");
    }
}
