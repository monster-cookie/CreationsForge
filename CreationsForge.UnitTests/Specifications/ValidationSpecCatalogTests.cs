using CreationsForge.Specification.Records;
using CreationsForge.Specification.Validation;
using Shouldly;

namespace CreationsForge.UnitTests.Specifications;

/// <summary>
/// Tests the production validation specification catalog.
/// </summary>
public class ValidationSpecCatalogTests
{
    /// <summary>
    /// Verifies that production validation specs are exposed from the Specification project.
    /// </summary>
    [Fact]
    public void All_ReturnsValidationSpecs()
    {
        ValidationSpecCatalog.All.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Verifies that validation specs use known record metadata from the production record catalog.
    /// </summary>
    [Fact]
    public void All_ReferencesKnownRecordSpecifications()
    {
        var knownRecordIDs = RecordSpecificationCatalog.All
            .Select(specification => specification.RecordID)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        ValidationSpecCatalog.All.ShouldAllBe(specification =>
            knownRecordIDs.Contains(specification.RecordType.RecordID));
    }

    /// <summary>
    /// Verifies that validation samples declare the fields required by the validation runners.
    /// </summary>
    [Fact]
    public void All_DeclaresSampleIdentity()
    {
        ValidationSpecCatalog.All.ShouldAllBe(specification => !string.IsNullOrWhiteSpace(specification.SampleName));
        ValidationSpecCatalog.All.ShouldAllBe(specification => !string.IsNullOrWhiteSpace(specification.FormKey));
        ValidationSpecCatalog.All.ShouldAllBe(specification => specification.RecordType != null);
    }

    /// <summary>
    /// Verifies that validation specs only target games supported by their record specification.
    /// </summary>
    [Fact]
    public void All_TargetsGamesSupportedByRecordSpecification()
    {
        ValidationSpecCatalog.All.ShouldAllBe(specification =>
            specification.RecordType.GameSupport.Any(support => support.Game == specification.Game));
    }
}
