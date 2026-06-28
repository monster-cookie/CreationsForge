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
/// Contains record comparison scenarios for Actor Value Information records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that record comparison actor value information maps scalar fields and perk tree.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForActorValueInformation_MapsScalarFieldsAndPerkTree()
    {
        var formKey = CreateFormKey("Skyrim.esm", 0x150);
        var associatedSkill = CreateFormKey("Skyrim.esm", 0x201);
        var perk = CreateFormKey("Skyrim.esm", 0x202);
        var baseActorValue = CreateActorValueInformation("Base.esm", formKey, "Archery", "ARC", "Base description", "BaseCNAM", 1.1, 2.2, 3.3, "Base notes", 10, "BaseFlags", "Skill", 0, 100);
        baseActorValue.PerkTree.Add(CreateActorValueInformationPerkTreeEntry("Base.esm", formKey, associatedSkill, perk, 0, "BaseFNAM", 4, 5));
        var patchActorValue = CreateActorValueInformation("Patch.esp", formKey, "Archery", "ARC", "Patch description", "PatchCNAM", 1.5, 2.5, 3.5, "Patch notes", 20, "PatchFlags", "Skill", -10, 110);
        patchActorValue.PerkTree.Add(CreateActorValueInformationPerkTreeEntry("Patch.esp", formKey, associatedSkill, perk, 0, "PatchFNAM", 6, 7));
        var actorValueInformationRepository = new TestActorValueInformationRepository
        {
            Records =
            [
                baseActorValue,
                patchActorValue
            ]
        };
        var service = CreateService(actorValueInformationRepository: actorValueInformationRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ActorValueInformation.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Archery", "Archery"]);
        comparison.Fields.Single(field => field.FieldName == "Abbreviation").Values.Select(value => value.DisplayValue).ShouldBe(["ARC", "ARC"]);
        comparison.Fields.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue).ShouldBe(["Base description", "Patch description"]);
        comparison.Fields.Single(field => field.FieldName == "CNAM").Values.Select(value => value.DisplayValue).ShouldBe(["BaseCNAM", "PatchCNAM"]);
        comparison.Fields.Single(field => field.FieldName == "Skill.ImproveMult").Values.Select(value => value.DisplayValue).ShouldBe(["1.1", "1.5"]);
        comparison.Fields.Single(field => field.FieldName == "Skill.ImproveOffset").Values.Select(value => value.DisplayValue).ShouldBe(["2.2", "2.5"]);
        comparison.Fields.Single(field => field.FieldName == "Skill.UseMult").Values.Select(value => value.DisplayValue).ShouldBe(["3.3", "3.5"]);
        comparison.Fields.Single(field => field.FieldName == "ContextNotes").Values.Select(value => value.DisplayValue).ShouldBe(["Base notes", "Patch notes"]);
        comparison.Fields.Single(field => field.FieldName == "DefaultValue").Values.Select(value => value.DisplayValue).ShouldBe(["10", "20"]);
        comparison.Fields.Single(field => field.FieldName == "Flags").Values.Select(value => value.DisplayValue).ShouldBe(["BaseFlags", "PatchFlags"]);
        comparison.Fields.Single(field => field.FieldName == "Type").Values.Select(value => value.DisplayValue).ShouldBe(["Skill", "Skill"]);
        comparison.Fields.Single(field => field.FieldName == "Min").Values.Select(value => value.DisplayValue).ShouldBe(["0", "-10"]);
        comparison.Fields.Single(field => field.FieldName == "Max").Values.Select(value => value.DisplayValue).ShouldBe(["100", "110"]);
        var perkTree = comparison.Fields.Single(field => field.FieldName == "PerkTree");
        var perkTreeEntry = perkTree.Children.Single(field => field.FieldName == "PerkTree [0]");
        perkTreeEntry.Children.Single(field => field.FieldName == "AssociatedSkill").Values.Select(value => value.DisplayValue).ShouldBe(["Skyrim.esm:00000201", "Skyrim.esm:00000201"]);
        perkTreeEntry.Children.Single(field => field.FieldName == "FNAM").Values.Select(value => value.DisplayValue).ShouldBe(["BaseFNAM", "PatchFNAM"]);
        perkTreeEntry.Children.Single(field => field.FieldName == "ConnectionLineToIndices").Values.Select(value => value.DisplayValue).ShouldBe(["4", "6"]);
    }

    /// <summary>
    /// Verifies that Actor Value Information scalar rows are selected from the injected comparison specification while
    /// perk-tree rows remain strategy-based.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForActorValueInformation_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Skyrim.esm", 0x151);
        var associatedSkill = CreateFormKey("Skyrim.esm", 0x201);
        var perk = CreateFormKey("Skyrim.esm", 0x202);
        var baseActorValue = CreateActorValueInformation("Base.esm", formKey, "Archery", "ARC", "Base description", "BaseCNAM", 1.1, 2.2, 3.3, "Base notes", 10, "BaseFlags", "Skill", 0, 100);
        baseActorValue.PerkTree.Add(CreateActorValueInformationPerkTreeEntry("Base.esm", formKey, associatedSkill, perk, 0, "BaseFNAM", 4, 5));
        var patchActorValue = CreateActorValueInformation("Patch.esp", formKey, "Archery", "ARC", "Patch description", "PatchCNAM", 1.5, 2.5, 3.5, "Patch notes", 20, "PatchFlags", "Skill", -10, 110);
        patchActorValue.PerkTree.Add(CreateActorValueInformationPerkTreeEntry("Patch.esp", formKey, associatedSkill, perk, 0, "PatchFNAM", 6, 7));
        var actorValueInformationRepository = new TestActorValueInformationRepository
        {
            Records =
            [
                baseActorValue,
                patchActorValue
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.ActorValueInformation.RecordID,
                RecordType = SupportedRecordSpecifications.ActorValueInformation.RecordType,
                TableName = SupportedRecordSpecifications.ActorValueInformation.TableName,
                FriendlyName = SupportedRecordSpecifications.ActorValueInformation.FriendlyName,
                GameSupport = SupportedRecordSpecifications.ActorValueInformation.GameSupport,
                Fields = SupportedRecordSpecifications.ActorValueInformation.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "DefaultValue",
                            SourcePath = "DefaultValue",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            actorValueInformationRepository: actorValueInformationRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ActorValueInformation.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "DefaultValue").Values.Select(value => value.DisplayValue)
            .ShouldBe(["10", "20"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Skill.ImproveMult");
        comparison.Fields.Single(field => field.FieldName == "PerkTree").Children.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Verifies that specification-declared localized Actor Value Information rows use the selected record text
    /// language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForActorValueInformation_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Skyrim.esm", 0x152);
        var actorValueInformationRepository = new TestActorValueInformationRepository
        {
            Records =
            [
                CreateActorValueInformation("Base.esm", formKey, "Archery", "ARC", "Base description", "BaseCNAM", 1.1, 2.2, 3.3, "Base notes", 10, "BaseFlags", "Skill", 0, 100),
                CreateActorValueInformation("Patch.esp", formKey, "Archery", "ARC", "Patch description", "PatchCNAM", 1.5, 2.5, 3.5, "Patch notes", 20, "PatchFlags", "Skill", -10, 110)
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Akteurwert"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Akteurwert"),
                CreateLocalizedString("Base.esm", formKey, "Abbreviation", "German", "BAW"),
                CreateLocalizedString("Patch.esp", formKey, "Abbreviation", "German", "PAW"),
                CreateLocalizedString("Base.esm", formKey, "Description", "German", "Basis Beschreibung"),
                CreateLocalizedString("Patch.esp", formKey, "Description", "German", "Patch Beschreibung")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            actorValueInformationRepository: actorValueInformationRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ActorValueInformation.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Akteurwert", "Patch Akteurwert"]);
        comparison.Fields.Single(field => field.FieldName == "Abbreviation").Values.Select(value => value.DisplayValue)
            .ShouldBe(["BAW", "PAW"]);
        comparison.Fields.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Beschreibung", "Patch Beschreibung"]);
    }
}
