using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IStarfieldPluginReaderService
{
    IList<PluginLoadOrderEntryDTO> GetLoadOrder();
    PluginSourceInfoDTO GetSourceInfo(string pluginPath);
    StarfieldPluginMetadataDTO GetMetadata(string pluginPath);
}
