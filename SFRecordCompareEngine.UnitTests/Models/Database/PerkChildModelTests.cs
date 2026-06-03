using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Models.Database;

public class PerkChildModelTests
{
    [Fact]
    public void RankConstructor_MapsDTO()
    {
        var modKey = new ModKey("Example", ModType.Master);
        var formKey = new FormKey(new ModKey("Origin", ModType.Master), 200);
        var unknownStaticFormKey = new FormKey(new ModKey("Static", ModType.Master), 201);
        var importedAtUtc = new DateTime(2026, 6, 3, 9, 0, 0, DateTimeKind.Utc);
        var dto = new PerkRankDTO
        {
            ModKey = modKey,
            FormKey = formKey,
            RankIndex = 4,
            Description = "Description",
            UnknownStaticFormKey = unknownStaticFormKey,
            ConditionCount = 2,
            ActivityCount = 1,
            ImportedAtUTC = importedAtUtc
        };

        var result = new PerkRank(dto);

        result.ModKeyName.ShouldBe("Example");
        result.FormKeyId.ShouldBe(200);
        result.RankIndex.ShouldBe(4);
        result.Description.ShouldBe("Description");
        result.UnknownStaticModKeyName.ShouldBe(unknownStaticFormKey.ModKey.Name);
        result.UnknownStaticModKeyType.ShouldBe((int)unknownStaticFormKey.ModKey.Type);
        result.UnknownStaticModKeyFileName.ShouldBe(unknownStaticFormKey.ModKey.FileName);
        result.UnknownStaticFormKeyId.ShouldBe((int)unknownStaticFormKey.ID);
        result.ConditionCount.ShouldBe(2);
        result.ActivityCount.ShouldBe(1);
        result.ImportedAtUTC.ShouldBe(importedAtUtc);
    }

    [Fact]
    public void RankEffectConstructor_MapsDTO()
    {
        var dto = new PerkRankEffectDTO
        {
            ModKey = new ModKey("Example", ModType.Master),
            FormKey = new FormKey(new ModKey("Origin", ModType.Master), 300),
            RankIndex = 1,
            EffectIndex = 2,
            MutagenObjectType = "PerkEntryPointModifyValue",
            Rank = 3,
            Priority = 4,
            PerkEntryId = 5,
            Flags = "Flag",
            ButtonLabel = "Button",
            ConditionCount = 6,
            EntryPoint = "ModExp",
            PerkConditionTabCount = 7,
            Modification = "Multiply",
            Value = 1.5f,
            ImportedAtUTC = new DateTime(2026, 6, 3, 9, 5, 0, DateTimeKind.Utc)
        };

        var result = new PerkRankEffect(dto);

        result.FormKeyId.ShouldBe(300);
        result.RankIndex.ShouldBe(1);
        result.EffectIndex.ShouldBe(2);
        result.MutagenObjectType.ShouldBe("PerkEntryPointModifyValue");
        result.Rank.ShouldBe(3);
        result.Priority.ShouldBe(4);
        result.PerkEntryId.ShouldBe(5);
        result.Modification.ShouldBe("Multiply");
        result.Value.ShouldBe(1.5f);
    }

    [Fact]
    public void BackgroundSkillConstructor_MapsDTO()
    {
        var skillFormKey = new FormKey(new ModKey("Skill", ModType.Master), 401);
        var dto = new PerkBackgroundSkillDTO
        {
            ModKey = new ModKey("Example", ModType.Master),
            FormKey = new FormKey(new ModKey("Origin", ModType.Master), 400),
            SkillFormKey = skillFormKey,
            SkillIndex = 2,
            ImportedAtUTC = new DateTime(2026, 6, 3, 9, 10, 0, DateTimeKind.Utc)
        };

        var result = new PerkBackgroundSkill(dto);

        result.FormKeyId.ShouldBe(400);
        result.SkillModKeyName.ShouldBe("Skill");
        result.SkillFormKeyId.ShouldBe(401);
        result.SkillIndex.ShouldBe(2);
    }
}
