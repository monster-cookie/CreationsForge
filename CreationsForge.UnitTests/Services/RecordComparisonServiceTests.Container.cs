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
/// Contains record comparison scenarios for Container records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that Container scalar rows are selected from the injected comparison specification while undeclared
    /// child groups are omitted.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForContainer_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x12B);
        var itemFormKey = CreateFormKey("Starfield.esm", 0x333);
        var terminalFormKey = CreateFormKey("Starfield.esm", 0x444);
        var actorValueFormKey = CreateFormKey("Starfield.esm", 0x555);
        var forcedLocationFormKey = CreateFormKey("Starfield.esm", 0x666);
        var baseContainer = CreateContainer("Base.esm", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Base.esm", formKey, itemFormKey, 0, 2)], "meshes\\base.anim");
        baseContainer.Properties.Add(CreateContainerProperty("Base.esm", formKey, actorValueFormKey, 0, 10));
        baseContainer.ForcedLocations.Add(forcedLocationFormKey);
        var patchContainer = CreateContainer("Patch.esp", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Patch.esp", formKey, itemFormKey, 0, 4)], "meshes\\patch.anim");
        patchContainer.Properties.Add(CreateContainerProperty("Patch.esp", formKey, actorValueFormKey, 0, 20));
        patchContainer.ForcedLocations.Add(forcedLocationFormKey);
        var containerRepository = new TestContainerRepository
        {
            Records =
            [
                baseContainer,
                patchContainer
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Container.RecordID,
                RecordType = SupportedRecordSpecifications.Container.RecordType,
                TableName = SupportedRecordSpecifications.Container.TableName,
                FriendlyName = SupportedRecordSpecifications.Container.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Container.GameSupport,
                Fields = SupportedRecordSpecifications.Container.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "AnimationGraph",
                            SourcePath = "AnimationGraph",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(containerRepository: containerRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Container.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "AnimationGraph").Values.Select(value => value.DisplayValue)
            .ShouldBe(["meshes\\base.anim", "meshes\\patch.anim"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "NativeTerminalFormKey");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Items");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Property [0]");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "ForcedLocations[0]");
    }

    /// <summary>
    /// Verifies that Container comparison maps scalar rows, item rows, property rows, forced locations, model rows,
    /// and animation fields.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForContainer_MapsContainerFieldsItemsModelsAndAnimationFields()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2000);
        var itemFormKey = CreateFormKey("Starfield.esm", 0x333);
        var terminalFormKey = CreateFormKey("Starfield.esm", 0x444);
        var actorValueFormKey = CreateFormKey("Starfield.esm", 0x555);
        var forcedLocationFormKey = CreateFormKey("Starfield.esm", 0x666);
        var baseContainer = CreateContainer("Base.esm", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Base.esm", formKey, itemFormKey, 0, 2)], "meshes\\base.anim");
        baseContainer.Properties.Add(CreateContainerProperty("Base.esm", formKey, actorValueFormKey, 0, 10));
        baseContainer.ForcedLocations.Add(forcedLocationFormKey);
        var patchContainer = CreateContainer("Patch.esp", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Patch.esp", formKey, itemFormKey, 0, 4)], "meshes\\patch.anim");
        patchContainer.Properties.Add(CreateContainerProperty("Patch.esp", formKey, actorValueFormKey, 0, 20));
        patchContainer.ForcedLocations.Add(forcedLocationFormKey);
        var containerRepository = new TestContainerRepository
        {
            Records =
            [
                baseContainer,
                patchContainer
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
        var property = comparison.Fields.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "ActorValue").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000555", "Starfield.esm:00000555"]);
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["10", "20"]);
        comparison.Fields.Single(field => field.FieldName == "ForcedLocations[0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000666", "Starfield.esm:00000666"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\SetDressing\\Container01.nif", "Meshes\\SetDressing\\Container01.nif"]);
        comparison.Fields.Single(field => field.FieldName == "AnimationGraph").Values.Select(value => value.DisplayValue).ShouldBe(["meshes\\base.anim", "meshes\\patch.anim"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Base Form Components");
    }
}
