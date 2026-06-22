using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Book.Skyrim;

public class SkyrimBookSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "10F776:Skyrim.esm")]
    [Trait("EditorID", "AtrFrgDaedricRecipe00")]
    [Trait("SpriggitFile", "Books/AtrFrgDaedricRecipe00 - 10F776_Skyrim.esm.yaml")]
    public void Skyrim_BOOK_ShouldMatchSpriggitSample_AtrFrgDaedricRecipe00()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Book,
            "AtrFrgDaedricRecipe00");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Book,
            "10F776:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        dtoFields["Text.Count"].ShouldBe("1");
        spriggitFields["BookText.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["BookText[7].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        NormalizeText(spriggitFields["BookText[7].String"]).ShouldBe(NormalizeText(dtoFields["Text[0].String"]));
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Keywords[0]"].ShouldBe(dtoFields["Keywords[0].Keyword"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["PickUpSound"].ShouldBe(dtoFields["PickUpSound.Start"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "01AFD7:Skyrim.esm")]
    [Trait("EditorID", "Book0ArgonianAccountBook1")]
    [Trait("SpriggitFile", "Books/Book0ArgonianAccountBook1 - 01AFD7_Skyrim.esm.yaml")]
    public void Skyrim_BOOK_ShouldMatchSpriggitSample_Book0ArgonianAccountBook1()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Book,
            "Book0ArgonianAccountBook1");
        var dto = Helpers.GetDTO<BookDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Book,
            "01AFD7:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        dtoFields["Text.Count"].ShouldBe("1");
        spriggitFields["BookText.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["BookText[7].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        NormalizeText(spriggitFields["BookText[7].String"]).ShouldBe(NormalizeText(dtoFields["Text[0].String"]));
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Keywords[0]"].ShouldBe(dtoFields["Keywords[0].Keyword"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBounds.First"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBounds.Second"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    private static string NormalizeModelFile(string modelFile)
    {
        return modelFile.StartsWith("Meshes\\", StringComparison.OrdinalIgnoreCase)
            ? modelFile
            : "Meshes\\" + modelFile;
    }

    private static string NormalizeText(string text)
    {
        return text
            .Replace("\\r\\n", "\r\n", StringComparison.Ordinal)
            .Replace("\\\"", "\"", StringComparison.Ordinal);
    }
}
