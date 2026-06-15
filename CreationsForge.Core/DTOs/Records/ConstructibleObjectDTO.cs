using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class ConstructibleObjectDTO : RecordDTO, IHasScriptingAdaptersRecordDTO, IHasRawRecordPayloadsRecordDTO
{
    public int? Version2 { get; set; }

    public string? Description { get; set; }

    public FormKeyDTO? CreatedObjectFormKey { get; set; }

    public FormKeyDTO? WorkbenchKeywordFormKey { get; set; }

    public int? CreatedObjectCount { get; set; }

    public int? AmountProduced { get; set; }

    public int? MenuSortOrder { get; set; }

    public string? LearnMethod { get; set; }

    public string? Flags { get; set; }

    public IList<ConstructibleObjectComponentDTO> Components { get; set; } = new List<ConstructibleObjectComponentDTO>();

    public IList<ConstructibleObjectCategoryDTO> Categories { get; set; } = new List<ConstructibleObjectCategoryDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();
}
