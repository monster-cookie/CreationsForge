using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Magic Effect record specification metadata.
/// </summary>
internal static class MagicEffectRecordSpecification
{

    /// <summary>
    /// Gets the Magic Effect record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "MGEF",
        RecordType = "MagicEffect",
        TableName = "MagicEffects",
        FriendlyName = "Magic Effect",
        GameSupport = CreateCurrentGameSupport("MagicEffects", "MagicEffects"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "MagicEffects",
            ImportOrder = 9,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("MagicEffects", "MagicEffects"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated magic effect display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Description",
                SpriggitPath = "Description",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated magic effect description."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted magic effect flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "CastType",
                SpriggitPath = "CastType",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Magic effect cast type text."
            },
            new RecordFieldSpecification
            {
                FieldName = "TargetType",
                SpriggitPath = "TargetType",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Magic effect target type text."
            },
            new RecordFieldSpecification
            {
                FieldName = "ActorValue2FormKey",
                SpriggitPath = "ActorValue2FormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional actor value FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "ResistValueFormKey",
                SpriggitPath = "ResistValueFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional resist value FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "PerkToApplyFormKey",
                SpriggitPath = "PerkToApplyFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional perk-to-apply FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "EquipAbilityFormKey",
                SpriggitPath = "EquipAbilityFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional equip ability FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "ExplosionFormKey",
                SpriggitPath = "ExplosionFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional explosion FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "CastingArtFormKey",
                SpriggitPath = "CastingArtFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional casting art FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "HitEffectArtFormKey",
                SpriggitPath = "HitEffectArtFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional hit effect art FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "HitShaderFormKey",
                SpriggitPath = "HitShaderFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional hit shader FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "ImageSpaceModifierFormKey",
                SpriggitPath = "ImageSpaceModifierFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional image space modifier FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "ImpactDataFormKey",
                SpriggitPath = "ImpactDataFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional impact data FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "ProjectileFormKey",
                SpriggitPath = "ProjectileFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional projectile FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Archetype",
                SpriggitPath = "Archetype",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Magic effect archetype text."
            },
            new RecordFieldSpecification
            {
                FieldName = "UnknownFloat3",
                SpriggitPath = "UnknownFloat3",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional source float retained on the magic effect parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "UnknownInt2",
                SpriggitPath = "UnknownInt2",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional source integer retained on the magic effect parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "Unknown",
                SpriggitPath = "Unknown",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional source text retained on the magic effect parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "Unknown2",
                SpriggitPath = "Unknown2",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Second optional source text retained on the magic effect parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "DataTypeState",
                SpriggitPath = "DataTypeState",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Magic effect data type state text."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Name",
                    SourcePath = "Name",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Description",
                    SourcePath = "Description",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Flags",
                    SourcePath = "Flags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CastType",
                    SourcePath = "CastType",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "TargetType",
                    SourcePath = "TargetType",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ActorValue2FormKey",
                    SourcePath = "ActorValue2FormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ResistValueFormKey",
                    SourcePath = "ResistValueFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "PerkToApplyFormKey",
                    SourcePath = "PerkToApplyFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "EquipAbilityFormKey",
                    SourcePath = "EquipAbilityFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ExplosionFormKey",
                    SourcePath = "ExplosionFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CastingArtFormKey",
                    SourcePath = "CastingArtFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "HitEffectArtFormKey",
                    SourcePath = "HitEffectArtFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "HitShaderFormKey",
                    SourcePath = "HitShaderFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ImageSpaceModifierFormKey",
                    SourcePath = "ImageSpaceModifierFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ImpactDataFormKey",
                    SourcePath = "ImpactDataFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ProjectileFormKey",
                    SourcePath = "ProjectileFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Archetype",
                    SourcePath = "Archetype",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "UnknownFloat3",
                    SourcePath = "UnknownFloat3",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "UnknownInt2",
                    SourcePath = "UnknownInt2",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Unknown",
                    SourcePath = "Unknown",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Unknown2",
                    SourcePath = "Unknown2",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "DataTypeState",
                    SourcePath = "DataTypeState",
                    ValueKind = RecordFieldValueKind.Text
                }
            ],
            ChildGroups =
            [
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.KeywordMappings,
                    GroupName = "Keywords",
                    Description = "Shared keyword mapping rows appended after Magic Effect scalar parent rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.SoundMappings,
                    GroupName = "Sounds",
                    Description = "Shared sound mapping rows appended after keyword rows."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows and keyword/sound child-group dispatch; " +
            "script rows remain strategy-based."
    };
}

