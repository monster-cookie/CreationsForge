using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("Perk")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID", AutoIncrement = false)]
public class Perk
{
    public Perk()
    { }

    public Perk(PerkDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyModKeyName = dto.FormKey.ModKey.Name;
        FormKeyModKeyType = (int)dto.FormKey.ModKey.Type;
        FormKeyModKeyFileName = dto.FormKey.ModKey.FileName;
        FormKeyId = (int)dto.FormKey.ID;
        EditorId = dto.EditorID;
        FormVersion = dto.FormVersion;
        StarfieldMajorRecordFlags = (int)dto.StarfieldMajorRecordFlags;
        Version2 = dto.Version2;
        VersionControl = dto.VersionControl;
        ImportedAtUTC = dto.ImportedAtUTC;
        Name = dto.Name;
        Description = dto.Description;
        Flags = dto.Flags;
        SkillGroup = dto.SkillGroup;
        CrewAssignment = dto.CrewAssignment;
        PerkIcon = dto.PerkIcon;
        Category = dto.Category;
        if (dto.RestrictionFormKey.HasValue)
        {
            RestrictionModKeyName = dto.RestrictionFormKey.Value.ModKey.Name;
            RestrictionModKeyType = (int)dto.RestrictionFormKey.Value.ModKey.Type;
            RestrictionModKeyFileName = dto.RestrictionFormKey.Value.ModKey.FileName;
            RestrictionFormKeyId = (int)dto.RestrictionFormKey.Value.ID;
        }

        if (dto.TrainingFormKey.HasValue)
        {
            TrainingModKeyName = dto.TrainingFormKey.Value.ModKey.Name;
            TrainingModKeyType = (int)dto.TrainingFormKey.Value.ModKey.Type;
            TrainingModKeyFileName = dto.TrainingFormKey.Value.ModKey.FileName;
            TrainingFormKeyId = (int)dto.TrainingFormKey.Value.ID;
        }

        MajorFlags = dto.MajorFlags;
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; } = (int)ModType.Master;
    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;
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
    [Column("Category")] public string? Category { get; set; }
    [Column("Restriction_ModKey_Name")] public string? RestrictionModKeyName { get; set; }
    [Column("Restriction_ModKey_Type")] public int? RestrictionModKeyType { get; set; }
    [Column("Restriction_ModKey_FileName")] public string? RestrictionModKeyFileName { get; set; }
    [Column("Restriction_FormKey_ID")] public int? RestrictionFormKeyId { get; set; }
    [Column("Training_ModKey_Name")] public string? TrainingModKeyName { get; set; }
    [Column("Training_ModKey_Type")] public int? TrainingModKeyType { get; set; }
    [Column("Training_ModKey_FileName")] public string? TrainingModKeyFileName { get; set; }
    [Column("Training_FormKey_ID")] public int? TrainingFormKeyId { get; set; }
    [Column("MajorFlags")] public string? MajorFlags { get; set; }
}
