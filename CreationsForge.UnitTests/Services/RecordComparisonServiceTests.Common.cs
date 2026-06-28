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
/// Contains service-wide record comparison scenarios that are not specific to one record type.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that record comparison single column keeps values neutral.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForSingleColumn_KeepsValuesNeutral()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x321);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5)
            ]
        };
        var service = CreateService(globalRepository: globalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Data").State.ShouldBe(RecordComparisonValueState.Neutral);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Single().State.ShouldBe(RecordComparisonValueState.Neutral);
    }

    /// <summary>
    /// Verifies that record comparison non comparable common fields keeps values neutral.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForNonComparableCommonFields_KeepsValuesNeutral()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x654);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5),
                CreateGlobal("Patch.esp", formKey, "MyGlobal", 2.5)
            ]
        };
        var service = CreateService(globalRepository: globalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "FormVersion").State.ShouldBe(RecordComparisonValueState.Neutral);
        comparison.Fields.Single(field => field.FieldName == "MajorRecordFlags").State.ShouldBe(RecordComparisonValueState.Neutral);
    }

    /// <summary>
    /// Verifies that record comparison unsupported record type returns empty comparison.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForUnsupportedRecordType_ReturnsEmptyComparison()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x999);
        var service = CreateService();

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, "ARMO", formKey);

        comparison.RecordType.ShouldBe("ARMO");
        comparison.FormKey.ShouldBeSameAs(formKey);
        comparison.Columns.ShouldBeEmpty();
        comparison.Fields.ShouldBeEmpty();
    }
}
