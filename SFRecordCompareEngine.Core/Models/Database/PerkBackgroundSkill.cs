using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("PerkBackgroundSkills")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Skill_Index", AutoIncrement = false)]
public class PerkBackgroundSkill
{
    public PerkBackgroundSkill()
    { }

    public PerkBackgroundSkill(PerkBackgroundSkillDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyModKeyName = dto.FormKey.ModKey.Name;
        FormKeyModKeyType = (int)dto.FormKey.ModKey.Type;
        FormKeyModKeyFileName = dto.FormKey.ModKey.FileName;
        FormKeyId = (int)dto.FormKey.ID;
        SkillModKeyName = dto.SkillFormKey.ModKey.Name;
        SkillModKeyType = (int)dto.SkillFormKey.ModKey.Type;
        SkillModKeyFileName = dto.SkillFormKey.ModKey.FileName;
        SkillFormKeyId = (int)dto.SkillFormKey.ID;
        SkillIndex = dto.SkillIndex;
        ImportedAtUTC = dto.ImportedAtUTC;
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; } = (int)ModType.Master;
    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("Skill_ModKey_Name")] public string SkillModKeyName { get; set; } = string.Empty;
    [Column("Skill_ModKey_Type")] public int SkillModKeyType { get; set; } = (int)ModType.Master;
    [Column("Skill_ModKey_FileName")] public string SkillModKeyFileName { get; set; } = string.Empty;
    [Column("Skill_FormKey_ID")] public int SkillFormKeyId { get; set; }
    [Column("Skill_Index")] public int SkillIndex { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}

