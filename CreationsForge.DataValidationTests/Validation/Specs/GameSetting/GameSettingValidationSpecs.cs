using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.GameSetting;

public static class GameSettingValidationSpecs
{
    public static ValidationSpec Starfield_sAbort()
    {
        return GameSetting(SupportedGame.Starfield, "sAbort", "0657E0:Starfield.esm", GameSettingDataType.String);
    }

    public static ValidationSpec Starfield_sActivate()
    {
        return GameSetting(SupportedGame.Starfield, "sActivate", "0D4DFC:Starfield.esm", GameSettingDataType.String);
    }

    public static ValidationSpec Starfield_sActivateCreatureCalmed()
    {
        return GameSetting(SupportedGame.Starfield, "sActivateCreatureCalmed", "0D4DEB:Starfield.esm", GameSettingDataType.String);
    }

    public static ValidationSpec Starfield_bAllowBlinksDuringSpeech()
    {
        return GameSetting(SupportedGame.Starfield, "bAllowBlinksDuringSpeech", "0F9CFD:Starfield.esm", GameSettingDataType.Boolean);
    }

    public static ValidationSpec Starfield_bBoostpackInitialThrustOnlyOnTakeoff()
    {
        return GameSetting(SupportedGame.Starfield, "bBoostpackInitialThrustOnlyOnTakeoff", "024CA5:Starfield.esm", GameSettingDataType.Boolean);
    }

    public static ValidationSpec Starfield_fActorDefaultTurningSpeed()
    {
        return GameSetting(SupportedGame.Starfield, "fActorDefaultTurningSpeed", "101046:Starfield.esm", GameSettingDataType.Float);
    }

    public static ValidationSpec Starfield_fActorSwimBreathDamage()
    {
        return GameSetting(SupportedGame.Starfield, "fActorSwimBreathDamage", "097F48:Starfield.esm", GameSettingDataType.Float);
    }

    public static ValidationSpec Starfield_iAICombatRestoreHealthPercentage()
    {
        return GameSetting(SupportedGame.Starfield, "iAICombatRestoreHealthPercentage", "01A237:Starfield.esm", GameSettingDataType.Integer);
    }

    public static ValidationSpec Starfield_iAIMaxSocialDistanceToTriggerEvent()
    {
        return GameSetting(SupportedGame.Starfield, "iAIMaxSocialDistanceToTriggerEvent", "003207:Starfield.esm", GameSettingDataType.Integer);
    }

    public static ValidationSpec Starfield_uDefaultLevelZone01max()
    {
        return GameSetting(SupportedGame.Starfield, "uDefaultLevelZone01max", "246BD8:Starfield.esm", GameSettingDataType.UnsignedInteger);
    }

    public static ValidationSpec Starfield_uDefaultLevelZone02min()
    {
        return GameSetting(SupportedGame.Starfield, "uDefaultLevelZone02min", "246BD9:Starfield.esm", GameSettingDataType.UnsignedInteger);
    }

    public static ValidationSpec Fallout4_sAbortText()
    {
        return GameSetting(SupportedGame.Fallout4, "sAbortText", "0D4C40:Fallout4.esm", GameSettingDataType.String);
    }

    public static ValidationSpec Fallout4_sAccept()
    {
        return GameSetting(SupportedGame.Fallout4, "sAccept", "0D4DC4:Fallout4.esm", GameSettingDataType.String);
    }

    public static ValidationSpec Fallout4_sActivate()
    {
        return GameSetting(SupportedGame.Fallout4, "sActivate", "0D4DFC:Fallout4.esm", GameSettingDataType.String);
    }

    public static ValidationSpec Fallout4_bAllowBlinksDuringSpeech()
    {
        return GameSetting(SupportedGame.Fallout4, "bAllowBlinksDuringSpeech", "0F9CFD:Fallout4.esm", GameSettingDataType.Boolean);
    }

    public static ValidationSpec Fallout4_fActionPointsAttackOneHandMelee()
    {
        return GameSetting(SupportedGame.Fallout4, "fActionPointsAttackOneHandMelee", "01A145:Fallout4.esm", GameSettingDataType.Float);
    }

    public static ValidationSpec Fallout4_fActionPointsAttackRanged()
    {
        return GameSetting(SupportedGame.Fallout4, "fActionPointsAttackRanged", "08A207:Fallout4.esm", GameSettingDataType.Float);
    }

    public static ValidationSpec Fallout4_iAICombatRestoreHealthPercentage()
    {
        return GameSetting(SupportedGame.Fallout4, "iAICombatRestoreHealthPercentage", "01A237:Fallout4.esm", GameSettingDataType.Integer);
    }

    public static ValidationSpec Fallout4_iAISocialDistanceToTriggerEvent()
    {
        return GameSetting(SupportedGame.Fallout4, "iAISocialDistanceToTriggerEvent", "01A83A:Fallout4.esm", GameSettingDataType.Integer);
    }

    public static ValidationSpec Fallout4_uDefaultLevelZone01max()
    {
        return GameSetting(SupportedGame.Fallout4, "uDefaultLevelZone01max", "246BD8:Fallout4.esm", GameSettingDataType.UnsignedInteger);
    }

    public static ValidationSpec Fallout4_uDefaultLevelZone02min()
    {
        return GameSetting(SupportedGame.Fallout4, "uDefaultLevelZone02min", "246BD9:Fallout4.esm", GameSettingDataType.UnsignedInteger);
    }

    public static ValidationSpec Skyrim_sAbortText()
    {
        return GameSetting(SupportedGame.Skyrim, "sAbortText", "0D4C40:Skyrim.esm", GameSettingDataType.String);
    }

    public static ValidationSpec Skyrim_sAccept()
    {
        return GameSetting(SupportedGame.Skyrim, "sAccept", "0D4DC4:Skyrim.esm", GameSettingDataType.String);
    }

    public static ValidationSpec Skyrim_sActionMapping()
    {
        return GameSetting(SupportedGame.Skyrim, "sActionMapping", "0D4B96:Skyrim.esm", GameSettingDataType.String);
    }

    public static ValidationSpec Skyrim_bRegenNPCMagickaDuringCast()
    {
        return GameSetting(SupportedGame.Skyrim, "bRegenNPCMagickaDuringCast", "0B3D8A:Skyrim.esm", GameSettingDataType.Boolean);
    }

    public static ValidationSpec Skyrim_fActionPointsAimAdjustment()
    {
        return GameSetting(SupportedGame.Skyrim, "fActionPointsAimAdjustment", "01A144:Skyrim.esm", GameSettingDataType.Float);
    }

    public static ValidationSpec Skyrim_fActionPointsAttackOneHandMelee()
    {
        return GameSetting(SupportedGame.Skyrim, "fActionPointsAttackOneHandMelee", "01A145:Skyrim.esm", GameSettingDataType.Float);
    }

    public static ValidationSpec Skyrim_iAICombatRestoreHealthPercentage()
    {
        return GameSetting(SupportedGame.Skyrim, "iAICombatRestoreHealthPercentage", "01A237:Skyrim.esm", GameSettingDataType.Integer);
    }

    public static ValidationSpec Skyrim_iAISocialDistanceToTriggerEvent()
    {
        return GameSetting(SupportedGame.Skyrim, "iAISocialDistanceToTriggerEvent", "01A83A:Skyrim.esm", GameSettingDataType.Integer);
    }

    private static ValidationSpec GameSetting(
        SupportedGame game,
        string sampleName,
        string formKey,
        GameSettingDataType dataType)
    {
        var spec = ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.GameSetting)
            .Sample(sampleName)
            .FormKey(formKey)
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
            case GameSettingDataType.Boolean:
                spec.AddRule(ValidationFieldRule.Field("Data", "Data.Boolean"));
                break;
            case GameSettingDataType.Float:
                spec.AddRule(ValidationFieldRule.Field("Data", "Data.Float", ValidationValueNormalizer.DecimalNumber));
                break;
            case GameSettingDataType.Integer:
                spec.AddRule(ValidationFieldRule.Field("Data", "Data.Integer"));
                break;
            case GameSettingDataType.String:
                spec.AddRule(ValidationFieldRule.TranslatedField("Data", "Data.String", requireAllLanguages: true));
                spec.AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
                break;
            case GameSettingDataType.UnsignedInteger:
                spec.AddRule(ValidationFieldRule.Field("Data", "Data.UnsignedInteger"));
                break;
            default:
                throw new InvalidOperationException("Unsupported game setting data type '" + dataType + "'.");
        }

        return spec.Build();
    }

    private static string GetMutagenObjectType(GameSettingDataType dataType)
    {
        return dataType switch
        {
            GameSettingDataType.Boolean => "GameSettingBool",
            GameSettingDataType.Float => "GameSettingFloat",
            GameSettingDataType.Integer => "GameSettingInt",
            GameSettingDataType.String => "GameSettingString",
            GameSettingDataType.UnsignedInteger => "GameSettingUInt",
            _ => dataType.ToString()
        };
    }
}
