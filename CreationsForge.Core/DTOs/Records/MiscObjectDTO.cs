using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class MiscObjectDTO : RecordDTO, IHasScriptingAdaptersRecordDTO, IHasModelsRecordDTO, IHasKeywordsRecordDTO, IHasSoundsRecordDTO
{
    public string? Name { get; set; }

    public string? ShortName { get; set; }

    public int? Value { get; set; }

    public float? Weight { get; set; }

    public float? DirtinessScale { get; set; }

    public FormKeyDTO? FeaturedItemMessageFormKey { get; set; }

    public string? Flag { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<RecordKeywordDTO> Keywords { get; set; } = new List<RecordKeywordDTO>();

    public IList<RecordSoundDTO> Sounds { get; set; } = new List<RecordSoundDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
