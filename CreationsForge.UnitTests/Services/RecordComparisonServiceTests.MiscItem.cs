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
/// Contains record comparison scenarios for Misc Item records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that record comparison misc item maps typed scalar fields.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForMiscItem_MapsTypedScalarFields()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x818);
        var messageFormKey = CreateFormKey("Starfield.esm", 0x444);
        var baseItem = CreateMiscItem("Base.esm", formKey, "Digipick", 35, 0.1f, null);
        baseItem.Components.Add(CreateMiscItemComponent("Base.esm", formKey, CreateFormKey("Starfield.esm", 0x777), 0, 0, 2));
        baseItem.Resources.Add(CreateMiscItemResource("Base.esm", formKey, CreateFormKey("Starfield.esm", 0x778), 0, 5));
        baseItem.Destructible = CreateMiscItemDestructible(CreateFormKey("Starfield.esm", 0x888), 100, 2, "BaseStage.nif", "AABB");
        var patchItem = CreateMiscItem("Patch.esp", formKey, "Digipick", 50, 0.2f, messageFormKey);
        patchItem.Components.Add(CreateMiscItemComponent("Patch.esp", formKey, CreateFormKey("Starfield.esm", 0x777), 0, 2, 4));
        patchItem.Resources.Add(CreateMiscItemResource("Patch.esp", formKey, CreateFormKey("Starfield.esm", 0x779), 0, 6));
        patchItem.Destructible = CreateMiscItemDestructible(CreateFormKey("Starfield.esm", 0x999), 90, 3, "PatchStage.nif", "CCDD");
        var miscItemRepository = new TestMiscItemRepository
        {
            Records =
            [
                baseItem,
                patchItem
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", formKey, "Meshes\\Clutter\\Digipick.nif"),
                CreateModel("Patch.esp", formKey, "Meshes\\Clutter\\Digipick.nif")
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", formKey, "DefaultScript", "PropertyName", "BaseValue"),
                CreateScriptingAdapter("Patch.esp", formKey, "DefaultScript", "PropertyName", "PatchValue")
            ]
        };
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.MiscItem.RecordID, formKey, CreateFormKey("Starfield.esm", 0x555), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.MiscItem.RecordID, formKey, CreateFormKey("Starfield.esm", 0x555), 0)
            ]
        };
        var soundMappingRepository = new TestSoundMappingRepository
        {
            Records =
            [
                CreateSoundMapping("Base.esm", RecordTypeCatalog.MiscItem.RecordID, formKey, "PickupSound", 1, "ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6"),
                CreateSoundMapping("Patch.esp", RecordTypeCatalog.MiscItem.RecordID, formKey, "PickupSound", 1, "ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6")
            ]
        };
        var service = CreateService(
            miscItemRepository: miscItemRepository,
            modelRepository: modelRepository,
            keywordMappingRepository: keywordMappingRepository,
            soundMappingRepository: soundMappingRepository,
            scriptingAdapterRepository: scriptingAdapterRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.MiscItem.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Digipick", "Digipick"]);
        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["35", "50"]);
        comparison.Fields.Single(field => field.FieldName == "Weight").Values.Select(value => value.DisplayValue).ShouldBe(["0.1", "0.2"]);
        comparison.Fields.Single(field => field.FieldName == "FeaturedItemMessage").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:00000444"]);
        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000555", "Starfield.esm:00000555"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\Clutter\\Digipick.nif", "Meshes\\Clutter\\Digipick.nif"]);
        var sounds = comparison.Fields.Single(field => field.FieldName == "Sounds");
        var pickupSound = sounds.Children.Single(field => field.FieldName == "PickupSound [1]");
        pickupSound.Children.Single(field => field.FieldName == "Start").Values.Select(value => value.DisplayValue).ShouldBe(["ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6", "ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6"]);
        var components = comparison.Fields.Single(field => field.FieldName == "Components");
        var component = components.Children.Single(field => field.FieldName == "Component [0]");
        component.Children.Single(field => field.FieldName == "DisplayIndex").Values.Select(value => value.DisplayValue).ShouldBe(["0", "2"]);
        component.Children.Single(field => field.FieldName == "Count").Values.Select(value => value.DisplayValue).ShouldBe(["2", "4"]);
        var resources = comparison.Fields.Single(field => field.FieldName == "Resources");
        var resource = resources.Children.Single(field => field.FieldName == "Resource [0]");
        resource.Children.Single(field => field.FieldName == "Resource").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Starfield.esm:00000778", "Starfield.esm:00000779"]);
        resource.Children.Single(field => field.FieldName == "Count").Values.Select(value => value.DisplayValue)
            .ShouldBe(["5", "6"]);
        var destructible = comparison.Fields.Single(field => field.FieldName == "Destructible");
        destructible.Children.Single(field => field.FieldName == "Health").Values.Select(value => value.DisplayValue).ShouldBe(["100", "90"]);
        destructible.Children.Single(field => field.FieldName == "DESTCount").Values.Select(value => value.DisplayValue).ShouldBe(["2", "3"]);
        var stage = destructible.Children.Single(field => field.FieldName == "Stage [0]");
        stage.Children.Single(field => field.FieldName == "Explosion").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000888", "Starfield.esm:00000999"]);
        stage.Children.Single(field => field.FieldName == "Model.File").Values.Select(value => value.DisplayValue).ShouldBe(["BaseStage.nif", "PatchStage.nif"]);
        stage.Children.Single(field => field.FieldName == "Model.Data").Values.Select(value => value.DisplayValue).ShouldBe(["AABB", "CCDD"]);

        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        var script = scripts.Children.Single(field => field.FieldName == "Script [0]");
        script.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["DefaultScript", "DefaultScript"]);
        var property = script.Children.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["PropertyName", "PropertyName"]);
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["BaseValue", "PatchValue"]);
    }

    /// <summary>
    /// Verifies that Misc Item scalar rows and child rows are selected from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForMiscItem_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x819);
        var messageFormKey = CreateFormKey("Starfield.esm", 0x444);
        var baseItem = CreateMiscItem("Base.esm", formKey, "Digipick", 35, 0.1f, null);
        baseItem.Components.Add(CreateMiscItemComponent("Base.esm", formKey, CreateFormKey("Starfield.esm", 0x777), 0, 0, 2));
        baseItem.Resources.Add(CreateMiscItemResource("Base.esm", formKey, CreateFormKey("Starfield.esm", 0x778), 0, 5));
        baseItem.Destructible = CreateMiscItemDestructible(CreateFormKey("Starfield.esm", 0x888), 100, 2, "BaseStage.nif", "AABB");
        var patchItem = CreateMiscItem("Patch.esp", formKey, "Digipick", 50, 0.2f, messageFormKey);
        patchItem.Components.Add(CreateMiscItemComponent("Patch.esp", formKey, CreateFormKey("Starfield.esm", 0x777), 0, 2, 4));
        patchItem.Resources.Add(CreateMiscItemResource("Patch.esp", formKey, CreateFormKey("Starfield.esm", 0x779), 0, 6));
        patchItem.Destructible = CreateMiscItemDestructible(CreateFormKey("Starfield.esm", 0x999), 90, 3, "PatchStage.nif", "CCDD");
        var miscItemRepository = new TestMiscItemRepository
        {
            Records =
            [
                baseItem,
                patchItem
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", formKey, "Meshes\\Clutter\\Digipick.nif"),
                CreateModel("Patch.esp", formKey, "Meshes\\Clutter\\Digipick.nif")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.MiscItem.RecordID,
                RecordType = SupportedRecordSpecifications.MiscItem.RecordType,
                TableName = SupportedRecordSpecifications.MiscItem.TableName,
                FriendlyName = SupportedRecordSpecifications.MiscItem.FriendlyName,
                GameSupport = SupportedRecordSpecifications.MiscItem.GameSupport,
                Fields = SupportedRecordSpecifications.MiscItem.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Value",
                            SourcePath = "Value",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ],
                    ChildGroups =
                    [
                        new RecordComparisonChildGroupSpecification
                        {
                            GroupKind = RecordComparisonChildGroupKind.ModelMappings,
                            GroupName = "Models",
                            Description = "Test model child group."
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(
            miscItemRepository: miscItemRepository,
            modelRepository: modelRepository,
            recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.MiscItem.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["35", "50"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Weight");
        comparison.Fields.Single(field => field.FieldName == "Model").Children.ShouldNotBeEmpty();
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Destructible");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Components");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Resources");
    }
}
