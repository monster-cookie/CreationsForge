using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.DTOs.Records.Metadata;

namespace CreationsForge.Core.DTOs.Records;

public class MiscItemDTO : RecordDTO, IHasScriptingAdaptersDTO, IHasModelsDTO, IKeywords, ISounds, IHasRawRecordPayloadsDTO
{
    public ObjectBoundsDTO? ObjectBounds { get; set; }

    public BookTransformsDTO? Transforms { get; set; }

    [FormKeyColumnPrefix("PreviewTransform")]
    public FormKeyDTO? PreviewTransform { get; set; }

    public TranslatedStringDTO? Name { get; set; }

    public TranslatedStringDTO? ShortName { get; set; }

    public int? Value { get; set; }

    public float? Weight { get; set; }

    public float? DirtinessScale { get; set; }

    public FormKeyDTO? FeaturedItemMessage { get; set; }

    public string? Flag { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<SoundMappingDTO> Sounds { get; set; } = new List<SoundMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<MiscItemComponentDTO> Components { get; set; } = new List<MiscItemComponentDTO>();

    public IList<MiscItemResourceDTO> Resources { get; set; } = new List<MiscItemResourceDTO>();

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();
}
