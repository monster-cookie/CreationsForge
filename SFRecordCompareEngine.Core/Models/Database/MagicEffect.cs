using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("MagicEffect")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class MagicEffect
{
    public MagicEffect()
    { }

    public MagicEffect(MagicEffectDTO dto)
    {
        ModKeyName = dto.ModKey.Name; ModKeyType = (int)dto.ModKey.Type; ModKeyFileName = dto.ModKey.FileName; FormKeyId = (int)dto.FormKey.ID;
        EditorId = dto.EditorID; FormVersion = dto.FormVersion; StarfieldMajorRecordFlags = (int)dto.StarfieldMajorRecordFlags; Version2 = dto.Version2; VersionControl = dto.VersionControl; ImportedAtUTC = dto.ImportedAtUTC;
        Name = dto.Name; Description = dto.Description; Flags = dto.Flags; CastType = dto.CastType; TargetType = dto.TargetType;
        ActorValue2FormKey = dto.ActorValue2FormKey?.ToString(); ResistValueFormKey = dto.ResistValueFormKey?.ToString(); PerkToApplyFormKey = dto.PerkToApplyFormKey?.ToString(); EquipAbilityFormKey = dto.EquipAbilityFormKey?.ToString(); ExplosionFormKey = dto.ExplosionFormKey?.ToString(); CastingArtFormKey = dto.CastingArtFormKey?.ToString(); HitEffectArtFormKey = dto.HitEffectArtFormKey?.ToString(); HitShaderFormKey = dto.HitShaderFormKey?.ToString(); ImageSpaceModifierFormKey = dto.ImageSpaceModifierFormKey?.ToString(); ImpactDataFormKey = dto.ImpactDataFormKey?.ToString(); ProjectileFormKey = dto.ProjectileFormKey?.ToString();
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
    [Column("CastType")] public string? CastType { get; set; }
    [Column("TargetType")] public string? TargetType { get; set; }
    [Column("ActorValue2FormKey")] public string? ActorValue2FormKey { get; set; }
    [Column("ResistValueFormKey")] public string? ResistValueFormKey { get; set; }
    [Column("PerkToApplyFormKey")] public string? PerkToApplyFormKey { get; set; }
    [Column("EquipAbilityFormKey")] public string? EquipAbilityFormKey { get; set; }
    [Column("ExplosionFormKey")] public string? ExplosionFormKey { get; set; }
    [Column("CastingArtFormKey")] public string? CastingArtFormKey { get; set; }
    [Column("HitEffectArtFormKey")] public string? HitEffectArtFormKey { get; set; }
    [Column("HitShaderFormKey")] public string? HitShaderFormKey { get; set; }
    [Column("ImageSpaceModifierFormKey")] public string? ImageSpaceModifierFormKey { get; set; }
    [Column("ImpactDataFormKey")] public string? ImpactDataFormKey { get; set; }
    [Column("ProjectileFormKey")] public string? ProjectileFormKey { get; set; }
}
