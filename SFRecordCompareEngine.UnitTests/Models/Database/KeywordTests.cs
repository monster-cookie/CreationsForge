using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;
using Shouldly;
using Keyword = SFRecordCompareEngine.Core.Models.Database.Keyword;

namespace SFRecordCompareEngine.UnitTests.Models.Database;

public class KeywordTests
{
    [Fact]
    public void Constructor_MapsDTO()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(modKey, 123);
        var attractionRuleFormKey = new FormKey(modKey, 456);
        var dto = new KeywordDTO
        {
            ModKey = modKey,
            FormKey = formKey,
            EditorID = "Editor",
            FormVersion = 44,
            StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)1,
            Version2 = 2,
            VersionControl = 3,
            ImportedAtUTC = DateTime.UtcNow,
            Name = "Name",
            Color = "#00FFFFFF",
            Type = "Attraction",
            Notes = "Notes",
            FlashLinkageName = "Linkage",
            AttractionRuleFormKey = attractionRuleFormKey
        };

        var result = new Keyword(dto);

        result.Name.ShouldBe("Name");
        result.Color.ShouldBe("#00FFFFFF");
        result.Type.ShouldBe("Attraction");
        result.Notes.ShouldBe("Notes");
        result.FlashLinkageName.ShouldBe("Linkage");
        result.AttractionRuleFormKey.ShouldBe(attractionRuleFormKey.ToString());
    }
}