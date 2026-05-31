using System.Globalization;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class StarfieldRecordReaderService : IStarfieldRecordReaderService
{
    #region FormList

    public IReadOnlyList<FormListDTO> GetFormLists(PluginDTO plugin)
    {
        var mod = LoadMod(plugin.ModKey);
        return mod.FormLists.Select(record => new FormListDTO
        {
            ModKey = plugin.ModKey,
            FormKey = record.FormKey,
            EditorID = record.EditorID ?? string.Empty,
            FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags,
            Version2 = record.Version2,
            VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow,
            AddToListFormKey = record.AddToList.FormKey,
            Items = record.Items.Select(item =>
            {
                item.TryGetModKey(out var itemModKey);
                return new FormListItemDataDTO
                {
                    ItemModKey = itemModKey,
                    ItemFormKey = item.FormKey
                };
            }).ToList()
        }).ToList();
    }

    #endregion

    #region GameSettings

    public IReadOnlyList<GameSettingDTO> GetGameSettings(PluginDTO plugin)
    {
        var mod = LoadMod(plugin.ModKey);
        return mod.GameSettings.Select(record => new GameSettingDTO
        {
            ModKey = plugin.ModKey,
            FormKey = record.FormKey,
            EditorID = record.EditorID ?? string.Empty,
            FormVersion = record.FormVersion,
            StarfieldMajorRecordFlags = record.StarfieldMajorRecordFlags,
            Version2 = record.Version2,
            VersionControl = (int)record.VersionControl,
            ImportedAtUTC = DateTime.UtcNow,
            SettingType = GetGameSettingType(record),
            Data = GetGameSettingData(record),
            RawData = GetGameSettingRawData(record),
            IsCompressed = 0,
            IsDeleted = 0
        }).ToList();
    }

    private static string GetGameSettingType(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter => "GameSettingBool",
            IGameSettingFloatGetter => "GameSettingFloat",
            IGameSettingIntGetter => "GameSettingInt",
            IGameSettingStringGetter => "GameSettingString",
            IGameSettingUIntGetter => "GameSettingUInt",
            _ => record.GetType().Name
        };
    }

    private static string? GetGameSettingData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            IGameSettingFloatGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            IGameSettingIntGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            IGameSettingStringGetter gameSetting => gameSetting.Data?.ToString(),
            IGameSettingUIntGetter gameSetting => Convert.ToString(gameSetting.Data, CultureInfo.InvariantCulture),
            _ => null
        };
    }

    private static double? GetGameSettingRawData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingBoolGetter gameSetting => gameSetting.Data == true ? 1 : 0,
            IGameSettingFloatGetter gameSetting => gameSetting.Data,
            IGameSettingIntGetter gameSetting => gameSetting.Data,
            IGameSettingUIntGetter gameSetting => gameSetting.Data,
            _ => null
        };
    }

    private static IStarfieldModGetter LoadMod(ModKey modKey)
    {
        var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(Path.Join(environment.DataFolderPath, modKey.FileName))
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(environment.DataFolderPath)
            .Construct();
        return mod;
    }

    #endregion
}