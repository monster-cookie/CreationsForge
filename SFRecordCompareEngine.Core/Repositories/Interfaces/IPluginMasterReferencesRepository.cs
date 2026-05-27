using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IPluginMasterReferencesRepository
{
    /// <summary>
    /// Get all master references for a given mod key.
    /// </summary>
    /// <param name="modKey">The mod key to retrieve master references for</param>
    /// <returns>The list of master references for the given mod key or an empty list if none are found</returns>
    IList<PluginMasterReferenceDTO> GetMasterReferences(ModKey modKey);
    
    /// <summary>
    /// Save a plugin master reference.
    /// </summary>
    /// <param name="dto">The DTO of master reference information to save</param>
    void Save(PluginMasterReferenceDTO dto);
}