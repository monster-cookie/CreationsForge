using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Class.Skyrim;

public class SkyrimClassSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "0E3A6E:Skyrim.esm")]
    [Trait("EditorID", "TrainerAlchemyExpert")]
    [Trait("SpriggitFile", "Classes/TrainerAlchemyExpert - 0E3A6E_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_TrainerAlchemyExpert()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Class,
            "TrainerAlchemyExpert");
        var dto = Helpers.GetDTO<ClassDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Class,
            "0E3A6E:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "BleedoutDefault").ShouldBe(Helpers.GetDTOField(dto, "BleedoutDefault"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "MaxTrainingLevel").ShouldBe(Helpers.GetDTOField(dto, "MaxTrainingLevel"));
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
        Helpers.GetSpriggitField(spriggit, "Teaches").ShouldBe(Helpers.GetDTOField(dto, "Teaches"));
        Helpers.GetSpriggitField(spriggit, "Unknown").ShouldBe(Helpers.GetDTOField(dto, "Unknown"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[10]").ShouldBe(Helpers.GetDTOField(dto, "Value[10]"));
        Helpers.GetSpriggitField(spriggit, "Value[11]").ShouldBe(Helpers.GetDTOField(dto, "Value[11]"));
        Helpers.GetSpriggitField(spriggit, "Value[12]").ShouldBe(Helpers.GetDTOField(dto, "Value[12]"));
        Helpers.GetSpriggitField(spriggit, "Value[13]").ShouldBe(Helpers.GetDTOField(dto, "Value[13]"));
        Helpers.GetSpriggitField(spriggit, "Value[14]").ShouldBe(Helpers.GetDTOField(dto, "Value[14]"));
        Helpers.GetSpriggitField(spriggit, "Value[15]").ShouldBe(Helpers.GetDTOField(dto, "Value[15]"));
        Helpers.GetSpriggitField(spriggit, "Value[16]").ShouldBe(Helpers.GetDTOField(dto, "Value[16]"));
        Helpers.GetSpriggitField(spriggit, "Value[17]").ShouldBe(Helpers.GetDTOField(dto, "Value[17]"));
        Helpers.GetSpriggitField(spriggit, "Value[18]").ShouldBe(Helpers.GetDTOField(dto, "Value[18]"));
        Helpers.GetSpriggitField(spriggit, "Value[19]").ShouldBe(Helpers.GetDTOField(dto, "Value[19]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[20]").ShouldBe(Helpers.GetDTOField(dto, "Value[20]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Value[4]").ShouldBe(Helpers.GetDTOField(dto, "Value[4]"));
        Helpers.GetSpriggitField(spriggit, "Value[5]").ShouldBe(Helpers.GetDTOField(dto, "Value[5]"));
        Helpers.GetSpriggitField(spriggit, "Value[6]").ShouldBe(Helpers.GetDTOField(dto, "Value[6]"));
        Helpers.GetSpriggitField(spriggit, "Value[7]").ShouldBe(Helpers.GetDTOField(dto, "Value[7]"));
        Helpers.GetSpriggitField(spriggit, "Value[8]").ShouldBe(Helpers.GetDTOField(dto, "Value[8]"));
        Helpers.GetSpriggitField(spriggit, "Value[9]").ShouldBe(Helpers.GetDTOField(dto, "Value[9]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VoicePoints").ShouldBe(Helpers.GetDTOField(dto, "VoicePoints"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "BleedoutDefault", "Description.TargetLanguage", "EditorID", "FormKey", "MaxTrainingLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Teaches", "Unknown", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[18]", "Value[19]", "Value[2]", "Value[20]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl", "VoicePoints");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "BleedoutDefault", "Description.TargetLanguage", "EditorID", "FormKey", "MaxTrainingLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Teaches", "Unknown", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[18]", "Value[19]", "Value[2]", "Value[20]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl", "VoicePoints");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "0E3A5D:Skyrim.esm")]
    [Trait("EditorID", "TrainerAlchemyJourneyman")]
    [Trait("SpriggitFile", "Classes/TrainerAlchemyJourneyman - 0E3A5D_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_TrainerAlchemyJourneyman()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Class,
            "TrainerAlchemyJourneyman");
        var dto = Helpers.GetDTO<ClassDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Class,
            "0E3A5D:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "BleedoutDefault").ShouldBe(Helpers.GetDTOField(dto, "BleedoutDefault"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "MaxTrainingLevel").ShouldBe(Helpers.GetDTOField(dto, "MaxTrainingLevel"));
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
        Helpers.GetSpriggitField(spriggit, "Teaches").ShouldBe(Helpers.GetDTOField(dto, "Teaches"));
        Helpers.GetSpriggitField(spriggit, "Unknown").ShouldBe(Helpers.GetDTOField(dto, "Unknown"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[10]").ShouldBe(Helpers.GetDTOField(dto, "Value[10]"));
        Helpers.GetSpriggitField(spriggit, "Value[11]").ShouldBe(Helpers.GetDTOField(dto, "Value[11]"));
        Helpers.GetSpriggitField(spriggit, "Value[12]").ShouldBe(Helpers.GetDTOField(dto, "Value[12]"));
        Helpers.GetSpriggitField(spriggit, "Value[13]").ShouldBe(Helpers.GetDTOField(dto, "Value[13]"));
        Helpers.GetSpriggitField(spriggit, "Value[14]").ShouldBe(Helpers.GetDTOField(dto, "Value[14]"));
        Helpers.GetSpriggitField(spriggit, "Value[15]").ShouldBe(Helpers.GetDTOField(dto, "Value[15]"));
        Helpers.GetSpriggitField(spriggit, "Value[16]").ShouldBe(Helpers.GetDTOField(dto, "Value[16]"));
        Helpers.GetSpriggitField(spriggit, "Value[17]").ShouldBe(Helpers.GetDTOField(dto, "Value[17]"));
        Helpers.GetSpriggitField(spriggit, "Value[18]").ShouldBe(Helpers.GetDTOField(dto, "Value[18]"));
        Helpers.GetSpriggitField(spriggit, "Value[19]").ShouldBe(Helpers.GetDTOField(dto, "Value[19]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[20]").ShouldBe(Helpers.GetDTOField(dto, "Value[20]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Value[4]").ShouldBe(Helpers.GetDTOField(dto, "Value[4]"));
        Helpers.GetSpriggitField(spriggit, "Value[5]").ShouldBe(Helpers.GetDTOField(dto, "Value[5]"));
        Helpers.GetSpriggitField(spriggit, "Value[6]").ShouldBe(Helpers.GetDTOField(dto, "Value[6]"));
        Helpers.GetSpriggitField(spriggit, "Value[7]").ShouldBe(Helpers.GetDTOField(dto, "Value[7]"));
        Helpers.GetSpriggitField(spriggit, "Value[8]").ShouldBe(Helpers.GetDTOField(dto, "Value[8]"));
        Helpers.GetSpriggitField(spriggit, "Value[9]").ShouldBe(Helpers.GetDTOField(dto, "Value[9]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VoicePoints").ShouldBe(Helpers.GetDTOField(dto, "VoicePoints"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "BleedoutDefault", "Description.TargetLanguage", "EditorID", "FormKey", "MaxTrainingLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Teaches", "Unknown", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[18]", "Value[19]", "Value[2]", "Value[20]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl", "VoicePoints");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "BleedoutDefault", "Description.TargetLanguage", "EditorID", "FormKey", "MaxTrainingLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Teaches", "Unknown", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[18]", "Value[19]", "Value[2]", "Value[20]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl", "VoicePoints");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "02F202:Skyrim.esm")]
    [Trait("EditorID", "AAAPlayerSpellswordClass")]
    [Trait("SpriggitFile", "Classes/AAAPlayerSpellswordClass - 02F202_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_AAAPlayerSpellswordClass()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Class,
            "AAAPlayerSpellswordClass");
        var dto = Helpers.GetDTO<ClassDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Class,
            "02F202:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "BleedoutDefault").ShouldBe(Helpers.GetDTOField(dto, "BleedoutDefault"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
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
        Helpers.GetSpriggitField(spriggit, "Teaches").ShouldBe(Helpers.GetDTOField(dto, "Teaches"));
        Helpers.GetSpriggitField(spriggit, "Unknown").ShouldBe(Helpers.GetDTOField(dto, "Unknown"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[10]").ShouldBe(Helpers.GetDTOField(dto, "Value[10]"));
        Helpers.GetSpriggitField(spriggit, "Value[11]").ShouldBe(Helpers.GetDTOField(dto, "Value[11]"));
        Helpers.GetSpriggitField(spriggit, "Value[12]").ShouldBe(Helpers.GetDTOField(dto, "Value[12]"));
        Helpers.GetSpriggitField(spriggit, "Value[13]").ShouldBe(Helpers.GetDTOField(dto, "Value[13]"));
        Helpers.GetSpriggitField(spriggit, "Value[14]").ShouldBe(Helpers.GetDTOField(dto, "Value[14]"));
        Helpers.GetSpriggitField(spriggit, "Value[15]").ShouldBe(Helpers.GetDTOField(dto, "Value[15]"));
        Helpers.GetSpriggitField(spriggit, "Value[16]").ShouldBe(Helpers.GetDTOField(dto, "Value[16]"));
        Helpers.GetSpriggitField(spriggit, "Value[17]").ShouldBe(Helpers.GetDTOField(dto, "Value[17]"));
        Helpers.GetSpriggitField(spriggit, "Value[18]").ShouldBe(Helpers.GetDTOField(dto, "Value[18]"));
        Helpers.GetSpriggitField(spriggit, "Value[19]").ShouldBe(Helpers.GetDTOField(dto, "Value[19]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[20]").ShouldBe(Helpers.GetDTOField(dto, "Value[20]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Value[4]").ShouldBe(Helpers.GetDTOField(dto, "Value[4]"));
        Helpers.GetSpriggitField(spriggit, "Value[5]").ShouldBe(Helpers.GetDTOField(dto, "Value[5]"));
        Helpers.GetSpriggitField(spriggit, "Value[6]").ShouldBe(Helpers.GetDTOField(dto, "Value[6]"));
        Helpers.GetSpriggitField(spriggit, "Value[7]").ShouldBe(Helpers.GetDTOField(dto, "Value[7]"));
        Helpers.GetSpriggitField(spriggit, "Value[8]").ShouldBe(Helpers.GetDTOField(dto, "Value[8]"));
        Helpers.GetSpriggitField(spriggit, "Value[9]").ShouldBe(Helpers.GetDTOField(dto, "Value[9]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VoicePoints").ShouldBe(Helpers.GetDTOField(dto, "VoicePoints"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "BleedoutDefault", "Description.TargetLanguage", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Teaches", "Unknown", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[18]", "Value[19]", "Value[2]", "Value[20]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl", "VoicePoints");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "BleedoutDefault", "Description.TargetLanguage", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Teaches", "Unknown", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[18]", "Value[19]", "Value[2]", "Value[20]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl", "VoicePoints");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "013177:Skyrim.esm")]
    [Trait("EditorID", "CombatSpellsword")]
    [Trait("SpriggitFile", "Classes/CombatSpellsword - 013177_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_CombatSpellsword()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Class,
            "CombatSpellsword");
        var dto = Helpers.GetDTO<ClassDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Class,
            "013177:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "BleedoutDefault").ShouldBe(Helpers.GetDTOField(dto, "BleedoutDefault"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "MaxTrainingLevel").ShouldBe(Helpers.GetDTOField(dto, "MaxTrainingLevel"));
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
        Helpers.GetSpriggitField(spriggit, "Teaches").ShouldBe(Helpers.GetDTOField(dto, "Teaches"));
        Helpers.GetSpriggitField(spriggit, "Unknown").ShouldBe(Helpers.GetDTOField(dto, "Unknown"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[10]").ShouldBe(Helpers.GetDTOField(dto, "Value[10]"));
        Helpers.GetSpriggitField(spriggit, "Value[11]").ShouldBe(Helpers.GetDTOField(dto, "Value[11]"));
        Helpers.GetSpriggitField(spriggit, "Value[12]").ShouldBe(Helpers.GetDTOField(dto, "Value[12]"));
        Helpers.GetSpriggitField(spriggit, "Value[13]").ShouldBe(Helpers.GetDTOField(dto, "Value[13]"));
        Helpers.GetSpriggitField(spriggit, "Value[14]").ShouldBe(Helpers.GetDTOField(dto, "Value[14]"));
        Helpers.GetSpriggitField(spriggit, "Value[15]").ShouldBe(Helpers.GetDTOField(dto, "Value[15]"));
        Helpers.GetSpriggitField(spriggit, "Value[16]").ShouldBe(Helpers.GetDTOField(dto, "Value[16]"));
        Helpers.GetSpriggitField(spriggit, "Value[17]").ShouldBe(Helpers.GetDTOField(dto, "Value[17]"));
        Helpers.GetSpriggitField(spriggit, "Value[18]").ShouldBe(Helpers.GetDTOField(dto, "Value[18]"));
        Helpers.GetSpriggitField(spriggit, "Value[19]").ShouldBe(Helpers.GetDTOField(dto, "Value[19]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[20]").ShouldBe(Helpers.GetDTOField(dto, "Value[20]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Value[4]").ShouldBe(Helpers.GetDTOField(dto, "Value[4]"));
        Helpers.GetSpriggitField(spriggit, "Value[5]").ShouldBe(Helpers.GetDTOField(dto, "Value[5]"));
        Helpers.GetSpriggitField(spriggit, "Value[6]").ShouldBe(Helpers.GetDTOField(dto, "Value[6]"));
        Helpers.GetSpriggitField(spriggit, "Value[7]").ShouldBe(Helpers.GetDTOField(dto, "Value[7]"));
        Helpers.GetSpriggitField(spriggit, "Value[8]").ShouldBe(Helpers.GetDTOField(dto, "Value[8]"));
        Helpers.GetSpriggitField(spriggit, "Value[9]").ShouldBe(Helpers.GetDTOField(dto, "Value[9]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "BleedoutDefault", "Description.TargetLanguage", "EditorID", "FormKey", "MaxTrainingLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Teaches", "Unknown", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[18]", "Value[19]", "Value[2]", "Value[20]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "BleedoutDefault", "Description.TargetLanguage", "EditorID", "FormKey", "MaxTrainingLevel", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Teaches", "Unknown", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[18]", "Value[19]", "Value[2]", "Value[20]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "01325D:Skyrim.esm")]
    [Trait("EditorID", "Bard")]
    [Trait("SpriggitFile", "Classes/Bard - 01325D_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_Bard()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Class,
            "Bard");
        var dto = Helpers.GetDTO<ClassDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Class,
            "01325D:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "BleedoutDefault").ShouldBe(Helpers.GetDTOField(dto, "BleedoutDefault"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
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
        Helpers.GetSpriggitField(spriggit, "Teaches").ShouldBe(Helpers.GetDTOField(dto, "Teaches"));
        Helpers.GetSpriggitField(spriggit, "Unknown").ShouldBe(Helpers.GetDTOField(dto, "Unknown"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[10]").ShouldBe(Helpers.GetDTOField(dto, "Value[10]"));
        Helpers.GetSpriggitField(spriggit, "Value[11]").ShouldBe(Helpers.GetDTOField(dto, "Value[11]"));
        Helpers.GetSpriggitField(spriggit, "Value[12]").ShouldBe(Helpers.GetDTOField(dto, "Value[12]"));
        Helpers.GetSpriggitField(spriggit, "Value[13]").ShouldBe(Helpers.GetDTOField(dto, "Value[13]"));
        Helpers.GetSpriggitField(spriggit, "Value[14]").ShouldBe(Helpers.GetDTOField(dto, "Value[14]"));
        Helpers.GetSpriggitField(spriggit, "Value[15]").ShouldBe(Helpers.GetDTOField(dto, "Value[15]"));
        Helpers.GetSpriggitField(spriggit, "Value[16]").ShouldBe(Helpers.GetDTOField(dto, "Value[16]"));
        Helpers.GetSpriggitField(spriggit, "Value[17]").ShouldBe(Helpers.GetDTOField(dto, "Value[17]"));
        Helpers.GetSpriggitField(spriggit, "Value[18]").ShouldBe(Helpers.GetDTOField(dto, "Value[18]"));
        Helpers.GetSpriggitField(spriggit, "Value[19]").ShouldBe(Helpers.GetDTOField(dto, "Value[19]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[20]").ShouldBe(Helpers.GetDTOField(dto, "Value[20]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Value[4]").ShouldBe(Helpers.GetDTOField(dto, "Value[4]"));
        Helpers.GetSpriggitField(spriggit, "Value[5]").ShouldBe(Helpers.GetDTOField(dto, "Value[5]"));
        Helpers.GetSpriggitField(spriggit, "Value[6]").ShouldBe(Helpers.GetDTOField(dto, "Value[6]"));
        Helpers.GetSpriggitField(spriggit, "Value[7]").ShouldBe(Helpers.GetDTOField(dto, "Value[7]"));
        Helpers.GetSpriggitField(spriggit, "Value[8]").ShouldBe(Helpers.GetDTOField(dto, "Value[8]"));
        Helpers.GetSpriggitField(spriggit, "Value[9]").ShouldBe(Helpers.GetDTOField(dto, "Value[9]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "BleedoutDefault", "Description.TargetLanguage", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Teaches", "Unknown", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[18]", "Value[19]", "Value[2]", "Value[20]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "BleedoutDefault", "Description.TargetLanguage", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Teaches", "Unknown", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[18]", "Value[19]", "Value[2]", "Value[20]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl");
    }
}