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
/// Contains record comparison scenarios for Global records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that record comparison global creates plugin columns and data rows.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForGlobal_CreatesPluginColumnsAndDataRows()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x123);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5, "GlobalShort", "Constant"),
                CreateGlobal("Patch.esp", formKey, "MyGlobal", 2.5, "GlobalFloat", "Constant")
            ]
        };
        var service = CreateService(globalRepository: globalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.Global.RecordID);
        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "MutagenObjectType").Values.Select(value => value.DisplayValue).ShouldBe(["GlobalShort", "GlobalFloat"]);
        comparison.Fields.Single(field => field.FieldName == "MajorFlags").Values.Select(value => value.DisplayValue).ShouldBe(["Constant", "Constant"]);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["1.5", "2.5"]);
        comparison.Fields.Single(field => field.FieldName == "Data").State.ShouldBe(RecordComparisonValueState.Conflict);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
    }

    /// <summary>
    /// Verifies that the Global pilot path reads type-specific rows from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForGlobal_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x124);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5, "GlobalShort", "Constant")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Global.RecordID,
                RecordType = SupportedRecordSpecifications.Global.RecordType,
                TableName = SupportedRecordSpecifications.Global.TableName,
                FriendlyName = SupportedRecordSpecifications.Global.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Global.GameSupport,
                Fields = SupportedRecordSpecifications.Global.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Data",
                            SourcePath = "Data",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(globalRepository: globalRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.Fields.ShouldContain(field => field.FieldName == "Data");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "MutagenObjectType");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "MajorFlags");
    }
}
