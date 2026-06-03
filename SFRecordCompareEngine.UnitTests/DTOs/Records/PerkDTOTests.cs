using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records;
using Shouldly;
using Perk = SFRecordCompareEngine.Core.Models.Database.Perk;

namespace SFRecordCompareEngine.UnitTests.DTOs.Records;

public class PerkDTOTests
{
    [Fact]
    public void Constructor_MapsModel()
    {
        var importedAtUtc = new DateTime(2026, 6, 3, 8, 15, 0, DateTimeKind.Utc);
        var restrictionFormKey = new FormKey(new ModKey("Restrictions", ModType.Master), 123);
        var trainingFormKey = new FormKey(new ModKey("Training", ModType.Master), 456);
        var model = new Perk
        {
            ModKeyName = "Example",
            ModKeyType = (int)ModType.Master,
            ModKeyFileName = "Example.esm",
            FormKeyModKeyName = "Origin",
            FormKeyModKeyType = (int)ModType.Master,
            FormKeyModKeyFileName = "Origin.esm",
            FormKeyId = 789,
            EditorId = "ExamplePerk",
            FormVersion = 581,
            StarfieldMajorRecordFlags = (int)StarfieldMajorRecord.StarfieldMajorRecordFlag.NotPlayable,
            Version2 = 12,
            VersionControl = 12345,
            ImportedAtUTC = importedAtUtc,
            Name = "Example Name",
            Description = "Example Description",
            Flags = "PcPlayable",
            SkillGroup = "Basic",
            CrewAssignment = "Allowed",
            PerkIcon = "Icon",
            Category = "Combat",
            RestrictionModKeyName = restrictionFormKey.ModKey.Name,
            RestrictionModKeyType = (int)restrictionFormKey.ModKey.Type,
            RestrictionModKeyFileName = restrictionFormKey.ModKey.FileName,
            RestrictionFormKeyId = (int)restrictionFormKey.ID,
            TrainingModKeyName = trainingFormKey.ModKey.Name,
            TrainingModKeyType = (int)trainingFormKey.ModKey.Type,
            TrainingModKeyFileName = trainingFormKey.ModKey.FileName,
            TrainingFormKeyId = (int)trainingFormKey.ID,
            MajorFlags = "NonPlayable"
        };

        var result = new PerkDTO(model);

        result.ModKey.Name.ShouldBe("Example");
        result.FormKey.ModKey.Name.ShouldBe("Origin");
        result.FormKey.ID.ShouldBe(789U);
        result.EditorID.ShouldBe("ExamplePerk");
        result.StarfieldMajorRecordFlags.ShouldBe(StarfieldMajorRecord.StarfieldMajorRecordFlag.NotPlayable);
        result.Category.ShouldBe("Combat");
        result.RestrictionFormKey.ShouldBe(restrictionFormKey);
        result.TrainingFormKey.ShouldBe(trainingFormKey);
        result.MajorFlags.ShouldBe("NonPlayable");
    }

    [Fact]
    public void Constructor_LeavesNullableReferenceTuplesNull()
    {
        var model = new Perk
        {
            ModKeyName = "Example",
            ModKeyType = (int)ModType.Master,
            ModKeyFileName = "Example.esm",
            FormKeyModKeyName = "Origin",
            FormKeyModKeyType = (int)ModType.Master,
            FormKeyModKeyFileName = "Origin.esm",
            FormKeyId = 789,
            EditorId = "ExamplePerk",
            FormVersion = 581,
            StarfieldMajorRecordFlags = 0,
            Version2 = 12,
            VersionControl = 12345,
            ImportedAtUTC = new DateTime(2026, 6, 3, 8, 15, 0, DateTimeKind.Utc),
            Flags = string.Empty
        };

        var result = new PerkDTO(model);

        result.RestrictionFormKey.ShouldBeNull();
        result.TrainingFormKey.ShouldBeNull();
    }
}
