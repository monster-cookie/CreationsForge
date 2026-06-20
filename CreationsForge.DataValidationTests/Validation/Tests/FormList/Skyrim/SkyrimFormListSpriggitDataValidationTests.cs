using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.FormList.Skyrim;

public class SkyrimFormListSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "06F3F7:Skyrim.esm")]
    [Trait("EditorID", "AAAMothPlantTypes")]
    [Trait("SpriggitFile", "FormLists/AAAMothPlantTypes - 06F3F7_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ShouldMatchSpriggitSample_AAAMothPlantTypes()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.FormList,
            "AAAMothPlantTypes");
        var dto = Helpers.GetDTO<FormListDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.FormList,
            "06F3F7:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "045C32:Skyrim.esm")]
    [Trait("EditorID", "CityWindhelmResidentList")]
    [Trait("SpriggitFile", "FormLists/CityWindhelmResidentList - 045C32_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ShouldMatchSpriggitSample_CityWindhelmResidentList()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.FormList,
            "CityWindhelmResidentList");
        var dto = Helpers.GetDTO<FormListDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.FormList,
            "045C32:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "026953:Skyrim.esm")]
    [Trait("EditorID", "CrimeFactionsList")]
    [Trait("SpriggitFile", "FormLists/CrimeFactionsList - 026953_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ShouldMatchSpriggitSample_CrimeFactionsList()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.FormList,
            "CrimeFactionsList");
        var dto = Helpers.GetDTO<FormListDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.FormList,
            "026953:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "000D14:Skyrim.esm")]
    [Trait("EditorID", "DraugrWeapons")]
    [Trait("SpriggitFile", "FormLists/DraugrWeapons - 000D14_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ShouldMatchSpriggitSample_DraugrWeapons()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.FormList,
            "DraugrWeapons");
        var dto = Helpers.GetDTO<FormListDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.FormList,
            "000D14:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
    }
}