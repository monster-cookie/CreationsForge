using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MagicEffect.Fallout4;

public class Fallout4MagicEffectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "247A6C:Fallout4.esm")]
    [Trait("EditorID", "CritCryoFreezeEffect")]
    [Trait("SpriggitFile", "MagicEffects/CritCryoFreezeEffect - 247A6C_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSample_CritCryoFreezeEffect()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MagicEffect,
            "CritCryoFreezeEffect");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MagicEffect,
            "247A6C:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Archetype.Type").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Type"));
        Helpers.GetSpriggitField(spriggit, "CastingSoundLevel").ShouldBe(Helpers.GetDTOField(dto, "CastingSoundLevel"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
        Helpers.GetSpriggitField(spriggit, "DualCastScale").ShouldBe(Helpers.GetDTOField(dto, "DualCastScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "ResistValue").ShouldBe(Helpers.GetDTOField(dto, "ResistValueFormKey"));
        Helpers.GetSpriggitField(spriggit, "Sounds").ShouldBe(Helpers.GetDTOField(dto, "Sounds"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Unknown1").ShouldBe(Helpers.GetDTOField(dto, "Unknown1"));
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

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.MutagenObjectType", "Archetype.Type", "CastingSoundLevel", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "DualCastScale", "EditorID", "FormKey", "HitShader", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ResistValue", "Sounds", "TargetType", "Unknown1", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.MutagenObjectType", "Archetype.Type", "CastingSoundLevel", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "DualCastScale", "EditorID", "FormKey", "HitShader", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ResistValueFormKey", "Sounds", "TargetType", "Unknown1", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "18C354:Fallout4.esm")]
    [Trait("EditorID", "CryoFreezeEffect01")]
    [Trait("SpriggitFile", "MagicEffects/CryoFreezeEffect01 - 18C354_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSample_CryoFreezeEffect01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MagicEffect,
            "CryoFreezeEffect01");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MagicEffect,
            "18C354:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Archetype.Type").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Type"));
        Helpers.GetSpriggitField(spriggit, "CastingSoundLevel").ShouldBe(Helpers.GetDTOField(dto, "CastingSoundLevel"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
        Helpers.GetSpriggitField(spriggit, "DualCastScale").ShouldBe(Helpers.GetDTOField(dto, "DualCastScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "ResistValue").ShouldBe(Helpers.GetDTOField(dto, "ResistValueFormKey"));
        Helpers.GetSpriggitField(spriggit, "Sounds").ShouldBe(Helpers.GetDTOField(dto, "Sounds"));
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
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].Object"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.MutagenObjectType", "Archetype.Type", "CastingSoundLevel", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "DualCastScale", "EditorID", "FormKey", "HitShader", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ResistValue", "Sounds", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][4].Object", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][5].Object");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.MutagenObjectType", "Archetype.Type", "CastingSoundLevel", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "DualCastScale", "EditorID", "FormKey", "HitShader", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ResistValueFormKey", "Sounds", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][4].Object", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][5].Object");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "18C356:Fallout4.esm")]
    [Trait("EditorID", "CryoFreezeEffect02")]
    [Trait("SpriggitFile", "MagicEffects/CryoFreezeEffect02 - 18C356_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSample_CryoFreezeEffect02()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MagicEffect,
            "CryoFreezeEffect02");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MagicEffect,
            "18C356:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Archetype.Type").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Type"));
        Helpers.GetSpriggitField(spriggit, "CastingSoundLevel").ShouldBe(Helpers.GetDTOField(dto, "CastingSoundLevel"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
        Helpers.GetSpriggitField(spriggit, "DualCastScale").ShouldBe(Helpers.GetDTOField(dto, "DualCastScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "ResistValue").ShouldBe(Helpers.GetDTOField(dto, "ResistValueFormKey"));
        Helpers.GetSpriggitField(spriggit, "Sounds").ShouldBe(Helpers.GetDTOField(dto, "Sounds"));
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
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].Name"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.MutagenObjectType", "Archetype.Type", "CastingSoundLevel", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "DualCastScale", "EditorID", "FormKey", "HitShader", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ResistValue", "Sounds", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][4].Object", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][5].Object", "VirtualMachineAdapter[0][6].Data", "VirtualMachineAdapter[0][6].MutagenObjectType", "VirtualMachineAdapter[0][6].Name");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.MutagenObjectType", "Archetype.Type", "CastingSoundLevel", "CastType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "DualCastScale", "EditorID", "FormKey", "HitShader", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ResistValueFormKey", "Sounds", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][4].Object", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][5].Object", "VirtualMachineAdapter[0][6].Data", "VirtualMachineAdapter[0][6].MutagenObjectType", "VirtualMachineAdapter[0][6].Name");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "171781:Fallout4.esm")]
    [Trait("EditorID", "PerkPainTrainKnockbackEffect")]
    [Trait("SpriggitFile", "MagicEffects/PerkPainTrainKnockbackEffect - 171781_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSample_PerkPainTrainKnockbackEffect()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MagicEffect,
            "PerkPainTrainKnockbackEffect");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MagicEffect,
            "171781:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Archetype.ActorValue").ShouldBe(Helpers.GetDTOField(dto, "Archetype.ActorValue"));
        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "CastingSoundLevel").ShouldBe(Helpers.GetDTOField(dto, "CastingSoundLevel"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Unknown2[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Unknown2[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Unknown2[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Unknown2[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Unknown2[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Unknown2[2]"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DualCastScale").ShouldBe(Helpers.GetDTOField(dto, "DualCastScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "Sound").ShouldBe(Helpers.GetDTOField(dto, "Sound"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][0].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][0].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[2].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[2].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[2][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[2][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[2][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[2][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[2][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[2][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[2][1].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[2][1].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[2][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[2][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[2][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[2][1].Name"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.ActorValue", "Archetype.MutagenObjectType", "CastingSoundLevel", "CastType", "CompareOperator", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.Unknown2[0]", "Data.Unknown2[1]", "Data.Unknown2[2]", "Description.TargetLanguage", "DualCastScale", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Sound", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[1].Count", "VirtualMachineAdapter[1].Name", "VirtualMachineAdapter[1][0].Data", "VirtualMachineAdapter[1][0].MutagenObjectType", "VirtualMachineAdapter[1][0].Name", "VirtualMachineAdapter[2].Count", "VirtualMachineAdapter[2].Name", "VirtualMachineAdapter[2][0].MutagenObjectType", "VirtualMachineAdapter[2][0].Name", "VirtualMachineAdapter[2][0].Object", "VirtualMachineAdapter[2][1].Data", "VirtualMachineAdapter[2][1].MutagenObjectType", "VirtualMachineAdapter[2][1].Name");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.ActorValue", "Archetype.MutagenObjectType", "CastingSoundLevel", "CastType", "CompareOperator", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.Unknown2[0]", "Data.Unknown2[1]", "Data.Unknown2[2]", "Description.TargetLanguage", "DualCastScale", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Sound", "TargetType", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[1].Count", "VirtualMachineAdapter[1].Name", "VirtualMachineAdapter[1][0].Data", "VirtualMachineAdapter[1][0].MutagenObjectType", "VirtualMachineAdapter[1][0].Name", "VirtualMachineAdapter[2].Count", "VirtualMachineAdapter[2].Name", "VirtualMachineAdapter[2][0].MutagenObjectType", "VirtualMachineAdapter[2][0].Name", "VirtualMachineAdapter[2][0].Object", "VirtualMachineAdapter[2][1].Data", "VirtualMachineAdapter[2][1].MutagenObjectType", "VirtualMachineAdapter[2][1].Name");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0AE04F:Fallout4.esm")]
    [Trait("EditorID", "DN102_LabDemo3ParalyzeEffect")]
    [Trait("SpriggitFile", "MagicEffects/DN102_LabDemo3ParalyzeEffect - 0AE04F_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSample_DN102_LabDemo3ParalyzeEffect()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MagicEffect,
            "DN102_LabDemo3ParalyzeEffect");
        var dto = Helpers.GetDTO<MagicEffectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MagicEffect,
            "0AE04F:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Archetype.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Archetype.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Archetype.Type").ShouldBe(Helpers.GetDTOField(dto, "Archetype.Type"));
        Helpers.GetSpriggitField(spriggit, "CastingSoundLevel").ShouldBe(Helpers.GetDTOField(dto, "CastingSoundLevel"));
        Helpers.GetSpriggitField(spriggit, "CastType").ShouldBe(Helpers.GetDTOField(dto, "CastType"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Unknown2[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Unknown2[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Unknown2[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Unknown2[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Unknown2[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Unknown2[2]"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "DualCastScale").ShouldBe(Helpers.GetDTOField(dto, "DualCastScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HitShader").ShouldBe(Helpers.GetDTOField(dto, "HitShader"));
        Helpers.GetSpriggitField(spriggit, "ImpactData").ShouldBe(Helpers.GetDTOField(dto, "ImpactData"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "Projectile").ShouldBe(Helpers.GetDTOField(dto, "Projectile"));
        Helpers.GetSpriggitField(spriggit, "Sound").ShouldBe(Helpers.GetDTOField(dto, "Sound"));
        Helpers.GetSpriggitField(spriggit, "TargetType").ShouldBe(Helpers.GetDTOField(dto, "TargetType"));
        Helpers.GetSpriggitField(spriggit, "Unknown1").ShouldBe(Helpers.GetDTOField(dto, "Unknown1"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Alias").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Alias"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Name"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Archetype.MutagenObjectType", "Archetype.Type", "CastingSoundLevel", "CastType", "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.Unknown2[0]", "Data.Unknown2[1]", "Data.Unknown2[2]", "Description.TargetLanguage", "DualCastScale", "EditorID", "FormKey", "HitShader", "ImpactData", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Projectile", "Sound", "TargetType", "Unknown1", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].Alias", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].Data", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Archetype.MutagenObjectType", "Archetype.Type", "CastingSoundLevel", "CastType", "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.Unknown2[0]", "Data.Unknown2[1]", "Data.Unknown2[2]", "Description.TargetLanguage", "DualCastScale", "EditorID", "FormKey", "HitShader", "ImpactData", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Projectile", "Sound", "TargetType", "Unknown1", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].Alias", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].Data", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name");
    }
}