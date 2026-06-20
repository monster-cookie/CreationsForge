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

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "Background").ShouldBe(Helpers.GetDTOField(dto, "Background"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FNAM").ShouldBe(Helpers.GetDTOField(dto, "FNAM"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "FurnitureTemplate").ShouldBe(Helpers.GetDTOField(dto, "FurnitureTemplateFormKey"));
        Helpers.GetSpriggitField(spriggit, "GNAM").ShouldBe(Helpers.GetDTOField(dto, "GNAM"));
        Helpers.GetSpriggitField(spriggit, "JNAM").ShouldBe(Helpers.GetDTOField(dto, "JNAM"));
        Helpers.GetSpriggitField(spriggit, "MarkerFlags").ShouldBe(Helpers.GetDTOField(dto, "MarkerFlags"));
        Helpers.GetSpriggitField(spriggit, "MarkerModel").ShouldBe(Helpers.GetDTOField(dto, "MarkerModel"));
        Helpers.GetSpriggitField(spriggit, "Menu").ShouldBe(Helpers.GetDTOField(dto, "MenuFormKey"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
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
        Helpers.GetSpriggitField(spriggit, "PNAM").ShouldBe(Helpers.GetDTOField(dto, "PNAM"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchData").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchData"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "Background", "BNAM", "CNAM", "EditorID", "FNAM", "FormKey", "FormVersion", "FurnitureTemplate", "GNAM", "JNAM", "MarkerFlags", "MarkerModel", "Menu", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PNAM", "REFL", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "WorkbenchData");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "Background", "BNAM", "CNAM", "EditorID", "FNAM", "FormKey", "FormVersion", "FurnitureTemplateFormKey", "GNAM", "JNAM", "MarkerFlags", "MarkerModel", "MenuFormKey", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PNAM", "REFL", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "WorkbenchData");
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

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "Background").ShouldBe(Helpers.GetDTOField(dto, "Background"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FNAM").ShouldBe(Helpers.GetDTOField(dto, "FNAM"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "FurnitureTemplate").ShouldBe(Helpers.GetDTOField(dto, "FurnitureTemplateFormKey"));
        Helpers.GetSpriggitField(spriggit, "GNAM").ShouldBe(Helpers.GetDTOField(dto, "GNAM"));
        Helpers.GetSpriggitField(spriggit, "JNAM").ShouldBe(Helpers.GetDTOField(dto, "JNAM"));
        Helpers.GetSpriggitField(spriggit, "MarkerFlags").ShouldBe(Helpers.GetDTOField(dto, "MarkerFlags"));
        Helpers.GetSpriggitField(spriggit, "MarkerModel").ShouldBe(Helpers.GetDTOField(dto, "MarkerModel"));
        Helpers.GetSpriggitField(spriggit, "Menu").ShouldBe(Helpers.GetDTOField(dto, "MenuFormKey"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
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
        Helpers.GetSpriggitField(spriggit, "PNAM").ShouldBe(Helpers.GetDTOField(dto, "PNAM"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchData").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchData"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "Background", "BNAM", "CNAM", "EditorID", "FNAM", "FormKey", "FormVersion", "FurnitureTemplate", "GNAM", "JNAM", "MarkerFlags", "MarkerModel", "Menu", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PNAM", "REFL", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "WorkbenchData");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "Background", "BNAM", "CNAM", "EditorID", "FNAM", "FormKey", "FormVersion", "FurnitureTemplateFormKey", "GNAM", "JNAM", "MarkerFlags", "MarkerModel", "MenuFormKey", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PNAM", "REFL", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "WorkbenchData");
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

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "Background").ShouldBe(Helpers.GetDTOField(dto, "Background"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FNAM").ShouldBe(Helpers.GetDTOField(dto, "FNAM"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "FurnitureTemplate").ShouldBe(Helpers.GetDTOField(dto, "FurnitureTemplateFormKey"));
        Helpers.GetSpriggitField(spriggit, "GNAM").ShouldBe(Helpers.GetDTOField(dto, "GNAM"));
        Helpers.GetSpriggitField(spriggit, "JNAM").ShouldBe(Helpers.GetDTOField(dto, "JNAM"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "MarkerFlags").ShouldBe(Helpers.GetDTOField(dto, "MarkerFlags"));
        Helpers.GetSpriggitField(spriggit, "MarkerModel").ShouldBe(Helpers.GetDTOField(dto, "MarkerModel"));
        Helpers.GetSpriggitField(spriggit, "Menu").ShouldBe(Helpers.GetDTOField(dto, "MenuFormKey"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
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
        Helpers.GetSpriggitField(spriggit, "PNAM").ShouldBe(Helpers.GetDTOField(dto, "PNAM"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchData").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchData"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "Background", "BNAM", "CNAM", "EditorID", "FNAM", "FormKey", "FormVersion", "FurnitureTemplate", "GNAM", "JNAM", "MajorRecordFlagsRaw", "MarkerFlags", "MarkerModel", "Menu", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PNAM", "REFL", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "WorkbenchData");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "Background", "BNAM", "CNAM", "EditorID", "FNAM", "FormKey", "FormVersion", "FurnitureTemplateFormKey", "GNAM", "JNAM", "MajorRecordFlags", "MarkerFlags", "MarkerModel", "MenuFormKey", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PNAM", "REFL", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "WorkbenchData");
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

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "Background").ShouldBe(Helpers.GetDTOField(dto, "Background"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FNAM").ShouldBe(Helpers.GetDTOField(dto, "FNAM"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "GNAM").ShouldBe(Helpers.GetDTOField(dto, "GNAM"));
        Helpers.GetSpriggitField(spriggit, "JNAM").ShouldBe(Helpers.GetDTOField(dto, "JNAM"));
        Helpers.GetSpriggitField(spriggit, "MarkerFlags").ShouldBe(Helpers.GetDTOField(dto, "MarkerFlags"));
        Helpers.GetSpriggitField(spriggit, "MarkerModel").ShouldBe(Helpers.GetDTOField(dto, "MarkerModel"));
        Helpers.GetSpriggitField(spriggit, "Menu").ShouldBe(Helpers.GetDTOField(dto, "MenuFormKey"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
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
        Helpers.GetSpriggitField(spriggit, "PNAM").ShouldBe(Helpers.GetDTOField(dto, "PNAM"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchData").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchData"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "Background", "BNAM", "CNAM", "EditorID", "FNAM", "FormKey", "FormVersion", "GNAM", "JNAM", "MarkerFlags", "MarkerModel", "Menu", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PNAM", "REFL", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "WorkbenchData");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "Background", "BNAM", "CNAM", "EditorID", "FNAM", "FormKey", "FormVersion", "GNAM", "JNAM", "MarkerFlags", "MarkerModel", "MenuFormKey", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PNAM", "REFL", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "WorkbenchData");
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

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "Background").ShouldBe(Helpers.GetDTOField(dto, "Background"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FNAM").ShouldBe(Helpers.GetDTOField(dto, "FNAM"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "FurnitureTemplate").ShouldBe(Helpers.GetDTOField(dto, "FurnitureTemplateFormKey"));
        Helpers.GetSpriggitField(spriggit, "GNAM").ShouldBe(Helpers.GetDTOField(dto, "GNAM"));
        Helpers.GetSpriggitField(spriggit, "JNAM").ShouldBe(Helpers.GetDTOField(dto, "JNAM"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "MarkerFlags").ShouldBe(Helpers.GetDTOField(dto, "MarkerFlags"));
        Helpers.GetSpriggitField(spriggit, "MarkerModel").ShouldBe(Helpers.GetDTOField(dto, "MarkerModel"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
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
        Helpers.GetSpriggitField(spriggit, "PNAM").ShouldBe(Helpers.GetDTOField(dto, "PNAM"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchData").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchData"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "Background", "BNAM", "CNAM", "EditorID", "FNAM", "FormKey", "FormVersion", "FurnitureTemplate", "GNAM", "JNAM", "MajorRecordFlagsRaw", "MarkerFlags", "MarkerModel", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PNAM", "REFL", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "WorkbenchData");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "Background", "BNAM", "CNAM", "EditorID", "FNAM", "FormKey", "FormVersion", "FurnitureTemplateFormKey", "GNAM", "JNAM", "MajorRecordFlags", "MarkerFlags", "MarkerModel", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PNAM", "REFL", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Name", "WorkbenchData");
    }
}