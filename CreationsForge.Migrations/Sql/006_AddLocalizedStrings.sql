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

UPDATE Plugins
SET ImportState = 'Changed',
    InvalidatedAtUTC = CURRENT_TIMESTAMP,
    ImportMessage = 'Localized string schema changed; reimport required.',
    ImportDetails = 'Migration 006 added LocalizedStrings child rows for translated record text.'
WHERE ImportState IN ('Current', 'PartiallyImported');
