using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Utilities;
using CreationsForge.Fallout4.DTOs;
using CreationsForge.Fallout4.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Fallout4;

namespace CreationsForge.Fallout4;

public class Fallout4PluginReaderService : IFallout4PluginReaderService
{
    private readonly Fallout4GameMetadataService GameMetadataService;

    public Fallout4PluginReaderService(Fallout4GameMetadataService gameMetadataService)
    {
        GameMetadataService = gameMetadataService;
    }

    public GameDTO ReadGame()
    {
        _ = typeof(Fallout4Mod);
        return GameMetadataService.GetGame();
    }

    public IReadOnlyList<PluginLoadOrderEntryDTO> ReadLoadOrder()
    {
        var environment = GameEnvironment.Typical.Fallout4(Fallout4Release.Fallout4);
        return environment.LoadOrder.ListedOrder
            .Select((plugin, index) => new PluginLoadOrderEntryDTO
            {
                Game = SupportedGame.Fallout4,
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
        var mod = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(Path.Combine(dataFolderPath, loadOrderEntry.ModKey.FileName))
            .WithDataFolder(dataFolderPath)
            .Construct();

        return new Fallout4PluginDTO
        {
            Game = SupportedGame.Fallout4,
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
            Incc = mod.ModHeader.INCC
        };
    }

    public IReadOnlyList<PluginMasterReferenceDTO> ReadMasterReferences(PluginDTO plugin)
    {
        var dataFolderPath = GetDataFolderPath();
        var mod = Fallout4Mod.Create(Fallout4Release.Fallout4)
            .FromPath(Path.Combine(dataFolderPath, plugin.ModKey.FileName))
            .WithDataFolder(dataFolderPath)
            .Construct();

        var importedAtUTC = DateTime.UtcNow;
        return mod.MasterReferences
            .Select(masterReference => new PluginMasterReferenceDTO
            {
                Game = SupportedGame.Fallout4,
                MasterModKey = ModKeyDTOMapper.FromModKey(masterReference.Master),
                PluginModKey = plugin.ModKey,
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    protected virtual string GetDataFolderPath()
    {
        var environment = GameEnvironment.Typical.Fallout4(Fallout4Release.Fallout4);
        return environment.DataFolderPath;
    }
}
