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
