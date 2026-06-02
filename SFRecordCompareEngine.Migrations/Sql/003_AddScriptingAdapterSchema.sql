CREATE TABLE ScriptingAdapters
(
    ModKey_Name      TEXT    NOT NULL,
    ModKey_Type      INTEGER NOT NULL,
    ModKey_FileName  TEXT    NOT NULL,
    RecordType       TEXT    NOT NULL,
    FormKey_ID       INTEGER NOT NULL,
    Name             TEXT    NOT NULL,
    Script_Index     INTEGER NOT NULL,
    ImportedAtUTC    TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ID, Name),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Script_Index >= 0)
);

CREATE INDEX IX_ScriptingAdapters_RecordLookup ON ScriptingAdapters (RecordType, FormKey_ID);
CREATE INDEX IX_ScriptingAdapters_ScriptIndex ON ScriptingAdapters (Script_Index);

CREATE TABLE ScriptingAdapterProperties
(
    ModKey_Name            TEXT    NOT NULL,
    ModKey_Type            INTEGER NOT NULL,
    ModKey_FileName        TEXT    NOT NULL,
    RecordType             TEXT    NOT NULL,
    FormKey_ID             INTEGER NOT NULL,
    ScriptingAdapter_Name  TEXT    NOT NULL,
    Property_Index         INTEGER NOT NULL,
    Name                   TEXT    NOT NULL,
    MutagenObjectType      TEXT    NOT NULL,
    Data_Bool              INTEGER NULL,
    Data_Int               INTEGER NULL,
    Data_Float             REAL    NULL,
    Data_String            TEXT    NULL,
    Object_ModKey_Name     TEXT    NULL,
    Object_ModKey_Type     INTEGER NULL,
    Object_ModKey_FileName TEXT    NULL,
    Object_FormKey_ID      INTEGER NULL,
    Object_Alias           INTEGER NULL,
    Object_Unused          INTEGER NULL,
    ImportedAtUTC          TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ID, ScriptingAdapter_Name, Property_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ID, ScriptingAdapter_Name)
        REFERENCES ScriptingAdapters (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ID, Name) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Property_Index >= 0),
    CHECK (Data_Bool IS NULL OR Data_Bool IN (0, 1)),
    CHECK (Object_FormKey_ID IS NULL OR Object_FormKey_ID >= 0)
);

CREATE INDEX IX_ScriptingAdapterProperties_RecordLookup ON ScriptingAdapterProperties (RecordType, FormKey_ID);
CREATE INDEX IX_ScriptingAdapterProperties_PropertyIndex ON ScriptingAdapterProperties (Property_Index);
CREATE INDEX IX_ScriptingAdapterProperties_ObjectLookup ON ScriptingAdapterProperties (Object_FormKey_ID);

CREATE TABLE ScriptingAdapterPropertyListItems
(
    ModKey_Name            TEXT    NOT NULL,
    ModKey_Type            INTEGER NOT NULL,
    ModKey_FileName        TEXT    NOT NULL,
    RecordType             TEXT    NOT NULL,
    FormKey_ID             INTEGER NOT NULL,
    ScriptingAdapter_Name  TEXT    NOT NULL,
    Property_Index         INTEGER NOT NULL,
    ListItem_Index         INTEGER NOT NULL,
    MutagenObjectType      TEXT    NOT NULL,
    Data_Bool              INTEGER NULL,
    Data_Int               INTEGER NULL,
    Data_Float             REAL    NULL,
    Data_String            TEXT    NULL,
    Object_ModKey_Name     TEXT    NULL,
    Object_ModKey_Type     INTEGER NULL,
    Object_ModKey_FileName TEXT    NULL,
    Object_FormKey_ID      INTEGER NULL,
    Object_Alias           INTEGER NULL,
    Object_Unused          INTEGER NULL,
    ImportedAtUTC          TEXT    NOT NULL,
    PRIMARY KEY (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ID, ScriptingAdapter_Name, Property_Index, ListItem_Index),
    FOREIGN KEY (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ID, ScriptingAdapter_Name, Property_Index)
        REFERENCES ScriptingAdapterProperties (ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ID, ScriptingAdapter_Name, Property_Index) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Property_Index >= 0),
    CHECK (ListItem_Index >= 0),
    CHECK (Data_Bool IS NULL OR Data_Bool IN (0, 1)),
    CHECK (Object_FormKey_ID IS NULL OR Object_FormKey_ID >= 0)
);

CREATE INDEX IX_ScriptingAdapterPropertyListItems_RecordLookup ON ScriptingAdapterPropertyListItems (RecordType, FormKey_ID);
CREATE INDEX IX_ScriptingAdapterPropertyListItems_ListItemIndex ON ScriptingAdapterPropertyListItems (ListItem_Index);
CREATE INDEX IX_ScriptingAdapterPropertyListItems_ObjectLookup ON ScriptingAdapterPropertyListItems (Object_FormKey_ID);
