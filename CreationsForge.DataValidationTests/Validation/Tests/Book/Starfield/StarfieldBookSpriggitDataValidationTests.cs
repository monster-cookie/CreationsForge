using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Book.Starfield;

public class StarfieldBookSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "165BF3:Starfield.esm")]
    [Trait("EditorID", "NH_SouvenirSlate")]
    [Trait("SpriggitFile", "Books/NH_SouvenirSlate - 165BF3_Starfield.esm.yaml")]
    public void Starfield_BOOK_ShouldMatchSpriggitSample_NH_SouvenirSlate()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "NH_SouvenirSlate");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "165BF3:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        dtoFields["DataSlateHeaderLeft.Count"].ShouldBe("1");
        spriggitFields["DataSlateHeaderLeft.TargetLanguage"].ShouldBe(dtoFields["DataSlateHeaderLeft.TargetLanguage"]);
        spriggitFields["DataSlateHeaderLeft[1].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[0].Language"]);
        spriggitFields["DataSlateHeaderLeft[1].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[0].String"]);
        dtoFields["DataSlateHeaderRight.Count"].ShouldBe("1");
        spriggitFields["DataSlateHeaderRight.TargetLanguage"].ShouldBe(dtoFields["DataSlateHeaderRight.TargetLanguage"]);
        spriggitFields["DataSlateHeaderRight[1].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[0].Language"]);
        spriggitFields["DataSlateHeaderRight[1].String"].ShouldBe(dtoFields["DataSlateHeaderRight[0].String"]);
        spriggitFields["DataSlateType"].ShouldBe(dtoFields["DataSlateType"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Components.Count"].ShouldBe(dtoFields["Components.Count"]);
        dtoFields["RawPayloads.Count"].ShouldBe("1");
        dtoFields["RawPayloads[0].PayloadSlot"].ShouldBe("BaseFormComponents.LodOwnerComponentBinaryOverlay.REFL");
        dtoFields["RawPayloads[0].PayloadType"].ShouldBe("LodOwnerComponentBinaryOverlay");
        dtoFields["RawPayloads[0].SourcePath"].ShouldBe("Components.LodOwnerComponentBinaryOverlay.REFL");
        NormalizeHexPayload(spriggitFields["Components[0].REFL"]).ShouldBe(NormalizeHexPayload(dtoFields["RawPayloads[0].PayloadValue"]));
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBounds.First"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBounds.Second"]);
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        dtoFields["Text.Count"].ShouldBe("1");
        spriggitFields["Text.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["Text[1].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        spriggitFields["Text[1].String"].ShouldBe(dtoFields["Text[0].String"]);
        spriggitFields["InventoryArt"].ShouldBe(dtoFields["InventoryArt"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["XALG"].ShouldBe(dtoFields["XALG"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "1F40EE:Starfield.esm")]
    [Trait("EditorID", "UC07_ScrappingNiira")]
    [Trait("SpriggitFile", "Books/UC07_ScrappingNiira - 1F40EE_Starfield.esm.yaml")]
    public void Starfield_BOOK_ShouldMatchSpriggitSample_UC07_ScrappingNiira()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "UC07_ScrappingNiira");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "1F40EE:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["DataSlateType"].ShouldBe(dtoFields["DataSlateType"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Components.Count"].ShouldBe(dtoFields["Components.Count"]);
        dtoFields["RawPayloads.Count"].ShouldBe("1");
        dtoFields["RawPayloads[0].PayloadSlot"].ShouldBe("BaseFormComponents.LodOwnerComponentBinaryOverlay.REFL");
        dtoFields["RawPayloads[0].PayloadType"].ShouldBe("LodOwnerComponentBinaryOverlay");
        dtoFields["RawPayloads[0].SourcePath"].ShouldBe("Components.LodOwnerComponentBinaryOverlay.REFL");
        NormalizeHexPayload(spriggitFields["Components[0].REFL"]).ShouldBe(NormalizeHexPayload(dtoFields["RawPayloads[0].PayloadValue"]));
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBounds.First"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBounds.Second"]);
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        dtoFields["Text.Count"].ShouldBe("1");
        spriggitFields["Text.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["Text[1].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        spriggitFields["Text[1].String"].ShouldBe(dtoFields["Text[0].String"]);
        spriggitFields["InventoryArt"].ShouldBe(dtoFields["InventoryArt"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Scripts.Count"].ShouldBe(dtoFields["ScriptingAdapters.Count"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties.Count"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties.Count"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].Objects.Count"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].ListItems.Count"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].MutagenObjectType"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].Objects[0].Object"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].ListItems[0].ObjectFormKey"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].Objects[1].Object"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].ListItems[1].ObjectFormKey"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].Objects[2].Object"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].ListItems[2].ObjectFormKey"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].Objects[3].Object"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].ListItems[3].ObjectFormKey"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[1].MutagenObjectType"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[1].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[1].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[1].Object"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[1].ObjectFormKey"]);
        spriggitFields["XALG"].ShouldBe(dtoFields["XALG"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "26E6B1:Starfield.esm")]
    [Trait("EditorID", "SQ_PlanetSurveySlate00_025")]
    [Trait("SpriggitFile", "Books/SQ_PlanetSurveySlate00_025 - 26E6B1_Starfield.esm.yaml")]
    public void Starfield_BOOK_ShouldMatchSpriggitSample_SQ_PlanetSurveySlate00_025()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "SQ_PlanetSurveySlate00_025");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "26E6B1:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["DataSlateType"].ShouldBe(dtoFields["DataSlateType"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Components.Count"].ShouldBe(dtoFields["Components.Count"]);
        dtoFields["RawPayloads.Count"].ShouldBe("1");
        dtoFields["RawPayloads[0].PayloadSlot"].ShouldBe("BaseFormComponents.LodOwnerComponentBinaryOverlay.REFL");
        dtoFields["RawPayloads[0].PayloadType"].ShouldBe("LodOwnerComponentBinaryOverlay");
        dtoFields["RawPayloads[0].SourcePath"].ShouldBe("Components.LodOwnerComponentBinaryOverlay.REFL");
        NormalizeHexPayload(spriggitFields["Components[0].REFL"]).ShouldBe(NormalizeHexPayload(dtoFields["RawPayloads[0].PayloadValue"]));
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBounds.First"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBounds.Second"]);
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        dtoFields["Text.Count"].ShouldBe("1");
        spriggitFields["Text.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["Text[1].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        spriggitFields["Text[1].String"].ShouldBe(dtoFields["Text[0].String"]);
        spriggitFields["InventoryArt"].ShouldBe(dtoFields["InventoryArt"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Scripts.Count"].ShouldBe(dtoFields["ScriptingAdapters.Count"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Name"]);
        dtoFields["ScriptingAdapters[0].Properties.Count"].ShouldBe("0");
        spriggitFields["XALG"].ShouldBe(dtoFields["XALG"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "070510:Starfield.esm")]
    [Trait("EditorID", "_RENAME_TestDataslate")]
    [Trait("SpriggitFile", "Books/_RENAME_TestDataslate - 070510_Starfield.esm.yaml")]
    public void Starfield_BOOK_ShouldMatchSpriggitSample_RENAME_TestDataslate()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "_RENAME_TestDataslate");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "070510:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        dtoFields["DataSlateHeaderLeft.Count"].ShouldBe("1");
        spriggitFields["DataSlateHeaderLeft.TargetLanguage"].ShouldBe(dtoFields["DataSlateHeaderLeft.TargetLanguage"]);
        spriggitFields["DataSlateHeaderLeft[1].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[0].Language"]);
        spriggitFields["DataSlateHeaderLeft[1].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[0].String"]);
        dtoFields["DataSlateHeaderRight.Count"].ShouldBe("1");
        spriggitFields["DataSlateHeaderRight.TargetLanguage"].ShouldBe(dtoFields["DataSlateHeaderRight.TargetLanguage"]);
        spriggitFields["DataSlateHeaderRight[1].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[0].Language"]);
        spriggitFields["DataSlateHeaderRight[1].String"].ShouldBe(dtoFields["DataSlateHeaderRight[0].String"]);
        spriggitFields["DataSlateType"].ShouldBe(dtoFields["DataSlateType"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Components.Count"].ShouldBe(dtoFields["Components.Count"]);
        dtoFields["RawPayloads.Count"].ShouldBe("1");
        dtoFields["RawPayloads[0].PayloadSlot"].ShouldBe("BaseFormComponents.LodOwnerComponentBinaryOverlay.REFL");
        dtoFields["RawPayloads[0].PayloadType"].ShouldBe("LodOwnerComponentBinaryOverlay");
        dtoFields["RawPayloads[0].SourcePath"].ShouldBe("Components.LodOwnerComponentBinaryOverlay.REFL");
        NormalizeHexPayload(spriggitFields["Components[0].REFL"]).ShouldBe(NormalizeHexPayload(dtoFields["RawPayloads[0].PayloadValue"]));
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBounds.First"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBounds.Second"]);
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        dtoFields["Text.Count"].ShouldBe("1");
        spriggitFields["Text.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["Text[1].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        spriggitFields["Text[1].String"].ShouldBe(dtoFields["Text[0].String"]);
        spriggitFields["InventoryArt"].ShouldBe(dtoFields["InventoryArt"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["XALG"].ShouldBe(dtoFields["XALG"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "045631:Starfield.esm")]
    [Trait("EditorID", "TreasureMap_Resource_AnySystem_Unique_Aldumite")]
    [Trait("SpriggitFile", "Books/TreasureMap_Resource_AnySystem_Unique_Aldumite - 045631_Starfield.esm.yaml")]
    public void Starfield_BOOK_ShouldMatchSpriggitSample_TreasureMap_Resource_AnySystem_Unique_Aldumite()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "TreasureMap_Resource_AnySystem_Unique_Aldumite");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Book,
            "045631:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Components.Count"].ShouldBe(dtoFields["Components.Count"]);
        dtoFields["RawPayloads.Count"].ShouldBe("1");
        dtoFields["RawPayloads[0].PayloadSlot"].ShouldBe("BaseFormComponents.LodOwnerComponentBinaryOverlay.REFL");
        dtoFields["RawPayloads[0].PayloadType"].ShouldBe("LodOwnerComponentBinaryOverlay");
        dtoFields["RawPayloads[0].SourcePath"].ShouldBe("Components.LodOwnerComponentBinaryOverlay.REFL");
        NormalizeHexPayload(spriggitFields["Components[0].REFL"]).ShouldBe(NormalizeHexPayload(dtoFields["RawPayloads[0].PayloadValue"]));
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBounds.First"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBounds.Second"]);
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        dtoFields["Text.Count"].ShouldBe("1");
        spriggitFields["Text.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["Text[1].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        spriggitFields["Text[1].String"].ShouldBe(dtoFields["Text[0].String"]);
        spriggitFields["InventoryArt"].ShouldBe(dtoFields["InventoryArt"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Scripts.Count"].ShouldBe(dtoFields["ScriptingAdapters.Count"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties.Count"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties.Count"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].MutagenObjectType"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].Object"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].ObjectFormKey"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[1].MutagenObjectType"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[1].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[1].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[1].Object"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[1].ObjectFormKey"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[2].MutagenObjectType"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[2].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[2].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[2].Object"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[2].ObjectFormKey"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[3].Data"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[3].DataInt"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[3].MutagenObjectType"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[3].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[3].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[3].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[4].Data"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[4].DataInt"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[4].MutagenObjectType"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[4].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[4].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[4].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[5].Data"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[5].DataInt"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[5].MutagenObjectType"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[5].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[5].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[5].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[6].Data"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[6].DataInt"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[6].MutagenObjectType"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[6].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[6].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[6].Name"]);
        spriggitFields["XALG"].ShouldBe(dtoFields["XALG"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    private static string NormalizeModelFile(string modelFile)
    {
        return modelFile.StartsWith("Meshes\\", StringComparison.OrdinalIgnoreCase)
            ? modelFile
            : "Meshes\\" + modelFile;
    }

    private static string NormalizeHexPayload(string payload)
    {
        return payload.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
            ? payload[2..]
            : payload;
    }
}
