CREATE TABLE IF NOT EXISTS Keyword (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Keyword_Name
ON Keyword(Name);

CREATE TABLE IF NOT EXISTS Faction (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Faction_Name
ON Faction(Name);

CREATE TABLE IF NOT EXISTS Message (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Message_Name
ON Message(Name);

CREATE TABLE IF NOT EXISTS GameplayOptionsGroup (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_GameplayOptionsGroup_Name
ON GameplayOptionsGroup(Name);

CREATE TABLE IF NOT EXISTS Static (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ObjectBounds TEXT NULL,
    Model TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Static_Name
ON Static(Name);

CREATE TABLE IF NOT EXISTS StaticCollection (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ObjectBounds TEXT NULL,
    Model TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_StaticCollection_Name
ON StaticCollection(Name);

CREATE TABLE IF NOT EXISTS Activator (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ObjectBounds TEXT NULL,
    Model TEXT NULL,
    Destructible TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_Activator_Name
ON Activator(Name);

CREATE TABLE IF NOT EXISTS ActivatorKeyword (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    ItemIndex INTEGER NOT NULL,
    KeywordFormKey TEXT NOT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES Activator(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_ActivatorKeyword_KeywordFormKey
ON ActivatorKeyword(KeywordFormKey);

CREATE TABLE IF NOT EXISTS MiscItem (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ObjectBounds TEXT NULL,
    Model TEXT NULL,
    Destructible TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_MiscItem_Name
ON MiscItem(Name);

CREATE TABLE IF NOT EXISTS MiscItemKeyword (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    ItemIndex INTEGER NOT NULL,
    KeywordFormKey TEXT NOT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES MiscItem(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_MiscItemKeyword_KeywordFormKey
ON MiscItemKeyword(KeywordFormKey);

CREATE TABLE IF NOT EXISTS GameplayOption (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_GameplayOption_Name
ON GameplayOption(Name);

CREATE TABLE IF NOT EXISTS GameplayOptionKeyword (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    ItemIndex INTEGER NOT NULL,
    KeywordFormKey TEXT NOT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES GameplayOption(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_GameplayOptionKeyword_KeywordFormKey
ON GameplayOptionKeyword(KeywordFormKey);

CREATE TABLE IF NOT EXISTS MagicEffect (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    Name TEXT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES RecordHeader(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID))
);

CREATE INDEX IF NOT EXISTS IX_MagicEffect_Name
ON MagicEffect(Name);

CREATE TABLE IF NOT EXISTS MagicEffectKeyword (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    FormID TEXT NOT NULL,
    ItemIndex INTEGER NOT NULL,
    KeywordFormKey TEXT NOT NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, FormID, ItemIndex),

    FOREIGN KEY (ModKey, FormID)
        REFERENCES MagicEffect(ModKey, FormID)
        ON DELETE CASCADE,

    CHECK (length(FormID) = 6),
    CHECK (FormID = upper(FormID)),
    CHECK (ItemIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_MagicEffectKeyword_KeywordFormKey
ON MagicEffectKeyword(KeywordFormKey);
