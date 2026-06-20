using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.FormList.Starfield;

public class StarfieldFormListSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2117E6:Starfield.esm")]
    [Trait("EditorID", "AkilaVendorVeryHighOrganicResources")]
    [Trait("SpriggitFile", "FormLists/AkilaVendorVeryHighOrganicResources - 2117E6_Starfield.esm.yaml")]
    public void Starfield_FLST_ShouldMatchSpriggitSample_AkilaVendorVeryHighOrganicResources()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.FormList,
            "AkilaVendorVeryHighOrganicResources");
        var dto = Helpers.GetDTO<FormListDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.FormList,
            "2117E6:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "AddToList").ShouldBe(Helpers.GetDTOField(dto, "AddToListFormKey"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AddToList", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AddToListFormKey", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2117EC:Starfield.esm")]
    [Trait("EditorID", "AkilaVendorVeryLowOrganicResources")]
    [Trait("SpriggitFile", "FormLists/AkilaVendorVeryLowOrganicResources - 2117EC_Starfield.esm.yaml")]
    public void Starfield_FLST_ShouldMatchSpriggitSample_AkilaVendorVeryLowOrganicResources()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.FormList,
            "AkilaVendorVeryLowOrganicResources");
        var dto = Helpers.GetDTO<FormListDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.FormList,
            "2117EC:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "AddToList").ShouldBe(Helpers.GetDTOField(dto, "AddToListFormKey"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AddToList", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AddToListFormKey", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2117F0:Starfield.esm")]
    [Trait("EditorID", "AlikaVendorLowOrganicResources")]
    [Trait("SpriggitFile", "FormLists/AlikaVendorLowOrganicResources - 2117F0_Starfield.esm.yaml")]
    public void Starfield_FLST_ShouldMatchSpriggitSample_AlikaVendorLowOrganicResources()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.FormList,
            "AlikaVendorLowOrganicResources");
        var dto = Helpers.GetDTO<FormListDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.FormList,
            "2117F0:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "AddToList").ShouldBe(Helpers.GetDTOField(dto, "AddToListFormKey"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AddToList", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AddToListFormKey", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "0C3830:Starfield.esm")]
    [Trait("EditorID", "COND_imgui_1_Assorted")]
    [Trait("SpriggitFile", "FormLists/COND_imgui_1_Assorted - 0C3830_Starfield.esm.yaml")]
    public void Starfield_FLST_ShouldMatchSpriggitSample_COND_imgui_1_Assorted()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.FormList,
            "COND_imgui_1_Assorted");
        var dto = Helpers.GetDTO<FormListDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.FormList,
            "0C3830:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "AddToList").ShouldBe(Helpers.GetDTOField(dto, "AddToListFormKey"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
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
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AddToList", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AddToListFormKey", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Version2", "VersionControl");
    }
}