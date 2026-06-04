using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Models.Database;

public class MiscItemChildModelTests
{
    [Fact]
    public void ObjectBoundsConstructor_MapsDTO()
    {
        var dto = CreateParent();
        dto.ObjectBounds = new MiscItemObjectBoundsDTO { FirstX = 1, FirstY = 2, FirstZ = 3, SecondX = 4, SecondY = 5, SecondZ = 6 };

        var result = new MiscItemObjectBounds(dto);

        result.FormKeyId.ShouldBe(123);
        result.FirstX.ShouldBe(1);
        result.SecondZ.ShouldBe(6);
    }

    private static MiscItemDTO CreateParent()
    {
        return new MiscItemDTO
        {
            ModKey = new ModKey("Example", ModType.Master),
            FormKey = new FormKey(new ModKey("Origin", ModType.Master), 123),
            EditorID = "ExampleMiscItem",
            FormVersion = 581,
            StarfieldMajorRecordFlags = StarfieldMajorRecord.StarfieldMajorRecordFlag.NotPlayable,
            Version2 = 2,
            VersionControl = 3,
            ImportedAtUTC = DateTime.UtcNow
        };
    }
}
