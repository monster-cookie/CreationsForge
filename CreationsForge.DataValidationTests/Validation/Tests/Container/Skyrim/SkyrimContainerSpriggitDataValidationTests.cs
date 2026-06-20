using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Container.Skyrim;

public class SkyrimContainerSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "02065B:Skyrim.esm")]
    [Trait("EditorID", "TreasFalmerChestBoss")]
    [Trait("SpriggitFile", "Containers/TreasFalmerChestBoss - 02065B_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ShouldMatchSpriggitSample_TreasFalmerChestBoss()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Container,
            "TreasFalmerChestBoss");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Container,
            "02065B:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CloseSound"].ShouldBe(dtoFields["CloseSound"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[10]"].ShouldBe(dtoFields["Count[10]"]);
        spriggitFields["Count[11]"].ShouldBe(dtoFields["Count[11]"]);
        spriggitFields["Count[12]"].ShouldBe(dtoFields["Count[12]"]);
        spriggitFields["Count[13]"].ShouldBe(dtoFields["Count[13]"]);
        spriggitFields["Count[14]"].ShouldBe(dtoFields["Count[14]"]);
        spriggitFields["Count[15]"].ShouldBe(dtoFields["Count[15]"]);
        spriggitFields["Count[16]"].ShouldBe(dtoFields["Count[16]"]);
        spriggitFields["Count[17]"].ShouldBe(dtoFields["Count[17]"]);
        spriggitFields["Count[18]"].ShouldBe(dtoFields["Count[18]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["Count[6]"].ShouldBe(dtoFields["Count[6]"]);
        spriggitFields["Count[7]"].ShouldBe(dtoFields["Count[7]"]);
        spriggitFields["Count[8]"].ShouldBe(dtoFields["Count[8]"]);
        spriggitFields["Count[9]"].ShouldBe(dtoFields["Count[9]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[10]"].ShouldBe(dtoFields["Item[10]"]);
        spriggitFields["Item[11]"].ShouldBe(dtoFields["Item[11]"]);
        spriggitFields["Item[12]"].ShouldBe(dtoFields["Item[12]"]);
        spriggitFields["Item[13]"].ShouldBe(dtoFields["Item[13]"]);
        spriggitFields["Item[14]"].ShouldBe(dtoFields["Item[14]"]);
        spriggitFields["Item[15]"].ShouldBe(dtoFields["Item[15]"]);
        spriggitFields["Item[16]"].ShouldBe(dtoFields["Item[16]"]);
        spriggitFields["Item[17]"].ShouldBe(dtoFields["Item[17]"]);
        spriggitFields["Item[18]"].ShouldBe(dtoFields["Item[18]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Item[5]"].ShouldBe(dtoFields["Item[5]"]);
        spriggitFields["Item[6]"].ShouldBe(dtoFields["Item[6]"]);
        spriggitFields["Item[7]"].ShouldBe(dtoFields["Item[7]"]);
        spriggitFields["Item[8]"].ShouldBe(dtoFields["Item[8]"]);
        spriggitFields["Item[9]"].ShouldBe(dtoFields["Item[9]"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound"].ShouldBe(dtoFields["OpenSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "0B1176:Skyrim.esm")]
    [Trait("EditorID", "TreasFalmerChestBossDwarven")]
    [Trait("SpriggitFile", "Containers/TreasFalmerChestBossDwarven - 0B1176_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ShouldMatchSpriggitSample_TreasFalmerChestBossDwarven()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Container,
            "TreasFalmerChestBossDwarven");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Container,
            "0B1176:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CloseSound"].ShouldBe(dtoFields["CloseSound"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[10]"].ShouldBe(dtoFields["Count[10]"]);
        spriggitFields["Count[11]"].ShouldBe(dtoFields["Count[11]"]);
        spriggitFields["Count[12]"].ShouldBe(dtoFields["Count[12]"]);
        spriggitFields["Count[13]"].ShouldBe(dtoFields["Count[13]"]);
        spriggitFields["Count[14]"].ShouldBe(dtoFields["Count[14]"]);
        spriggitFields["Count[15]"].ShouldBe(dtoFields["Count[15]"]);
        spriggitFields["Count[16]"].ShouldBe(dtoFields["Count[16]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["Count[6]"].ShouldBe(dtoFields["Count[6]"]);
        spriggitFields["Count[7]"].ShouldBe(dtoFields["Count[7]"]);
        spriggitFields["Count[8]"].ShouldBe(dtoFields["Count[8]"]);
        spriggitFields["Count[9]"].ShouldBe(dtoFields["Count[9]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[10]"].ShouldBe(dtoFields["Item[10]"]);
        spriggitFields["Item[11]"].ShouldBe(dtoFields["Item[11]"]);
        spriggitFields["Item[12]"].ShouldBe(dtoFields["Item[12]"]);
        spriggitFields["Item[13]"].ShouldBe(dtoFields["Item[13]"]);
        spriggitFields["Item[14]"].ShouldBe(dtoFields["Item[14]"]);
        spriggitFields["Item[15]"].ShouldBe(dtoFields["Item[15]"]);
        spriggitFields["Item[16]"].ShouldBe(dtoFields["Item[16]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Item[5]"].ShouldBe(dtoFields["Item[5]"]);
        spriggitFields["Item[6]"].ShouldBe(dtoFields["Item[6]"]);
        spriggitFields["Item[7]"].ShouldBe(dtoFields["Item[7]"]);
        spriggitFields["Item[8]"].ShouldBe(dtoFields["Item[8]"]);
        spriggitFields["Item[9]"].ShouldBe(dtoFields["Item[9]"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound"].ShouldBe(dtoFields["OpenSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "020659:Skyrim.esm")]
    [Trait("EditorID", "TreasFalmerChest")]
    [Trait("SpriggitFile", "Containers/TreasFalmerChest - 020659_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ShouldMatchSpriggitSample_TreasFalmerChest()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Container,
            "TreasFalmerChest");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Container,
            "020659:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CloseSound"].ShouldBe(dtoFields["CloseSound"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[10]"].ShouldBe(dtoFields["Count[10]"]);
        spriggitFields["Count[11]"].ShouldBe(dtoFields["Count[11]"]);
        spriggitFields["Count[12]"].ShouldBe(dtoFields["Count[12]"]);
        spriggitFields["Count[13]"].ShouldBe(dtoFields["Count[13]"]);
        spriggitFields["Count[14]"].ShouldBe(dtoFields["Count[14]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["Count[6]"].ShouldBe(dtoFields["Count[6]"]);
        spriggitFields["Count[7]"].ShouldBe(dtoFields["Count[7]"]);
        spriggitFields["Count[8]"].ShouldBe(dtoFields["Count[8]"]);
        spriggitFields["Count[9]"].ShouldBe(dtoFields["Count[9]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[10]"].ShouldBe(dtoFields["Item[10]"]);
        spriggitFields["Item[11]"].ShouldBe(dtoFields["Item[11]"]);
        spriggitFields["Item[12]"].ShouldBe(dtoFields["Item[12]"]);
        spriggitFields["Item[13]"].ShouldBe(dtoFields["Item[13]"]);
        spriggitFields["Item[14]"].ShouldBe(dtoFields["Item[14]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Item[5]"].ShouldBe(dtoFields["Item[5]"]);
        spriggitFields["Item[6]"].ShouldBe(dtoFields["Item[6]"]);
        spriggitFields["Item[7]"].ShouldBe(dtoFields["Item[7]"]);
        spriggitFields["Item[8]"].ShouldBe(dtoFields["Item[8]"]);
        spriggitFields["Item[9]"].ShouldBe(dtoFields["Item[9]"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound"].ShouldBe(dtoFields["OpenSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "0A918C:Skyrim.esm")]
    [Trait("EditorID", "BeeHive")]
    [Trait("SpriggitFile", "Containers/BeeHive - 0A918C_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ShouldMatchSpriggitSample_BeeHive()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Container,
            "BeeHive");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Container,
            "0A918C:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound"].ShouldBe(dtoFields["OpenSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[1].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[1].Count"]);
        spriggitFields["VirtualMachineAdapter[1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1].Name"]);
        spriggitFields["VirtualMachineAdapter[1][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[1][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[1][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1][0].Name"]);
        spriggitFields["VirtualMachineAdapter[1][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[1][0].Object"]);
        spriggitFields["VirtualMachineAdapter[1][1].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[1][1].Data"]);
        spriggitFields["VirtualMachineAdapter[1][1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[1][1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[1][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1][1].Name"]);
        spriggitFields["VirtualMachineAdapter[1][2].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[1][2].Data"]);
        spriggitFields["VirtualMachineAdapter[1][2].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[1][2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[1][2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1][2].Name"]);
        spriggitFields["VirtualMachineAdapter[1][3].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[1][3].Data"]);
        spriggitFields["VirtualMachineAdapter[1][3].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[1][3].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[1][3].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1][3].Name"]);
        spriggitFields["VirtualMachineAdapter[1][4].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[1][4].Data"]);
        spriggitFields["VirtualMachineAdapter[1][4].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[1][4].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[1][4].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1][4].Name"]);
        spriggitFields["VirtualMachineAdapter[1][5].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[1][5].Data"]);
        spriggitFields["VirtualMachineAdapter[1][5].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[1][5].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[1][5].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1][5].Name"]);
        spriggitFields["VirtualMachineAdapter[1][6].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[1][6].Data"]);
        spriggitFields["VirtualMachineAdapter[1][6].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[1][6].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[1][6].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1][6].Name"]);
        spriggitFields["VirtualMachineAdapter[1][7].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[1][7].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[1][7].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1][7].Name"]);
        spriggitFields["VirtualMachineAdapter[1][7].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[1][7].Object"]);
        spriggitFields["VirtualMachineAdapter[1][8].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[1][8].Data"]);
        spriggitFields["VirtualMachineAdapter[1][8].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[1][8].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[1][8].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[1][8].Name"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "07434B:Skyrim.esm")]
    [Trait("EditorID", "MerchantCaravanAChest")]
    [Trait("SpriggitFile", "Containers/MerchantCaravanAChest - 07434B_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ShouldMatchSpriggitSample_MerchantCaravanAChest()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Container,
            "MerchantCaravanAChest");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Container,
            "07434B:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[10]"].ShouldBe(dtoFields["Count[10]"]);
        spriggitFields["Count[11]"].ShouldBe(dtoFields["Count[11]"]);
        spriggitFields["Count[12]"].ShouldBe(dtoFields["Count[12]"]);
        spriggitFields["Count[13]"].ShouldBe(dtoFields["Count[13]"]);
        spriggitFields["Count[14]"].ShouldBe(dtoFields["Count[14]"]);
        spriggitFields["Count[15]"].ShouldBe(dtoFields["Count[15]"]);
        spriggitFields["Count[16]"].ShouldBe(dtoFields["Count[16]"]);
        spriggitFields["Count[17]"].ShouldBe(dtoFields["Count[17]"]);
        spriggitFields["Count[18]"].ShouldBe(dtoFields["Count[18]"]);
        spriggitFields["Count[19]"].ShouldBe(dtoFields["Count[19]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[20]"].ShouldBe(dtoFields["Count[20]"]);
        spriggitFields["Count[21]"].ShouldBe(dtoFields["Count[21]"]);
        spriggitFields["Count[22]"].ShouldBe(dtoFields["Count[22]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["Count[6]"].ShouldBe(dtoFields["Count[6]"]);
        spriggitFields["Count[7]"].ShouldBe(dtoFields["Count[7]"]);
        spriggitFields["Count[8]"].ShouldBe(dtoFields["Count[8]"]);
        spriggitFields["Count[9]"].ShouldBe(dtoFields["Count[9]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[10]"].ShouldBe(dtoFields["Item[10]"]);
        spriggitFields["Item[11]"].ShouldBe(dtoFields["Item[11]"]);
        spriggitFields["Item[12]"].ShouldBe(dtoFields["Item[12]"]);
        spriggitFields["Item[13]"].ShouldBe(dtoFields["Item[13]"]);
        spriggitFields["Item[14]"].ShouldBe(dtoFields["Item[14]"]);
        spriggitFields["Item[15]"].ShouldBe(dtoFields["Item[15]"]);
        spriggitFields["Item[16]"].ShouldBe(dtoFields["Item[16]"]);
        spriggitFields["Item[17]"].ShouldBe(dtoFields["Item[17]"]);
        spriggitFields["Item[18]"].ShouldBe(dtoFields["Item[18]"]);
        spriggitFields["Item[19]"].ShouldBe(dtoFields["Item[19]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[20]"].ShouldBe(dtoFields["Item[20]"]);
        spriggitFields["Item[21]"].ShouldBe(dtoFields["Item[21]"]);
        spriggitFields["Item[22]"].ShouldBe(dtoFields["Item[22]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Item[5]"].ShouldBe(dtoFields["Item[5]"]);
        spriggitFields["Item[6]"].ShouldBe(dtoFields["Item[6]"]);
        spriggitFields["Item[7]"].ShouldBe(dtoFields["Item[7]"]);
        spriggitFields["Item[8]"].ShouldBe(dtoFields["Item[8]"]);
        spriggitFields["Item[9]"].ShouldBe(dtoFields["Item[9]"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
