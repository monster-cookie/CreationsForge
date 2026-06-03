using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("PerkRankEffects")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index, Effect_Index", AutoIncrement = false)]
public class PerkRankEffect
{
    public PerkRankEffect()
    { }

    public PerkRankEffect(PerkRankEffectDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyModKeyName = dto.FormKey.ModKey.Name;
        FormKeyModKeyType = (int)dto.FormKey.ModKey.Type;
        FormKeyModKeyFileName = dto.FormKey.ModKey.FileName;
        FormKeyId = (int)dto.FormKey.ID;
        RankIndex = dto.RankIndex;
        EffectIndex = dto.EffectIndex;
        MutagenObjectType = dto.MutagenObjectType;
        Rank = dto.Rank;
        Priority = dto.Priority;
        PerkEntryId = dto.PerkEntryId;
        Flags = dto.Flags;
        ButtonLabel = dto.ButtonLabel;
        ConditionCount = dto.ConditionCount;
        EntryPoint = dto.EntryPoint;
        PerkConditionTabCount = dto.PerkConditionTabCount;
        Modification = dto.Modification;
        Value = dto.Value;
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
    [Column("Effect_Index")] public int EffectIndex { get; set; }
    [Column("MutagenObjectType")] public string MutagenObjectType { get; set; } = string.Empty;
    [Column("Rank")] public int Rank { get; set; }
    [Column("Priority")] public int Priority { get; set; }
    [Column("PerkEntryID")] public int? PerkEntryId { get; set; }
    [Column("Flags")] public string? Flags { get; set; }
    [Column("ButtonLabel")] public string? ButtonLabel { get; set; }
    [Column("ConditionCount")] public int ConditionCount { get; set; }
    [Column("EntryPoint")] public string? EntryPoint { get; set; }
    [Column("PerkConditionTabCount")] public int? PerkConditionTabCount { get; set; }
    [Column("Modification")] public string? Modification { get; set; }
    [Column("Value")] public float? Value { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}

