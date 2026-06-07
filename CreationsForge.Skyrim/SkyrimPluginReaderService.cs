using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Utilities;
using CreationsForge.Skyrim.DTOs;
using CreationsForge.Skyrim.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim;

public class SkyrimPluginReaderService : ISkyrimPluginReaderService
{
    private readonly SkyrimGameMetadataService GameMetadataService;

    public SkyrimPluginReaderService(SkyrimGameMetadataService gameMetadataService)
    {
        GameMetadataService = gameMetadataService;
    }

    public GameDTO ReadGame()
    {
        _ = typeof(SkyrimMod);
        return GameMetadataService.GetGame();
    }

    public IReadOnlyList<PluginLoadOrderEntryDTO> ReadLoadOrder()
    {
        var environment = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);
        return environment.LoadOrder.ListedOrder
            .Select((plugin, index) => new PluginLoadOrderEntryDTO
            {
                Game = SupportedGame.Skyrim,
                ModKey = ModKeyDTOMapper.FromModKey(plugin.ModKey),
                LoadOrderIndex = index,
                Enabled = plugin.Enabled
            })
            .ToList();
    }

    public PluginSourceInfoDTO ReadSourceInfo(ModKeyDTO modKey)
    {
        var fileInfo = new FileInfo(Path.Combine(GetDataFolderPath(), modKey.FileName));
        return new PluginSourceInfoDTO
        {
            Exists = fileInfo.Exists,
            LastWriteUTCTicks = fileInfo.Exists ? fileInfo.LastWriteTimeUtc.Ticks : 0,
            FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0
        };
    }

    public bool IsUnsupported(PluginLoadOrderEntryDTO loadOrderEntry)
    {
        return false;
    }

    public PluginDTO ReadPluginMetadata(PluginLoadOrderEntryDTO loadOrderEntry, PluginSourceInfoDTO sourceInfo)
    {
        var dataFolderPath = GetDataFolderPath();
        var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(Path.Combine(dataFolderPath, loadOrderEntry.ModKey.FileName))
            .WithDataFolder(dataFolderPath)
            .Construct();

        return new SkyrimPluginDTO
        {
            Game = SupportedGame.Skyrim,
            ModKey = ModKeyDTOMapper.FromModKey(mod.ModKey),
            LoadOrderIndex = loadOrderEntry.LoadOrderIndex,
            Enabled = loadOrderEntry.Enabled,
            ExistsOnDisk = sourceInfo.Exists,
            ImportState = sourceInfo.Exists ? PluginImportState.Current : PluginImportState.Missing,
            HeaderFlags = (int)mod.ModHeader.Flags,
            FormVersion = mod.ModHeader.FormVersion,
            Author = mod.ModHeader.Author,
            Description = mod.ModHeader.Description,
            RecordCount = (int)mod.ModHeader.Stats.NumRecords,
            SourceLastWriteUTCTicks = sourceInfo.LastWriteUTCTicks,
            SourceFileSizeBytes = sourceInfo.FileSizeBytes,
            LastCheckedUTC = DateTime.UtcNow,
            LastImportedUTC = DateTime.UtcNow,
            Incc = mod.ModHeader.INCC,
            Intv = mod.ModHeader.INTV
        };
    }

    public IReadOnlyList<PluginMasterReferenceDTO> ReadMasterReferences(PluginDTO plugin)
    {
        var dataFolderPath = GetDataFolderPath();
        var mod = SkyrimMod.Create(SkyrimRelease.SkyrimSE)
            .FromPath(Path.Combine(dataFolderPath, plugin.ModKey.FileName))
            .WithDataFolder(dataFolderPath)
            .Construct();

        var importedAtUTC = DateTime.UtcNow;
        return mod.MasterReferences
            .Select(masterReference => new PluginMasterReferenceDTO
            {
                Game = SupportedGame.Skyrim,
                MasterModKey = ModKeyDTOMapper.FromModKey(masterReference.Master),
                PluginModKey = plugin.ModKey,
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    protected virtual string GetDataFolderPath()
    {
        var environment = GameEnvironment.Typical.Skyrim(SkyrimRelease.SkyrimSE);
        return environment.DataFolderPath;
    }
}
