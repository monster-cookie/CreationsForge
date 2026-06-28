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
/// Contains record comparison scenarios for Book records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that Book scalar rows are selected from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForBook_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x129);
        var bookRepository = new TestBookRepository
        {
            Records =
            [
                CreateBook("Base.esm", formKey, "Captain's Log", 100),
                CreateBook("Patch.esp", formKey, "Captain's Log", 150)
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Book.RecordID,
                RecordType = SupportedRecordSpecifications.Book.RecordType,
                TableName = SupportedRecordSpecifications.Book.TableName,
                FriendlyName = SupportedRecordSpecifications.Book.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Book.GameSupport,
                Fields = SupportedRecordSpecifications.Book.Fields,
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
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(bookRepository: bookRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Book.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue)
            .ShouldBe(["100", "150"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Flags");
    }

    /// <summary>
    /// Verifies that record comparison book maps book fields and children.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForBook_MapsBookFieldsAndChildren()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x3000);
        var bookRepository = new TestBookRepository
        {
            Records =
            [
                CreateBook("Base.esm", formKey, "Captain's Log", 100),
                CreateBook("Patch.esp", formKey, "Captain's Log", 150)
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, "Meshes\\SetDressing\\Books\\Book01.nif"),
                CreateModel("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, "Meshes\\SetDressing\\Books\\Book01.nif")
            ]
        };
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, CreateFormKey("Starfield.esm", 0x101), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, CreateFormKey("Starfield.esm", 0x101), 0)
            ]
        };
        var soundMappingRepository = new TestSoundMappingRepository
        {
            Records =
            [
                CreateSoundMapping("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, "PickupSound", 0, "pickup"),
                CreateSoundMapping("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, "PickupSound", 0, "pickup")
            ]
        };
        var service = CreateService(
            bookRepository: bookRepository,
            modelRepository: modelRepository,
            keywordMappingRepository: keywordMappingRepository,
            soundMappingRepository: soundMappingRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Book.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Captain's Log", "Captain's Log"]);
        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["100", "150"]);
        comparison.Fields.Single(field => field.FieldName == "Transforms.Inventory").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000999", "Starfield.esm:00000999"]);
        comparison.Fields.Single(field => field.FieldName == "InventoryArt").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000998", "Starfield.esm:00000998"]);
        comparison.Fields.Single(field => field.FieldName == "PreviewTransform").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000888", "Starfield.esm:00000888"]);
        comparison.Fields.Single(field => field.FieldName == "FeaturedItemMessage").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000777", "Starfield.esm:00000777"]);
        comparison.Fields.Single(field => field.FieldName == "Text").Values.Select(value => value.DisplayValue).ShouldBe(["Base text", "Patch text"]);
        comparison.Fields.Single(field => field.FieldName == "Teaches.MutagenObjectType").Values.Select(value => value.DisplayValue).ShouldBe(["Skill", "Skill"]);
        comparison.Fields.Single(field => field.FieldName == "Teaches.Perk").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000666", "Starfield.esm:00000666"]);
        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000101", "Starfield.esm:00000101"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\SetDressing\\Books\\Book01.nif", "Meshes\\SetDressing\\Books\\Book01.nif"]);
        var sounds = comparison.Fields.Single(field => field.FieldName == "Sounds");
        sounds.Children.Single(field => field.FieldName == "PickupSound").Children.Single(field => field.FieldName == "Start").Values.Select(value => value.DisplayValue).ShouldBe(["pickup", "pickup"]);
    }
}
