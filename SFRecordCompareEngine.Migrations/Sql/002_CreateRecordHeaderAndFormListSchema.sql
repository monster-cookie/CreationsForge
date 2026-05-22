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
