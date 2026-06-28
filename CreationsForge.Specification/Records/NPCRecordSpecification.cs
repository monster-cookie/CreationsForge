using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the NPC record specification metadata.
/// </summary>
internal static class NPCRecordSpecification
{

    /// <summary>
    /// Gets the NPC record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "NPC_",
        RecordType = "NPC",
        TableName = "NPCs",
        FriendlyName = "NPC",
        GameSupport = CreateCurrentGameSupport("NPCs", "NPCs"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "NPCs",
            ImportOrder = 8,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("NPCs", "NPCs"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "IsCompressed",
                SpriggitPath = "IsCompressed",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional compressed-record flag reported by Spriggit."
            },
            new RecordFieldSpecification
            {
                FieldName = "ObjectBoundsFirst",
                SpriggitPath = "ObjectBounds.First",
                ValueKind = RecordFieldValueKind.Text,
                Description = "First persisted object-bounds vector text."
            },
            new RecordFieldSpecification
            {
                FieldName = "ObjectBoundsSecond",
                SpriggitPath = "ObjectBounds.Second",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Second persisted object-bounds vector text."
            },
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated NPC display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "ShortName",
                SpriggitPath = "ShortName",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated short display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "LongName",
                SpriggitPath = "LongName",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated long display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted NPC flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "MajorFlags",
                SpriggitPath = "MajorFlags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted NPC major flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "Level",
                SpriggitPath = "Level",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Nested level data expanded by strategy-based comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "Configuration",
                SpriggitPath = "Configuration",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Nested configuration data expanded by strategy-based comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "DispositionBase",
                SpriggitPath = "DispositionBase",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Base disposition value."
            },
            new RecordFieldSpecification
            {
                FieldName = "Aggression",
                SpriggitPath = "Aggression",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Actor aggression setting."
            },
            new RecordFieldSpecification
            {
                FieldName = "Confidence",
                SpriggitPath = "Confidence",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Actor confidence setting."
            },
            new RecordFieldSpecification
            {
                FieldName = "EnergyLevel",
                SpriggitPath = "EnergyLevel",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Actor energy level."
            },
            new RecordFieldSpecification
            {
                FieldName = "Responsibility",
                SpriggitPath = "Responsibility",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Actor responsibility setting."
            },
            new RecordFieldSpecification
            {
                FieldName = "Assistance",
                SpriggitPath = "Assistance",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Actor assistance setting."
            },
            new RecordFieldSpecification
            {
                FieldName = "Mood",
                SpriggitPath = "Mood",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional actor mood text."
            },
            new RecordFieldSpecification
            {
                FieldName = "GearedUpWeapons",
                SpriggitPath = "GearedUpWeapons",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Geared-up weapons count from player skills data."
            },
            new RecordFieldSpecification
            {
                FieldName = "HeightMin",
                SpriggitPath = "HeightMin",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Minimum actor height, displayed with the DTO's numeric precision metadata."
            },
            new RecordFieldSpecification
            {
                FieldName = "HeightMax",
                SpriggitPath = "HeightMax",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Maximum actor height, displayed with the DTO's numeric precision metadata."
            },
            new RecordFieldSpecification
            {
                FieldName = "Height",
                SpriggitPath = "Height",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Scalar Skyrim actor height, displayed with the DTO's numeric precision metadata."
            },
            new RecordFieldSpecification
            {
                FieldName = "SkinToneIndex",
                SpriggitPath = "SkinToneIndex",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional skin tone index."
            },
            new RecordFieldSpecification
            {
                FieldName = "Skin",
                SpriggitPath = "Skin",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional Fallout 4 skin FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Pronoun",
                SpriggitPath = "Pronoun",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional pronoun text."
            },
            new RecordFieldSpecification
            {
                FieldName = "VoiceFormKey",
                SpriggitPath = "VoiceFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional voice type FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "RaceFormKey",
                SpriggitPath = "RaceFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional race FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "AttackRace",
                SpriggitPath = "AttackRace",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional attack race FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "CombatOverridePackageListFormKey",
                SpriggitPath = "CombatOverridePackageListFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional combat override package-list FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "CombatStyleFormKey",
                SpriggitPath = "CombatStyleFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional combat style FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "DefaultPackageListFormKey",
                SpriggitPath = "DefaultPackageListFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional default package-list FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "CrimeFactionFormKey",
                SpriggitPath = "CrimeFactionFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional crime faction FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Packages",
                SpriggitPath = "Packages",
                ValueKind = RecordFieldValueKind.Collection,
                IsCollection = true,
                Description = "Package FormKeys expanded by strategy-based comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "ForcedLocations",
                SpriggitPath = "ForcedLocations",
                ValueKind = RecordFieldValueKind.Collection,
                IsCollection = true,
                Description = "Forced location FormKeys expanded by strategy-based comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "HeadParts",
                SpriggitPath = "HeadParts",
                ValueKind = RecordFieldValueKind.Collection,
                IsCollection = true,
                Description = "Head part FormKeys expanded by strategy-based comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "ActorEffects",
                SpriggitPath = "ActorEffect",
                ValueKind = RecordFieldValueKind.Collection,
                IsCollection = true,
                Description = "Actor effect FormKeys expanded by strategy-based comparison rows."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "IsCompressed",
                    SourcePath = "IsCompressed",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBoundsFirst",
                    SourcePath = "ObjectBoundsFirst",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBoundsSecond",
                    SourcePath = "ObjectBoundsSecond",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Name",
                    SourcePath = "Name",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ShortName",
                    SourcePath = "ShortName",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "LongName",
                    SourcePath = "LongName",
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
                    FieldName = "MajorFlags",
                    SourcePath = "MajorFlags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "DispositionBase",
                    SourcePath = "DispositionBase",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Aggression",
                    SourcePath = "Aggression",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Confidence",
                    SourcePath = "Confidence",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "EnergyLevel",
                    SourcePath = "EnergyLevel",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Responsibility",
                    SourcePath = "Responsibility",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Assistance",
                    SourcePath = "Assistance",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Mood",
                    SourcePath = "Mood",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "GearedUpWeapons",
                    SourcePath = "GearedUpWeapons",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "HeightMin",
                    SourcePath = "HeightMin",
                    ValueKind = RecordFieldValueKind.Number,
                    Description = "Displayed through the existing numeric precision hook."
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "HeightMax",
                    SourcePath = "HeightMax",
                    ValueKind = RecordFieldValueKind.Number,
                    Description = "Displayed through the existing numeric precision hook."
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Height",
                    SourcePath = "Height",
                    ValueKind = RecordFieldValueKind.Number,
                    Description = "Displayed through the existing numeric precision hook."
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "SkinToneIndex",
                    SourcePath = "SkinToneIndex",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Skin",
                    SourcePath = "Skin",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Pronoun",
                    SourcePath = "Pronoun",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VoiceFormKey",
                    SourcePath = "VoiceFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "RaceFormKey",
                    SourcePath = "RaceFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AttackRace",
                    SourcePath = "AttackRace",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CombatOverridePackageListFormKey",
                    SourcePath = "CombatOverridePackageListFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CombatStyleFormKey",
                    SourcePath = "CombatStyleFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "DefaultPackageListFormKey",
                    SourcePath = "DefaultPackageListFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrimeFactionFormKey",
                    SourcePath = "CrimeFactionFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                }
            ],
            ChildGroups =
            [
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCLevel,
                    GroupName = "Level",
                    Description = "Nested NPC level data appended after pre-level scalar parent rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCConfiguration,
                    GroupName = "Configuration",
                    Description = "Nested NPC configuration data appended after level rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCSupplementalFields,
                    GroupName = "Supplemental Fields",
                    Description = "Supplemental NPC parent, weight, face, and template rows appended after core scalar rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCPackages,
                    GroupName = "Packages",
                    Description = "NPC package FormKey rows appended after supplemental rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCForcedLocations,
                    GroupName = "ForcedLocations",
                    Description = "NPC forced-location FormKey rows appended after package rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCHeadParts,
                    GroupName = "HeadParts",
                    Description = "NPC head-part FormKey rows appended after forced-location rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCActorEffects,
                    GroupName = "ActorEffects",
                    Description = "NPC actor-effect FormKey rows appended after head-part rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCFactions,
                    GroupName = "Factions",
                    Description = "NPC faction membership rows appended after actor-effect rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCProperties,
                    GroupName = "Properties",
                    Description = "NPC actor-value property rows appended after faction rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCItems,
                    GroupName = "Items",
                    Description = "NPC inventory item rows appended after property rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCPerks,
                    GroupName = "Perks",
                    Description = "NPC perk rows appended after item rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCMorphs,
                    GroupName = "Morphs",
                    Description = "NPC morph rows appended after perk rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCFaceMorphs,
                    GroupName = "FaceMorphs",
                    Description = "NPC face morph position rows appended after morph rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCFaceDialPositions,
                    GroupName = "FaceDialPositions",
                    Description = "NPC face dial position rows appended after face morph position rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCFaceMorphGroups,
                    GroupName = "FaceMorphGroups",
                    Description = "NPC nested face morph group rows appended after face dial position rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCMorphBlends,
                    GroupName = "MorphBlends",
                    Description = "NPC morph blend rows appended after face morph group rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCTints,
                    GroupName = "Tints",
                    Description = "NPC tint rows appended after morph blend rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCTintLayers,
                    GroupName = "TintLayers",
                    Description = "NPC tint layer rows appended after tint rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCFaceTintingLayers,
                    GroupName = "FaceTintingLayers",
                    Description = "NPC face tinting layer rows appended after tint layer rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.NPCPlayerSkills,
                    GroupName = "PlayerSkills",
                    Description = "NPC player skill rows appended after face tinting layer rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.KeywordMappings,
                    GroupName = "Keywords",
                    Description = "Shared keyword mapping rows appended after scalar parent and record-specific rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.SoundMappings,
                    GroupName = "Sounds",
                    Description = "Shared sound mapping rows appended after keyword rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ScriptingAdapterMappings,
                    GroupName = "Scripts",
                    Description = "Shared scripting adapter rows appended after sound rows."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives top-level scalar parent rows and keyword/sound/script " +
            "child-group dispatch plus NPC level, configuration, supplemental, form-key list, and actor data row " +
            "dispatch."
    };
}


