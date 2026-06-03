using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.DTOs.Records;

public class PerkChildDTOTests
{
    [Fact]
    public void RankConstructor_MapsModel()
    {
        var importedAtUtc = new DateTime(2026, 6, 3, 8, 20, 0, DateTimeKind.Utc);
        var unknownStaticFormKey = new FormKey(new ModKey("Static", ModType.Master), 12);
        var model = new PerkRank
        {
            ModKeyName = "Example",
            ModKeyType = (int)ModType.Master,
            ModKeyFileName = "Example.esm",
            FormKeyModKeyName = "Origin",
            FormKeyModKeyType = (int)ModType.Master,
            FormKeyModKeyFileName = "Origin.esm",
            FormKeyId = 100,
            RankIndex = 2,
            Description = "Rank description",
            UnknownStaticModKeyName = unknownStaticFormKey.ModKey.Name,
            UnknownStaticModKeyType = (int)unknownStaticFormKey.ModKey.Type,
            UnknownStaticModKeyFileName = unknownStaticFormKey.ModKey.FileName,
            UnknownStaticFormKeyId = (int)unknownStaticFormKey.ID,
            ConditionCount = 3,
            ActivityCount = 4,
            ImportedAtUTC = importedAtUtc
        };

        var result = new PerkRankDTO(model);

        result.ModKey.Name.ShouldBe("Example");
        result.FormKey.ID.ShouldBe(100U);
        result.RankIndex.ShouldBe(2);
        result.Description.ShouldBe("Rank description");
        result.UnknownStaticFormKey.ShouldBe(unknownStaticFormKey);
        result.ConditionCount.ShouldBe(3);
        result.ActivityCount.ShouldBe(4);
        result.ImportedAtUTC.ShouldBe(importedAtUtc);
    }

    [Fact]
    public void RankConstructor_LeavesNullableReferenceTupleNull()
    {
        var model = new PerkRank
        {
            ModKeyName = "Example",
            ModKeyType = (int)ModType.Master,
            ModKeyFileName = "Example.esm",
            FormKeyModKeyName = "Origin",
            FormKeyModKeyType = (int)ModType.Master,
            FormKeyModKeyFileName = "Origin.esm",
            FormKeyId = 100,
            RankIndex = 2,
            ConditionCount = 3,
            ActivityCount = 4,
            ImportedAtUTC = new DateTime(2026, 6, 3, 8, 20, 0, DateTimeKind.Utc)
        };

        var result = new PerkRankDTO(model);

        result.UnknownStaticFormKey.ShouldBeNull();
    }

    [Fact]
    public void RankEffectConstructor_MapsModel()
    {
        var importedAtUtc = new DateTime(2026, 6, 3, 8, 25, 0, DateTimeKind.Utc);
        var model = new PerkRankEffect
        {
            ModKeyName = "Example",
            ModKeyType = (int)ModType.Master,
            ModKeyFileName = "Example.esm",
            FormKeyModKeyName = "Origin",
            FormKeyModKeyType = (int)ModType.Master,
            FormKeyModKeyFileName = "Origin.esm",
            FormKeyId = 101,
            RankIndex = 1,
            EffectIndex = 5,
            MutagenObjectType = "PerkEntryPointModifyValue",
            Rank = 3,
            Priority = 7,
            PerkEntryId = 9,
            Flags = "RunImmediately",
            ButtonLabel = "Button",
            ConditionCount = 2,
            EntryPoint = "ModExp",
            PerkConditionTabCount = 4,
            Modification = "Multiply",
            Value = 1.25f,
            ImportedAtUTC = importedAtUtc
        };

        var result = new PerkRankEffectDTO(model);

        result.FormKey.ID.ShouldBe(101U);
        result.RankIndex.ShouldBe(1);
        result.EffectIndex.ShouldBe(5);
        result.MutagenObjectType.ShouldBe("PerkEntryPointModifyValue");
        result.Rank.ShouldBe(3);
        result.Priority.ShouldBe(7);
        result.PerkEntryId.ShouldBe(9);
        result.EntryPoint.ShouldBe("ModExp");
        result.Modification.ShouldBe("Multiply");
        result.Value.ShouldBe(1.25f);
        result.ImportedAtUTC.ShouldBe(importedAtUtc);
    }

    [Fact]
    public void BackgroundSkillConstructor_MapsModel()
    {
        var importedAtUtc = new DateTime(2026, 6, 3, 8, 30, 0, DateTimeKind.Utc);
        var model = new PerkBackgroundSkill
        {
            ModKeyName = "Example",
            ModKeyType = (int)ModType.Master,
            ModKeyFileName = "Example.esm",
            FormKeyModKeyName = "Origin",
            FormKeyModKeyType = (int)ModType.Master,
            FormKeyModKeyFileName = "Origin.esm",
            FormKeyId = 102,
            SkillModKeyName = "Skill",
            SkillModKeyType = (int)ModType.Master,
            SkillModKeyFileName = "Skill.esm",
            SkillFormKeyId = 103,
            SkillIndex = 1,
            ImportedAtUTC = importedAtUtc
        };

        var result = new PerkBackgroundSkillDTO(model);

        result.FormKey.ID.ShouldBe(102U);
        result.SkillFormKey.ModKey.Name.ShouldBe("Skill");
        result.SkillFormKey.ID.ShouldBe(103U);
        result.SkillIndex.ShouldBe(1);
        result.ImportedAtUTC.ShouldBe(importedAtUtc);
    }
}
