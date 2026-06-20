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
        spriggitFields["Count[23]"].ShouldBe(dtoFields["Count[23]"]);
        spriggitFields["Count[24]"].ShouldBe(dtoFields["Count[24]"]);
        spriggitFields["Count[25]"].ShouldBe(dtoFields["Count[25]"]);
        spriggitFields["Count[26]"].ShouldBe(dtoFields["Count[26]"]);
        spriggitFields["Count[27]"].ShouldBe(dtoFields["Count[27]"]);
        spriggitFields["Count[28]"].ShouldBe(dtoFields["Count[28]"]);
        spriggitFields["Count[29]"].ShouldBe(dtoFields["Count[29]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[30]"].ShouldBe(dtoFields["Count[30]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["Count[6]"].ShouldBe(dtoFields["Count[6]"]);
        spriggitFields["Count[7]"].ShouldBe(dtoFields["Count[7]"]);
        spriggitFields["Count[8]"].ShouldBe(dtoFields["Count[8]"]);
        spriggitFields["Count[9]"].ShouldBe(dtoFields["Count[9]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["PreviewTransform"].ShouldBe(dtoFields["PreviewTransform"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["PreviewTransform"].ShouldBe(dtoFields["PreviewTransform"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
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
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Destructible.Count"].ShouldBe(dtoFields["Destructible.Count"]);
        spriggitFields["Destructible.Data.DESTCount"].ShouldBe(dtoFields["Destructible.Data.DESTCount"]);
        spriggitFields["Destructible.Data.Health"].ShouldBe(dtoFields["Destructible.Data.Health"]);
        spriggitFields["Destructible[0].Count"].ShouldBe(dtoFields["Destructible[0].Count"]);
        spriggitFields["Destructible[0].Explosion"].ShouldBe(dtoFields["Destructible[0].Explosion"]);
        spriggitFields["Destructible[0].HealthPercent"].ShouldBe(dtoFields["Destructible[0].HealthPercent"]);
        spriggitFields["Destructible[0].Model.Data"].ShouldBe(dtoFields["Destructible[0].Model.Data"]);
        spriggitFields["Destructible[0].Model.File"].ShouldBe(dtoFields["Destructible[0].Model.File"]);
        spriggitFields["Destructible[0].ModelDamageStage"].ShouldBe(dtoFields["Destructible[0].ModelDamageStage"]);
        spriggitFields["Destructible[0].SelfDamagePerSecond"].ShouldBe(dtoFields["Destructible[0].SelfDamagePerSecond"]);
        spriggitFields["Destructible[0][0]"].ShouldBe(dtoFields["Destructible[0][0]"]);
        spriggitFields["Destructible[1].Index"].ShouldBe(dtoFields["Destructible[1].Index"]);
        spriggitFields["Destructible[1].ModelDamageStage"].ShouldBe(dtoFields["Destructible[1].ModelDamageStage"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["PreviewTransform"].ShouldBe(dtoFields["PreviewTransform"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FeaturedItemMessage"].ShouldBe(dtoFields["FeaturedItemMessageFormKey"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["PreviewTransform"].ShouldBe(dtoFields["PreviewTransform"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["VirtualMachineAdapter[0][1].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Data"]);
        spriggitFields["VirtualMachineAdapter[0][1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Name"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["PreviewTransform"].ShouldBe(dtoFields["PreviewTransform"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["VirtualMachineAdapter[0][1].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Data"]);
        spriggitFields["VirtualMachineAdapter[0][1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Name"]);
        spriggitFields["VirtualMachineAdapter[0][2].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Data"]);
        spriggitFields["VirtualMachineAdapter[0][2].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Name"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
