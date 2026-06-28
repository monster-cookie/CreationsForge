using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Specification.Records;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>
/// Contains record comparison scenarios for Faction records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that Faction comparison uses specification-owned scalar rows and child-group dispatch.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForFaction_MapsScalarFieldsAndChildGroups()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x160);
        var keywordFormKey = CreateFormKey("Starfield.esm", 0x301);
        var baseFaction = CreateFaction("Base.esm", formKey, "Constellation", "BaseFlags", 100, keywordFormKey);
        AddFactionChildren(baseFaction, "Base.esm", formKey, "Ally", "Base Rank", 4, 10);
        var patchFaction = CreateFaction("Patch.esp", formKey, "Constellation", "PatchFlags", 200, keywordFormKey);
        AddFactionChildren(patchFaction, "Patch.esp", formKey, "Friend", "Patch Rank", 8, 20);
        var factionRepository = new TestFactionRepository
        {
            Records =
            [
                baseFaction,
                patchFaction
            ]
        };
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.Faction.RecordID, formKey, keywordFormKey, 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.Faction.RecordID, formKey, keywordFormKey, 0)
            ]
        };
        var service = CreateService(factionRepository: factionRepository, keywordMappingRepository: keywordMappingRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Faction.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Constellation", "Constellation"]);
        comparison.Fields.Single(field => field.FieldName == "Flags").Values.Select(value => value.DisplayValue).ShouldBe(["BaseFlags", "PatchFlags"]);
        comparison.Fields.Single(field => field.FieldName == "FormationRadius").Values.Select(value => value.DisplayValue).ShouldBe(["100", "200"]);
        comparison.Fields.Single(field => field.FieldName == "Keyword").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000301", "Starfield.esm:00000301"]);
        comparison.Fields.Single(field => field.FieldName == "CrimeValues.Murder").Values.Select(value => value.DisplayValue).ShouldBe(["10", "20"]);
        comparison.Fields.Single(field => field.FieldName == "VendorValues.StartHour").Values.Select(value => value.DisplayValue).ShouldBe(["8", "10"]);
        comparison.Fields.Single(field => field.FieldName == "VendorLocation.Target.Link").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000301", "Starfield.esm:00000301"]);
        var relations = comparison.Fields.Single(field => field.FieldName == "Relations");
        relations.Children.Single(field => field.FieldName == "Relation [0]").Children.Single(field => field.FieldName == "Reaction").Values.Select(value => value.DisplayValue).ShouldBe(["Ally", "Friend"]);
        var ranks = comparison.Fields.Single(field => field.FieldName == "Ranks");
        ranks.Children.Single(field => field.FieldName == "Rank [0]").Children.Single(field => field.FieldName == "Number").Values.Select(value => value.DisplayValue).ShouldBe(["4", "8"]);
        comparison.Fields.Single(field => field.FieldName == "Conditions").Children.ShouldNotBeEmpty();
        comparison.Fields.Single(field => field.FieldName == "Components").Children.ShouldNotBeEmpty();
        comparison.Fields.Single(field => field.FieldName == "Keywords").Children.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Verifies that Faction scalar rows are selected from the injected comparison specification while undeclared
    /// child groups are omitted.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForFaction_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x161);
        var keywordFormKey = CreateFormKey("Starfield.esm", 0x301);
        var baseFaction = CreateFaction("Base.esm", formKey, "Constellation", "BaseFlags", 100, keywordFormKey);
        AddFactionChildren(baseFaction, "Base.esm", formKey, "Ally", "Base Rank", 4, 10);
        var patchFaction = CreateFaction("Patch.esp", formKey, "Constellation", "PatchFlags", 200, keywordFormKey);
        AddFactionChildren(patchFaction, "Patch.esp", formKey, "Friend", "Patch Rank", 8, 20);
        var factionRepository = new TestFactionRepository
        {
            Records =
            [
                baseFaction,
                patchFaction
            ]
        };
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.Faction.RecordID, formKey, keywordFormKey, 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.Faction.RecordID, formKey, keywordFormKey, 0)
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Faction.RecordID,
                RecordType = SupportedRecordSpecifications.Faction.RecordType,
                TableName = SupportedRecordSpecifications.Faction.TableName,
                FriendlyName = SupportedRecordSpecifications.Faction.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Faction.GameSupport,
                Fields = SupportedRecordSpecifications.Faction.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "FormationRadius",
                            SourcePath = "FormationRadius",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            factionRepository: factionRepository,
            keywordMappingRepository: keywordMappingRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Faction.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "FormationRadius").Values.Select(value => value.DisplayValue)
            .ShouldBe(["100", "200"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Flags");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Relations");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Ranks");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Components");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Keywords");
    }

    /// <summary>
    /// Verifies that specification-declared localized Faction rows use the selected record text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForFaction_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x162);
        var factionRepository = new TestFactionRepository
        {
            Records =
            [
                CreateFaction("Base.esm", formKey, "Constellation", "BaseFlags", 100, CreateFormKey("Starfield.esm", 0x301)),
                CreateFaction("Patch.esp", formKey, "Constellation", "PatchFlags", 200, CreateFormKey("Starfield.esm", 0x301))
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Fraktion"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Fraktion")
            ]
        };
        var applicationSettingsService = new TestApplicationSettingsService { RecordTextLanguage = Language.German };
        var service = CreateService(
            factionRepository: factionRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            applicationSettingsService: applicationSettingsService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Faction.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Fraktion", "Patch Fraktion"]);
    }

}
