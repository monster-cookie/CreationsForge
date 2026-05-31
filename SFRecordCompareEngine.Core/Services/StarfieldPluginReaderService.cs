using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class StarfieldPluginReaderService : IStarfieldPluginReaderService
{
    /// <inheritdoc />
    public virtual string GetDataFolderPath()
    {
        var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
        return environment.DataFolderPath;
    }

    /// <inheritdoc />
    public IList<PluginLoadOrderEntryDTO> GetLoadOrder()
    {
        var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
        return environment.LoadOrder.ListedOrder
            .Select((plugin, index) => new PluginLoadOrderEntryDTO
            {
                ModKey = plugin.ModKey,
                LoadOrderIndex = index,
                Enabled = plugin.Enabled
            })
            .ToList();
    }

    /// <inheritdoc />
    public PluginSourceInfoDTO GetSourceInfo(ModKey modKey)
    {
        var fileInfo = new FileInfo(Path.Combine(GetDataFolderPath(), modKey.FileName));
        return new PluginSourceInfoDTO
        {
            Exists = fileInfo.Exists,
            LastWriteUTCTicks = fileInfo.Exists ? fileInfo.LastWriteTimeUtc.Ticks : 0,
            FileSizeBytes = fileInfo.Exists ? fileInfo.Length : 0
        };
    }

    /// <inheritdoc />
    public StarfieldPluginMetadataDTO GetMetadata(ModKey modKey)
    {
        var mod = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(Path.Combine(GetDataFolderPath(), modKey.FileName))
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(GetDataFolderPath())
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