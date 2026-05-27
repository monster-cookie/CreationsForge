using System.Configuration;
using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Starfield;
using NPoco;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginImportService : IPluginImportService
{
    private readonly ILogger Logger = Log.ForContext<PluginImportService>();

    private readonly IDatabaseSchemaInitializer DatabaseSchemaInitializer;
    private readonly IDatabase Database;
    private readonly IPluginService PluginService;
    private readonly IPluginRepository PluginRepository;
    private readonly IPluginMasterReferencesRepository PluginMasterReferencesRepository;

    public PluginImportService(
        IDatabaseSchemaInitializer databaseSchemaInitializer, 
        IDatabase database, 
        IPluginService pluginService,
        IPluginRepository pluginRepository,
        IPluginMasterReferencesRepository pluginMasterReferencesRepository
    )
    {
        DatabaseSchemaInitializer = databaseSchemaInitializer;
        Database = database;
        PluginService = pluginService;
        PluginRepository = pluginRepository;
        PluginMasterReferencesRepository = pluginMasterReferencesRepository;
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
            
            ImportPlugin(entry, result, progress, loadOrderEntries.Count, cancellationToken);
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

        Logger.Information(
            "Plugin import completed: discovered {PluginsDiscovered}, unchanged {PluginsUnchanged}, changed {PluginsChanged}, invalidated {PluginsInvalidated}, imported {PluginsImported}, missing {PluginsMissing}, failed {PluginsFailed}, unsupported {PluginsUnsupported}, master references {MasterReferencesImported}, record headers {RecordHeadersImported}, typed detail rows {TypedRecordDetailRowsImported}, FormList items {FormListItemsImported}, record failures {RecordImportFailures}, unsupported record types {UnsupportedRecordTypes}",
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
            result.UnsupportedRecordTypes
        );
        
        return result;
    }

    private void ImportPlugin(PluginLoadOrderEntryDTO entry, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        ImportStarfieldPlugin(entry, result, progress, totalPlugins, cancellationToken);
        
        // Before we can process all the master references, we need the plugin stubs filled out
        ImportStarfieldPluginMasterReferences(entry, result, progress, totalPlugins, cancellationToken);
        
            
        // Finally, the long arduous part importing all the records
        ImportStarfieldPluginRecords(entry, result, progress, totalPlugins, cancellationToken);
    }

    #region Starfield Import Helpers

    private void ImportStarfieldPlugin(PluginLoadOrderEntryDTO entry, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        var existingPlugin = PluginRepository.GetByModKey(entry.ModKey);
        var fileInfo = new FileInfo(entry.PluginPath);
        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(entry.PluginPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath)
            .Construct();
        
        if (IsUnsupportedPlugin(entry))
        {
            result.PluginsUnsupported++;
            Logger.Information("Skipping unsupported Starfield plugin {ModKey} from {PluginPath}", entry.ModKey, entry.PluginPath);

            var unsupportedPluginDTO = new PluginDTO
            {
                ModKey = entry.ModKey,
                LoadOrderIndex = entry.LoadOrderIndex,
                Enabled = entry.Enabled,
                ExistsOnDisk = fileInfo.Exists,
                ImportState = nameof(PluginImportState.Unsupported),
                HeaderFlags = mod.ModHeader.Flags,
                FormVersion = mod.ModHeader.FormVersion,
                Author = mod.ModHeader.Author ?? "Unknown",
                LastCheckedUtc = DateTime.UtcNow
            };
            
            PluginRepository.Save(unsupportedPluginDTO);
            return;
        }
        
        if (!fileInfo.Exists)
        {
            result.PluginsMissing++;

            var missingPluginDTO = new PluginDTO
            {
                ModKey = entry.ModKey,
                LoadOrderIndex = entry.LoadOrderIndex,
                Enabled = entry.Enabled,
                ExistsOnDisk = fileInfo.Exists,
                ImportState = nameof(PluginImportState.Unsupported),
                HeaderFlags = mod.ModHeader.Flags,
                FormVersion = mod.ModHeader.FormVersion,
                Author = mod.ModHeader.Author ?? "Unknown",
                LastCheckedUtc = DateTime.UtcNow
            };
            
            PluginRepository.Save(missingPluginDTO);
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
            
            PluginRepository.Save(existingPlugin);
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
                    LoadOrderIndex = entry.LoadOrderIndex,
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
            
            PluginRepository.Save(dto);
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
                    LoadOrderIndex = entry.LoadOrderIndex,
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

            PluginRepository.Save(erroredPluginDTO);
        }
    }

    private void ImportStarfieldPluginMasterReferences(PluginLoadOrderEntryDTO entry, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(entry.PluginPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath)
            .Construct();

        if (!mod.MasterReferences.Any()) return;
        
        Logger.Information("Importing master references for {Name} from {FileName}, found {Count} parent masters", entry.ModKey.Name, mod.ModKey.FileName, mod.MasterReferences.Count);
        progress?.Report(new PluginImportProgressDTO
        {
            CurrentPluginName = entry.PluginFileName,
            CurrentModKey = entry.ModKey,
            PluginIndex = entry.LoadOrderIndex,
            PluginCount = totalPlugins,
            StatusText = $"Importing {mod.MasterReferences.Count} master references for {entry.PluginFileName} ({entry.LoadOrderIndex} of {totalPlugins})...",
            IsIndeterminate = false
        });

        foreach (var master in mod.MasterReferences)
        {
            
            var currentMaster = PluginRepository.GetByModKey(master.Master);
            if (currentMaster is null) continue;

            progress?.Report(new PluginImportProgressDTO
            {
                CurrentPluginName = entry.PluginFileName,
                CurrentModKey = entry.ModKey,
                PluginIndex = entry.LoadOrderIndex,
                PluginCount = totalPlugins,
                StatusText = $"Importing {currentMaster.ModKey} at load order {currentMaster.LoadOrderIndex} which is a child of {entry.PluginFileName} at load order {entry.LoadOrderIndex}...",
                IsIndeterminate = false
            });
            
            Logger.Debug("Found master reference {MasterName} for {PluginName}", currentMaster.ModKey.Name, entry.ModKey.Name);
            var masterReferenceDTO = new PluginMasterReferenceDTO
            {
                ModKey = currentMaster.ModKey,
                MasterReferenceIndex = currentMaster.LoadOrderIndex,
                ParentModKey = entry.ModKey,
                ParentLoadOrderIndex = entry.LoadOrderIndex,
                ImportedAtUtc = DateTime.UtcNow
            };
            
            PluginMasterReferencesRepository.Save(masterReferenceDTO);
            result.MasterReferencesImported++;
        }
        Logger.Information("Finished importing master references for {Name} from {FileName}, found {Count} parent masters", entry.ModKey.Name, mod.ModKey.FileName, mod.MasterReferences.Count);
        
        progress?.Report(new PluginImportProgressDTO
        {
            CurrentPluginName = entry.PluginFileName,
            CurrentModKey = entry.ModKey,
            PluginIndex = entry.LoadOrderIndex,
            PluginCount = totalPlugins,
            StatusText = $"Finished importing master references for {entry.PluginFileName}, found {mod.MasterReferences.Count} parent masters...",
            IsIndeterminate = false
        });
    }

    private void ImportStarfieldPluginRecords(PluginLoadOrderEntryDTO entry, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    #endregion

    private static bool IsUnsupportedPlugin(PluginLoadOrderEntryDTO loadOrderEntry)
    {
        var pluginFileName = string.IsNullOrWhiteSpace(loadOrderEntry.PluginFileName) ? loadOrderEntry.ModKey.FileName.ToString() : loadOrderEntry.PluginFileName;
        return pluginFileName.StartsWith("BlueprintShips", StringComparison.OrdinalIgnoreCase) && pluginFileName.EndsWith(".esm", StringComparison.OrdinalIgnoreCase);
    }
}
