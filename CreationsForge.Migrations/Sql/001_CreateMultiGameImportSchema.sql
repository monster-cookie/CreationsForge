CREATE TABLE Games -- noqa: 
(
    Game               TEXT NOT NULL,
    DisplayName        TEXT NOT NULL,
    InstallationFolder TEXT NULL,
    DataFolder         TEXT NULL,
    ImportedAtUTC      TEXT NOT NULL,
    PRIMARY KEY (Game),
    CHECK (Game IN ('Starfield', 'Fallout4', 'Skyrim'))
);

CREATE TABLE Plugins
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    LoadOrderIndex          INTEGER NOT NULL,
    Enabled                 INTEGER NOT NULL DEFAULT 1,
    ExistsOnDisk            INTEGER NOT NULL DEFAULT 1,
    ImportState             TEXT    NOT NULL DEFAULT 'Current',
    HeaderFlags             INTEGER NOT NULL,
    FormVersion             INTEGER NOT NULL,
    Author                  TEXT    NULL,
    Description             TEXT    NULL,
    RecordCount             INTEGER NOT NULL DEFAULT 0,
    SourceLastWriteUTCTicks INTEGER NOT NULL,
    SourceFileSizeBytes     INTEGER NOT NULL,
    LastCheckedUTC          TEXT    NOT NULL,
    LastImportedUTC         TEXT    NULL,
    InvalidatedAtUTC        TEXT    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName),
    FOREIGN KEY (Game) REFERENCES Games (Game) ON DELETE CASCADE,
    CHECK (Enabled IN (0, 1)),
    CHECK (ExistsOnDisk IN (0, 1)),
    CHECK (ImportState IN ('Current', 'Changed', 'Missing', 'Failed', 'Unsupported')),
    CHECK (RecordCount >= 0)
);

CREATE INDEX IX_Plugins_Game_LoadOrderIndex ON Plugins (Game, LoadOrderIndex);
CREATE INDEX IX_Plugins_Game_ImportState ON Plugins (Game, ImportState);

CREATE TABLE StarfieldPlugins
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    Branch                  TEXT    NOT NULL,
    InteriorCellCount       INTEGER NULL,
    Intv                    INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (Game = 'Starfield'),
    CHECK (InteriorCellCount IS NULL OR InteriorCellCount >= 0)
);

CREATE TABLE Fallout4Plugins
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    Incc                    INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (Game = 'Fallout4'),
    CHECK (Incc IS NULL OR Incc >= 0)
);

CREATE TABLE SkyrimPlugins
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    Incc                    INTEGER NULL,
    Intv                    INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (Game = 'Skyrim'),
    CHECK (Incc IS NULL OR Incc >= 0)
);

CREATE TABLE PluginMasterReferences
(
    Game                   TEXT    NOT NULL,
    Master_ModKey_Name     TEXT    NOT NULL,
    Master_ModKey_Type     INTEGER NOT NULL,
    Master_ModKey_FileName TEXT    NOT NULL,
    Plugin_ModKey_Name     TEXT    NOT NULL,
    Plugin_ModKey_Type     INTEGER NOT NULL,
    Plugin_ModKey_FileName TEXT    NOT NULL,
    ImportedAtUTC          TEXT    NOT NULL,
    PRIMARY KEY (Game, Master_ModKey_Name, Master_ModKey_Type, Master_ModKey_FileName, Plugin_ModKey_Name, Plugin_ModKey_Type, Plugin_ModKey_FileName),
    FOREIGN KEY (Game, Master_ModKey_Name, Master_ModKey_Type, Master_ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, Plugin_ModKey_Name, Plugin_ModKey_Type, Plugin_ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE
);

CREATE INDEX IX_PluginMasterReferences_MasterModKey ON PluginMasterReferences (Game, Master_ModKey_Name, Master_ModKey_Type, Master_ModKey_FileName);
CREATE INDEX IX_PluginMasterReferences_PluginModKey ON PluginMasterReferences (Game, Plugin_ModKey_Name, Plugin_ModKey_Type, Plugin_ModKey_FileName);

CREATE TABLE RecordInstances
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    RecordType              TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    EditorID                TEXT    NOT NULL,
    FormVersion             INTEGER NOT NULL,
    MajorRecordFlags        INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    UNIQUE (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_RecordInstances_FormKey ON RecordInstances (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE FormLists
(
    Game                      TEXT    NOT NULL,
    ModKey_Name               TEXT    NOT NULL,
    ModKey_Type               INTEGER NOT NULL,
    ModKey_FileName           TEXT    NOT NULL,
    FormKey_ModKey_Name       TEXT    NOT NULL,
    FormKey_ModKey_Type       INTEGER NOT NULL,
    FormKey_ModKey_FileName   TEXT    NOT NULL,
    FormKey_ID                INTEGER NOT NULL,
    EditorID                  TEXT    NOT NULL,
    FormVersion               INTEGER NOT NULL,
    MajorRecordFlags          INTEGER NOT NULL,
    ImportedAtUTC             TEXT    NOT NULL,
    AddToList_ModKey_Name     TEXT    NULL,
    AddToList_ModKey_Type     INTEGER NULL,
    AddToList_ModKey_FileName TEXT    NULL,
    AddToList_FormKey_ID      INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK ((AddToList_ModKey_Name IS NULL AND AddToList_ModKey_Type IS NULL AND AddToList_ModKey_FileName IS NULL AND AddToList_FormKey_ID IS NULL) OR (AddToList_ModKey_Name IS NOT NULL AND AddToList_ModKey_Type IS NOT NULL AND AddToList_ModKey_FileName IS NOT NULL AND AddToList_FormKey_ID IS NOT NULL)),
    CHECK (AddToList_FormKey_ID IS NULL OR AddToList_FormKey_ID >= 0)
);

CREATE INDEX IX_FormLists_FormKey ON FormLists (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE FormListItems
(
    Game                    TEXT    NOT NULL,
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
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Item_ModKey_Name, Item_ModKey_Type, Item_ModKey_FileName, Item_FormKey_ID, Item_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES FormLists (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Item_FormKey_ID >= 0),
    CHECK (Item_Index >= 0)
);

CREATE INDEX IX_FormListItems_FormList ON FormListItems (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_FormListItems_Item_FormKey ON FormListItems (Game, Item_ModKey_Name, Item_ModKey_Type, Item_ModKey_FileName, Item_FormKey_ID);

CREATE TABLE GameSettings
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    EditorID                TEXT    NOT NULL,
    FormVersion             INTEGER NOT NULL,
    MajorRecordFlags        INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    SettingType             TEXT    NULL,
    Data                    TEXT    NULL,
    NumericData             REAL    NULL,
    IntegerData             INTEGER NULL,
    BooleanData             INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (BooleanData IS NULL OR BooleanData IN (0, 1))
);

CREATE INDEX IX_GameSettings_FormKey ON GameSettings (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE Globals
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    EditorID                TEXT    NOT NULL,
    FormVersion             INTEGER NOT NULL,
    MajorRecordFlags        INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    Data                    REAL    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_Globals_FormKey ON Globals (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE Keywords
(
    Game                         TEXT    NOT NULL,
    ModKey_Name                  TEXT    NOT NULL,
    ModKey_Type                  INTEGER NOT NULL,
    ModKey_FileName              TEXT    NOT NULL,
    FormKey_ModKey_Name          TEXT    NOT NULL,
    FormKey_ModKey_Type          INTEGER NOT NULL,
    FormKey_ModKey_FileName      TEXT    NOT NULL,
    FormKey_ID                   INTEGER NOT NULL,
    EditorID                     TEXT    NOT NULL,
    FormVersion                  INTEGER NOT NULL,
    MajorRecordFlags             INTEGER NOT NULL,
    ImportedAtUTC                TEXT    NOT NULL,
    Name                         TEXT    NULL,
    Color                        TEXT    NOT NULL,
    Type                         TEXT    NOT NULL,
    Notes                        TEXT    NULL,
    FlashLinkageName             TEXT    NULL,
    AttractionRule_ModKey_Name   TEXT    NULL,
    AttractionRule_ModKey_Type   INTEGER NULL,
    AttractionRule_ModKey_FileName TEXT  NULL,
    AttractionRule_FormKey_ID    INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_Keywords_FormKey ON Keywords (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE ActorValueInformation
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    EditorID                TEXT    NOT NULL,
    FormVersion             INTEGER NOT NULL,
    MajorRecordFlags        INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    Name                    TEXT    NULL,
    Abbreviation            TEXT    NULL,
    ContextNotes            TEXT    NULL,
    DefaultValue            REAL    NULL,
    Flags                   TEXT    NULL,
    Type                    TEXT    NULL,
    Min                     REAL    NULL,
    Max                     REAL    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_ActorValueInformation_FormKey ON ActorValueInformation (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE NPCs
(
    Game                                      TEXT    NOT NULL,
    ModKey_Name                               TEXT    NOT NULL,
    ModKey_Type                               INTEGER NOT NULL,
    ModKey_FileName                           TEXT    NOT NULL,
    FormKey_ModKey_Name                       TEXT    NOT NULL,
    FormKey_ModKey_Type                       INTEGER NOT NULL,
    FormKey_ModKey_FileName                   TEXT    NOT NULL,
    FormKey_ID                                INTEGER NOT NULL,
    EditorID                                  TEXT    NOT NULL,
    FormVersion                               INTEGER NOT NULL,
    MajorRecordFlags                          INTEGER NOT NULL,
    ImportedAtUTC                             TEXT    NOT NULL,
    Name                                      TEXT    NULL,
    ShortName                                 TEXT    NULL,
    LongName                                  TEXT    NULL,
    DispositionBase                           INTEGER NOT NULL,
    Aggression                                TEXT    NOT NULL,
    Confidence                                TEXT    NOT NULL,
    EnergyLevel                               INTEGER NOT NULL,
    Responsibility                            TEXT    NOT NULL,
    Assistance                                TEXT    NOT NULL,
    GearedUpWeapons                           INTEGER NOT NULL,
    HeightMin                                 REAL    NOT NULL,
    HeightMax                                 REAL    NOT NULL,
    SkinToneIndex                             INTEGER NULL,
    Pronoun                                   TEXT    NULL,
    Voice_ModKey_Name                         TEXT    NULL,
    Voice_ModKey_Type                         INTEGER NULL,
    Voice_ModKey_FileName                     TEXT    NULL,
    Voice_FormKey_ID                          INTEGER NULL,
    Race_ModKey_Name                          TEXT    NULL,
    Race_ModKey_Type                          INTEGER NULL,
    Race_ModKey_FileName                      TEXT    NULL,
    Race_FormKey_ID                           INTEGER NULL,
    CombatOverridePackageList_ModKey_Name     TEXT    NULL,
    CombatOverridePackageList_ModKey_Type     INTEGER NULL,
    CombatOverridePackageList_ModKey_FileName TEXT    NULL,
    CombatOverridePackageList_FormKey_ID      INTEGER NULL,
    CombatStyle_ModKey_Name                   TEXT    NULL,
    CombatStyle_ModKey_Type                   INTEGER NULL,
    CombatStyle_ModKey_FileName               TEXT    NULL,
    CombatStyle_FormKey_ID                    INTEGER NULL,
    DefaultPackageList_ModKey_Name            TEXT    NULL,
    DefaultPackageList_ModKey_Type            INTEGER NULL,
    DefaultPackageList_ModKey_FileName        TEXT    NULL,
    DefaultPackageList_FormKey_ID             INTEGER NULL,
    CrimeFaction_ModKey_Name                  TEXT    NULL,
    CrimeFaction_ModKey_Type                  INTEGER NULL,
    CrimeFaction_ModKey_FileName              TEXT    NULL,
    CrimeFaction_FormKey_ID                   INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_NPCs_FormKey ON NPCs (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE MagicEffects
(
    Game                              TEXT    NOT NULL,
    ModKey_Name                       TEXT    NOT NULL,
    ModKey_Type                       INTEGER NOT NULL,
    ModKey_FileName                   TEXT    NOT NULL,
    FormKey_ModKey_Name               TEXT    NOT NULL,
    FormKey_ModKey_Type               INTEGER NOT NULL,
    FormKey_ModKey_FileName           TEXT    NOT NULL,
    FormKey_ID                        INTEGER NOT NULL,
    EditorID                          TEXT    NOT NULL,
    FormVersion                       INTEGER NOT NULL,
    MajorRecordFlags                  INTEGER NOT NULL,
    ImportedAtUTC                     TEXT    NOT NULL,
    Name                              TEXT    NULL,
    Description                       TEXT    NULL,
    Flags                             TEXT    NOT NULL,
    CastType                          TEXT    NULL,
    TargetType                        TEXT    NULL,
    ActorValue2_ModKey_Name           TEXT    NULL,
    ActorValue2_ModKey_Type           INTEGER NULL,
    ActorValue2_ModKey_FileName       TEXT    NULL,
    ActorValue2_FormKey_ID            INTEGER NULL,
    ResistValue_ModKey_Name           TEXT    NULL,
    ResistValue_ModKey_Type           INTEGER NULL,
    ResistValue_ModKey_FileName       TEXT    NULL,
    ResistValue_FormKey_ID            INTEGER NULL,
    PerkToApply_ModKey_Name           TEXT    NULL,
    PerkToApply_ModKey_Type           INTEGER NULL,
    PerkToApply_ModKey_FileName       TEXT    NULL,
    PerkToApply_FormKey_ID            INTEGER NULL,
    EquipAbility_ModKey_Name          TEXT    NULL,
    EquipAbility_ModKey_Type          INTEGER NULL,
    EquipAbility_ModKey_FileName      TEXT    NULL,
    EquipAbility_FormKey_ID           INTEGER NULL,
    Explosion_ModKey_Name             TEXT    NULL,
    Explosion_ModKey_Type             INTEGER NULL,
    Explosion_ModKey_FileName         TEXT    NULL,
    Explosion_FormKey_ID              INTEGER NULL,
    CastingArt_ModKey_Name            TEXT    NULL,
    CastingArt_ModKey_Type            INTEGER NULL,
    CastingArt_ModKey_FileName        TEXT    NULL,
    CastingArt_FormKey_ID             INTEGER NULL,
    HitEffectArt_ModKey_Name          TEXT    NULL,
    HitEffectArt_ModKey_Type          INTEGER NULL,
    HitEffectArt_ModKey_FileName      TEXT    NULL,
    HitEffectArt_FormKey_ID           INTEGER NULL,
    HitShader_ModKey_Name             TEXT    NULL,
    HitShader_ModKey_Type             INTEGER NULL,
    HitShader_ModKey_FileName         TEXT    NULL,
    HitShader_FormKey_ID              INTEGER NULL,
    ImageSpaceModifier_ModKey_Name    TEXT    NULL,
    ImageSpaceModifier_ModKey_Type    INTEGER NULL,
    ImageSpaceModifier_ModKey_FileName TEXT   NULL,
    ImageSpaceModifier_FormKey_ID     INTEGER NULL,
    ImpactData_ModKey_Name            TEXT    NULL,
    ImpactData_ModKey_Type            INTEGER NULL,
    ImpactData_ModKey_FileName        TEXT    NULL,
    ImpactData_FormKey_ID             INTEGER NULL,
    Projectile_ModKey_Name            TEXT    NULL,
    Projectile_ModKey_Type            INTEGER NULL,
    Projectile_ModKey_FileName        TEXT    NULL,
    Projectile_FormKey_ID             INTEGER NULL,
    Archetype                         TEXT    NULL,
    UnknownFloat3                     REAL    NULL,
    UnknownInt2                       INTEGER NULL,
    Unknown                           TEXT    NULL,
    Unknown2                          TEXT    NULL,
    DataTypeState                     TEXT    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_MagicEffects_FormKey ON MagicEffects (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE MiscObjects
(
    Game                                  TEXT    NOT NULL,
    ModKey_Name                           TEXT    NOT NULL,
    ModKey_Type                           INTEGER NOT NULL,
    ModKey_FileName                       TEXT    NOT NULL,
    FormKey_ModKey_Name                   TEXT    NOT NULL,
    FormKey_ModKey_Type                   INTEGER NOT NULL,
    FormKey_ModKey_FileName               TEXT    NOT NULL,
    FormKey_ID                            INTEGER NOT NULL,
    EditorID                              TEXT    NOT NULL,
    FormVersion                           INTEGER NOT NULL,
    MajorRecordFlags                      INTEGER NOT NULL,
    ImportedAtUTC                         TEXT    NOT NULL,
    Name                                  TEXT    NULL,
    ShortName                             TEXT    NULL,
    Value                                 INTEGER NULL,
    Weight                                REAL    NULL,
    DirtinessScale                        REAL    NULL,
    FeaturedItemMessage_ModKey_Name       TEXT    NULL,
    FeaturedItemMessage_ModKey_Type       INTEGER NULL,
    FeaturedItemMessage_ModKey_FileName   TEXT    NULL,
    FeaturedItemMessage_FormKey_ID        INTEGER NULL,
    FLAG                                  TEXT    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_MiscObjects_FormKey ON MiscObjects (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE Perks
(
    Game                            TEXT    NOT NULL,
    ModKey_Name                     TEXT    NOT NULL,
    ModKey_Type                     INTEGER NOT NULL,
    ModKey_FileName                 TEXT    NOT NULL,
    FormKey_ModKey_Name             TEXT    NOT NULL,
    FormKey_ModKey_Type             INTEGER NOT NULL,
    FormKey_ModKey_FileName         TEXT    NOT NULL,
    FormKey_ID                      INTEGER NOT NULL,
    EditorID                        TEXT    NOT NULL,
    FormVersion                     INTEGER NOT NULL,
    MajorRecordFlags                INTEGER NOT NULL,
    ImportedAtUTC                   TEXT    NOT NULL,
    Name                            TEXT    NULL,
    Description                     TEXT    NULL,
    Flags                           TEXT    NOT NULL,
    SkillGroup                      TEXT    NULL,
    CrewAssignment                  TEXT    NULL,
    PerkIcon                        TEXT    NULL,
    Category                        TEXT    NULL,
    Restriction_ModKey_Name         TEXT    NULL,
    Restriction_ModKey_Type         INTEGER NULL,
    Restriction_ModKey_FileName     TEXT    NULL,
    Restriction_FormKey_ID          INTEGER NULL,
    Training_ModKey_Name            TEXT    NULL,
    Training_ModKey_Type            INTEGER NULL,
    Training_ModKey_FileName        TEXT    NULL,
    Training_FormKey_ID             INTEGER NULL,
    MajorFlags                      TEXT    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_Perks_FormKey ON Perks (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

CREATE TABLE RecordKeywords
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    RecordType              TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    Keyword_ModKey_Name     TEXT    NOT NULL,
    Keyword_ModKey_Type     INTEGER NOT NULL,
    Keyword_ModKey_FileName TEXT    NOT NULL,
    Keyword_FormKey_ID      INTEGER NOT NULL,
    Keyword_Index           INTEGER NOT NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Keyword_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Keyword_FormKey_ID >= 0),
    CHECK (Keyword_Index >= 0)
);

CREATE TABLE PerkRanks
(
    Game                            TEXT    NOT NULL,
    ModKey_Name                     TEXT    NOT NULL,
    ModKey_Type                     INTEGER NOT NULL,
    ModKey_FileName                 TEXT    NOT NULL,
    FormKey_ModKey_Name             TEXT    NOT NULL,
    FormKey_ModKey_Type             INTEGER NOT NULL,
    FormKey_ModKey_FileName         TEXT    NOT NULL,
    FormKey_ID                      INTEGER NOT NULL,
    Rank_Index                      INTEGER NOT NULL,
    Description                     TEXT    NULL,
    UnknownStatic_ModKey_Name       TEXT    NULL,
    UnknownStatic_ModKey_Type       INTEGER NULL,
    UnknownStatic_ModKey_FileName   TEXT    NULL,
    UnknownStatic_FormKey_ID        INTEGER NULL,
    ConditionCount                  INTEGER NOT NULL,
    ActivityCount                   INTEGER NOT NULL,
    ImportedAtUTC                   TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES Perks (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Rank_Index >= 0)
);

CREATE TABLE PerkRankEffects
(
    Game                    TEXT    NOT NULL,
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
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index, Effect_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index)
        REFERENCES PerkRanks (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Rank_Index >= 0),
    CHECK (Effect_Index >= 0)
);

CREATE TABLE PerkBackgroundSkills
(
    Game                    TEXT    NOT NULL,
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
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Skill_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES Perks (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Skill_FormKey_ID >= 0),
    CHECK (Skill_Index >= 0)
);

CREATE TABLE Models
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    RecordType              TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    ModelSlot               TEXT    NOT NULL,
    ModelGender             TEXT    NOT NULL,
    File                    TEXT    NULL,
    TextureFileHashes       TEXT    NULL,
    LightLayer              INTEGER NULL,
    Flags                   TEXT    NULL,
    ColorRemappingIndex     REAL    NULL,
    FlagsVestigial          TEXT    NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ModelSlot, ModelGender),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (ModelSlot <> '')
);

CREATE TABLE ModelMaterialSwaps
(
    Game                            TEXT    NOT NULL,
    ModKey_Name                     TEXT    NOT NULL,
    ModKey_Type                     INTEGER NOT NULL,
    ModKey_FileName                 TEXT    NOT NULL,
    RecordType                      TEXT    NOT NULL,
    FormKey_ModKey_Name             TEXT    NOT NULL,
    FormKey_ModKey_Type             INTEGER NOT NULL,
    FormKey_ModKey_FileName         TEXT    NOT NULL,
    FormKey_ID                      INTEGER NOT NULL,
    ModelSlot                       TEXT    NOT NULL,
    ModelGender                     TEXT    NOT NULL,
    MaterialSwap_ModKey_Name        TEXT    NOT NULL,
    MaterialSwap_ModKey_Type        INTEGER NOT NULL,
    MaterialSwap_ModKey_FileName    TEXT    NOT NULL,
    MaterialSwap_FormKey_ID         INTEGER NOT NULL,
    MaterialSwap_Index              INTEGER NOT NULL,
    ImportedAtUTC                   TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ModelSlot, ModelGender, MaterialSwap_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ModelSlot, ModelGender)
        REFERENCES Models (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ModelSlot, ModelGender) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (ModelSlot <> ''),
    CHECK (MaterialSwap_FormKey_ID >= 0),
    CHECK (MaterialSwap_Index >= 0)
);

CREATE TABLE RecordSounds
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    RecordType              TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    SoundSlot               TEXT    NOT NULL,
    Sound_Index             INTEGER NOT NULL,
    Start                   TEXT    NULL,
    Versioning              TEXT    NULL,
    Unknown                 TEXT    NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, SoundSlot, Sound_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (SoundSlot <> ''),
    CHECK (Sound_Index >= 0)
);

CREATE TABLE ScriptingAdapters
(
    Game                    TEXT    NOT NULL,
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
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Name),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Script_Index >= 0)
);

CREATE TABLE ScriptingAdapterProperties
(
    Game                    TEXT    NOT NULL,
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
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name, Property_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name)
        REFERENCES ScriptingAdapters (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Name) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Property_Index >= 0),
    CHECK (Data_Bool IS NULL OR Data_Bool IN (0, 1)),
    CHECK (Object_FormKey_ID IS NULL OR Object_FormKey_ID >= 0)
);

CREATE TABLE ScriptingAdapterPropertyListItems
(
    Game                    TEXT    NOT NULL,
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
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name, Property_Index, ListItem_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name, Property_Index)
        REFERENCES ScriptingAdapterProperties (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name, Property_Index) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Property_Index >= 0),
    CHECK (ListItem_Index >= 0),
    CHECK (Data_Bool IS NULL OR Data_Bool IN (0, 1)),
    CHECK (Object_FormKey_ID IS NULL OR Object_FormKey_ID >= 0)
);

CREATE VIEW StarfieldPluginDetails AS
SELECT
    p.Game,
    p.ModKey_Name,
    p.ModKey_Type,
    p.ModKey_FileName,
    p.LoadOrderIndex,
    p.Enabled,
    p.ExistsOnDisk,
    p.ImportState,
    p.HeaderFlags,
    p.FormVersion,
    p.Author,
    p.Description,
    p.RecordCount,
    p.SourceLastWriteUTCTicks,
    p.SourceFileSizeBytes,
    p.LastCheckedUTC,
    p.LastImportedUTC,
    p.InvalidatedAtUTC,
    sf.Branch,
    sf.InteriorCellCount,
    sf.Intv
FROM Plugins p
INNER JOIN StarfieldPlugins sf
    ON sf.Game = p.Game
    AND sf.ModKey_Name = p.ModKey_Name
    AND sf.ModKey_Type = p.ModKey_Type
    AND sf.ModKey_FileName = p.ModKey_FileName
WHERE p.Game = 'Starfield';

CREATE VIEW Fallout4PluginDetails AS
SELECT
    p.Game,
    p.ModKey_Name,
    p.ModKey_Type,
    p.ModKey_FileName,
    p.LoadOrderIndex,
    p.Enabled,
    p.ExistsOnDisk,
    p.ImportState,
    p.HeaderFlags,
    p.FormVersion,
    p.Author,
    p.Description,
    p.RecordCount,
    p.SourceLastWriteUTCTicks,
    p.SourceFileSizeBytes,
    p.LastCheckedUTC,
    p.LastImportedUTC,
    p.InvalidatedAtUTC,
    fo4.Incc
FROM Plugins p
INNER JOIN Fallout4Plugins fo4
    ON fo4.Game = p.Game
    AND fo4.ModKey_Name = p.ModKey_Name
    AND fo4.ModKey_Type = p.ModKey_Type
    AND fo4.ModKey_FileName = p.ModKey_FileName
WHERE p.Game = 'Fallout4';

CREATE VIEW SkyrimPluginDetails AS
SELECT
    p.Game,
    p.ModKey_Name,
    p.ModKey_Type,
    p.ModKey_FileName,
    p.LoadOrderIndex,
    p.Enabled,
    p.ExistsOnDisk,
    p.ImportState,
    p.HeaderFlags,
    p.FormVersion,
    p.Author,
    p.Description,
    p.RecordCount,
    p.SourceLastWriteUTCTicks,
    p.SourceFileSizeBytes,
    p.LastCheckedUTC,
    p.LastImportedUTC,
    p.InvalidatedAtUTC,
    sky.Incc,
    sky.Intv
FROM Plugins p
INNER JOIN SkyrimPlugins sky
    ON sky.Game = p.Game
    AND sky.ModKey_Name = p.ModKey_Name
    AND sky.ModKey_Type = p.ModKey_Type
    AND sky.ModKey_FileName = p.ModKey_FileName
WHERE p.Game = 'Skyrim';
