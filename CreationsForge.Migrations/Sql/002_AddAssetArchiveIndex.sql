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
