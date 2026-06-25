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
        var insidePrivateMethod = false;
        var privateMethodOpened = false;
        var privateMethodBraceDepth = 0;

        foreach (var line in File.ReadLines(path))
        {
            lineNumber++;
            if (ContainsForbiddenPattern(line))
            {
                yield return Path.GetRelativePath(FindRepositoryRoot(), path) + ":" + lineNumber + ": " + line.Trim();
            }

            if (!insidePrivateMethod && IsPrivateMethodDeclaration(line))
            {
                insidePrivateMethod = true;
                privateMethodOpened = false;
                privateMethodBraceDepth = 0;
            }

            if (insidePrivateMethod && ContainsPrivateAssertionPattern(line))
            {
                yield return Path.GetRelativePath(FindRepositoryRoot(), path) + ":" + lineNumber + ": " + line.Trim();
            }

            if (!insidePrivateMethod)
            {
                continue;
            }

            privateMethodBraceDepth += CountBraceDelta(line);
            privateMethodOpened = privateMethodOpened || line.Contains('{', StringComparison.Ordinal);
            if (privateMethodOpened && privateMethodBraceDepth <= 0)
            {
                insidePrivateMethod = false;
            }
        }
    }

    private static bool IsPrivateMethodDeclaration(string line)
    {
        var trimmedLine = line.TrimStart();
        return trimmedLine.StartsWith("private ", StringComparison.Ordinal) &&
               trimmedLine.Contains('(', StringComparison.Ordinal) &&
               !trimmedLine.EndsWith(';');
    }

    private static bool ContainsPrivateAssertionPattern(string line)
    {
        return line.Contains(".Should", StringComparison.Ordinal) ||
               line.Contains("Assert.", StringComparison.Ordinal);
    }

    private static int CountBraceDelta(string line)
    {
        return line.Count(character => character == '{') - line.Count(character => character == '}');
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
