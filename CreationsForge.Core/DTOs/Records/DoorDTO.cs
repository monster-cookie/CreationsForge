using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents a door record and its typed child data.
/// </summary>
public class DoorDTO : RecordDTO, IHasModelsDTO, IKeywords, ISounds, IHasScriptingAdaptersDTO, IHasComponentsDTO, IHasReflectionDTO
{
    public string? ObjectBoundsFirst { get; set; }

    public string? ObjectBoundsSecond { get; set; }

    public TranslatedStringDTO? Name { get; set; }

    public string? Flags { get; set; }

    /// <summary>
    /// Gets or sets the named major record flags exported by Spriggit for this door.
    /// </summary>
    public string? MajorFlags { get; set; }

    public FormKeyDTO? NativeTerminalFormKey { get; set; }

    public string? SoundLevel { get; set; }

    public string? FacingAxisOverride { get; set; }

    public string? AnimationGraph { get; set; }

    public string? AnimationSkeleton { get; set; }

    public string? AnimationDirectory { get; set; }

    public string? AnimationFile { get; set; }

    public IList<ModelDTO> Models { get; set; } = new List<ModelDTO>();

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<SoundMappingDTO> Sounds { get; set; } = new List<SoundMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<RecordComponentDTO> Components { get; set; } = new List<RecordComponentDTO>();

    /// <summary>
    /// Gets or sets component reflection rows exported by Spriggit as <c>REFL</c> fields.
    /// </summary>
    public IList<ReflectionDTO> Reflections { get; set; } = new List<ReflectionDTO>();

    /// <summary>
    /// Gets or sets forced location form-key references attached to this door.
    /// </summary>
    public IList<FormKeyDTO> ForcedLocations { get; set; } = new List<FormKeyDTO>();

    /// <summary>
    /// Gets or sets the navmesh geometry exported by Spriggit for doors that define navigation data.
    /// </summary>
    public StaticNavmeshGeometryDTO? NavmeshGeometry { get; set; }
}
