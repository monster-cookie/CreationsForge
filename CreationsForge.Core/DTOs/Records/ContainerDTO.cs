using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class ContainerDTO : RecordDTO, IHasModelsDTO, IKeywords, ISounds, IHasScriptingAdaptersDTO, IHasRawRecordPayloadsDTO
{
    public string? ObjectBoundsFirst { get; set; }

    public string? ObjectBoundsSecond { get; set; }

    public TranslatedStringDTO? Name { get; set; }

    public string? Flags { get; set; }

    public string? MajorFlags { get; set; }

    public FormKeyDTO? NativeTerminalFormKey { get; set; }

    public string? AnimationGraph { get; set; }

    public string? AnimationSkeleton { get; set; }

    public string? AnimationDirectory { get; set; }

    public string? AnimationFile { get; set; }

    public IList<ContainerItemDTO> Items { get; set; } = new List<ContainerItemDTO>();

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<SoundMappingDTO> Sounds { get; set; } = new List<SoundMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();
}
