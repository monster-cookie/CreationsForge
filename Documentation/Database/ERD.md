# SQLite Entity Relationship Diagram

## Diagram

This diagram includes relationships declared as SQLite foreign keys. Composite keys are shown by marking each
participating column as `PK` or `FK`.

```mermaid
erDiagram
    Plugins {
        TEXT ModKey_Name PK
        INTEGER ModKey_Type PK
        TEXT ModKey_FileName PK
        INTEGER LoadOrderIndex
        INTEGER Enabled
        INTEGER ExistsOnDisk
        TEXT ImportState
        INTEGER HeaderFlags
        INTEGER FormVersion
        TEXT Author
        TEXT Branch
        INTEGER InteriorCellCount
        INTEGER SourceLastWriteUTCTicks
        INTEGER SourceFileSizeBytes
        TEXT LastCheckedUTC
        TEXT LastImportedUTC
        TEXT InvalidatedAtUTC
        INTEGER RecordCount
    }

    PluginMasterReferences {
        TEXT Master_ModKey_Name PK, FK
        INTEGER Master_ModKey_Type PK, FK
        TEXT Master_ModKey_FileName PK, FK
        TEXT Plugin_ModKey_Name PK, FK
        INTEGER Plugin_ModKey_Type PK, FK
        TEXT Plugin_ModKey_FileName PK, FK
        TEXT ImportedAtUTC
    }

    FormList {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER StarfieldMajorRecordFlags
        INTEGER Version2
        INTEGER VersionControl
        TEXT ImportedAtUTC
        TEXT AddToListFormKey
    }

    FormListItems {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT Item_ModKey_Name PK
        INTEGER Item_ModKey_Type PK
        TEXT Item_ModKey_FileName PK
        INTEGER Item_FormKey_ID PK
        INTEGER Item_Index PK
        TEXT ImportedAtUTC
    }

    GameSetting {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER StarfieldMajorRecordFlags
        INTEGER Version2
        INTEGER VersionControl
        TEXT ImportedAtUTC
        TEXT SettingType
        TEXT Data
        REAL RawData
        INTEGER XALG
        INTEGER IsCompressed
        INTEGER IsDeleted
    }

    Global {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER StarfieldMajorRecordFlags
        INTEGER Version2
        INTEGER VersionControl
        TEXT ImportedAtUTC
        REAL Data
    }

    MiscItem {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER StarfieldMajorRecordFlags
        INTEGER Version2
        INTEGER VersionControl
        TEXT ImportedAtUTC
        TEXT Name
        TEXT ShortName
        INTEGER Value
        REAL Weight
        REAL DirtinessScale
        TEXT FeaturedItemMessage_ModKey_Name
        INTEGER FeaturedItemMessage_ModKey_Type
        TEXT FeaturedItemMessage_ModKey_FileName
        INTEGER FeaturedItemMessage_FormKey_ID
        TEXT FLAG
    }

    MiscItemObjectBounds {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        REAL First_X
        REAL First_Y
        REAL First_Z
        REAL Second_X
        REAL Second_Y
        REAL Second_Z
        TEXT ImportedAtUTC
    }

    MiscItemObjectPaletteDefaults {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT Flags
        REAL SinkMeters
        REAL SinkVariance
        REAL XYOffsetVariance
        TEXT FootprintSize
        REAL ScalePercent
        REAL ScaleVariance
        REAL AngleXDegrees
        REAL AngleXVariance
        REAL AngleYDegrees
        REAL AngleYVariance
        REAL AngleZDegrees
        REAL AngleZVariance
        REAL SlopePercent
        REAL SlopePercentVariance
        REAL Density
        REAL FrequencyPercent
        REAL SlopeLimit
        REAL DistanceBelowWater
        REAL DistanceAboveWater
        TEXT ImportedAtUTC
    }

    MiscItemTransforms {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT InventoryIcon_FormKey
        TEXT Outpost_FormKey
        TEXT Ship_FormKey
        TEXT Preview_FormKey
        TEXT Inventory_FormKey
        TEXT Workbench_FormKey
        TEXT MainGameUI_FormKey
        TEXT ImportedAtUTC
    }

    MiscItemModels {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT File
        TEXT TextureFileHashes
        INTEGER LightLayer
        TEXT Flags
        REAL ColorRemappingIndex
        TEXT FlagsVestigial
        TEXT ImportedAtUTC
    }

    MiscItemModelMaterialSwaps {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT MaterialSwap_FormKey
        INTEGER MaterialSwap_Index PK
        TEXT ImportedAtUTC
    }

    MiscItemSounds {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT SoundType PK
        TEXT Start
        TEXT Stop
        TEXT Condition_FormKey
        TEXT EventMapping_FormKey
        TEXT ImportedAtUTC
    }

    MiscItemKeywords {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT Keyword_FormKey
        INTEGER Keyword_Index PK
        TEXT ImportedAtUTC
    }

    MiscItemDestructibles {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Health
        INTEGER StageCount
        TEXT Flags
        TEXT ImportedAtUTC
    }

    MiscItemDestructibleResistances {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT DamageType_FormKey
        INTEGER Value
        INTEGER Resistance_Index PK
        TEXT ImportedAtUTC
    }

    MiscItemDestructionStages {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Stage_Index PK
        INTEGER HealthPercent
        INTEGER SourceIndex
        INTEGER ModelDamageStage
        TEXT Flags
        INTEGER SelfDamagePerSecond
        TEXT Explosion_FormKey
        TEXT Debris_FormKey
        INTEGER DebrisCount
        TEXT SequenceName
        TEXT Model_File
        INTEGER Model_LightLayer
        TEXT Model_Flags
        TEXT ImportedAtUTC
    }

    MiscItemDestructionStageMaterialSwaps {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Stage_Index PK, FK
        TEXT MaterialSwap_FormKey
        INTEGER MaterialSwap_Index PK
        TEXT ImportedAtUTC
    }

    Keyword {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER StarfieldMajorRecordFlags
        INTEGER Version2
        INTEGER VersionControl
        TEXT ImportedAtUTC
        TEXT Name
        TEXT Color
        TEXT Type
        TEXT Notes
        TEXT FlashLinkageName
        TEXT AttractionRuleFormKey
    }

    NPC {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER StarfieldMajorRecordFlags
        INTEGER Version2
        INTEGER VersionControl
        TEXT ImportedAtUTC
        TEXT Name
        TEXT ShortName
        TEXT LongName
        INTEGER DispositionBase
        TEXT Aggression
        TEXT Confidence
        INTEGER EnergyLevel
        TEXT Responsibility
        TEXT Assistance
        INTEGER GearedUpWeapons
        REAL HeightMin
        REAL HeightMax
        INTEGER SkinToneIndex
        TEXT Pronoun
        TEXT VoiceFormKey
        TEXT RaceFormKey
        TEXT CombatOverridePackageListFormKey
        TEXT CombatStyleFormKey
        TEXT DefaultPackageListFormKey
        TEXT CrimeFactionFormKey
    }

    ActorValueInformation {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER StarfieldMajorRecordFlags
        INTEGER Version2
        INTEGER VersionControl
        TEXT ImportedAtUTC
        TEXT Name
        TEXT Abbreviation
        TEXT ContextNotes
        REAL DefaultValue
        TEXT Flags
        TEXT Type
        REAL Min
        REAL Max
    }

    MagicEffect {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER StarfieldMajorRecordFlags
        INTEGER Version2
        INTEGER VersionControl
        TEXT ImportedAtUTC
        TEXT Name
        TEXT Description
        TEXT Flags
        TEXT CastType
        TEXT TargetType
        TEXT ActorValue2FormKey
        TEXT ResistValueFormKey
        TEXT PerkToApplyFormKey
        TEXT EquipAbilityFormKey
        TEXT ExplosionFormKey
        TEXT CastingArtFormKey
        TEXT HitEffectArtFormKey
        TEXT HitShaderFormKey
        TEXT ImageSpaceModifierFormKey
        TEXT ImpactDataFormKey
        TEXT ProjectileFormKey
    }

    Perk {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER StarfieldMajorRecordFlags
        INTEGER Version2
        INTEGER VersionControl
        TEXT ImportedAtUTC
        TEXT Name
        TEXT Description
        TEXT Flags
        TEXT SkillGroup
        TEXT CrewAssignment
        TEXT PerkIcon
        TEXT Category
        TEXT Restriction_ModKey_Name
        INTEGER Restriction_ModKey_Type
        TEXT Restriction_ModKey_FileName
        INTEGER Restriction_FormKey_ID
        TEXT Training_ModKey_Name
        INTEGER Training_ModKey_Type
        TEXT Training_ModKey_FileName
        INTEGER Training_FormKey_ID
        TEXT MajorFlags
    }

    PerkRanks {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Rank_Index PK
        TEXT Description
        TEXT UnknownStatic_ModKey_Name
        INTEGER UnknownStatic_ModKey_Type
        TEXT UnknownStatic_ModKey_FileName
        INTEGER UnknownStatic_FormKey_ID
        INTEGER ConditionCount
        INTEGER ActivityCount
        TEXT ImportedAtUTC
    }

    PerkRankEffects {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Rank_Index PK, FK
        INTEGER Effect_Index PK
        TEXT MutagenObjectType
        INTEGER Rank
        INTEGER Priority
        INTEGER PerkEntryID
        TEXT Flags
        TEXT ButtonLabel
        INTEGER ConditionCount
        TEXT EntryPoint
        INTEGER PerkConditionTabCount
        TEXT Modification
        REAL Value
        TEXT ImportedAtUTC
    }

    PerkBackgroundSkills {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT Skill_ModKey_Name
        INTEGER Skill_ModKey_Type
        TEXT Skill_ModKey_FileName
        INTEGER Skill_FormKey_ID
        INTEGER Skill_Index PK
        TEXT ImportedAtUTC
    }

    ScriptingAdapters {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT Name PK
        INTEGER Script_Index
        TEXT ImportedAtUTC
    }

    ScriptingAdapterProperties {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT ScriptingAdapter_Name PK, FK
        INTEGER Property_Index PK
        TEXT Name
        TEXT MutagenObjectType
        INTEGER Data_Bool
        INTEGER Data_Int
        REAL Data_Float
        TEXT Data_String
        TEXT Object_ModKey_Name
        INTEGER Object_ModKey_Type
        TEXT Object_ModKey_FileName
        INTEGER Object_FormKey_ID
        INTEGER Object_Alias
        INTEGER Object_Unused
        TEXT ImportedAtUTC
    }

    ScriptingAdapterPropertyListItems {
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT ScriptingAdapter_Name PK, FK
        INTEGER Property_Index PK, FK
        INTEGER ListItem_Index PK
        TEXT MutagenObjectType
        INTEGER Data_Bool
        INTEGER Data_Int
        REAL Data_Float
        TEXT Data_String
        TEXT Object_ModKey_Name
        INTEGER Object_ModKey_Type
        TEXT Object_ModKey_FileName
        INTEGER Object_FormKey_ID
        INTEGER Object_Alias
        INTEGER Object_Unused
        TEXT ImportedAtUTC
    }

    Plugins ||--o{ PluginMasterReferences : "is declared master"
    Plugins ||--o{ PluginMasterReferences : "declares masters"
    Plugins ||--o{ FormList : contains
    FormList ||--o{ FormListItems : contains
    Plugins ||--o{ GameSetting : contains
    Plugins ||--o{ Global : contains
    Plugins ||--o{ MiscItem : contains
    MiscItem ||--o| MiscItemObjectBounds : contains
    MiscItem ||--o| MiscItemObjectPaletteDefaults : contains
    MiscItem ||--o| MiscItemTransforms : contains
    MiscItem ||--o| MiscItemModels : contains
    MiscItemModels ||--o{ MiscItemModelMaterialSwaps : contains
    MiscItem ||--o{ MiscItemSounds : contains
    MiscItem ||--o{ MiscItemKeywords : contains
    MiscItem ||--o| MiscItemDestructibles : contains
    MiscItemDestructibles ||--o{ MiscItemDestructibleResistances : contains
    MiscItemDestructibles ||--o{ MiscItemDestructionStages : contains
    MiscItemDestructionStages ||--o{ MiscItemDestructionStageMaterialSwaps : contains
    Plugins ||--o{ Keyword : contains
    Plugins ||--o{ NPC : contains
    Plugins ||--o{ ActorValueInformation : contains
    Plugins ||--o{ MagicEffect : contains
    Plugins ||--o{ Perk : contains
    Perk ||--o{ PerkRanks : contains
    PerkRanks ||--o{ PerkRankEffects : contains
    Perk ||--o{ PerkBackgroundSkills : contains
    Plugins ||--o{ ScriptingAdapters : contains
    ScriptingAdapters ||--o{ ScriptingAdapterProperties : contains
    ScriptingAdapterProperties ||--o{ ScriptingAdapterPropertyListItems : contains
```

## Important Indexes

The schema has no separately declared unique indexes. Composite primary keys provide row uniqueness.

- `Plugins`: indexes on `LoadOrderIndex`, `ImportState`, and the source fingerprint columns.
- `PluginMasterReferences`: indexes on the declared-master key and declaring-plugin key.
- `FormListItems`: indexes on the owning form-list key, the item `FormKey` columns, and `Item_Index`.
- Each typed record table: a non-unique index on the full origin `FormKey` for cross-plugin comparison lookup.
- `PerkRanks`: indexes on the owning Perk key columns and `Rank_Index`.
- `PerkRankEffects`: indexes on the owning Perk rank key columns and `Effect_Index`.
- `PerkBackgroundSkills`: indexes on the owning Perk key columns, referenced skill `FormKey` columns, and
  `Skill_Index`.
- `ScriptingAdapters`: indexes on `RecordType` plus the origin `FormKey` columns, and on `Script_Index`.
- `ScriptingAdapterProperties`: indexes on `RecordType` plus the origin `FormKey` columns, `Property_Index`, and the
  object `FormKey` columns.
- `ScriptingAdapterPropertyListItems`: indexes on `RecordType` plus the origin `FormKey` columns, `ListItem_Index`,
  and the object `FormKey` columns.

## Important Constraints

- Every declared foreign key uses `ON DELETE CASCADE`.
- `Plugins.Enabled` and `Plugins.ExistsOnDisk` must be `0` or `1`.
- `Plugins.ImportState` must be `Current`, `Changed`, `Missing`, `Failed`, or `Unsupported`.
- `Plugins.RecordCount`, every typed-record `FormKey_ID`, and `FormListItems.Item_FormKey_ID` must be non-negative.
- `GameSetting.IsCompressed` and `GameSetting.IsDeleted` must be `0` or `1`.
- `PerkRanks.Rank_Index`, `ConditionCount`, and `ActivityCount` must be non-negative.
- `PerkRankEffects.Rank_Index`, `Effect_Index`, `Rank`, `Priority`, and `ConditionCount` must be non-negative.
- `PerkRankEffects.PerkEntryID` and `PerkConditionTabCount` must be `NULL` or non-negative.
- `PerkBackgroundSkills.Skill_FormKey_ID` and `Skill_Index` must be non-negative.

## Inferred Relationships

These columns contain record-reference data but are not declared SQLite foreign keys. They are intentionally omitted
from the Mermaid relationship lines:

- `FormList.AddToListFormKey`
- `FormListItems.Item_ModKey_Name`, `Item_ModKey_Type`, `Item_ModKey_FileName`, and `Item_FormKey_ID`
- `Keyword.AttractionRuleFormKey`
- `NPC.VoiceFormKey`, `RaceFormKey`, `CombatOverridePackageListFormKey`, `CombatStyleFormKey`,
  `DefaultPackageListFormKey`, and `CrimeFactionFormKey`
- `MagicEffect.ActorValue2FormKey`, `ResistValueFormKey`, `PerkToApplyFormKey`, `EquipAbilityFormKey`,
  `ExplosionFormKey`, `CastingArtFormKey`, `HitEffectArtFormKey`, `HitShaderFormKey`, `ImageSpaceModifierFormKey`,
  `ImpactDataFormKey`, and `ProjectileFormKey`
- `Perk.Restriction_ModKey_Name`, `Restriction_ModKey_Type`, `Restriction_ModKey_FileName`, and
  `Restriction_FormKey_ID`
- `Perk.Training_ModKey_Name`, `Training_ModKey_Type`, `Training_ModKey_FileName`, and `Training_FormKey_ID`
- `PerkRanks.UnknownStatic_ModKey_Name`, `UnknownStatic_ModKey_Type`, `UnknownStatic_ModKey_FileName`, and
  `UnknownStatic_FormKey_ID`
- `PerkBackgroundSkills.Skill_ModKey_Name`, `Skill_ModKey_Type`, `Skill_ModKey_FileName`, and `Skill_FormKey_ID`
- Typed-record and VMAD `FormKey_ModKey_Name`, `FormKey_ModKey_Type`, `FormKey_ModKey_FileName`, and `FormKey_ID`
  persist origin `FormKey` identity but are not declared SQLite foreign keys to `Plugins`
- `ScriptingAdapters.RecordType` and the origin `FormKey` columns
- `ScriptingAdapterProperties.Object_ModKey_Name`, `Object_ModKey_Type`, `Object_ModKey_FileName`,
  and `Object_FormKey_ID`
- `ScriptingAdapterPropertyListItems.Object_ModKey_Name`, `Object_ModKey_Type`, `Object_ModKey_FileName`,
  and `Object_FormKey_ID`

