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
/// Contains record comparison scenarios for magic effects, perks, statics, containers, and constructible objects.
/// </summary>
public partial class RecordComparisonServiceTests
{
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

    [Fact]
    public void GetRecordComparison_ForPerk_ExpandsRanksBackgroundSkillsAndScripts()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2CE2C0);
        var staticFormKey = CreateFormKey("Starfield.esm", 0x333);
        var backgroundSkillFormKey = CreateFormKey("Starfield.esm", 0x444);
        var perkRepository = new TestPerkRepository
        {
            Records =
            [
                CreatePerk("Base.esm", formKey, "Chemistry", staticFormKey, backgroundSkillFormKey, "Base description", "Base value"),
                CreatePerk("Patch.esp", formKey, "Chemistry", staticFormKey, backgroundSkillFormKey, "Patch description", "Patch value")
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", RecordTypeCatalog.Perk.RecordID, formKey, "SkillScript", "RankProperty", "Base script value"),
                CreateScriptingAdapter("Patch.esp", RecordTypeCatalog.Perk.RecordID, formKey, "SkillScript", "RankProperty", "Patch script value")
            ]
        };
        var service = CreateService(
            perkRepository: perkRepository,
            scriptingAdapterRepository: scriptingAdapterRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Perk.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Chemistry", "Chemistry"]);
        var ranks = comparison.Fields.Single(field => field.FieldName == "Ranks");
        var rank = ranks.Children.Single(field => field.FieldName == "Rank [0]");
        rank.Children.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue).ShouldBe(["Base description", "Patch description"]);
        rank.Children.Single(field => field.FieldName == "UnknownStaticFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000333", "Starfield.esm:00000333"]);
        var effects = rank.Children.Single(field => field.FieldName == "Effects");
        var effect = effects.Children.Single(field => field.FieldName == "Effect [0]");
        effect.Children.Single(field => field.FieldName == "MutagenObjectType").Values.Select(value => value.DisplayValue).ShouldBe(["PerkEntryPointModifyValue", "PerkEntryPointModifyValue"]);
        effect.Children.Single(field => field.FieldName == "EntryPoint").Values.Select(value => value.DisplayValue).ShouldBe(["ModSkillUse", "ModSkillUse"]);
        effect.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["1.5", "2.5"]);
        var backgroundSkills = comparison.Fields.Single(field => field.FieldName == "Background Skills");
        backgroundSkills.Children.Single(field => field.FieldName == "Skill [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000444", "Starfield.esm:00000444"]);
        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        var script = scripts.Children.Single(field => field.FieldName == "Script [0]");
        script.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["SkillScript", "SkillScript"]);
        var property = script.Children.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["Base script value", "Patch script value"]);
    }

    /// <summary>
    /// Verifies that Perk scalar rows are selected from the injected comparison specification while strategy rows
    /// remain outside the scalar metadata path.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForPerk_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2CE2C1);
        var staticFormKey = CreateFormKey("Starfield.esm", 0x333);
        var backgroundSkillFormKey = CreateFormKey("Starfield.esm", 0x444);
        var perkRepository = new TestPerkRepository
        {
            Records =
            [
                CreatePerk("Base.esm", formKey, "Chemistry", staticFormKey, backgroundSkillFormKey, "Base description", "Base value"),
                CreatePerk("Patch.esp", formKey, "Chemistry", staticFormKey, backgroundSkillFormKey, "Patch description", "Patch value")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Perk.RecordID,
                RecordType = SupportedRecordSpecifications.Perk.RecordType,
                TableName = SupportedRecordSpecifications.Perk.TableName,
                FriendlyName = SupportedRecordSpecifications.Perk.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Perk.GameSupport,
                Fields = SupportedRecordSpecifications.Perk.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "PerkIcon",
                            SourcePath = "PerkIcon",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(perkRepository: perkRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Perk.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "PerkIcon").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Patch_Science_Chemistry", "Patch_Science_Chemistry"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "SkillGroup");
        comparison.Fields.Single(field => field.FieldName == "Ranks").Children.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Verifies that specification-declared localized Perk rows use the selected record text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForPerk_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2CE2C2);
        var staticFormKey = CreateFormKey("Starfield.esm", 0x333);
        var backgroundSkillFormKey = CreateFormKey("Starfield.esm", 0x444);
        var perkRepository = new TestPerkRepository
        {
            Records =
            [
                CreatePerk("Base.esm", formKey, "Chemistry", staticFormKey, backgroundSkillFormKey, "Base description", "Base value"),
                CreatePerk("Patch.esp", formKey, "Chemistry", staticFormKey, backgroundSkillFormKey, "Patch description", "Patch value")
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Vorteil"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Vorteil"),
                CreateLocalizedString("Base.esm", formKey, "Description", "German", "Basis Beschreibung"),
                CreateLocalizedString("Patch.esp", formKey, "Description", "German", "Patch Beschreibung")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            perkRepository: perkRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Perk.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Vorteil", "Patch Vorteil"]);
        comparison.Fields.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Beschreibung", "Patch Beschreibung"]);
    }

    [Fact]
    public void GetRecordComparison_ForStatic_MapsStaticFieldsModelDataAndReflectPayloads()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x1000);
        var staticRepository = new TestStaticRepository
        {
            Records =
            [
                CreateStatic("Base.esm", formKey, 35, "0, 0, 0", null),
                CreateStatic("Patch.esp", formKey, 45, "0, 0, 0", 1.25)
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Static.RecordID, formKey, "Meshes\\SetDressing\\Rock01.nif", "AABB"),
                CreateModel("Patch.esp", RecordTypeCatalog.Static.RecordID, formKey, "Meshes\\SetDressing\\Rock01.nif", "CCDD")
            ]
        };
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.Static.RecordID, formKey, CreateFormKey("Starfield.esm", 0x555), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.Static.RecordID, formKey, CreateFormKey("Starfield.esm", 0x666), 0)
            ]
        };
        var reflectionRepository = new TestReflectionRepository
        {
            Records =
            [
                CreateReflection("Base.esm", formKey, 0, "ReflectionComponent", "Components[0].REFL", "AABB"),
                CreateReflection("Patch.esp", formKey, 0, "ReflectionComponent", "Components[0].REFL", "CCDD")
            ]
        };
        var service = CreateService(
            staticRepository: staticRepository,
            modelRepository: modelRepository,
            keywordMappingRepository: keywordMappingRepository,
            reflectionRepository: reflectionRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Static.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.Static.RecordID);
        comparison.Fields.Single(field => field.FieldName == "MaxAngle").Values.Select(value => value.DisplayValue).ShouldBe(["35", "45"]);
        comparison.Fields.Single(field => field.FieldName == "ObjectBoundsFirst").Values.Select(value => value.DisplayValue).ShouldBe(["0, 0, 0", "0, 0, 0"]);
        comparison.Fields.Single(field => field.FieldName == "UnknownDNAMFloat").Values.Select(value => value.DisplayValue).ShouldBe(["", "1.25"]);
        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000555", "Starfield.esm:00000666"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\SetDressing\\Rock01.nif", "Meshes\\SetDressing\\Rock01.nif"]);
        model.Children.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["AABB", "CCDD"]);
        var reflection = comparison.Fields.Single(field => field.FieldName == "Reflection");
        var reflect = reflection.Children.Single(field => field.FieldName == "Components[0].REFL");
        reflect.Children.Single(field => field.FieldName == "ComponentType").Values.Select(value => value.DisplayValue).ShouldBe(["ReflectionComponent", "ReflectionComponent"]);
        reflect.Children.Single(field => field.FieldName == "SourcePath").Values.Select(value => value.DisplayValue).ShouldBe(["Components[0].REFL", "Components[0].REFL"]);
        var reflectValues = reflect.Children.Single(field => field.FieldName == "REFL").Values;
        reflectValues.Select(value => value.DisplayValue).ShouldBe(["[UNPARSEABLE REFLECTION DATA]", "[UNPARSEABLE REFLECTION DATA]"]);
        reflectValues.Select(value => value.DetailValue).ShouldBe(["AABB", "CCDD"]);
        reflectValues.Select(value => value.DisplayKind).ShouldBe([RecordComparisonValueDisplayKind.RawBinaryPayload, RecordComparisonValueDisplayKind.RawBinaryPayload]);
        reflect.Children.Single(field => field.FieldName == "REFL").State.ShouldBe(RecordComparisonValueState.Conflict);
    }

    [Fact]
    public void GetRecordComparison_ForContainer_MapsContainerFieldsItemsModelsAndAnimationFields()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2000);
        var itemFormKey = CreateFormKey("Starfield.esm", 0x333);
        var terminalFormKey = CreateFormKey("Starfield.esm", 0x444);
        var containerRepository = new TestContainerRepository
        {
            Records =
            [
                CreateContainer("Base.esm", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Base.esm", formKey, itemFormKey, 0, 2)], "meshes\\base.anim"),
                CreateContainer("Patch.esp", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Patch.esp", formKey, itemFormKey, 0, 4)], "meshes\\patch.anim")
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Container.RecordID, formKey, "Meshes\\SetDressing\\Container01.nif"),
                CreateModel("Patch.esp", RecordTypeCatalog.Container.RecordID, formKey, "Meshes\\SetDressing\\Container01.nif")
            ]
        };
        var service = CreateService(
            containerRepository: containerRepository,
            modelRepository: modelRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Container.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.Container.RecordID);
        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Storage Crate", "Storage Crate"]);
        comparison.Fields.Single(field => field.FieldName == "NativeTerminalFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000444", "Starfield.esm:00000444"]);
        var items = comparison.Fields.Single(field => field.FieldName == "Items");
        var item = items.Children.Single(field => field.FieldName == "Item [0]");
        item.Children.Single(field => field.FieldName == "Item").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000333", "Starfield.esm:00000333"]);
        item.Children.Single(field => field.FieldName == "Count").Values.Select(value => value.DisplayValue).ShouldBe(["2", "4"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\SetDressing\\Container01.nif", "Meshes\\SetDressing\\Container01.nif"]);
        comparison.Fields.Single(field => field.FieldName == "AnimationGraph").Values.Select(value => value.DisplayValue).ShouldBe(["meshes\\base.anim", "meshes\\patch.anim"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Base Form Components");
    }

    [Fact]
    public void GetRecordComparison_ForConstructibleObject_MapsComponentsConditionsScriptsAndCreatedObjectCount()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2500);
        var createdObjectFormKey = CreateFormKey("Starfield.esm", 0x111);
        var workbenchKeywordFormKey = CreateFormKey("Starfield.esm", 0x222);
        var componentFormKey = CreateFormKey("Starfield.esm", 0x333);
        var recipeFilterFormKey = CreateFormKey("Starfield.esm", 0x444);
        var constructibleObjectRepository = new TestConstructibleObjectRepository
        {
            Records =
            [
                CreateConstructibleObject("Base.esm", formKey, createdObjectFormKey, workbenchKeywordFormKey, componentFormKey, recipeFilterFormKey, 2),
                CreateConstructibleObject("Patch.esp", formKey, createdObjectFormKey, workbenchKeywordFormKey, componentFormKey, recipeFilterFormKey, 4)
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", RecordTypeCatalog.ConstructibleObject.RecordID, formKey, "RecipeScript", "Enabled", "True"),
                CreateScriptingAdapter("Patch.esp", RecordTypeCatalog.ConstructibleObject.RecordID, formKey, "RecipeScript", "Enabled", "False")
            ]
        };
        var service = CreateService(
            constructibleObjectRepository: constructibleObjectRepository,
            scriptingAdapterRepository: scriptingAdapterRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConstructibleObject.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.ConstructibleObject.RecordID);
        comparison.Fields.Single(field => field.FieldName == "CreatedObjectFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000111", "Starfield.esm:00000111"]);
        comparison.Fields.Single(field => field.FieldName == "WorkbenchKeywordFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000222", "Starfield.esm:00000222"]);
        comparison.Fields.Single(field => field.FieldName == "CreatedObjectCount").Values.Select(value => value.DisplayValue).ShouldBe(["2", "4"]);
        comparison.Fields.Single(field => field.FieldName == "AmountProduced").Values.Select(value => value.DisplayValue).ShouldBe(["2", "4"]);
        var components = comparison.Fields.Single(field => field.FieldName == "Components");
        var component = components.Children.Single(field => field.FieldName == "Component [0]");
        component.Children.Single(field => field.FieldName == "ComponentFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000333", "Starfield.esm:00000333"]);
        component.Children.Single(field => field.FieldName == "Count").Values.Select(value => value.DisplayValue).ShouldBe(["3", "3"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Categories");
        var recipeFilters = comparison.Fields.Single(field => field.FieldName == "RecipeFilters");
        recipeFilters.Children.Single(field => field.FieldName == "RecipeFilter [0]").Children.Single(field => field.FieldName == "RecipeFilterFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000444", "Starfield.esm:00000444"]);
        var conditions = comparison.Fields.Single(field => field.FieldName == "Conditions");
        var condition = conditions.Children.Single();
        condition.FieldName.ShouldBe("Condition [0]");
        condition.Values.Select(value => value.DisplayValue).ShouldBe(["GetItemCount() EqualTo 2", "GetItemCount() EqualTo 4"]);
        condition.State.ShouldBe(RecordComparisonValueState.Conflict);
        condition.Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
        condition.Children.ShouldBeEmpty();
        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        scripts.Children.Single(field => field.FieldName == "Script [0]").Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["RecipeScript", "RecipeScript"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Created Object Counts");
    }

    /// <summary>
    /// Verifies that Constructible Object scalar rows are selected from the injected comparison specification while
    /// child rows remain strategy-based.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForConstructibleObject_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2501);
        var createdObjectFormKey = CreateFormKey("Starfield.esm", 0x111);
        var workbenchKeywordFormKey = CreateFormKey("Starfield.esm", 0x222);
        var componentFormKey = CreateFormKey("Starfield.esm", 0x333);
        var recipeFilterFormKey = CreateFormKey("Starfield.esm", 0x444);
        var constructibleObjectRepository = new TestConstructibleObjectRepository
        {
            Records =
            [
                CreateConstructibleObject("Base.esm", formKey, createdObjectFormKey, workbenchKeywordFormKey, componentFormKey, recipeFilterFormKey, 2),
                CreateConstructibleObject("Patch.esp", formKey, createdObjectFormKey, workbenchKeywordFormKey, componentFormKey, recipeFilterFormKey, 4)
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.ConstructibleObject.RecordID,
                RecordType = SupportedRecordSpecifications.ConstructibleObject.RecordType,
                TableName = SupportedRecordSpecifications.ConstructibleObject.TableName,
                FriendlyName = SupportedRecordSpecifications.ConstructibleObject.FriendlyName,
                GameSupport = SupportedRecordSpecifications.ConstructibleObject.GameSupport,
                Fields = SupportedRecordSpecifications.ConstructibleObject.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "AmountProduced",
                            SourcePath = "AmountProduced",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            constructibleObjectRepository: constructibleObjectRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConstructibleObject.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "AmountProduced").Values.Select(value => value.DisplayValue)
            .ShouldBe(["2", "4"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "CreatedObjectFormKey");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "WorkbenchKeywordFormKey");
        comparison.Fields.Single(field => field.FieldName == "Components").Children.ShouldNotBeEmpty();
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Conditions");
    }

}
