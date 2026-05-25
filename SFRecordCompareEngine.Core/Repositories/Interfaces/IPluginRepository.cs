using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IPluginRepository
{
    PluginMetadataDTO? GetByModKey(IDatabase database, ModKey modKey);
    IList<PluginMetadataDTO> GetAll(IDatabase database);
    IList<PluginMetadataDTO> GetPlugins(IDatabase database);
    IList<PluginMetadataDTO> GetOpenablePlugins(IDatabase database);
    IList<PluginMetadataDTO> SearchPlugins(IDatabase database, string searchText);
    IList<PluginMetadataDTO> SearchOpenablePlugins(IDatabase database, string searchText);
    IList<PluginMasterReferenceDTO> GetMasterReferences(IDatabase database, string modKey);
    IList<PluginResolutionHierarchyDTO> GetResolutionHierarchy(IDatabase database, string modKey);
    void UpsertPlugin(IDatabase database, PluginMetadataDTO plugin);
    void UpsertMissingPlaceholder(IDatabase database, string modKey, string checkedAtUtc);
    void ReplaceMasterReferences(IDatabase database, ModKey modKey, IList<PluginMasterReferenceDTO> masterReferences);
    void RefreshParentLoadOrderIndexes(IDatabase database);
    void MarkPluginsNotInLoadOrder(IDatabase database, HashSet<ModKey> currentModKeys, string checkedAtUtc);
}
