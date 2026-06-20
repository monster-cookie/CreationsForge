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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["BleedoutDefault"].ShouldBe(dtoFields["BleedoutDefault"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["MaxTrainingLevel"].ShouldBe(dtoFields["MaxTrainingLevel"]);
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
        spriggitFields["Teaches"].ShouldBe(dtoFields["Teaches"]);
        spriggitFields["Unknown"].ShouldBe(dtoFields["Unknown"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[10]"].ShouldBe(dtoFields["Value[10]"]);
        spriggitFields["Value[11]"].ShouldBe(dtoFields["Value[11]"]);
        spriggitFields["Value[12]"].ShouldBe(dtoFields["Value[12]"]);
        spriggitFields["Value[13]"].ShouldBe(dtoFields["Value[13]"]);
        spriggitFields["Value[14]"].ShouldBe(dtoFields["Value[14]"]);
        spriggitFields["Value[15]"].ShouldBe(dtoFields["Value[15]"]);
        spriggitFields["Value[16]"].ShouldBe(dtoFields["Value[16]"]);
        spriggitFields["Value[17]"].ShouldBe(dtoFields["Value[17]"]);
        spriggitFields["Value[18]"].ShouldBe(dtoFields["Value[18]"]);
        spriggitFields["Value[19]"].ShouldBe(dtoFields["Value[19]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[20]"].ShouldBe(dtoFields["Value[20]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Value[4]"].ShouldBe(dtoFields["Value[4]"]);
        spriggitFields["Value[5]"].ShouldBe(dtoFields["Value[5]"]);
        spriggitFields["Value[6]"].ShouldBe(dtoFields["Value[6]"]);
        spriggitFields["Value[7]"].ShouldBe(dtoFields["Value[7]"]);
        spriggitFields["Value[8]"].ShouldBe(dtoFields["Value[8]"]);
        spriggitFields["Value[9]"].ShouldBe(dtoFields["Value[9]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VoicePoints"].ShouldBe(dtoFields["VoicePoints"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["BleedoutDefault"].ShouldBe(dtoFields["BleedoutDefault"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["MaxTrainingLevel"].ShouldBe(dtoFields["MaxTrainingLevel"]);
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
        spriggitFields["Teaches"].ShouldBe(dtoFields["Teaches"]);
        spriggitFields["Unknown"].ShouldBe(dtoFields["Unknown"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[10]"].ShouldBe(dtoFields["Value[10]"]);
        spriggitFields["Value[11]"].ShouldBe(dtoFields["Value[11]"]);
        spriggitFields["Value[12]"].ShouldBe(dtoFields["Value[12]"]);
        spriggitFields["Value[13]"].ShouldBe(dtoFields["Value[13]"]);
        spriggitFields["Value[14]"].ShouldBe(dtoFields["Value[14]"]);
        spriggitFields["Value[15]"].ShouldBe(dtoFields["Value[15]"]);
        spriggitFields["Value[16]"].ShouldBe(dtoFields["Value[16]"]);
        spriggitFields["Value[17]"].ShouldBe(dtoFields["Value[17]"]);
        spriggitFields["Value[18]"].ShouldBe(dtoFields["Value[18]"]);
        spriggitFields["Value[19]"].ShouldBe(dtoFields["Value[19]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[20]"].ShouldBe(dtoFields["Value[20]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Value[4]"].ShouldBe(dtoFields["Value[4]"]);
        spriggitFields["Value[5]"].ShouldBe(dtoFields["Value[5]"]);
        spriggitFields["Value[6]"].ShouldBe(dtoFields["Value[6]"]);
        spriggitFields["Value[7]"].ShouldBe(dtoFields["Value[7]"]);
        spriggitFields["Value[8]"].ShouldBe(dtoFields["Value[8]"]);
        spriggitFields["Value[9]"].ShouldBe(dtoFields["Value[9]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VoicePoints"].ShouldBe(dtoFields["VoicePoints"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["BleedoutDefault"].ShouldBe(dtoFields["BleedoutDefault"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
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
        spriggitFields["Teaches"].ShouldBe(dtoFields["Teaches"]);
        spriggitFields["Unknown"].ShouldBe(dtoFields["Unknown"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[10]"].ShouldBe(dtoFields["Value[10]"]);
        spriggitFields["Value[11]"].ShouldBe(dtoFields["Value[11]"]);
        spriggitFields["Value[12]"].ShouldBe(dtoFields["Value[12]"]);
        spriggitFields["Value[13]"].ShouldBe(dtoFields["Value[13]"]);
        spriggitFields["Value[14]"].ShouldBe(dtoFields["Value[14]"]);
        spriggitFields["Value[15]"].ShouldBe(dtoFields["Value[15]"]);
        spriggitFields["Value[16]"].ShouldBe(dtoFields["Value[16]"]);
        spriggitFields["Value[17]"].ShouldBe(dtoFields["Value[17]"]);
        spriggitFields["Value[18]"].ShouldBe(dtoFields["Value[18]"]);
        spriggitFields["Value[19]"].ShouldBe(dtoFields["Value[19]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[20]"].ShouldBe(dtoFields["Value[20]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Value[4]"].ShouldBe(dtoFields["Value[4]"]);
        spriggitFields["Value[5]"].ShouldBe(dtoFields["Value[5]"]);
        spriggitFields["Value[6]"].ShouldBe(dtoFields["Value[6]"]);
        spriggitFields["Value[7]"].ShouldBe(dtoFields["Value[7]"]);
        spriggitFields["Value[8]"].ShouldBe(dtoFields["Value[8]"]);
        spriggitFields["Value[9]"].ShouldBe(dtoFields["Value[9]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VoicePoints"].ShouldBe(dtoFields["VoicePoints"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["BleedoutDefault"].ShouldBe(dtoFields["BleedoutDefault"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["MaxTrainingLevel"].ShouldBe(dtoFields["MaxTrainingLevel"]);
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
        spriggitFields["Teaches"].ShouldBe(dtoFields["Teaches"]);
        spriggitFields["Unknown"].ShouldBe(dtoFields["Unknown"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[10]"].ShouldBe(dtoFields["Value[10]"]);
        spriggitFields["Value[11]"].ShouldBe(dtoFields["Value[11]"]);
        spriggitFields["Value[12]"].ShouldBe(dtoFields["Value[12]"]);
        spriggitFields["Value[13]"].ShouldBe(dtoFields["Value[13]"]);
        spriggitFields["Value[14]"].ShouldBe(dtoFields["Value[14]"]);
        spriggitFields["Value[15]"].ShouldBe(dtoFields["Value[15]"]);
        spriggitFields["Value[16]"].ShouldBe(dtoFields["Value[16]"]);
        spriggitFields["Value[17]"].ShouldBe(dtoFields["Value[17]"]);
        spriggitFields["Value[18]"].ShouldBe(dtoFields["Value[18]"]);
        spriggitFields["Value[19]"].ShouldBe(dtoFields["Value[19]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[20]"].ShouldBe(dtoFields["Value[20]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Value[4]"].ShouldBe(dtoFields["Value[4]"]);
        spriggitFields["Value[5]"].ShouldBe(dtoFields["Value[5]"]);
        spriggitFields["Value[6]"].ShouldBe(dtoFields["Value[6]"]);
        spriggitFields["Value[7]"].ShouldBe(dtoFields["Value[7]"]);
        spriggitFields["Value[8]"].ShouldBe(dtoFields["Value[8]"]);
        spriggitFields["Value[9]"].ShouldBe(dtoFields["Value[9]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["BleedoutDefault"].ShouldBe(dtoFields["BleedoutDefault"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
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
        spriggitFields["Teaches"].ShouldBe(dtoFields["Teaches"]);
        spriggitFields["Unknown"].ShouldBe(dtoFields["Unknown"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[10]"].ShouldBe(dtoFields["Value[10]"]);
        spriggitFields["Value[11]"].ShouldBe(dtoFields["Value[11]"]);
        spriggitFields["Value[12]"].ShouldBe(dtoFields["Value[12]"]);
        spriggitFields["Value[13]"].ShouldBe(dtoFields["Value[13]"]);
        spriggitFields["Value[14]"].ShouldBe(dtoFields["Value[14]"]);
        spriggitFields["Value[15]"].ShouldBe(dtoFields["Value[15]"]);
        spriggitFields["Value[16]"].ShouldBe(dtoFields["Value[16]"]);
        spriggitFields["Value[17]"].ShouldBe(dtoFields["Value[17]"]);
        spriggitFields["Value[18]"].ShouldBe(dtoFields["Value[18]"]);
        spriggitFields["Value[19]"].ShouldBe(dtoFields["Value[19]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[20]"].ShouldBe(dtoFields["Value[20]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Value[4]"].ShouldBe(dtoFields["Value[4]"]);
        spriggitFields["Value[5]"].ShouldBe(dtoFields["Value[5]"]);
        spriggitFields["Value[6]"].ShouldBe(dtoFields["Value[6]"]);
        spriggitFields["Value[7]"].ShouldBe(dtoFields["Value[7]"]);
        spriggitFields["Value[8]"].ShouldBe(dtoFields["Value[8]"]);
        spriggitFields["Value[9]"].ShouldBe(dtoFields["Value[9]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
