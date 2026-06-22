using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class MiscObjectDTO : RecordDTO, IHasScriptingAdaptersRecordDTO, IHasModelsRecordDTO, IKeywords, ISounds
{
    public TranslatedStringDTO? Name { get; set; }

    public TranslatedStringDTO? ShortName { get; set; }

    public int? Value { get; set; }

    public float? Weight { get; set; }

    public float? DirtinessScale { get; set; }

    public FormKeyDTO? FeaturedItemMessageFormKey { get; set; }

    public string? Flag { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<SoundMappingDTO> Sounds { get; set; } = new List<SoundMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
