using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records.Interfaces;

public interface IHasScriptingAdaptersRecordDTO
{
    ModKey ModKey { get; }
    FormKey FormKey { get; }
    IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; }
}
