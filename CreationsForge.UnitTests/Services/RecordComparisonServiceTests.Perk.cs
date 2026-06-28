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
/// Contains record comparison scenarios for Perk records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that Perk comparison keeps strategy-owned rank rows while rendering specification-declared shared
    /// script and script-fragment child groups.
    /// </summary>
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
        var scriptFragments = comparison.Fields.Single(field => field.FieldName == "Script Fragments");
        var scriptFragment = scriptFragments.Children.Single(field => field.FieldName == "Rank");
        scriptFragment.Children.Single(field => field.FieldName == "FragmentName").Values.Select(value => value.DisplayValue)
            .ShouldBe(["BaseRankFragment", "PatchRankFragment"]);
        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        var script = scripts.Children.Single(field => field.FieldName == "Script [0]");
        script.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["SkillScript", "SkillScript"]);
        var property = script.Children.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["Base script value", "Patch script value"]);
    }

    /// <summary>
    /// Verifies that Perk scalar rows and child rows are selected from the injected comparison specification.
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
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Ranks");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Background Skills");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Script Fragments");
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
        var applicationSettingsService = new TestApplicationSettingsService { RecordTextLanguage = Language.German };
        var service = CreateService(
            perkRepository: perkRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            applicationSettingsService: applicationSettingsService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Perk.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Vorteil", "Patch Vorteil"]);
        comparison.Fields.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Beschreibung", "Patch Beschreibung"]);
    }
}
