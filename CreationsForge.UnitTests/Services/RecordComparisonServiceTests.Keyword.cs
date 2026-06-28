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
/// Contains record comparison scenarios for Keyword records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that Keyword scalar rows are selected from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForKeyword_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x125);
        var keywordRepository = new TestKeywordRepository
        {
            Records =
            [
                CreateKeyword("Base.esm", formKey, "BaseType", "Blue"),
                CreateKeyword("Patch.esp", formKey, "PatchType", "Red")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Keyword.RecordID,
                RecordType = SupportedRecordSpecifications.Keyword.RecordType,
                TableName = SupportedRecordSpecifications.Keyword.TableName,
                FriendlyName = SupportedRecordSpecifications.Keyword.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Keyword.GameSupport,
                Fields = SupportedRecordSpecifications.Keyword.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Type",
                            SourcePath = "Type",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(keywordRepository: keywordRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Keyword.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Type").Values.Select(value => value.DisplayValue)
            .ShouldBe(["BaseType", "PatchType"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Color");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
    }

    /// <summary>
    /// Verifies that specification-declared localized Keyword rows use the selected record text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForKeyword_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x127);
        var keywordRepository = new TestKeywordRepository
        {
            Records =
            [
                CreateKeyword("Base.esm", formKey, "BaseType", "Blue"),
                CreateKeyword("Patch.esp", formKey, "PatchType", "Red")
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Schluesselwort"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Schluesselwort")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            keywordRepository: keywordRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Keyword.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Schluesselwort", "Patch Schluesselwort"]);
    }
}
