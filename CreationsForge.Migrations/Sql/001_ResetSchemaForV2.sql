-- CreationsForge v2 reset baseline schema
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
    ImportMessage           TEXT    NULL,
    ImportDetails           TEXT    NULL,
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
    CHECK (ImportState IN ('Current', 'Changed', 'PartiallyImported', 'Missing', 'Failed', 'Unsupported')),
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
    Version2                INTEGER NULL,
    VersionControl          INTEGER NULL,
    DataType                TEXT    NOT NULL,
    Data                    TEXT    NULL,
    FloatData               REAL    NULL,
    IntegerData             INTEGER NULL,
    UnsignedIntegerData     INTEGER NULL,
    BooleanData             INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (DataType IN ('Boolean', 'Float', 'Integer', 'String', 'UnsignedInteger')),
    CHECK (UnsignedIntegerData IS NULL OR UnsignedIntegerData >= 0),
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
    Version2                INTEGER NULL,
    VersionControl          INTEGER NULL,
    Name                    TEXT    NULL,
    Abbreviation            TEXT    NULL,
    Description             TEXT    NULL,
    CNAM                    TEXT    NULL,
    Skill_ImproveMult       REAL    NULL,
    Skill_ImproveOffset     REAL    NULL,
    Skill_UseMult           REAL    NULL,
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

CREATE TABLE MiscItems
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

CREATE INDEX IX_MiscItems_FormKey ON MiscItems (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);

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

CREATE TABLE KeywordMappings
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

CREATE TABLE SoundMappings
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

CREATE TABLE AssetArchiveFiles
(
    Game                    TEXT    NOT NULL,
    DataFolder              TEXT    NOT NULL,
    ArchivePath             TEXT    NOT NULL,
    ArchiveFileName         TEXT    NOT NULL,
    ArchiveExtension        TEXT    NOT NULL,
    ArchiveType             TEXT    NOT NULL,
    SourceLastWriteUTCTicks INTEGER NOT NULL,
    SourceFileSizeBytes     INTEGER NOT NULL,
    IndexedAtUTC            TEXT    NOT NULL,
    PRIMARY KEY (Game, ArchivePath),
    CHECK (Game IN ('Starfield', 'Fallout4', 'Skyrim')),
    CHECK (SourceLastWriteUTCTicks >= 0),
    CHECK (SourceFileSizeBytes >= 0)
);

CREATE INDEX IX_AssetArchiveFiles_Game_DataFolder ON AssetArchiveFiles (Game, DataFolder);

CREATE TABLE AssetArchiveEntries
(
    Game                TEXT    NOT NULL,
    ArchivePath         TEXT    NOT NULL,
    NormalizedEntryPath TEXT    NOT NULL,
    RootFolder          TEXT    NOT NULL,
    Extension           TEXT    NOT NULL,
    PackedSize          INTEGER NOT NULL,
    UnpackedSize        INTEGER NOT NULL,
    PRIMARY KEY (Game, ArchivePath, NormalizedEntryPath),
    FOREIGN KEY (Game, ArchivePath) REFERENCES AssetArchiveFiles (Game, ArchivePath) ON DELETE CASCADE,
    CHECK (PackedSize >= 0),
    CHECK (UnpackedSize >= 0)
);

CREATE INDEX IX_AssetArchiveEntries_Game_NormalizedEntryPath ON AssetArchiveEntries (Game, NormalizedEntryPath COLLATE NOCASE);
CREATE INDEX IX_AssetArchiveEntries_Game_RootFolder_Extension ON AssetArchiveEntries (Game, RootFolder, Extension);

CREATE TABLE Statics
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
    Version2                INTEGER NULL,
    ObjectBounds_First      TEXT    NULL,
    ObjectBounds_Second     TEXT    NULL,
    MaxAngle                REAL    NULL,
    UnknownDNAMFloat        REAL    NULL,
    LeafAmplitude           REAL    NULL,
    LeafFrequency           REAL    NULL,
    Unused                  TEXT    NULL,
    DNAMDataTypeState       TEXT    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE TABLE RawRecordPayloads
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
    PayloadSlot             TEXT    NOT NULL,
    Payload_Index           INTEGER NOT NULL,
    PayloadType             TEXT    NOT NULL,
    SourcePath              TEXT    NULL,
    PayloadValue            TEXT    NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, PayloadSlot, Payload_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (PayloadSlot <> ''),
    CHECK (Payload_Index >= 0),
    CHECK (PayloadType <> '')
);

CREATE TABLE Containers
(
    Game                        TEXT    NOT NULL,
    ModKey_Name                 TEXT    NOT NULL,
    ModKey_Type                 INTEGER NOT NULL,
    ModKey_FileName             TEXT    NOT NULL,
    FormKey_ModKey_Name         TEXT    NOT NULL,
    FormKey_ModKey_Type         INTEGER NOT NULL,
    FormKey_ModKey_FileName     TEXT    NOT NULL,
    FormKey_ID                  INTEGER NOT NULL,
    EditorID                    TEXT    NOT NULL,
    FormVersion                 INTEGER NOT NULL,
    MajorRecordFlags            INTEGER NOT NULL,
    ImportedAtUTC               TEXT    NOT NULL,
    Version2                    INTEGER NULL,
    ObjectBounds_First          TEXT    NULL,
    ObjectBounds_Second         TEXT    NULL,
    Name                        TEXT    NULL,
    Flags                       TEXT    NULL,
    MajorFlags                  TEXT    NULL,
    NativeTerminal_ModKey_Name     TEXT    NULL,
    NativeTerminal_ModKey_Type     INTEGER NULL,
    NativeTerminal_ModKey_FileName TEXT    NULL,
    NativeTerminal_FormKey_ID      INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE TABLE ContainerItems
(
    Game                    TEXT    NOT NULL,
    ModKey_Name             TEXT    NOT NULL,
    ModKey_Type             INTEGER NOT NULL,
    ModKey_FileName         TEXT    NOT NULL,
    FormKey_ModKey_Name     TEXT    NOT NULL,
    FormKey_ModKey_Type     INTEGER NOT NULL,
    FormKey_ModKey_FileName TEXT    NOT NULL,
    FormKey_ID              INTEGER NOT NULL,
    Item_Index              INTEGER NOT NULL,
    Item_ModKey_Name        TEXT    NOT NULL,
    Item_ModKey_Type        INTEGER NOT NULL,
    Item_ModKey_FileName    TEXT    NOT NULL,
    Item_FormKey_ID         INTEGER NOT NULL,
    Count                   INTEGER NULL,
    ImportedAtUTC           TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Item_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES Containers (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Item_Index >= 0),
    CHECK (Item_FormKey_ID >= 0)
);

CREATE INDEX IX_RecordInstances_Game_RecordType_Plugin ON RecordInstances (Game, RecordType, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_RecordInstances_Game_RecordType_FormKey ON RecordInstances (Game, RecordType, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_FormLists_Game_Plugin ON FormLists (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_FormLists_Game_FormKey_Collated ON FormLists (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_GameSettings_Game_Plugin ON GameSettings (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_GameSettings_Game_FormKey_Collated ON GameSettings (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Globals_Game_Plugin ON Globals (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Globals_Game_FormKey_Collated ON Globals (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Keywords_Game_Plugin ON Keywords (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Keywords_Game_FormKey_Collated ON Keywords (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ActorValueInformation_Game_Plugin ON ActorValueInformation (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ActorValueInformation_Game_FormKey_Collated ON ActorValueInformation (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_NPCs_Game_Plugin ON NPCs (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_NPCs_Game_FormKey_Collated ON NPCs (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_MagicEffects_Game_Plugin ON MagicEffects (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_MagicEffects_Game_FormKey_Collated ON MagicEffects (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_MiscItems_Game_Plugin ON MiscItems (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_MiscItems_Game_FormKey_Collated ON MiscItems (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Perks_Game_Plugin ON Perks (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Perks_Game_FormKey_Collated ON Perks (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Statics_FormKey ON Statics (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_Statics_Game_Plugin ON Statics (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Statics_Game_FormKey_Collated ON Statics (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_RawRecordPayloads_Game_Record_FormKey ON RawRecordPayloads (Game, RecordType, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Containers_Game_Plugin ON Containers (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Containers_Game_FormKey_Collated ON Containers (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ContainerItems_Game_FormKey ON ContainerItems (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

CREATE TABLE Books
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
    Version2                        INTEGER NULL,
    VersionControl                  INTEGER NULL,
    ObjectBounds_First              TEXT    NULL,
    ObjectBounds_Second             TEXT    NULL,
    Transforms_Inventory_ModKey_Name TEXT   NULL,
    Transforms_Inventory_ModKey_Type INTEGER NULL,
    Transforms_Inventory_ModKey_FileName TEXT NULL,
    Transforms_Inventory_FormKey_ID INTEGER NULL,
    InventoryArt_ModKey_Name        TEXT    NULL,
    InventoryArt_ModKey_Type        INTEGER NULL,
    InventoryArt_ModKey_FileName    TEXT    NULL,
    InventoryArt_FormKey_ID         INTEGER NULL,
    PreviewTransform_ModKey_Name    TEXT    NULL,
    PreviewTransform_ModKey_Type    INTEGER NULL,
    PreviewTransform_ModKey_FileName TEXT   NULL,
    PreviewTransform_FormKey_ID     INTEGER NULL,
    FeaturedItemMessage_ModKey_Name TEXT    NULL,
    FeaturedItemMessage_ModKey_Type INTEGER NULL,
    FeaturedItemMessage_ModKey_FileName TEXT NULL,
    FeaturedItemMessage_FormKey_ID  INTEGER NULL,
    XALG                            INTEGER NULL,
    Name                            TEXT    NULL,
    Text                            TEXT    NULL,
    Value                           INTEGER NULL,
    Weight                          REAL    NULL,
    Flags                           TEXT    NULL,
    Teaches_MutagenObjectType       TEXT    NULL,
    Teaches_Perk_ModKey_Name        TEXT    NULL,
    Teaches_Perk_ModKey_Type        INTEGER NULL,
    Teaches_Perk_ModKey_FileName    TEXT    NULL,
    Teaches_Perk_FormKey_ID         INTEGER NULL,
    Teaches_RawContent              TEXT    NULL,
    DataSlateType                   TEXT    NULL,
    Description                     TEXT    NULL,
    DataSlateHeaderLeft             TEXT    NULL,
    DataSlateHeaderRight            TEXT    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_Books_FormKey ON Books (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_Books_Game_Plugin ON Books (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Books_Game_FormKey_Collated ON Books (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

CREATE TABLE Doors
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
    Version2                        INTEGER NULL,
    ObjectBounds_First              TEXT    NULL,
    ObjectBounds_Second             TEXT    NULL,
    Name                            TEXT    NULL,
    Flags                           TEXT    NULL,
    NativeTerminal_ModKey_Name      TEXT    NULL,
    NativeTerminal_ModKey_Type      INTEGER NULL,
    NativeTerminal_ModKey_FileName  TEXT    NULL,
    NativeTerminal_FormKey_ID       INTEGER NULL,
    SoundLevel                      TEXT    NULL,
    FacingAxisOverride              TEXT    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_Doors_FormKey ON Doors (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_Doors_Game_Plugin ON Doors (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Doors_Game_FormKey_Collated ON Doors (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

CREATE TABLE Terminals
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    EditorID                            TEXT    NOT NULL,
    FormVersion                         INTEGER NOT NULL,
    MajorRecordFlags                    INTEGER NOT NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    Version2                            INTEGER NULL,
    ObjectBounds_First                  TEXT    NULL,
    ObjectBounds_Second                 TEXT    NULL,
    Menu_ModKey_Name                    TEXT    NULL,
    Menu_ModKey_Type                    INTEGER NULL,
    Menu_ModKey_FileName                TEXT    NULL,
    Menu_FormKey_ID                     INTEGER NULL,
    Background                          TEXT    NULL,
    Name                                TEXT    NULL,
    PNAM                                TEXT    NULL,
    FNAM                                TEXT    NULL,
    JNAM                                TEXT    NULL,
    MarkerFlags                         INTEGER NULL,
    GNAM                                TEXT    NULL,
    WorkbenchData                       TEXT    NULL,
    FurnitureTemplate_ModKey_Name       TEXT    NULL,
    FurnitureTemplate_ModKey_Type       INTEGER NULL,
    FurnitureTemplate_ModKey_FileName   TEXT    NULL,
    FurnitureTemplate_FormKey_ID        INTEGER NULL,
    MarkerModel                         TEXT    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_Terminals_FormKey ON Terminals (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_Terminals_Game_Plugin ON Terminals (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Terminals_Game_FormKey_Collated ON Terminals (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

CREATE TABLE TerminalMarkerParameters
(
    Game                            TEXT    NOT NULL,
    ModKey_Name                     TEXT    NOT NULL,
    ModKey_Type                     INTEGER NOT NULL,
    ModKey_FileName                 TEXT    NOT NULL,
    FormKey_ModKey_Name             TEXT    NOT NULL,
    FormKey_ModKey_Type             INTEGER NOT NULL,
    FormKey_ModKey_FileName         TEXT    NOT NULL,
    FormKey_ID                      INTEGER NOT NULL,
    Parameter_Index                 INTEGER NOT NULL,
    Offset                          TEXT    NULL,
    EntryTypes                      TEXT    NULL,
    ExitTypes                       TEXT    NULL,
    ImportedAtUTC                   TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Parameter_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES Terminals (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Parameter_Index >= 0)
);

CREATE INDEX IX_TerminalMarkerParameters_Game_FormKey ON TerminalMarkerParameters (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

CREATE TABLE ConstructibleObjects
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    EditorID                            TEXT    NOT NULL,
    FormVersion                         INTEGER NOT NULL,
    MajorRecordFlags                    INTEGER NOT NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    Version2                            INTEGER NULL,
    Description                         TEXT    NULL,
    CreatedObject_ModKey_Name           TEXT    NULL,
    CreatedObject_ModKey_Type           INTEGER NULL,
    CreatedObject_ModKey_FileName       TEXT    NULL,
    CreatedObject_FormKey_ID            INTEGER NULL,
    WorkbenchKeyword_ModKey_Name        TEXT    NULL,
    WorkbenchKeyword_ModKey_Type        INTEGER NULL,
    WorkbenchKeyword_ModKey_FileName    TEXT    NULL,
    WorkbenchKeyword_FormKey_ID         INTEGER NULL,
    CreatedObjectCount                  INTEGER NULL,
    AmountProduced                      INTEGER NULL,
    MenuSortOrder                       INTEGER NULL,
    LearnMethod                         TEXT    NULL,
    Flags                               TEXT    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (CreatedObjectCount IS NULL OR CreatedObjectCount >= 0),
    CHECK (AmountProduced IS NULL OR AmountProduced >= 0)
);

CREATE TABLE ConstructibleObjectComponents
(
    Game                            TEXT    NOT NULL,
    ModKey_Name                     TEXT    NOT NULL,
    ModKey_Type                     INTEGER NOT NULL,
    ModKey_FileName                 TEXT    NOT NULL,
    FormKey_ModKey_Name             TEXT    NOT NULL,
    FormKey_ModKey_Type             INTEGER NOT NULL,
    FormKey_ModKey_FileName         TEXT    NOT NULL,
    FormKey_ID                      INTEGER NOT NULL,
    Component_Index                 INTEGER NOT NULL,
    Component_ModKey_Name           TEXT    NOT NULL,
    Component_ModKey_Type           INTEGER NOT NULL,
    Component_ModKey_FileName       TEXT    NOT NULL,
    Component_FormKey_ID            INTEGER NOT NULL,
    Count                           INTEGER NULL,
    ImportedAtUTC                   TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Component_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES ConstructibleObjects (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Component_Index >= 0),
    CHECK (Component_FormKey_ID >= 0),
    CHECK (Count IS NULL OR Count >= 0)
);

CREATE TABLE ConstructibleObjectCategories
(
    Game                            TEXT    NOT NULL,
    ModKey_Name                     TEXT    NOT NULL,
    ModKey_Type                     INTEGER NOT NULL,
    ModKey_FileName                 TEXT    NOT NULL,
    FormKey_ModKey_Name             TEXT    NOT NULL,
    FormKey_ModKey_Type             INTEGER NOT NULL,
    FormKey_ModKey_FileName         TEXT    NOT NULL,
    FormKey_ID                      INTEGER NOT NULL,
    Category_Index                  INTEGER NOT NULL,
    Category_ModKey_Name            TEXT    NOT NULL,
    Category_ModKey_Type            INTEGER NOT NULL,
    Category_ModKey_FileName        TEXT    NOT NULL,
    Category_FormKey_ID             INTEGER NOT NULL,
    ImportedAtUTC                   TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Category_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES ConstructibleObjects (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Category_Index >= 0),
    CHECK (Category_FormKey_ID >= 0)
);

CREATE TABLE ConstructibleObjectRecipeFilters
(
    Game                            TEXT    NOT NULL,
    ModKey_Name                     TEXT    NOT NULL,
    ModKey_Type                     INTEGER NOT NULL,
    ModKey_FileName                 TEXT    NOT NULL,
    FormKey_ModKey_Name             TEXT    NOT NULL,
    FormKey_ModKey_Type             INTEGER NOT NULL,
    FormKey_ModKey_FileName         TEXT    NOT NULL,
    FormKey_ID                      INTEGER NOT NULL,
    RecipeFilter_Index              INTEGER NOT NULL,
    RecipeFilter_ModKey_Name        TEXT    NOT NULL,
    RecipeFilter_ModKey_Type        INTEGER NOT NULL,
    RecipeFilter_ModKey_FileName    TEXT    NOT NULL,
    RecipeFilter_FormKey_ID         INTEGER NOT NULL,
    ImportedAtUTC                   TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, RecipeFilter_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES ConstructibleObjects (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (RecipeFilter_Index >= 0),
    CHECK (RecipeFilter_FormKey_ID >= 0)
);

CREATE TABLE ConditionForms
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    EditorID                            TEXT    NOT NULL,
    FormVersion                         INTEGER NOT NULL,
    MajorRecordFlags                    INTEGER NOT NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    Version2                            INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_ConstructibleObjects_FormKey ON ConstructibleObjects (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_ConstructibleObjects_Game_Plugin ON ConstructibleObjects (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConstructibleObjects_Game_FormKey_Collated ON ConstructibleObjects (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConstructibleObjectComponents_Game_FormKey ON ConstructibleObjectComponents (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConstructibleObjectCategories_Game_FormKey ON ConstructibleObjectCategories (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConstructibleObjectRecipeFilters_Game_FormKey ON ConstructibleObjectRecipeFilters (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConditionForms_FormKey ON ConditionForms (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_ConditionForms_Game_Plugin ON ConditionForms (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConditionForms_Game_FormKey_Collated ON ConditionForms (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE TABLE Classes
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    EditorID                            TEXT    NOT NULL,
    FormVersion                         INTEGER NOT NULL,
    MajorRecordFlags                    INTEGER NOT NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    Version2                            INTEGER NULL,
    Name                                TEXT    NULL,
    Description                         TEXT    NULL,
    Teaches                             TEXT    NULL,
    MaxTrainingLevel                    INTEGER NULL,
    BleedoutDefault                     REAL    NULL,
    VoicePoints                         REAL    NULL,
    Unknown                             REAL    NULL,
    Unknown2                            REAL    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (MaxTrainingLevel IS NULL OR MaxTrainingLevel >= 0)
);

CREATE TABLE ClassProperties
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    Property_Index                      INTEGER NOT NULL,
    ActorValue_ModKey_Name              TEXT    NULL,
    ActorValue_ModKey_Type              INTEGER NULL,
    ActorValue_ModKey_FileName          TEXT    NULL,
    ActorValue_FormKey_ID               INTEGER NULL,
    Value                               REAL    NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Property_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES Classes (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Property_Index >= 0),
    CHECK (ActorValue_FormKey_ID IS NULL OR ActorValue_FormKey_ID >= 0)
);

CREATE TABLE ClassWeights
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    WeightType                          TEXT    NOT NULL,
    Weight_Index                        INTEGER NOT NULL,
    Key                                 TEXT    NOT NULL,
    Value                               REAL    NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, WeightType, Weight_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES Classes (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (WeightType <> ''),
    CHECK (Weight_Index >= 0),
    CHECK (Key <> '')
);

CREATE TABLE Factions
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
    Version2                                  INTEGER NULL,
    Name                                      TEXT    NULL,
    Flags                                     TEXT    NULL,
    FormationRadius                           REAL    NULL,
    Keyword_ModKey_Name                       TEXT    NULL,
    Keyword_ModKey_Type                       INTEGER NULL,
    Keyword_ModKey_FileName                   TEXT    NULL,
    Keyword_FormKey_ID                        INTEGER NULL,
    Herd_ModKey_Name                          TEXT    NULL,
    Herd_ModKey_Type                          INTEGER NULL,
    Herd_ModKey_FileName                      TEXT    NULL,
    Herd_FormKey_ID                           INTEGER NULL,
    VoiceType_ModKey_Name                     TEXT    NULL,
    VoiceType_ModKey_Type                     INTEGER NULL,
    VoiceType_ModKey_FileName                 TEXT    NULL,
    VoiceType_FormKey_ID                      INTEGER NULL,
    SharedCrimeFactionList_ModKey_Name        TEXT    NULL,
    SharedCrimeFactionList_ModKey_Type        INTEGER NULL,
    SharedCrimeFactionList_ModKey_FileName    TEXT    NULL,
    SharedCrimeFactionList_FormKey_ID         INTEGER NULL,
    VendorBuySellList_ModKey_Name             TEXT    NULL,
    VendorBuySellList_ModKey_Type             INTEGER NULL,
    VendorBuySellList_ModKey_FileName         TEXT    NULL,
    VendorBuySellList_FormKey_ID              INTEGER NULL,
    MerchantContainer_ModKey_Name             TEXT    NULL,
    MerchantContainer_ModKey_Type             INTEGER NULL,
    MerchantContainer_ModKey_FileName         TEXT    NULL,
    MerchantContainer_FormKey_ID              INTEGER NULL,
    ExteriorJailMarker_ModKey_Name            TEXT    NULL,
    ExteriorJailMarker_ModKey_Type            INTEGER NULL,
    ExteriorJailMarker_ModKey_FileName        TEXT    NULL,
    ExteriorJailMarker_FormKey_ID             INTEGER NULL,
    FollowerWaitMarker_ModKey_Name            TEXT    NULL,
    FollowerWaitMarker_ModKey_Type            INTEGER NULL,
    FollowerWaitMarker_ModKey_FileName        TEXT    NULL,
    FollowerWaitMarker_FormKey_ID             INTEGER NULL,
    StolenGoodsContainer_ModKey_Name          TEXT    NULL,
    StolenGoodsContainer_ModKey_Type          INTEGER NULL,
    StolenGoodsContainer_ModKey_FileName      TEXT    NULL,
    StolenGoodsContainer_FormKey_ID           INTEGER NULL,
    PlayerInventoryContainer_ModKey_Name      TEXT    NULL,
    PlayerInventoryContainer_ModKey_Type      INTEGER NULL,
    PlayerInventoryContainer_ModKey_FileName  TEXT    NULL,
    PlayerInventoryContainer_FormKey_ID       INTEGER NULL,
    JailOutfit_ModKey_Name                    TEXT    NULL,
    JailOutfit_ModKey_Type                    INTEGER NULL,
    JailOutfit_ModKey_FileName                TEXT    NULL,
    JailOutfit_FormKey_ID                     INTEGER NULL,
    CrimeArrest                               INTEGER NULL,
    CrimeAttackOnSight                        INTEGER NULL,
    CrimeMurder                               INTEGER NULL,
    CrimeAssault                              INTEGER NULL,
    CrimeTrespass                             INTEGER NULL,
    CrimePickpocket                           INTEGER NULL,
    CrimeSteal                                INTEGER NULL,
    CrimeStealMult                            REAL    NULL,
    CrimeEscape                               INTEGER NULL,
    CrimeWerewolf                             INTEGER NULL,
    CrimeUnknown                              INTEGER NULL,
    VendorStartHour                           REAL    NULL,
    VendorEndHour                             REAL    NULL,
    VendorRadius                              INTEGER NULL,
    VendorBuysStolenItems                     INTEGER NULL,
    VendorBuysNonStolenItems                  INTEGER NULL,
    VendorBuySellEverythingNotInList          INTEGER NULL,
    VendorLocationMutagenObjectType           TEXT    NULL,
    VendorLocationType                        TEXT    NULL,
    VendorLocationLink_ModKey_Name            TEXT    NULL,
    VendorLocationLink_ModKey_Type            INTEGER NULL,
    VendorLocationLink_ModKey_FileName        TEXT    NULL,
    VendorLocationLink_FormKey_ID             INTEGER NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE TABLE FactionRelations
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    Relation_Index                      INTEGER NOT NULL,
    Target_ModKey_Name                  TEXT    NULL,
    Target_ModKey_Type                  INTEGER NULL,
    Target_ModKey_FileName              TEXT    NULL,
    Target_FormKey_ID                   INTEGER NULL,
    Reaction                            TEXT    NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Relation_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES Factions (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Relation_Index >= 0),
    CHECK (Target_FormKey_ID IS NULL OR Target_FormKey_ID >= 0)
);

CREATE TABLE FactionRanks
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    Rank_Index                          INTEGER NOT NULL,
    RankNumber                          INTEGER NULL,
    MaleTitle                           TEXT    NULL,
    FemaleTitle                         TEXT    NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Rank_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES Factions (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Rank_Index >= 0)
);

CREATE TABLE ConditionRules
(
    Game                                      TEXT    NOT NULL,
    ModKey_Name                               TEXT    NOT NULL,
    ModKey_Type                               INTEGER NOT NULL,
    ModKey_FileName                           TEXT    NOT NULL,
    RecordType                                TEXT    NOT NULL,
    FormKey_ModKey_Name                       TEXT    NOT NULL,
    FormKey_ModKey_Type                       INTEGER NOT NULL,
    FormKey_ModKey_FileName                   TEXT    NOT NULL,
    FormKey_ID                                INTEGER NOT NULL,
    ConditionSlot                             TEXT    NOT NULL,
    Condition_Index                           INTEGER NOT NULL,
    MutagenObjectType                         TEXT    NOT NULL,
    DataMutagenObjectType                     TEXT    NULL,
    CompareOperator                           TEXT    NULL,
    ComparisonValue                           TEXT    NULL,
    ComparisonValue_ModKey_Name               TEXT    NULL,
    ComparisonValue_ModKey_Type               INTEGER NULL,
    ComparisonValue_ModKey_FileName           TEXT    NULL,
    ComparisonValue_FormKey_ID                INTEGER NULL,
    ImportedAtUTC                             TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ConditionSlot, Condition_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (RecordType <> ''),
    CHECK (FormKey_ID >= 0),
    CHECK (ConditionSlot <> ''),
    CHECK (Condition_Index >= 0),
    CHECK (ComparisonValue_FormKey_ID IS NULL OR ComparisonValue_FormKey_ID >= 0)
);

CREATE TABLE ConditionRuleParameters
(
    Game                                      TEXT    NOT NULL,
    ModKey_Name                               TEXT    NOT NULL,
    ModKey_Type                               INTEGER NOT NULL,
    ModKey_FileName                           TEXT    NOT NULL,
    RecordType                                TEXT    NOT NULL,
    FormKey_ModKey_Name                       TEXT    NOT NULL,
    FormKey_ModKey_Type                       INTEGER NOT NULL,
    FormKey_ModKey_FileName                   TEXT    NOT NULL,
    FormKey_ID                                INTEGER NOT NULL,
    ConditionSlot                             TEXT    NOT NULL,
    Condition_Index                           INTEGER NOT NULL,
    Parameter_Name                            TEXT    NOT NULL,
    ParameterValue                            TEXT    NULL,
    Parameter_ModKey_Name                     TEXT    NULL,
    Parameter_ModKey_Type                     INTEGER NULL,
    Parameter_ModKey_FileName                 TEXT    NULL,
    Parameter_FormKey_ID                      INTEGER NULL,
    ImportedAtUTC                             TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ConditionSlot, Condition_Index, Parameter_Name),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ConditionSlot, Condition_Index)
        REFERENCES ConditionRules (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ConditionSlot, Condition_Index) ON DELETE CASCADE,
    CHECK (RecordType <> ''),
    CHECK (FormKey_ID >= 0),
    CHECK (ConditionSlot <> ''),
    CHECK (Condition_Index >= 0),
    CHECK (Parameter_Name <> ''),
    CHECK (Parameter_FormKey_ID IS NULL OR Parameter_FormKey_ID >= 0)
);

CREATE TABLE Components
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    RecordType                          TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    Component_Index                     INTEGER NOT NULL,
    MutagenObjectType                   TEXT    NOT NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Component_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (RecordType <> ''),
    CHECK (FormKey_ID >= 0),
    CHECK (Component_Index >= 0)
);

CREATE TABLE ComponentItems
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    RecordType                          TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    Component_Index                     INTEGER NOT NULL,
    Item_Index                          INTEGER NOT NULL,
    Unknown1                            REAL    NULL,
    Unknown2                            REAL    NULL,
    Unknown3                            REAL    NULL,
    Unknown4                            REAL    NULL,
    Unknown5                            REAL    NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Component_Index, Item_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Component_Index)
        REFERENCES Components (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Component_Index) ON DELETE CASCADE,
    CHECK (RecordType <> ''),
    CHECK (FormKey_ID >= 0),
    CHECK (Component_Index >= 0),
    CHECK (Item_Index >= 0)
);

CREATE INDEX IX_Classes_FormKey ON Classes (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_Classes_Game_Plugin ON Classes (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Classes_Game_FormKey_Collated ON Classes (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ClassProperties_Game_FormKey ON ClassProperties (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ClassWeights_Game_FormKey ON ClassWeights (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Factions_FormKey ON Factions (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_Factions_Game_Plugin ON Factions (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Factions_Game_FormKey_Collated ON Factions (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_FactionRelations_Game_FormKey ON FactionRelations (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_FactionRanks_Game_FormKey ON FactionRanks (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConditionRules_Game_FormKey ON ConditionRules (Game, RecordType, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConditionRuleParameters_Game_FormKey ON ConditionRuleParameters (Game, RecordType, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Components_Game_FormKey ON Components (Game, RecordType, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ComponentItems_Game_FormKey ON ComponentItems (Game, RecordType, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

CREATE TABLE ActorValueInformationPerkTreeEntries
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    PerkTree_Index                      INTEGER NOT NULL,
    AssociatedSkill_ModKey_Name         TEXT    NULL,
    AssociatedSkill_ModKey_Type         INTEGER NULL,
    AssociatedSkill_ModKey_FileName     TEXT    NULL,
    AssociatedSkill_FormKey_ID          INTEGER NULL,
    FNAM                                TEXT    NULL,
    HorizontalPosition                  REAL    NULL,
    EntryIndex                          INTEGER NULL,
    PerkGridX                           INTEGER NULL,
    PerkGridY                           INTEGER NULL,
    VerticalPosition                    REAL    NULL,
    Perk_ModKey_Name                    TEXT    NULL,
    Perk_ModKey_Type                    INTEGER NULL,
    Perk_ModKey_FileName                TEXT    NULL,
    Perk_FormKey_ID                     INTEGER NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, PerkTree_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES ActorValueInformation (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (PerkTree_Index >= 0),
    CHECK ((AssociatedSkill_ModKey_Name IS NULL AND AssociatedSkill_ModKey_Type IS NULL AND AssociatedSkill_ModKey_FileName IS NULL AND AssociatedSkill_FormKey_ID IS NULL) OR
           (AssociatedSkill_ModKey_Name IS NOT NULL AND AssociatedSkill_ModKey_Type IS NOT NULL AND AssociatedSkill_ModKey_FileName IS NOT NULL AND AssociatedSkill_FormKey_ID IS NOT NULL)),
    CHECK ((Perk_ModKey_Name IS NULL AND Perk_ModKey_Type IS NULL AND Perk_ModKey_FileName IS NULL AND Perk_FormKey_ID IS NULL) OR
           (Perk_ModKey_Name IS NOT NULL AND Perk_ModKey_Type IS NOT NULL AND Perk_ModKey_FileName IS NOT NULL AND Perk_FormKey_ID IS NOT NULL))
);

CREATE INDEX IX_ActorValueInformationPerkTreeEntries_Game_FormKey
    ON ActorValueInformationPerkTreeEntries (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

CREATE TABLE ActorValueInformationPerkTreeConnectionLineIndices
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    PerkTree_Index                      INTEGER NOT NULL,
    ConnectionLine_Index                INTEGER NOT NULL,
    TargetIndex                         INTEGER NOT NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, PerkTree_Index, ConnectionLine_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, PerkTree_Index)
        REFERENCES ActorValueInformationPerkTreeEntries (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, PerkTree_Index) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (PerkTree_Index >= 0),
    CHECK (ConnectionLine_Index >= 0),
    CHECK (TargetIndex >= 0)
);

CREATE INDEX IX_ActorValueInformationPerkTreeConnectionLineIndices_Game_FormKey
    ON ActorValueInformationPerkTreeConnectionLineIndices (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

CREATE TABLE LocalizedStrings
(
    Game                                TEXT    NOT NULL,
    ModKey_Name                         TEXT    NOT NULL,
    ModKey_Type                         INTEGER NOT NULL,
    ModKey_FileName                     TEXT    NOT NULL,
    RecordType                          TEXT    NOT NULL,
    FormKey_ModKey_Name                 TEXT    NOT NULL,
    FormKey_ModKey_Type                 INTEGER NOT NULL,
    FormKey_ModKey_FileName             TEXT    NOT NULL,
    FormKey_ID                          INTEGER NOT NULL,
    SourceField                         TEXT    NOT NULL,
    Language                            TEXT    NOT NULL,
    Value                               TEXT    NOT NULL,
    ImportedAtUTC                       TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, SourceField, Language),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (RecordType <> ''),
    CHECK (FormKey_ID >= 0),
    CHECK (SourceField <> ''),
    CHECK (Language <> '')
);

CREATE INDEX IX_LocalizedStrings_Game_Record_FormKey
    ON LocalizedStrings (Game, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
