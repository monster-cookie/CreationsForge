using NPoco;
using Serilog;
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
    private readonly IRecordImportService RecordImportService;
    private readonly IStarfieldPluginReaderService StarfieldPluginReaderService;

    public PluginImportService(
        IDatabaseSchemaInitializer databaseSchemaInitializer, 
        IDatabase database, 
        IPluginService pluginService,
        IPluginRepository pluginRepository,
        IPluginMasterReferencesRepository pluginMasterReferencesRepository,
        IRecordImportService recordImportService,
        IStarfieldPluginReaderService starfieldPluginReaderService
    )
    {
        DatabaseSchemaInitializer = databaseSchemaInitializer;
        Database = database;
        PluginService = pluginService;
        PluginRepository = pluginRepository;
        PluginMasterReferencesRepository = pluginMasterReferencesRepository;
        RecordImportService = recordImportService;
        StarfieldPluginReaderService = starfieldPluginReaderService;
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
        var plugin = ImportStarfieldPlugin(entry, result, progress, totalPlugins, cancellationToken);
        if (plugin == null) return;
        
        // Before we can process all the master references, we need the plugin stubs filled out
        ImportStarfieldPluginMasterReferences(entry, result, progress, totalPlugins, cancellationToken);
        
        // Finally, the long arduous part importing all the records
        ImportStarfieldPluginRecords(plugin, result, progress, totalPlugins, cancellationToken);
    }

    #region Starfield Import Helpers

    private PluginDTO? ImportStarfieldPlugin(PluginLoadOrderEntryDTO entry, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        var existingPlugin = PluginRepository.GetByModKey(entry.ModKey);
        var sourceInfo = StarfieldPluginReaderService.GetSourceInfo(entry.PluginPath);
        
        if (IsUnsupportedPlugin(entry))
        {
            result.PluginsUnsupported++;
            Logger.Information("Skipping unsupported Starfield plugin {ModKey} from {PluginPath}", entry.ModKey, entry.PluginPath);

            var unsupportedPluginDTO = new PluginDTO
            {
                ModKey = entry.ModKey,
                LoadOrderIndex = entry.LoadOrderIndex,
                Enabled = entry.Enabled,
                ExistsOnDisk = sourceInfo.Exists,
                ImportState = nameof(PluginImportState.Unsupported),
                LastCheckedUTC = DateTime.UtcNow,
                SourceLastWriteUTCTicks = sourceInfo.LastWriteUTCTicks,
                SourceFileSizeBytes = sourceInfo.FileSizeBytes
            };
            
            PluginRepository.Save(unsupportedPluginDTO);
            return null;
        }
        
        if (!sourceInfo.Exists)
        {
            result.PluginsMissing++;

            var missingPluginDTO = new PluginDTO
            {
                ModKey = entry.ModKey,
                LoadOrderIndex = entry.LoadOrderIndex,
                Enabled = entry.Enabled,
                ExistsOnDisk = sourceInfo.Exists,
                ImportState = nameof(PluginImportState.Missing),
                LastCheckedUTC = DateTime.UtcNow,
                SourceLastWriteUTCTicks = sourceInfo.LastWriteUTCTicks,
                SourceFileSizeBytes = sourceInfo.FileSizeBytes
            };
            
            PluginRepository.Save(missingPluginDTO);
            return null;
        }

        var sourceLastWriteUTCTicks = sourceInfo.LastWriteUTCTicks;
        var sourceFileSizeBytes = sourceInfo.FileSizeBytes;
        var isUnchanged = existingPlugin is not null && existingPlugin.SourceLastWriteUTCTicks == sourceLastWriteUTCTicks && existingPlugin.SourceFileSizeBytes == sourceFileSizeBytes;
        if (isUnchanged)
        {
            result.PluginsUnchanged++;
            Logger.Information("Skipping unchanged plugin {ModKey}: source last write ticks {SourceLastWriteUtcTicks}, source file size {SourceFileSizeBytes}, import state {ImportState}", entry.ModKey, sourceLastWriteUTCTicks, sourceFileSizeBytes, existingPlugin!.ImportState);
            
            existingPlugin.LastCheckedUTC = DateTime.UtcNow;
            
            PluginRepository.Save(existingPlugin);
            return null;
        }
        
        if (existingPlugin is not null)
        {
            result.PluginsChanged++;
            result.PluginsInvalidated++;
            Logger.Information("Plugin {ModKey} changed: stored last write ticks {StoredLastWriteUtcTicks}, current last write ticks {CurrentLastWriteUtcTicks}, stored file size {StoredFileSizeBytes}, current file size {CurrentFileSizeBytes}, stored import state {ImportState}",
                entry.ModKey,
                existingPlugin.SourceLastWriteUTCTicks,
                sourceLastWriteUTCTicks,
                existingPlugin.SourceFileSizeBytes,
                sourceFileSizeBytes,
                existingPlugin.ImportState);
        }

        try
        {
            var metadata = StarfieldPluginReaderService.GetMetadata(entry.PluginPath);
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
                dto.ExistsOnDisk = sourceInfo.Exists;
                dto.ImportState = nameof(PluginImportState.Current);
                dto.HeaderFlags = metadata.HeaderFlags;
                dto.FormVersion = metadata.FormVersion;
                dto.Author = metadata.Author;
                dto.InteriorCellCount = metadata.InteriorCellCount;
                dto.LastCheckedUTC = DateTime.UtcNow;
                dto.LastImportedUTC = DateTime.UtcNow;
                dto.InvalidatedAtUTC = null;
                dto.SourceLastWriteUTCTicks = sourceLastWriteUTCTicks;
                dto.SourceFileSizeBytes = sourceFileSizeBytes;
            }
            else
            {
                dto = new PluginDTO
                {
                    ModKey = entry.ModKey,
                    LoadOrderIndex = entry.LoadOrderIndex,
                    Enabled = entry.Enabled,
                    ExistsOnDisk = sourceInfo.Exists,
                    ImportState = nameof(PluginImportState.Current),
                    HeaderFlags = metadata.HeaderFlags,
                    FormVersion = metadata.FormVersion,
                    Author = metadata.Author,
                    InteriorCellCount = metadata.InteriorCellCount,
                    LastCheckedUTC = DateTime.UtcNow,
                    LastImportedUTC = DateTime.UtcNow,
                    InvalidatedAtUTC = null,
                    SourceLastWriteUTCTicks = sourceLastWriteUTCTicks,
                    SourceFileSizeBytes = sourceFileSizeBytes
                };
            }
            
            PluginRepository.Save(dto);
            result.PluginsImported++;
            return dto;
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
                erroredPluginDTO.LastCheckedUTC = DateTime.UtcNow;
                erroredPluginDTO.LastImportedUTC = existingPlugin?.LastImportedUTC;
                erroredPluginDTO.InvalidatedAtUTC = DateTime.UtcNow;
            }
            else
            {
                erroredPluginDTO = new PluginDTO
                {
                    ModKey = entry.ModKey,
                    LoadOrderIndex = entry.LoadOrderIndex,
                    Enabled = entry.Enabled,
                    ExistsOnDisk = sourceInfo.Exists,
                    ImportState = nameof(PluginImportState.Failed),
                    LastCheckedUTC = DateTime.UtcNow,
                    LastImportedUTC = existingPlugin?.LastImportedUTC,
                    InvalidatedAtUTC = DateTime.UtcNow,
                    SourceLastWriteUTCTicks = sourceInfo.LastWriteUTCTicks,
                    SourceFileSizeBytes = sourceInfo.FileSizeBytes
                };
            }

            PluginRepository.Save(erroredPluginDTO);
            return null;
        }
    }

    private void ImportStarfieldPluginMasterReferences(PluginLoadOrderEntryDTO entry, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        var metadata = StarfieldPluginReaderService.GetMetadata(entry.PluginPath);

        if (!metadata.MasterReferences.Any()) return;
        
        Logger.Information("Importing master references for {Name} from {FileName}, found {Count} parent masters", entry.ModKey.Name, metadata.ModKey.FileName, metadata.MasterReferences.Count);
        progress?.Report(new PluginImportProgressDTO
        {
            CurrentPluginName = entry.PluginFileName,
            CurrentModKey = entry.ModKey,
            PluginIndex = entry.LoadOrderIndex,
            PluginCount = totalPlugins,
            StatusText = $"Importing {metadata.MasterReferences.Count} master references for {entry.PluginFileName} ({entry.LoadOrderIndex} of {totalPlugins})...",
            IsIndeterminate = false
        });

        foreach (var master in metadata.MasterReferences)
        {
            
            var currentMaster = PluginRepository.GetByModKey(master);
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
                ImportedAtUTC = DateTime.UtcNow
            };
            
            PluginMasterReferencesRepository.Save(masterReferenceDTO);
            result.MasterReferencesImported++;
        }
        Logger.Information("Finished importing master references for {Name} from {FileName}, found {Count} parent masters", entry.ModKey.Name, metadata.ModKey.FileName, metadata.MasterReferences.Count);
        
        progress?.Report(new PluginImportProgressDTO
        {
            CurrentPluginName = entry.PluginFileName,
            CurrentModKey = entry.ModKey,
            PluginIndex = entry.LoadOrderIndex,
            PluginCount = totalPlugins,
            StatusText = $"Finished importing master references for {entry.PluginFileName}, found {metadata.MasterReferences.Count} parent masters...",
            IsIndeterminate = false
        });
    }

    private void ImportStarfieldPluginRecords(PluginDTO plugin, PluginImportResultDTO result, IProgress<PluginImportProgressDTO>? progress, int totalPlugins, CancellationToken cancellationToken)
    {
        var recordImportResult = RecordImportService.ImportPluginRecords(plugin, cancellationToken);
        result.RecordHeadersImported += recordImportResult.HeadersImported;
        result.TypedRecordDetailRowsImported += recordImportResult.DetailRowsImported;
        result.FormListItemsImported += recordImportResult.FormListItemsImported;
        result.RecordImportFailures += recordImportResult.RecordsFailed;
        result.UnsupportedRecordTypes += recordImportResult.UnsupportedRecordTypes;
    }

    #endregion

    private static bool IsUnsupportedPlugin(PluginLoadOrderEntryDTO loadOrderEntry)
    {
        var pluginFileName = string.IsNullOrWhiteSpace(loadOrderEntry.PluginFileName) ? loadOrderEntry.ModKey.FileName.ToString() : loadOrderEntry.PluginFileName;
        return pluginFileName.StartsWith("BlueprintShips", StringComparison.OrdinalIgnoreCase) && pluginFileName.EndsWith(".esm", StringComparison.OrdinalIgnoreCase);
    }
}
