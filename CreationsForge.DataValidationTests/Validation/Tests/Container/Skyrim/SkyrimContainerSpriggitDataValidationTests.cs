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

        Helpers.GetSpriggitField(spriggit, "CloseSound").ShouldBe(Helpers.GetDTOField(dto, "CloseSound"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[10]").ShouldBe(Helpers.GetDTOField(dto, "Count[10]"));
        Helpers.GetSpriggitField(spriggit, "Count[11]").ShouldBe(Helpers.GetDTOField(dto, "Count[11]"));
        Helpers.GetSpriggitField(spriggit, "Count[12]").ShouldBe(Helpers.GetDTOField(dto, "Count[12]"));
        Helpers.GetSpriggitField(spriggit, "Count[13]").ShouldBe(Helpers.GetDTOField(dto, "Count[13]"));
        Helpers.GetSpriggitField(spriggit, "Count[14]").ShouldBe(Helpers.GetDTOField(dto, "Count[14]"));
        Helpers.GetSpriggitField(spriggit, "Count[15]").ShouldBe(Helpers.GetDTOField(dto, "Count[15]"));
        Helpers.GetSpriggitField(spriggit, "Count[16]").ShouldBe(Helpers.GetDTOField(dto, "Count[16]"));
        Helpers.GetSpriggitField(spriggit, "Count[17]").ShouldBe(Helpers.GetDTOField(dto, "Count[17]"));
        Helpers.GetSpriggitField(spriggit, "Count[18]").ShouldBe(Helpers.GetDTOField(dto, "Count[18]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "Count[6]").ShouldBe(Helpers.GetDTOField(dto, "Count[6]"));
        Helpers.GetSpriggitField(spriggit, "Count[7]").ShouldBe(Helpers.GetDTOField(dto, "Count[7]"));
        Helpers.GetSpriggitField(spriggit, "Count[8]").ShouldBe(Helpers.GetDTOField(dto, "Count[8]"));
        Helpers.GetSpriggitField(spriggit, "Count[9]").ShouldBe(Helpers.GetDTOField(dto, "Count[9]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[10]").ShouldBe(Helpers.GetDTOField(dto, "Item[10]"));
        Helpers.GetSpriggitField(spriggit, "Item[11]").ShouldBe(Helpers.GetDTOField(dto, "Item[11]"));
        Helpers.GetSpriggitField(spriggit, "Item[12]").ShouldBe(Helpers.GetDTOField(dto, "Item[12]"));
        Helpers.GetSpriggitField(spriggit, "Item[13]").ShouldBe(Helpers.GetDTOField(dto, "Item[13]"));
        Helpers.GetSpriggitField(spriggit, "Item[14]").ShouldBe(Helpers.GetDTOField(dto, "Item[14]"));
        Helpers.GetSpriggitField(spriggit, "Item[15]").ShouldBe(Helpers.GetDTOField(dto, "Item[15]"));
        Helpers.GetSpriggitField(spriggit, "Item[16]").ShouldBe(Helpers.GetDTOField(dto, "Item[16]"));
        Helpers.GetSpriggitField(spriggit, "Item[17]").ShouldBe(Helpers.GetDTOField(dto, "Item[17]"));
        Helpers.GetSpriggitField(spriggit, "Item[18]").ShouldBe(Helpers.GetDTOField(dto, "Item[18]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Item[5]").ShouldBe(Helpers.GetDTOField(dto, "Item[5]"));
        Helpers.GetSpriggitField(spriggit, "Item[6]").ShouldBe(Helpers.GetDTOField(dto, "Item[6]"));
        Helpers.GetSpriggitField(spriggit, "Item[7]").ShouldBe(Helpers.GetDTOField(dto, "Item[7]"));
        Helpers.GetSpriggitField(spriggit, "Item[8]").ShouldBe(Helpers.GetDTOField(dto, "Item[8]"));
        Helpers.GetSpriggitField(spriggit, "Item[9]").ShouldBe(Helpers.GetDTOField(dto, "Item[9]"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound").ShouldBe(Helpers.GetDTOField(dto, "OpenSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CloseSound", "Count[0]", "Count[1]", "Count[10]", "Count[11]", "Count[12]", "Count[13]", "Count[14]", "Count[15]", "Count[16]", "Count[17]", "Count[18]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[10]", "Item[11]", "Item[12]", "Item[13]", "Item[14]", "Item[15]", "Item[16]", "Item[17]", "Item[18]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Item[9]", "MajorRecordFlagsRaw", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CloseSound", "Count[0]", "Count[1]", "Count[10]", "Count[11]", "Count[12]", "Count[13]", "Count[14]", "Count[15]", "Count[16]", "Count[17]", "Count[18]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[10]", "Item[11]", "Item[12]", "Item[13]", "Item[14]", "Item[15]", "Item[16]", "Item[17]", "Item[18]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Item[9]", "MajorRecordFlags", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CloseSound").ShouldBe(Helpers.GetDTOField(dto, "CloseSound"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[10]").ShouldBe(Helpers.GetDTOField(dto, "Count[10]"));
        Helpers.GetSpriggitField(spriggit, "Count[11]").ShouldBe(Helpers.GetDTOField(dto, "Count[11]"));
        Helpers.GetSpriggitField(spriggit, "Count[12]").ShouldBe(Helpers.GetDTOField(dto, "Count[12]"));
        Helpers.GetSpriggitField(spriggit, "Count[13]").ShouldBe(Helpers.GetDTOField(dto, "Count[13]"));
        Helpers.GetSpriggitField(spriggit, "Count[14]").ShouldBe(Helpers.GetDTOField(dto, "Count[14]"));
        Helpers.GetSpriggitField(spriggit, "Count[15]").ShouldBe(Helpers.GetDTOField(dto, "Count[15]"));
        Helpers.GetSpriggitField(spriggit, "Count[16]").ShouldBe(Helpers.GetDTOField(dto, "Count[16]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "Count[6]").ShouldBe(Helpers.GetDTOField(dto, "Count[6]"));
        Helpers.GetSpriggitField(spriggit, "Count[7]").ShouldBe(Helpers.GetDTOField(dto, "Count[7]"));
        Helpers.GetSpriggitField(spriggit, "Count[8]").ShouldBe(Helpers.GetDTOField(dto, "Count[8]"));
        Helpers.GetSpriggitField(spriggit, "Count[9]").ShouldBe(Helpers.GetDTOField(dto, "Count[9]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[10]").ShouldBe(Helpers.GetDTOField(dto, "Item[10]"));
        Helpers.GetSpriggitField(spriggit, "Item[11]").ShouldBe(Helpers.GetDTOField(dto, "Item[11]"));
        Helpers.GetSpriggitField(spriggit, "Item[12]").ShouldBe(Helpers.GetDTOField(dto, "Item[12]"));
        Helpers.GetSpriggitField(spriggit, "Item[13]").ShouldBe(Helpers.GetDTOField(dto, "Item[13]"));
        Helpers.GetSpriggitField(spriggit, "Item[14]").ShouldBe(Helpers.GetDTOField(dto, "Item[14]"));
        Helpers.GetSpriggitField(spriggit, "Item[15]").ShouldBe(Helpers.GetDTOField(dto, "Item[15]"));
        Helpers.GetSpriggitField(spriggit, "Item[16]").ShouldBe(Helpers.GetDTOField(dto, "Item[16]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Item[5]").ShouldBe(Helpers.GetDTOField(dto, "Item[5]"));
        Helpers.GetSpriggitField(spriggit, "Item[6]").ShouldBe(Helpers.GetDTOField(dto, "Item[6]"));
        Helpers.GetSpriggitField(spriggit, "Item[7]").ShouldBe(Helpers.GetDTOField(dto, "Item[7]"));
        Helpers.GetSpriggitField(spriggit, "Item[8]").ShouldBe(Helpers.GetDTOField(dto, "Item[8]"));
        Helpers.GetSpriggitField(spriggit, "Item[9]").ShouldBe(Helpers.GetDTOField(dto, "Item[9]"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound").ShouldBe(Helpers.GetDTOField(dto, "OpenSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CloseSound", "Count[0]", "Count[1]", "Count[10]", "Count[11]", "Count[12]", "Count[13]", "Count[14]", "Count[15]", "Count[16]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[10]", "Item[11]", "Item[12]", "Item[13]", "Item[14]", "Item[15]", "Item[16]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Item[9]", "MajorRecordFlagsRaw", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CloseSound", "Count[0]", "Count[1]", "Count[10]", "Count[11]", "Count[12]", "Count[13]", "Count[14]", "Count[15]", "Count[16]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[10]", "Item[11]", "Item[12]", "Item[13]", "Item[14]", "Item[15]", "Item[16]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Item[9]", "MajorRecordFlags", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CloseSound").ShouldBe(Helpers.GetDTOField(dto, "CloseSound"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[10]").ShouldBe(Helpers.GetDTOField(dto, "Count[10]"));
        Helpers.GetSpriggitField(spriggit, "Count[11]").ShouldBe(Helpers.GetDTOField(dto, "Count[11]"));
        Helpers.GetSpriggitField(spriggit, "Count[12]").ShouldBe(Helpers.GetDTOField(dto, "Count[12]"));
        Helpers.GetSpriggitField(spriggit, "Count[13]").ShouldBe(Helpers.GetDTOField(dto, "Count[13]"));
        Helpers.GetSpriggitField(spriggit, "Count[14]").ShouldBe(Helpers.GetDTOField(dto, "Count[14]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "Count[6]").ShouldBe(Helpers.GetDTOField(dto, "Count[6]"));
        Helpers.GetSpriggitField(spriggit, "Count[7]").ShouldBe(Helpers.GetDTOField(dto, "Count[7]"));
        Helpers.GetSpriggitField(spriggit, "Count[8]").ShouldBe(Helpers.GetDTOField(dto, "Count[8]"));
        Helpers.GetSpriggitField(spriggit, "Count[9]").ShouldBe(Helpers.GetDTOField(dto, "Count[9]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[10]").ShouldBe(Helpers.GetDTOField(dto, "Item[10]"));
        Helpers.GetSpriggitField(spriggit, "Item[11]").ShouldBe(Helpers.GetDTOField(dto, "Item[11]"));
        Helpers.GetSpriggitField(spriggit, "Item[12]").ShouldBe(Helpers.GetDTOField(dto, "Item[12]"));
        Helpers.GetSpriggitField(spriggit, "Item[13]").ShouldBe(Helpers.GetDTOField(dto, "Item[13]"));
        Helpers.GetSpriggitField(spriggit, "Item[14]").ShouldBe(Helpers.GetDTOField(dto, "Item[14]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Item[5]").ShouldBe(Helpers.GetDTOField(dto, "Item[5]"));
        Helpers.GetSpriggitField(spriggit, "Item[6]").ShouldBe(Helpers.GetDTOField(dto, "Item[6]"));
        Helpers.GetSpriggitField(spriggit, "Item[7]").ShouldBe(Helpers.GetDTOField(dto, "Item[7]"));
        Helpers.GetSpriggitField(spriggit, "Item[8]").ShouldBe(Helpers.GetDTOField(dto, "Item[8]"));
        Helpers.GetSpriggitField(spriggit, "Item[9]").ShouldBe(Helpers.GetDTOField(dto, "Item[9]"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound").ShouldBe(Helpers.GetDTOField(dto, "OpenSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CloseSound", "Count[0]", "Count[1]", "Count[10]", "Count[11]", "Count[12]", "Count[13]", "Count[14]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[10]", "Item[11]", "Item[12]", "Item[13]", "Item[14]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Item[9]", "MajorRecordFlagsRaw", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CloseSound", "Count[0]", "Count[1]", "Count[10]", "Count[11]", "Count[12]", "Count[13]", "Count[14]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[10]", "Item[11]", "Item[12]", "Item[13]", "Item[14]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Item[9]", "MajorRecordFlags", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound").ShouldBe(Helpers.GetDTOField(dto, "OpenSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][1].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][1].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][2].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][2].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][3].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][3].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][3].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][3].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][3].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][4].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][4].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][4].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][4].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][4].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][4].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][5].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][5].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][5].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][5].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][5].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][5].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][6].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][6].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][6].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][6].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][6].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][6].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][7].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][7].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][7].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][7].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][7].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][7].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][8].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][8].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][8].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][8].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[1][8].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[1][8].Name"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Count[0]", "Count[1]", "Count[2]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[2]", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[1].Count", "VirtualMachineAdapter[1].Name", "VirtualMachineAdapter[1][0].MutagenObjectType", "VirtualMachineAdapter[1][0].Name", "VirtualMachineAdapter[1][0].Object", "VirtualMachineAdapter[1][1].Data", "VirtualMachineAdapter[1][1].MutagenObjectType", "VirtualMachineAdapter[1][1].Name", "VirtualMachineAdapter[1][2].Data", "VirtualMachineAdapter[1][2].MutagenObjectType", "VirtualMachineAdapter[1][2].Name", "VirtualMachineAdapter[1][3].Data", "VirtualMachineAdapter[1][3].MutagenObjectType", "VirtualMachineAdapter[1][3].Name", "VirtualMachineAdapter[1][4].Data", "VirtualMachineAdapter[1][4].MutagenObjectType", "VirtualMachineAdapter[1][4].Name", "VirtualMachineAdapter[1][5].Data", "VirtualMachineAdapter[1][5].MutagenObjectType", "VirtualMachineAdapter[1][5].Name", "VirtualMachineAdapter[1][6].Data", "VirtualMachineAdapter[1][6].MutagenObjectType", "VirtualMachineAdapter[1][6].Name", "VirtualMachineAdapter[1][7].MutagenObjectType", "VirtualMachineAdapter[1][7].Name", "VirtualMachineAdapter[1][7].Object", "VirtualMachineAdapter[1][8].Data", "VirtualMachineAdapter[1][8].MutagenObjectType", "VirtualMachineAdapter[1][8].Name");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Count[0]", "Count[1]", "Count[2]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[2]", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[1].Count", "VirtualMachineAdapter[1].Name", "VirtualMachineAdapter[1][0].MutagenObjectType", "VirtualMachineAdapter[1][0].Name", "VirtualMachineAdapter[1][0].Object", "VirtualMachineAdapter[1][1].Data", "VirtualMachineAdapter[1][1].MutagenObjectType", "VirtualMachineAdapter[1][1].Name", "VirtualMachineAdapter[1][2].Data", "VirtualMachineAdapter[1][2].MutagenObjectType", "VirtualMachineAdapter[1][2].Name", "VirtualMachineAdapter[1][3].Data", "VirtualMachineAdapter[1][3].MutagenObjectType", "VirtualMachineAdapter[1][3].Name", "VirtualMachineAdapter[1][4].Data", "VirtualMachineAdapter[1][4].MutagenObjectType", "VirtualMachineAdapter[1][4].Name", "VirtualMachineAdapter[1][5].Data", "VirtualMachineAdapter[1][5].MutagenObjectType", "VirtualMachineAdapter[1][5].Name", "VirtualMachineAdapter[1][6].Data", "VirtualMachineAdapter[1][6].MutagenObjectType", "VirtualMachineAdapter[1][6].Name", "VirtualMachineAdapter[1][7].MutagenObjectType", "VirtualMachineAdapter[1][7].Name", "VirtualMachineAdapter[1][7].Object", "VirtualMachineAdapter[1][8].Data", "VirtualMachineAdapter[1][8].MutagenObjectType", "VirtualMachineAdapter[1][8].Name");
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

        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[10]").ShouldBe(Helpers.GetDTOField(dto, "Count[10]"));
        Helpers.GetSpriggitField(spriggit, "Count[11]").ShouldBe(Helpers.GetDTOField(dto, "Count[11]"));
        Helpers.GetSpriggitField(spriggit, "Count[12]").ShouldBe(Helpers.GetDTOField(dto, "Count[12]"));
        Helpers.GetSpriggitField(spriggit, "Count[13]").ShouldBe(Helpers.GetDTOField(dto, "Count[13]"));
        Helpers.GetSpriggitField(spriggit, "Count[14]").ShouldBe(Helpers.GetDTOField(dto, "Count[14]"));
        Helpers.GetSpriggitField(spriggit, "Count[15]").ShouldBe(Helpers.GetDTOField(dto, "Count[15]"));
        Helpers.GetSpriggitField(spriggit, "Count[16]").ShouldBe(Helpers.GetDTOField(dto, "Count[16]"));
        Helpers.GetSpriggitField(spriggit, "Count[17]").ShouldBe(Helpers.GetDTOField(dto, "Count[17]"));
        Helpers.GetSpriggitField(spriggit, "Count[18]").ShouldBe(Helpers.GetDTOField(dto, "Count[18]"));
        Helpers.GetSpriggitField(spriggit, "Count[19]").ShouldBe(Helpers.GetDTOField(dto, "Count[19]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[20]").ShouldBe(Helpers.GetDTOField(dto, "Count[20]"));
        Helpers.GetSpriggitField(spriggit, "Count[21]").ShouldBe(Helpers.GetDTOField(dto, "Count[21]"));
        Helpers.GetSpriggitField(spriggit, "Count[22]").ShouldBe(Helpers.GetDTOField(dto, "Count[22]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "Count[6]").ShouldBe(Helpers.GetDTOField(dto, "Count[6]"));
        Helpers.GetSpriggitField(spriggit, "Count[7]").ShouldBe(Helpers.GetDTOField(dto, "Count[7]"));
        Helpers.GetSpriggitField(spriggit, "Count[8]").ShouldBe(Helpers.GetDTOField(dto, "Count[8]"));
        Helpers.GetSpriggitField(spriggit, "Count[9]").ShouldBe(Helpers.GetDTOField(dto, "Count[9]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[10]").ShouldBe(Helpers.GetDTOField(dto, "Item[10]"));
        Helpers.GetSpriggitField(spriggit, "Item[11]").ShouldBe(Helpers.GetDTOField(dto, "Item[11]"));
        Helpers.GetSpriggitField(spriggit, "Item[12]").ShouldBe(Helpers.GetDTOField(dto, "Item[12]"));
        Helpers.GetSpriggitField(spriggit, "Item[13]").ShouldBe(Helpers.GetDTOField(dto, "Item[13]"));
        Helpers.GetSpriggitField(spriggit, "Item[14]").ShouldBe(Helpers.GetDTOField(dto, "Item[14]"));
        Helpers.GetSpriggitField(spriggit, "Item[15]").ShouldBe(Helpers.GetDTOField(dto, "Item[15]"));
        Helpers.GetSpriggitField(spriggit, "Item[16]").ShouldBe(Helpers.GetDTOField(dto, "Item[16]"));
        Helpers.GetSpriggitField(spriggit, "Item[17]").ShouldBe(Helpers.GetDTOField(dto, "Item[17]"));
        Helpers.GetSpriggitField(spriggit, "Item[18]").ShouldBe(Helpers.GetDTOField(dto, "Item[18]"));
        Helpers.GetSpriggitField(spriggit, "Item[19]").ShouldBe(Helpers.GetDTOField(dto, "Item[19]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[20]").ShouldBe(Helpers.GetDTOField(dto, "Item[20]"));
        Helpers.GetSpriggitField(spriggit, "Item[21]").ShouldBe(Helpers.GetDTOField(dto, "Item[21]"));
        Helpers.GetSpriggitField(spriggit, "Item[22]").ShouldBe(Helpers.GetDTOField(dto, "Item[22]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Item[5]").ShouldBe(Helpers.GetDTOField(dto, "Item[5]"));
        Helpers.GetSpriggitField(spriggit, "Item[6]").ShouldBe(Helpers.GetDTOField(dto, "Item[6]"));
        Helpers.GetSpriggitField(spriggit, "Item[7]").ShouldBe(Helpers.GetDTOField(dto, "Item[7]"));
        Helpers.GetSpriggitField(spriggit, "Item[8]").ShouldBe(Helpers.GetDTOField(dto, "Item[8]"));
        Helpers.GetSpriggitField(spriggit, "Item[9]").ShouldBe(Helpers.GetDTOField(dto, "Item[9]"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Count[0]", "Count[1]", "Count[10]", "Count[11]", "Count[12]", "Count[13]", "Count[14]", "Count[15]", "Count[16]", "Count[17]", "Count[18]", "Count[19]", "Count[2]", "Count[20]", "Count[21]", "Count[22]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[10]", "Item[11]", "Item[12]", "Item[13]", "Item[14]", "Item[15]", "Item[16]", "Item[17]", "Item[18]", "Item[19]", "Item[2]", "Item[20]", "Item[21]", "Item[22]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Item[9]", "MajorRecordFlagsRaw", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Count[0]", "Count[1]", "Count[10]", "Count[11]", "Count[12]", "Count[13]", "Count[14]", "Count[15]", "Count[16]", "Count[17]", "Count[18]", "Count[19]", "Count[2]", "Count[20]", "Count[21]", "Count[22]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[10]", "Item[11]", "Item[12]", "Item[13]", "Item[14]", "Item[15]", "Item[16]", "Item[17]", "Item[18]", "Item[19]", "Item[2]", "Item[20]", "Item[21]", "Item[22]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Item[9]", "MajorRecordFlags", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "Version2", "VersionControl");
    }
}