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
/// Contains record comparison scenarios for Magic Effect records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that record comparison magic effect expands keywords and flattens magic effect data.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForMagicEffect_ExpandsKeywordsAndFlattensMagicEffectData()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2C5A68);
        var magicEffectRepository = new TestMagicEffectRepository
        {
            Records =
            [
                CreateMagicEffect("Base.esm", formKey, "Elemental Blast", "52", 5),
                CreateMagicEffect("Patch.esp", formKey, "Elemental Blast", "52", 7)
            ]
        };
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, CreateFormKey("Starfield.esm", 0x111), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, CreateFormKey("Patch.esp", 0x222), 0)
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, "FXScripts:FXResourceCollectionVisuals", "TargetVFX", "BaseVFX"),
                CreateScriptingAdapter("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, "FXScripts:FXResourceCollectionVisuals", "TargetVFX", "PatchVFX")
            ]
        };
        var soundMappingRepository = new TestSoundMappingRepository
        {
            Records =
            [
                CreateSoundMapping("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, "Charge", 2, "a328413d-b619-45b5-0d9e-aa9d0ade8280", "Break0", "000000"),
                CreateSoundMapping("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, "Charge", 2, "a328413d-b619-45b5-0d9e-aa9d0ade8280", "Break0", "000000")
            ]
        };
        var service = CreateService(
            magicEffectRepository: magicEffectRepository,
            keywordMappingRepository: keywordMappingRepository,
            soundMappingRepository: soundMappingRepository,
            scriptingAdapterRepository: scriptingAdapterRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.MagicEffect.RecordID, formKey);

        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000111", "Patch.esp:00000222"]);

        comparison.Fields.ShouldNotContain(field => field.FieldName == "Data");
        comparison.Fields.Single(field => field.FieldName == "Archetype").Values.Select(value => value.DisplayValue).ShouldBe(["52", "52"]);
        comparison.Fields.Single(field => field.FieldName == "UnknownInt2").Values.Select(value => value.DisplayValue).ShouldBe(["5", "7"]);
        var sounds = comparison.Fields.Single(field => field.FieldName == "Sounds");
        var chargeSound = sounds.Children.Single(field => field.FieldName == "Charge [2]");
        chargeSound.Children.Single(field => field.FieldName == "Start").Values.Select(value => value.DisplayValue).ShouldBe(["a328413d-b619-45b5-0d9e-aa9d0ade8280", "a328413d-b619-45b5-0d9e-aa9d0ade8280"]);
        chargeSound.Children.Single(field => field.FieldName == "Versioning").Values.Select(value => value.DisplayValue).ShouldBe(["Break0", "Break0"]);

        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        var script = scripts.Children.Single(field => field.FieldName == "Script [0]");
        script.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["FXScripts:FXResourceCollectionVisuals", "FXScripts:FXResourceCollectionVisuals"]);
        var property = script.Children.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["TargetVFX", "TargetVFX"]);
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["BaseVFX", "PatchVFX"]);
    }

    /// <summary>
    /// Verifies that Magic Effect scalar rows are selected from the injected comparison specification while strategy
    /// rows remain outside the scalar metadata path.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForMagicEffect_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2C5A69);
        var magicEffectRepository = new TestMagicEffectRepository
        {
            Records =
            [
                CreateMagicEffect("Base.esm", formKey, "Elemental Blast", "52", 5, castType: "BaseCast"),
                CreateMagicEffect("Patch.esp", formKey, "Elemental Blast", "60", 7, castType: "PatchCast")
            ]
        };
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, CreateFormKey("Starfield.esm", 0x111), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, CreateFormKey("Patch.esp", 0x222), 0)
            ]
        };
        var soundMappingRepository = new TestSoundMappingRepository
        {
            Records =
            [
                CreateSoundMapping("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, "Charge", 2, "BaseSound", "Break0", "000000"),
                CreateSoundMapping("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, "Charge", 2, "PatchSound", "Break0", "000000")
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, "FXScript", "TargetVFX", "BaseVFX"),
                CreateScriptingAdapter("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, "FXScript", "TargetVFX", "PatchVFX")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.MagicEffect.RecordID,
                RecordType = SupportedRecordSpecifications.MagicEffect.RecordType,
                TableName = SupportedRecordSpecifications.MagicEffect.TableName,
                FriendlyName = SupportedRecordSpecifications.MagicEffect.FriendlyName,
                GameSupport = SupportedRecordSpecifications.MagicEffect.GameSupport,
                Fields = SupportedRecordSpecifications.MagicEffect.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "CastType",
                            SourcePath = "CastType",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ],
                    ChildGroups =
                    [
                        new RecordComparisonChildGroupSpecification
                        {
                            GroupKind = RecordComparisonChildGroupKind.KeywordMappings,
                            GroupName = "Keywords",
                            Description = "Test keyword child group."
                        },
                        new RecordComparisonChildGroupSpecification
                        {
                            GroupKind = RecordComparisonChildGroupKind.SoundMappings,
                            GroupName = "Sounds",
                            Description = "Test sound child group."
                        },
                        new RecordComparisonChildGroupSpecification
                        {
                            GroupKind = RecordComparisonChildGroupKind.ScriptingAdapterMappings,
                            GroupName = "Scripts",
                            Description = "Test scripting adapter child group."
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            magicEffectRepository: magicEffectRepository,
            keywordMappingRepository: keywordMappingRepository,
            soundMappingRepository: soundMappingRepository,
            scriptingAdapterRepository: scriptingAdapterRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.MagicEffect.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "CastType").Values.Select(value => value.DisplayValue)
            .ShouldBe(["BaseCast", "PatchCast"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Archetype");
        comparison.Fields.Single(field => field.FieldName == "Keywords").Children.ShouldNotBeEmpty();
        comparison.Fields.Single(field => field.FieldName == "Sounds").Children.ShouldNotBeEmpty();
        comparison.Fields.Single(field => field.FieldName == "Scripts").Children.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Verifies that Magic Effect child rows are controlled by child-group metadata rather than scalar field metadata.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForMagicEffect_UsesInjectedChildGroupSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2C5A70);
        var magicEffectRepository = new TestMagicEffectRepository
        {
            Records =
            [
                CreateMagicEffect("Base.esm", formKey, "Elemental Blast", "52", 5, castType: "BaseCast"),
                CreateMagicEffect("Patch.esp", formKey, "Elemental Blast", "60", 7, castType: "PatchCast")
            ]
        };
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, CreateFormKey("Starfield.esm", 0x111), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, CreateFormKey("Patch.esp", 0x222), 0)
            ]
        };
        var soundMappingRepository = new TestSoundMappingRepository
        {
            Records =
            [
                CreateSoundMapping("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, "Charge", 2, "BaseSound", "Break0", "000000"),
                CreateSoundMapping("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, "Charge", 2, "PatchSound", "Break0", "000000")
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, "FXScript", "TargetVFX", "BaseVFX"),
                CreateScriptingAdapter("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, "FXScript", "TargetVFX", "PatchVFX")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.MagicEffect.RecordID,
                RecordType = SupportedRecordSpecifications.MagicEffect.RecordType,
                TableName = SupportedRecordSpecifications.MagicEffect.TableName,
                FriendlyName = SupportedRecordSpecifications.MagicEffect.FriendlyName,
                GameSupport = SupportedRecordSpecifications.MagicEffect.GameSupport,
                Fields = SupportedRecordSpecifications.MagicEffect.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "CastType",
                            SourcePath = "CastType",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            magicEffectRepository: magicEffectRepository,
            keywordMappingRepository: keywordMappingRepository,
            soundMappingRepository: soundMappingRepository,
            scriptingAdapterRepository: scriptingAdapterRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.MagicEffect.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "CastType").Values.Select(value => value.DisplayValue)
            .ShouldBe(["BaseCast", "PatchCast"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Keywords");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Sounds");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Scripts");
    }

    /// <summary>
    /// Verifies that specification-declared localized Magic Effect rows use the selected record text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForMagicEffect_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2C5A70);
        var magicEffectRepository = new TestMagicEffectRepository
        {
            Records =
            [
                CreateMagicEffect("Base.esm", formKey, "Elemental Blast", "52", 5, description: "Base description"),
                CreateMagicEffect("Patch.esp", formKey, "Elemental Blast", "60", 7, description: "Patch description")
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Magieeffekt"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Magieeffekt"),
                CreateLocalizedString("Base.esm", formKey, "Description", "German", "Basis Beschreibung"),
                CreateLocalizedString("Patch.esp", formKey, "Description", "German", "Patch Beschreibung")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            magicEffectRepository: magicEffectRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.MagicEffect.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Magieeffekt", "Patch Magieeffekt"]);
        comparison.Fields.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Beschreibung", "Patch Beschreibung"]);
    }
}
