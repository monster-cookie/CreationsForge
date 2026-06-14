using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class BookDTO : RecordDTO, IHasModelsRecordDTO, IHasKeywordsRecordDTO, IHasSoundsRecordDTO, IHasScriptingAdaptersRecordDTO, IHasRawRecordPayloadsRecordDTO
{
    public int? Version2 { get; set; }

    public string? ObjectBoundsFirst { get; set; }

    public string? ObjectBoundsSecond { get; set; }

    public FormKeyDTO? InventoryTransformFormKey { get; set; }

    public int? Xalg { get; set; }

    public string? Name { get; set; }

    public string? Text { get; set; }

    public int? Value { get; set; }

    public float? Weight { get; set; }

    public string? Flags { get; set; }

    public string? TeachesType { get; set; }

    public string? TeachesRawContent { get; set; }

    public string? DataSlateType { get; set; }

    public string? Description { get; set; }

    public string? DataSlateHeaderLeft { get; set; }

    public string? DataSlateHeaderRight { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<RecordKeywordDTO> Keywords { get; set; } = new List<RecordKeywordDTO>();

    public IList<RecordSoundDTO> Sounds { get; set; } = new List<RecordSoundDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();
}
