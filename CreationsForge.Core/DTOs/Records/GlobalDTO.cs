using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class GlobalDTO : RecordDTO, IHasScriptingAdaptersDTO
{
    public string? MutagenObjectType { get; set; }

    public string? MajorFlags { get; set; }

    public double? Data { get; set; }

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
