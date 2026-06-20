using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MagicEffect.Skyrim;

public class SkyrimMagicEffectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0D22FA:Skyrim.esm")]
    [Trait("EditorID", "ShockDamageMassConcAimed")]
    [Trait("SpriggitFile", "MagicEffects/ShockDamageMassConcAimed - 0D22FA_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSample_ShockDamageMassConcAimed()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MagicEffect,
            "ShockDamageMassConcAimed");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MagicEffect,
            "0D22FA:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.ActorValue"].ShouldBe(dtoFields["Archetype.ActorValue"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["Archetype.Type"].ShouldBe(dtoFields["Archetype.Type"]);
        spriggitFields["BaseCost"].ShouldBe(dtoFields["BaseCost"]);
        spriggitFields["CastingArt"].ShouldBe(dtoFields["CastingArt"]);
        spriggitFields["CastingLight"].ShouldBe(dtoFields["CastingLight"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["DualCastScale"].ShouldBe(dtoFields["DualCastScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["EquipAbility"].ShouldBe(dtoFields["EquipAbilityFormKey"]);
        spriggitFields["Explosion"].ShouldBe(dtoFields["Explosion"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["ImageSpaceModifier"].ShouldBe(dtoFields["ImageSpaceModifier"]);
        spriggitFields["ImpactData"].ShouldBe(dtoFields["ImpactData"]);
        spriggitFields["MagicSkill"].ShouldBe(dtoFields["MagicSkill"]);
        spriggitFields["MenuDisplayObject"].ShouldBe(dtoFields["MenuDisplayObject"]);
        spriggitFields["MinimumSkillLevel"].ShouldBe(dtoFields["MinimumSkillLevel"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Projectile"].ShouldBe(dtoFields["Projectile"]);
        spriggitFields["ResistValue"].ShouldBe(dtoFields["ResistValueFormKey"]);
        spriggitFields["SecondActorValue"].ShouldBe(dtoFields["SecondActorValue"]);
        spriggitFields["SecondActorValueWeight"].ShouldBe(dtoFields["SecondActorValueWeight"]);
        spriggitFields["Sound[0]"].ShouldBe(dtoFields["Sound[0]"]);
        spriggitFields["Sound[1]"].ShouldBe(dtoFields["Sound[1]"]);
        spriggitFields["Sound[2]"].ShouldBe(dtoFields["Sound[2]"]);
        spriggitFields["SpellmakingCastingTime"].ShouldBe(dtoFields["SpellmakingCastingTime"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Data"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Name"]);
        spriggitFields["VirtualMachineAdapter[0][1].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Object"]);
        spriggitFields["VirtualMachineAdapter[0][2].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Name"]);
        spriggitFields["VirtualMachineAdapter[0][2].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Object"]);
        spriggitFields["VirtualMachineAdapter[0][3].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Data"]);
        spriggitFields["VirtualMachineAdapter[0][3].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][3].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Name"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "1019D6:Skyrim.esm")]
    [Trait("EditorID", "dunVolunruudPickaxeEffect")]
    [Trait("SpriggitFile", "MagicEffects/dunVolunruudPickaxeEffect - 1019D6_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSample_dunVolunruudPickaxeEffect()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MagicEffect,
            "dunVolunruudPickaxeEffect");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MagicEffect,
            "1019D6:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.ActorValue"].ShouldBe(dtoFields["Archetype.ActorValue"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["Archetype.Type"].ShouldBe(dtoFields["Archetype.Type"]);
        spriggitFields["BaseCost"].ShouldBe(dtoFields["BaseCost"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["DualCastScale"].ShouldBe(dtoFields["DualCastScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["EnchantShader"].ShouldBe(dtoFields["EnchantShader"]);
        spriggitFields["EquipAbility"].ShouldBe(dtoFields["EquipAbilityFormKey"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["ImpactData"].ShouldBe(dtoFields["ImpactData"]);
        spriggitFields["MagicSkill"].ShouldBe(dtoFields["MagicSkill"]);
        spriggitFields["MenuDisplayObject"].ShouldBe(dtoFields["MenuDisplayObject"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["ResistValue"].ShouldBe(dtoFields["ResistValueFormKey"]);
        spriggitFields["SecondActorValue"].ShouldBe(dtoFields["SecondActorValue"]);
        spriggitFields["SecondActorValueWeight"].ShouldBe(dtoFields["SecondActorValueWeight"]);
        spriggitFields["Sound[0]"].ShouldBe(dtoFields["Sound[0]"]);
        spriggitFields["Sound[1]"].ShouldBe(dtoFields["Sound[1]"]);
        spriggitFields["Sound[2]"].ShouldBe(dtoFields["Sound[2]"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0CDB75:Skyrim.esm")]
    [Trait("EditorID", "ArmorFFSelf100")]
    [Trait("SpriggitFile", "MagicEffects/ArmorFFSelf100 - 0CDB75_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSample_ArmorFFSelf100()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MagicEffect,
            "ArmorFFSelf100");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MagicEffect,
            "0CDB75:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.ActorValue"].ShouldBe(dtoFields["Archetype.ActorValue"]);
        spriggitFields["Archetype.Association"].ShouldBe(dtoFields["Archetype.Association"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["BaseCost"].ShouldBe(dtoFields["BaseCost"]);
        spriggitFields["CastingArt"].ShouldBe(dtoFields["CastingArt"]);
        spriggitFields["CastingLight"].ShouldBe(dtoFields["CastingLight"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["DualCastScale"].ShouldBe(dtoFields["DualCastScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["EquipAbility"].ShouldBe(dtoFields["EquipAbilityFormKey"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["HitEffectArt"].ShouldBe(dtoFields["HitEffectArt"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["ImageSpaceModifier"].ShouldBe(dtoFields["ImageSpaceModifier"]);
        spriggitFields["MagicSkill"].ShouldBe(dtoFields["MagicSkill"]);
        spriggitFields["MenuDisplayObject"].ShouldBe(dtoFields["MenuDisplayObject"]);
        spriggitFields["MinimumSkillLevel"].ShouldBe(dtoFields["MinimumSkillLevel"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["PerkToApply"].ShouldBe(dtoFields["PerkToApplyFormKey"]);
        spriggitFields["SkillUsageMultiplier"].ShouldBe(dtoFields["SkillUsageMultiplier"]);
        spriggitFields["Sound[0]"].ShouldBe(dtoFields["Sound[0]"]);
        spriggitFields["Sound[1]"].ShouldBe(dtoFields["Sound[1]"]);
        spriggitFields["Sound[2]"].ShouldBe(dtoFields["Sound[2]"]);
        spriggitFields["SpellmakingCastingTime"].ShouldBe(dtoFields["SpellmakingCastingTime"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "09B246:Skyrim.esm")]
    [Trait("EditorID", "DA15WabbajackFF")]
    [Trait("SpriggitFile", "MagicEffects/DA15WabbajackFF - 09B246_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSample_DA15WabbajackFF()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MagicEffect,
            "DA15WabbajackFF");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MagicEffect,
            "09B246:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.ActorValue"].ShouldBe(dtoFields["Archetype.ActorValue"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["BaseCost"].ShouldBe(dtoFields["BaseCost"]);
        spriggitFields["CastingArt"].ShouldBe(dtoFields["CastingArt"]);
        spriggitFields["CastingLight"].ShouldBe(dtoFields["CastingLight"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["DualCastScale"].ShouldBe(dtoFields["DualCastScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["ImpactData"].ShouldBe(dtoFields["ImpactData"]);
        spriggitFields["MagicSkill"].ShouldBe(dtoFields["MagicSkill"]);
        spriggitFields["MenuDisplayObject"].ShouldBe(dtoFields["MenuDisplayObject"]);
        spriggitFields["MinimumSkillLevel"].ShouldBe(dtoFields["MinimumSkillLevel"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Projectile"].ShouldBe(dtoFields["Projectile"]);
        spriggitFields["ResistValue"].ShouldBe(dtoFields["ResistValueFormKey"]);
        spriggitFields["SkillUsageMultiplier"].ShouldBe(dtoFields["SkillUsageMultiplier"]);
        spriggitFields["Sound[0]"].ShouldBe(dtoFields["Sound[0]"]);
        spriggitFields["Sound[1]"].ShouldBe(dtoFields["Sound[1]"]);
        spriggitFields["Sound[2]"].ShouldBe(dtoFields["Sound[2]"]);
        spriggitFields["SpellmakingCastingTime"].ShouldBe(dtoFields["SpellmakingCastingTime"]);
        spriggitFields["TaperWeight"].ShouldBe(dtoFields["TaperWeight"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["VirtualMachineAdapter[0][1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Name"]);
        spriggitFields["VirtualMachineAdapter[0][1].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Object"]);
        spriggitFields["VirtualMachineAdapter[0][10].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][10].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][10].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][10].Name"]);
        spriggitFields["VirtualMachineAdapter[0][10].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][10].Object"]);
        spriggitFields["VirtualMachineAdapter[0][11].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][11].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][11].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][11].Name"]);
        spriggitFields["VirtualMachineAdapter[0][11].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][11].Object"]);
        spriggitFields["VirtualMachineAdapter[0][12].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][12].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][12].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][12].Name"]);
        spriggitFields["VirtualMachineAdapter[0][12].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][12].Object"]);
        spriggitFields["VirtualMachineAdapter[0][13].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][13].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][13].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][13].Name"]);
        spriggitFields["VirtualMachineAdapter[0][13].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][13].Object"]);
        spriggitFields["VirtualMachineAdapter[0][14].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][14].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][14].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][14].Name"]);
        spriggitFields["VirtualMachineAdapter[0][14].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][14].Object"]);
        spriggitFields["VirtualMachineAdapter[0][15].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][15].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][15].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][15].Name"]);
        spriggitFields["VirtualMachineAdapter[0][15].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][15].Object"]);
        spriggitFields["VirtualMachineAdapter[0][16].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][16].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][16].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][16].Name"]);
        spriggitFields["VirtualMachineAdapter[0][16].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][16].Object"]);
        spriggitFields["VirtualMachineAdapter[0][17].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][17].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][17].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][17].Name"]);
        spriggitFields["VirtualMachineAdapter[0][17].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][17].Object"]);
        spriggitFields["VirtualMachineAdapter[0][18].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][18].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][18].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][18].Name"]);
        spriggitFields["VirtualMachineAdapter[0][18].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][18].Object"]);
        spriggitFields["VirtualMachineAdapter[0][19].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][19].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][19].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][19].Name"]);
        spriggitFields["VirtualMachineAdapter[0][19].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][19].Object"]);
        spriggitFields["VirtualMachineAdapter[0][2].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Name"]);
        spriggitFields["VirtualMachineAdapter[0][2].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Object"]);
        spriggitFields["VirtualMachineAdapter[0][20].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][20].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][20].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][20].Name"]);
        spriggitFields["VirtualMachineAdapter[0][20].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][20].Object"]);
        spriggitFields["VirtualMachineAdapter[0][21].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][21].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][21].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][21].Name"]);
        spriggitFields["VirtualMachineAdapter[0][21].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][21].Object"]);
        spriggitFields["VirtualMachineAdapter[0][22].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][22].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][22].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][22].Name"]);
        spriggitFields["VirtualMachineAdapter[0][22].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][22].Object"]);
        spriggitFields["VirtualMachineAdapter[0][23].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][23].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][23].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][23].Name"]);
        spriggitFields["VirtualMachineAdapter[0][23].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][23].Object"]);
        spriggitFields["VirtualMachineAdapter[0][24].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][24].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][24].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][24].Name"]);
        spriggitFields["VirtualMachineAdapter[0][24].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][24].Object"]);
        spriggitFields["VirtualMachineAdapter[0][25].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][25].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][25].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][25].Name"]);
        spriggitFields["VirtualMachineAdapter[0][25].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][25].Object"]);
        spriggitFields["VirtualMachineAdapter[0][26].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][26].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][26].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][26].Name"]);
        spriggitFields["VirtualMachineAdapter[0][26].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][26].Object"]);
        spriggitFields["VirtualMachineAdapter[0][27].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][27].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][27].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][27].Name"]);
        spriggitFields["VirtualMachineAdapter[0][27].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][27].Object"]);
        spriggitFields["VirtualMachineAdapter[0][28].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][28].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][28].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][28].Name"]);
        spriggitFields["VirtualMachineAdapter[0][28].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][28].Object"]);
        spriggitFields["VirtualMachineAdapter[0][29].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][29].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][29].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][29].Name"]);
        spriggitFields["VirtualMachineAdapter[0][29].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][29].Object"]);
        spriggitFields["VirtualMachineAdapter[0][3].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][3].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Name"]);
        spriggitFields["VirtualMachineAdapter[0][3].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Object"]);
        spriggitFields["VirtualMachineAdapter[0][30].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][30].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][30].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][30].Name"]);
        spriggitFields["VirtualMachineAdapter[0][30].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][30].Object"]);
        spriggitFields["VirtualMachineAdapter[0][4].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][4].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].Name"]);
        spriggitFields["VirtualMachineAdapter[0][4].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][5].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].Object"]);
        spriggitFields["VirtualMachineAdapter[0][6].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][6].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].Name"]);
        spriggitFields["VirtualMachineAdapter[0][6].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].Object"]);
        spriggitFields["VirtualMachineAdapter[0][7].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][7].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][7].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][7].Name"]);
        spriggitFields["VirtualMachineAdapter[0][7].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][7].Object"]);
        spriggitFields["VirtualMachineAdapter[0][8].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][8].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][8].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][8].Name"]);
        spriggitFields["VirtualMachineAdapter[0][8].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][8].Object"]);
        spriggitFields["VirtualMachineAdapter[0][9].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][9].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][9].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][9].Name"]);
        spriggitFields["VirtualMachineAdapter[0][9].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][9].Object"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0FB406:Skyrim.esm")]
    [Trait("EditorID", "dunHalldirAggDownFFAimedArea")]
    [Trait("SpriggitFile", "MagicEffects/dunHalldirAggDownFFAimedArea - 0FB406_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ShouldMatchSpriggitSample_dunHalldirAggDownFFAimedArea()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MagicEffect,
            "dunHalldirAggDownFFAimedArea");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MagicEffect,
            "0FB406:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.ActorValue"].ShouldBe(dtoFields["Archetype.ActorValue"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["Archetype.Type"].ShouldBe(dtoFields["Archetype.Type"]);
        spriggitFields["BaseCost"].ShouldBe(dtoFields["BaseCost"]);
        spriggitFields["CastingArt"].ShouldBe(dtoFields["CastingArt"]);
        spriggitFields["CastingLight"].ShouldBe(dtoFields["CastingLight"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["CompareOperator[0]"].ShouldBe(dtoFields["CompareOperator[0]"]);
        spriggitFields["CompareOperator[1]"].ShouldBe(dtoFields["CompareOperator[1]"]);
        spriggitFields["CompareOperator[2]"].ShouldBe(dtoFields["CompareOperator[2]"]);
        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["ComparisonValue[2]"].ShouldBe(dtoFields["ComparisonValue[2]"]);
        spriggitFields["Data.Keyword[0]"].ShouldBe(dtoFields["Data.Keyword[0]"]);
        spriggitFields["Data.Keyword[1]"].ShouldBe(dtoFields["Data.Keyword[1]"]);
        spriggitFields["Data.Keyword[2]"].ShouldBe(dtoFields["Data.Keyword[2]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.MutagenObjectType[4]"].ShouldBe(dtoFields["Data.MutagenObjectType[4]"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["DualCastScale"].ShouldBe(dtoFields["DualCastScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["Explosion"].ShouldBe(dtoFields["Explosion"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["ImpactData"].ShouldBe(dtoFields["ImpactData"]);
        spriggitFields["MagicSkill"].ShouldBe(dtoFields["MagicSkill"]);
        spriggitFields["MenuDisplayObject"].ShouldBe(dtoFields["MenuDisplayObject"]);
        spriggitFields["MinimumSkillLevel"].ShouldBe(dtoFields["MinimumSkillLevel"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Projectile"].ShouldBe(dtoFields["Projectile"]);
        spriggitFields["SkillUsageMultiplier"].ShouldBe(dtoFields["SkillUsageMultiplier"]);
        spriggitFields["Sound[0]"].ShouldBe(dtoFields["Sound[0]"]);
        spriggitFields["Sound[1]"].ShouldBe(dtoFields["Sound[1]"]);
        spriggitFields["Sound[2]"].ShouldBe(dtoFields["Sound[2]"]);
        spriggitFields["Sound[3]"].ShouldBe(dtoFields["Sound[3]"]);
        spriggitFields["SpellmakingArea"].ShouldBe(dtoFields["SpellmakingArea"]);
        spriggitFields["SpellmakingCastingTime"].ShouldBe(dtoFields["SpellmakingCastingTime"]);
        spriggitFields["TaperWeight"].ShouldBe(dtoFields["TaperWeight"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["VirtualMachineAdapter[0][1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Name"]);
        spriggitFields["VirtualMachineAdapter[0][1].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Object"]);
        spriggitFields["VirtualMachineAdapter[0][2].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Name"]);
        spriggitFields["VirtualMachineAdapter[0][2].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Object"]);
        spriggitFields["VirtualMachineAdapter[0][3].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][3].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Name"]);
        spriggitFields["VirtualMachineAdapter[0][3].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Object"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
