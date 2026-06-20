using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class MagicEffectRepository : TypedRecordRepositoryBase, IMagicEffectRepository
{
    public MagicEffectRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.MagicEffect.RecordID;

    protected override string TableName => RecordTypeCatalog.MagicEffect.TableName;

    public IReadOnlyList<MagicEffectDTO> GetByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        return FetchByFormKey<MagicEffectRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("Description"),
                    SelectColumn("Flags"),
                    SelectColumn("CastType"),
                    SelectColumn("TargetType"),
                    SelectColumn("ActorValue2_ModKey_Name", "ActorValue2ModKeyName"),
                    SelectColumn("ActorValue2_ModKey_Type", "ActorValue2ModKeyType"),
                    SelectColumn("ActorValue2_ModKey_FileName", "ActorValue2ModKeyFileName"),
                    SelectColumn("ActorValue2_FormKey_ID", "ActorValue2FormKeyId"),
                    SelectColumn("ResistValue_ModKey_Name", "ResistValueModKeyName"),
                    SelectColumn("ResistValue_ModKey_Type", "ResistValueModKeyType"),
                    SelectColumn("ResistValue_ModKey_FileName", "ResistValueModKeyFileName"),
                    SelectColumn("ResistValue_FormKey_ID", "ResistValueFormKeyId"),
                    SelectColumn("PerkToApply_ModKey_Name", "PerkToApplyModKeyName"),
                    SelectColumn("PerkToApply_ModKey_Type", "PerkToApplyModKeyType"),
                    SelectColumn("PerkToApply_ModKey_FileName", "PerkToApplyModKeyFileName"),
                    SelectColumn("PerkToApply_FormKey_ID", "PerkToApplyFormKeyId"),
                    SelectColumn("EquipAbility_ModKey_Name", "EquipAbilityModKeyName"),
                    SelectColumn("EquipAbility_ModKey_Type", "EquipAbilityModKeyType"),
                    SelectColumn("EquipAbility_ModKey_FileName", "EquipAbilityModKeyFileName"),
                    SelectColumn("EquipAbility_FormKey_ID", "EquipAbilityFormKeyId"),
                    SelectColumn("Explosion_ModKey_Name", "ExplosionModKeyName"),
                    SelectColumn("Explosion_ModKey_Type", "ExplosionModKeyType"),
                    SelectColumn("Explosion_ModKey_FileName", "ExplosionModKeyFileName"),
                    SelectColumn("Explosion_FormKey_ID", "ExplosionFormKeyId"),
                    SelectColumn("CastingArt_ModKey_Name", "CastingArtModKeyName"),
                    SelectColumn("CastingArt_ModKey_Type", "CastingArtModKeyType"),
                    SelectColumn("CastingArt_ModKey_FileName", "CastingArtModKeyFileName"),
                    SelectColumn("CastingArt_FormKey_ID", "CastingArtFormKeyId"),
                    SelectColumn("HitEffectArt_ModKey_Name", "HitEffectArtModKeyName"),
                    SelectColumn("HitEffectArt_ModKey_Type", "HitEffectArtModKeyType"),
                    SelectColumn("HitEffectArt_ModKey_FileName", "HitEffectArtModKeyFileName"),
                    SelectColumn("HitEffectArt_FormKey_ID", "HitEffectArtFormKeyId"),
                    SelectColumn("HitShader_ModKey_Name", "HitShaderModKeyName"),
                    SelectColumn("HitShader_ModKey_Type", "HitShaderModKeyType"),
                    SelectColumn("HitShader_ModKey_FileName", "HitShaderModKeyFileName"),
                    SelectColumn("HitShader_FormKey_ID", "HitShaderFormKeyId"),
                    SelectColumn("ImageSpaceModifier_ModKey_Name", "ImageSpaceModifierModKeyName"),
                    SelectColumn("ImageSpaceModifier_ModKey_Type", "ImageSpaceModifierModKeyType"),
                    SelectColumn("ImageSpaceModifier_ModKey_FileName", "ImageSpaceModifierModKeyFileName"),
                    SelectColumn("ImageSpaceModifier_FormKey_ID", "ImageSpaceModifierFormKeyId"),
                    SelectColumn("ImpactData_ModKey_Name", "ImpactDataModKeyName"),
                    SelectColumn("ImpactData_ModKey_Type", "ImpactDataModKeyType"),
                    SelectColumn("ImpactData_ModKey_FileName", "ImpactDataModKeyFileName"),
                    SelectColumn("ImpactData_FormKey_ID", "ImpactDataFormKeyId"),
                    SelectColumn("Projectile_ModKey_Name", "ProjectileModKeyName"),
                    SelectColumn("Projectile_ModKey_Type", "ProjectileModKeyType"),
                    SelectColumn("Projectile_ModKey_FileName", "ProjectileModKeyFileName"),
                    SelectColumn("Projectile_FormKey_ID", "ProjectileFormKeyId"),
                    SelectColumn("Archetype"),
                    SelectColumn("UnknownFloat3"),
                    SelectColumn("UnknownInt2"),
                    SelectColumn("Unknown"),
                    SelectColumn("Unknown2"),
                    SelectColumn("DataTypeState")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
    }

    public void Save(MagicEffectDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO MagicEffects (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, Description, Flags, CastType, TargetType,
                ActorValue2_ModKey_Name, ActorValue2_ModKey_Type, ActorValue2_ModKey_FileName, ActorValue2_FormKey_ID,
                ResistValue_ModKey_Name, ResistValue_ModKey_Type, ResistValue_ModKey_FileName, ResistValue_FormKey_ID,
                PerkToApply_ModKey_Name, PerkToApply_ModKey_Type, PerkToApply_ModKey_FileName, PerkToApply_FormKey_ID,
                EquipAbility_ModKey_Name, EquipAbility_ModKey_Type, EquipAbility_ModKey_FileName, EquipAbility_FormKey_ID,
                Explosion_ModKey_Name, Explosion_ModKey_Type, Explosion_ModKey_FileName, Explosion_FormKey_ID,
                CastingArt_ModKey_Name, CastingArt_ModKey_Type, CastingArt_ModKey_FileName, CastingArt_FormKey_ID,
                HitEffectArt_ModKey_Name, HitEffectArt_ModKey_Type, HitEffectArt_ModKey_FileName, HitEffectArt_FormKey_ID,
                HitShader_ModKey_Name, HitShader_ModKey_Type, HitShader_ModKey_FileName, HitShader_FormKey_ID,
                ImageSpaceModifier_ModKey_Name, ImageSpaceModifier_ModKey_Type, ImageSpaceModifier_ModKey_FileName, ImageSpaceModifier_FormKey_ID,
                ImpactData_ModKey_Name, ImpactData_ModKey_Type, ImpactData_ModKey_FileName, ImpactData_FormKey_ID,
                Projectile_ModKey_Name, Projectile_ModKey_Type, Projectile_ModKey_FileName, Projectile_FormKey_ID,
                Archetype, UnknownFloat3, UnknownInt2, Unknown, Unknown2, DataTypeState)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @Description, @Flags, @CastType, @TargetType,
                @ActorValue2ModKeyName, @ActorValue2ModKeyType, @ActorValue2ModKeyFileName, @ActorValue2FormKeyId,
                @ResistValueModKeyName, @ResistValueModKeyType, @ResistValueModKeyFileName, @ResistValueFormKeyId,
                @PerkToApplyModKeyName, @PerkToApplyModKeyType, @PerkToApplyModKeyFileName, @PerkToApplyFormKeyId,
                @EquipAbilityModKeyName, @EquipAbilityModKeyType, @EquipAbilityModKeyFileName, @EquipAbilityFormKeyId,
                @ExplosionModKeyName, @ExplosionModKeyType, @ExplosionModKeyFileName, @ExplosionFormKeyId,
                @CastingArtModKeyName, @CastingArtModKeyType, @CastingArtModKeyFileName, @CastingArtFormKeyId,
                @HitEffectArtModKeyName, @HitEffectArtModKeyType, @HitEffectArtModKeyFileName, @HitEffectArtFormKeyId,
                @HitShaderModKeyName, @HitShaderModKeyType, @HitShaderModKeyFileName, @HitShaderFormKeyId,
                @ImageSpaceModifierModKeyName, @ImageSpaceModifierModKeyType, @ImageSpaceModifierModKeyFileName, @ImageSpaceModifierFormKeyId,
                @ImpactDataModKeyName, @ImpactDataModKeyType, @ImpactDataModKeyFileName, @ImpactDataFormKeyId,
                @ProjectileModKeyName, @ProjectileModKeyType, @ProjectileModKeyFileName, @ProjectileFormKeyId,
                @Archetype, @UnknownFloat3, @UnknownInt2, @Unknown, @Unknown2, @DataTypeState);
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id,
                EditorId = dto.EditorID,
                dto.FormVersion,
                dto.MajorRecordFlags,
                dto.ImportedAtUTC,
                Name = GetEnglishText(dto.Name),
                Description = GetEnglishText(dto.Description),
                dto.Flags,
                dto.CastType,
                dto.TargetType,
                ActorValue2ModKeyName = dto.ActorValue2FormKey?.ModKey.Name,
                ActorValue2ModKeyType = dto.ActorValue2FormKey?.ModKey.Type,
                ActorValue2ModKeyFileName = dto.ActorValue2FormKey?.ModKey.FileName,
                ActorValue2FormKeyId = dto.ActorValue2FormKey?.Id,
                ResistValueModKeyName = dto.ResistValueFormKey?.ModKey.Name,
                ResistValueModKeyType = dto.ResistValueFormKey?.ModKey.Type,
                ResistValueModKeyFileName = dto.ResistValueFormKey?.ModKey.FileName,
                ResistValueFormKeyId = dto.ResistValueFormKey?.Id,
                PerkToApplyModKeyName = dto.PerkToApplyFormKey?.ModKey.Name,
                PerkToApplyModKeyType = dto.PerkToApplyFormKey?.ModKey.Type,
                PerkToApplyModKeyFileName = dto.PerkToApplyFormKey?.ModKey.FileName,
                PerkToApplyFormKeyId = dto.PerkToApplyFormKey?.Id,
                EquipAbilityModKeyName = dto.EquipAbilityFormKey?.ModKey.Name,
                EquipAbilityModKeyType = dto.EquipAbilityFormKey?.ModKey.Type,
                EquipAbilityModKeyFileName = dto.EquipAbilityFormKey?.ModKey.FileName,
                EquipAbilityFormKeyId = dto.EquipAbilityFormKey?.Id,
                ExplosionModKeyName = dto.ExplosionFormKey?.ModKey.Name,
                ExplosionModKeyType = dto.ExplosionFormKey?.ModKey.Type,
                ExplosionModKeyFileName = dto.ExplosionFormKey?.ModKey.FileName,
                ExplosionFormKeyId = dto.ExplosionFormKey?.Id,
                CastingArtModKeyName = dto.CastingArtFormKey?.ModKey.Name,
                CastingArtModKeyType = dto.CastingArtFormKey?.ModKey.Type,
                CastingArtModKeyFileName = dto.CastingArtFormKey?.ModKey.FileName,
                CastingArtFormKeyId = dto.CastingArtFormKey?.Id,
                HitEffectArtModKeyName = dto.HitEffectArtFormKey?.ModKey.Name,
                HitEffectArtModKeyType = dto.HitEffectArtFormKey?.ModKey.Type,
                HitEffectArtModKeyFileName = dto.HitEffectArtFormKey?.ModKey.FileName,
                HitEffectArtFormKeyId = dto.HitEffectArtFormKey?.Id,
                HitShaderModKeyName = dto.HitShaderFormKey?.ModKey.Name,
                HitShaderModKeyType = dto.HitShaderFormKey?.ModKey.Type,
                HitShaderModKeyFileName = dto.HitShaderFormKey?.ModKey.FileName,
                HitShaderFormKeyId = dto.HitShaderFormKey?.Id,
                ImageSpaceModifierModKeyName = dto.ImageSpaceModifierFormKey?.ModKey.Name,
                ImageSpaceModifierModKeyType = dto.ImageSpaceModifierFormKey?.ModKey.Type,
                ImageSpaceModifierModKeyFileName = dto.ImageSpaceModifierFormKey?.ModKey.FileName,
                ImageSpaceModifierFormKeyId = dto.ImageSpaceModifierFormKey?.Id,
                ImpactDataModKeyName = dto.ImpactDataFormKey?.ModKey.Name,
                ImpactDataModKeyType = dto.ImpactDataFormKey?.ModKey.Type,
                ImpactDataModKeyFileName = dto.ImpactDataFormKey?.ModKey.FileName,
                ImpactDataFormKeyId = dto.ImpactDataFormKey?.Id,
                ProjectileModKeyName = dto.ProjectileFormKey?.ModKey.Name,
                ProjectileModKeyType = dto.ProjectileFormKey?.ModKey.Type,
                ProjectileModKeyFileName = dto.ProjectileFormKey?.ModKey.FileName,
                ProjectileFormKeyId = dto.ProjectileFormKey?.Id,
                dto.Archetype,
                dto.UnknownFloat3,
                dto.UnknownInt2,
                dto.Unknown,
                dto.Unknown2,
                dto.DataTypeState
            });
    }

    private static MagicEffectDTO ToDTO(MagicEffectRow record, CreationsForge.Core.Enums.SupportedGame game)
    {
        var dto = new MagicEffectDTO
        {
            Game = game,
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new CreationsForge.Core.DTOs.Plugins.FormKeyDTO { ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = FromEnglish(record.Name),
            Description = FromEnglish(record.Description),
            Flags = record.Flags,
            CastType = record.CastType,
            TargetType = record.TargetType,
            ActorValue2FormKey = CreateNullableFormKey(record.ActorValue2ModKeyName, record.ActorValue2ModKeyType, record.ActorValue2ModKeyFileName, record.ActorValue2FormKeyId),
            ResistValueFormKey = CreateNullableFormKey(record.ResistValueModKeyName, record.ResistValueModKeyType, record.ResistValueModKeyFileName, record.ResistValueFormKeyId),
            PerkToApplyFormKey = CreateNullableFormKey(record.PerkToApplyModKeyName, record.PerkToApplyModKeyType, record.PerkToApplyModKeyFileName, record.PerkToApplyFormKeyId),
            EquipAbilityFormKey = CreateNullableFormKey(record.EquipAbilityModKeyName, record.EquipAbilityModKeyType, record.EquipAbilityModKeyFileName, record.EquipAbilityFormKeyId),
            ExplosionFormKey = CreateNullableFormKey(record.ExplosionModKeyName, record.ExplosionModKeyType, record.ExplosionModKeyFileName, record.ExplosionFormKeyId),
            CastingArtFormKey = CreateNullableFormKey(record.CastingArtModKeyName, record.CastingArtModKeyType, record.CastingArtModKeyFileName, record.CastingArtFormKeyId),
            HitEffectArtFormKey = CreateNullableFormKey(record.HitEffectArtModKeyName, record.HitEffectArtModKeyType, record.HitEffectArtModKeyFileName, record.HitEffectArtFormKeyId),
            HitShaderFormKey = CreateNullableFormKey(record.HitShaderModKeyName, record.HitShaderModKeyType, record.HitShaderModKeyFileName, record.HitShaderFormKeyId),
            ImageSpaceModifierFormKey = CreateNullableFormKey(record.ImageSpaceModifierModKeyName, record.ImageSpaceModifierModKeyType, record.ImageSpaceModifierModKeyFileName, record.ImageSpaceModifierFormKeyId),
            ImpactDataFormKey = CreateNullableFormKey(record.ImpactDataModKeyName, record.ImpactDataModKeyType, record.ImpactDataModKeyFileName, record.ImpactDataFormKeyId),
            ProjectileFormKey = CreateNullableFormKey(record.ProjectileModKeyName, record.ProjectileModKeyType, record.ProjectileModKeyFileName, record.ProjectileFormKeyId),
            Archetype = record.Archetype,
            UnknownFloat3 = record.UnknownFloat3,
            UnknownInt2 = record.UnknownInt2,
            Unknown = record.Unknown,
            Unknown2 = record.Unknown2,
            DataTypeState = record.DataTypeState
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private sealed class MagicEffectRow : RecordRow
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Flags { get; set; } = string.Empty;
        public string? CastType { get; set; }
        public string? TargetType { get; set; }
        public string? ActorValue2ModKeyName { get; set; }
        public int? ActorValue2ModKeyType { get; set; }
        public string? ActorValue2ModKeyFileName { get; set; }
        public long? ActorValue2FormKeyId { get; set; }
        public string? ResistValueModKeyName { get; set; }
        public int? ResistValueModKeyType { get; set; }
        public string? ResistValueModKeyFileName { get; set; }
        public long? ResistValueFormKeyId { get; set; }
        public string? PerkToApplyModKeyName { get; set; }
        public int? PerkToApplyModKeyType { get; set; }
        public string? PerkToApplyModKeyFileName { get; set; }
        public long? PerkToApplyFormKeyId { get; set; }
        public string? EquipAbilityModKeyName { get; set; }
        public int? EquipAbilityModKeyType { get; set; }
        public string? EquipAbilityModKeyFileName { get; set; }
        public long? EquipAbilityFormKeyId { get; set; }
        public string? ExplosionModKeyName { get; set; }
        public int? ExplosionModKeyType { get; set; }
        public string? ExplosionModKeyFileName { get; set; }
        public long? ExplosionFormKeyId { get; set; }
        public string? CastingArtModKeyName { get; set; }
        public int? CastingArtModKeyType { get; set; }
        public string? CastingArtModKeyFileName { get; set; }
        public long? CastingArtFormKeyId { get; set; }
        public string? HitEffectArtModKeyName { get; set; }
        public int? HitEffectArtModKeyType { get; set; }
        public string? HitEffectArtModKeyFileName { get; set; }
        public long? HitEffectArtFormKeyId { get; set; }
        public string? HitShaderModKeyName { get; set; }
        public int? HitShaderModKeyType { get; set; }
        public string? HitShaderModKeyFileName { get; set; }
        public long? HitShaderFormKeyId { get; set; }
        public string? ImageSpaceModifierModKeyName { get; set; }
        public int? ImageSpaceModifierModKeyType { get; set; }
        public string? ImageSpaceModifierModKeyFileName { get; set; }
        public long? ImageSpaceModifierFormKeyId { get; set; }
        public string? ImpactDataModKeyName { get; set; }
        public int? ImpactDataModKeyType { get; set; }
        public string? ImpactDataModKeyFileName { get; set; }
        public long? ImpactDataFormKeyId { get; set; }
        public string? ProjectileModKeyName { get; set; }
        public int? ProjectileModKeyType { get; set; }
        public string? ProjectileModKeyFileName { get; set; }
        public long? ProjectileFormKeyId { get; set; }
        public string? Archetype { get; set; }
        public float? UnknownFloat3 { get; set; }
        public int? UnknownInt2 { get; set; }
        public string? Unknown { get; set; }
        public string? Unknown2 { get; set; }
        public string? DataTypeState { get; set; }
    }
}
