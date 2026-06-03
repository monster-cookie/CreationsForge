using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IScriptingAdapterPropertyListItemRepository
{
    IList<ScriptingAdapterPropertyListItemDTO> GetByRecord(ModKey modKey, string recordType, FormKey formKey);
    void Save(ScriptingAdapterPropertyListItemDTO dto);
}
