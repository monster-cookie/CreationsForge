using CreationsForge.Specification.Records;
using CreationsForge.Specification.Validation;

namespace CreationsForge.Specification.Validation.Specs.GameSetting;

public static class GameSettingValidationSpecs
{
    public static ValidationSpec Starfield_sAbort()
    {
        return GameSetting(SpecificationGame.Starfield, "sAbort", "0657E0:Starfield.esm", ValidationGameSettingDataType.String);
    }

    public static ValidationSpec Starfield_sActivate()
    {
        return GameSetting(SpecificationGame.Starfield, "sActivate", "0D4DFC:Starfield.esm", ValidationGameSettingDataType.String);
    }

    public static ValidationSpec Starfield_sActivateCreatureCalmed()
    {
        return GameSetting(SpecificationGame.Starfield, "sActivateCreatureCalmed", "0D4DEB:Starfield.esm", ValidationGameSettingDataType.String);
    }

    public static ValidationSpec Starfield_bAllowBlinksDuringSpeech()
    {
        return GameSetting(SpecificationGame.Starfield, "bAllowBlinksDuringSpeech", "0F9CFD:Starfield.esm", ValidationGameSettingDataType.Boolean);
    }

    public static ValidationSpec Starfield_bBoostpackInitialThrustOnlyOnTakeoff()
    {
        return GameSetting(SpecificationGame.Starfield, "bBoostpackInitialThrustOnlyOnTakeoff", "024CA5:Starfield.esm", ValidationGameSettingDataType.Boolean);
    }

    public static ValidationSpec Starfield_fActorDefaultTurningSpeed()
    {
        return GameSetting(SpecificationGame.Starfield, "fActorDefaultTurningSpeed", "101046:Starfield.esm", ValidationGameSettingDataType.Float);
    }

    public static ValidationSpec Starfield_fActorSwimBreathDamage()
    {
        return GameSetting(SpecificationGame.Starfield, "fActorSwimBreathDamage", "097F48:Starfield.esm", ValidationGameSettingDataType.Float);
    }

    public static ValidationSpec Starfield_iAICombatRestoreHealthPercentage()
    {
        return GameSetting(SpecificationGame.Starfield, "iAICombatRestoreHealthPercentage", "01A237:Starfield.esm", ValidationGameSettingDataType.Integer);
    }

    public static ValidationSpec Starfield_iAIMaxSocialDistanceToTriggerEvent()
    {
        return GameSetting(SpecificationGame.Starfield, "iAIMaxSocialDistanceToTriggerEvent", "003207:Starfield.esm", ValidationGameSettingDataType.Integer);
    }

    public static ValidationSpec Starfield_uDefaultLevelZone01max()
    {
        return GameSetting(SpecificationGame.Starfield, "uDefaultLevelZone01max", "246BD8:Starfield.esm", ValidationGameSettingDataType.UnsignedInteger);
    }

    public static ValidationSpec Starfield_uDefaultLevelZone02min()
    {
        return GameSetting(SpecificationGame.Starfield, "uDefaultLevelZone02min", "246BD9:Starfield.esm", ValidationGameSettingDataType.UnsignedInteger);
    }

    public static ValidationSpec Fallout4_sAbortText()
    {
        return GameSetting(SpecificationGame.Fallout4, "sAbortText", "0D4C40:Fallout4.esm", ValidationGameSettingDataType.String);
    }

    /// <summary>
    /// Builds the Fallout 4 <c>sAccept</c> game setting validation spec, including a UI row expectation for string data.
    /// </summary>
    /// <returns>The validation spec for the Fallout 4 <c>sAccept</c> sample.</returns>
    public static ValidationSpec Fallout4_sAccept()
    {
        var spec = GameSetting(SpecificationGame.Fallout4, "sAccept", "0D4DC4:Fallout4.esm", ValidationGameSettingDataType.String);
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Data"]));
        return spec;
    }

    public static ValidationSpec Fallout4_sActivate()
    {
        return GameSetting(SpecificationGame.Fallout4, "sActivate", "0D4DFC:Fallout4.esm", ValidationGameSettingDataType.String);
    }

    public static ValidationSpec Fallout4_bAllowBlinksDuringSpeech()
    {
        return GameSetting(SpecificationGame.Fallout4, "bAllowBlinksDuringSpeech", "0F9CFD:Fallout4.esm", ValidationGameSettingDataType.Boolean);
    }

    public static ValidationSpec Fallout4_fActionPointsAttackOneHandMelee()
    {
        return GameSetting(SpecificationGame.Fallout4, "fActionPointsAttackOneHandMelee", "01A145:Fallout4.esm", ValidationGameSettingDataType.Float);
    }

    /// <summary>
    /// Builds the Fallout 4 <c>fActionPointsAttackRanged</c> game setting validation spec, including a UI row expectation for float data.
    /// </summary>
    /// <returns>The validation spec for the Fallout 4 <c>fActionPointsAttackRanged</c> sample.</returns>
    public static ValidationSpec Fallout4_fActionPointsAttackRanged()
    {
        var spec = GameSetting(SpecificationGame.Fallout4, "fActionPointsAttackRanged", "08A207:Fallout4.esm", ValidationGameSettingDataType.Float);
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.DtoField(["Data"], "Data.Float"));
        return spec;
    }

    public static ValidationSpec Fallout4_iAICombatRestoreHealthPercentage()
    {
        var spec = GameSetting(SpecificationGame.Fallout4, "iAICombatRestoreHealthPercentage", "01A237:Fallout4.esm", ValidationGameSettingDataType.Integer);
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.DtoField(["Data"], "Data.Integer"));
        return spec;
    }

    public static ValidationSpec Fallout4_iAISocialDistanceToTriggerEvent()
    {
        return GameSetting(SpecificationGame.Fallout4, "iAISocialDistanceToTriggerEvent", "01A83A:Fallout4.esm", ValidationGameSettingDataType.Integer);
    }

    public static ValidationSpec Fallout4_uDefaultLevelZone01max()
    {
        return GameSetting(SpecificationGame.Fallout4, "uDefaultLevelZone01max", "246BD8:Fallout4.esm", ValidationGameSettingDataType.UnsignedInteger);
    }

    public static ValidationSpec Fallout4_uDefaultLevelZone02min()
    {
        return GameSetting(SpecificationGame.Fallout4, "uDefaultLevelZone02min", "246BD9:Fallout4.esm", ValidationGameSettingDataType.UnsignedInteger);
    }

    public static ValidationSpec Skyrim_sAbortText()
    {
        return GameSetting(SpecificationGame.Skyrim, "sAbortText", "0D4C40:Skyrim.esm", ValidationGameSettingDataType.String);
    }

    public static ValidationSpec Skyrim_sAccept()
    {
        return GameSetting(SpecificationGame.Skyrim, "sAccept", "0D4DC4:Skyrim.esm", ValidationGameSettingDataType.String);
    }

    public static ValidationSpec Skyrim_sActionMapping()
    {
        return GameSetting(SpecificationGame.Skyrim, "sActionMapping", "0D4B96:Skyrim.esm", ValidationGameSettingDataType.String);
    }

    public static ValidationSpec Skyrim_bRegenNPCMagickaDuringCast()
    {
        return GameSetting(SpecificationGame.Skyrim, "bRegenNPCMagickaDuringCast", "0B3D8A:Skyrim.esm", ValidationGameSettingDataType.Boolean);
    }

    public static ValidationSpec Skyrim_fActionPointsAimAdjustment()
    {
        return GameSetting(SpecificationGame.Skyrim, "fActionPointsAimAdjustment", "01A144:Skyrim.esm", ValidationGameSettingDataType.Float);
    }

    public static ValidationSpec Skyrim_fActionPointsAttackOneHandMelee()
    {
        return GameSetting(SpecificationGame.Skyrim, "fActionPointsAttackOneHandMelee", "01A145:Skyrim.esm", ValidationGameSettingDataType.Float);
    }

    public static ValidationSpec Skyrim_iAICombatRestoreHealthPercentage()
    {
        return GameSetting(SpecificationGame.Skyrim, "iAICombatRestoreHealthPercentage", "01A237:Skyrim.esm", ValidationGameSettingDataType.Integer);
    }

    public static ValidationSpec Skyrim_iAISocialDistanceToTriggerEvent()
    {
        return GameSetting(SpecificationGame.Skyrim, "iAISocialDistanceToTriggerEvent", "01A83A:Skyrim.esm", ValidationGameSettingDataType.Integer);
    }

    private static ValidationSpec GameSetting(
        SpecificationGame game,
        string sampleName,
        string formKey,
        ValidationGameSettingDataType dataType)
    {
        var spec = ValidationSpecBuilder
            .ForRecord(game, SupportedRecordSpecifications.GameSetting)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddBaselineUiComparisonExpectations(
                new[] { "Data" })
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "MajorRecordFlags",
                "MajorRecordFlags",
                "0",
                "Mutagen exposes the default MajorRecordFlags value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Version2",
                "Version2",
                "0",
                "Mutagen exposes the default Version2 value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.DtoExpectedValue("DataType", dataType.ToString()))
            .AddRule(ValidationFieldRule.DtoExpectedValue("Data.DataType", dataType.ToString()))
            .AddRule(ValidationFieldRule.DtoExpectedValue("Data.MutagenObjectType", GetMutagenObjectType(dataType)));

        switch (dataType)
        {
            case ValidationGameSettingDataType.Boolean:
                spec.AddRule(ValidationFieldRule.Field("Data", "Data.Boolean"));
                break;
            case ValidationGameSettingDataType.Float:
                spec.AddRule(ValidationFieldRule.Field("Data", "Data.Float", ValidationValueNormalizer.DecimalNumber));
                break;
            case ValidationGameSettingDataType.Integer:
                spec.AddRule(ValidationFieldRule.Field("Data", "Data.Integer"));
                break;
            case ValidationGameSettingDataType.String:
                spec.AddRule(ValidationFieldRule.TranslatedField("Data", "Data.String", requireAllLanguages: true));
                spec.AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
                break;
            case ValidationGameSettingDataType.UnsignedInteger:
                spec.AddRule(ValidationFieldRule.Field("Data", "Data.UnsignedInteger"));
                break;
            default:
                throw new InvalidOperationException("Unsupported game setting data type '" + dataType + "'.");
        }

        return spec.Build();
    }

    private static string GetMutagenObjectType(ValidationGameSettingDataType dataType)
    {
        return dataType switch
        {
            ValidationGameSettingDataType.Boolean => "GameSettingBool",
            ValidationGameSettingDataType.Float => "GameSettingFloat",
            ValidationGameSettingDataType.Integer => "GameSettingInt",
            ValidationGameSettingDataType.String => "GameSettingString",
            ValidationGameSettingDataType.UnsignedInteger => "GameSettingUInt",
            _ => dataType.ToString()
        };
    }

    /// <summary>
    /// Identifies the game-setting value shape expected by validation specs without referencing Core DTO enums.
    /// </summary>
    private enum ValidationGameSettingDataType
    {
        /// <summary>
        /// Indicates a Boolean game setting value.
        /// </summary>
        Boolean,

        /// <summary>
        /// Indicates a floating-point game setting value.
        /// </summary>
        Float,

        /// <summary>
        /// Indicates a signed integer game setting value.
        /// </summary>
        Integer,

        /// <summary>
        /// Indicates a localized string game setting value.
        /// </summary>
        String,

        /// <summary>
        /// Indicates an unsigned integer game setting value.
        /// </summary>
        UnsignedInteger
    }
}
