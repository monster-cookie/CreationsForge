using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;
using Shouldly;
using MiscItem = SFRecordCompareEngine.Core.Models.Database.MiscItem;

namespace SFRecordCompareEngine.UnitTests.Models.Database;

public class MiscItemTests
{
    [Fact]
    public void Constructor_MapsParentDTO()
    {
        var featuredItemMessage = new FormKey(new ModKey("Message", ModType.Master), 456);
        var dto = new MiscItemDTO
        {
            ModKey = new ModKey("Example", ModType.Master),
            FormKey = new FormKey(new ModKey("Origin", ModType.Master), 123),
            EditorID = "ExampleMiscItem",
            FormVersion = 581,
            StarfieldMajorRecordFlags = StarfieldMajorRecord.StarfieldMajorRecordFlag.NotPlayable,
            Version2 = 2,
            VersionControl = 3,
            ImportedAtUTC = DateTime.UtcNow,
            DirtinessScale = 0.5f,
            FeaturedItemMessageFormKey = featuredItemMessage,
            Flag = "01000000"
        };

        var result = new MiscItem(dto);

        result.DirtinessScale.ShouldBe(0.5f);
        result.FeaturedItemMessageModKeyName.ShouldBe("Message");
        result.FeaturedItemMessageFormKeyId.ShouldBe(456);
        result.Flag.ShouldBe("01000000");
    }
}
