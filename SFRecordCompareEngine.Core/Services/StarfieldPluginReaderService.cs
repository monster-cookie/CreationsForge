using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class StarfieldPluginReaderService : IStarfieldPluginReaderService
{
    public IList<PluginLoadOrderEntryDTO> GetLoadOrder()
    {
        var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
        return environment.LoadOrder.ListedOrder
            .Select((plugin, index) => new PluginLoadOrderEntryDTO
            {
                ModKey = plugin.ModKey,
                PluginFileName = plugin.FileName,
                PluginPath = Path.Join(environment.DataFolderPath, plugin.FileName),
                LoadOrderIndex = index,
                Enabled = plugin.Enabled
            })
            .ToList();
    }

    public PluginSourceInfoDTO GetSourceInfo(string pluginPath)
    {
        var fileInfo = new FileInfo(pluginPath);
        return new PluginSourceInfoDTO
        {
            Exists = fileInfo.Exists,
            LastWriteUTCTicks = fileInfo.Exists ? fileInfo.LastWriteTimeUtc.Ticks : 0,
            FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0
        };
    }

    public StarfieldPluginMetadataDTO GetMetadata(string pluginPath)
    {
        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(pluginPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath)
            .Construct();

        return new StarfieldPluginMetadataDTO
        {
            ModKey = mod.ModKey,
            HeaderFlags = mod.ModHeader.Flags,
            FormVersion = mod.ModHeader.FormVersion,
            Author = mod.ModHeader.Author ?? "Unknown",
            InteriorCellCount = mod.ModHeader.InteriorCellCount,
            MasterReferences = mod.MasterReferences.Select(master => master.Master).ToList()
        };
    }
}
