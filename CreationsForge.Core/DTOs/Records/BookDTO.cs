using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.DTOs.Records.Metadata;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class BookDTO : RecordDTO, IHasName, IHasText, IHasTranslatedFields, IHasModelsRecordDTO, IHasKeywordsRecordDTO, IHasSoundsRecordDTO, IHasScriptingAdaptersRecordDTO, IHasComponentsRecordDTO, IHasRawRecordPayloadsRecordDTO
{
    public ObjectBoundsDTO? ObjectBounds { get; set; }

    public BookTransformsDTO? Transforms { get; set; }

    [FormKeyColumnPrefix("InventoryArt")]
    public FormKeyDTO? InventoryArt { get; set; }

    [FormKeyColumnPrefix("PreviewTransform")]
    public FormKeyDTO? PreviewTransform { get; set; }

    [FormKeyColumnPrefix("FeaturedItemMessage")]
    public FormKeyDTO? FeaturedItemMessage { get; set; }

    public int? XALG { get; set; }

    [LocalizedField("Name")]
    public TranslatedStringDTO? Name { get; set; }

    [SpriggitPath(SupportedGame.Fallout4, "BookText")]
    [SpriggitPath(SupportedGame.Skyrim, "BookText")]
    [LocalizedField(SupportedGame.Starfield, "Text")]
    [LocalizedField("BookText")]
    public TranslatedStringDTO? Text { get; set; }

    public int? Value { get; set; }

    public float? Weight { get; set; }

    public string? Flags { get; set; }

    public BookTeachesDTO? Teaches { get; set; }

    public string? DataSlateType { get; set; }

    public TranslatedStringDTO? Description { get; set; }

    public TranslatedStringDTO? DataSlateHeaderLeft { get; set; }

    public TranslatedStringDTO? DataSlateHeaderRight { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<RecordKeywordDTO> Keywords { get; set; } = new List<RecordKeywordDTO>();

    public IList<RecordSoundDTO> Sounds { get; set; } = new List<RecordSoundDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<RecordComponentDTO> Components { get; set; } = new List<RecordComponentDTO>();

    public IList<RawRecordPayloadDTO> RawPayloads { get; set; } = new List<RawRecordPayloadDTO>();

    public IEnumerable<TranslatedFieldDTO> GetTranslatedFields()
    {
        yield return new TranslatedFieldDTO { SourceField = "Name", Value = Name };
        yield return new TranslatedFieldDTO
        {
            SourceField = Game == SupportedGame.Starfield ? "Text" : "BookText",
            Value = Text
        };
        yield return new TranslatedFieldDTO { SourceField = "Description", Value = Description };
        yield return new TranslatedFieldDTO { SourceField = "DataSlateHeaderLeft", Value = DataSlateHeaderLeft };
        yield return new TranslatedFieldDTO { SourceField = "DataSlateHeaderRight", Value = DataSlateHeaderRight };
    }
}
