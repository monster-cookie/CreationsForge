using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Models.Database;

public class FormListItemTests
{
    [Fact]
    public void Constructor_MapsDTO()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var itemModKey = new ModKey("Item", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var itemFormKey = new FormKey(itemModKey, 456);
        var importedAtUTC = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
        var dto = new FormListItemDTO
        {
            ModKey = modKey,
            FormKey = formKey,
            ItemModKey = itemModKey,
            ItemFormKey = itemFormKey,
            ItemIndex = 7,
            ImportedAtUTC = importedAtUTC
        };

        var result = new FormListItem(dto);

        result.ModKeyName.ShouldBe(modKey.Name);
        result.ModKeyType.ShouldBe((int)modKey.Type);
        result.ModKeyFileName.ShouldBe(modKey.FileName);
        result.FormKeyID.ShouldBe((int)formKey.ID);
        result.ItemModKeyName.ShouldBe(itemModKey.Name);
        result.ItemModKeyType.ShouldBe((int)itemModKey.Type);
        result.ItemModKeyFileName.ShouldBe(itemModKey.FileName);
        result.ItemFormKeyID.ShouldBe((int)itemFormKey.ID);
        result.ItemIndex.ShouldBe(7);
        result.ImportedAtUTC.ShouldBe(importedAtUTC);
    }
}
