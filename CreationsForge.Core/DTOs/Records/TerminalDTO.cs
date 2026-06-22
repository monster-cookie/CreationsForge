using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class TerminalDTO : RecordDTO, IHasModelsRecordDTO, IKeywords, IHasScriptingAdaptersRecordDTO, IHasRawRecordPayloadsRecordDTO, IHasTerminalMarkerParametersRecordDTO
{
    public string? ObjectBoundsFirst { get; set; }

    public string? ObjectBoundsSecond { get; set; }

    public FormKeyDTO? MenuFormKey { get; set; }

    public string? Background { get; set; }

    public TranslatedStringDTO? Name { get; set; }

    public string? Pnam { get; set; }

    public string? Fnam { get; set; }

    public string? Jnam { get; set; }

    public long? MarkerFlags { get; set; }

    public string? Gnam { get; set; }

    public string? WorkbenchData { get; set; }

    public FormKeyDTO? FurnitureTemplateFormKey { get; set; }

    public string? MarkerModel { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();

    public IList<TerminalMarkerParameterDTO> MarkerParameters { get; set; } = new List<TerminalMarkerParameterDTO>();
}
