using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Terminal.Starfield;

public class StarfieldTerminalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "2D1D29:Starfield.esm")]
    [Trait("EditorID", "AkilaLife04_Computer")]
    [Trait("SpriggitFile", "Terminals/AkilaLife04_Computer - 2D1D29_Starfield.esm.yaml")]
    public void Starfield_TERM_ShouldMatchSpriggitSample_AkilaLife04_Computer()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Terminal,
            "AkilaLife04_Computer");
        var dto = Helpers.GetDTO<TerminalDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Terminal,
            "2D1D29:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["Background"].ShouldBe(dtoFields["Background"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["FurnitureTemplate"].ShouldBe(dtoFields["FurnitureTemplateFormKey"]);
        spriggitFields["GNAM"].ShouldBe(dtoFields["GNAM"]);
        spriggitFields["JNAM"].ShouldBe(dtoFields["JNAM"]);
        spriggitFields["MarkerFlags"].ShouldBe(dtoFields["MarkerFlags"]);
        spriggitFields["MarkerModel"].ShouldBe(dtoFields["MarkerModel"]);
        spriggitFields["Menu"].ShouldBe(dtoFields["MenuFormKey"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
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
        spriggitFields["PNAM"].ShouldBe(dtoFields["PNAM"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["WorkbenchData"].ShouldBe(dtoFields["WorkbenchData"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "2D2617:Starfield.esm")]
    [Trait("EditorID", "AkilaLife08_FarmingComputer")]
    [Trait("SpriggitFile", "Terminals/AkilaLife08_FarmingComputer - 2D2617_Starfield.esm.yaml")]
    public void Starfield_TERM_ShouldMatchSpriggitSample_AkilaLife08_FarmingComputer()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Terminal,
            "AkilaLife08_FarmingComputer");
        var dto = Helpers.GetDTO<TerminalDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Terminal,
            "2D2617:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["Background"].ShouldBe(dtoFields["Background"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["FurnitureTemplate"].ShouldBe(dtoFields["FurnitureTemplateFormKey"]);
        spriggitFields["GNAM"].ShouldBe(dtoFields["GNAM"]);
        spriggitFields["JNAM"].ShouldBe(dtoFields["JNAM"]);
        spriggitFields["MarkerFlags"].ShouldBe(dtoFields["MarkerFlags"]);
        spriggitFields["MarkerModel"].ShouldBe(dtoFields["MarkerModel"]);
        spriggitFields["Menu"].ShouldBe(dtoFields["MenuFormKey"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
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
        spriggitFields["PNAM"].ShouldBe(dtoFields["PNAM"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["WorkbenchData"].ShouldBe(dtoFields["WorkbenchData"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "386CD0:Starfield.esm")]
    [Trait("EditorID", "BE_ShipComputer_BarStanding")]
    [Trait("SpriggitFile", "Terminals/BE_ShipComputer_BarStanding - 386CD0_Starfield.esm.yaml")]
    public void Starfield_TERM_ShouldMatchSpriggitSample_BE_ShipComputer_BarStanding()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Terminal,
            "BE_ShipComputer_BarStanding");
        var dto = Helpers.GetDTO<TerminalDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Terminal,
            "386CD0:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["Background"].ShouldBe(dtoFields["Background"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["FurnitureTemplate"].ShouldBe(dtoFields["FurnitureTemplateFormKey"]);
        spriggitFields["GNAM"].ShouldBe(dtoFields["GNAM"]);
        spriggitFields["JNAM"].ShouldBe(dtoFields["JNAM"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["MarkerFlags"].ShouldBe(dtoFields["MarkerFlags"]);
        spriggitFields["MarkerModel"].ShouldBe(dtoFields["MarkerModel"]);
        spriggitFields["Menu"].ShouldBe(dtoFields["MenuFormKey"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
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
        spriggitFields["PNAM"].ShouldBe(dtoFields["PNAM"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["WorkbenchData"].ShouldBe(dtoFields["WorkbenchData"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "261A51:Starfield.esm")]
    [Trait("EditorID", "City_NA_Botany02Terminal")]
    [Trait("SpriggitFile", "Terminals/City_NA_Botany02Terminal - 261A51_Starfield.esm.yaml")]
    public void Starfield_TERM_ShouldMatchSpriggitSample_City_NA_Botany02Terminal()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Terminal,
            "City_NA_Botany02Terminal");
        var dto = Helpers.GetDTO<TerminalDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Terminal,
            "261A51:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["Background"].ShouldBe(dtoFields["Background"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["GNAM"].ShouldBe(dtoFields["GNAM"]);
        spriggitFields["JNAM"].ShouldBe(dtoFields["JNAM"]);
        spriggitFields["MarkerFlags"].ShouldBe(dtoFields["MarkerFlags"]);
        spriggitFields["MarkerModel"].ShouldBe(dtoFields["MarkerModel"]);
        spriggitFields["Menu"].ShouldBe(dtoFields["MenuFormKey"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
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
        spriggitFields["PNAM"].ShouldBe(dtoFields["PNAM"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["WorkbenchData"].ShouldBe(dtoFields["WorkbenchData"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "19F266:Starfield.esm")]
    [Trait("EditorID", "TerminalSittingActivatorA01_Desk")]
    [Trait("SpriggitFile", "Terminals/TerminalSittingActivatorA01_Desk - 19F266_Starfield.esm.yaml")]
    public void Starfield_TERM_ShouldMatchSpriggitSample_TerminalSittingActivatorA01_Desk()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Terminal,
            "TerminalSittingActivatorA01_Desk");
        var dto = Helpers.GetDTO<TerminalDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Terminal,
            "19F266:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["Background"].ShouldBe(dtoFields["Background"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["FurnitureTemplate"].ShouldBe(dtoFields["FurnitureTemplateFormKey"]);
        spriggitFields["GNAM"].ShouldBe(dtoFields["GNAM"]);
        spriggitFields["JNAM"].ShouldBe(dtoFields["JNAM"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["MarkerFlags"].ShouldBe(dtoFields["MarkerFlags"]);
        spriggitFields["MarkerModel"].ShouldBe(dtoFields["MarkerModel"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
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
        spriggitFields["PNAM"].ShouldBe(dtoFields["PNAM"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["WorkbenchData"].ShouldBe(dtoFields["WorkbenchData"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
