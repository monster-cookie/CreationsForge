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
CREATE INDEX IX_MiscObjects_Game_Plugin ON MiscObjects (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_MiscObjects_Game_FormKey_Collated ON MiscObjects (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Perks_Game_Plugin ON Perks (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Perks_Game_FormKey_Collated ON Perks (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Statics_FormKey ON Statics (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_Statics_Game_Plugin ON Statics (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Statics_Game_FormKey_Collated ON Statics (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_RawRecordPayloads_Game_Record_FormKey ON RawRecordPayloads (Game, RecordType, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Containers_Game_Plugin ON Containers (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_Containers_Game_FormKey_Collated ON Containers (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ContainerItems_Game_FormKey ON ContainerItems (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
