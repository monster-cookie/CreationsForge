using System.Globalization;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Utilities;
using CreationsForge.Fallout4.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Fallout4;

public class Fallout4RecordReaderService : IFallout4RecordReaderService
{
    private readonly Fallout4GameMetadataService GameMetadataService;

    public Fallout4RecordReaderService(Fallout4GameMetadataService gameMetadataService)
    {
        GameMetadataService = gameMetadataService;
    }

    public PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var mod = LoadMod(plugin);
        cancellationToken.ThrowIfCancellationRequested();
        var formLists = MapFormLists(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var gameSettings = MapGameSettings(plugin, mod);
        cancellationToken.ThrowIfCancellationRequested();
        var globals = MapGlobals(plugin, mod);

        return new PluginRecordSetDTO
        {
            FormLists = formLists,
            GameSettings = gameSettings,
            Globals = globals
        };
    }

    public IReadOnlyList<FormListDTO> ReadFormLists(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapFormLists(plugin, mod);
    }

    private static IReadOnlyList<FormListDTO> MapFormLists(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return mod.FormLists
            .Select(record => new FormListDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.Fallout4MajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Items = record.Items.Select((item, itemIndex) => new FormListItemDTO
                {
                    Game = SupportedGame.Fallout4,
                    ModKey = plugin.ModKey,
                    FormKey = MapFormKey(record.FormKey),
                    ItemFormKey = MapFormKey(item.FormKey),
                    ItemIndex = itemIndex,
                    ImportedAtUTC = DateTime.UtcNow
                }).ToList()
            })
            .ToList();
    }

    public IReadOnlyList<GameSettingDTO> ReadGameSettings(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapGameSettings(plugin, mod);
    }

    private static IReadOnlyList<GameSettingDTO> MapGameSettings(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return mod.GameSettings
            .Select(record => new GameSettingDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.Fallout4MajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                SettingType = GetGameSettingType(record),
                Data = GetGameSettingData(record),
                NumericData = GetGameSettingNumericData(record),
                IntegerData = GetGameSettingIntegerData(record),
                BooleanData = GetGameSettingBooleanData(record)
            })
            .ToList();
    }

    public IReadOnlyList<GlobalDTO> ReadGlobals(PluginDTO plugin)
    {
        var mod = LoadMod(plugin);
        return MapGlobals(plugin, mod);
    }

    private static IReadOnlyList<GlobalDTO> MapGlobals(PluginDTO plugin, IFallout4ModGetter mod)
    {
        return mod.Globals
            .Select(record => new GlobalDTO
            {
                Game = SupportedGame.Fallout4,
                ModKey = plugin.ModKey,
                FormKey = MapFormKey(record.FormKey),
                EditorID = record.EditorID ?? string.Empty,
                FormVersion = record.FormVersion,
                MajorRecordFlags = (int)record.Fallout4MajorRecordFlags,
                ImportedAtUTC = DateTime.UtcNow,
                Data = GetGlobalData(record)
            })
            .ToList();
    }

    protected virtual IFallout4ModGetter LoadMod(PluginDTO plugin)
    {
        var dataFolderPath = GetDataFolderPath();
        return Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(Path.Combine(dataFolderPath, plugin.ModKey.FileName))
            .WithDataFolder(dataFolderPath)
            .Construct();
    }

    private string GetDataFolderPath()
    {
        var environment = GameEnvironment.Typical.Fallout4(Fallout4Release.Fallout4);
        return environment.DataFolderPath;
    }

    private static FormKeyDTO MapFormKey(FormKey formKey)
    {
        return new FormKeyDTO
        {
            ModKey = ModKeyDTOMapper.FromModKey(formKey.ModKey),
            Id = formKey.ID
        };
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

    private static double? GetGameSettingNumericData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingFloatGetter gameSetting => gameSetting.Data,
            IGameSettingIntGetter gameSetting => gameSetting.Data,
            IGameSettingUIntGetter gameSetting => gameSetting.Data,
            _ => null
        };
    }

    private static int? GetGameSettingIntegerData(IGameSettingGetter record)
    {
        return record switch
        {
            IGameSettingIntGetter gameSetting => gameSetting.Data,
            IGameSettingUIntGetter gameSetting when gameSetting.Data <= int.MaxValue => (int)gameSetting.Data,
            _ => null
        };
    }

    private static bool? GetGameSettingBooleanData(IGameSettingGetter record)
    {
        return record is IGameSettingBoolGetter gameSetting ? gameSetting.Data : null;
    }

    private static double? GetGlobalData(IGlobalGetter record)
    {
        var rawFloat = record switch
        {
            GlobalFloat global => global.RawFloat,
            GlobalInt global => global.RawFloat,
            GlobalShort global => global.RawFloat,
            Global global => global.RawFloat,
            _ => null
        };

        if (rawFloat.HasValue) return rawFloat;

        var rawFloatProperty = record.GetType().GetProperty("RawFloat");
        return rawFloatProperty?.GetValue(record) is float value ? value : null;
    }
}
