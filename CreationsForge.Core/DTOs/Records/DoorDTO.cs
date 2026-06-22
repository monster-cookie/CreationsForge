using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class DoorDTO : RecordDTO, IHasModelsRecordDTO, IKeywords, ISounds, IHasScriptingAdaptersRecordDTO, IHasComponentsRecordDTO, IHasRawRecordPayloadsRecordDTO
{
    public string? ObjectBoundsFirst { get; set; }

    public string? ObjectBoundsSecond { get; set; }

    public TranslatedStringDTO? Name { get; set; }

    public string? Flags { get; set; }

    public FormKeyDTO? NativeTerminalFormKey { get; set; }

    public string? SoundLevel { get; set; }

    public string? FacingAxisOverride { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<SoundMappingDTO> Sounds { get; set; } = new List<SoundMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<RecordComponentDTO> Components { get; set; } = new List<RecordComponentDTO>();

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();
}
