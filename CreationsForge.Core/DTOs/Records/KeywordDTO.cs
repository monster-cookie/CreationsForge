using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class KeywordDTO : RecordDTO, IHasScriptingAdaptersRecordDTO
{
    public string? Name { get; set; }

    public required string Color { get; set; }

    public required string Type { get; set; }

    public string? Notes { get; set; }

    public string? FlashLinkageName { get; set; }

    public FormKeyDTO? AttractionRuleFormKey { get; set; }

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
