using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MiscItem.Fallout4;

public class Fallout4MiscItemSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "247E7F:Fallout4.esm")]
    [Trait("EditorID", "Debug_Components")]
    [Trait("SpriggitFile", "MiscItems/Debug_Components - 247E7F_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ShouldMatchSpriggitSample_Debug_Components()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MiscObject,
            "Debug_Components");
        var dto = Helpers.GetDTO<MiscObjectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MiscObject,
            "247E7F:Fallout4.esm");

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
        Helpers.GetSpriggitField(spriggit, "Count[23]").ShouldBe(Helpers.GetDTOField(dto, "Count[23]"));
        Helpers.GetSpriggitField(spriggit, "Count[24]").ShouldBe(Helpers.GetDTOField(dto, "Count[24]"));
        Helpers.GetSpriggitField(spriggit, "Count[25]").ShouldBe(Helpers.GetDTOField(dto, "Count[25]"));
        Helpers.GetSpriggitField(spriggit, "Count[26]").ShouldBe(Helpers.GetDTOField(dto, "Count[26]"));
        Helpers.GetSpriggitField(spriggit, "Count[27]").ShouldBe(Helpers.GetDTOField(dto, "Count[27]"));
        Helpers.GetSpriggitField(spriggit, "Count[28]").ShouldBe(Helpers.GetDTOField(dto, "Count[28]"));
        Helpers.GetSpriggitField(spriggit, "Count[29]").ShouldBe(Helpers.GetDTOField(dto, "Count[29]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[30]").ShouldBe(Helpers.GetDTOField(dto, "Count[30]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "Count[6]").ShouldBe(Helpers.GetDTOField(dto, "Count[6]"));
        Helpers.GetSpriggitField(spriggit, "Count[7]").ShouldBe(Helpers.GetDTOField(dto, "Count[7]"));
        Helpers.GetSpriggitField(spriggit, "Count[8]").ShouldBe(Helpers.GetDTOField(dto, "Count[8]"));
        Helpers.GetSpriggitField(spriggit, "Count[9]").ShouldBe(Helpers.GetDTOField(dto, "Count[9]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PreviewTransform").ShouldBe(Helpers.GetDTOField(dto, "PreviewTransform"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "Weight").ShouldBe(Helpers.GetDTOField(dto, "Weight"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Count[0]", "Count[1]", "Count[10]", "Count[11]", "Count[12]", "Count[13]", "Count[14]", "Count[15]", "Count[16]", "Count[17]", "Count[18]", "Count[19]", "Count[2]", "Count[20]", "Count[21]", "Count[22]", "Count[23]", "Count[24]", "Count[25]", "Count[26]", "Count[27]", "Count[28]", "Count[29]", "Count[3]", "Count[30]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "EditorID", "FormKey", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBounds.First", "ObjectBounds.Second", "PreviewTransform", "Value", "Version2", "VersionControl", "Weight");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Count[0]", "Count[1]", "Count[10]", "Count[11]", "Count[12]", "Count[13]", "Count[14]", "Count[15]", "Count[16]", "Count[17]", "Count[18]", "Count[19]", "Count[2]", "Count[20]", "Count[21]", "Count[22]", "Count[23]", "Count[24]", "Count[25]", "Count[26]", "Count[27]", "Count[28]", "Count[29]", "Count[3]", "Count[30]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "EditorID", "FormKey", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PreviewTransform", "Value", "Version2", "VersionControl", "Weight");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0A4754:Fallout4.esm")]
    [Trait("EditorID", "FFDiamondCity07Paper")]
    [Trait("SpriggitFile", "MiscItems/FFDiamondCity07Paper - 0A4754_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ShouldMatchSpriggitSample_FFDiamondCity07Paper()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MiscObject,
            "FFDiamondCity07Paper");
        var dto = Helpers.GetDTO<MiscObjectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MiscObject,
            "0A4754:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PreviewTransform").ShouldBe(Helpers.GetDTOField(dto, "PreviewTransform"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
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
        Helpers.GetSpriggitField(spriggit, "Weight").ShouldBe(Helpers.GetDTOField(dto, "Weight"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBounds.First", "ObjectBounds.Second", "PreviewTransform", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][10].MutagenObjectType", "VirtualMachineAdapter[0][10].Name", "VirtualMachineAdapter[0][10].Object", "VirtualMachineAdapter[0][11].MutagenObjectType", "VirtualMachineAdapter[0][11].Name", "VirtualMachineAdapter[0][11].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][4].Object", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][5].Object", "VirtualMachineAdapter[0][6].MutagenObjectType", "VirtualMachineAdapter[0][6].Name", "VirtualMachineAdapter[0][6].Object", "VirtualMachineAdapter[0][7].MutagenObjectType", "VirtualMachineAdapter[0][7].Name", "VirtualMachineAdapter[0][7].Object", "VirtualMachineAdapter[0][8].MutagenObjectType", "VirtualMachineAdapter[0][8].Name", "VirtualMachineAdapter[0][8].Object", "VirtualMachineAdapter[0][9].MutagenObjectType", "VirtualMachineAdapter[0][9].Name", "VirtualMachineAdapter[0][9].Object", "Weight");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PreviewTransform", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][10].MutagenObjectType", "VirtualMachineAdapter[0][10].Name", "VirtualMachineAdapter[0][10].Object", "VirtualMachineAdapter[0][11].MutagenObjectType", "VirtualMachineAdapter[0][11].Name", "VirtualMachineAdapter[0][11].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][4].Object", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][5].Object", "VirtualMachineAdapter[0][6].MutagenObjectType", "VirtualMachineAdapter[0][6].Name", "VirtualMachineAdapter[0][6].Object", "VirtualMachineAdapter[0][7].MutagenObjectType", "VirtualMachineAdapter[0][7].Name", "VirtualMachineAdapter[0][7].Object", "VirtualMachineAdapter[0][8].MutagenObjectType", "VirtualMachineAdapter[0][8].Name", "VirtualMachineAdapter[0][8].Object", "VirtualMachineAdapter[0][9].MutagenObjectType", "VirtualMachineAdapter[0][9].Name", "VirtualMachineAdapter[0][9].Object", "Weight");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "01F8F9:Fallout4.esm")]
    [Trait("EditorID", "FireExtinguisher01")]
    [Trait("SpriggitFile", "MiscItems/FireExtinguisher01 - 01F8F9_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ShouldMatchSpriggitSample_FireExtinguisher01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MiscObject,
            "FireExtinguisher01");
        var dto = Helpers.GetDTO<MiscObjectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MiscObject,
            "01F8F9:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Destructible.Count").ShouldBe(Helpers.GetDTOField(dto, "Destructible.Count"));
        Helpers.GetSpriggitField(spriggit, "Destructible.Data.DESTCount").ShouldBe(Helpers.GetDTOField(dto, "Destructible.Data.DESTCount"));
        Helpers.GetSpriggitField(spriggit, "Destructible.Data.Health").ShouldBe(Helpers.GetDTOField(dto, "Destructible.Data.Health"));
        Helpers.GetSpriggitField(spriggit, "Destructible[0].Count").ShouldBe(Helpers.GetDTOField(dto, "Destructible[0].Count"));
        Helpers.GetSpriggitField(spriggit, "Destructible[0].Explosion").ShouldBe(Helpers.GetDTOField(dto, "Destructible[0].Explosion"));
        Helpers.GetSpriggitField(spriggit, "Destructible[0].HealthPercent").ShouldBe(Helpers.GetDTOField(dto, "Destructible[0].HealthPercent"));
        Helpers.GetSpriggitField(spriggit, "Destructible[0].Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Destructible[0].Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Destructible[0].Model.File").ShouldBe(Helpers.GetDTOField(dto, "Destructible[0].Model.File"));
        Helpers.GetSpriggitField(spriggit, "Destructible[0].ModelDamageStage").ShouldBe(Helpers.GetDTOField(dto, "Destructible[0].ModelDamageStage"));
        Helpers.GetSpriggitField(spriggit, "Destructible[0].SelfDamagePerSecond").ShouldBe(Helpers.GetDTOField(dto, "Destructible[0].SelfDamagePerSecond"));
        Helpers.GetSpriggitField(spriggit, "Destructible[0][0]").ShouldBe(Helpers.GetDTOField(dto, "Destructible[0][0]"));
        Helpers.GetSpriggitField(spriggit, "Destructible[1].Index").ShouldBe(Helpers.GetDTOField(dto, "Destructible[1].Index"));
        Helpers.GetSpriggitField(spriggit, "Destructible[1].ModelDamageStage").ShouldBe(Helpers.GetDTOField(dto, "Destructible[1].ModelDamageStage"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PreviewTransform").ShouldBe(Helpers.GetDTOField(dto, "PreviewTransform"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "Weight").ShouldBe(Helpers.GetDTOField(dto, "Weight"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Count[0]", "Count[1]", "Count[2]", "Destructible.Count", "Destructible.Data.DESTCount", "Destructible.Data.Health", "Destructible[0].Count", "Destructible[0].Explosion", "Destructible[0].HealthPercent", "Destructible[0].Model.Data", "Destructible[0].Model.File", "Destructible[0].ModelDamageStage", "Destructible[0].SelfDamagePerSecond", "Destructible[0][0]", "Destructible[1].Index", "Destructible[1].ModelDamageStage", "EditorID", "FormKey", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBounds.First", "ObjectBounds.Second", "PreviewTransform", "Value", "Version2", "VersionControl", "Weight");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Count[0]", "Count[1]", "Count[2]", "Destructible.Count", "Destructible.Data.DESTCount", "Destructible.Data.Health", "Destructible[0].Count", "Destructible[0].Explosion", "Destructible[0].HealthPercent", "Destructible[0].Model.Data", "Destructible[0].Model.File", "Destructible[0].ModelDamageStage", "Destructible[0].SelfDamagePerSecond", "Destructible[0][0]", "Destructible[1].Index", "Destructible[1].ModelDamageStage", "EditorID", "FormKey", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PreviewTransform", "Value", "Version2", "VersionControl", "Weight");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "178B51:Fallout4.esm")]
    [Trait("EditorID", "BobbleHead_Agility")]
    [Trait("SpriggitFile", "MiscItems/BobbleHead_Agility - 178B51_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ShouldMatchSpriggitSample_BobbleHead_Agility()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MiscObject,
            "BobbleHead_Agility");
        var dto = Helpers.GetDTO<MiscObjectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MiscObject,
            "178B51:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FeaturedItemMessage").ShouldBe(Helpers.GetDTOField(dto, "FeaturedItemMessageFormKey"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PreviewTransform").ShouldBe(Helpers.GetDTOField(dto, "PreviewTransform"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Name"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FeaturedItemMessage", "FormKey", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBounds.First", "ObjectBounds.Second", "PreviewTransform", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].Data", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FeaturedItemMessageFormKey", "FormKey", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PreviewTransform", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].Data", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "04E3A2:Fallout4.esm")]
    [Trait("EditorID", "MS11GuidanceChip")]
    [Trait("SpriggitFile", "MiscItems/MS11GuidanceChip - 04E3A2_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ShouldMatchSpriggitSample_MS11GuidanceChip()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MiscObject,
            "MS11GuidanceChip");
        var dto = Helpers.GetDTO<MiscObjectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.MiscObject,
            "04E3A2:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PreviewTransform").ShouldBe(Helpers.GetDTOField(dto, "PreviewTransform"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Name"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Count[0]", "Count[1]", "EditorID", "FormKey", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBounds.First", "ObjectBounds.Second", "PreviewTransform", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].Data", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][2].Data", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Count[0]", "Count[1]", "EditorID", "FormKey", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PreviewTransform", "Value", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].Data", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][2].Data", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name");
    }
}