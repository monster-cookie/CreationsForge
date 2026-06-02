using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IScriptingAdapterRepository
{
    IList<ScriptingAdapterDTO> GetByRecord(ModKey modKey, string recordType, FormKey formKey);
    void DeleteByRecord(ModKey modKey, string recordType, FormKey formKey);
    void Save(ScriptingAdapterDTO dto);
}
