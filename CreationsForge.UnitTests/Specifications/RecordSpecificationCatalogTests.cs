using CreationsForge.Specification.Records;
using CreationsForge.Core.DTOs.Records;
using Shouldly;

namespace CreationsForge.UnitTests.Specifications;

/// <summary>
/// Tests the static production record specification catalog used by specification-aware workflows.
/// </summary>
public class RecordSpecificationCatalogTests
{
    /// <summary>
    /// Verifies that the catalog includes the current imported record families in import-dispatch order.
    /// </summary>
    [Fact]
    public void All_ReturnsCurrentImportRecordSpecifications()
    {
        var recordIDs = RecordSpecificationCatalog.All
            .OrderBy(specification => specification.Import.ImportOrder)
            .Select(specification => specification.RecordID)
            .ToList();

        recordIDs.ShouldBe(
        [
            "FLST",
            "GMST",
            "GLOB",
            "CLAS",
            "FACT",
            "MISC",
            "KYWD",
            "AVIF",
            "NPC_",
            "MGEF",
            "PERK",
            "STAT",
            "CONT",
            "COBJ",
            "CNDF",
            "BOOK",
            "DOOR",
            "TERM"
        ]);
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
    /// Verifies that import order values are unique and contiguous so dispatch order stays deterministic.
    /// </summary>
    [Fact]
    public void All_DoesNotExposeDuplicateImportOrders()
    {
        var importOrders = RecordSpecificationCatalog.All
            .Select(specification => specification.Import.ImportOrder)
            .Order()
            .ToList();

        importOrders.ShouldBe(Enumerable.Range(0, RecordSpecificationCatalog.All.Count).ToList());
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
    /// Verifies that game support filtering returns the current record families for the selected game adapter.
    /// </summary>
    [Fact]
    public void GetSupportedByGame_ReturnsRecordsSupportedByRequestedGame()
    {
        var specifications = RecordSpecificationCatalog.GetSupportedByGame(SpecificationGame.Starfield);

        specifications.Select(specification => specification.RecordID).ShouldBe(
        [
            "FLST",
            "GMST",
            "GLOB",
            "CLAS",
            "FACT",
            "MISC",
            "KYWD",
            "AVIF",
            "NPC_",
            "MGEF",
            "PERK",
            "STAT",
            "CONT",
            "COBJ",
            "CNDF",
            "BOOK",
            "DOOR",
            "TERM"
        ], ignoreOrder: true);
    }

    /// <summary>
    /// Verifies that active pilot comparison specifications define at least one comparison row.
    /// </summary>
    [Fact]
    public void All_ActivePilotSpecificationsExposeComparisonFields()
    {
        var comparisonBackedRecordIDs = RecordSpecificationCatalog.All
            .Where(specification => specification.Comparison.Fields.Count > 0)
            .Select(specification => specification.RecordID)
            .ToList();

        comparisonBackedRecordIDs.ShouldBe(["FLST", "GMST", "GLOB"], ignoreOrder: true);
    }

    /// <summary>
    /// Verifies that import specifications point at real plugin record-set collections.
    /// </summary>
    [Fact]
    public void All_ActivePilotSpecificationsReferencePluginRecordSetProperties()
    {
        var recordSetProperties = typeof(PluginRecordSetDTO)
            .GetProperties()
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        RecordSpecificationCatalog.All.ShouldAllBe(specification =>
            recordSetProperties.Contains(specification.Import.PluginRecordSetPropertyName));
    }

    /// <summary>
    /// Verifies that reader specifications point at real plugin record-set collections.
    /// </summary>
    [Fact]
    public void All_ReaderSpecificationsReferencePluginRecordSetProperties()
    {
        var recordSetProperties = typeof(PluginRecordSetDTO)
            .GetProperties()
            .Select(property => property.Name)
            .ToHashSet(StringComparer.Ordinal);

        RecordSpecificationCatalog.All.ShouldAllBe(specification =>
            recordSetProperties.Contains(specification.Reader.PluginRecordSetPropertyName));
    }

    /// <summary>
    /// Verifies that reader and import metadata target the same DTO collection during the metadata foundation phase.
    /// </summary>
    [Fact]
    public void All_ReaderSpecificationsMatchImportRecordSetProperties()
    {
        RecordSpecificationCatalog.All.ShouldAllBe(specification =>
            specification.Reader.PluginRecordSetPropertyName == specification.Import.PluginRecordSetPropertyName);
    }

    /// <summary>
    /// Verifies that reader-facing Mutagen collection names are populated for the catalog and supported game entries.
    /// </summary>
    [Fact]
    public void All_ReaderSpecificationsExposeMutagenCollectionNames()
    {
        RecordSpecificationCatalog.All.ShouldAllBe(specification =>
            !string.IsNullOrWhiteSpace(specification.Reader.DefaultMutagenCollectionName));

        RecordSpecificationCatalog.All.ShouldAllBe(specification =>
            specification.GameSupport.All(support => !string.IsNullOrWhiteSpace(support.MutagenCollectionName)));
    }
}
