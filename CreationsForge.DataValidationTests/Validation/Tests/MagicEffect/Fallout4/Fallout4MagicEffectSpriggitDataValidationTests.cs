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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["Archetype.Type"].ShouldBe(dtoFields["Archetype.Type"]);
        spriggitFields["CastingSoundLevel"].ShouldBe(dtoFields["CastingSoundLevel"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);
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
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["DualCastScale"].ShouldBe(dtoFields["DualCastScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["ResistValue"].ShouldBe(dtoFields["ResistValueFormKey"]);
        spriggitFields["Sounds"].ShouldBe(dtoFields["Sounds"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Unknown1"].ShouldBe(dtoFields["Unknown1"]);
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

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["Archetype.Type"].ShouldBe(dtoFields["Archetype.Type"]);
        spriggitFields["CastingSoundLevel"].ShouldBe(dtoFields["CastingSoundLevel"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);
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
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["DualCastScale"].ShouldBe(dtoFields["DualCastScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["ResistValue"].ShouldBe(dtoFields["ResistValueFormKey"]);
        spriggitFields["Sounds"].ShouldBe(dtoFields["Sounds"]);
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
        spriggitFields["VirtualMachineAdapter[0][4].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][4].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].Name"]);
        spriggitFields["VirtualMachineAdapter[0][4].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][5].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].Object"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["Archetype.Type"].ShouldBe(dtoFields["Archetype.Type"]);
        spriggitFields["CastingSoundLevel"].ShouldBe(dtoFields["CastingSoundLevel"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);
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
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["DualCastScale"].ShouldBe(dtoFields["DualCastScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["ResistValue"].ShouldBe(dtoFields["ResistValueFormKey"]);
        spriggitFields["Sounds"].ShouldBe(dtoFields["Sounds"]);
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
        spriggitFields["VirtualMachineAdapter[0][4].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][4].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].Name"]);
        spriggitFields["VirtualMachineAdapter[0][4].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][5].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].Object"]);
        spriggitFields["VirtualMachineAdapter[0][6].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].Data"]);
        spriggitFields["VirtualMachineAdapter[0][6].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][6].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].Name"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.ActorValue"].ShouldBe(dtoFields["Archetype.ActorValue"]);
        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["CastingSoundLevel"].ShouldBe(dtoFields["CastingSoundLevel"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["CompareOperator"].ShouldBe(dtoFields["CompareOperator"]);
        spriggitFields["Data.Function[0]"].ShouldBe(dtoFields["Data.Function[0]"]);
        spriggitFields["Data.Function[1]"].ShouldBe(dtoFields["Data.Function[1]"]);
        spriggitFields["Data.Function[2]"].ShouldBe(dtoFields["Data.Function[2]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.ParameterOneNumber[0]"].ShouldBe(dtoFields["Data.ParameterOneNumber[0]"]);
        spriggitFields["Data.ParameterOneNumber[1]"].ShouldBe(dtoFields["Data.ParameterOneNumber[1]"]);
        spriggitFields["Data.ParameterOneNumber[2]"].ShouldBe(dtoFields["Data.ParameterOneNumber[2]"]);
        spriggitFields["Data.ParameterOneRecord[0]"].ShouldBe(dtoFields["Data.ParameterOneRecord[0]"]);
        spriggitFields["Data.ParameterOneRecord[1]"].ShouldBe(dtoFields["Data.ParameterOneRecord[1]"]);
        spriggitFields["Data.ParameterOneRecord[2]"].ShouldBe(dtoFields["Data.ParameterOneRecord[2]"]);
        spriggitFields["Data.Unknown2[0]"].ShouldBe(dtoFields["Data.Unknown2[0]"]);
        spriggitFields["Data.Unknown2[1]"].ShouldBe(dtoFields["Data.Unknown2[1]"]);
        spriggitFields["Data.Unknown2[2]"].ShouldBe(dtoFields["Data.Unknown2[2]"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["DualCastScale"].ShouldBe(dtoFields["DualCastScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["Sound"].ShouldBe(dtoFields["Sound"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["VirtualMachineAdapter[1].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[1].Count"]);
        spriggitFields["VirtualMachineAdapter[1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1].Name"]);
        spriggitFields["VirtualMachineAdapter[1][0].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[1][0].Data"]);
        spriggitFields["VirtualMachineAdapter[1][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[1][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[1][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1][0].Name"]);
        spriggitFields["VirtualMachineAdapter[2].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[2].Count"]);
        spriggitFields["VirtualMachineAdapter[2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[2].Name"]);
        spriggitFields["VirtualMachineAdapter[2][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[2][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[2][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[2][0].Name"]);
        spriggitFields["VirtualMachineAdapter[2][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[2][0].Object"]);
        spriggitFields["VirtualMachineAdapter[2][1].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[2][1].Data"]);
        spriggitFields["VirtualMachineAdapter[2][1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[2][1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[2][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[2][1].Name"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Archetype.MutagenObjectType"].ShouldBe(dtoFields["Archetype.MutagenObjectType"]);
        spriggitFields["Archetype.Type"].ShouldBe(dtoFields["Archetype.Type"]);
        spriggitFields["CastingSoundLevel"].ShouldBe(dtoFields["CastingSoundLevel"]);
        spriggitFields["CastType"].ShouldBe(dtoFields["CastType"]);
        spriggitFields["CompareOperator"].ShouldBe(dtoFields["CompareOperator"]);
        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["Data.Function[0]"].ShouldBe(dtoFields["Data.Function[0]"]);
        spriggitFields["Data.Function[1]"].ShouldBe(dtoFields["Data.Function[1]"]);
        spriggitFields["Data.Function[2]"].ShouldBe(dtoFields["Data.Function[2]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.ParameterOneNumber[0]"].ShouldBe(dtoFields["Data.ParameterOneNumber[0]"]);
        spriggitFields["Data.ParameterOneNumber[1]"].ShouldBe(dtoFields["Data.ParameterOneNumber[1]"]);
        spriggitFields["Data.ParameterOneNumber[2]"].ShouldBe(dtoFields["Data.ParameterOneNumber[2]"]);
        spriggitFields["Data.ParameterOneRecord[0]"].ShouldBe(dtoFields["Data.ParameterOneRecord[0]"]);
        spriggitFields["Data.ParameterOneRecord[1]"].ShouldBe(dtoFields["Data.ParameterOneRecord[1]"]);
        spriggitFields["Data.ParameterOneRecord[2]"].ShouldBe(dtoFields["Data.ParameterOneRecord[2]"]);
        spriggitFields["Data.Unknown2[0]"].ShouldBe(dtoFields["Data.Unknown2[0]"]);
        spriggitFields["Data.Unknown2[1]"].ShouldBe(dtoFields["Data.Unknown2[1]"]);
        spriggitFields["Data.Unknown2[2]"].ShouldBe(dtoFields["Data.Unknown2[2]"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["DualCastScale"].ShouldBe(dtoFields["DualCastScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["HitShader"].ShouldBe(dtoFields["HitShader"]);
        spriggitFields["ImpactData"].ShouldBe(dtoFields["ImpactData"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["Projectile"].ShouldBe(dtoFields["Projectile"]);
        spriggitFields["Sound"].ShouldBe(dtoFields["Sound"]);
        spriggitFields["TargetType"].ShouldBe(dtoFields["TargetType"]);
        spriggitFields["Unknown1"].ShouldBe(dtoFields["Unknown1"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Alias"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Alias"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
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
}
