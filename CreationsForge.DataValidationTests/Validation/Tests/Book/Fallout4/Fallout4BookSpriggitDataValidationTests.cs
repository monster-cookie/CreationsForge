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

        dtoFields["Text.Count"].ShouldBe("1");
        spriggitFields["BookText.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["BookText[2].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        NormalizeText(spriggitFields["BookText[2].String"]).ShouldBe(NormalizeText(dtoFields["Text[0].String"]));
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["PreviewTransform"].ShouldBe(dtoFields["PreviewTransformFormKey"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields.ContainsKey("VirtualMachineAdapter.Scripts.Count").ShouldBeFalse();
        dtoFields["ScriptingAdapters.Count"].ShouldBe("0");

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

        dtoFields["Text.Count"].ShouldBe("1");
        spriggitFields["BookText.TargetLanguage"].ShouldBe(dtoFields["Text.TargetLanguage"]);
        spriggitFields["BookText[2].Language"].ShouldBe(dtoFields["Text[0].Language"]);
        NormalizeText(spriggitFields["BookText[2].String"]).ShouldBe(NormalizeText(dtoFields["Text[0].String"]));
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VirtualMachineAdapter.Scripts.Count"].ShouldBe(dtoFields["ScriptingAdapters.Count"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties.Count"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties.Count"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[0].Object"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[0].ObjectFormKey"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[1].Name"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[1].Name"]);
        spriggitFields["VirtualMachineAdapter.Scripts[0].Properties[1].Object"].ShouldBe(dtoFields["ScriptingAdapters[0].Properties[1].ObjectFormKey"]);

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
