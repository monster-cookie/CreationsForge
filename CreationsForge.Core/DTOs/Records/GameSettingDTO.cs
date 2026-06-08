namespace CreationsForge.Core.DTOs.Records;

public class GameSettingDTO : RecordDTO
{
    public string? SettingType { get; set; }

    public string? Data { get; set; }

    public double? NumericData { get; set; }

    public int? IntegerData { get; set; }

    public bool? BooleanData { get; set; }
}
