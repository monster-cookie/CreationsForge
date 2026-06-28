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
/// Contains record comparison scenarios for Door records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that Door scalar rows are selected from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForDoor_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x12A);
        var nativeTerminalFormKey = CreateFormKey("Starfield.esm", 0x555);
        var doorRepository = new TestDoorRepository
        {
            Records =
            [
                CreateDoor("Base.esm", formKey, "Airlock", nativeTerminalFormKey, "Both"),
                CreateDoor("Patch.esp", formKey, "Airlock", nativeTerminalFormKey, "Positive")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Door.RecordID,
                RecordType = SupportedRecordSpecifications.Door.RecordType,
                TableName = SupportedRecordSpecifications.Door.TableName,
                FriendlyName = SupportedRecordSpecifications.Door.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Door.GameSupport,
                Fields = SupportedRecordSpecifications.Door.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "FacingAxisOverride",
                            SourcePath = "FacingAxisOverride",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(doorRepository: doorRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Door.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "FacingAxisOverride").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Both", "Positive"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "NativeTerminalFormKey");
    }

    /// <summary>
    /// Verifies that record comparison door maps door fields and children.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForDoor_MapsDoorFieldsAndChildren()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x4000);
        var nativeTerminalFormKey = CreateFormKey("Starfield.esm", 0x555);
        var doorRepository = new TestDoorRepository
        {
            Records =
            [
                CreateDoor("Base.esm", formKey, "Airlock", nativeTerminalFormKey, "Both"),
                CreateDoor("Patch.esp", formKey, "Airlock", nativeTerminalFormKey, "Positive")
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Door.RecordID, formKey, "Meshes\\Architecture\\Door01.nif"),
                CreateModel("Patch.esp", RecordTypeCatalog.Door.RecordID, formKey, "Meshes\\Architecture\\Door01.nif")
            ]
        };
        var service = CreateService(
            doorRepository: doorRepository,
            modelRepository: modelRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Door.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Airlock", "Airlock"]);
        comparison.Fields.Single(field => field.FieldName == "NativeTerminalFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000555", "Starfield.esm:00000555"]);
        comparison.Fields.Single(field => field.FieldName == "FacingAxisOverride").Values.Select(value => value.DisplayValue).ShouldBe(["Both", "Positive"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\Architecture\\Door01.nif", "Meshes\\Architecture\\Door01.nif"]);
    }
}
