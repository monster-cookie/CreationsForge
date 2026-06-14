ALTER TABLE MiscObjects RENAME TO MiscItems;

DROP INDEX IF EXISTS IX_MiscObjects_Game_Plugin;
DROP INDEX IF EXISTS IX_MiscObjects_Game_FormKey_Collated;

CREATE INDEX IX_MiscItems_Game_Plugin ON MiscItems (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_MiscItems_Game_FormKey_Collated ON MiscItems (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

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
    ObjectBounds_First              TEXT    NULL,
    ObjectBounds_Second             TEXT    NULL,
    InventoryTransform_ModKey_Name  TEXT    NULL,
    InventoryTransform_ModKey_Type  INTEGER NULL,
    InventoryTransform_ModKey_FileName TEXT NULL,
    InventoryTransform_FormKey_ID   INTEGER NULL,
    XALG                            INTEGER NULL,
    Name                            TEXT    NULL,
    Text                            TEXT    NULL,
    Value                           INTEGER NULL,
    Weight                          REAL    NULL,
    Flags                           TEXT    NULL,
    TeachesType                     TEXT    NULL,
    TeachesRawContent               TEXT    NULL,
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
