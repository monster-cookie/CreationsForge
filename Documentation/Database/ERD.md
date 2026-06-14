# SQLite Entity Relationship Diagram

## Diagram

This diagram includes only relationships declared as SQLite foreign keys. Composite keys are shown by marking each
participating column as `PK` or `FK`.

```mermaid
erDiagram
    Games {
        TEXT Game PK
        TEXT DisplayName
        TEXT InstallationFolder
        TEXT DataFolder
        TEXT ImportedAtUTC
    }

    Plugins {
        TEXT Game PK, FK
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
        TEXT Description
        INTEGER RecordCount
        INTEGER SourceLastWriteUTCTicks
        INTEGER SourceFileSizeBytes
        TEXT LastCheckedUTC
        TEXT LastImportedUTC
        TEXT InvalidatedAtUTC
    }

    StarfieldPlugins {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT Branch
        INTEGER InteriorCellCount
        INTEGER Intv
    }

    Fallout4Plugins {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER Incc
    }

    SkyrimPlugins {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        INTEGER Incc
        INTEGER Intv
    }

    PluginMasterReferences {
        TEXT Game PK, FK
        TEXT Master_ModKey_Name PK, FK
        INTEGER Master_ModKey_Type PK, FK
        TEXT Master_ModKey_FileName PK, FK
        TEXT Plugin_ModKey_Name PK, FK
        INTEGER Plugin_ModKey_Type PK, FK
        TEXT Plugin_ModKey_FileName PK, FK
        TEXT ImportedAtUTC
    }

    RecordInstances {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK
        TEXT FormKey_ModKey_Name PK
        INTEGER FormKey_ModKey_Type PK
        TEXT FormKey_ModKey_FileName PK
        INTEGER FormKey_ID PK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
        TEXT ImportedAtUTC
    }

    FormLists {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
        TEXT ImportedAtUTC
        TEXT AddToList_ModKey_Name
        INTEGER AddToList_ModKey_Type
        TEXT AddToList_ModKey_FileName
        INTEGER AddToList_FormKey_ID
    }

    FormListItems {
        TEXT Game PK, FK
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

    GameSettings {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
        TEXT ImportedAtUTC
        TEXT SettingType
        TEXT Data
        REAL NumericData
        INTEGER IntegerData
        INTEGER BooleanData
    }

    Globals {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
        TEXT ImportedAtUTC
        REAL Data
    }

    MiscObjects {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
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

    Keywords {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
        TEXT ImportedAtUTC
        TEXT Name
        TEXT Color
        TEXT Type
        TEXT Notes
        TEXT FlashLinkageName
        TEXT AttractionRule_ModKey_Name
        INTEGER AttractionRule_ModKey_Type
        TEXT AttractionRule_ModKey_FileName
        INTEGER AttractionRule_FormKey_ID
    }

    ActorValueInformation {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
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

    NPCs {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
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
        TEXT Voice_ModKey_Name
        INTEGER Voice_ModKey_Type
        TEXT Voice_ModKey_FileName
        INTEGER Voice_FormKey_ID
        TEXT Race_ModKey_Name
        INTEGER Race_ModKey_Type
        TEXT Race_ModKey_FileName
        INTEGER Race_FormKey_ID
        TEXT CombatOverridePackageList_ModKey_Name
        INTEGER CombatOverridePackageList_ModKey_Type
        TEXT CombatOverridePackageList_ModKey_FileName
        INTEGER CombatOverridePackageList_FormKey_ID
        TEXT CombatStyle_ModKey_Name
        INTEGER CombatStyle_ModKey_Type
        TEXT CombatStyle_ModKey_FileName
        INTEGER CombatStyle_FormKey_ID
        TEXT DefaultPackageList_ModKey_Name
        INTEGER DefaultPackageList_ModKey_Type
        TEXT DefaultPackageList_ModKey_FileName
        INTEGER DefaultPackageList_FormKey_ID
        TEXT CrimeFaction_ModKey_Name
        INTEGER CrimeFaction_ModKey_Type
        TEXT CrimeFaction_ModKey_FileName
        INTEGER CrimeFaction_FormKey_ID
    }

    MagicEffects {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
        TEXT ImportedAtUTC
        TEXT Name
        TEXT Description
        TEXT Flags
        TEXT CastType
        TEXT TargetType
        TEXT ActorValue2_ModKey_Name
        INTEGER ActorValue2_ModKey_Type
        TEXT ActorValue2_ModKey_FileName
        INTEGER ActorValue2_FormKey_ID
        TEXT ResistValue_ModKey_Name
        INTEGER ResistValue_ModKey_Type
        TEXT ResistValue_ModKey_FileName
        INTEGER ResistValue_FormKey_ID
        TEXT PerkToApply_ModKey_Name
        INTEGER PerkToApply_ModKey_Type
        TEXT PerkToApply_ModKey_FileName
        INTEGER PerkToApply_FormKey_ID
        TEXT EquipAbility_ModKey_Name
        INTEGER EquipAbility_ModKey_Type
        TEXT EquipAbility_ModKey_FileName
        INTEGER EquipAbility_FormKey_ID
        TEXT Explosion_ModKey_Name
        INTEGER Explosion_ModKey_Type
        TEXT Explosion_ModKey_FileName
        INTEGER Explosion_FormKey_ID
        TEXT CastingArt_ModKey_Name
        INTEGER CastingArt_ModKey_Type
        TEXT CastingArt_ModKey_FileName
        INTEGER CastingArt_FormKey_ID
        TEXT HitEffectArt_ModKey_Name
        INTEGER HitEffectArt_ModKey_Type
        TEXT HitEffectArt_ModKey_FileName
        INTEGER HitEffectArt_FormKey_ID
        TEXT HitShader_ModKey_Name
        INTEGER HitShader_ModKey_Type
        TEXT HitShader_ModKey_FileName
        INTEGER HitShader_FormKey_ID
        TEXT ImageSpaceModifier_ModKey_Name
        INTEGER ImageSpaceModifier_ModKey_Type
        TEXT ImageSpaceModifier_ModKey_FileName
        INTEGER ImageSpaceModifier_FormKey_ID
        TEXT ImpactData_ModKey_Name
        INTEGER ImpactData_ModKey_Type
        TEXT ImpactData_ModKey_FileName
        INTEGER ImpactData_FormKey_ID
        TEXT Projectile_ModKey_Name
        INTEGER Projectile_ModKey_Type
        TEXT Projectile_ModKey_FileName
        INTEGER Projectile_FormKey_ID
        TEXT Archetype
        REAL UnknownFloat3
        INTEGER UnknownInt2
        TEXT Unknown
        TEXT Unknown2
        TEXT DataTypeState
        TEXT ImportedAtUTC
    }

    Perks {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
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

    Statics {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
        TEXT ImportedAtUTC
        INTEGER Version2
        TEXT ObjectBounds_First
        TEXT ObjectBounds_Second
        REAL MaxAngle
        REAL UnknownDNAMFloat
        REAL LeafAmplitude
        REAL LeafFrequency
        TEXT Unused
        TEXT DNAMDataTypeState
    }

    Containers {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT EditorID
        INTEGER FormVersion
        INTEGER MajorRecordFlags
        TEXT ImportedAtUTC
        INTEGER Version2
        TEXT ObjectBounds_First
        TEXT ObjectBounds_Second
        TEXT Name
        TEXT Flags
        TEXT MajorFlags
        TEXT NativeTerminal_ModKey_Name
        INTEGER NativeTerminal_ModKey_Type
        TEXT NativeTerminal_ModKey_FileName
        INTEGER NativeTerminal_FormKey_ID
    }

    ContainerItems {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Item_Index PK
        TEXT Item_ModKey_Name
        INTEGER Item_ModKey_Type
        TEXT Item_ModKey_FileName
        INTEGER Item_FormKey_ID
        INTEGER Count
        TEXT ImportedAtUTC
    }

    RecordKeywords {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT Keyword_ModKey_Name
        INTEGER Keyword_ModKey_Type
        TEXT Keyword_ModKey_FileName
        INTEGER Keyword_FormKey_ID
        INTEGER Keyword_Index PK
        TEXT ImportedAtUTC
    }

    PerkRanks {
        TEXT Game PK, FK
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
        TEXT Game PK, FK
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
        TEXT Game PK, FK
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

    Models {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT ModelSlot PK
        TEXT ModelGender PK
        TEXT File
        TEXT TextureFileHashes
        INTEGER LightLayer
        TEXT Flags
        REAL ColorRemappingIndex
        TEXT FlagsVestigial
        TEXT ImportedAtUTC
    }

    ModelMaterialSwaps {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT ModelSlot PK, FK
        TEXT ModelGender PK, FK
        TEXT MaterialSwap_ModKey_Name
        INTEGER MaterialSwap_ModKey_Type
        TEXT MaterialSwap_ModKey_FileName
        INTEGER MaterialSwap_FormKey_ID
        INTEGER MaterialSwap_Index PK
        TEXT ImportedAtUTC
    }

    RecordSounds {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT SoundSlot PK
        INTEGER Sound_Index PK
        TEXT Start
        TEXT Versioning
        TEXT Unknown
        TEXT ImportedAtUTC
    }

    RawRecordPayloads {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT PayloadSlot PK
        INTEGER Payload_Index PK
        TEXT PayloadType
        TEXT PayloadValue
        TEXT ImportedAtUTC
    }

    ScriptingAdapters {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT Name PK
        INTEGER Script_Index
        TEXT ImportedAtUTC
    }

    ScriptingAdapterProperties {
        TEXT Game PK, FK
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
        TEXT Game PK, FK
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

    AssetArchiveFiles {
        TEXT Game PK
        TEXT DataFolder
        TEXT ArchivePath PK
        TEXT ArchiveFileName
        TEXT ArchiveExtension
        TEXT ArchiveType
        INTEGER SourceLastWriteUTCTicks
        INTEGER SourceFileSizeBytes
        TEXT IndexedAtUTC
    }

    AssetArchiveEntries {
        TEXT Game PK, FK
        TEXT ArchivePath PK, FK
        TEXT NormalizedEntryPath PK
        TEXT RootFolder
        TEXT Extension
        INTEGER PackedSize
        INTEGER UnpackedSize
    }

    Games ||--o{ Plugins : contains
    Plugins ||--o| StarfieldPlugins : extends
    Plugins ||--o| Fallout4Plugins : extends
    Plugins ||--o| SkyrimPlugins : extends
    Plugins ||--o{ PluginMasterReferences : "is declared master"
    Plugins ||--o{ PluginMasterReferences : "declares masters"
    Plugins ||--o{ RecordInstances : contains
    RecordInstances ||--o| FormLists : "typed detail"
    FormLists ||--o{ FormListItems : contains
    RecordInstances ||--o| GameSettings : "typed detail"
    RecordInstances ||--o| Globals : "typed detail"
    RecordInstances ||--o| MiscObjects : "typed detail"
    RecordInstances ||--o| Keywords : "typed detail"
    RecordInstances ||--o| ActorValueInformation : "typed detail"
    RecordInstances ||--o| NPCs : "typed detail"
    RecordInstances ||--o| MagicEffects : "typed detail"
    RecordInstances ||--o| Perks : "typed detail"
    RecordInstances ||--o| Statics : "typed detail"
    RecordInstances ||--o| Containers : "typed detail"
    Containers ||--o{ ContainerItems : contains
    RecordInstances ||--o{ RecordKeywords : contains
    Perks ||--o{ PerkRanks : contains
    PerkRanks ||--o{ PerkRankEffects : contains
    Perks ||--o{ PerkBackgroundSkills : contains
    RecordInstances ||--o{ Models : contains
    Models ||--o{ ModelMaterialSwaps : contains
    RecordInstances ||--o{ RecordSounds : contains
    RecordInstances ||--o{ RawRecordPayloads : contains
    RecordInstances ||--o{ ScriptingAdapters : contains
    ScriptingAdapters ||--o{ ScriptingAdapterProperties : contains
    ScriptingAdapterProperties ||--o{ ScriptingAdapterPropertyListItems : contains
    AssetArchiveFiles ||--o{ AssetArchiveEntries : contains
```

## Index Notes

Indexes are documented in `DATABASE.md`. Migration `002_AddAssetArchiveIndex.sql` adds active-plugin browse indexes
for `RecordInstances` and typed parent tables, plus indexes for `Statics`, `Containers`, `ContainerItems`, and
`RawRecordPayloads`.

`Plugins.ImportState` is constrained to `Current`, `Changed`, `PartiallyImported`, `Missing`, `Failed`, or
`Unsupported`.

## Inferred Relationships

These columns contain record-reference data but are not declared SQLite foreign keys:

- `FormLists.AddToList_ModKey_Name`, `AddToList_ModKey_Type`, `AddToList_ModKey_FileName`,
  and `AddToList_FormKey_ID`
- `FormListItems.Item_ModKey_Name`, `Item_ModKey_Type`, `Item_ModKey_FileName`, and `Item_FormKey_ID`
- `Containers.NativeTerminal_ModKey_Name`, `NativeTerminal_ModKey_Type`, `NativeTerminal_ModKey_FileName`, and
  `NativeTerminal_FormKey_ID`
- `ContainerItems.Item_ModKey_Name`, `Item_ModKey_Type`, `Item_ModKey_FileName`, and `Item_FormKey_ID`
- `ModelMaterialSwaps.MaterialSwap_ModKey_Name`, `MaterialSwap_ModKey_Type`, `MaterialSwap_ModKey_FileName`,
  and `MaterialSwap_FormKey_ID`
- `RecordKeywords.Keyword_ModKey_Name`, `Keyword_ModKey_Type`, `Keyword_ModKey_FileName`, and `Keyword_FormKey_ID`
- New Starfield record-reference columns on `MiscObjects`, `Keywords`, `NPCs`, `MagicEffects`, `Perks`,
  `PerkRanks`, `PerkBackgroundSkills`, `ScriptingAdapterProperties`, and `ScriptingAdapterPropertyListItems`
