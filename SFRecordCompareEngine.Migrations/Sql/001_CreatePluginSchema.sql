CREATE TABLE IF NOT EXISTS Plugins
(
    ModKey_Name             TEXT                NOT NULL,
    ModKey_Type             INTEGER             NOT NULL,
    ModKey_FileName         TEXT COLLATE NOCASE NOT NULL,
    GameRelease             TEXT                NOT NULL,
    LoadOrderIndex          INTEGER             NOT NULL,
    PluginFileName          TEXT                NOT NULL,
    PluginPath              TEXT                NOT NULL,
    Enabled                 INTEGER             NOT NULL DEFAULT 1,
    ExistsOnDisk            INTEGER             NOT NULL DEFAULT 1,
    ImportState             TEXT                NOT NULL DEFAULT 'Current',
    HeaderFlags             INTEGER             NOT NULL,
    FormVersion             INTEGER             NOT NULL,
    Author                  TEXT                NOT NULL,
    Branch                  TEXT                NOT NULL,
    InteriorCellCount       INTEGER             NOT NULL,
    SourceLastWriteUtcTicks INTEGER             NOT NULL,
    SourceFileSizeBytes     INTEGER             NOT NULL,
    LastCheckedUtc          TEXT                NOT NULL,
    LastImportedUtc         TEXT                NULL,
    InvalidatedAtUtc        TEXT                NULL,

    PRIMARY KEY (ModKey_Name, ModKey_FileName, ModKey_Type),
    CHECK (Enabled IN (0, 1)),
    CHECK (ExistsOnDisk IN (0, 1)),
    CHECK (ImportState IN ('Current', 'Changed', 'Missing', 'Failed', 'Unsupported'))
);

CREATE INDEX IF NOT EXISTS IX_Plugins_LoadOrderIndex
    ON Plugins (LoadOrderIndex);

CREATE INDEX IF NOT EXISTS IX_Plugins_PluginFileName
    ON Plugins (PluginFileName);

CREATE INDEX IF NOT EXISTS IX_Plugins_ImportState
    ON Plugins (ImportState);

CREATE INDEX IF NOT EXISTS IX_Plugins_SourceFingerprint
    ON Plugins (SourceLastWriteUtcTicks, SourceFileSizeBytes);

CREATE TABLE IF NOT EXISTS PluginMasterReferences
(
    ModKey_Name            TEXT                NOT NULL,
    ModKey_Type            INTEGER             NOT NULL,
    ModKey_FileName        TEXT COLLATE NOCASE NOT NULL,
    Parent_ModKey_Name     TEXT                NOT NULL,
    Parent_ModKey_Type     INTEGER             NOT NULL,
    Parent_ModKey_FileName TEXT COLLATE NOCASE NOT NULL,
    MasterReferenceIndex   INTEGER             NOT NULL,
    ParentLoadOrderIndex   INTEGER             NOT NULL,
    ImportedAtUtc          TEXT                NOT NULL,

    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, Parent_ModKey_Name, Parent_ModKey_Type, Parent_ModKey_FileName),

    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName)
        REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName)
        ON DELETE CASCADE,

    FOREIGN KEY (Parent_ModKey_Name, Parent_ModKey_Type, Parent_ModKey_FileName)
        REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName)
        ON DELETE RESTRICT,

    CHECK (MasterReferenceIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_PluginMasterReferences_ModKey_ParentLoadOrderIndex
    ON PluginMasterReferences (ModKey_Name, ModKey_Type, ModKey_FileName, ParentLoadOrderIndex);

CREATE INDEX IF NOT EXISTS IX_PluginMasterReferences_ParentModKey
    ON PluginMasterReferences (Parent_ModKey_Name, Parent_ModKey_Type, Parent_ModKey_FileName);

CREATE UNIQUE INDEX IF NOT EXISTS UX_PluginMasterReferences_ModKey_MasterReferenceIndex
    ON PluginMasterReferences (ModKey_Name, ModKey_Type, ModKey_FileName, MasterReferenceIndex);

CREATE TABLE IF NOT EXISTS RecordHeader
(
    ModKey                    TEXT COLLATE NOCASE NOT NULL,
    FormID                    TEXT                NOT NULL,
    RecordType                TEXT                NOT NULL,
    FormKey                   TEXT                NOT NULL,
    EditorID                  TEXT                NULL,
    PluginFileName            TEXT                NOT NULL,
    FormVersion               INTEGER             NULL,
    StarfieldMajorRecordFlags INTEGER             NULL,
    Version2                  INTEGER             NULL,
    VersionControl            TEXT                NULL,
    ImportedAtUtc             TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey)
        REFERENCES Plugins (ModKey)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_RecordHeader_FormKey
    ON RecordHeader (FormKey);

CREATE INDEX IF NOT EXISTS IX_RecordHeader_EditorID
    ON RecordHeader (EditorID);

CREATE INDEX IF NOT EXISTS IX_RecordHeader_RecordType_FormID
    ON RecordHeader (RecordType, FormID);

CREATE INDEX IF NOT EXISTS IX_RecordHeader_RecordType_EditorID
    ON RecordHeader (RecordType, EditorID);

CREATE TABLE IF NOT EXISTS FormList
(
    ModKey           TEXT COLLATE NOCASE NOT NULL,
    FormID           TEXT                NOT NULL,
    AddToListFormKey TEXT                NULL,
    ImportedAtUtc    TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_FormList_AddToListFormKey
    ON FormList (AddToListFormKey);

CREATE TABLE IF NOT EXISTS FormListItem
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    ItemIndex     INTEGER             NOT NULL,
    ItemFormKey   TEXT                NOT NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES FormList (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_FormListItem_ModKey_FormID
    ON FormListItem (ModKey, FormID);

CREATE INDEX IF NOT EXISTS IX_FormListItem_ItemFormKey
    ON FormListItem (ItemFormKey);

CREATE TABLE IF NOT EXISTS GameSetting
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    SettingType   TEXT                NULL,
    TitleString   TEXT                NULL,
    Data          TEXT                NULL,
    RawData       REAL                NULL,
    XALG          INTEGER             NULL,
    IsCompressed  INTEGER             NULL,
    IsDeleted     INTEGER             NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (IsCompressed IS NULL OR IsCompressed IN (0, 1)),
    CHECK (IsDeleted IS NULL OR IsDeleted IN (0, 1))
);

CREATE INDEX IF NOT EXISTS IX_GameSetting_SettingType
    ON GameSetting (SettingType);

CREATE INDEX IF NOT EXISTS IX_GameSetting_Data
    ON GameSetting (Data);

CREATE TABLE IF NOT EXISTS Cell
(
    ModKey                  TEXT COLLATE NOCASE NOT NULL,
    FormID                  TEXT                NOT NULL,
    Name                    TEXT                NULL,
    Flags                   TEXT                NULL,
    MajorFlags              TEXT                NULL,
    LightingTemplateFormKey TEXT                NULL,
    ImageSpaceFormKey       TEXT                NULL,
    LocationFormKey         TEXT                NULL,
    WaterFormKey            TEXT                NULL,
    WaterHeight             TEXT                NULL,
    IsLinkedRefTransient    INTEGER             NULL,
    ImportedAtUtc           TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (IsLinkedRefTransient IS NULL OR IsLinkedRefTransient IN (0, 1))
);

CREATE INDEX IF NOT EXISTS IX_Cell_LocationFormKey
    ON Cell (LocationFormKey);

CREATE TABLE IF NOT EXISTS CellGroupLocation
(
    ModKey               TEXT COLLATE NOCASE NOT NULL,
    CellFormID           TEXT                NOT NULL,
    LocationIndex        INTEGER             NOT NULL,
    LocationKind         TEXT                NOT NULL,
    WorldspaceFormID     TEXT                NULL,
    BlockNumber          INTEGER             NULL,
    SubBlockNumber       INTEGER             NULL,
    BlockX               INTEGER             NULL,
    BlockY               INTEGER             NULL,
    SubBlockX            INTEGER             NULL,
    SubBlockY            INTEGER             NULL,
    CellIndex            INTEGER             NULL,
    BlockGroupType       TEXT                NULL,
    SubBlockGroupType    TEXT                NULL,
    BlockLastModified    INTEGER             NULL,
    SubBlockLastModified INTEGER             NULL,
    BlockUnknown         INTEGER             NULL,
    SubBlockUnknown      INTEGER             NULL,
    ImportedAtUtc        TEXT                NOT NULL,

    PRIMARY KEY (ModKey, CellFormID, LocationIndex),

    FOREIGN KEY (ModKey, CellFormID)
        REFERENCES Cell (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(CellFormID) = 6),
    CHECK (CellFormID = upper(CellFormID)),
    CHECK (LocationIndex >= 0),
    CHECK (LocationKind IN ('InteriorCell', 'WorldspaceTopCell', 'WorldspaceSubCell')),
    CHECK (CellIndex IS NULL OR CellIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_CellGroupLocation_CellFormID
    ON CellGroupLocation (CellFormID);

CREATE INDEX IF NOT EXISTS IX_CellGroupLocation_WorldspaceFormID
    ON CellGroupLocation (WorldspaceFormID);

CREATE TABLE IF NOT EXISTS CellPlacedRecord
(
    ModKey         TEXT COLLATE NOCASE NOT NULL,
    CellFormID     TEXT                NOT NULL,
    PlacementGroup TEXT                NOT NULL,
    ItemIndex      INTEGER             NOT NULL,
    PlacedFormKey  TEXT                NULL,
    BaseFormKey    TEXT                NULL,
    EditorID       TEXT                NULL,
    Position       TEXT                NULL,
    Rotation       TEXT                NULL,
    IsDeleted      INTEGER             NULL,
    ImportedAtUtc  TEXT                NOT NULL,

    PRIMARY KEY (ModKey, CellFormID, PlacementGroup, ItemIndex),

    FOREIGN KEY (ModKey, CellFormID)
        REFERENCES Cell (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(CellFormID) = 6),
    CHECK (CellFormID = upper(CellFormID)),
    CHECK (PlacementGroup IN ('Persistent', 'Temporary')),
    CHECK (ItemIndex >= 0),
    CHECK (IsDeleted IS NULL OR IsDeleted IN (0, 1))
);

CREATE INDEX IF NOT EXISTS IX_CellPlacedRecord_PlacedFormKey
    ON CellPlacedRecord (PlacedFormKey);

CREATE INDEX IF NOT EXISTS IX_CellPlacedRecord_BaseFormKey
    ON CellPlacedRecord (BaseFormKey);

CREATE TABLE IF NOT EXISTS Worldspace
(
    ModKey                  TEXT COLLATE NOCASE NOT NULL,
    FormID                  TEXT                NOT NULL,
    Name                    TEXT                NULL,
    ParentWorldspaceFormKey TEXT                NULL,
    ClimateFormKey          TEXT                NULL,
    WaterFormKey            TEXT                NULL,
    TopCellFormKey          TEXT                NULL,
    WorldMapCellOffset      TEXT                NULL,
    WorldMapOffsetScale     TEXT                NULL,
    ImportedAtUtc           TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Worldspace_TopCellFormKey
    ON Worldspace (TopCellFormKey);

CREATE TABLE IF NOT EXISTS Keyword
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    Name          TEXT                NULL,
    Color         TEXT                NULL,
    KeywordType   TEXT                NULL,
    FNAM          TEXT                NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Keyword_Name
    ON Keyword (Name);

CREATE INDEX IF NOT EXISTS IX_Keyword_KeywordType
    ON Keyword (KeywordType);

CREATE TABLE IF NOT EXISTS Faction
(
    ModKey                         TEXT COLLATE NOCASE NOT NULL,
    FormID                         TEXT                NOT NULL,
    Name                           TEXT                NULL,
    KeywordFormKey                 TEXT                NULL,
    Flags                          TEXT                NULL,
    CrimeValuesArrest              INTEGER             NULL,
    CrimeValuesMurder              INTEGER             NULL,
    CrimeValuesAssault             INTEGER             NULL,
    CrimeValuesTrespass            INTEGER             NULL,
    CrimeValuesPickpocket          INTEGER             NULL,
    CrimeValuesStealMultiplier     REAL                NULL,
    CrimeValuesEscape              INTEGER             NULL,
    CrimeValuesPiracy              INTEGER             NULL,
    CrimeValuesSmuggleMultiplier   REAL                NULL,
    VendorValuesStartHour          INTEGER             NULL,
    VendorValuesEndHour            INTEGER             NULL,
    VendorValuesBuysStolenItems    INTEGER             NULL,
    VendorValuesBuysNonStolenItems INTEGER             NULL,
    ImportedAtUtc                  TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Faction_Name
    ON Faction (Name);

CREATE INDEX IF NOT EXISTS IX_Faction_KeywordFormKey
    ON Faction (KeywordFormKey);

CREATE TABLE IF NOT EXISTS FactionRelation
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    ItemIndex     INTEGER             NOT NULL,
    TargetFormKey TEXT                NOT NULL,
    Reaction      TEXT                NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES Faction (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_FactionRelation_TargetFormKey
    ON FactionRelation (TargetFormKey);

CREATE TABLE IF NOT EXISTS Message
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    Name          TEXT                NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Message_Name
    ON Message (Name);

CREATE TABLE IF NOT EXISTS GameplayOptionsGroup
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    Name          TEXT                NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_GameplayOptionsGroup_Name
    ON GameplayOptionsGroup (Name);

CREATE TABLE IF NOT EXISTS Static
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    Name          TEXT                NULL,
    ObjectBounds  TEXT                NULL,
    Model         TEXT                NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Static_Name
    ON Static (Name);

CREATE TABLE IF NOT EXISTS StaticCollection
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    Name          TEXT                NULL,
    ObjectBounds  TEXT                NULL,
    Model         TEXT                NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_StaticCollection_Name
    ON StaticCollection (Name);

CREATE TABLE IF NOT EXISTS Activator
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    Name          TEXT                NULL,
    ObjectBounds  TEXT                NULL,
    Model         TEXT                NULL,
    Destructible  TEXT                NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Activator_Name
    ON Activator (Name);

CREATE TABLE IF NOT EXISTS ActivatorKeyword
(
    ModKey         TEXT COLLATE NOCASE NOT NULL,
    FormID         TEXT                NOT NULL,
    ItemIndex      INTEGER             NOT NULL,
    KeywordFormKey TEXT                NOT NULL,
    ImportedAtUtc  TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES Activator (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_ActivatorKeyword_KeywordFormKey
    ON ActivatorKeyword (KeywordFormKey);

CREATE TABLE IF NOT EXISTS MiscItem
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    Name          TEXT                NULL,
    ObjectBounds  TEXT                NULL,
    Model         TEXT                NULL,
    Destructible  TEXT                NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_MiscItem_Name
    ON MiscItem (Name);

CREATE TABLE IF NOT EXISTS MiscItemKeyword
(
    ModKey         TEXT COLLATE NOCASE NOT NULL,
    FormID         TEXT                NOT NULL,
    ItemIndex      INTEGER             NOT NULL,
    KeywordFormKey TEXT                NOT NULL,
    ImportedAtUtc  TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES MiscItem (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_MiscItemKeyword_KeywordFormKey
    ON MiscItemKeyword (KeywordFormKey);

CREATE TABLE IF NOT EXISTS GameplayOption
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    Name          TEXT                NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_GameplayOption_Name
    ON GameplayOption (Name);

CREATE TABLE IF NOT EXISTS GameplayOptionKeyword
(
    ModKey         TEXT COLLATE NOCASE NOT NULL,
    FormID         TEXT                NOT NULL,
    ItemIndex      INTEGER             NOT NULL,
    KeywordFormKey TEXT                NOT NULL,
    ImportedAtUtc  TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES GameplayOption (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_GameplayOptionKeyword_KeywordFormKey
    ON GameplayOptionKeyword (KeywordFormKey);

CREATE TABLE IF NOT EXISTS MagicEffect
(
    ModKey        TEXT COLLATE NOCASE NOT NULL,
    FormID        TEXT                NOT NULL,
    Name          TEXT                NULL,
    ImportedAtUtc TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_MagicEffect_Name
    ON MagicEffect (Name);

CREATE TABLE IF NOT EXISTS MagicEffectKeyword
(
    ModKey         TEXT COLLATE NOCASE NOT NULL,
    FormID         TEXT                NOT NULL,
    ItemIndex      INTEGER             NOT NULL,
    KeywordFormKey TEXT                NOT NULL,
    ImportedAtUtc  TEXT                NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES MagicEffect (ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_MagicEffectKeyword_KeywordFormKey
    ON MagicEffectKeyword (KeywordFormKey);
