using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MiscItem.Skyrim;

public class SkyrimMiscItemSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0D0756:Skyrim.esm")]
    [Trait("EditorID", "MGRDragonHeartScales")]
    [Trait("SpriggitFile", "MiscItems/MGRDragonHeartScales - 0D0756_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ShouldMatchSpriggitSample_MGRDragonHeartScales()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MiscItem,
            "MGRDragonHeartScales");
        var dto = Helpers.GetDTO<MiscItemDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MiscItem,
            "0D0756:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Count"].ShouldBe(dtoFields["Model.Count"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model[0].Name"].ShouldBe(dtoFields["Model[0].Name"]);
        spriggitFields["Model[0].NewTexture"].ShouldBe(dtoFields["Model[0].NewTexture"]);
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
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "06F993:Skyrim.esm")]
    [Trait("EditorID", "Firewood01")]
    [Trait("SpriggitFile", "MiscItems/Firewood01 - 06F993_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ShouldMatchSpriggitSample_Firewood01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MiscItem,
            "Firewood01");
        var dto = Helpers.GetDTO<MiscItemDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MiscItem,
            "06F993:Skyrim.esm");

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
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0D4BE7:Skyrim.esm")]
    [Trait("EditorID", "FoxPeltSnow")]
    [Trait("SpriggitFile", "MiscItems/FoxPeltSnow - 0D4BE7_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ShouldMatchSpriggitSample_FoxPeltSnow()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MiscItem,
            "FoxPeltSnow");
        var dto = Helpers.GetDTO<MiscItemDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MiscItem,
            "0D4BE7:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Count"].ShouldBe(dtoFields["Model.Count"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model[0].Name"].ShouldBe(dtoFields["Model[0].Name"]);
        spriggitFields["Model[0].NewTexture"].ShouldBe(dtoFields["Model[0].NewTexture"]);
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
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "02996F:Skyrim.esm")]
    [Trait("EditorID", "C04HagravenHead")]
    [Trait("SpriggitFile", "MiscItems/C04HagravenHead - 02996F_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ShouldMatchSpriggitSample_C04HagravenHead()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MiscItem,
            "C04HagravenHead");
        var dto = Helpers.GetDTO<MiscItemDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MiscItem,
            "02996F:Skyrim.esm");

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
        spriggitFields["VirtualMachineAdapter[0][3].Alias"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Alias"]);
        spriggitFields["VirtualMachineAdapter[0][3].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][3].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Name"]);
        spriggitFields["VirtualMachineAdapter[0][3].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Object"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0B08C7:Skyrim.esm")]
    [Trait("EditorID", "dunUniqueBeeInJar")]
    [Trait("SpriggitFile", "MiscItems/dunUniqueBeeInJar - 0B08C7_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ShouldMatchSpriggitSample_dunUniqueBeeInJar()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MiscItem,
            "dunUniqueBeeInJar");
        var dto = Helpers.GetDTO<MiscItemDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.MiscItem,
            "0B08C7:Skyrim.esm");

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
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
