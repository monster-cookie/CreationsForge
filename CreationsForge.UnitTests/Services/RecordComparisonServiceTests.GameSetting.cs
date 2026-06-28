using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Specification.Records;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>
/// Contains record comparison scenarios for Game Setting records.
/// </summary>
public partial class RecordComparisonServiceTests
{
    /// <summary>
    /// Verifies that record comparison game setting hides redundant typed value fields.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForGameSetting_HidesRedundantTypedValueFields()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x456);
        var gameSettingRepository = new TestGameSettingRepository
        {
            Records =
            [
                CreateGameSetting("Base.esm", formKey, "fSetting", GameSettingDataType.Float, floatData: 1.25),
                CreateGameSetting("Patch.esp", formKey, "fSetting", GameSettingDataType.Float, floatData: 1.75)
            ]
        };
        var service = CreateService(gameSettingRepository: gameSettingRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.GameSetting.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "MutagenObjectType").Values.Select(value => value.DisplayValue).ShouldBe(["GameSettingFloat", "GameSettingFloat"]);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["1.25", "1.75"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "FloatData");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "IntegerData");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "UnsignedIntegerData");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "BooleanData");
    }

    /// <summary>
    /// Verifies that record comparison game setting uses selected localized data.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForGameSetting_UsesSelectedLocalizedData()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x457);
        var gameSettingRepository = new TestGameSettingRepository
        {
            Records =
            [
                CreateGameSetting("Base.esm", formKey, "sSetting", GameSettingDataType.String, stringData: "Base English"),
                CreateGameSetting("Patch.esp", formKey, "sSetting", GameSettingDataType.String, stringData: "Patch English")
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Data", "English", "Base English"),
                CreateLocalizedString("Base.esm", formKey, "Data", "German", "Base German"),
                CreateLocalizedString("Patch.esp", formKey, "Data", "English", "Patch English"),
                CreateLocalizedString("Patch.esp", formKey, "Data", "German", "Patch German")
            ]
        };
        var applicationSettingsService = new TestApplicationSettingsService { RecordTextLanguage = Language.German };
        var service = CreateService(
            gameSettingRepository: gameSettingRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            applicationSettingsService: applicationSettingsService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.GameSetting.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["Base German", "Patch German"]);
    }
}
