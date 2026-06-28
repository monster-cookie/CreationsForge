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
/// Contains record comparison scenarios for Class records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that Class comparison uses specification-owned scalar rows and child-group dispatch.
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
    /// Verifies that Class scalar rows are selected from the injected comparison specification while undeclared child
    /// groups are omitted.
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
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Properties");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "SkillWeights");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "StatWeights");
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
}
