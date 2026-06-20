using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class PerkDTO : RecordDTO, IHasScriptingAdaptersRecordDTO
{
    public TranslatedStringDTO? Name { get; set; }

    public TranslatedStringDTO? Description { get; set; }

    public required string Flags { get; set; }

    public string? SkillGroup { get; set; }

    public string? CrewAssignment { get; set; }

    public string? PerkIcon { get; set; }

    public string? Category { get; set; }

    public FormKeyDTO? RestrictionFormKey { get; set; }

    public FormKeyDTO? TrainingFormKey { get; set; }

    public string? MajorFlags { get; set; }

    public IList<PerkRankDTO> Ranks { get; set; } = new List<PerkRankDTO>();

    public IList<PerkBackgroundSkillDTO> BackgroundSkills { get; set; } = new List<PerkBackgroundSkillDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
