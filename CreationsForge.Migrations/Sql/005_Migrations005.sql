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

CREATE TABLE RecordComponents
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

CREATE TABLE RecordComponentItems
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
        REFERENCES RecordComponents (Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Component_Index) ON DELETE CASCADE,
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
CREATE INDEX IX_RecordComponents_Game_FormKey ON RecordComponents (Game, RecordType, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_RecordComponentItems_Game_FormKey ON RecordComponentItems (Game, RecordType, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

ALTER TABLE Books ADD COLUMN PreviewTransform_ModKey_Name TEXT NULL;
ALTER TABLE Books ADD COLUMN PreviewTransform_ModKey_Type INTEGER NULL;
ALTER TABLE Books ADD COLUMN PreviewTransform_ModKey_FileName TEXT NULL;
ALTER TABLE Books ADD COLUMN PreviewTransform_FormKey_ID INTEGER NULL;

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

INSERT OR REPLACE INTO ConditionRules (
    Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName,
    FormKey_ID, ConditionSlot, Condition_Index, MutagenObjectType, DataMutagenObjectType, CompareOperator, ComparisonValue,
    ComparisonValue_ModKey_Name, ComparisonValue_ModKey_Type, ComparisonValue_ModKey_FileName, ComparisonValue_FormKey_ID, ImportedAtUTC)
SELECT
    Game, ModKey_Name, ModKey_Type, ModKey_FileName, 'CNDF', FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName,
    FormKey_ID, 'Conditions', Condition_Index, MutagenObjectType, DataMutagenObjectType, CompareOperator, ComparisonValue,
    ComparisonValue_ModKey_Name, ComparisonValue_ModKey_Type, ComparisonValue_ModKey_FileName, ComparisonValue_FormKey_ID, ImportedAtUTC
FROM ConditionFormConditions;

INSERT OR REPLACE INTO ConditionRuleParameters (
    Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName,
    FormKey_ID, ConditionSlot, Condition_Index, Parameter_Name, ParameterValue, Parameter_ModKey_Name, Parameter_ModKey_Type,
    Parameter_ModKey_FileName, Parameter_FormKey_ID, ImportedAtUTC)
SELECT
    Game, ModKey_Name, ModKey_Type, ModKey_FileName, 'CNDF', FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName,
    FormKey_ID, 'Conditions', Condition_Index, Parameter_Name, ParameterValue, Parameter_ModKey_Name, Parameter_ModKey_Type,
    Parameter_ModKey_FileName, Parameter_FormKey_ID, ImportedAtUTC
FROM ConditionFormConditionParameters;

DROP TABLE ConditionFormConditionParameters;
DROP TABLE ConditionFormConditions;

UPDATE Plugins
SET ImportState = 'Changed',
    InvalidatedAtUTC = strftime('%Y-%m-%dT%H:%M:%fZ', 'now'),
    ImportMessage = 'Unreleased migration 005 schema changed; reimport required.',
    ImportDetails = 'Migration 005 added shared Class/Faction tables, shared condition/component tables, LocalizedStrings child rows, and Book PreviewTransform columns.'
WHERE ImportState IN ('Current', 'PartiallyImported');
