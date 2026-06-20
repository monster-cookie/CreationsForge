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

        spriggitFields["DataSlateHeaderLeft.Count"].ShouldBe(dtoFields["DataSlateHeaderLeft.Count"]);
        spriggitFields["DataSlateHeaderLeft.TargetLanguage"].ShouldBe(dtoFields["DataSlateHeaderLeft.TargetLanguage"]);
        spriggitFields["DataSlateHeaderLeft[0].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[0].Language"]);
        spriggitFields["DataSlateHeaderLeft[0].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[0].String"]);
        spriggitFields["DataSlateHeaderLeft[1].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[1].Language"]);
        spriggitFields["DataSlateHeaderLeft[1].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[1].String"]);
        spriggitFields["DataSlateHeaderLeft[2].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[2].Language"]);
        spriggitFields["DataSlateHeaderLeft[2].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[2].String"]);
        spriggitFields["DataSlateHeaderLeft[3].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[3].Language"]);
        spriggitFields["DataSlateHeaderLeft[3].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[3].String"]);
        spriggitFields["DataSlateHeaderLeft[4].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[4].Language"]);
        spriggitFields["DataSlateHeaderLeft[4].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[4].String"]);
        spriggitFields["DataSlateHeaderLeft[5].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[5].Language"]);
        spriggitFields["DataSlateHeaderLeft[5].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[5].String"]);
        spriggitFields["DataSlateHeaderLeft[6].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[6].Language"]);
        spriggitFields["DataSlateHeaderLeft[6].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[6].String"]);
        spriggitFields["DataSlateHeaderLeft[7].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[7].Language"]);
        spriggitFields["DataSlateHeaderLeft[7].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[7].String"]);
        spriggitFields["DataSlateHeaderLeft[8].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[8].Language"]);
        spriggitFields["DataSlateHeaderLeft[8].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[8].String"]);
        spriggitFields["DataSlateHeaderRight.Count"].ShouldBe(dtoFields["DataSlateHeaderRight.Count"]);
        spriggitFields["DataSlateHeaderRight.TargetLanguage"].ShouldBe(dtoFields["DataSlateHeaderRight.TargetLanguage"]);
        spriggitFields["DataSlateHeaderRight[0].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[0].Language"]);
        spriggitFields["DataSlateHeaderRight[0].String"].ShouldBe(dtoFields["DataSlateHeaderRight[0].String"]);
        spriggitFields["DataSlateHeaderRight[1].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[1].Language"]);
        spriggitFields["DataSlateHeaderRight[1].String"].ShouldBe(dtoFields["DataSlateHeaderRight[1].String"]);
        spriggitFields["DataSlateHeaderRight[2].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[2].Language"]);
        spriggitFields["DataSlateHeaderRight[2].String"].ShouldBe(dtoFields["DataSlateHeaderRight[2].String"]);
        spriggitFields["DataSlateHeaderRight[3].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[3].Language"]);
        spriggitFields["DataSlateHeaderRight[3].String"].ShouldBe(dtoFields["DataSlateHeaderRight[3].String"]);
        spriggitFields["DataSlateHeaderRight[4].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[4].Language"]);
        spriggitFields["DataSlateHeaderRight[4].String"].ShouldBe(dtoFields["DataSlateHeaderRight[4].String"]);
        spriggitFields["DataSlateHeaderRight[5].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[5].Language"]);
        spriggitFields["DataSlateHeaderRight[5].String"].ShouldBe(dtoFields["DataSlateHeaderRight[5].String"]);
        spriggitFields["DataSlateHeaderRight[6].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[6].Language"]);
        spriggitFields["DataSlateHeaderRight[6].String"].ShouldBe(dtoFields["DataSlateHeaderRight[6].String"]);
        spriggitFields["DataSlateHeaderRight[7].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[7].Language"]);
        spriggitFields["DataSlateHeaderRight[7].String"].ShouldBe(dtoFields["DataSlateHeaderRight[7].String"]);
        spriggitFields["DataSlateHeaderRight[8].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[8].Language"]);
        spriggitFields["DataSlateHeaderRight[8].String"].ShouldBe(dtoFields["DataSlateHeaderRight[8].String"]);
        spriggitFields["DataSlateType"].ShouldBe(dtoFields["DataSlateType"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Components.Count"].ShouldBe(dtoFields["Components.Count"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
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
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["Text.Count"].ShouldBe(dtoFields["Text.Count"]);
        spriggitFields["Text.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["Text[0].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        spriggitFields["Text[0].String"].ShouldBe(dtoFields["Text[0].String"]);
        spriggitFields["Text[1].Language"].ShouldBe(dtoFields["Text[1].Language"]);
        spriggitFields["Text[1].String"].ShouldBe(dtoFields["Text[1].String"]);
        spriggitFields["Text[2].Language"].ShouldBe(dtoFields["Text[2].Language"]);
        spriggitFields["Text[2].String"].ShouldBe(dtoFields["Text[2].String"]);
        spriggitFields["Text[3].Language"].ShouldBe(dtoFields["Text[3].Language"]);
        spriggitFields["Text[3].String"].ShouldBe(dtoFields["Text[3].String"]);
        spriggitFields["Text[4].Language"].ShouldBe(dtoFields["Text[4].Language"]);
        spriggitFields["Text[4].String"].ShouldBe(dtoFields["Text[4].String"]);
        spriggitFields["Text[5].Language"].ShouldBe(dtoFields["Text[5].Language"]);
        spriggitFields["Text[5].String"].ShouldBe(dtoFields["Text[5].String"]);
        spriggitFields["Text[6].Language"].ShouldBe(dtoFields["Text[6].Language"]);
        spriggitFields["Text[6].String"].ShouldBe(dtoFields["Text[6].String"]);
        spriggitFields["Text[7].Language"].ShouldBe(dtoFields["Text[7].Language"]);
        spriggitFields["Text[7].String"].ShouldBe(dtoFields["Text[7].String"]);
        spriggitFields["Text[8].Language"].ShouldBe(dtoFields["Text[8].Language"]);
        spriggitFields["Text[8].String"].ShouldBe(dtoFields["Text[8].String"]);
        spriggitFields["Transforms.Inventory"].ShouldBe(dtoFields["Transforms.Inventory"]);
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
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
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
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["Text.Count"].ShouldBe(dtoFields["Text.Count"]);
        spriggitFields["Text.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["Text[0].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        spriggitFields["Text[0].String"].ShouldBe(dtoFields["Text[0].String"]);
        spriggitFields["Text[1].Language"].ShouldBe(dtoFields["Text[1].Language"]);
        spriggitFields["Text[1].String"].ShouldBe(dtoFields["Text[1].String"]);
        spriggitFields["Text[2].Language"].ShouldBe(dtoFields["Text[2].Language"]);
        spriggitFields["Text[2].String"].ShouldBe(dtoFields["Text[2].String"]);
        spriggitFields["Text[3].Language"].ShouldBe(dtoFields["Text[3].Language"]);
        spriggitFields["Text[3].String"].ShouldBe(dtoFields["Text[3].String"]);
        spriggitFields["Text[4].Language"].ShouldBe(dtoFields["Text[4].Language"]);
        spriggitFields["Text[4].String"].ShouldBe(dtoFields["Text[4].String"]);
        spriggitFields["Text[5].Language"].ShouldBe(dtoFields["Text[5].Language"]);
        spriggitFields["Text[5].String"].ShouldBe(dtoFields["Text[5].String"]);
        spriggitFields["Text[6].Language"].ShouldBe(dtoFields["Text[6].Language"]);
        spriggitFields["Text[6].String"].ShouldBe(dtoFields["Text[6].String"]);
        spriggitFields["Text[7].Language"].ShouldBe(dtoFields["Text[7].Language"]);
        spriggitFields["Text[7].String"].ShouldBe(dtoFields["Text[7].String"]);
        spriggitFields["Text[8].Language"].ShouldBe(dtoFields["Text[8].Language"]);
        spriggitFields["Text[8].String"].ShouldBe(dtoFields["Text[8].String"]);
        spriggitFields["Transforms.Inventory"].ShouldBe(dtoFields["Transforms.Inventory"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Count"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Object"]);
        spriggitFields["VirtualMachineAdapter[0][0][1].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][1].Object"]);
        spriggitFields["VirtualMachineAdapter[0][0][2].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][2].Object"]);
        spriggitFields["VirtualMachineAdapter[0][0][3].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][3].Object"]);
        spriggitFields["VirtualMachineAdapter[0][1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Name"]);
        spriggitFields["VirtualMachineAdapter[0][1].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Object"]);
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
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
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
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["Text.Count"].ShouldBe(dtoFields["Text.Count"]);
        spriggitFields["Text.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["Text[0].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        spriggitFields["Text[0].String"].ShouldBe(dtoFields["Text[0].String"]);
        spriggitFields["Text[1].Language"].ShouldBe(dtoFields["Text[1].Language"]);
        spriggitFields["Text[1].String"].ShouldBe(dtoFields["Text[1].String"]);
        spriggitFields["Text[2].Language"].ShouldBe(dtoFields["Text[2].Language"]);
        spriggitFields["Text[2].String"].ShouldBe(dtoFields["Text[2].String"]);
        spriggitFields["Text[3].Language"].ShouldBe(dtoFields["Text[3].Language"]);
        spriggitFields["Text[3].String"].ShouldBe(dtoFields["Text[3].String"]);
        spriggitFields["Text[4].Language"].ShouldBe(dtoFields["Text[4].Language"]);
        spriggitFields["Text[4].String"].ShouldBe(dtoFields["Text[4].String"]);
        spriggitFields["Text[5].Language"].ShouldBe(dtoFields["Text[5].Language"]);
        spriggitFields["Text[5].String"].ShouldBe(dtoFields["Text[5].String"]);
        spriggitFields["Text[6].Language"].ShouldBe(dtoFields["Text[6].Language"]);
        spriggitFields["Text[6].String"].ShouldBe(dtoFields["Text[6].String"]);
        spriggitFields["Text[7].Language"].ShouldBe(dtoFields["Text[7].Language"]);
        spriggitFields["Text[7].String"].ShouldBe(dtoFields["Text[7].String"]);
        spriggitFields["Text[8].Language"].ShouldBe(dtoFields["Text[8].Language"]);
        spriggitFields["Text[8].String"].ShouldBe(dtoFields["Text[8].String"]);
        spriggitFields["Transforms.Inventory"].ShouldBe(dtoFields["Transforms.Inventory"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
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

        spriggitFields["DataSlateHeaderLeft.Count"].ShouldBe(dtoFields["DataSlateHeaderLeft.Count"]);
        spriggitFields["DataSlateHeaderLeft.TargetLanguage"].ShouldBe(dtoFields["DataSlateHeaderLeft.TargetLanguage"]);
        spriggitFields["DataSlateHeaderLeft[0].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[0].Language"]);
        spriggitFields["DataSlateHeaderLeft[0].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[0].String"]);
        spriggitFields["DataSlateHeaderLeft[1].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[1].Language"]);
        spriggitFields["DataSlateHeaderLeft[1].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[1].String"]);
        spriggitFields["DataSlateHeaderLeft[2].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[2].Language"]);
        spriggitFields["DataSlateHeaderLeft[2].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[2].String"]);
        spriggitFields["DataSlateHeaderLeft[3].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[3].Language"]);
        spriggitFields["DataSlateHeaderLeft[3].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[3].String"]);
        spriggitFields["DataSlateHeaderLeft[4].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[4].Language"]);
        spriggitFields["DataSlateHeaderLeft[4].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[4].String"]);
        spriggitFields["DataSlateHeaderLeft[5].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[5].Language"]);
        spriggitFields["DataSlateHeaderLeft[5].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[5].String"]);
        spriggitFields["DataSlateHeaderLeft[6].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[6].Language"]);
        spriggitFields["DataSlateHeaderLeft[6].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[6].String"]);
        spriggitFields["DataSlateHeaderLeft[7].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[7].Language"]);
        spriggitFields["DataSlateHeaderLeft[7].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[7].String"]);
        spriggitFields["DataSlateHeaderLeft[8].Language"].ShouldBe(dtoFields["DataSlateHeaderLeft[8].Language"]);
        spriggitFields["DataSlateHeaderLeft[8].String"].ShouldBe(dtoFields["DataSlateHeaderLeft[8].String"]);
        spriggitFields["DataSlateHeaderRight.Count"].ShouldBe(dtoFields["DataSlateHeaderRight.Count"]);
        spriggitFields["DataSlateHeaderRight.TargetLanguage"].ShouldBe(dtoFields["DataSlateHeaderRight.TargetLanguage"]);
        spriggitFields["DataSlateHeaderRight[0].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[0].Language"]);
        spriggitFields["DataSlateHeaderRight[0].String"].ShouldBe(dtoFields["DataSlateHeaderRight[0].String"]);
        spriggitFields["DataSlateHeaderRight[1].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[1].Language"]);
        spriggitFields["DataSlateHeaderRight[1].String"].ShouldBe(dtoFields["DataSlateHeaderRight[1].String"]);
        spriggitFields["DataSlateHeaderRight[2].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[2].Language"]);
        spriggitFields["DataSlateHeaderRight[2].String"].ShouldBe(dtoFields["DataSlateHeaderRight[2].String"]);
        spriggitFields["DataSlateHeaderRight[3].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[3].Language"]);
        spriggitFields["DataSlateHeaderRight[3].String"].ShouldBe(dtoFields["DataSlateHeaderRight[3].String"]);
        spriggitFields["DataSlateHeaderRight[4].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[4].Language"]);
        spriggitFields["DataSlateHeaderRight[4].String"].ShouldBe(dtoFields["DataSlateHeaderRight[4].String"]);
        spriggitFields["DataSlateHeaderRight[5].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[5].Language"]);
        spriggitFields["DataSlateHeaderRight[5].String"].ShouldBe(dtoFields["DataSlateHeaderRight[5].String"]);
        spriggitFields["DataSlateHeaderRight[6].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[6].Language"]);
        spriggitFields["DataSlateHeaderRight[6].String"].ShouldBe(dtoFields["DataSlateHeaderRight[6].String"]);
        spriggitFields["DataSlateHeaderRight[7].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[7].Language"]);
        spriggitFields["DataSlateHeaderRight[7].String"].ShouldBe(dtoFields["DataSlateHeaderRight[7].String"]);
        spriggitFields["DataSlateHeaderRight[8].Language"].ShouldBe(dtoFields["DataSlateHeaderRight[8].Language"]);
        spriggitFields["DataSlateHeaderRight[8].String"].ShouldBe(dtoFields["DataSlateHeaderRight[8].String"]);
        spriggitFields["DataSlateType"].ShouldBe(dtoFields["DataSlateType"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Components.Count"].ShouldBe(dtoFields["Components.Count"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
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
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["Text.Count"].ShouldBe(dtoFields["Text.Count"]);
        spriggitFields["Text.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["Text[0].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        spriggitFields["Text[0].String"].ShouldBe(dtoFields["Text[0].String"]);
        spriggitFields["Text[1].Language"].ShouldBe(dtoFields["Text[1].Language"]);
        spriggitFields["Text[1].String"].ShouldBe(dtoFields["Text[1].String"]);
        spriggitFields["Text[2].Language"].ShouldBe(dtoFields["Text[2].Language"]);
        spriggitFields["Text[2].String"].ShouldBe(dtoFields["Text[2].String"]);
        spriggitFields["Text[3].Language"].ShouldBe(dtoFields["Text[3].Language"]);
        spriggitFields["Text[3].String"].ShouldBe(dtoFields["Text[3].String"]);
        spriggitFields["Text[4].Language"].ShouldBe(dtoFields["Text[4].Language"]);
        spriggitFields["Text[4].String"].ShouldBe(dtoFields["Text[4].String"]);
        spriggitFields["Text[5].Language"].ShouldBe(dtoFields["Text[5].Language"]);
        spriggitFields["Text[5].String"].ShouldBe(dtoFields["Text[5].String"]);
        spriggitFields["Text[6].Language"].ShouldBe(dtoFields["Text[6].Language"]);
        spriggitFields["Text[6].String"].ShouldBe(dtoFields["Text[6].String"]);
        spriggitFields["Text[7].Language"].ShouldBe(dtoFields["Text[7].Language"]);
        spriggitFields["Text[7].String"].ShouldBe(dtoFields["Text[7].String"]);
        spriggitFields["Text[8].Language"].ShouldBe(dtoFields["Text[8].Language"]);
        spriggitFields["Text[8].String"].ShouldBe(dtoFields["Text[8].String"]);
        spriggitFields["Transforms.Inventory"].ShouldBe(dtoFields["Transforms.Inventory"]);
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
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
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
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["Text.Count"].ShouldBe(dtoFields["Text.Count"]);
        spriggitFields["Text.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["Text[0].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        spriggitFields["Text[0].String"].ShouldBe(dtoFields["Text[0].String"]);
        spriggitFields["Text[1].Language"].ShouldBe(dtoFields["Text[1].Language"]);
        spriggitFields["Text[1].String"].ShouldBe(dtoFields["Text[1].String"]);
        spriggitFields["Text[2].Language"].ShouldBe(dtoFields["Text[2].Language"]);
        spriggitFields["Text[2].String"].ShouldBe(dtoFields["Text[2].String"]);
        spriggitFields["Text[3].Language"].ShouldBe(dtoFields["Text[3].Language"]);
        spriggitFields["Text[3].String"].ShouldBe(dtoFields["Text[3].String"]);
        spriggitFields["Text[4].Language"].ShouldBe(dtoFields["Text[4].Language"]);
        spriggitFields["Text[4].String"].ShouldBe(dtoFields["Text[4].String"]);
        spriggitFields["Text[5].Language"].ShouldBe(dtoFields["Text[5].Language"]);
        spriggitFields["Text[5].String"].ShouldBe(dtoFields["Text[5].String"]);
        spriggitFields["Text[6].Language"].ShouldBe(dtoFields["Text[6].Language"]);
        spriggitFields["Text[6].String"].ShouldBe(dtoFields["Text[6].String"]);
        spriggitFields["Text[7].Language"].ShouldBe(dtoFields["Text[7].Language"]);
        spriggitFields["Text[7].String"].ShouldBe(dtoFields["Text[7].String"]);
        spriggitFields["Text[8].Language"].ShouldBe(dtoFields["Text[8].Language"]);
        spriggitFields["Text[8].String"].ShouldBe(dtoFields["Text[8].String"]);
        spriggitFields["Transforms.Inventory"].ShouldBe(dtoFields["Transforms.Inventory"]);
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
        spriggitFields["VirtualMachineAdapter[0][2].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Name"]);
        spriggitFields["VirtualMachineAdapter[0][2].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Object"]);
        spriggitFields["VirtualMachineAdapter[0][3].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Data"]);
        spriggitFields["VirtualMachineAdapter[0][3].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][3].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Name"]);
        spriggitFields["VirtualMachineAdapter[0][4].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].Data"]);
        spriggitFields["VirtualMachineAdapter[0][4].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][4].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].Data"]);
        spriggitFields["VirtualMachineAdapter[0][5].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][5].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].Name"]);
        spriggitFields["VirtualMachineAdapter[0][6].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].Data"]);
        spriggitFields["VirtualMachineAdapter[0][6].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][6].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].Name"]);
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
}
