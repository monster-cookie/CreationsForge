using CreationsForge.Specification.Records;
using Shouldly;

namespace CreationsForge.UnitTests.Specifications;

/// <summary>
/// Tests the static production record specification catalog used by the first specification foundation slice.
/// </summary>
public class RecordSpecificationCatalogTests
{
    /// <summary>
    /// Verifies that the foundation catalog includes the pilot record families chosen for the first slice.
    /// </summary>
    [Fact]
    public void All_ReturnsPilotRecordSpecifications()
    {
        var recordIDs = RecordSpecificationCatalog.All.Select(specification => specification.RecordID).ToList();

        recordIDs.ShouldBe(["FLST", "GMST", "GLOB"], ignoreOrder: true);
    }

    /// <summary>
    /// Verifies that record identifiers are unique so registry lookups cannot silently choose between duplicates.
    /// </summary>
    [Fact]
    public void All_DoesNotExposeDuplicateRecordIDs()
    {
        var duplicateRecordIDs = RecordSpecificationCatalog.All
            .GroupBy(specification => specification.RecordID, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        duplicateRecordIDs.ShouldBeEmpty();
    }

    /// <summary>
    /// Verifies that catalog lookup is case-insensitive because record IDs are stable Bethesda identifiers rather
    /// than user-authored text.
    /// </summary>
    [Fact]
    public void FindByRecordID_MatchesCaseInsensitiveRecordID()
    {
        var specification = RecordSpecificationCatalog.FindByRecordID("glob");

        specification.ShouldNotBeNull();
        specification.RecordID.ShouldBe("GLOB");
    }

    /// <summary>
    /// Verifies that unknown or empty record identifiers return no specification instead of throwing.
    /// </summary>
    [Fact]
    public void FindByRecordID_ReturnsNullForUnknownRecordID()
    {
        RecordSpecificationCatalog.FindByRecordID("NOPE").ShouldBeNull();
        RecordSpecificationCatalog.FindByRecordID(string.Empty).ShouldBeNull();
    }

    /// <summary>
    /// Verifies that game support filtering returns the current pilot records for the selected game adapter.
    /// </summary>
    [Fact]
    public void GetSupportedByGame_ReturnsRecordsSupportedByRequestedGame()
    {
        var specifications = RecordSpecificationCatalog.GetSupportedByGame(SpecificationGame.Starfield);

        specifications.Select(specification => specification.RecordID).ShouldBe(["FLST", "GMST", "GLOB"], ignoreOrder: true);
    }
}
