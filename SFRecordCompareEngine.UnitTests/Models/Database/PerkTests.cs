using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;
using Shouldly;
using Perk = SFRecordCompareEngine.Core.Models.Database.Perk;

namespace SFRecordCompareEngine.UnitTests.Models.Database;

public class PerkTests
{
    [Fact]
    public void Constructor_MapsDTO()
    {
        var restrictionFormKey = new FormKey(new ModKey("Restriction", ModType.Master), 501);
        var trainingFormKey = new FormKey(new ModKey("Training", ModType.Master), 502);
        var dto = new PerkDTO
        {
            ModKey = new ModKey("Example", ModType.Master),
            FormKey = new FormKey(new ModKey("Origin", ModType.Master), 500),
            EditorID = "ExamplePerk",
            FormVersion = 581,
            StarfieldMajorRecordFlags = StarfieldMajorRecord.StarfieldMajorRecordFlag.NotPlayable,
            Version2 = 12,
            VersionControl = 12345,
            ImportedAtUTC = new DateTime(2026, 6, 3, 9, 15, 0, DateTimeKind.Utc),
            Name = "Name",
            Description = "Description",
            Flags = "PcPlayable",
            SkillGroup = "Basic",
            CrewAssignment = "Allowed",
            PerkIcon = "Icon",
            Category = "Combat",
            RestrictionFormKey = restrictionFormKey,
            TrainingFormKey = trainingFormKey,
            MajorFlags = "NonPlayable"
        };

        var result = new Perk(dto);

        result.ModKeyName.ShouldBe("Example");
        result.FormKeyId.ShouldBe(500);
        result.EditorId.ShouldBe("ExamplePerk");
        result.StarfieldMajorRecordFlags.ShouldBe((int)StarfieldMajorRecord.StarfieldMajorRecordFlag.NotPlayable);
        result.Category.ShouldBe("Combat");
        result.RestrictionModKeyName.ShouldBe(restrictionFormKey.ModKey.Name);
        result.RestrictionModKeyType.ShouldBe((int)restrictionFormKey.ModKey.Type);
        result.RestrictionModKeyFileName.ShouldBe(restrictionFormKey.ModKey.FileName);
        result.RestrictionFormKeyId.ShouldBe((int)restrictionFormKey.ID);
        result.TrainingModKeyName.ShouldBe(trainingFormKey.ModKey.Name);
        result.TrainingModKeyType.ShouldBe((int)trainingFormKey.ModKey.Type);
        result.TrainingModKeyFileName.ShouldBe(trainingFormKey.ModKey.FileName);
        result.TrainingFormKeyId.ShouldBe((int)trainingFormKey.ID);
        result.MajorFlags.ShouldBe("NonPlayable");
    }
}
