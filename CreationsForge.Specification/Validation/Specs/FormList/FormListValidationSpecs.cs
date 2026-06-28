using CreationsForge.Specification.Records;
using CreationsForge.Specification.Validation;

namespace CreationsForge.Specification.Validation.Specs.FormList;

public static class FormListValidationSpecs
{
    public static ValidationSpec Starfield_AkilaVendorVeryHighOrganicResources()
    {
        var spec = StarfieldFormList("AkilaVendorVeryHighOrganicResources", "2117E6:Starfield.esm", withName: false);
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(
            ["Items[0]"],
            "Starfield.esm:000055B1"));
        return spec;
    }

    /// <summary>
    /// Builds the Starfield <c>AkilaVendorVeryLowOrganicResources</c> form list validation spec, including a UI item row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>AkilaVendorVeryLowOrganicResources</c> sample.</returns>
    public static ValidationSpec Starfield_AkilaVendorVeryLowOrganicResources()
    {
        var spec = StarfieldFormList("AkilaVendorVeryLowOrganicResources", "2117EC:Starfield.esm", withName: false);
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Items[0]"]));
        return spec;
    }

    public static ValidationSpec Starfield_AlikaVendorLowOrganicResources()
    {
        return StarfieldFormList("AlikaVendorLowOrganicResources", "2117F0:Starfield.esm", withName: false);
    }

    /// <summary>
    /// Builds the Starfield <c>COND_imgui_1_Assorted</c> form list validation spec, including a UI editor ID row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>COND_imgui_1_Assorted</c> sample.</returns>
    public static ValidationSpec Starfield_COND_imgui_1_Assorted()
    {
        var spec = StarfieldFormList("COND_imgui_1_Assorted", "0C3830:Starfield.esm", withName: true);
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(["EditorID"], "COND_imgui_1_Assorted"));
        return spec;
    }

    public static ValidationSpec Fallout4_CA_JunkItems()
    {
        return Fallout4FormList("CA_JunkItems", "246EE7:Fallout4.esm", withName: true);
    }

    public static ValidationSpec Fallout4_ChargenOptionsSortList()
    {
        return Fallout4FormList("ChargenOptionsSortList", "1A4AE8:Fallout4.esm", withName: false);
    }

    public static ValidationSpec Fallout4_CompanionCrime__Common()
    {
        return Fallout4FormList("CompanionCrime__Common", "2494E7:Fallout4.esm", withName: false);
    }

    public static ValidationSpec Fallout4_VoicesEmpty()
    {
        return Fallout4FormList("VoicesEmpty", "14EC02:Fallout4.esm", withName: true);
    }

    public static ValidationSpec Skyrim_AAAMothPlantTypes()
    {
        return SkyrimFormList("AAAMothPlantTypes", "06F3F7:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_CityWindhelmResidentList()
    {
        return SkyrimFormList("CityWindhelmResidentList", "045C32:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_CrimeFactionsList()
    {
        return SkyrimFormList("CrimeFactionsList", "026953:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_DraugrWeapons()
    {
        return SkyrimFormList("DraugrWeapons", "000D14:Skyrim.esm");
    }

    private static ValidationSpec StarfieldFormList(string sampleName, string formKey, bool withName)
    {
        var spec = BaseFormList(SpecificationGame.Starfield, sampleName, formKey);
        if (withName)
        {
            spec.AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true));
            spec.AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
        }

        return spec.Build();
    }

    private static ValidationSpec Fallout4FormList(string sampleName, string formKey, bool withName)
    {
        var spec = BaseFormList(SpecificationGame.Fallout4, sampleName, formKey);
        if (withName)
        {
            spec.AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true));
            spec.AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
        }

        return spec.Build();
    }

    private static ValidationSpec SkyrimFormList(string sampleName, string formKey)
    {
        return BaseFormList(SpecificationGame.Skyrim, sampleName, formKey)
            .Build();
    }

    private static ValidationSpecBuilder BaseFormList(SpecificationGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, SupportedRecordSpecifications.FormList)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddBaselineUiComparisonExpectations(
                new[] { "MajorRecordFlags" })
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "MajorRecordFlags",
                "MajorRecordFlags",
                "0",
                "Mutagen exposes the default MajorRecordFlags value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.FormKeyList("Items", "Items", "Item"));
    }
}
