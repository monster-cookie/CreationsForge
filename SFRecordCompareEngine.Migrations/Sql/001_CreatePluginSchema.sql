CREATE TABLE Plugins
(
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    LoadOrderIndex          INTEGER NOT NULL,
    Enabled                 INTEGER NOT NULL DEFAULT 1,
    ExistsOnDisk            INTEGER NOT NULL DEFAULT 1,
    ImportState             TEXT    NOT NULL DEFAULT 'Current',
    HeaderFlags             INTEGER NOT NULL,
    FormVersion             INTEGER NOT NULL,
    Author                  TEXT    NOT NULL,
    Branch                  TEXT    NOT NULL,
    InteriorCellCount       INTEGER NOT NULL,
    RecordCount             INTEGER NOT NULL DEFAULT 0,
    SourceLastWriteUTCTicks INTEGER NOT NULL,
    SourceFileSizeBytes     INTEGER NOT NULL,
    LastCheckedUTC          TEXT    NOT NULL,
    LastImportedUTC         TEXT    NULL,
    InvalidatedAtUTC        TEXT    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName),
    CHECK (Enabled IN (0, 1)),
    CHECK (ExistsOnDisk IN (0, 1)),
    CHECK (ImportState IN ('Current', 'Changed', 'Missing', 'Failed', 'Unsupported')),
    CHECK (RecordCount >= 0)
);

CREATE INDEX IX_Plugins_LoadOrderIndex ON Plugins (LoadOrderIndex);
CREATE INDEX IX_Plugins_ImportState ON Plugins (ImportState);
CREATE INDEX IX_Plugins_SourceFingerprint ON Plugins (SourceLastWriteUTCTicks, SourceFileSizeBytes);

CREATE TABLE PluginMasterReferences
(
    Master_ModKey_Name     TEXT    NOT NULL,
    Master_ModKey_Type     INTEGER NOT NULL,
    Master_ModKey_FileName TEXT    NOT NULL,
    Plugin_ModKey_Name     TEXT    NOT NULL,
    Plugin_ModKey_Type     INTEGER NOT NULL,
    Plugin_ModKey_FileName TEXT    NOT NULL,
    ImportedAtUTC          TEXT    NOT NULL,
    PRIMARY KEY (Master_ModKey_Name, Master_ModKey_Type, Master_ModKey_FileName, Plugin_ModKey_Name, Plugin_ModKey_Type, Plugin_ModKey_FileName),
    FOREIGN KEY (Master_ModKey_Name, Master_ModKey_Type, Master_ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Plugin_ModKey_Name, Plugin_ModKey_Type, Plugin_ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE
);

CREATE INDEX IX_PluginMasterReferences_MasterModKey ON PluginMasterReferences (Master_ModKey_Name, Master_ModKey_Type, Master_ModKey_FileName);
CREATE INDEX IX_PluginMasterReferences_PluginModKey ON PluginMasterReferences (Plugin_ModKey_Name, Plugin_ModKey_Type, Plugin_ModKey_FileName);

CREATE TABLE FormList
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ModKey_Name       TEXT    NOT NULL,
    FormKey_ModKey_Type       INTEGER NOT NULL,
    FormKey_ModKey_FileName   TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    AddToListFormKey          TEXT    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_FormList_FormKey ON FormList (FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE FormListItems
(
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    Item_ModKey_Name        TEXT    NOT NULL,
    Item_ModKey_Type        INTEGER NOT NULL,
    Item_ModKey_FileName    TEXT    NOT NULL,
    Item_FormKey_ID         INTEGER NOT NULL,
    Item_Index              INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Item_ModKey_Name, Item_ModKey_Type, Item_ModKey_FileName, Item_FormKey_ID, Item_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES FormList (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Item_FormKey_ID >= 0),
    CHECK (Item_Index >= 0)
);

CREATE INDEX IX_FormListItems_FormList ON FormListItems (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_FormListItems_Item_FormKey ON FormListItems (Item_ModKey_Name, Item_ModKey_Type, Item_ModKey_FileName, Item_FormKey_ID);
CREATE INDEX IX_FormListItems_Item_Index ON FormListItems (Item_Index);

CREATE TABLE GameSetting
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ModKey_Name       TEXT    NOT NULL,
    FormKey_ModKey_Type       INTEGER NOT NULL,
    FormKey_ModKey_FileName   TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    SettingType               TEXT    NULL,
    Data                      TEXT    NULL,
    RawData                   REAL    NULL,
    XALG                      INTEGER NULL,
    IsCompressed              INTEGER NOT NULL,
    IsDeleted                 INTEGER NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (IsCompressed IN (0, 1)),
    CHECK (IsDeleted IN (0, 1))
);

CREATE INDEX IX_GameSetting_FormKey ON GameSetting (FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE Global
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ModKey_Name       TEXT    NOT NULL,
    FormKey_ModKey_Type       INTEGER NOT NULL,
    FormKey_ModKey_FileName   TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    Data                      REAL    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_Global_FormKey ON Global (FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE MiscItem
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ModKey_Name       TEXT    NOT NULL,
    FormKey_ModKey_Type       INTEGER NOT NULL,
    FormKey_ModKey_FileName   TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    Name                      TEXT    NULL,
    ShortName                 TEXT    NULL,
    Value                     INTEGER NULL,
    Weight                    REAL    NULL,
    DirtinessScale            REAL    NULL,
    FeaturedItemMessage_ModKey_Name TEXT NULL,
    FeaturedItemMessage_ModKey_Type INTEGER NULL,
    FeaturedItemMessage_ModKey_FileName TEXT NULL,
    FeaturedItemMessage_FormKey_ID INTEGER NULL,
    FLAG                      TEXT    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_MiscItem_FormKey ON MiscItem (FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE MiscItemObjectBounds
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    First_X REAL NOT NULL, First_Y REAL NOT NULL, First_Z REAL NOT NULL, Second_X REAL NOT NULL, Second_Y REAL NOT NULL, Second_Z REAL NOT NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) REFERENCES MiscItem ON DELETE CASCADE
);

CREATE TABLE MiscItemObjectPaletteDefaults
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    Flags TEXT NULL, SinkMeters REAL NULL, SinkVariance REAL NULL, XYOffsetVariance REAL NULL, FootprintSize TEXT NULL, ScalePercent REAL NULL, ScaleVariance REAL NULL,
    AngleXDegrees REAL NULL, AngleXVariance REAL NULL, AngleYDegrees REAL NULL, AngleYVariance REAL NULL, AngleZDegrees REAL NULL, AngleZVariance REAL NULL,
    SlopePercent REAL NULL, SlopePercentVariance REAL NULL, Density REAL NULL, FrequencyPercent REAL NULL, SlopeLimit REAL NULL, DistanceBelowWater REAL NULL, DistanceAboveWater REAL NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) REFERENCES MiscItem ON DELETE CASCADE
);

CREATE TABLE MiscItemTransforms
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    InventoryIcon_FormKey TEXT NULL, Outpost_FormKey TEXT NULL, Ship_FormKey TEXT NULL, Preview_FormKey TEXT NULL, Inventory_FormKey TEXT NULL, Workbench_FormKey TEXT NULL, MainGameUI_FormKey TEXT NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) REFERENCES MiscItem ON DELETE CASCADE
);

CREATE TABLE MiscItemModels
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    File TEXT NULL, TextureFileHashes TEXT NULL, LightLayer INTEGER NULL, Flags TEXT NULL, ColorRemappingIndex REAL NULL, FlagsVestigial TEXT NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) REFERENCES MiscItem ON DELETE CASCADE
);

CREATE TABLE MiscItemModelMaterialSwaps
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    MaterialSwap_FormKey TEXT NOT NULL, MaterialSwap_Index INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, MaterialSwap_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) REFERENCES MiscItemModels ON DELETE CASCADE
);

CREATE TABLE MiscItemSounds
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    SoundType TEXT NOT NULL, Start TEXT NULL, Stop TEXT NULL, Condition_FormKey TEXT NULL, EventMapping_FormKey TEXT NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, SoundType),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) REFERENCES MiscItem ON DELETE CASCADE
);

CREATE TABLE MiscItemKeywords
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    Keyword_FormKey TEXT NOT NULL, Keyword_Index INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Keyword_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) REFERENCES MiscItem ON DELETE CASCADE
);

CREATE TABLE MiscItemDestructibles
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    Health INTEGER NULL, StageCount INTEGER NULL, Flags TEXT NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) REFERENCES MiscItem ON DELETE CASCADE
);

CREATE TABLE MiscItemDestructibleResistances
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    DamageType_FormKey TEXT NOT NULL, Value INTEGER NOT NULL, Resistance_Index INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Resistance_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) REFERENCES MiscItemDestructibles ON DELETE CASCADE
);

CREATE TABLE MiscItemDestructionStages
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    Stage_Index INTEGER NOT NULL, HealthPercent INTEGER NULL, SourceIndex INTEGER NULL, ModelDamageStage INTEGER NULL, Flags TEXT NULL, SelfDamagePerSecond INTEGER NULL,
    Explosion_FormKey TEXT NULL, Debris_FormKey TEXT NULL, DebrisCount INTEGER NULL, SequenceName TEXT NULL, Model_File TEXT NULL, Model_LightLayer INTEGER NULL, Model_Flags TEXT NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Stage_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) REFERENCES MiscItemDestructibles ON DELETE CASCADE
);

CREATE TABLE MiscItemDestructionStageMaterialSwaps
(
    ModKey_Name TEXT NOT NULL, ModKey_Type INTEGER NOT NULL, ModKey_FileName TEXT NOT NULL,
    FormKey_ModKey_Name TEXT NOT NULL, FormKey_ModKey_Type INTEGER NOT NULL, FormKey_ModKey_FileName TEXT NOT NULL, FormKey_ID INTEGER NOT NULL,
    Stage_Index INTEGER NOT NULL, MaterialSwap_FormKey TEXT NOT NULL, MaterialSwap_Index INTEGER NOT NULL, ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Stage_Index, MaterialSwap_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Stage_Index) REFERENCES MiscItemDestructionStages ON DELETE CASCADE
);

CREATE TABLE Keyword
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ModKey_Name       TEXT    NOT NULL,
    FormKey_ModKey_Type       INTEGER NOT NULL,
    FormKey_ModKey_FileName   TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    Name                      TEXT    NULL,
    Color                     TEXT    NOT NULL,
    Type                      TEXT    NOT NULL,
    Notes                     TEXT    NULL,
    FlashLinkageName          TEXT    NULL,
    AttractionRuleFormKey     TEXT    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_Keyword_FormKey ON Keyword (FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE NPC
(
    ModKey_Name                      TEXT    NOT NULL,
    ModKey_Type                      INTEGER NOT NULL,
    ModKey_FileName                  TEXT    NOT NULL,
    FormKey_ModKey_Name              TEXT    NOT NULL,
    FormKey_ModKey_Type              INTEGER NOT NULL,
    FormKey_ModKey_FileName          TEXT    NOT NULL,
    FormKey_ID                       INTEGER NOT NULL,
    EditorID                         TEXT    NOT NULL,
    FormVersion                      INTEGER NOT NULL,
    StarfieldMajorRecordFlags        INTEGER NOT NULL,
    Version2                         INTEGER NOT NULL,
    VersionControl                   INTEGER NOT NULL,
    ImportedAtUTC                    TEXT    NOT NULL,
    Name                             TEXT    NULL,
    ShortName                        TEXT    NULL,
    LongName                         TEXT    NULL,
    DispositionBase                  INTEGER NOT NULL,
    Aggression                       TEXT    NOT NULL,
    Confidence                       TEXT    NOT NULL,
    EnergyLevel                      INTEGER NOT NULL,
    Responsibility                   TEXT    NOT NULL,
    Assistance                       TEXT    NOT NULL,
    GearedUpWeapons                  INTEGER NOT NULL,
    HeightMin                        REAL    NOT NULL,
    HeightMax                        REAL    NOT NULL,
    SkinToneIndex                    INTEGER NULL,
    Pronoun                          TEXT    NULL,
    VoiceFormKey                     TEXT    NULL,
    RaceFormKey                      TEXT    NULL,
    CombatOverridePackageListFormKey TEXT    NULL,
    CombatStyleFormKey               TEXT    NULL,
    DefaultPackageListFormKey        TEXT    NULL,
    CrimeFactionFormKey              TEXT    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_NPC_FormKey ON NPC (FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE ActorValueInformation
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ModKey_Name       TEXT    NOT NULL,
    FormKey_ModKey_Type       INTEGER NOT NULL,
    FormKey_ModKey_FileName   TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    Name                      TEXT    NULL,
    Abbreviation              TEXT    NULL,
    ContextNotes              TEXT    NULL,
    DefaultValue              REAL    NULL,
    Flags                     TEXT    NULL,
    Type                      TEXT    NULL,
    Min                       REAL    NULL,
    Max                       REAL    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_ActorValueInformation_FormKey ON ActorValueInformation (FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE MagicEffect
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ModKey_Name       TEXT    NOT NULL,
    FormKey_ModKey_Type       INTEGER NOT NULL,
    FormKey_ModKey_FileName   TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    Name                      TEXT    NULL,
    Description               TEXT    NULL,
    Flags                     TEXT    NOT NULL,
    CastType                  TEXT    NULL,
    TargetType                TEXT    NULL,
    ActorValue2FormKey        TEXT    NULL,
    ResistValueFormKey        TEXT    NULL,
    PerkToApplyFormKey        TEXT    NULL,
    EquipAbilityFormKey       TEXT    NULL,
    ExplosionFormKey          TEXT    NULL,
    CastingArtFormKey         TEXT    NULL,
    HitEffectArtFormKey       TEXT    NULL,
    HitShaderFormKey          TEXT    NULL,
    ImageSpaceModifierFormKey TEXT    NULL,
    ImpactDataFormKey         TEXT    NULL,
    ProjectileFormKey         TEXT    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_MagicEffect_FormKey ON MagicEffect (FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE Perk
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ModKey_Name       TEXT    NOT NULL,
    FormKey_ModKey_Type       INTEGER NOT NULL,
    FormKey_ModKey_FileName   TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    Name                      TEXT    NULL,
    Description               TEXT    NULL,
    Flags                     TEXT    NOT NULL,
    SkillGroup                TEXT    NULL,
    CrewAssignment            TEXT    NULL,
    PerkIcon                  TEXT    NULL,
    Category                  TEXT    NULL,
    Restriction_ModKey_Name   TEXT    NULL,
    Restriction_ModKey_Type   INTEGER NULL,
    Restriction_ModKey_FileName TEXT  NULL,
    Restriction_FormKey_ID    INTEGER NULL,
    Training_ModKey_Name      TEXT    NULL,
    Training_ModKey_Type      INTEGER NULL,
    Training_ModKey_FileName  TEXT    NULL,
    Training_FormKey_ID       INTEGER NULL,
    MajorFlags                TEXT    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK ((Restriction_ModKey_Name IS NULL AND Restriction_ModKey_Type IS NULL AND Restriction_ModKey_FileName IS NULL AND Restriction_FormKey_ID IS NULL) OR (Restriction_ModKey_Name IS NOT NULL AND Restriction_ModKey_Type IS NOT NULL AND Restriction_ModKey_FileName IS NOT NULL AND Restriction_FormKey_ID IS NOT NULL)),
    CHECK ((Training_ModKey_Name IS NULL AND Training_ModKey_Type IS NULL AND Training_ModKey_FileName IS NULL AND Training_FormKey_ID IS NULL) OR (Training_ModKey_Name IS NOT NULL AND Training_ModKey_Type IS NOT NULL AND Training_ModKey_FileName IS NOT NULL AND Training_FormKey_ID IS NOT NULL)),
    CHECK (Restriction_FormKey_ID IS NULL OR Restriction_FormKey_ID >= 0),
    CHECK (Training_FormKey_ID IS NULL OR Training_FormKey_ID >= 0)
);

CREATE INDEX IX_Perk_FormKey ON Perk (FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE PerkRanks
(
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    Rank_Index              INTEGER NOT NULL,
    Description             TEXT    NULL,
    UnknownStatic_ModKey_Name TEXT  NULL,
    UnknownStatic_ModKey_Type INTEGER NULL,
    UnknownStatic_ModKey_FileName TEXT NULL,
    UnknownStatic_FormKey_ID INTEGER NULL,
    ConditionCount          INTEGER NOT NULL,
    ActivityCount           INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES Perk (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Rank_Index >= 0),
    CHECK ((UnknownStatic_ModKey_Name IS NULL AND UnknownStatic_ModKey_Type IS NULL AND UnknownStatic_ModKey_FileName IS NULL AND UnknownStatic_FormKey_ID IS NULL) OR (UnknownStatic_ModKey_Name IS NOT NULL AND UnknownStatic_ModKey_Type IS NOT NULL AND UnknownStatic_ModKey_FileName IS NOT NULL AND UnknownStatic_FormKey_ID IS NOT NULL)),
    CHECK (UnknownStatic_FormKey_ID IS NULL OR UnknownStatic_FormKey_ID >= 0),
    CHECK (ConditionCount >= 0),
    CHECK (ActivityCount >= 0)
);

CREATE INDEX IX_PerkRanks_Perk ON PerkRanks (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_PerkRanks_RankIndex ON PerkRanks (Rank_Index);

CREATE TABLE PerkRankEffects
(
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    Rank_Index              INTEGER NOT NULL,
    Effect_Index            INTEGER NOT NULL,
    MutagenObjectType       TEXT    NOT NULL,
    Rank                    INTEGER NOT NULL,
    Priority                INTEGER NOT NULL,
    PerkEntryID             INTEGER NULL,
    Flags                   TEXT    NULL,
    ButtonLabel             TEXT    NULL,
    ConditionCount          INTEGER NOT NULL,
    EntryPoint              TEXT    NULL,
    PerkConditionTabCount   INTEGER NULL,
    Modification            TEXT    NULL,
    Value                   REAL    NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index, Effect_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index)
        REFERENCES PerkRanks (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Rank_Index >= 0),
    CHECK (Effect_Index >= 0),
    CHECK (Rank >= 0),
    CHECK (Priority >= 0),
    CHECK (PerkEntryID IS NULL OR PerkEntryID >= 0),
    CHECK (ConditionCount >= 0),
    CHECK (PerkConditionTabCount IS NULL OR PerkConditionTabCount >= 0)
);

CREATE INDEX IX_PerkRankEffects_PerkRank ON PerkRankEffects (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index);
CREATE INDEX IX_PerkRankEffects_EffectIndex ON PerkRankEffects (Effect_Index);

CREATE TABLE PerkBackgroundSkills
(
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    Skill_ModKey_Name       TEXT    NOT NULL,
    Skill_ModKey_Type       INTEGER NOT NULL,
    Skill_ModKey_FileName   TEXT    NOT NULL,
    Skill_FormKey_ID        INTEGER NOT NULL,
    Skill_Index             INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Skill_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES Perk (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Skill_FormKey_ID >= 0),
    CHECK (Skill_Index >= 0)
);

CREATE INDEX IX_PerkBackgroundSkills_Perk ON PerkBackgroundSkills (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_PerkBackgroundSkills_Skill_FormKey ON PerkBackgroundSkills (Skill_ModKey_Name, Skill_ModKey_Type, Skill_ModKey_FileName, Skill_FormKey_ID);
CREATE INDEX IX_PerkBackgroundSkills_SkillIndex ON PerkBackgroundSkills (Skill_Index);

CREATE TABLE ScriptingAdapters
(
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    RecordType              TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    Name                    TEXT    NOT NULL,
    Script_Index            INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Name),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Script_Index >= 0)
);

CREATE INDEX IX_ScriptingAdapters_RecordLookup ON ScriptingAdapters (RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_ScriptingAdapters_ScriptIndex ON ScriptingAdapters (Script_Index);

CREATE TABLE ScriptingAdapterProperties
(
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    RecordType              TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    ScriptingAdapter_Name   TEXT    NOT NULL,
    Property_Index          INTEGER NOT NULL,
    Name                    TEXT    NOT NULL,
    MutagenObjectType       TEXT    NOT NULL,
    Data_Bool               INTEGER NULL,
    Data_Int                INTEGER NULL,
    Data_Float              REAL    NULL,
    Data_String             TEXT    NULL,
    Object_ModKey_Name      TEXT    NULL,
    Object_ModKey_Type      INTEGER NULL,
    Object_ModKey_FileName  TEXT    NULL,
    Object_FormKey_ID       INTEGER NULL,
    Object_Alias            INTEGER NULL,
    Object_Unused           INTEGER NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name, Property_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name)
        REFERENCES ScriptingAdapters (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Name) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Property_Index >= 0),
    CHECK (Data_Bool IS NULL OR Data_Bool IN (0, 1)),
    CHECK (Object_FormKey_ID IS NULL OR Object_FormKey_ID >= 0)
);

CREATE INDEX IX_ScriptingAdapterProperties_RecordLookup ON ScriptingAdapterProperties (RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_ScriptingAdapterProperties_PropertyIndex ON ScriptingAdapterProperties (Property_Index);
CREATE INDEX IX_ScriptingAdapterProperties_ObjectLookup ON ScriptingAdapterProperties (Object_ModKey_Name, Object_ModKey_Type, Object_ModKey_FileName, Object_FormKey_ID);

CREATE TABLE ScriptingAdapterPropertyListItems
(
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    RecordType              TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    ScriptingAdapter_Name   TEXT    NOT NULL,
    Property_Index          INTEGER NOT NULL,
    ListItem_Index          INTEGER NOT NULL,
    MutagenObjectType       TEXT    NOT NULL,
    Data_Bool               INTEGER NULL,
    Data_Int                INTEGER NULL,
    Data_Float              REAL    NULL,
    Data_String             TEXT    NULL,
    Object_ModKey_Name      TEXT    NULL,
    Object_ModKey_Type      INTEGER NULL,
    Object_ModKey_FileName  TEXT    NULL,
    Object_FormKey_ID       INTEGER NULL,
    Object_Alias            INTEGER NULL,
    Object_Unused           INTEGER NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name, Property_Index, ListItem_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name, Property_Index)
        REFERENCES ScriptingAdapterProperties (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name, Property_Index) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Property_Index >= 0),
    CHECK (ListItem_Index >= 0),
    CHECK (Data_Bool IS NULL OR Data_Bool IN (0, 1)),
    CHECK (Object_FormKey_ID IS NULL OR Object_FormKey_ID >= 0)
);

CREATE INDEX IX_ScriptingAdapterPropertyListItems_RecordLookup ON ScriptingAdapterPropertyListItems (RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_ScriptingAdapterPropertyListItems_ListItemIndex ON ScriptingAdapterPropertyListItems (ListItem_Index);
CREATE INDEX IX_ScriptingAdapterPropertyListItems_ObjectLookup ON ScriptingAdapterPropertyListItems (Object_ModKey_Name, Object_ModKey_Type, Object_ModKey_FileName, Object_FormKey_ID);
