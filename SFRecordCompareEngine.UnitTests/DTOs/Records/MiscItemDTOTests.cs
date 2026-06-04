using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;
using Shouldly;
using MiscItem = SFRecordCompareEngine.Core.Models.Database.MiscItem;

namespace SFRecordCompareEngine.UnitTests.DTOs.Records;

public class MiscItemDTOTests
{
    [Fact]
    public void Constructor_MapsParentModel()
    {
        var model = new MiscItem
        {
            ModKeyName = "Example",
            ModKeyType = (int)ModType.Master,
            FormKeyModKeyName = "Origin",
            FormKeyModKeyType = (int)ModType.Master,
            FormKeyId = 123,
            EditorId = "ExampleMiscItem",
            FormVersion = 581,
            StarfieldMajorRecordFlags = (int)StarfieldMajorRecord.StarfieldMajorRecordFlag.NotPlayable,
            Version2 = 2,
            VersionControl = 3,
            ImportedAtUTC = DateTime.UtcNow,
            Name = "Name",
            ShortName = "Short",
            Value = 10,
            Weight = 1.5,
            DirtinessScale = 0.25f,
            FeaturedItemMessageModKeyName = "Message",
            FeaturedItemMessageModKeyType = (int)ModType.Master,
            FeaturedItemMessageFormKeyId = 456,
            Flag = "01000000"
        };

        var result = new MiscItemDTO(model);

        result.FormKey.ShouldBe(new FormKey(new ModKey("Origin", ModType.Master), 123));
        result.Name.ShouldBe("Name");
        result.DirtinessScale.ShouldBe(0.25f);
        result.FeaturedItemMessageFormKey.ShouldBe(new FormKey(new ModKey("Message", ModType.Master), 456));
        result.Flag.ShouldBe("01000000");
    }
}
