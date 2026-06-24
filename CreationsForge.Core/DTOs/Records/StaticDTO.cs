using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class StaticDTO : RecordDTO, IHasModelsDTO, IKeywords, IHasRawRecordPayloadsDTO
{
    public TranslatedStringDTO? Name { get; set; }

    public string? ObjectBoundsFirst { get; set; }

    public string? ObjectBoundsSecond { get; set; }

    public double? MaxAngle { get; set; }

    public double? UnknownDNAMFloat { get; set; }

    public double? LeafAmplitude { get; set; }

    public double? LeafFrequency { get; set; }

    public string? Unused { get; set; }

    public string? DNAMDataTypeState { get; set; }

    public double? DirtinessScale { get; set; }

    public FormKeyDTO? SnapTemplate { get; set; }

    public FormKeyDTO? PreviewTransform { get; set; }

    public FormKeyDTO? Material { get; set; }

    public string? LodLevel0 { get; set; }

    public string? LodLevel1 { get; set; }

    public string? LodLevel2 { get; set; }

    public string? LodLevel3 { get; set; }

    public StaticNavmeshGeometryDTO? NavmeshGeometry { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<StaticPropertyDTO> Properties { get; set; } = new List<StaticPropertyDTO>();

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();
}
