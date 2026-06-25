using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents a container record and its typed child data.
/// </summary>
public class ContainerDTO : RecordDTO, IHasModelsDTO, IKeywords, ISounds, IHasScriptingAdaptersDTO, IHasComponentsDTO, IHasReflectionDTO
{
    public string? ObjectBoundsFirst { get; set; }

    public string? ObjectBoundsSecond { get; set; }

    public TranslatedStringDTO? Name { get; set; }

    public string? Flags { get; set; }

    public string? MajorFlags { get; set; }

    public FormKeyDTO? NativeTerminalFormKey { get; set; }

    /// <summary>
    /// Gets or sets the snap template link for Starfield containers, or <c>null</c> when the source omits it.
    /// </summary>
    public FormKeyDTO? SnapTemplate { get; set; }

    /// <summary>
    /// Gets or sets the contains-only filter link for Starfield display containers, or <c>null</c> when omitted.
    /// </summary>
    public FormKeyDTO? ContainsOnlyFilter { get; set; }

    /// <summary>
    /// Gets or sets transform links exported by Spriggit under the <c>Transforms</c> object.
    /// </summary>
    public ContainerTransformsDTO? Transforms { get; set; }

    public string? AnimationGraph { get; set; }

    public string? AnimationSkeleton { get; set; }

    public string? AnimationDirectory { get; set; }

    public string? AnimationFile { get; set; }

    public IList<ContainerItemDTO> Items { get; set; } = new List<ContainerItemDTO>();

    /// <summary>
    /// Gets or sets actor-value property entries for the container.
    /// </summary>
    public IList<ContainerPropertyDTO> Properties { get; set; } = new List<ContainerPropertyDTO>();

    /// <summary>
    /// Gets or sets forced location links for the container.
    /// </summary>
    public IList<FormKeyDTO> ForcedLocations { get; set; } = new List<FormKeyDTO>();

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<SoundMappingDTO> Sounds { get; set; } = new List<SoundMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    /// <summary>
    /// Gets or sets structured component rows for container components such as display cases.
    /// </summary>
    public IList<RecordComponentDTO> Components { get; set; } = new List<RecordComponentDTO>();

    /// <summary>
    /// Gets or sets component reflection rows exported by Spriggit as <c>REFL</c> fields.
    /// </summary>
    public IList<ReflectionDTO> Reflections { get; set; } = new List<ReflectionDTO>();
}
