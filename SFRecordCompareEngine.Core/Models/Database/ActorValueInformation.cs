using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("ActorValueInformation")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class ActorValueInformation
{
    public ActorValueInformation()
    { }

    public ActorValueInformation(ActorValueInformationDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyId = (int)dto.FormKey.ID;
        EditorId = dto.EditorID;
        FormVersion = dto.FormVersion;
        StarfieldMajorRecordFlags = (int)dto.StarfieldMajorRecordFlags;
        Version2 = dto.Version2;
        VersionControl = dto.VersionControl;
        ImportedAtUTC = dto.ImportedAtUTC;
        Name = dto.Name;
        Abbreviation = dto.Abbreviation;
        ContextNotes = dto.ContextNotes;
        DefaultValue = dto.DefaultValue;
        Flags = dto.Flags;
        Type = dto.Type;
        Min = dto.Min;
        Max = dto.Max;
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("EditorID")] public string EditorId { get; set; } = string.Empty;
    [Column("FormVersion")] public int FormVersion { get; set; }
    [Column("StarfieldMajorRecordFlags")] public int StarfieldMajorRecordFlags { get; set; }
    [Column("Version2")] public int Version2 { get; set; }
    [Column("VersionControl")] public int VersionControl { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
    [Column("Name")] public string? Name { get; set; }
    [Column("Abbreviation")] public string? Abbreviation { get; set; }
    [Column("ContextNotes")] public string? ContextNotes { get; set; }
    [Column("DefaultValue")] public double? DefaultValue { get; set; }
    [Column("Flags")] public string? Flags { get; set; }
    [Column("Type")] public string? Type { get; set; }
    [Column("Min")] public double? Min { get; set; }
    [Column("Max")] public double? Max { get; set; }
}