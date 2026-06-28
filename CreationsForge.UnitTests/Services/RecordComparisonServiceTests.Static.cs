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
/// Contains record comparison scenarios for Static records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that Static scalar rows are selected from the injected comparison specification while undeclared
    /// property, model, and reflection child rows remain outside the metadata path.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForStatic_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x126);
        var actorValueFormKey = CreateFormKey("Starfield.esm", 0x201);
        var baseStatic = CreateStatic("Base.esm", formKey, 35, "0, 0, 0", null);
        baseStatic.Properties.Add(CreateStaticProperty("Base.esm", formKey, actorValueFormKey, 0, 10));
        var patchStatic = CreateStatic("Patch.esp", formKey, 45, "1, 1, 1", 1.25);
        patchStatic.Properties.Add(CreateStaticProperty("Patch.esp", formKey, actorValueFormKey, 0, 20));
        var staticRepository = new TestStaticRepository
        {
            Records =
            [
                baseStatic,
                patchStatic
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Static.RecordID, formKey, "Meshes\\SetDressing\\Rock01.nif"),
                CreateModel("Patch.esp", RecordTypeCatalog.Static.RecordID, formKey, "Meshes\\SetDressing\\Rock01.nif")
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
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Static.RecordID,
                RecordType = SupportedRecordSpecifications.Static.RecordType,
                TableName = SupportedRecordSpecifications.Static.TableName,
                FriendlyName = SupportedRecordSpecifications.Static.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Static.GameSupport,
                Fields = SupportedRecordSpecifications.Static.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "MaxAngle",
                            SourcePath = "MaxAngle",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            staticRepository: staticRepository,
            modelRepository: modelRepository,
            reflectionRepository: reflectionRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Static.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "MaxAngle").Values.Select(value => value.DisplayValue)
            .ShouldBe(["35", "45"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "ObjectBoundsFirst");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Property [0]");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Model");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Reflection");
    }

    /// <summary>
    /// Verifies that specification-declared localized Static rows use the selected record text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForStatic_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x128);
        var staticRepository = new TestStaticRepository
        {
            Records =
            [
                CreateStatic("Base.esm", formKey, 35, "0, 0, 0", null),
                CreateStatic("Patch.esp", formKey, 45, "1, 1, 1", 1.25)
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Statik"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Statik")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            staticRepository: staticRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Static.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Statik", "Patch Statik"]);
    }

    /// <summary>
    /// Verifies that Static comparison maps scalar rows, property rows, model data, and reflection payloads.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForStatic_MapsStaticFieldsModelDataAndReflectPayloads()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x1000);
        var actorValueFormKey = CreateFormKey("Starfield.esm", 0x201);
        var baseStatic = CreateStatic("Base.esm", formKey, 35, "0, 0, 0", null);
        baseStatic.Properties.Add(CreateStaticProperty("Base.esm", formKey, actorValueFormKey, 0, 10));
        var patchStatic = CreateStatic("Patch.esp", formKey, 45, "0, 0, 0", 1.25);
        patchStatic.Properties.Add(CreateStaticProperty("Patch.esp", formKey, actorValueFormKey, 0, 20));
        var staticRepository = new TestStaticRepository
        {
            Records =
            [
                baseStatic,
                patchStatic
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
        var property = comparison.Fields.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "ActorValue").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000201", "Starfield.esm:00000201"]);
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["10", "20"]);
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
}
