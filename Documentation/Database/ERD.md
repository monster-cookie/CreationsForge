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
        TEXT ImportMessage
        TEXT ImportDetails
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
        INTEGER Version2
        INTEGER VersionControl
        TEXT DataType
        TEXT Data
        REAL FloatData
        INTEGER IntegerData
        INTEGER UnsignedIntegerData
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
        INTEGER Version2
        INTEGER VersionControl
        TEXT MutagenObjectType
        TEXT MajorFlags
        REAL Data
    }

    Classes {
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
        INTEGER Version2
        INTEGER VersionControl
        TEXT ImportedAtUTC
        TEXT Name
        TEXT Description
        TEXT Teaches
        INTEGER MaxTrainingLevel
        REAL BleedoutDefault
        REAL VoicePoints
        REAL Unknown
        REAL Unknown2
    }

    ClassProperties {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Property_Index PK
        TEXT ActorValue_ModKey_Name
        INTEGER ActorValue_ModKey_Type
        TEXT ActorValue_ModKey_FileName
        INTEGER ActorValue_FormKey_ID
        REAL Value
        TEXT ImportedAtUTC
    }

    ClassWeights {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT WeightType PK
        INTEGER Weight_Index PK
        TEXT Key
        REAL Value
        TEXT ImportedAtUTC
    }

    Factions {
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
        INTEGER VersionControl
        TEXT Name
        TEXT Flags
        REAL FormationRadius
        TEXT Keyword_ModKey_Name
        INTEGER Keyword_ModKey_Type
        TEXT Keyword_ModKey_FileName
        INTEGER Keyword_FormKey_ID
        TEXT Herd_ModKey_Name
        INTEGER Herd_ModKey_Type
        TEXT Herd_ModKey_FileName
        INTEGER Herd_FormKey_ID
        TEXT VoiceType_ModKey_Name
        INTEGER VoiceType_ModKey_Type
        TEXT VoiceType_ModKey_FileName
        INTEGER VoiceType_FormKey_ID
        TEXT SharedCrimeFactionList_ModKey_Name
        INTEGER SharedCrimeFactionList_ModKey_Type
        TEXT SharedCrimeFactionList_ModKey_FileName
        INTEGER SharedCrimeFactionList_FormKey_ID
        TEXT VendorBuySellList_ModKey_Name
        INTEGER VendorBuySellList_ModKey_Type
        TEXT VendorBuySellList_ModKey_FileName
        INTEGER VendorBuySellList_FormKey_ID
        TEXT MerchantContainer_ModKey_Name
        INTEGER MerchantContainer_ModKey_Type
        TEXT MerchantContainer_ModKey_FileName
        INTEGER MerchantContainer_FormKey_ID
        TEXT ExteriorJailMarker_ModKey_Name
        INTEGER ExteriorJailMarker_ModKey_Type
        TEXT ExteriorJailMarker_ModKey_FileName
        INTEGER ExteriorJailMarker_FormKey_ID
        TEXT FollowerWaitMarker_ModKey_Name
        INTEGER FollowerWaitMarker_ModKey_Type
        TEXT FollowerWaitMarker_ModKey_FileName
        INTEGER FollowerWaitMarker_FormKey_ID
        TEXT StolenGoodsContainer_ModKey_Name
        INTEGER StolenGoodsContainer_ModKey_Type
        TEXT StolenGoodsContainer_ModKey_FileName
        INTEGER StolenGoodsContainer_FormKey_ID
        TEXT PlayerInventoryContainer_ModKey_Name
        INTEGER PlayerInventoryContainer_ModKey_Type
        TEXT PlayerInventoryContainer_ModKey_FileName
        INTEGER PlayerInventoryContainer_FormKey_ID
        TEXT JailOutfit_ModKey_Name
        INTEGER JailOutfit_ModKey_Type
        TEXT JailOutfit_ModKey_FileName
        INTEGER JailOutfit_FormKey_ID
        INTEGER CrimeValues_Arrest
        INTEGER CrimeValues_AttackOnSight
        INTEGER CrimeValues_Murder
        INTEGER CrimeValues_Assault
        INTEGER CrimeValues_Trespass
        INTEGER CrimeValues_Pickpocket
        INTEGER CrimeValues_Steal
        REAL CrimeValues_StealMult
        REAL CrimeValues_StealMultiplier
        INTEGER CrimeValues_Escape
        INTEGER CrimeValues_Werewolf
        INTEGER CrimeValues_WerewolfUnused
        INTEGER CrimeValues_Unknown
        INTEGER CrimeValues_Piracy
        REAL CrimeValues_SmuggleMultiplier
        REAL VendorValues_StartHour
        REAL VendorValues_EndHour
        INTEGER VendorValues_Radius
        INTEGER VendorValues_BuysStolenItems
        INTEGER VendorValues_BuysNonStolenItems
        INTEGER VendorValues_BuySellEverythingNotInList
        TEXT VendorLocation_MutagenObjectType
        TEXT VendorLocation_Target_MutagenObjectType
        TEXT VendorLocation_Target_Type
        TEXT VendorLocation_Target_Link_ModKey_Name
        INTEGER VendorLocation_Target_Link_ModKey_Type
        TEXT VendorLocation_Target_Link_ModKey_FileName
        INTEGER VendorLocation_Target_Link_FormKey_ID
    }

    FactionRelations {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Relation_Index PK
        TEXT Target_ModKey_Name
        INTEGER Target_ModKey_Type
        TEXT Target_ModKey_FileName
        INTEGER Target_FormKey_ID
        TEXT Reaction
        TEXT ImportedAtUTC
    }

    FactionRanks {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Rank_Index PK
        INTEGER Number
        TEXT Title_Male
        TEXT Title_Female
        TEXT ImportedAtUTC
    }

    ConditionRules {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT ConditionSlot PK
        INTEGER Condition_Index PK
        TEXT MutagenObjectType
        TEXT DataMutagenObjectType
        TEXT CompareOperator
        TEXT Flags
        INTEGER Unknown2
        TEXT ComparisonValue
        TEXT ComparisonValue_ModKey_Name
        INTEGER ComparisonValue_ModKey_Type
        TEXT ComparisonValue_ModKey_FileName
        INTEGER ComparisonValue_FormKey_ID
        TEXT ImportedAtUTC
    }

    ConditionRuleParameters {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT ConditionSlot PK, FK
        INTEGER Condition_Index PK, FK
        TEXT Parameter_Name PK
        TEXT ParameterValue
        TEXT Parameter_ModKey_Name
        INTEGER Parameter_ModKey_Type
        TEXT Parameter_ModKey_FileName
        INTEGER Parameter_FormKey_ID
        TEXT ImportedAtUTC
    }

    MiscItems {
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
        INTEGER VersionControl
        TEXT ObjectBounds_First
        TEXT ObjectBounds_Second
        TEXT Transforms_Inventory_ModKey_Name
        INTEGER Transforms_Inventory_ModKey_Type
        TEXT Transforms_Inventory_ModKey_FileName
        INTEGER Transforms_Inventory_FormKey_ID
        TEXT PreviewTransform_ModKey_Name
        INTEGER PreviewTransform_ModKey_Type
        TEXT PreviewTransform_ModKey_FileName
        INTEGER PreviewTransform_FormKey_ID
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

    MiscItemComponents {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT Component_ModKey_Name
        INTEGER Component_ModKey_Type
        TEXT Component_ModKey_FileName
        INTEGER Component_FormKey_ID
        INTEGER Component_Index PK
        INTEGER DisplayIndex
        INTEGER Count
        TEXT ImportedAtUTC
    }

    MiscItemDestructibles {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Health
        INTEGER DESTCount
        TEXT ImportedAtUTC
    }

    MiscItemDestructibleStages {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Stage_Index PK
        INTEGER StageRecordIndex
        INTEGER HealthPercent
        INTEGER ModelDamageStage
        TEXT Flags
        INTEGER SelfDamagePerSecond
        TEXT Explosion_ModKey_Name
        INTEGER Explosion_ModKey_Type
        TEXT Explosion_ModKey_FileName
        INTEGER Explosion_FormKey_ID
        TEXT Model_File
        TEXT Model_Data
        TEXT ImportedAtUTC
    }

    MiscItemResources {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT Resource_ModKey_Name
        INTEGER Resource_ModKey_Type
        TEXT Resource_ModKey_FileName
        INTEGER Resource_FormKey_ID
        INTEGER Resource_Index PK
        INTEGER Count
        TEXT ImportedAtUTC
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
        INTEGER Version2
        INTEGER VersionControl
        TEXT FNAM
        TEXT WAIM
        TEXT WFIR
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
        TEXT Description
        TEXT CNAM
        REAL Skill_ImproveMult
        REAL Skill_ImproveOffset
        REAL Skill_UseMult
        TEXT ContextNotes
        REAL DefaultValue
        TEXT Flags
        TEXT Type
        REAL Min
        REAL Max
    }

    ActorValueInformationPerkTreeEntries {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER PerkTree_Index PK
        TEXT AssociatedSkill_ModKey_Name
        INTEGER AssociatedSkill_ModKey_Type
        TEXT AssociatedSkill_ModKey_FileName
        INTEGER AssociatedSkill_FormKey_ID
        TEXT FNAM
        REAL HorizontalPosition
        INTEGER EntryIndex
        INTEGER PerkGridX
        INTEGER PerkGridY
        REAL VerticalPosition
        TEXT Perk_ModKey_Name
        INTEGER Perk_ModKey_Type
        TEXT Perk_ModKey_FileName
        INTEGER Perk_FormKey_ID
        TEXT ImportedAtUTC
    }

    ActorValueInformationPerkTreeConnectionLineIndices {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER PerkTree_Index PK, FK
        INTEGER ConnectionLine_Index PK
        INTEGER TargetIndex
        TEXT ImportedAtUTC
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
        INTEGER Version2
        INTEGER VersionControl
        INTEGER DispositionBase
        TEXT Aggression
        TEXT Confidence
        INTEGER EnergyLevel
        TEXT Responsibility
        TEXT Assistance
        TEXT Mood
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
        TEXT Template
        TEXT DefaultTemplate
        TEXT TemplateActors
        TEXT WornArmor
        TEXT FaceMorph
        TEXT FaceParts
        TEXT HeadParts
        TEXT HeadTexture
        TEXT SleepingOutfit
        TEXT TintLayers
        TEXT Tints
        TEXT SpaceOutfit
        TEXT BodyMorphRegionValues
        TEXT ObjectTemplates
        TEXT AIData
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
        INTEGER Version2
        INTEGER VersionControl
        TEXT Flags
        TEXT CastType
        TEXT TargetType
        TEXT CastingSoundLevel
        TEXT DualCastScale
        TEXT Unknown1
        TEXT BaseCost
        TEXT MagicSkill
        TEXT CastingLight_ModKey_Name
        INTEGER CastingLight_ModKey_Type
        TEXT CastingLight_ModKey_FileName
        INTEGER CastingLight_FormKey_ID
        TEXT MenuDisplayObject_ModKey_Name
        INTEGER MenuDisplayObject_ModKey_Type
        TEXT MenuDisplayObject_ModKey_FileName
        INTEGER MenuDisplayObject_FormKey_ID
        INTEGER MinimumSkillLevel
        TEXT SkillUsageMultiplier
        TEXT SpellmakingCastingTime
        TEXT TaperWeight
        TEXT SecondActorValue
        TEXT SecondActorValueWeight
        INTEGER SpellmakingArea
        TEXT EnchantShader_ModKey_Name
        INTEGER EnchantShader_ModKey_Type
        TEXT EnchantShader_ModKey_FileName
        INTEGER EnchantShader_FormKey_ID
        TEXT ActorValue2_ModKey_Name
        INTEGER ActorValue2_ModKey_Type
        TEXT ActorValue2_ModKey_FileName
        INTEGER ActorValue2_FormKey_ID
        TEXT ResistValue_ModKey_Name
        INTEGER ResistValue_ModKey_Type
        TEXT ResistValue_ModKey_FileName
        INTEGER ResistValue_FormKey_ID
        TEXT ResistValue
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
        TEXT ArchetypeActorValue
        TEXT ArchetypeAssociation_ModKey_Name
        INTEGER ArchetypeAssociation_ModKey_Type
        TEXT ArchetypeAssociation_ModKey_FileName
        INTEGER ArchetypeAssociation_FormKey_ID
        REAL UnknownFloat1
        REAL UnknownFloat3
        REAL UnknownFloat4
        INTEGER UnknownInt2
        INTEGER UnknownInt3
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
        INTEGER Level
        INTEGER NumRanks
        INTEGER Playable
        INTEGER Hidden
        TEXT NextPerk_ModKey_Name
        INTEGER NextPerk_ModKey_Type
        TEXT NextPerk_ModKey_FileName
        INTEGER NextPerk_FormKey_ID
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
        TEXT Name
        INTEGER Version2
        TEXT ObjectBounds_First
        TEXT ObjectBounds_Second
        REAL MaxAngle
        REAL UnknownDNAMFloat
        REAL LeafAmplitude
        REAL LeafFrequency
        TEXT Unused
        TEXT DNAMDataTypeState
        REAL DirtinessScale
        TEXT SnapTemplate_ModKey_Name
        INTEGER SnapTemplate_ModKey_Type
        TEXT SnapTemplate_ModKey_FileName
        INTEGER SnapTemplate_FormKey_ID
        TEXT PreviewTransform_ModKey_Name
        INTEGER PreviewTransform_ModKey_Type
        TEXT PreviewTransform_ModKey_FileName
        INTEGER PreviewTransform_FormKey_ID
        TEXT Material_ModKey_Name
        INTEGER Material_ModKey_Type
        TEXT Material_ModKey_FileName
        INTEGER Material_FormKey_ID
        TEXT Lod_Level0
        TEXT Lod_Level1
        TEXT Lod_Level2
        TEXT Lod_Level3
        TEXT NavmeshGeometry
    }

    StaticProperties {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Property_Index PK
        TEXT ActorValue_ModKey_Name
        INTEGER ActorValue_ModKey_Type
        TEXT ActorValue_ModKey_FileName
        INTEGER ActorValue_FormKey_ID
        REAL Value
        TEXT ImportedAtUTC
    }

    Books {
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
        INTEGER VersionControl
        TEXT ObjectBounds_First
        TEXT ObjectBounds_Second
        TEXT Transforms_Inventory_ModKey_Name
        INTEGER Transforms_Inventory_ModKey_Type
        TEXT Transforms_Inventory_ModKey_FileName
        INTEGER Transforms_Inventory_FormKey_ID
        TEXT InventoryArt_ModKey_Name
        INTEGER InventoryArt_ModKey_Type
        TEXT InventoryArt_ModKey_FileName
        INTEGER InventoryArt_FormKey_ID
        TEXT PreviewTransform_ModKey_Name
        INTEGER PreviewTransform_ModKey_Type
        TEXT PreviewTransform_ModKey_FileName
        INTEGER PreviewTransform_FormKey_ID
        TEXT FeaturedItemMessage_ModKey_Name
        INTEGER FeaturedItemMessage_ModKey_Type
        TEXT FeaturedItemMessage_ModKey_FileName
        INTEGER FeaturedItemMessage_FormKey_ID
        INTEGER XALG
        TEXT Name
        TEXT Text
        INTEGER Value
        REAL Weight
        TEXT Flags
        TEXT Teaches_MutagenObjectType
        TEXT Teaches_Perk_ModKey_Name
        INTEGER Teaches_Perk_ModKey_Type
        TEXT Teaches_Perk_ModKey_FileName
        INTEGER Teaches_Perk_FormKey_ID
        TEXT Teaches_RawContent
        TEXT DataSlateType
        TEXT Description
        TEXT DataSlateHeaderLeft
        TEXT DataSlateHeaderRight
    }

    Doors {
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
        INTEGER VersionControl
        TEXT ObjectBounds_First
        TEXT ObjectBounds_Second
        TEXT Name
        TEXT Flags
        TEXT NativeTerminal_ModKey_Name
        INTEGER NativeTerminal_ModKey_Type
        TEXT NativeTerminal_ModKey_FileName
        INTEGER NativeTerminal_FormKey_ID
        TEXT SoundLevel
        TEXT FacingAxisOverride
        TEXT AnimationGraph
        TEXT AnimationSkeleton
        TEXT AnimationDirectory
        TEXT AnimationFile
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
        INTEGER VersionControl
        TEXT ObjectBounds_First
        TEXT ObjectBounds_Second
        TEXT Name
        TEXT Flags
        TEXT MajorFlags
        TEXT NativeTerminal_ModKey_Name
        INTEGER NativeTerminal_ModKey_Type
        TEXT NativeTerminal_ModKey_FileName
        INTEGER NativeTerminal_FormKey_ID
        TEXT AnimationGraph
        TEXT AnimationSkeleton
        TEXT AnimationDirectory
        TEXT AnimationFile
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

    ConditionForms {
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
        INTEGER VersionControl
    }

    ConstructibleObjects {
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
        INTEGER VersionControl
        TEXT Description
        TEXT CreatedObject_ModKey_Name
        INTEGER CreatedObject_ModKey_Type
        TEXT CreatedObject_ModKey_FileName
        INTEGER CreatedObject_FormKey_ID
        TEXT WorkbenchKeyword_ModKey_Name
        INTEGER WorkbenchKeyword_ModKey_Type
        TEXT WorkbenchKeyword_ModKey_FileName
        INTEGER WorkbenchKeyword_FormKey_ID
        INTEGER CreatedObjectCount
        INTEGER AmountProduced
        INTEGER Value
        REAL MenuSortOrder
        TEXT LearnMethod
        TEXT Flags
        TEXT MajorFlags
    }

    ConstructibleObjectComponents {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Component_Index PK
        TEXT Component_ModKey_Name
        INTEGER Component_ModKey_Type
        TEXT Component_ModKey_FileName
        INTEGER Component_FormKey_ID
        INTEGER Count
        TEXT ImportedAtUTC
    }

    ConstructibleObjectCategories {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Category_Index PK
        TEXT Category_ModKey_Name
        INTEGER Category_ModKey_Type
        TEXT Category_ModKey_FileName
        INTEGER Category_FormKey_ID
        TEXT ImportedAtUTC
    }

    ConstructibleObjectRecipeFilters {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER RecipeFilter_Index PK
        TEXT RecipeFilter_ModKey_Name
        INTEGER RecipeFilter_ModKey_Type
        TEXT RecipeFilter_ModKey_FileName
        INTEGER RecipeFilter_FormKey_ID
        TEXT ImportedAtUTC
    }

    Terminals {
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
        INTEGER VersionControl
        TEXT ObjectBounds_First
        TEXT ObjectBounds_Second
        TEXT Menu_ModKey_Name
        INTEGER Menu_ModKey_Type
        TEXT Menu_ModKey_FileName
        INTEGER Menu_FormKey_ID
        TEXT Background
        TEXT HeaderText
        TEXT WelcomeText
        TEXT Name
        TEXT PNAM
        TEXT FNAM
        TEXT Flags
        TEXT MajorFlags
        TEXT JNAM
        TEXT MarkerFlags
        TEXT GNAM
        TEXT WorkbenchData
        TEXT FurnitureTemplate_ModKey_Name
        INTEGER FurnitureTemplate_ModKey_Type
        TEXT FurnitureTemplate_ModKey_FileName
        INTEGER FurnitureTemplate_FormKey_ID
        TEXT MarkerModel
        TEXT AnimationGraph
        TEXT AnimationSkeleton
        TEXT AnimationDirectory
        TEXT AnimationFile
    }

    TerminalForcedLocations {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT ForcedLocation_ModKey_Name
        INTEGER ForcedLocation_ModKey_Type
        TEXT ForcedLocation_ModKey_FileName
        INTEGER ForcedLocation_FormKey_ID
        INTEGER ForcedLocation_Index PK
        TEXT ImportedAtUTC
    }

    TerminalMarkerParameters {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Parameter_Index PK
        INTEGER Enabled
        TEXT Offset
        TEXT EntryTypes
        TEXT ExitTypes
        TEXT Unknown
        TEXT ImportedAtUTC
    }

    TerminalBodyTexts {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER BodyText_Index PK
        TEXT Text
        TEXT ImportedAtUTC
    }

    TerminalMenuItems {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER MenuItem_Index PK
        TEXT ItemText
        TEXT Type
        INTEGER ItemId
        TEXT Submenu_ModKey_Name
        INTEGER Submenu_ModKey_Type
        TEXT Submenu_ModKey_FileName
        INTEGER Submenu_FormKey_ID
        TEXT DisplayText
        TEXT ImportedAtUTC
    }

    KeywordMappings {
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

    Components {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Component_Index PK
        TEXT MutagenObjectType
        TEXT ImportedAtUTC
    }

    ComponentItems {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Component_Index PK, FK
        INTEGER Item_Index PK
        REAL Unknown1
        REAL Unknown2
        REAL Unknown3
        REAL Unknown4
        REAL Unknown5
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
        TEXT ActorValue
        TEXT Spell
        TEXT Quest
        INTEGER Stage
        TEXT ImportedAtUTC
    }

    PerkRankActivities {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Rank_Index PK, FK
        INTEGER Activity_Index PK
        TEXT ATAN
        TEXT Name
        TEXT Description
        TEXT ANAM
        TEXT Configuration
        TEXT ImportedAtUTC
    }

    PerkRankActivityProgressionEvaluators {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Rank_Index PK, FK
        INTEGER Activity_Index PK, FK
        INTEGER Evaluator_Index PK
        TEXT Name
        TEXT ImportedAtUTC
    }

    PerkEffects {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
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
        TEXT ActorValue
        TEXT Spell
        TEXT Quest
        INTEGER Stage
        TEXT ImportedAtUTC
    }

    PerkEffectConditionTabs {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        INTEGER Rank_Index PK
        INTEGER Effect_Index PK
        INTEGER ConditionTab_Index PK
        INTEGER RunOnTabIndex
        INTEGER ConditionCount
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
        TEXT Data
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
        TEXT Name
        TEXT MaterialSwap_ModKey_Name
        INTEGER MaterialSwap_ModKey_Type
        TEXT MaterialSwap_ModKey_FileName
        INTEGER MaterialSwap_FormKey_ID
        INTEGER MaterialSwap_Index PK
        TEXT ImportedAtUTC
    }

    SoundMappings {
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
        TEXT MutagenObjectType
        TEXT Start
        TEXT Stop
        TEXT InheritsSoundsFrom
        TEXT Versioning
        TEXT Unknown
        TEXT ImportedAtUTC
    }

    ScriptFragments {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT FragmentSlot PK
        INTEGER Fragment_Index PK
        TEXT MutagenObjectType
        TEXT ScriptName
        TEXT FragmentName
        INTEGER Unknown2
        INTEGER ExtraBindDataVersion
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
        TEXT SourcePath
        TEXT PayloadValue
        TEXT ImportedAtUTC
    }

    LocalizedStrings {
        TEXT Game PK, FK
        TEXT ModKey_Name PK, FK
        INTEGER ModKey_Type PK, FK
        TEXT ModKey_FileName PK, FK
        TEXT RecordType PK, FK
        TEXT FormKey_ModKey_Name PK, FK
        INTEGER FormKey_ModKey_Type PK, FK
        TEXT FormKey_ModKey_FileName PK, FK
        INTEGER FormKey_ID PK, FK
        TEXT SourceField PK
        TEXT Language PK
        TEXT Value
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
    RecordInstances ||--o| Classes : "typed detail"
    Classes ||--o{ ClassProperties : contains
    Classes ||--o{ ClassWeights : contains
    RecordInstances ||--o| Factions : "typed detail"
    Factions ||--o{ FactionRelations : contains
    Factions ||--o{ FactionRanks : contains
    RecordInstances ||--o{ ConditionRules : contains
    ConditionRules ||--o{ ConditionRuleParameters : contains
    RecordInstances ||--o| MiscItems : "typed detail"
    MiscItems ||--o{ MiscItemComponents : contains
    MiscItems ||--o| MiscItemDestructibles : contains
    MiscItemDestructibles ||--o{ MiscItemDestructibleStages : contains
    MiscItems ||--o{ MiscItemResources : contains
    RecordInstances ||--o| Keywords : "typed detail"
    RecordInstances ||--o| ActorValueInformation : "typed detail"
    ActorValueInformation ||--o{ ActorValueInformationPerkTreeEntries : contains
    ActorValueInformationPerkTreeEntries ||--o{ ActorValueInformationPerkTreeConnectionLineIndices : contains
    RecordInstances ||--o| NPCs : "typed detail"
    RecordInstances ||--o| MagicEffects : "typed detail"
    RecordInstances ||--o| Perks : "typed detail"
    RecordInstances ||--o| Statics : "typed detail"
    Statics ||--o{ StaticProperties : contains
    RecordInstances ||--o| Books : "typed detail"
    RecordInstances ||--o| Doors : "typed detail"
    RecordInstances ||--o| Containers : "typed detail"
    Containers ||--o{ ContainerItems : contains
    RecordInstances ||--o| ConditionForms : "typed detail"
    RecordInstances ||--o| ConstructibleObjects : "typed detail"
    ConstructibleObjects ||--o{ ConstructibleObjectComponents : contains
    ConstructibleObjects ||--o{ ConstructibleObjectCategories : contains
    ConstructibleObjects ||--o{ ConstructibleObjectRecipeFilters : contains
    RecordInstances ||--o| Terminals : "typed detail"
    Terminals ||--o{ TerminalForcedLocations : contains
    Terminals ||--o{ TerminalMarkerParameters : contains
    Terminals ||--o{ TerminalBodyTexts : contains
    Terminals ||--o{ TerminalMenuItems : contains
    RecordInstances ||--o{ KeywordMappings : contains
    RecordInstances ||--o{ Components : contains
    Components ||--o{ ComponentItems : contains
    Perks ||--o{ PerkRanks : contains
    PerkRanks ||--o{ PerkRankEffects : contains
    PerkRanks ||--o{ PerkRankActivities : contains
    PerkRankActivities ||--o{ PerkRankActivityProgressionEvaluators : contains
    Perks ||--o{ PerkEffects : contains
    Perks ||--o{ PerkEffectConditionTabs : contains
    Perks ||--o{ PerkBackgroundSkills : contains
    RecordInstances ||--o{ Models : contains
    Models ||--o{ ModelMaterialSwaps : contains
    RecordInstances ||--o{ SoundMappings : contains
    RecordInstances ||--o{ ScriptFragments : contains
    RecordInstances ||--o{ RawRecordPayloads : contains
    RecordInstances ||--o{ LocalizedStrings : contains
    RecordInstances ||--o{ ScriptingAdapters : contains
    ScriptingAdapters ||--o{ ScriptingAdapterProperties : contains
    ScriptingAdapterProperties ||--o{ ScriptingAdapterPropertyListItems : contains
    AssetArchiveFiles ||--o{ AssetArchiveEntries : contains
```

## Index Notes

Indexes are documented in `DATABASE.md`. Migration `001_ResetSchemaForV2.sql` creates active-plugin browse indexes for
`RecordInstances` and typed parent tables, indexes for child lookup tables, the localized-string form-key lookup index,
and ActorValueInformation child-table form-key lookup indexes.

`Plugins.ImportState` is constrained to `Current`, `Changed`, `PartiallyImported`, `Missing`, `Failed`, or
`Unsupported`.

## Inferred Relationships

These columns contain record-reference data but are not declared SQLite foreign keys:

- `FormLists.AddToList_ModKey_Name`, `AddToList_ModKey_Type`, `AddToList_ModKey_FileName`,
  and `AddToList_FormKey_ID`
- `FormListItems.Item_ModKey_Name`, `Item_ModKey_Type`, `Item_ModKey_FileName`, and `Item_FormKey_ID`
- `ClassProperties.ActorValue_ModKey_Name`, `ActorValue_ModKey_Type`, `ActorValue_ModKey_FileName`, and
  `ActorValue_FormKey_ID`
- `ActorValueInformationPerkTreeEntries.AssociatedSkill_ModKey_Name`, `AssociatedSkill_ModKey_Type`,
  `AssociatedSkill_ModKey_FileName`, and `AssociatedSkill_FormKey_ID`
- `ActorValueInformationPerkTreeEntries.Perk_ModKey_Name`, `Perk_ModKey_Type`, `Perk_ModKey_FileName`, and
  `Perk_FormKey_ID`
- `Factions.Keyword_ModKey_Name`, `Keyword_ModKey_Type`, `Keyword_ModKey_FileName`, and `Keyword_FormKey_ID`
- `Factions.Herd_ModKey_Name`, `Herd_ModKey_Type`, `Herd_ModKey_FileName`, and `Herd_FormKey_ID`
- `Factions.VoiceType_ModKey_Name`, `VoiceType_ModKey_Type`, `VoiceType_ModKey_FileName`, and
  `VoiceType_FormKey_ID`
- `Factions.SharedCrimeFactionList_ModKey_Name`, `SharedCrimeFactionList_ModKey_Type`,
  `SharedCrimeFactionList_ModKey_FileName`, and `SharedCrimeFactionList_FormKey_ID`
- `Factions.VendorBuySellList_ModKey_Name`, `VendorBuySellList_ModKey_Type`,
  `VendorBuySellList_ModKey_FileName`, and `VendorBuySellList_FormKey_ID`
- `Factions.MerchantContainer_ModKey_Name`, `MerchantContainer_ModKey_Type`,
  `MerchantContainer_ModKey_FileName`, and `MerchantContainer_FormKey_ID`
- `Factions.ExteriorJailMarker_ModKey_Name`, `ExteriorJailMarker_ModKey_Type`,
  `ExteriorJailMarker_ModKey_FileName`, and `ExteriorJailMarker_FormKey_ID`
- `Factions.FollowerWaitMarker_ModKey_Name`, `FollowerWaitMarker_ModKey_Type`,
  `FollowerWaitMarker_ModKey_FileName`, and `FollowerWaitMarker_FormKey_ID`
- `Factions.StolenGoodsContainer_ModKey_Name`, `StolenGoodsContainer_ModKey_Type`,
  `StolenGoodsContainer_ModKey_FileName`, and `StolenGoodsContainer_FormKey_ID`
- `Factions.PlayerInventoryContainer_ModKey_Name`, `PlayerInventoryContainer_ModKey_Type`,
  `PlayerInventoryContainer_ModKey_FileName`, and `PlayerInventoryContainer_FormKey_ID`
- `Factions.JailOutfit_ModKey_Name`, `JailOutfit_ModKey_Type`, `JailOutfit_ModKey_FileName`, and
  `JailOutfit_FormKey_ID`
- `Factions.VendorLocation_Target_Link_ModKey_Name`, `VendorLocation_Target_Link_ModKey_Type`,
  `VendorLocation_Target_Link_ModKey_FileName`, and `VendorLocation_Target_Link_FormKey_ID`
- `FactionRelations.Target_ModKey_Name`, `Target_ModKey_Type`, `Target_ModKey_FileName`, and
  `Target_FormKey_ID`
- `ConditionRules.ComparisonValue_ModKey_Name`, `ComparisonValue_ModKey_Type`,
  `ComparisonValue_ModKey_FileName`, and `ComparisonValue_FormKey_ID`
- `ConditionRuleParameters.Parameter_ModKey_Name`, `Parameter_ModKey_Type`, `Parameter_ModKey_FileName`, and
  `Parameter_FormKey_ID`
- `MiscItems.FeaturedItemMessage_ModKey_Name`, `FeaturedItemMessage_ModKey_Type`,
  `FeaturedItemMessage_ModKey_FileName`, and `FeaturedItemMessage_FormKey_ID`
- `MiscItems.Transforms_Inventory_ModKey_Name`, `Transforms_Inventory_ModKey_Type`,
  `Transforms_Inventory_ModKey_FileName`, and `Transforms_Inventory_FormKey_ID`
- `MiscItems.PreviewTransform_ModKey_Name`, `PreviewTransform_ModKey_Type`,
  `PreviewTransform_ModKey_FileName`, and `PreviewTransform_FormKey_ID`
- `MiscItemComponents.Component_ModKey_Name`, `Component_ModKey_Type`, `Component_ModKey_FileName`, and
  `Component_FormKey_ID`
- `MiscItemDestructibleStages.Explosion_ModKey_Name`, `Explosion_ModKey_Type`, `Explosion_ModKey_FileName`, and
  `Explosion_FormKey_ID`
- `MiscItemResources.Resource_ModKey_Name`, `Resource_ModKey_Type`, `Resource_ModKey_FileName`, and
  `Resource_FormKey_ID`
- `Books.Transforms_Inventory_ModKey_Name`, `Transforms_Inventory_ModKey_Type`,
  `Transforms_Inventory_ModKey_FileName`, and `Transforms_Inventory_FormKey_ID`
- `Books.InventoryArt_ModKey_Name`, `InventoryArt_ModKey_Type`, `InventoryArt_ModKey_FileName`,
  and `InventoryArt_FormKey_ID`
- `Books.PreviewTransform_ModKey_Name`, `PreviewTransform_ModKey_Type`,
  `PreviewTransform_ModKey_FileName`, and `PreviewTransform_FormKey_ID`
- `Books.FeaturedItemMessage_ModKey_Name`, `FeaturedItemMessage_ModKey_Type`,
  `FeaturedItemMessage_ModKey_FileName`, and `FeaturedItemMessage_FormKey_ID`
- `Books.Teaches_Perk_ModKey_Name`, `Teaches_Perk_ModKey_Type`, `Teaches_Perk_ModKey_FileName`,
  and `Teaches_Perk_FormKey_ID`
- `Doors.NativeTerminal_ModKey_Name`, `NativeTerminal_ModKey_Type`, `NativeTerminal_ModKey_FileName`, and
  `NativeTerminal_FormKey_ID`
- `Containers.NativeTerminal_ModKey_Name`, `NativeTerminal_ModKey_Type`, `NativeTerminal_ModKey_FileName`, and
  `NativeTerminal_FormKey_ID`
- `ConstructibleObjects.CreatedObject_ModKey_Name`, `CreatedObject_ModKey_Type`,
  `CreatedObject_ModKey_FileName`, and `CreatedObject_FormKey_ID`
- `ConstructibleObjects.WorkbenchKeyword_ModKey_Name`, `WorkbenchKeyword_ModKey_Type`,
  `WorkbenchKeyword_ModKey_FileName`, and `WorkbenchKeyword_FormKey_ID`
- `Terminals.Menu_ModKey_Name`, `Menu_ModKey_Type`, `Menu_ModKey_FileName`, and `Menu_FormKey_ID`
- `Terminals.FurnitureTemplate_ModKey_Name`, `FurnitureTemplate_ModKey_Type`,
  `FurnitureTemplate_ModKey_FileName`, and `FurnitureTemplate_FormKey_ID`
- `TerminalForcedLocations.ForcedLocation_ModKey_Name`, `ForcedLocation_ModKey_Type`,
  `ForcedLocation_ModKey_FileName`, and `ForcedLocation_FormKey_ID`
- `TerminalMenuItems.Submenu_ModKey_Name`, `Submenu_ModKey_Type`, `Submenu_ModKey_FileName`, and
  `Submenu_FormKey_ID`
- `ContainerItems.Item_ModKey_Name`, `Item_ModKey_Type`, `Item_ModKey_FileName`, and `Item_FormKey_ID`
- `ConstructibleObjectComponents.Component_ModKey_Name`, `Component_ModKey_Type`,
  `Component_ModKey_FileName`, and `Component_FormKey_ID`
- `ConstructibleObjectCategories.Category_ModKey_Name`, `Category_ModKey_Type`,
  `Category_ModKey_FileName`, and `Category_FormKey_ID`
- `ConstructibleObjectRecipeFilters.RecipeFilter_ModKey_Name`, `RecipeFilter_ModKey_Type`,
  `RecipeFilter_ModKey_FileName`, and `RecipeFilter_FormKey_ID`
- `ModelMaterialSwaps.MaterialSwap_ModKey_Name`, `MaterialSwap_ModKey_Type`, `MaterialSwap_ModKey_FileName`,
  and `MaterialSwap_FormKey_ID`
- `KeywordMappings.Keyword_ModKey_Name`, `Keyword_ModKey_Type`, `Keyword_ModKey_FileName`, and `Keyword_FormKey_ID`
- New Starfield record-reference columns on `MiscItems`, `Keywords`, `NPCs`, `MagicEffects`, `Perks`,
  `PerkRanks`, `PerkBackgroundSkills`, `ScriptingAdapterProperties`, and `ScriptingAdapterPropertyListItems`
