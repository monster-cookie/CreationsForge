using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests;

public class DataValidationAssertionStyleTests
{
    [Fact]
    [Trait("Category", "DataValidationStyle")]
    public void SpriggitComparisons_ShouldRemainExplicit()
    {
        var testDirectory = Path.Combine(FindRepositoryRoot(), "CreationsForge.DataValidationTests", "Validation", "Tests");
        var violations = Directory.EnumerateFiles(testDirectory, "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.EndsWith("Helpers.cs", StringComparison.OrdinalIgnoreCase))
            .Where(path => !path.EndsWith(nameof(DataValidationAssertionStyleTests) + ".cs", StringComparison.OrdinalIgnoreCase))
            .SelectMany(FindViolations)
            .ToList();

        violations.ShouldBeEmpty();
    }

    private static IEnumerable<string> FindViolations(string path)
    {
        var lineNumber = 0;
        foreach (var line in File.ReadLines(path))
        {
            lineNumber++;
            if (ContainsForbiddenPattern(line))
            {
                yield return Path.GetRelativePath(FindRepositoryRoot(), path) + ":" + lineNumber + ": " + line.Trim();
            }
        }
    }

    private static bool ContainsForbiddenPattern(string line)
    {
        return line.Contains("foreach (var field in spriggit.Fields", StringComparison.Ordinal) ||
               line.Contains("foreach (var field in spriggitFields", StringComparison.Ordinal) ||
               line.Contains("TryGetValue(field.Key", StringComparison.Ordinal) ||
               line.Contains("GlobalSpriggitDTO", StringComparison.Ordinal) ||
               line.Contains("Helpers.GetDTOFields(", StringComparison.Ordinal) ||
               line.Contains("Helpers.GetSpriggitListValues(spriggit", StringComparison.Ordinal);
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

        throw new DirectoryNotFoundException("Unable to find repository root from '" + AppContext.BaseDirectory + "'.");
    }
}
