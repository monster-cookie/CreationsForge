/*
 * Plugins table, with composite primary key (ModKey_Name, ModKey_Type, ModKey_FileName)
 */
CREATE TABLE Plugins (
    ModKey_Name TEXT NOT NULL,
    ModKey_Type INTEGER NOT NULL,
    ModKey_FileName TEXT NOT NULL,
    LoadOrderIndex INTEGER NOT NULL,
    Enabled INTEGER NOT NULL DEFAULT 1,
    ExistsOnDisk INTEGER NOT NULL DEFAULT 1,
    ImportState TEXT NOT NULL DEFAULT 'Current',
    HeaderFlags INTEGER NOT NULL,
    FormVersion INTEGER NOT NULL,
    Author TEXT NOT NULL,
    Branch TEXT NOT NULL,
    InteriorCellCount INTEGER NOT NULL,
    SourceLastWriteUtcTicks INTEGER NOT NULL,
    SourceFileSizeBytes INTEGER NOT NULL,
    LastCheckedUtc TEXT NOT NULL,
    LastImportedUtc TEXT NULL,
    InvalidatedAtUtc TEXT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName),
    CHECK (Enabled IN (0, 1)),
    CHECK (ExistsOnDisk IN (0, 1)),
    CHECK (ImportState IN ('Current', 'Changed', 'Missing', 'Failed', 'Unsupported'))
);

CREATE INDEX IX_Plugins_LoadOrderIndex ON Plugins (LoadOrderIndex);

CREATE INDEX IX_Plugins_ImportState ON Plugins (ImportState);

CREATE INDEX IX_Plugins_SourceFingerprint ON Plugins (SourceLastWriteUtcTicks, SourceFileSizeBytes);

/*
 * PluginMasterReferences table, with composite primary key (ModKey_Name,ModKey_Type,ModKey_FileName,Parent_ModKey_Name,Parent_ModKey_Type,Parent_ModKey_FileName)
 */
CREATE TABLE PluginMasterReferences (
    ModKey_Name TEXT NOT NULL,
    ModKey_Type INTEGER NOT NULL,
    ModKey_FileName TEXT NOT NULL,
    Parent_ModKey_Name TEXT NOT NULL,
    Parent_ModKey_Type INTEGER NOT NULL,
    Parent_ModKey_FileName TEXT NOT NULL,
    MasterReferenceIndex INTEGER NOT NULL,
    ParentLoadOrderIndex INTEGER NOT NULL,
    ImportedAtUTC TEXT NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, Parent_ModKey_Name, Parent_ModKey_Type, Parent_ModKey_FileName),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Parent_ModKey_Name, Parent_ModKey_Type, Parent_ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (MasterReferenceIndex >= 0),
    CHECK (ParentLoadOrderIndex >= 0)
);

CREATE INDEX IX_PluginMasterReferences_ModKey_ParentLoadOrderIndex ON PluginMasterReferences (ModKey_Name, ModKey_Type, ModKey_FileName, ParentLoadOrderIndex);

CREATE INDEX IX_PluginMasterReferences_ParentModKey ON PluginMasterReferences (Parent_ModKey_Name, Parent_ModKey_Type, Parent_ModKey_FileName);

CREATE UNIQUE INDEX UX_PluginMasterReferences_ModKey_MasterReferenceIndex ON PluginMasterReferences (ModKey_Name, ModKey_Type, ModKey_FileName, MasterReferenceIndex);

/*
 * FormList table, with composite primary key (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID)
 */
CREATE TABLE FormList (
    ModKey_Name TEXT NOT NULL,
    ModKey_Type INTEGER NOT NULL,
    ModKey_FileName TEXT NOT NULL,
    FormKey_ID INTEGER NOT NULL,
    EditorID TEXT NOT NULL,
    FormVersion INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2 INTEGER NOT NULL,
    VersionControl INTEGER NOT NULL,
    ImportedAtUTC TEXT NOT NULL,
    -- End Record Header --
    AddToListFormKey TEXT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

CREATE INDEX IX_FormList_FormKey_ID ON FormList (FormKey_ID);

/*
 * Form List Items table, with composite primary key (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID, Item_Index)
 */
CREATE TABLE FormListItems (
    ModKey_Name TEXT NOT NULL, 
    ModKey_Type INTEGER NOT NULL, 
    ModKey_FileName TEXT NOT NULL, 
    FormKey_ID INTEGER NOT NULL,
    Item_ModKey_Name TEXT NOT NULL,
    Item_ModKey_Type INTEGER NOT NULL,
    Item_ModKey_FileName TEXT NOT NULL,
    Item_FormKey_ID INTEGER NOT NULL, 
    ImportedAtUTC TEXT NOT NULL, 
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, Item_ModKey_Name, Item_ModKey_Type, Item_ModKey_FileName, FormKey_ID), 
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID) REFERENCES FormList (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0), 
    CHECK (Item_FormKey_ID >= 0)
);

CREATE INDEX IX_FormListItems_Item_FormKey_ID_ModKey_FormKey_ID ON FormListItems (Item_FormKey_ID, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID);

/*
 * GameSetting table, with composite primary key (ModKey_Name,ModKey_Type,ModKey_FileName,FormKey_ID)
 */
CREATE TABLE GameSetting (
    ModKey_Name TEXT NOT NULL,
    ModKey_Type INTEGER NOT NULL,
    ModKey_FileName TEXT NOT NULL,
    FormKey_ID INTEGER NOT NULL,
    EditorID TEXT NOT NULL,
    FormVersion INTEGER NOT NULL,
    StarfieldMajorRecordFlags INTEGER NOT NULL,
    Version2 INTEGER NOT NULL,
    VersionControl INTEGER NOT NULL,
    ImportedAtUTC TEXT NOT NULL,
    -- End Record Header --
    SettingType TEXT NULL,
    TitleString TEXT NULL,
    Data TEXT NULL,
    RawData REAL NULL,
    XALG INTEGER NULL,
    IsCompressed INTEGER NOT NULL,
    IsDeleted INTEGER NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ID),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (IsCompressed IS NULL OR IsCompressed IN (0, 1)),
    CHECK (IsDeleted IS NULL OR IsDeleted IN (0, 1))
);

CREATE INDEX IX_GameSetting_FormKey_ID ON GameSetting (FormKey_ID);
