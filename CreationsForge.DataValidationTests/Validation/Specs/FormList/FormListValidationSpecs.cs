using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.FormList;

public static class FormListValidationSpecs
{
    public static ValidationSpec Starfield_AkilaVendorVeryHighOrganicResources()
    {
        return StarfieldFormList("AkilaVendorVeryHighOrganicResources", "2117E6:Starfield.esm", withName: false);
    }

    public static ValidationSpec Starfield_AkilaVendorVeryLowOrganicResources()
    {
        return StarfieldFormList("AkilaVendorVeryLowOrganicResources", "2117EC:Starfield.esm", withName: false);
    }

    public static ValidationSpec Starfield_AlikaVendorLowOrganicResources()
    {
        return StarfieldFormList("AlikaVendorLowOrganicResources", "2117F0:Starfield.esm", withName: false);
    }

    public static ValidationSpec Starfield_COND_imgui_1_Assorted()
    {
        return StarfieldFormList("COND_imgui_1_Assorted", "0C3830:Starfield.esm", withName: true);
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
        var spec = BaseFormList(SupportedGame.Starfield, sampleName, formKey);
        if (withName)
        {
            spec.AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true));
            spec.AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
        }

        return spec.Build();
    }

    private static ValidationSpec Fallout4FormList(string sampleName, string formKey, bool withName)
    {
        var spec = BaseFormList(SupportedGame.Fallout4, sampleName, formKey);
        if (withName)
        {
            spec.AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true));
            spec.AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
        }

        return spec.Build();
    }

    private static ValidationSpec SkyrimFormList(string sampleName, string formKey)
    {
        return BaseFormList(SupportedGame.Skyrim, sampleName, formKey)
            .Build();
    }

    private static ValidationSpecBuilder BaseFormList(SupportedGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.FormList)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "MajorRecordFlags",
                "MajorRecordFlags",
                "0",
                "Mutagen exposes the default MajorRecordFlags value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.FormKeyList("Items", "Items", "Item"));
    }
}
