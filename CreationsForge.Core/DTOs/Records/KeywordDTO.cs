using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class KeywordDTO : RecordDTO, IHasScriptingAdaptersDTO
{
    public TranslatedStringDTO? Name { get; set; }

    public string? Color { get; set; }

    public string? Type { get; set; }

    public string? Notes { get; set; }

    public string? FlashLinkageName { get; set; }

    public string? FNAM { get; set; }

    public string? WAIM { get; set; }

    public string? WFIR { get; set; }

    public FormKeyDTO? AttractionRule { get; set; }

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
