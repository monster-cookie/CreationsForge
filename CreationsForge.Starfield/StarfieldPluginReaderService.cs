using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Utilities;
using CreationsForge.Starfield.DTOs;
using CreationsForge.Starfield.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield;

public class StarfieldPluginReaderService : IStarfieldPluginReaderService
{
    private readonly StarfieldGameMetadataService GameMetadataService;

    public StarfieldPluginReaderService(StarfieldGameMetadataService gameMetadataService)
    {
        GameMetadataService = gameMetadataService;
    }

    public GameDTO ReadGame()
    {
        _ = typeof(StarfieldMod);
        return GameMetadataService.GetGame();
    }

    public IReadOnlyList<PluginLoadOrderEntryDTO> ReadLoadOrder()
    {
        var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
        return environment.LoadOrder.ListedOrder
            .Select((plugin, index) => new PluginLoadOrderEntryDTO
            {
                Game = SupportedGame.Starfield,
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
        return loadOrderEntry.ModKey.FileName.StartsWith("BlueprintShips", StringComparison.OrdinalIgnoreCase) && loadOrderEntry.ModKey.FileName.EndsWith(".esm", StringComparison.OrdinalIgnoreCase);
    }

    public PluginDTO ReadPluginMetadata(PluginLoadOrderEntryDTO loadOrderEntry, PluginSourceInfoDTO sourceInfo)
    {
        var mod = StarfieldModConstruction.Load(loadOrderEntry.ModKey);

        return new StarfieldPluginDTO
        {
            Game = SupportedGame.Starfield,
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
            Branch = mod.ModHeader.Branch ?? string.Empty,
            InteriorCellCount = mod.ModHeader.InteriorCellCount,
            Intv = mod.ModHeader.INTV
        };
    }

    public IReadOnlyList<PluginMasterReferenceDTO> ReadMasterReferences(PluginDTO plugin)
    {
        var mod = StarfieldModConstruction.Load(plugin.ModKey);

        var importedAtUTC = DateTime.UtcNow;
        return mod.MasterReferences
            .Select(masterReference => new PluginMasterReferenceDTO
            {
                Game = SupportedGame.Starfield,
                MasterModKey = ModKeyDTOMapper.FromModKey(masterReference.Master),
                PluginModKey = plugin.ModKey,
                ImportedAtUTC = importedAtUTC
            })
            .ToList();
    }

    protected virtual string GetDataFolderPath()
    {
        return StarfieldModConstruction.GetDataFolderPath();
    }
}
