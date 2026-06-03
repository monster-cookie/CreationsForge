using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class ScriptingAdapterDTO
{
    public required ModKey ModKey { get; set; }
    public required string RecordType { get; set; }
    public required FormKey FormKey { get; set; }
    public required string Name { get; set; }
    public required int ScriptIndex { get; set; }
    public required DateTime ImportedAtUTC { get; set; }
    public IList<ScriptingAdapterPropertyDTO> Properties { get; set; } = new List<ScriptingAdapterPropertyDTO>();
}
