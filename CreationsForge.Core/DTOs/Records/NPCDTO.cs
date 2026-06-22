using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class NPCDTO : RecordDTO, IHasScriptingAdaptersRecordDTO, IKeywords
{
    public TranslatedStringDTO? Name { get; set; }

    public TranslatedStringDTO? ShortName { get; set; }

    public TranslatedStringDTO? LongName { get; set; }

    public int DispositionBase { get; set; }

    public required string Aggression { get; set; }

    public required string Confidence { get; set; }

    public int EnergyLevel { get; set; }

    public required string Responsibility { get; set; }

    public required string Assistance { get; set; }

    public int GearedUpWeapons { get; set; }

    public double HeightMin { get; set; }

    public double HeightMax { get; set; }

    public int? SkinToneIndex { get; set; }

    public string? Pronoun { get; set; }

    public FormKeyDTO? VoiceFormKey { get; set; }

    public FormKeyDTO? RaceFormKey { get; set; }

    public FormKeyDTO? CombatOverridePackageListFormKey { get; set; }

    public FormKeyDTO? CombatStyleFormKey { get; set; }

    public FormKeyDTO? DefaultPackageListFormKey { get; set; }

    public FormKeyDTO? CrimeFactionFormKey { get; set; }

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
