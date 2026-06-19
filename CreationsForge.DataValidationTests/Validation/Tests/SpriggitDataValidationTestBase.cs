using CreationsForge.Core.Enums;
using CreationsForge.DataValidationTests.Validation.Configuration;
using CreationsForge.DataValidationTests.Validation.Environment;
using CreationsForge.DataValidationTests.Validation.Models;
using CreationsForge.DataValidationTests.Validation.Parsing;
using CreationsForge.DataValidationTests.Validation.Reports;
using CreationsForge.DataValidationTests.Validation.Services;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests;

[Trait("Category", "SpriggitDataValidation")]
public abstract class SpriggitDataValidationTestBase
{
    private const int InlineFailureLimit = 100;

    private static readonly ISet<SpriggitValidationCategory> FailingCategories = new HashSet<SpriggitValidationCategory>
    {
        SpriggitValidationCategory.MissingInCreationsForge,
        SpriggitValidationCategory.ValueMismatch,
        SpriggitValidationCategory.TypeMismatch,
        SpriggitValidationCategory.CollectionCountMismatch,
        SpriggitValidationCategory.CollectionOrderMismatch,
        SpriggitValidationCategory.Error
    };

    private static readonly SpriggitEnvironmentLoader EnvironmentLoader = new();
    private static readonly GameRecordSetProvider RecordSetProvider = new();
    private static readonly Lazy<SpriggitValidationManifest> Manifest = new(() => new SpriggitValidationConfigurationLoader().LoadManifest());
    private static readonly Lazy<SpriggitApprovedDifferenceSet> ApprovedDifferences = new(() => new SpriggitValidationConfigurationLoader().LoadApprovedDifferences());
    private static readonly Lazy<SpriggitEnvironmentConfiguration> SpriggitEnvironment = new(() => EnvironmentLoader.Load());

    protected static void ValidateScope(SupportedGame game, string recordType)
    {
        var samples = Manifest.Value.Samples
            .Where(sample =>
                string.Equals(sample.Game, game.ToString(), StringComparison.OrdinalIgnoreCase) &&
                string.Equals(sample.RecordType, recordType, StringComparison.OrdinalIgnoreCase))
            .ToList();
        var comparer = new SpriggitRecordComparer(new SpriggitApprovedDifferenceMatcher(ApprovedDifferences.Value.ApprovedDifferences.ToList()));
        var results = new List<SpriggitSampleValidationResult>();

        foreach (var sample in samples)
        {
            results.Add(ValidateSample(sample, EnvironmentLoader, SpriggitEnvironment.Value, RecordSetProvider, comparer));
        }

        var reportPath = new SpriggitValidationReportWriter().Write(results, game.ToString(), recordType);
        results.Count.ShouldBeGreaterThan(0, "Spriggit validation manifest should contain selected samples for " + game + " " + recordType + ".");
        AssertResults(results, reportPath);
    }

    private static void AssertResults(IReadOnlyList<SpriggitSampleValidationResult> results, string reportPath)
    {
        var assertions = new List<Action>();
        var failureCount = 0;

        foreach (var result in results)
        {
            foreach (var error in result.Errors)
            {
                failureCount++;
                if (assertions.Count >= InlineFailureLimit)
                {
                    continue;
                }

                var currentResult = result;
                var currentError = error;
                assertions.Add(() => ((string?)currentError).ShouldBeNull(BuildErrorFailureMessage(currentResult, currentError, reportPath)));
            }

            foreach (var comparison in result.Comparisons)
            {
                if (!FailingCategories.Contains(comparison.Category))
                {
                    continue;
                }

                failureCount++;
                if (assertions.Count >= InlineFailureLimit)
                {
                    continue;
                }

                var currentResult = result;
                var currentComparison = comparison;
                assertions.Add(() => currentComparison.Category.ShouldNotBe(currentComparison.Category, BuildComparisonFailureMessage(currentResult, currentComparison, reportPath)));
            }
        }

        results.SelectMany(result => result.Comparisons)
            .ShouldNotBeEmpty("Spriggit data validation should produce field comparisons.");

        if (failureCount > InlineFailureLimit)
        {
            var remainingFailureCount = failureCount - InlineFailureLimit;
            assertions.Add(() => remainingFailureCount.ShouldBe(0, remainingFailureCount + " additional Spriggit validation field failures were found. Full report: " + reportPath));
        }

        if (assertions.Count == 0)
        {
            return;
        }

        new object().ShouldSatisfyAllConditions(assertions.ToArray());
    }

    private static string BuildErrorFailureMessage(SpriggitSampleValidationResult result, string error, string reportPath)
    {
        return BuildRecordLabel(result) +
               System.Environment.NewLine +
               "Error: " + error +
               System.Environment.NewLine +
               "Report: " + reportPath;
    }

    private static string BuildComparisonFailureMessage(SpriggitSampleValidationResult result, SpriggitFieldComparison comparison, string reportPath)
    {
        return BuildRecordLabel(result) +
               System.Environment.NewLine +
               "Field: " + comparison.FieldPath +
               System.Environment.NewLine +
               "Category: " + comparison.Category +
               System.Environment.NewLine +
               "Spriggit value: " + FormatValue(comparison.SpriggitValue) +
               System.Environment.NewLine +
               "DTO value: " + FormatValue(comparison.CreationsForgeValue) +
               System.Environment.NewLine +
               "Normalized Spriggit value: " + FormatValue(comparison.NormalizedSpriggitValue) +
               System.Environment.NewLine +
               "Normalized DTO value: " + FormatValue(comparison.NormalizedCreationsForgeValue) +
               System.Environment.NewLine +
               "Notes: " + FormatValue(comparison.Notes) +
               System.Environment.NewLine +
               "Report: " + reportPath;
    }

    private static string BuildRecordLabel(SpriggitSampleValidationResult result)
    {
        return "Record: " +
               result.Sample.Game +
               " " +
               result.Sample.RecordType +
               " " +
               result.Sample.FormKey +
               " " +
               FormatValue(result.Sample.EditorId) +
               System.Environment.NewLine +
               "Spriggit file: " +
               result.Sample.SpriggitFile;
    }

    private static string FormatValue(string? value)
    {
        if (value is null)
        {
            return "<null>";
        }

        return value.Length <= 500
            ? value
            : value[..500] + "...";
    }

    private static SpriggitSampleValidationResult ValidateSample(
        SpriggitValidationSample sample,
        SpriggitEnvironmentLoader environmentLoader,
        SpriggitEnvironmentConfiguration spriggitEnvironment,
        GameRecordSetProvider recordSetProvider,
        SpriggitRecordComparer comparer)
    {
        if (!Enum.TryParse<SupportedGame>(sample.Game, ignoreCase: true, out var game))
        {
            return CreateErrorResult(sample, "Unsupported game in validation manifest: " + sample.Game);
        }

        try
        {
            var root = environmentLoader.GetExtractionRoot(game, spriggitEnvironment);
            var spriggitPath = Path.Combine(root, sample.SpriggitFile.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(spriggitPath))
            {
                return CreateErrorResult(sample, "Spriggit sample file does not exist: " + spriggitPath);
            }

            var spriggit = SpriggitYamlDocument.Load(spriggitPath);
            var record = recordSetProvider.GetRecord(game, sample.RecordType, sample.FormKey);
            return comparer.Compare(game, sample, spriggit, record);
        }
        catch (Exception exception)
        {
            return CreateErrorResult(sample, exception.Message);
        }
    }

    private static SpriggitSampleValidationResult CreateErrorResult(SpriggitValidationSample sample, string error)
    {
        return new SpriggitSampleValidationResult
        {
            Sample = sample,
            Errors = { error },
            Comparisons =
            {
                new SpriggitFieldComparison
                {
                    FieldPath = sample.FormKey,
                    Category = SpriggitValidationCategory.Error,
                    Notes = error
                }
            }
        };
    }
}
