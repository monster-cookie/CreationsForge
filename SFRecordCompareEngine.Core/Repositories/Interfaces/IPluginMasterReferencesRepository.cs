using System.Collections.Generic;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IPluginMasterReferencesRepository
{
    IList<PluginMasterReferenceDTO> GetMasterReferences(ModKey modKey);
}