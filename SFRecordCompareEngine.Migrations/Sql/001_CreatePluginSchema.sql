/*
 * Plugins table, with composite primary key (ModKey_Name, ModKey_Type, ModKey_FileName)
 */
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
    SourceLastWriteUTCTicks INTEGER NOT NULL,
    SourceFileSizeBytes     INTEGER NOT NULL,
    LastCheckedUTC          TEXT    NOT NULL,
    LastImportedUTC         TEXT    NULL,
    InvalidatedAtUTC        TEXT    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName),
    CHECK (Enabled IN (0, 1)),
    CHECK (ExistsOnDisk IN (0, 1)),
    CHECK (ImportState IN ('Current', 'Changed', 'Missing', 'Failed', 'Unsupported'))
);

CREATE INDEX IX_Plugins_LoadOrderIndex ON Plugins (LoadOrderIndex);

CREATE INDEX IX_Plugins_ImportState ON Plugins (ImportState);

CREATE INDEX IX_Plugins_SourceFingerprint ON Plugins (SourceLastWriteUTCTicks, SourceFileSizeBytes);

/*
 * PluginMasterReferences table, with composite primary key (Master_ModKey_Name,Master_ModKey_Type,Master_ModKey_FileName,Plugin_ModKey_Name,Plugin_ModKey_Type,Plugin_ModKey_FileName)
 */
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

/*
 * FormList table, with composite primary key (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID)
 */
CREATE TABLE FormList
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    -- End Record Header --
    AddToListFormKey          TEXT    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_FormList_FormKey_ID ON FormList (FormKey_ID);

/*
 * Form List Items table, with composite primary key (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID, Item_Index)
 */
CREATE TABLE FormListItems
(
    ModKey_Name          TEXT    NOT NULL,
    ModKey_Type          INTEGER NOT NULL,
    ModKey_FileName      TEXT    NOT NULL,
    FormKey_ID           INTEGER NOT NULL,
    Item_ModKey_Name     TEXT    NOT NULL,
    Item_ModKey_Type     INTEGER NOT NULL,
    Item_ModKey_FileName TEXT    NOT NULL,
    Item_FormKey_ID      INTEGER NOT NULL,
    Item_Index           INTEGER NOT NULL,
    ImportedAtUTC        TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID, Item_ModKey_Name, Item_ModKey_Type, Item_ModKey_FileName, Item_FormKey_ID, Item_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID) REFERENCES FormList (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Item_FormKey_ID >= 0)
);

CREATE INDEX IX_FormListItems_Item_FormKey_ID_ModKey_FormKey_ID ON FormListItems (Item_FormKey_ID, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID);
CREATE INDEX IX_FormListItems_Item_Index ON FormListItems (Item_Index);

/*
 * GameSetting table, with composite primary key (ModKey_Name,ModKey_Type,ModKey_FileName,FormKey_ID)
 */
CREATE TABLE GameSetting
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    -- End Record Header --
    SettingType               TEXT    NULL,
    Data                      TEXT    NULL,
    RawData                   REAL    NULL,
    XALG                      INTEGER NULL,
    IsCompressed              INTEGER NOT NULL,
    IsDeleted                 INTEGER NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (IsCompressed IS NULL OR IsCompressed IN (0, 1)),
    CHECK (IsDeleted IS NULL OR IsDeleted IN (0, 1))
);

CREATE INDEX IX_GameSetting_FormKey_ID ON GameSetting (FormKey_ID);

CREATE TABLE Global
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2                  INTEGER NOT NULL,
    VersionControl            INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    Data                      REAL    NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_Global_FormKey_ID ON Global (FormKey_ID);

CREATE TABLE MiscItem
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
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
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_MiscItem_FormKey_ID ON MiscItem (FormKey_ID);

CREATE TABLE Keyword
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
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
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_Keyword_FormKey_ID ON Keyword (FormKey_ID);

CREATE TABLE NPC
(
    ModKey_Name                      TEXT    NOT NULL,
    ModKey_Type                      INTEGER NOT NULL,
    ModKey_FileName                  TEXT    NOT NULL,
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
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_NPC_FormKey_ID ON NPC (FormKey_ID);

CREATE TABLE ActorValueInformation
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
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
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_ActorValueInformation_FormKey_ID ON ActorValueInformation (FormKey_ID);

CREATE TABLE MagicEffect
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
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
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_MagicEffect_FormKey_ID ON MagicEffect (FormKey_ID);

CREATE TABLE Perk
(
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
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
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);
CREATE INDEX IX_Perk_FormKey_ID ON Perk (FormKey_ID);
