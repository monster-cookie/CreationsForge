using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.DTOs.Records.Metadata;

namespace CreationsForge.Core.DTOs.Records;

public class ActorValueInformationDTO : RecordDTO, IHasName, IHasTranslatedFields, IHasScriptingAdaptersRecordDTO
{
    [LocalizedField("Name")]
    public TranslatedStringDTO? Name { get; set; }

    [LocalizedField("Abbreviation")]
    public TranslatedStringDTO? Abbreviation { get; set; }

    [LocalizedField("Description")]
    public TranslatedStringDTO? Description { get; set; }

    public string? CNAM { get; set; }

    public ActorValueInformationSkillDTO? Skill { get; set; }

    public string? ContextNotes { get; set; }

    public double? DefaultValue { get; set; }

    public string? Flags { get; set; }

    public string? Type { get; set; }

    public double? Min { get; set; }

    public double? Max { get; set; }

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<ActorValueInformationPerkTreeEntryDTO> PerkTree { get; set; } = new List<ActorValueInformationPerkTreeEntryDTO>();

    public IEnumerable<TranslatedFieldDTO> GetTranslatedFields()
    {
        yield return new TranslatedFieldDTO { SourceField = "Name", Value = Name };
        yield return new TranslatedFieldDTO { SourceField = "Abbreviation", Value = Abbreviation };
        yield return new TranslatedFieldDTO { SourceField = "Description", Value = Description };
    }
}
