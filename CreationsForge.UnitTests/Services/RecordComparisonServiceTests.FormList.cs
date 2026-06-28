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
/// Contains record comparison scenarios for Form List records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that record comparison form list expands item slots.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForFormList_ExpandsItemSlots()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x789);
        var firstItem = CreateFormKey("Starfield.esm", 0x111);
        var secondItem = CreateFormKey("Starfield.esm", 0x222);
        var formListRepository = new TestFormListRepository
        {
            Records =
            [
                CreateFormList("Base.esm", formKey, [CreateFormListItem("Base.esm", formKey, firstItem, 0)]),
                CreateFormList("Patch.esp", formKey, [CreateFormListItem("Patch.esp", formKey, firstItem, 0), CreateFormListItem("Patch.esp", formKey, secondItem, 1)])
            ]
        };
        var service = CreateService(formListRepository: formListRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.FormList.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "Items[0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000111", "Starfield.esm:00000111"]);
        comparison.Fields.Single(field => field.FieldName == "Items[0]").State.ShouldBe(RecordComparisonValueState.Identical);
        comparison.Fields.Single(field => field.FieldName == "Items[0]").Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Identical, RecordComparisonValueState.Identical]);
        comparison.Fields.Single(field => field.FieldName == "Items[1]").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:00000222"]);
        comparison.Fields.Single(field => field.FieldName == "Items[1]").State.ShouldBe(RecordComparisonValueState.Conflict);
    }
}
