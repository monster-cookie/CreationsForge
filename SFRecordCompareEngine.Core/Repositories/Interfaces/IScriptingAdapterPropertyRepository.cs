using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IScriptingAdapterPropertyRepository
{
    IList<ScriptingAdapterPropertyDTO> GetByRecord(ModKey modKey, string recordType, FormKey formKey);
    void Save(ScriptingAdapterPropertyDTO dto);
}
