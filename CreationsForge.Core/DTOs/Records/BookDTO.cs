using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.DTOs.Records.Metadata;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents a book or slate record and its typed child data.
/// </summary>
public class BookDTO : RecordDTO, IHasName, IHasText, IHasTranslatedFields, IHasModelsDTO, IKeywords, ISounds, IHasScriptingAdaptersDTO, IHasComponentsDTO, IHasReflectionDTO
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

    [LocalizedField("Description")]
    public TranslatedStringDTO? Description { get; set; }

    public TranslatedStringDTO? DataSlateHeaderLeft { get; set; }

    public TranslatedStringDTO? DataSlateHeaderRight { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<SoundMappingDTO> Sounds { get; set; } = new List<SoundMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<RecordComponentDTO> Components { get; set; } = new List<RecordComponentDTO>();

    /// <summary>
    /// Gets or sets component reflection rows exported by Spriggit as <c>REFL</c> fields.
    /// </summary>
    public IList<ReflectionDTO> Reflections { get; set; } = new List<ReflectionDTO>();

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
