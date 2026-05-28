using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.DTOs.Records;

public class FormListItemDTOTests
{
    [Fact]
    public void Constructor_MapsModel()
    {
        var importedAtUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var model = new FormListItem
        {
            ModKeyName = "Example",
            ModKeyType = (int)ModType.Master,
            ModKeyFileName = "Example.esm",
            FormKeyID = 123,
            ItemModKeyName = "Item",
            ItemModKeyType = (int)ModType.Master,
            ItemModKeyFileName = "Item.esm",
            ItemFormKeyID = 456,
            ImportedAtUTC = importedAtUTC
        };

        var result = new FormListItemDTO(model);

        result.ModKey.Name.ShouldBe("Example");
        result.ModKey.Type.ShouldBe(ModType.Master);
        result.FormKey.ID.ShouldBe(123U);
        result.ItemModKey.Name.ShouldBe("Item");
        result.ItemModKey.Type.ShouldBe(ModType.Master);
        result.ItemFormKey.ID.ShouldBe(456U);
        result.ImportedAtUTC.ShouldBe(importedAtUTC);
    }
}
