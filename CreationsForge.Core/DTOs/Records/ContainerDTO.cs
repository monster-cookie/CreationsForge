using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class ContainerDTO : RecordDTO, IHasModelsRecordDTO, IHasKeywordsRecordDTO, IHasSoundsRecordDTO, IHasRawRecordPayloadsRecordDTO
{
    public int? Version2 { get; set; }

    public string? ObjectBoundsFirst { get; set; }

    public string? ObjectBoundsSecond { get; set; }

    public string? Name { get; set; }

    public string? Flags { get; set; }

    public string? MajorFlags { get; set; }

    public FormKeyDTO? NativeTerminalFormKey { get; set; }

    public IList<ContainerItemDTO> Items { get; set; } = new List<ContainerItemDTO>();

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<RecordKeywordDTO> Keywords { get; set; } = new List<RecordKeywordDTO>();

    public IList<RecordSoundDTO> Sounds { get; set; } = new List<RecordSoundDTO>();

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();
}
