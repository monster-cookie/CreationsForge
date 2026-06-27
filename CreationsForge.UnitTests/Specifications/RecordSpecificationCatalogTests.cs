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
    /// Verifies that game support filtering returns the current record families for the Starfield adapter.
    /// </summary>
    [Fact]
    public void GetSupportedByGame_ReturnsStarfieldRecordsSupportedByRequestedGame()
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
    /// Verifies that Starfield-supported specifications expose the dispatch sequence consumed by the Starfield reader.
    /// </summary>
    [Fact]
    public void GetSupportedByGame_ReturnsStarfieldRecordsInReaderDispatchOrder()
    {
        var recordIDs = RecordSpecificationCatalog.GetSupportedByGame(SpecificationGame.Starfield)
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
    /// Verifies that game support filtering returns the current record families for the Fallout 4 adapter.
    /// </summary>
    [Fact]
    public void GetSupportedByGame_ReturnsFallout4RecordsSupportedByRequestedGame()
    {
        var specifications = RecordSpecificationCatalog.GetSupportedByGame(SpecificationGame.Fallout4);

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
            "BOOK",
            "DOOR",
            "TERM"
        ], ignoreOrder: true);
    }

    /// <summary>
    /// Verifies that Fallout 4-supported specifications expose the dispatch sequence consumed by the Fallout 4
    /// reader.
    /// </summary>
    [Fact]
    public void GetSupportedByGame_ReturnsFallout4RecordsInReaderDispatchOrder()
    {
        var recordIDs = RecordSpecificationCatalog.GetSupportedByGame(SpecificationGame.Fallout4)
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
            "BOOK",
            "DOOR",
            "TERM"
        ]);
    }

    /// <summary>
    /// Verifies that game support filtering returns the current record families for the Skyrim adapter.
    /// </summary>
    [Fact]
    public void GetSupportedByGame_ReturnsSkyrimRecordsSupportedByRequestedGame()
    {
        var specifications = RecordSpecificationCatalog.GetSupportedByGame(SpecificationGame.Skyrim);

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
            "BOOK",
            "DOOR"
        ], ignoreOrder: true);
    }

    /// <summary>
    /// Verifies that Skyrim-supported specifications expose the dispatch sequence consumed by the Skyrim reader.
    /// </summary>
    [Fact]
    public void GetSupportedByGame_ReturnsSkyrimRecordsInReaderDispatchOrder()
    {
        var recordIDs = RecordSpecificationCatalog.GetSupportedByGame(SpecificationGame.Skyrim)
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
            "BOOK",
            "DOOR"
        ]);
    }

    /// <summary>
    /// Verifies that active scalar comparison specifications define at least one comparison row.
    /// </summary>
    [Fact]
    public void All_ActiveScalarComparisonSpecificationsExposeComparisonFields()
    {
        var comparisonBackedRecordIDs = RecordSpecificationCatalog.All
            .Where(specification => specification.Comparison.Fields.Count > 0)
            .Select(specification => specification.RecordID)
            .ToList();

        comparisonBackedRecordIDs.ShouldBe(
            ["FLST", "GMST", "GLOB", "CLAS", "FACT", "MISC", "KYWD", "AVIF", "NPC_", "MGEF", "PERK", "STAT", "CONT", "COBJ", "CNDF", "BOOK", "DOOR", "TERM"],
            ignoreOrder: true);
    }

    /// <summary>
    /// Verifies that child-group comparison metadata is declared by the current shared child-row record families.
    /// </summary>
    [Fact]
    public void All_ComparisonChildGroupsExposeCurrentSharedStrategies()
    {
        var childGroups = RecordSpecificationCatalog.All
            .SelectMany(specification => specification.Comparison.ChildGroups.Select(group => new
            {
                specification.RecordID,
                Group = group
            }))
            .OrderBy(entry => entry.RecordID, StringComparer.Ordinal)
            .ToList();

        var keywordRecordIDs = childGroups
            .Where(entry => entry.Group.GroupKind == RecordComparisonChildGroupKind.KeywordMappings)
            .Select(entry => entry.RecordID)
            .OrderBy(recordID => recordID, StringComparer.Ordinal)
            .ToList();
        var soundRecordIDs = childGroups
            .Where(entry => entry.Group.GroupKind == RecordComparisonChildGroupKind.SoundMappings)
            .Select(entry => entry.RecordID)
            .OrderBy(recordID => recordID, StringComparer.Ordinal)
            .ToList();
        var modelRecordIDs = childGroups
            .Where(entry => entry.Group.GroupKind == RecordComparisonChildGroupKind.ModelMappings)
            .Select(entry => entry.RecordID)
            .OrderBy(recordID => recordID, StringComparer.Ordinal)
            .ToList();

        keywordRecordIDs.ShouldBe(
            ["BOOK", "CONT", "DOOR", "FACT", "MGEF", "MISC", "NPC_", "STAT", "TERM"]);
        soundRecordIDs.ShouldBe(["BOOK", "COBJ", "CONT", "DOOR", "MGEF", "MISC", "NPC_", "PERK"]);
        modelRecordIDs.ShouldBe(["BOOK", "CONT", "DOOR", "MISC", "STAT", "TERM"]);
        childGroups
            .Where(entry => entry.Group.GroupKind == RecordComparisonChildGroupKind.KeywordMappings)
            .ShouldAllBe(entry => entry.Group.GroupName == "Keywords");
        childGroups
            .Where(entry => entry.Group.GroupKind == RecordComparisonChildGroupKind.SoundMappings)
            .ShouldAllBe(entry => entry.Group.GroupName == "Sounds");
        childGroups
            .Where(entry => entry.Group.GroupKind == RecordComparisonChildGroupKind.ModelMappings)
            .ShouldAllBe(entry => entry.Group.GroupName == "Models");
        childGroups.ShouldAllBe(entry => !string.IsNullOrWhiteSpace(entry.Group.Description));
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

    /// <summary>
    /// Verifies that reader behavior metadata defaults to the overlay-safe path and does not silently allow missing
    /// collections.
    /// </summary>
    [Fact]
    public void All_ReaderSpecificationsExposeCurrentBehaviorDefaults()
    {
        RecordSpecificationCatalog.All.ShouldAllBe(specification => specification.Reader.UsesOverlaySafeMod);
        RecordSpecificationCatalog.All.ShouldAllBe(specification => !specification.Reader.IsOptionalCollection);
    }

    /// <summary>
    /// Verifies that full-binary reader metadata is limited to the current Fallout 4 terminal workaround.
    /// </summary>
    [Fact]
    public void All_ReaderSpecificationsExposeOnlyCurrentFullBinaryRequirements()
    {
        var fullBinarySpecifications = RecordSpecificationCatalog.All
            .Where(specification => specification.Reader.RequiresFullBinaryMod)
            .ToList();

        fullBinarySpecifications.Count.ShouldBe(1);
        fullBinarySpecifications[0].RecordID.ShouldBe("TERM");
        fullBinarySpecifications[0].Reader.RequiresFullBinaryModForGame(SpecificationGame.Fallout4).ShouldBeTrue();
        fullBinarySpecifications[0].Reader.RequiresFullBinaryModForGame(SpecificationGame.Starfield).ShouldBeFalse();
        fullBinarySpecifications[0].Reader.RequiresFullBinaryModForGame(SpecificationGame.Skyrim).ShouldBeFalse();
    }

    /// <summary>
    /// Verifies that game-specific full-binary reader overrides only target games that support the record family.
    /// </summary>
    [Fact]
    public void All_ReaderFullBinaryRequirementsReferenceSupportedGames()
    {
        foreach (var specification in RecordSpecificationCatalog.All)
        {
            foreach (var game in specification.Reader.GamesRequiringFullBinaryMod)
            {
                specification.GameSupport.Any(support => support.Game == game).ShouldBeTrue();
            }
        }
    }
}
