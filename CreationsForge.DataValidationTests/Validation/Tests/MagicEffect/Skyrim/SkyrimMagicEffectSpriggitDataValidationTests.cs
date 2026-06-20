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

        Helpers.GetSpriggitField(spriggit, "Archetype.ActorValue").ShouldBe(Helpers.GetDTOField(dto, "Archetype.ActorValue"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Archetype.Type").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Type"));
        Helpers.GetSpriggitField(spriggit, "BaseCost").ShouldBe(Helpers.GetDTOField(dto, "BaseCost"));
        Helpers.GetSpriggitField(spriggit, "CastingArt").ShouldBe(Helpers.GetDTOField(dto, "CastingArt"));
        Helpers.GetSpriggitField(spriggit, "CastingLight").ShouldBe(Helpers.GetDTOField(dto, "CastingLight"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "DualCastScale").ShouldBe(Helpers.GetDTOField(dto, "DualCastScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "EquipAbility").ShouldBe(Helpers.GetDTOField(dto, "EquipAbilityFormKey"));
        Helpers.GetSpriggitField(spriggit, "Explosion").ShouldBe(Helpers.GetDTOField(dto, "Explosion"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "ImageSpaceModifier").ShouldBe(Helpers.GetDTOField(dto, "ImageSpaceModifier"));
        Helpers.GetSpriggitField(spriggit, "ImpactData").ShouldBe(Helpers.GetDTOField(dto, "ImpactData"));
        Helpers.GetSpriggitField(spriggit, "MagicSkill").ShouldBe(Helpers.GetDTOField(dto, "MagicSkill"));
        Helpers.GetSpriggitField(spriggit, "MenuDisplayObject").ShouldBe(Helpers.GetDTOField(dto, "MenuDisplayObject"));
        Helpers.GetSpriggitField(spriggit, "MinimumSkillLevel").ShouldBe(Helpers.GetDTOField(dto, "MinimumSkillLevel"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "Projectile").ShouldBe(Helpers.GetDTOField(dto, "Projectile"));
        Helpers.GetSpriggitField(spriggit, "ResistValue").ShouldBe(Helpers.GetDTOField(dto, "ResistValueFormKey"));
        Helpers.GetSpriggitField(spriggit, "SecondActorValue").ShouldBe(Helpers.GetDTOField(dto, "SecondActorValue"));
        Helpers.GetSpriggitField(spriggit, "SecondActorValueWeight").ShouldBe(Helpers.GetDTOField(dto, "SecondActorValueWeight"));
        Helpers.GetSpriggitField(spriggit, "Sound[0]").ShouldBe(Helpers.GetDTOField(dto, "Sound[0]"));
        Helpers.GetSpriggitField(spriggit, "Sound[1]").ShouldBe(Helpers.GetDTOField(dto, "Sound[1]"));
        Helpers.GetSpriggitField(spriggit, "Sound[2]").ShouldBe(Helpers.GetDTOField(dto, "Sound[2]"));
        Helpers.GetSpriggitField(spriggit, "SpellmakingCastingTime").ShouldBe(Helpers.GetDTOField(dto, "SpellmakingCastingTime"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Name"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.ActorValue", "Archetype.MutagenObjectType", "Archetype.Type", "BaseCost", "CastingArt", "CastingLight", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DualCastScale", "EditorID", "EquipAbility", "Explosion", "FormKey", "HitShader", "ImageSpaceModifier", "ImpactData", "MagicSkill", "MenuDisplayObject", "MinimumSkillLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "ResistValue", "SecondActorValue", "SecondActorValueWeight", "Sound[0]", "Sound[1]", "Sound[2]", "SpellmakingCastingTime", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].Data", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].Data", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.ActorValue", "Archetype.MutagenObjectType", "Archetype.Type", "BaseCost", "CastingArt", "CastingLight", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DualCastScale", "EditorID", "EquipAbilityFormKey", "Explosion", "FormKey", "HitShader", "ImageSpaceModifier", "ImpactData", "MagicSkill", "MenuDisplayObject", "MinimumSkillLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "ResistValueFormKey", "SecondActorValue", "SecondActorValueWeight", "Sound[0]", "Sound[1]", "Sound[2]", "SpellmakingCastingTime", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].Data", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].Data", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name");
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

        Helpers.GetSpriggitField(spriggit, "Archetype.ActorValue").ShouldBe(Helpers.GetDTOField(dto, "Archetype.ActorValue"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Archetype.Type").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Type"));
        Helpers.GetSpriggitField(spriggit, "BaseCost").ShouldBe(Helpers.GetDTOField(dto, "BaseCost"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "DualCastScale").ShouldBe(Helpers.GetDTOField(dto, "DualCastScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "EnchantShader").ShouldBe(Helpers.GetDTOField(dto, "EnchantShader"));
        Helpers.GetSpriggitField(spriggit, "EquipAbility").ShouldBe(Helpers.GetDTOField(dto, "EquipAbilityFormKey"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "ImpactData").ShouldBe(Helpers.GetDTOField(dto, "ImpactData"));
        Helpers.GetSpriggitField(spriggit, "MagicSkill").ShouldBe(Helpers.GetDTOField(dto, "MagicSkill"));
        Helpers.GetSpriggitField(spriggit, "MenuDisplayObject").ShouldBe(Helpers.GetDTOField(dto, "MenuDisplayObject"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "ResistValue").ShouldBe(Helpers.GetDTOField(dto, "ResistValueFormKey"));
        Helpers.GetSpriggitField(spriggit, "SecondActorValue").ShouldBe(Helpers.GetDTOField(dto, "SecondActorValue"));
        Helpers.GetSpriggitField(spriggit, "SecondActorValueWeight").ShouldBe(Helpers.GetDTOField(dto, "SecondActorValueWeight"));
        Helpers.GetSpriggitField(spriggit, "Sound[0]").ShouldBe(Helpers.GetDTOField(dto, "Sound[0]"));
        Helpers.GetSpriggitField(spriggit, "Sound[1]").ShouldBe(Helpers.GetDTOField(dto, "Sound[1]"));
        Helpers.GetSpriggitField(spriggit, "Sound[2]").ShouldBe(Helpers.GetDTOField(dto, "Sound[2]"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.ActorValue", "Archetype.MutagenObjectType", "Archetype.Type", "BaseCost", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DualCastScale", "EditorID", "EnchantShader", "EquipAbility", "FormKey", "HitShader", "ImpactData", "MagicSkill", "MenuDisplayObject", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ResistValue", "SecondActorValue", "SecondActorValueWeight", "Sound[0]", "Sound[1]", "Sound[2]", "TargetType", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.ActorValue", "Archetype.MutagenObjectType", "Archetype.Type", "BaseCost", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DualCastScale", "EditorID", "EnchantShader", "EquipAbilityFormKey", "FormKey", "HitShader", "ImpactData", "MagicSkill", "MenuDisplayObject", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ResistValueFormKey", "SecondActorValue", "SecondActorValueWeight", "Sound[0]", "Sound[1]", "Sound[2]", "TargetType", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "Archetype.ActorValue").ShouldBe(Helpers.GetDTOField(dto, "Archetype.ActorValue"));
        Helpers.GetSpriggitField(spriggit, "Archetype.Association").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Association"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "BaseCost").ShouldBe(Helpers.GetDTOField(dto, "BaseCost"));
        Helpers.GetSpriggitField(spriggit, "CastingArt").ShouldBe(Helpers.GetDTOField(dto, "CastingArt"));
        Helpers.GetSpriggitField(spriggit, "CastingLight").ShouldBe(Helpers.GetDTOField(dto, "CastingLight"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "DualCastScale").ShouldBe(Helpers.GetDTOField(dto, "DualCastScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "EquipAbility").ShouldBe(Helpers.GetDTOField(dto, "EquipAbilityFormKey"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HitEffectArt").ShouldBe(Helpers.GetDTOField(dto, "HitEffectArt"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "ImageSpaceModifier").ShouldBe(Helpers.GetDTOField(dto, "ImageSpaceModifier"));
        Helpers.GetSpriggitField(spriggit, "MagicSkill").ShouldBe(Helpers.GetDTOField(dto, "MagicSkill"));
        Helpers.GetSpriggitField(spriggit, "MenuDisplayObject").ShouldBe(Helpers.GetDTOField(dto, "MenuDisplayObject"));
        Helpers.GetSpriggitField(spriggit, "MinimumSkillLevel").ShouldBe(Helpers.GetDTOField(dto, "MinimumSkillLevel"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "PerkToApply").ShouldBe(Helpers.GetDTOField(dto, "PerkToApplyFormKey"));
        Helpers.GetSpriggitField(spriggit, "SkillUsageMultiplier").ShouldBe(Helpers.GetDTOField(dto, "SkillUsageMultiplier"));
        Helpers.GetSpriggitField(spriggit, "Sound[0]").ShouldBe(Helpers.GetDTOField(dto, "Sound[0]"));
        Helpers.GetSpriggitField(spriggit, "Sound[1]").ShouldBe(Helpers.GetDTOField(dto, "Sound[1]"));
        Helpers.GetSpriggitField(spriggit, "Sound[2]").ShouldBe(Helpers.GetDTOField(dto, "Sound[2]"));
        Helpers.GetSpriggitField(spriggit, "SpellmakingCastingTime").ShouldBe(Helpers.GetDTOField(dto, "SpellmakingCastingTime"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.ActorValue", "Archetype.Association", "Archetype.MutagenObjectType", "BaseCost", "CastingArt", "CastingLight", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DualCastScale", "EditorID", "EquipAbility", "FormKey", "HitEffectArt", "HitShader", "ImageSpaceModifier", "MagicSkill", "MenuDisplayObject", "MinimumSkillLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "PerkToApply", "SkillUsageMultiplier", "Sound[0]", "Sound[1]", "Sound[2]", "SpellmakingCastingTime", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.ActorValue", "Archetype.Association", "Archetype.MutagenObjectType", "BaseCost", "CastingArt", "CastingLight", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DualCastScale", "EditorID", "EquipAbilityFormKey", "FormKey", "HitEffectArt", "HitShader", "ImageSpaceModifier", "MagicSkill", "MenuDisplayObject", "MinimumSkillLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "PerkToApplyFormKey", "SkillUsageMultiplier", "Sound[0]", "Sound[1]", "Sound[2]", "SpellmakingCastingTime", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object");
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

        Helpers.GetSpriggitField(spriggit, "Archetype.ActorValue").ShouldBe(Helpers.GetDTOField(dto, "Archetype.ActorValue"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "BaseCost").ShouldBe(Helpers.GetDTOField(dto, "BaseCost"));
        Helpers.GetSpriggitField(spriggit, "CastingArt").ShouldBe(Helpers.GetDTOField(dto, "CastingArt"));
        Helpers.GetSpriggitField(spriggit, "CastingLight").ShouldBe(Helpers.GetDTOField(dto, "CastingLight"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "DualCastScale").ShouldBe(Helpers.GetDTOField(dto, "DualCastScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "ImpactData").ShouldBe(Helpers.GetDTOField(dto, "ImpactData"));
        Helpers.GetSpriggitField(spriggit, "MagicSkill").ShouldBe(Helpers.GetDTOField(dto, "MagicSkill"));
        Helpers.GetSpriggitField(spriggit, "MenuDisplayObject").ShouldBe(Helpers.GetDTOField(dto, "MenuDisplayObject"));
        Helpers.GetSpriggitField(spriggit, "MinimumSkillLevel").ShouldBe(Helpers.GetDTOField(dto, "MinimumSkillLevel"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "Projectile").ShouldBe(Helpers.GetDTOField(dto, "Projectile"));
        Helpers.GetSpriggitField(spriggit, "ResistValue").ShouldBe(Helpers.GetDTOField(dto, "ResistValueFormKey"));
        Helpers.GetSpriggitField(spriggit, "SkillUsageMultiplier").ShouldBe(Helpers.GetDTOField(dto, "SkillUsageMultiplier"));
        Helpers.GetSpriggitField(spriggit, "Sound[0]").ShouldBe(Helpers.GetDTOField(dto, "Sound[0]"));
        Helpers.GetSpriggitField(spriggit, "Sound[1]").ShouldBe(Helpers.GetDTOField(dto, "Sound[1]"));
        Helpers.GetSpriggitField(spriggit, "Sound[2]").ShouldBe(Helpers.GetDTOField(dto, "Sound[2]"));
        Helpers.GetSpriggitField(spriggit, "SpellmakingCastingTime").ShouldBe(Helpers.GetDTOField(dto, "SpellmakingCastingTime"));
        Helpers.GetSpriggitField(spriggit, "TaperWeight").ShouldBe(Helpers.GetDTOField(dto, "TaperWeight"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][10].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][10].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][10].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][10].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][10].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][10].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][11].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][11].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][11].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][11].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][11].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][11].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][12].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][12].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][12].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][12].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][12].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][12].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][13].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][13].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][13].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][13].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][13].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][13].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][14].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][14].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][14].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][14].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][14].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][14].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][15].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][15].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][15].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][15].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][15].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][15].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][16].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][16].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][16].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][16].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][16].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][16].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][17].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][17].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][17].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][17].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][17].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][17].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][18].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][18].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][18].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][18].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][18].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][18].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][19].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][19].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][19].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][19].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][19].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][19].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][20].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][20].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][20].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][20].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][20].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][20].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][21].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][21].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][21].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][21].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][21].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][21].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][22].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][22].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][22].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][22].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][22].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][22].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][23].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][23].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][23].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][23].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][23].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][23].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][24].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][24].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][24].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][24].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][24].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][24].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][25].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][25].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][25].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][25].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][25].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][25].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][26].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][26].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][26].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][26].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][26].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][26].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][27].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][27].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][27].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][27].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][27].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][27].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][28].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][28].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][28].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][28].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][28].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][28].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][29].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][29].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][29].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][29].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][29].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][29].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][30].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][30].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][30].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][30].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][30].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][30].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][7].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][7].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][7].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][7].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][7].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][7].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][8].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][8].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][8].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][8].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][8].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][8].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][9].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][9].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][9].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][9].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][9].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][9].Object"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.ActorValue", "Archetype.MutagenObjectType", "BaseCost", "CastingArt", "CastingLight", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DualCastScale", "EditorID", "FormKey", "HitShader", "ImpactData", "MagicSkill", "MenuDisplayObject", "MinimumSkillLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "ResistValue", "SkillUsageMultiplier", "Sound[0]", "Sound[1]", "Sound[2]", "SpellmakingCastingTime", "TaperWeight", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][10].MutagenObjectType", "VirtualMachineAdapter[0][10].Name", "VirtualMachineAdapter[0][10].Object", "VirtualMachineAdapter[0][11].MutagenObjectType", "VirtualMachineAdapter[0][11].Name", "VirtualMachineAdapter[0][11].Object", "VirtualMachineAdapter[0][12].MutagenObjectType", "VirtualMachineAdapter[0][12].Name", "VirtualMachineAdapter[0][12].Object", "VirtualMachineAdapter[0][13].MutagenObjectType", "VirtualMachineAdapter[0][13].Name", "VirtualMachineAdapter[0][13].Object", "VirtualMachineAdapter[0][14].MutagenObjectType", "VirtualMachineAdapter[0][14].Name", "VirtualMachineAdapter[0][14].Object", "VirtualMachineAdapter[0][15].MutagenObjectType", "VirtualMachineAdapter[0][15].Name", "VirtualMachineAdapter[0][15].Object", "VirtualMachineAdapter[0][16].MutagenObjectType", "VirtualMachineAdapter[0][16].Name", "VirtualMachineAdapter[0][16].Object", "VirtualMachineAdapter[0][17].MutagenObjectType", "VirtualMachineAdapter[0][17].Name", "VirtualMachineAdapter[0][17].Object", "VirtualMachineAdapter[0][18].MutagenObjectType", "VirtualMachineAdapter[0][18].Name", "VirtualMachineAdapter[0][18].Object", "VirtualMachineAdapter[0][19].MutagenObjectType", "VirtualMachineAdapter[0][19].Name", "VirtualMachineAdapter[0][19].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][20].MutagenObjectType", "VirtualMachineAdapter[0][20].Name", "VirtualMachineAdapter[0][20].Object", "VirtualMachineAdapter[0][21].MutagenObjectType", "VirtualMachineAdapter[0][21].Name", "VirtualMachineAdapter[0][21].Object", "VirtualMachineAdapter[0][22].MutagenObjectType", "VirtualMachineAdapter[0][22].Name", "VirtualMachineAdapter[0][22].Object", "VirtualMachineAdapter[0][23].MutagenObjectType", "VirtualMachineAdapter[0][23].Name", "VirtualMachineAdapter[0][23].Object", "VirtualMachineAdapter[0][24].MutagenObjectType", "VirtualMachineAdapter[0][24].Name", "VirtualMachineAdapter[0][24].Object", "VirtualMachineAdapter[0][25].MutagenObjectType", "VirtualMachineAdapter[0][25].Name", "VirtualMachineAdapter[0][25].Object", "VirtualMachineAdapter[0][26].MutagenObjectType", "VirtualMachineAdapter[0][26].Name", "VirtualMachineAdapter[0][26].Object", "VirtualMachineAdapter[0][27].MutagenObjectType", "VirtualMachineAdapter[0][27].Name", "VirtualMachineAdapter[0][27].Object", "VirtualMachineAdapter[0][28].MutagenObjectType", "VirtualMachineAdapter[0][28].Name", "VirtualMachineAdapter[0][28].Object", "VirtualMachineAdapter[0][29].MutagenObjectType", "VirtualMachineAdapter[0][29].Name", "VirtualMachineAdapter[0][29].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object", "VirtualMachineAdapter[0][30].MutagenObjectType", "VirtualMachineAdapter[0][30].Name", "VirtualMachineAdapter[0][30].Object", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][4].Object", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][5].Object", "VirtualMachineAdapter[0][6].MutagenObjectType", "VirtualMachineAdapter[0][6].Name", "VirtualMachineAdapter[0][6].Object", "VirtualMachineAdapter[0][7].MutagenObjectType", "VirtualMachineAdapter[0][7].Name", "VirtualMachineAdapter[0][7].Object", "VirtualMachineAdapter[0][8].MutagenObjectType", "VirtualMachineAdapter[0][8].Name", "VirtualMachineAdapter[0][8].Object", "VirtualMachineAdapter[0][9].MutagenObjectType", "VirtualMachineAdapter[0][9].Name", "VirtualMachineAdapter[0][9].Object");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.ActorValue", "Archetype.MutagenObjectType", "BaseCost", "CastingArt", "CastingLight", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DualCastScale", "EditorID", "FormKey", "HitShader", "ImpactData", "MagicSkill", "MenuDisplayObject", "MinimumSkillLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "ResistValueFormKey", "SkillUsageMultiplier", "Sound[0]", "Sound[1]", "Sound[2]", "SpellmakingCastingTime", "TaperWeight", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][10].MutagenObjectType", "VirtualMachineAdapter[0][10].Name", "VirtualMachineAdapter[0][10].Object", "VirtualMachineAdapter[0][11].MutagenObjectType", "VirtualMachineAdapter[0][11].Name", "VirtualMachineAdapter[0][11].Object", "VirtualMachineAdapter[0][12].MutagenObjectType", "VirtualMachineAdapter[0][12].Name", "VirtualMachineAdapter[0][12].Object", "VirtualMachineAdapter[0][13].MutagenObjectType", "VirtualMachineAdapter[0][13].Name", "VirtualMachineAdapter[0][13].Object", "VirtualMachineAdapter[0][14].MutagenObjectType", "VirtualMachineAdapter[0][14].Name", "VirtualMachineAdapter[0][14].Object", "VirtualMachineAdapter[0][15].MutagenObjectType", "VirtualMachineAdapter[0][15].Name", "VirtualMachineAdapter[0][15].Object", "VirtualMachineAdapter[0][16].MutagenObjectType", "VirtualMachineAdapter[0][16].Name", "VirtualMachineAdapter[0][16].Object", "VirtualMachineAdapter[0][17].MutagenObjectType", "VirtualMachineAdapter[0][17].Name", "VirtualMachineAdapter[0][17].Object", "VirtualMachineAdapter[0][18].MutagenObjectType", "VirtualMachineAdapter[0][18].Name", "VirtualMachineAdapter[0][18].Object", "VirtualMachineAdapter[0][19].MutagenObjectType", "VirtualMachineAdapter[0][19].Name", "VirtualMachineAdapter[0][19].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][20].MutagenObjectType", "VirtualMachineAdapter[0][20].Name", "VirtualMachineAdapter[0][20].Object", "VirtualMachineAdapter[0][21].MutagenObjectType", "VirtualMachineAdapter[0][21].Name", "VirtualMachineAdapter[0][21].Object", "VirtualMachineAdapter[0][22].MutagenObjectType", "VirtualMachineAdapter[0][22].Name", "VirtualMachineAdapter[0][22].Object", "VirtualMachineAdapter[0][23].MutagenObjectType", "VirtualMachineAdapter[0][23].Name", "VirtualMachineAdapter[0][23].Object", "VirtualMachineAdapter[0][24].MutagenObjectType", "VirtualMachineAdapter[0][24].Name", "VirtualMachineAdapter[0][24].Object", "VirtualMachineAdapter[0][25].MutagenObjectType", "VirtualMachineAdapter[0][25].Name", "VirtualMachineAdapter[0][25].Object", "VirtualMachineAdapter[0][26].MutagenObjectType", "VirtualMachineAdapter[0][26].Name", "VirtualMachineAdapter[0][26].Object", "VirtualMachineAdapter[0][27].MutagenObjectType", "VirtualMachineAdapter[0][27].Name", "VirtualMachineAdapter[0][27].Object", "VirtualMachineAdapter[0][28].MutagenObjectType", "VirtualMachineAdapter[0][28].Name", "VirtualMachineAdapter[0][28].Object", "VirtualMachineAdapter[0][29].MutagenObjectType", "VirtualMachineAdapter[0][29].Name", "VirtualMachineAdapter[0][29].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object", "VirtualMachineAdapter[0][30].MutagenObjectType", "VirtualMachineAdapter[0][30].Name", "VirtualMachineAdapter[0][30].Object", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][4].Object", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][5].Object", "VirtualMachineAdapter[0][6].MutagenObjectType", "VirtualMachineAdapter[0][6].Name", "VirtualMachineAdapter[0][6].Object", "VirtualMachineAdapter[0][7].MutagenObjectType", "VirtualMachineAdapter[0][7].Name", "VirtualMachineAdapter[0][7].Object", "VirtualMachineAdapter[0][8].MutagenObjectType", "VirtualMachineAdapter[0][8].Name", "VirtualMachineAdapter[0][8].Object", "VirtualMachineAdapter[0][9].MutagenObjectType", "VirtualMachineAdapter[0][9].Name", "VirtualMachineAdapter[0][9].Object");
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

        Helpers.GetSpriggitField(spriggit, "Archetype.ActorValue").ShouldBe(Helpers.GetDTOField(dto, "Archetype.ActorValue"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Archetype.Type").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Type"));
        Helpers.GetSpriggitField(spriggit, "BaseCost").ShouldBe(Helpers.GetDTOField(dto, "BaseCost"));
        Helpers.GetSpriggitField(spriggit, "CastingArt").ShouldBe(Helpers.GetDTOField(dto, "CastingArt"));
        Helpers.GetSpriggitField(spriggit, "CastingLight").ShouldBe(Helpers.GetDTOField(dto, "CastingLight"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[0]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[0]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[1]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[1]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[2]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[4]"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "DualCastScale").ShouldBe(Helpers.GetDTOField(dto, "DualCastScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "Explosion").ShouldBe(Helpers.GetDTOField(dto, "Explosion"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "ImpactData").ShouldBe(Helpers.GetDTOField(dto, "ImpactData"));
        Helpers.GetSpriggitField(spriggit, "MagicSkill").ShouldBe(Helpers.GetDTOField(dto, "MagicSkill"));
        Helpers.GetSpriggitField(spriggit, "MenuDisplayObject").ShouldBe(Helpers.GetDTOField(dto, "MenuDisplayObject"));
        Helpers.GetSpriggitField(spriggit, "MinimumSkillLevel").ShouldBe(Helpers.GetDTOField(dto, "MinimumSkillLevel"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "Projectile").ShouldBe(Helpers.GetDTOField(dto, "Projectile"));
        Helpers.GetSpriggitField(spriggit, "SkillUsageMultiplier").ShouldBe(Helpers.GetDTOField(dto, "SkillUsageMultiplier"));
        Helpers.GetSpriggitField(spriggit, "Sound[0]").ShouldBe(Helpers.GetDTOField(dto, "Sound[0]"));
        Helpers.GetSpriggitField(spriggit, "Sound[1]").ShouldBe(Helpers.GetDTOField(dto, "Sound[1]"));
        Helpers.GetSpriggitField(spriggit, "Sound[2]").ShouldBe(Helpers.GetDTOField(dto, "Sound[2]"));
        Helpers.GetSpriggitField(spriggit, "Sound[3]").ShouldBe(Helpers.GetDTOField(dto, "Sound[3]"));
        Helpers.GetSpriggitField(spriggit, "SpellmakingArea").ShouldBe(Helpers.GetDTOField(dto, "SpellmakingArea"));
        Helpers.GetSpriggitField(spriggit, "SpellmakingCastingTime").ShouldBe(Helpers.GetDTOField(dto, "SpellmakingCastingTime"));
        Helpers.GetSpriggitField(spriggit, "TaperWeight").ShouldBe(Helpers.GetDTOField(dto, "TaperWeight"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Object"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.ActorValue", "Archetype.MutagenObjectType", "Archetype.Type", "BaseCost", "CastingArt", "CastingLight", "CastType", "CompareOperator[0]", "CompareOperator[1]", "CompareOperator[2]", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "Data.Keyword[0]", "Data.Keyword[1]", "Data.Keyword[2]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DualCastScale", "EditorID", "Explosion", "FormKey", "HitShader", "ImpactData", "MagicSkill", "MenuDisplayObject", "MinimumSkillLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "SkillUsageMultiplier", "Sound[0]", "Sound[1]", "Sound[2]", "Sound[3]", "SpellmakingArea", "SpellmakingCastingTime", "TaperWeight", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.ActorValue", "Archetype.MutagenObjectType", "Archetype.Type", "BaseCost", "CastingArt", "CastingLight", "CastType", "CompareOperator[0]", "CompareOperator[1]", "CompareOperator[2]", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "Data.Keyword[0]", "Data.Keyword[1]", "Data.Keyword[2]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DualCastScale", "EditorID", "Explosion", "FormKey", "HitShader", "ImpactData", "MagicSkill", "MenuDisplayObject", "MinimumSkillLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Projectile", "SkillUsageMultiplier", "Sound[0]", "Sound[1]", "Sound[2]", "Sound[3]", "SpellmakingArea", "SpellmakingCastingTime", "TaperWeight", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object");
    }
}