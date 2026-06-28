using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests;

/// <summary>
/// Audits whether Spriggit DTO validation samples have meaningful comparison UI row coverage.
/// </summary>
public class SpriggitComparisonUiCoverageAuditTests : IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the coverage audit tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and render comparison rows.</param>
    public SpriggitComparisonUiCoverageAuditTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Verifies validation specs do not rely on the default EditorID-only comparison UI fallback.
    /// </summary>
    [AvaloniaFact]
    [Trait("Category", "SpriggitUiCoverageAudit")]
    public void SpriggitComparisonUiCoverageAudit_ShouldNotHaveBlockingCoverageGaps()
    {
        var specs = GetValidationSpecs();
        var diagnostics = SpriggitComparisonUiCoverageAudit.GetDiagnostics(specs, fixture);
        var blockingDiagnostics = diagnostics
            .Where(diagnostic => diagnostic.IsBlocking)
            .Select(diagnostic => diagnostic.Format())
            .ToList();

        blockingDiagnostics.ShouldBeEmpty(
            "Spriggit comparison UI coverage should not have high-confidence gaps." +
            System.Environment.NewLine +
            string.Join(System.Environment.NewLine, blockingDiagnostics));
    }

    /// <summary>
    /// Gets all public no-argument validation spec factory methods in deterministic order.
    /// </summary>
    /// <returns>The validation specs declared by the spec classes.</returns>
    private static IReadOnlyList<ValidationSpec> GetValidationSpecs()
    {
        return ValidationSpecCatalog.All;
    }
}
