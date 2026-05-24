CREATE TABLE IF NOT EXISTS Plugins (
    ModKey TEXT COLLATE NOCASE NOT NULL PRIMARY KEY,
    GameRelease TEXT NOT NULL,
    LoadOrderIndex INTEGER NULL,
    PluginFileName TEXT NOT NULL,
    PluginPath TEXT NULL,
    Enabled INTEGER NOT NULL DEFAULT 1,
    ExistsOnDisk INTEGER NOT NULL DEFAULT 1,
    ImportState TEXT NOT NULL DEFAULT 'Current',
    HeaderFlags INTEGER NULL,
    FormVersion INTEGER NULL,
    Author TEXT NULL,
    Branch TEXT NULL,
    InteriorCellCount INTEGER NULL,
    SourceLastWriteUtcTicks INTEGER NULL,
    SourceFileSizeBytes INTEGER NULL,
    LastCheckedUtc TEXT NOT NULL,
    LastImportedUtc TEXT NULL,
    InvalidatedAtUtc TEXT NULL,

    CHECK (Enabled IN (0, 1)),
    CHECK (ExistsOnDisk IN (0, 1)),
    CHECK (ImportState IN ('Current', 'Changed', 'Missing', 'Failed', 'Unsupported'))
);

CREATE INDEX IF NOT EXISTS IX_Plugins_LoadOrderIndex
ON Plugins(LoadOrderIndex);

CREATE INDEX IF NOT EXISTS IX_Plugins_PluginFileName
ON Plugins(PluginFileName);

CREATE INDEX IF NOT EXISTS IX_Plugins_ImportState
ON Plugins(ImportState);

CREATE INDEX IF NOT EXISTS IX_Plugins_SourceFingerprint
ON Plugins(SourceLastWriteUtcTicks, SourceFileSizeBytes);

CREATE TABLE IF NOT EXISTS PluginMasterReferences (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    ParentModKey TEXT COLLATE NOCASE NOT NULL,
    MasterReferenceIndex INTEGER NOT NULL,
    ParentLoadOrderIndex INTEGER NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, ParentModKey),

    FOREIGN KEY (ModKey)
        REFERENCES Plugins(ModKey)
        ON DELETE CASCADE,

    FOREIGN KEY (ParentModKey)
        REFERENCES Plugins(ModKey)
        ON DELETE RESTRICT,

    CHECK (MasterReferenceIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_PluginMasterReferences_ModKey_ParentLoadOrderIndex
ON PluginMasterReferences(ModKey, ParentLoadOrderIndex);

CREATE INDEX IF NOT EXISTS IX_PluginMasterReferences_ParentModKey
ON PluginMasterReferences(ParentModKey);

CREATE UNIQUE INDEX IF NOT EXISTS UX_PluginMasterReferences_ModKey_MasterReferenceIndex
ON PluginMasterReferences(ModKey, MasterReferenceIndex);

CREATE VIEW IF NOT EXISTS PluginResolutionHierarchy AS
SELECT
    pmr.ModKey AS ChildModKey,
    pmr.ParentModKey AS HierarchyModKey,
    pmr.ParentLoadOrderIndex AS HierarchyLoadOrderIndex,
    pmr.MasterReferenceIndex,
    0 AS IsChild
FROM PluginMasterReferences pmr

UNION ALL

SELECT
    p.ModKey AS ChildModKey,
    p.ModKey AS HierarchyModKey,
    p.LoadOrderIndex AS HierarchyLoadOrderIndex,
    NULL AS MasterReferenceIndex,
    1 AS IsChild
FROM Plugins p;

CREATE TABLE IF NOT EXISTS RecordHeader (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    RecordType TEXT NOT NULL,
    FormKey TEXT NOT NULL,
    EditorID TEXT NULL,
    PluginFileName TEXT NOT NULL,
    FormVersion INTEGER NULL,
    StarfieldMajorRecordFlags INTEGER NULL,
    Version2 INTEGER NULL,
    VersionControl TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey)
        REFERENCES Plugins(ModKey)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_RecordHeader_FormKey
ON RecordHeader(FormKey);

CREATE INDEX IF NOT EXISTS IX_RecordHeader_EditorID
ON RecordHeader(EditorID);

CREATE INDEX IF NOT EXISTS IX_RecordHeader_RecordType_FormID
ON RecordHeader(RecordType, FormID);

CREATE INDEX IF NOT EXISTS IX_RecordHeader_RecordType_EditorID
ON RecordHeader(RecordType, EditorID);

CREATE TABLE IF NOT EXISTS FormList (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    AddToListFormKey TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_FormList_AddToListFormKey
ON FormList(AddToListFormKey);

CREATE TABLE IF NOT EXISTS FormListItem (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    ItemIndex INTEGER NOT NULL,
    ItemFormKey TEXT NOT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES FormList(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_FormListItem_ModKey_FormID
ON FormListItem(ModKey, FormID);

CREATE INDEX IF NOT EXISTS IX_FormListItem_ItemFormKey
ON FormListItem(ItemFormKey);

CREATE TABLE IF NOT EXISTS GameSetting (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    SettingType TEXT NULL,
    TitleString TEXT NULL,
    Data TEXT NULL,
    RawData REAL NULL,
    XALG INTEGER NULL,
    IsCompressed INTEGER NULL,
    IsDeleted INTEGER NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (IsCompressed IS NULL OR IsCompressed IN (0, 1)),
    CHECK (IsDeleted IS NULL OR IsDeleted IN (0, 1))
);

CREATE INDEX IF NOT EXISTS IX_GameSetting_SettingType
ON GameSetting(SettingType);

CREATE INDEX IF NOT EXISTS IX_GameSetting_Data
ON GameSetting(Data);

CREATE TABLE IF NOT EXISTS Cell (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    Flags TEXT NULL,
    MajorFlags TEXT NULL,
    LightingTemplateFormKey TEXT NULL,
    ImageSpaceFormKey TEXT NULL,
    LocationFormKey TEXT NULL,
    WaterFormKey TEXT NULL,
    WaterHeight TEXT NULL,
    IsLinkedRefTransient INTEGER NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (IsLinkedRefTransient IS NULL OR IsLinkedRefTransient IN (0, 1))
);

CREATE INDEX IF NOT EXISTS IX_Cell_LocationFormKey
ON Cell(LocationFormKey);

CREATE TABLE IF NOT EXISTS CellGroupLocation (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    CellFormID TEXT NOT NULL,
    LocationIndex INTEGER NOT NULL,
    LocationKind TEXT NOT NULL,
    WorldspaceFormID TEXT NULL,
    BlockNumber INTEGER NULL,
    SubBlockNumber INTEGER NULL,
    BlockX INTEGER NULL,
    BlockY INTEGER NULL,
    SubBlockX INTEGER NULL,
    SubBlockY INTEGER NULL,
    CellIndex INTEGER NULL,
    BlockGroupType TEXT NULL,
    SubBlockGroupType TEXT NULL,
    BlockLastModified INTEGER NULL,
    SubBlockLastModified INTEGER NULL,
    BlockUnknown INTEGER NULL,
    SubBlockUnknown INTEGER NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, CellFormID, LocationIndex),

    FOREIGN KEY (ModKey, CellFormID)
        REFERENCES Cell(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(CellFormID) = 6),
    CHECK (CellFormID = upper(CellFormID)),
    CHECK (LocationIndex >= 0),
    CHECK (LocationKind IN ('InteriorCell', 'WorldspaceTopCell', 'WorldspaceSubCell')),
    CHECK (CellIndex IS NULL OR CellIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_CellGroupLocation_CellFormID
ON CellGroupLocation(CellFormID);

CREATE INDEX IF NOT EXISTS IX_CellGroupLocation_WorldspaceFormID
ON CellGroupLocation(WorldspaceFormID);

CREATE TABLE IF NOT EXISTS CellPlacedRecord (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    CellFormID TEXT NOT NULL,
    PlacementGroup TEXT NOT NULL,
    ItemIndex INTEGER NOT NULL,
    PlacedFormKey TEXT NULL,
    BaseFormKey TEXT NULL,
    EditorID TEXT NULL,
    Position TEXT NULL,
    Rotation TEXT NULL,
    IsDeleted INTEGER NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, CellFormID, PlacementGroup, ItemIndex),

    FOREIGN KEY (ModKey, CellFormID)
        REFERENCES Cell(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(CellFormID) = 6),
    CHECK (CellFormID = upper(CellFormID)),
    CHECK (PlacementGroup IN ('Persistent', 'Temporary')),
    CHECK (ItemIndex >= 0),
    CHECK (IsDeleted IS NULL OR IsDeleted IN (0, 1))
);

CREATE INDEX IF NOT EXISTS IX_CellPlacedRecord_PlacedFormKey
ON CellPlacedRecord(PlacedFormKey);

CREATE INDEX IF NOT EXISTS IX_CellPlacedRecord_BaseFormKey
ON CellPlacedRecord(BaseFormKey);

CREATE TABLE IF NOT EXISTS Worldspace (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ParentWorldspaceFormKey TEXT NULL,
    ClimateFormKey TEXT NULL,
    WaterFormKey TEXT NULL,
    TopCellFormKey TEXT NULL,
    WorldMapCellOffset TEXT NULL,
    WorldMapOffsetScale TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Worldspace_TopCellFormKey
ON Worldspace(TopCellFormKey);
