using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.Global;

public static class GlobalValidationSpecs
{
    public static ValidationSpec Starfield_UpdateShatteredSpaceMaster()
    {
        return Global(SupportedGame.Starfield, "_UpdateShatteredSpaceMaster", "20C81D:Starfield.esm");
    }

    public static ValidationSpec Starfield_2B7FBD_Starfield_esm()
    {
        return Global(SupportedGame.Starfield, "2B7FBD_Starfield.esm", "2B7FBD:Starfield.esm");
    }

    public static ValidationSpec Starfield_2B91E0_Starfield_esm()
    {
        return Global(SupportedGame.Starfield, "2B91E0_Starfield.esm", "2B91E0:Starfield.esm");
    }

    public static ValidationSpec Fallout4_AO_Companion_Search_JunkThresholdValue()
    {
        return Global(SupportedGame.Fallout4, "AO_Companion_Search_JunkThresholdValue", "18E889:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_AO_Companion_Search_NextAllowedDaysUntil()
    {
        return Global(SupportedGame.Fallout4, "AO_Companion_Search_NextAllowedDaysUntil", "176107:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_AO_Dogmeat_Container_Bailout_Dist()
    {
        return Global(SupportedGame.Fallout4, "AO_Dogmeat_Container_Bailout_Dist", "043F14:Fallout4.esm");
    }

    /// <summary>
    /// Builds the Skyrim <c>1stPKillCam</c> global validation spec, including a UI row expectation for the scalar data value.
    /// </summary>
    /// <returns>The validation spec for the Skyrim <c>1stPKillCam</c> sample.</returns>
    public static ValidationSpec Skyrim_1stPKillCam()
    {
        var spec = Global(SupportedGame.Skyrim, "1stPKillCam", "10636A:Skyrim.esm");
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.DtoField(["Data"], "Data"));
        return spec;
    }

    public static ValidationSpec Skyrim_CarriageCost()
    {
        var spec = Global(SupportedGame.Skyrim, "CarriageCost", "050765:Skyrim.esm");
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.DtoField(["Data"], "Data"));
        return spec;
    }

    /// <summary>
    /// Builds the Skyrim <c>CarriageCostSmall</c> global validation spec, including a UI row expectation for the scalar data value.
    /// </summary>
    /// <returns>The validation spec for the Skyrim <c>CarriageCostSmall</c> sample.</returns>
    public static ValidationSpec Skyrim_CarriageCostSmall()
    {
        var spec = Global(SupportedGame.Skyrim, "CarriageCostSmall", "107702:Skyrim.esm");
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.DtoField(["Data"], "Data"));
        return spec;
    }

    private static ValidationSpec Global(SupportedGame game, string sampleName, string formKey)
    {
        var gameMajorRecordFlags = game switch
        {
            SupportedGame.Fallout4 => "Fallout4MajorRecordFlags",
            SupportedGame.Skyrim => "SkyrimMajorRecordFlags",
            _ => "StarfieldMajorRecordFlags"
        };

        return ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.Global)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"))
            .AddRule(ValidationFieldRule.ScalarList(gameMajorRecordFlags, "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "EditorID",
                "EditorID",
                string.Empty,
                "Mutagen exposes an empty EditorID string when Spriggit omits the field.",
                allowEmptyExpectedValue: true))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Version2",
                "Version2",
                "0",
                "Mutagen exposes the default Version2 value when Spriggit omits the zero-valued field."))
            .Build();
    }
}
