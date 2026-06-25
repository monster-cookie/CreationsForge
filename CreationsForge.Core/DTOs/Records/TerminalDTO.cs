using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents a terminal record and its typed child data.
/// </summary>
public class TerminalDTO : RecordDTO, IHasModelsDTO, IKeywords, IHasScriptingAdaptersDTO, IHasScriptFragmentsDTO, IHasReflectionDTO, IHasTerminalMarkerParametersRecordDTO, IHasConditionsDTO
{
    public string? ObjectBoundsFirst { get; set; }

    public string? ObjectBoundsSecond { get; set; }

    public FormKeyDTO? MenuFormKey { get; set; }

    public string? Background { get; set; }

    public TranslatedStringDTO? HeaderText { get; set; }

    public TranslatedStringDTO? WelcomeText { get; set; }

    public TranslatedStringDTO? Name { get; set; }

    public string? Pnam { get; set; }

    public string? Fnam { get; set; }

    public string? Flags { get; set; }

    public string? MajorFlags { get; set; }

    public string? Jnam { get; set; }

    public string? MarkerFlags { get; set; }

    public string? Gnam { get; set; }

    public string? WorkbenchData { get; set; }

    public FormKeyDTO? FurnitureTemplateFormKey { get; set; }

    public string? MarkerModel { get; set; }

    public string? AnimationGraph { get; set; }

    public string? AnimationSkeleton { get; set; }

    public string? AnimationDirectory { get; set; }

    public string? AnimationFile { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<ScriptFragmentDTO> ScriptFragments { get; set; } = new List<ScriptFragmentDTO>();

    /// <summary>
    /// Gets or sets component reflection rows exported by Spriggit as <c>REFL</c> fields.
    /// </summary>
    public IList<ReflectionDTO> Reflections { get; set; } = new List<ReflectionDTO>();

    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();

    public IList<TerminalMarkerParameterDTO> MarkerParameters { get; set; } = new List<TerminalMarkerParameterDTO>();

    public IList<FormKeyDTO> ForcedLocations { get; set; } = new List<FormKeyDTO>();

    public IList<TerminalBodyTextDTO> BodyTexts { get; set; } = new List<TerminalBodyTextDTO>();

    public IList<TerminalMenuItemDTO> MenuItems { get; set; } = new List<TerminalMenuItemDTO>();
}
