using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("PerkRanks")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index", AutoIncrement = false)]
public class PerkRank
{
    public PerkRank()
    { }

    public PerkRank(PerkRankDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyModKeyName = dto.FormKey.ModKey.Name;
        FormKeyModKeyType = (int)dto.FormKey.ModKey.Type;
        FormKeyModKeyFileName = dto.FormKey.ModKey.FileName;
        FormKeyId = (int)dto.FormKey.ID;
        RankIndex = dto.RankIndex;
        Description = dto.Description;
        if (dto.UnknownStaticFormKey.HasValue)
        {
            UnknownStaticModKeyName = dto.UnknownStaticFormKey.Value.ModKey.Name;
            UnknownStaticModKeyType = (int)dto.UnknownStaticFormKey.Value.ModKey.Type;
            UnknownStaticModKeyFileName = dto.UnknownStaticFormKey.Value.ModKey.FileName;
            UnknownStaticFormKeyId = (int)dto.UnknownStaticFormKey.Value.ID;
        }

        ConditionCount = dto.ConditionCount;
        ActivityCount = dto.ActivityCount;
        ImportedAtUTC = dto.ImportedAtUTC;
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; } = (int)ModType.Master;
    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("Rank_Index")] public int RankIndex { get; set; }
    [Column("Description")] public string? Description { get; set; }
    [Column("UnknownStatic_ModKey_Name")] public string? UnknownStaticModKeyName { get; set; }
    [Column("UnknownStatic_ModKey_Type")] public int? UnknownStaticModKeyType { get; set; }
    [Column("UnknownStatic_ModKey_FileName")] public string? UnknownStaticModKeyFileName { get; set; }
    [Column("UnknownStatic_FormKey_ID")] public int? UnknownStaticFormKeyId { get; set; }
    [Column("ConditionCount")] public int ConditionCount { get; set; }
    [Column("ActivityCount")] public int ActivityCount { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
