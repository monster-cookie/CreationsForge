using System.Reflection;
using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs;
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
        return typeof(ValidationSpec).Assembly
            .GetTypes()
            .Where(IsValidationSpecType)
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
            .Where(IsValidationSpecFactory)
            .OrderBy(method => method.DeclaringType?.FullName, StringComparer.Ordinal)
            .ThenBy(method => method.Name, StringComparer.Ordinal)
            .Select(method => (ValidationSpec)method.Invoke(null, [])!)
            .ToList();
    }

    /// <summary>
    /// Determines whether a type is a validation spec factory class.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns><c>true</c> when the type belongs to the validation specs namespace.</returns>
    private static bool IsValidationSpecType(Type type)
    {
        return type.IsClass &&
            type.IsAbstract &&
            type.IsSealed &&
            type.Namespace?.StartsWith("CreationsForge.DataValidationTests.Validation.Specs.", StringComparison.Ordinal) == true;
    }

    /// <summary>
    /// Determines whether a method creates one validation spec sample.
    /// </summary>
    /// <param name="method">The method to inspect.</param>
    /// <returns><c>true</c> when the method is a public no-argument validation spec factory.</returns>
    private static bool IsValidationSpecFactory(MethodInfo method)
    {
        return method.ReturnType == typeof(ValidationSpec) &&
            method.GetParameters().Length == 0;
    }
}
