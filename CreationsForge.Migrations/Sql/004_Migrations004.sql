CREATE TABLE ConstructibleObjects
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
    Description                         TEXT    NULL,
    CreatedObject_ModKey_Name           TEXT    NULL,
    CreatedObject_ModKey_Type           INTEGER NULL,
    CreatedObject_ModKey_FileName       TEXT    NULL,
    CreatedObject_FormKey_ID            INTEGER NULL,
    WorkbenchKeyword_ModKey_Name        TEXT    NULL,
    WorkbenchKeyword_ModKey_Type        INTEGER NULL,
    WorkbenchKeyword_ModKey_FileName    TEXT    NULL,
    WorkbenchKeyword_FormKey_ID         INTEGER NULL,
    CreatedObjectCount                  INTEGER NULL,
    AmountProduced                      INTEGER NULL,
    MenuSortOrder                       INTEGER NULL,
    LearnMethod                         TEXT    NULL,
    Flags                               TEXT    NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (CreatedObjectCount IS NULL OR CreatedObjectCount >= 0),
    CHECK (AmountProduced IS NULL OR AmountProduced >= 0)
);

CREATE TABLE ConstructibleObjectComponents
(
    Game                            TEXT    NOT NULL,
    ModKey_Name                     TEXT    NOT NULL,
    ModKey_Type                     INTEGER NOT NULL,
    ModKey_FileName                 TEXT    NOT NULL,
    FormKey_ModKey_Name             TEXT    NOT NULL,
    FormKey_ModKey_Type             INTEGER NOT NULL,
    FormKey_ModKey_FileName         TEXT    NOT NULL,
    FormKey_ID                      INTEGER NOT NULL,
    Component_Index                 INTEGER NOT NULL,
    Component_ModKey_Name           TEXT    NOT NULL,
    Component_ModKey_Type           INTEGER NOT NULL,
    Component_ModKey_FileName       TEXT    NOT NULL,
    Component_FormKey_ID            INTEGER NOT NULL,
    Count                           INTEGER NULL,
    ImportedAtUTC                   TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Component_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES ConstructibleObjects (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Component_Index >= 0),
    CHECK (Component_FormKey_ID >= 0),
    CHECK (Count IS NULL OR Count >= 0)
);

CREATE TABLE ConstructibleObjectCategories
(
    Game                            TEXT    NOT NULL,
    ModKey_Name                     TEXT    NOT NULL,
    ModKey_Type                     INTEGER NOT NULL,
    ModKey_FileName                 TEXT    NOT NULL,
    FormKey_ModKey_Name             TEXT    NOT NULL,
    FormKey_ModKey_Type             INTEGER NOT NULL,
    FormKey_ModKey_FileName         TEXT    NOT NULL,
    FormKey_ID                      INTEGER NOT NULL,
    Category_Index                  INTEGER NOT NULL,
    Category_ModKey_Name            TEXT    NOT NULL,
    Category_ModKey_Type            INTEGER NOT NULL,
    Category_ModKey_FileName        TEXT    NOT NULL,
    Category_FormKey_ID             INTEGER NOT NULL,
    ImportedAtUTC                   TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Category_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES ConstructibleObjects (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (Category_Index >= 0),
    CHECK (Category_FormKey_ID >= 0)
);

CREATE TABLE ConstructibleObjectRecipeFilters
(
    Game                            TEXT    NOT NULL,
    ModKey_Name                     TEXT    NOT NULL,
    ModKey_Type                     INTEGER NOT NULL,
    ModKey_FileName                 TEXT    NOT NULL,
    FormKey_ModKey_Name             TEXT    NOT NULL,
    FormKey_ModKey_Type             INTEGER NOT NULL,
    FormKey_ModKey_FileName         TEXT    NOT NULL,
    FormKey_ID                      INTEGER NOT NULL,
    RecipeFilter_Index              INTEGER NOT NULL,
    RecipeFilter_ModKey_Name        TEXT    NOT NULL,
    RecipeFilter_ModKey_Type        INTEGER NOT NULL,
    RecipeFilter_ModKey_FileName    TEXT    NOT NULL,
    RecipeFilter_FormKey_ID         INTEGER NOT NULL,
    ImportedAtUTC                   TEXT    NOT NULL,
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, RecipeFilter_Index),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES ConstructibleObjects (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0),
    CHECK (RecipeFilter_Index >= 0),
    CHECK (RecipeFilter_FormKey_ID >= 0)
);

CREATE TABLE ConditionForms
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
    PRIMARY KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID),
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName) REFERENCES Plugins (Game, ModKey_Name, ModKey_Type, ModKey_FileName) ON DELETE CASCADE,
    FOREIGN KEY (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID)
        REFERENCES RecordInstances (Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID) ON DELETE CASCADE,
    CHECK (FormKey_ID >= 0)
);

ALTER TABLE RawRecordPayloads ADD COLUMN SourcePath TEXT NULL;

CREATE INDEX IX_ConstructibleObjects_FormKey ON ConstructibleObjects (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_ConstructibleObjects_Game_Plugin ON ConstructibleObjects (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConstructibleObjects_Game_FormKey_Collated ON ConstructibleObjects (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConstructibleObjectComponents_Game_FormKey ON ConstructibleObjectComponents (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConstructibleObjectCategories_Game_FormKey ON ConstructibleObjectCategories (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConstructibleObjectRecipeFilters_Game_FormKey ON ConstructibleObjectRecipeFilters (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConditionForms_FormKey ON ConditionForms (Game, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID);
CREATE INDEX IX_ConditionForms_Game_Plugin ON ConditionForms (Game, ModKey_Name COLLATE NOCASE, ModKey_Type, ModKey_FileName COLLATE NOCASE, EditorID COLLATE NOCASE, FormKey_ID);
CREATE INDEX IX_ConditionForms_Game_FormKey_Collated ON ConditionForms (Game, FormKey_ModKey_Name COLLATE NOCASE, FormKey_ModKey_Type, FormKey_ModKey_FileName COLLATE NOCASE, FormKey_ID);

UPDATE Plugins
SET ImportState = 'Changed',
    InvalidatedAtUTC = strftime('%Y-%m-%dT%H:%M:%fZ', 'now')
WHERE ImportState IN ('Current', 'PartiallyImported');
