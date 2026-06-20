using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class StaticDTO : RecordDTO, IHasModelsRecordDTO, IHasKeywordsRecordDTO, IHasRawRecordPayloadsRecordDTO
{
    public string? ObjectBoundsFirst { get; set; }

    public string? ObjectBoundsSecond { get; set; }

    public double? MaxAngle { get; set; }

    public double? UnknownDNAMFloat { get; set; }

    public double? LeafAmplitude { get; set; }

    public double? LeafFrequency { get; set; }

    public string? Unused { get; set; }

    public string? DNAMDataTypeState { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<RecordKeywordDTO> Keywords { get; set; } = new List<RecordKeywordDTO>();

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();
}
