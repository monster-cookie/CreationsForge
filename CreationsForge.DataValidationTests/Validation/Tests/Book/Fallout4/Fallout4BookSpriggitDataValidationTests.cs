using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Book.Fallout4;

public class Fallout4BookSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "02B4DF:Fallout4.esm")]
    [Trait("EditorID", "BoS301ActuatorList")]
    [Trait("SpriggitFile", "Books/BoS301ActuatorList - 02B4DF_Fallout4.esm.yaml")]
    public void Fallout4_BOOK_ShouldMatchSpriggitSample_BoS301ActuatorList()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Book,
            "BoS301ActuatorList");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Book,
            "02B4DF:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["BookText.Count"].ShouldBe(dtoFields["Text.Count"]);
        spriggitFields["BookText.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "23C675:Fallout4.esm")]
    [Trait("EditorID", "DN054PowerArmorPaintJobPurchaseItem")]
    [Trait("SpriggitFile", "Books/DN054PowerArmorPaintJobPurchaseItem - 23C675_Fallout4.esm.yaml")]
    public void Fallout4_BOOK_ShouldMatchSpriggitSample_DN054PowerArmorPaintJobPurchaseItem()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Book,
            "DN054PowerArmorPaintJobPurchaseItem");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Book,
            "23C675:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["BookText.Count"].ShouldBe(dtoFields["Text.Count"]);
        spriggitFields["BookText.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);

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
