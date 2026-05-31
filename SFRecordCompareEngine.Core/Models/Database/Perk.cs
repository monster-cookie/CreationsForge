using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("Perk")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class Perk
{
    public Perk()
    { }

    public Perk(PerkDTO dto)
    {
        ModKeyName = dto.ModKey.Name; ModKeyType = (int)dto.ModKey.Type; ModKeyFileName = dto.ModKey.FileName; FormKeyId = (int)dto.FormKey.ID;
        EditorId = dto.EditorID; FormVersion = dto.FormVersion; StarfieldMajorRecordFlags = (int)dto.StarfieldMajorRecordFlags; Version2 = dto.Version2; VersionControl = dto.VersionControl; ImportedAtUTC = dto.ImportedAtUTC;
        Name = dto.Name; Description = dto.Description; Flags = dto.Flags; SkillGroup = dto.SkillGroup; CrewAssignment = dto.CrewAssignment; PerkIcon = dto.PerkIcon;
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
    [Column("Description")] public string? Description { get; set; }
    [Column("Flags")] public string Flags { get; set; } = string.Empty;
    [Column("SkillGroup")] public string? SkillGroup { get; set; }
    [Column("CrewAssignment")] public string? CrewAssignment { get; set; }
    [Column("PerkIcon")] public string? PerkIcon { get; set; }
}
