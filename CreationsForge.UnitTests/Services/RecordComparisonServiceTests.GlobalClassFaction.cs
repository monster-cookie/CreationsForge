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
/// Contains record comparison scenarios for Global, Class, and Faction records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    [Fact]
    public void GetRecordComparison_ForGlobal_CreatesPluginColumnsAndDataRows()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x123);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5, "GlobalShort", "Constant"),
                CreateGlobal("Patch.esp", formKey, "MyGlobal", 2.5, "GlobalFloat", "Constant")
            ]
        };
        var service = CreateService(globalRepository: globalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.Global.RecordID);
        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "MutagenObjectType").Values.Select(value => value.DisplayValue).ShouldBe(["GlobalShort", "GlobalFloat"]);
        comparison.Fields.Single(field => field.FieldName == "MajorFlags").Values.Select(value => value.DisplayValue).ShouldBe(["Constant", "Constant"]);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["1.5", "2.5"]);
        comparison.Fields.Single(field => field.FieldName == "Data").State.ShouldBe(RecordComparisonValueState.Conflict);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
    }

    /// <summary>
    /// Verifies that the Global pilot path reads type-specific rows from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForGlobal_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x124);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5, "GlobalShort", "Constant")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Global.RecordID,
                RecordType = SupportedRecordSpecifications.Global.RecordType,
                TableName = SupportedRecordSpecifications.Global.TableName,
                FriendlyName = SupportedRecordSpecifications.Global.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Global.GameSupport,
                Fields = SupportedRecordSpecifications.Global.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Data",
                            SourcePath = "Data",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(globalRepository: globalRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.Fields.ShouldContain(field => field.FieldName == "Data");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "MutagenObjectType");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "MajorFlags");
    }

    /// <summary>
    /// Verifies that Class comparison uses specification-owned scalar rows while retaining strategy-owned child rows.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForClass_MapsScalarFieldsAndChildGroups()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x140);
        var actorValueFormKey = CreateFormKey("Starfield.esm", 0x201);
        var baseClass = CreateClass("Base.esm", formKey, "Soldier", "Base description", "Ballistics", 50, 1.1, 2.2, 3.3, 4.4);
        baseClass.Properties.Add(CreateClassProperty("Base.esm", formKey, actorValueFormKey, 0, 10));
        baseClass.SkillWeights.Add(CreateClassWeight("Base.esm", formKey, "Skill", 0, "Ballistics", 4));
        baseClass.StatWeights.Add(CreateClassWeight("Base.esm", formKey, "Stat", 0, "Health", 5));
        var patchClass = CreateClass("Patch.esp", formKey, "Soldier", "Patch description", "Lasers", 75, 1.5, 2.5, 3.5, 4.5);
        patchClass.Properties.Add(CreateClassProperty("Patch.esp", formKey, actorValueFormKey, 0, 20));
        patchClass.SkillWeights.Add(CreateClassWeight("Patch.esp", formKey, "Skill", 0, "Ballistics", 8));
        patchClass.StatWeights.Add(CreateClassWeight("Patch.esp", formKey, "Stat", 0, "Health", 9));
        var classRepository = new TestClassRepository
        {
            Records =
            [
                baseClass,
                patchClass
            ]
        };
        var service = CreateService(classRepository: classRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Class.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Soldier", "Soldier"]);
        comparison.Fields.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue).ShouldBe(["Base description", "Patch description"]);
        comparison.Fields.Single(field => field.FieldName == "Teaches").Values.Select(value => value.DisplayValue).ShouldBe(["Ballistics", "Lasers"]);
        comparison.Fields.Single(field => field.FieldName == "MaxTrainingLevel").Values.Select(value => value.DisplayValue).ShouldBe(["50", "75"]);
        comparison.Fields.Single(field => field.FieldName == "BleedoutDefault").Values.Select(value => value.DisplayValue).ShouldBe(["1.1", "1.5"]);
        comparison.Fields.Single(field => field.FieldName == "VoicePoints").Values.Select(value => value.DisplayValue).ShouldBe(["2.2", "2.5"]);
        comparison.Fields.Single(field => field.FieldName == "Unknown").Values.Select(value => value.DisplayValue).ShouldBe(["3.3", "3.5"]);
        comparison.Fields.Single(field => field.FieldName == "Unknown2").Values.Select(value => value.DisplayValue).ShouldBe(["4.4", "4.5"]);
        var properties = comparison.Fields.Single(field => field.FieldName == "Properties");
        var property = properties.Children.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "ActorValueFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000201", "Starfield.esm:00000201"]);
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["10", "20"]);
        var skillWeights = comparison.Fields.Single(field => field.FieldName == "SkillWeights");
        skillWeights.Children.Single(field => field.FieldName == "SkillWeight [0]").Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["4", "8"]);
        var statWeights = comparison.Fields.Single(field => field.FieldName == "StatWeights");
        statWeights.Children.Single(field => field.FieldName == "StatWeight [0]").Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["5", "9"]);
    }

    /// <summary>
    /// Verifies that Class scalar rows are selected from the injected comparison specification while child rows remain
    /// strategy-based.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForClass_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x141);
        var baseClass = CreateClass("Base.esm", formKey, "Soldier", "Base description", "Ballistics", 50, 1.1, 2.2, 3.3, 4.4);
        baseClass.Properties.Add(CreateClassProperty("Base.esm", formKey, CreateFormKey("Starfield.esm", 0x201), 0, 10));
        var patchClass = CreateClass("Patch.esp", formKey, "Soldier", "Patch description", "Lasers", 75, 1.5, 2.5, 3.5, 4.5);
        patchClass.Properties.Add(CreateClassProperty("Patch.esp", formKey, CreateFormKey("Starfield.esm", 0x201), 0, 20));
        var classRepository = new TestClassRepository
        {
            Records =
            [
                baseClass,
                patchClass
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Class.RecordID,
                RecordType = SupportedRecordSpecifications.Class.RecordType,
                TableName = SupportedRecordSpecifications.Class.TableName,
                FriendlyName = SupportedRecordSpecifications.Class.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Class.GameSupport,
                Fields = SupportedRecordSpecifications.Class.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Teaches",
                            SourcePath = "Teaches",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(classRepository: classRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Class.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Teaches").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Ballistics", "Lasers"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "MaxTrainingLevel");
        comparison.Fields.Single(field => field.FieldName == "Properties").Children.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Verifies that specification-declared localized Class rows use the selected record text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForClass_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x142);
        var classRepository = new TestClassRepository
        {
            Records =
            [
                CreateClass("Base.esm", formKey, "Soldier", "Base description", "Ballistics", 50, 1.1, 2.2, 3.3, 4.4),
                CreateClass("Patch.esp", formKey, "Soldier", "Patch description", "Lasers", 75, 1.5, 2.5, 3.5, 4.5)
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Klasse"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Klasse"),
                CreateLocalizedString("Base.esm", formKey, "Description", "German", "Basis Beschreibung"),
                CreateLocalizedString("Patch.esp", formKey, "Description", "German", "Patch Beschreibung")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            classRepository: classRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Class.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Klasse", "Patch Klasse"]);
        comparison.Fields.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Beschreibung", "Patch Beschreibung"]);
    }

    /// <summary>
    /// Verifies that Faction comparison uses specification-owned scalar rows while retaining strategy-owned child rows.
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
    /// Verifies that Faction scalar rows are selected from the injected comparison specification while metadata-owned
    /// child rows remain outside the scalar metadata path.
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
        comparison.Fields.Single(field => field.FieldName == "Relations").Children.ShouldNotBeEmpty();
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
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            factionRepository: factionRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Faction.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Fraktion", "Patch Fraktion"]);
    }

    /// <summary>
    /// Verifies that Actor Value Information comparison uses specification-owned scalar rows while retaining
    /// strategy-owned perk-tree rows.
    /// </summary>
}
