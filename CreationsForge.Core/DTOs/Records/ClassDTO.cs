namespace CreationsForge.Core.DTOs.Records;

public class ClassDTO : RecordDTO
{
    public int? Version2 { get; set; }

    public TranslatedStringDTO? Name { get; set; }

    public TranslatedStringDTO? Description { get; set; }

    public string? Teaches { get; set; }

    public int? MaxTrainingLevel { get; set; }

    public double? BleedoutDefault { get; set; }

    public double? VoicePoints { get; set; }

    public double? Unknown { get; set; }

    public double? Unknown2 { get; set; }

    public IList<ClassPropertyDTO> Properties { get; set; } = new List<ClassPropertyDTO>();

    public IList<ClassWeightDTO> SkillWeights { get; set; } = new List<ClassWeightDTO>();

    public IList<ClassWeightDTO> StatWeights { get; set; } = new List<ClassWeightDTO>();
}
