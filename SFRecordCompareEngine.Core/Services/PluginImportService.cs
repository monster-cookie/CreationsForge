using System.Configuration;
using System.Globalization;
using System.IO;
using Mutagen.Bethesda.Starfield;
using NPoco;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginImportService : IPluginImportService
{
    private readonly ILogger Logger = Log.ForContext<PluginImportService>();
    
    private readonly IGameConfigurationStore GameConfigurationStore;

    private readonly IDatabaseSchemaInitializer DatabaseSchemaInitializer;
    private readonly IDatabase Database;
    private readonly IPluginService PluginService;
    private readonly IPluginRepository PluginRepository;

    public PluginImportService(
        IDatabaseSchemaInitializer databaseSchemaInitializer, 
        IDatabase database, 
        IPluginService pluginService,
        IPluginRepository pluginRepository,
        IGameConfigurationStore gameConfigurationStore
    )
    {
        DatabaseSchemaInitializer = databaseSchemaInitializer;
        Database = database;
        PluginService = pluginService;
        PluginRepository = pluginRepository;
        GameConfigurationStore = gameConfigurationStore;
    }

    public Task<PluginImportResultDTO> InitializeAndImportAsync(IProgress<PluginImportProgressDTO>? progress, CancellationToken cancellationToken)
    {
        return Task.Run(() => InitializeAndImport(progress, cancellationToken), cancellationToken);
    }

    private PluginImportResultDTO InitializeAndImport(IProgress<PluginImportProgressDTO>? progress, CancellationToken cancellationToken)
    {
        progress?.Report(new PluginImportProgressDTO
        {
            StatusText = "Initializing plugin database schema...",
            IsIndeterminate = true
        });
        
        DatabaseSchemaInitializer.Initialize();
        Logger.Information("Initialized SQLite database, starting import of plugins");
        
        var result = new PluginImportResultDTO();
        
        var loadOrderEntries = PluginService.GetLoadOrder();
        result.PluginsDiscovered = loadOrderEntries.Count;
        progress?.Report(new PluginImportProgressDTO
        {
            PluginCount = loadOrderEntries.Count,
            StatusText = $"Discovered {loadOrderEntries.Count} plugins.",
            IsIndeterminate = loadOrderEntries.Count == 0
        });
        
        // Need a new connection because of the transaction scope and to ensure the connection is properly disposed after the transaction completes
        using var transaction = Database.GetTransaction();

        var checkedAtUtc = FormatUtc(DateTimeOffset.UtcNow);

        foreach (var entry in loadOrderEntries)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            progress?.Report(new PluginImportProgressDTO
            {
                CurrentPluginName = entry.PluginFileName,
                CurrentModKey = entry.ModKey,
                PluginIndex = entry.LoadOrderIndex,
                PluginCount = loadOrderEntries.Count,
                StatusText = $"Checking {entry.PluginFileName} ({entry.LoadOrderIndex} of {loadOrderEntries.Count})...",
                IsIndeterminate = false
            });
            
            ImportPlugin(transaction, entry, checkedAtUtc, result, progress, loadOrderEntries.Count, cancellationToken);
        }
        
        // Complete and close the transaction scope
        transaction.Complete();
        
        progress?.Report(new PluginImportProgressDTO
        {
            PluginIndex = loadOrderEntries.Count,
            PluginCount = loadOrderEntries.Count,
            StatusText = $"Plugin database import completed. Imported {result.PluginsImported} plugins.",
            IsIndeterminate = false
        });

        Logger.Information("Plugin import completed: discovered {PluginsDiscovered}, unchanged {PluginsUnchanged}, changed {PluginsChanged}, invalidated {PluginsInvalidated}, imported {PluginsImported}, missing {PluginsMissing}, failed {PluginsFailed}, unsupported {PluginsUnsupported}, master references {MasterReferencesImported}, record headers {RecordHeadersImported}, typed detail rows {TypedRecordDetailRowsImported}, FormList items {FormListItemsImported}, record failures {RecordImportFailures}, unsupported record types {UnsupportedRecordTypes}",
            result.PluginsDiscovered,
            result.PluginsUnchanged,
            result.PluginsChanged,
            result.PluginsInvalidated,
            result.PluginsImported,
            result.PluginsMissing,
            result.PluginsFailed,
            result.PluginsUnsupported,
            result.MasterReferencesImported,
            result.RecordHeadersImported,
            result.TypedRecordDetailRowsImported,
            result.FormListItemsImported,
            result.RecordImportFailures,
            result.UnsupportedRecordTypes);        
        
        return result;
    }

    private void ImportPlugin(ITransaction transaction, PluginLoadOrderEntryDTO entry, string checkedAtUtc, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        if (GameConfigurationStore.SelectedGame == null) throw new ConfigurationErrorsException("No game selected in configuration (SelectedGame is null)");
        if (GameConfigurationStore.Game == null) throw new ConfigurationErrorsException("No game selected in configuration (Game is null)");
        if (GameConfigurationStore.Release == null) throw new ConfigurationErrorsException("No game selected in configuration (Release is null)");

        switch (GameConfigurationStore.SelectedGame)
        {
            case "Skyrim":
                ImportSkyrimPlugin(entry, checkedAtUtc, result, progress, totalPlugins, cancellationToken);
                break;
            case "Starfield":
                ImportStarfieldPlugin(entry, checkedAtUtc, result, progress, totalPlugins, cancellationToken);
                break;
            case "Fallout4":
                ImportFallout4Plugin(entry, checkedAtUtc, result, progress, totalPlugins, cancellationToken);
                break;
            default:
                throw new ConfigurationErrorsException($"Unsupported game: {GameConfigurationStore.SelectedGame}");
        }

    }

    private void ImportStarfieldPlugin(PluginLoadOrderEntryDTO entry, string checkedAtUtc, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        if (GameConfigurationStore.SelectedGame == null) throw new ConfigurationErrorsException("No game selected in configuration (SelectedGame is null)");
        if (GameConfigurationStore.Game == null) throw new ConfigurationErrorsException("No game selected in configuration (Game is null)");
        if (GameConfigurationStore.Release == null) throw new ConfigurationErrorsException("No game selected in configuration (Release is null)");

        var existingPlugin = PluginRepository.GetByModKey(entry.ModKey);
        var fileInfo = new FileInfo(entry.PluginPath);
        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(entry.PluginPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(GameConfigurationStore.Game.DataFolderPath)
            .Construct();
        
        if (IsUnsupportedPlugin(entry))
        {
            result.PluginsUnsupported++;
            Logger.Information("Skipping unsupported Starfield plugin {ModKey} from {PluginPath}", entry.ModKey, entry.PluginPath);

            var unsupportedPluginDTO = new PluginDTO
            {
                ModKey = entry.ModKey,
                GameRelease = GameConfigurationStore.SelectedGame ?? "None",
                LoadOrderIndex = entry.LoadOrderIndex,
                PluginFileName = entry.PluginFileName,
                PluginPath = entry.PluginPath,
                Enabled = entry.Enabled,
                ExistsOnDisk = fileInfo.Exists,
                ImportState = nameof(PluginImportState.Unsupported),
                HeaderFlags = mod.ModHeader.Flags,
                FormVersion = mod.ModHeader.FormVersion,
                Author = mod.ModHeader.Author ?? "Unknown",
                LastCheckedUtc = DateTime.UtcNow
            };
            
            PluginRepository.UpsertPlugin(unsupportedPluginDTO);
            return;
        }
        
        if (!fileInfo.Exists)
        {
            result.PluginsMissing++;

            var missingPluginDTO = new PluginDTO
            {
                ModKey = entry.ModKey,
                GameRelease = GameConfigurationStore.SelectedGame ?? "None",
                LoadOrderIndex = entry.LoadOrderIndex,
                PluginFileName = entry.PluginFileName,
                PluginPath = entry.PluginPath,
                Enabled = entry.Enabled,
                ExistsOnDisk = fileInfo.Exists,
                ImportState = nameof(PluginImportState.Unsupported),
                HeaderFlags = mod.ModHeader.Flags,
                FormVersion = mod.ModHeader.FormVersion,
                Author = mod.ModHeader.Author ?? "Unknown",
                LastCheckedUtc = DateTime.UtcNow
            };
            
            PluginRepository.UpsertPlugin(missingPluginDTO);
            return;
        }

        var sourceLastWriteUtcTicks = fileInfo.LastWriteTimeUtc.Ticks;
        var sourceFileSizeBytes = fileInfo.Length;
        var isUnchanged = existingPlugin is not null && existingPlugin.SourceLastWriteUtcTicks == sourceLastWriteUtcTicks && existingPlugin.SourceFileSizeBytes == sourceFileSizeBytes;
        if (isUnchanged)
        {
            result.PluginsUnchanged++;
            Logger.Information("Skipping unchanged plugin {ModKey}: source last write ticks {SourceLastWriteUtcTicks}, source file size {SourceFileSizeBytes}, import state {ImportState}", entry.ModKey, sourceLastWriteUtcTicks, sourceFileSizeBytes, existingPlugin!.ImportState);
            
            existingPlugin.LastCheckedUtc = DateTime.UtcNow;
            
            PluginRepository.UpsertPlugin(existingPlugin);
            return;
        }
        
        if (existingPlugin is not null)
        {
            result.PluginsChanged++;
            result.PluginsInvalidated++;
            Logger.Information("Plugin {ModKey} changed: stored last write ticks {StoredLastWriteUtcTicks}, current last write ticks {CurrentLastWriteUtcTicks}, stored file size {StoredFileSizeBytes}, current file size {CurrentFileSizeBytes}, stored import state {ImportState}",
                entry.ModKey,
                existingPlugin.SourceLastWriteUtcTicks,
                sourceLastWriteUtcTicks,
                existingPlugin.SourceFileSizeBytes,
                sourceFileSizeBytes,
                existingPlugin.ImportState);
        }

        try
        {
            progress?.Report(new PluginImportProgressDTO
            {
                CurrentPluginName = entry.PluginFileName,
                CurrentModKey = entry.ModKey,
                PluginIndex = entry.LoadOrderIndex,
                PluginCount = totalPlugins,
                StatusText = $"Importing changed or new plugin {entry.PluginFileName} ({entry.LoadOrderIndex} of {totalPlugins})...",
                IsIndeterminate = false
            });

            PluginDTO dto;
            if (existingPlugin is not null)
            {
                dto = existingPlugin;
                dto.LoadOrderIndex = entry.LoadOrderIndex;
                dto.PluginFileName = entry.PluginFileName;
                dto.PluginPath = entry.PluginPath;
                dto.Enabled = entry.Enabled;
                dto.ExistsOnDisk = fileInfo.Exists;
                dto.ImportState = nameof(PluginImportState.Current);
                dto.HeaderFlags = mod.ModHeader.Flags;
                dto.FormVersion = mod.ModHeader.FormVersion;
                dto.Author = mod.ModHeader.Author ?? "Unknown";
                dto.LastCheckedUtc = DateTime.UtcNow;
                dto.LastImportedUtc = DateTime.UtcNow;
                dto.InvalidatedAtUtc = null;
                dto.SourceLastWriteUtcTicks = sourceLastWriteUtcTicks;
                dto.SourceFileSizeBytes = sourceFileSizeBytes;
            }
            else
            {
                dto = new PluginDTO
                {
                    ModKey = entry.ModKey,
                    GameRelease = GameConfigurationStore.SelectedGame ?? "None",
                    LoadOrderIndex = entry.LoadOrderIndex,
                    PluginFileName = entry.PluginFileName,
                    PluginPath = entry.PluginPath,
                    Enabled = entry.Enabled,
                    ExistsOnDisk = fileInfo.Exists,
                    ImportState = nameof(PluginImportState.Current),
                    HeaderFlags = mod.ModHeader.Flags,
                    FormVersion = mod.ModHeader.FormVersion,
                    Author = mod.ModHeader.Author ?? "Unknown",
                    LastCheckedUtc = DateTime.UtcNow,
                    LastImportedUtc = DateTime.UtcNow,
                    InvalidatedAtUtc = null,
                    SourceLastWriteUtcTicks = sourceLastWriteUtcTicks,
                    SourceFileSizeBytes = sourceFileSizeBytes
                };
            }
            
            PluginRepository.UpsertPlugin(dto);
            result.PluginsImported++;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            result.PluginsFailed++;
            Logger.Error(ex, "Unable to import plugin metadata for {ModKey} from {PluginPath}", entry.ModKey, entry.PluginPath);

            PluginDTO erroredPluginDTO;
            if (existingPlugin is not null)
            {
                erroredPluginDTO = existingPlugin;
                erroredPluginDTO.ImportState = nameof(PluginImportState.Failed);
                erroredPluginDTO.LastCheckedUtc = DateTime.UtcNow;
                erroredPluginDTO.LastImportedUtc = existingPlugin?.LastImportedUtc;
                erroredPluginDTO.InvalidatedAtUtc = DateTime.UtcNow;
            }
            else
            {
                erroredPluginDTO = new PluginDTO
                {
                    ModKey = entry.ModKey,
                    GameRelease = GameConfigurationStore.SelectedGame ?? "None",
                    LoadOrderIndex = entry.LoadOrderIndex,
                    PluginFileName = entry.PluginFileName,
                    PluginPath = entry.PluginPath,
                    Enabled = entry.Enabled,
                    ExistsOnDisk = fileInfo.Exists,
                    ImportState = nameof(PluginImportState.Failed),
                    HeaderFlags = mod.ModHeader.Flags,
                    FormVersion = mod.ModHeader.FormVersion,
                    Author = mod.ModHeader.Author ?? "Unknown",
                    LastCheckedUtc = DateTime.UtcNow,
                    LastImportedUtc = existingPlugin?.LastImportedUtc,
                    InvalidatedAtUtc = DateTime.UtcNow
                };
            }

            PluginRepository.UpsertPlugin(erroredPluginDTO);
        }
        
    }

    private void ImportFallout4Plugin(PluginLoadOrderEntryDTO entry, string checkedAtUtc, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private void ImportSkyrimPlugin(PluginLoadOrderEntryDTO entry, string checkedAtUtc, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private static bool IsUnsupportedPlugin(PluginLoadOrderEntryDTO loadOrderEntry)
    {
        var pluginFileName = string.IsNullOrWhiteSpace(loadOrderEntry.PluginFileName) ? loadOrderEntry.ModKey.FileName.ToString() : loadOrderEntry.PluginFileName;
        return pluginFileName.StartsWith("BlueprintShips", StringComparison.OrdinalIgnoreCase) && pluginFileName.EndsWith(".esm", StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatUtc(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.UtcDateTime.ToString("O", CultureInfo.InvariantCulture);
    }
}
