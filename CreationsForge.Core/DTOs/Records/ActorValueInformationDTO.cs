using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class ActorValueInformationDTO : RecordDTO, IHasScriptingAdaptersRecordDTO
{
    public TranslatedStringDTO? Name { get; set; }

    public TranslatedStringDTO? Abbreviation { get; set; }

    public string? ContextNotes { get; set; }

    public double? DefaultValue { get; set; }

    public string? Flags { get; set; }

    public string? Type { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
