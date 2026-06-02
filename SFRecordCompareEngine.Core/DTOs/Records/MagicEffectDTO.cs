using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;
using MagicEffect = SFRecordCompareEngine.Core.Models.Database.MagicEffect;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class MagicEffectDTO : IHasScriptingAdaptersRecordDTO
{
    public MagicEffectDTO()
    { }

    [SetsRequiredMembers]
    public MagicEffectDTO(MagicEffect model)
    {
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        FormKey = new FormKey(ModKey, (uint)model.FormKeyId);
        EditorID = model.EditorId;
        FormVersion = model.FormVersion;
        StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)model.StarfieldMajorRecordFlags;
        Version2 = model.Version2;
        VersionControl = model.VersionControl;
        ImportedAtUTC = model.ImportedAtUTC;
        Name = model.Name;
        Description = model.Description;
        Flags = model.Flags;
        CastType = model.CastType;
        TargetType = model.TargetType;
        ActorValue2FormKey = ParseFormKey(model.ActorValue2FormKey);
        ResistValueFormKey = ParseFormKey(model.ResistValueFormKey);
        PerkToApplyFormKey = ParseFormKey(model.PerkToApplyFormKey);
        EquipAbilityFormKey = ParseFormKey(model.EquipAbilityFormKey);
        ExplosionFormKey = ParseFormKey(model.ExplosionFormKey);
        CastingArtFormKey = ParseFormKey(model.CastingArtFormKey);
        HitEffectArtFormKey = ParseFormKey(model.HitEffectArtFormKey);
        HitShaderFormKey = ParseFormKey(model.HitShaderFormKey);
        ImageSpaceModifierFormKey = ParseFormKey(model.ImageSpaceModifierFormKey);
        ImpactDataFormKey = ParseFormKey(model.ImpactDataFormKey);
        ProjectileFormKey = ParseFormKey(model.ProjectileFormKey);
    }

    public required ModKey ModKey { get; set; }
    public required FormKey FormKey { get; set; }
    public required string EditorID { get; set; }
    public required int FormVersion { get; set; }
    public required StarfieldMajorRecord.StarfieldMajorRecordFlag StarfieldMajorRecordFlags { get; set; }
    public required int Version2 { get; set; }
    public required int VersionControl { get; set; }
    public required DateTime ImportedAtUTC { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public required string Flags { get; set; }
    public string? CastType { get; set; }
    public string? TargetType { get; set; }
    public FormKey? ActorValue2FormKey { get; set; }
    public FormKey? ResistValueFormKey { get; set; }
    public FormKey? PerkToApplyFormKey { get; set; }
    public FormKey? EquipAbilityFormKey { get; set; }
    public FormKey? ExplosionFormKey { get; set; }
    public FormKey? CastingArtFormKey { get; set; }
    public FormKey? HitEffectArtFormKey { get; set; }
    public FormKey? HitShaderFormKey { get; set; }
    public FormKey? ImageSpaceModifierFormKey { get; set; }
    public FormKey? ImpactDataFormKey { get; set; }
    public FormKey? ProjectileFormKey { get; set; }
    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    private static FormKey? ParseFormKey(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : FormKey.Factory(value);
    }
}
