using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginService : IPluginService
{
    private readonly ILogger Logger = Log.ForContext<PluginService>();
    private readonly IGameConfigurationStore GameConfigurationStore;

    public PluginService(IGameConfigurationStore gameConfigurationStore)
    {
        GameConfigurationStore = gameConfigurationStore;
    }

    /// <inheritdoc />
    public IList<string> GetRecordTypes()
    {
        switch (GameConfigurationStore.SelectedGame)
        {
            case "Starfield":
                return MajorRecordTypeEnumerator
                    .GetMajorRecordTypesFor(GameCategory.Starfield)
                    .OrderBy(x => x.ClassType.Name)
                    .Select(x => x.ClassType.Name)
                    .ToList();
            case "Skyrim":
                return MajorRecordTypeEnumerator
                    .GetMajorRecordTypesFor(GameCategory.Skyrim)
                    .OrderBy(x => x.ClassType.Name)
                    .Select(x => x.ClassType.Name)
                    .ToList();
            case "Fallout4":
                return MajorRecordTypeEnumerator
                    .GetMajorRecordTypesFor(GameCategory.Fallout4)
                    .OrderBy(x => x.ClassType.Name)
                    .Select(x => x.ClassType.Name)
                    .ToList();
            default:
                Logger.Error("Game '{Game}' is not supported. Please select Starfield", GameConfigurationStore.SelectedGame);
                return new List<string>();
        }
    }
    
    /// <inheritdoc />
    public IList<string> GetPlugins()
    {
        try
        {
            var gameEnvironment = GameConfigurationStore.Game;
            if (gameEnvironment is null)
            {
                Logger.Warning("Unable to load plugins because no game environment is configured");
                return new List<string>();
            }

            var plugins = new List<string>();
            foreach (var plugin in gameEnvironment.LoadOrder.ListedOrder)
            {
                // Exclude the base game database as all other records automatically compare to it.
                if (plugin.FileName.Equals("Starfield.esm", StringComparison.CurrentCultureIgnoreCase)) continue;
                plugins.Add(plugin.FileName);
            }

            return plugins;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load plugins");
            return new List<string>();
        }
    }

    public PluginHeaderDTO? GetPluginHeader(string pluginName)
    {
        try
        {
            var plugin = LoadPlugin(pluginName);
            return new PluginHeaderDTO(pluginName, plugin.ModHeader);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load plugin header for {PluginName}", pluginName);
            return null;
        }
    }

    /// <inheritdoc />
    public IList<RecordSummaryDTO> GetRecords(string pluginName, string recordType)
    {
        try
        {
            var plugin = LoadPlugin(pluginName);
            var records = GetRecordsFromMutagenTypeOption(plugin, recordType) ?? GetRecordsFromPluginProperty(plugin, recordType);
            if (records is null)
            {
                Logger.Warning("Record type {RecordType} was not found for plugin {PluginName}", recordType, pluginName);
                return new List<RecordSummaryDTO>();
            }

            return records
                .Cast<object>()
                .Select(record => new RecordSummaryDTO
                {
                    FormID = GetStringValue(record, "FormKey") ?? GetStringValue(record, "FormID"),
                    EditorID = GetStringValue(record, "EditorID")
                })
                .ToList();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load {RecordType} records for {PluginName}", recordType, pluginName);
            return new List<RecordSummaryDTO>();
        }
    }

    private IStarfieldModDisposableGetter LoadPlugin(string pluginName)
    {
        var gameEnvironment = GameConfigurationStore.Game
            ?? throw new InvalidOperationException("No game environment is configured.");
        var pluginPath = Path.Combine(gameEnvironment.DataFolderPath.Path, pluginName);
        var modKey = ModKey.FromFileName(Path.GetFileName(pluginPath));
        var modPath = new ModPath(modKey, pluginPath);

        return StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(gameEnvironment.DataFolderPath.Path)
            .Construct();
    }

    private static string? GetStringValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);
        return value?.ToString();
    }

    private static System.Collections.IEnumerable? GetRecordsFromMutagenTypeOption(
        IStarfieldModGetter plugin,
        string recordType)
    {
        var method = typeof(TypeOptionSolidifierMixIns)
            .GetMethods()
            .Where(method => method.Name.Equals(recordType, StringComparison.Ordinal))
            .FirstOrDefault(method =>
            {
                var parameters = method.GetParameters();
                return parameters.Length == 1
                       && parameters[0].ParameterType.IsAssignableFrom(typeof(IEnumerable<IStarfieldModGetter>));
            });

        return method?.Invoke(null, [new[] { plugin }]) as System.Collections.IEnumerable;
    }

    private static System.Collections.IEnumerable? GetRecordsFromPluginProperty(
        IStarfieldModGetter plugin,
        string recordType)
    {
        var propertyNames = new[]
        {
            recordType,
            $"{recordType}s",
            recordType.EndsWith("y", StringComparison.OrdinalIgnoreCase)
                ? $"{recordType[..^1]}ies"
                : $"{recordType}s"
        };

        foreach (var propertyName in propertyNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var property = plugin.GetType().GetProperty(propertyName);
            if (property?.GetValue(plugin) is System.Collections.IEnumerable records)
            {
                return records;
            }
        }

        return null;
    }
}
