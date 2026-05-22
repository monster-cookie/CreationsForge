CREATE TABLE IF NOT EXISTS Plugins (
    ModKey TEXT COLLATE NOCASE NOT NULL PRIMARY KEY,
    GameRelease TEXT NOT NULL,
    LoadOrderIndex INTEGER NULL,
    PluginFileName TEXT NOT NULL,
    PluginPath TEXT NULL,
    Enabled INTEGER NOT NULL DEFAULT 1,
    ExistsOnDisk INTEGER NOT NULL DEFAULT 1,
    ImportState TEXT NOT NULL DEFAULT 'Current',
    HeaderFlags INTEGER NULL,
    FormVersion INTEGER NULL,
    Author TEXT NULL,
    Branch TEXT NULL,
    InteriorCellCount INTEGER NULL,
    SourceLastWriteUtcTicks INTEGER NULL,
    SourceFileSizeBytes INTEGER NULL,
    LastCheckedUtc TEXT NOT NULL,
    LastImportedUtc TEXT NULL,
    InvalidatedAtUtc TEXT NULL,

    CHECK (Enabled IN (0, 1)),
    CHECK (ExistsOnDisk IN (0, 1)),
    CHECK (ImportState IN ('Current', 'Changed', 'Missing', 'Failed'))
);

CREATE INDEX IF NOT EXISTS IX_Plugins_LoadOrderIndex
ON Plugins(LoadOrderIndex);

CREATE INDEX IF NOT EXISTS IX_Plugins_PluginFileName
ON Plugins(PluginFileName);

CREATE INDEX IF NOT EXISTS IX_Plugins_ImportState
ON Plugins(ImportState);

CREATE INDEX IF NOT EXISTS IX_Plugins_SourceFingerprint
ON Plugins(SourceLastWriteUtcTicks, SourceFileSizeBytes);

CREATE TABLE IF NOT EXISTS PluginMasterReferences (
    ModKey TEXT COLLATE NOCASE NOT NULL,
    ParentModKey TEXT COLLATE NOCASE NOT NULL,
    MasterReferenceIndex INTEGER NOT NULL,
    ParentLoadOrderIndex INTEGER NULL,
    ImportedAtUtc TEXT NOT NULL,

    PRIMARY KEY (ModKey, ParentModKey),

    FOREIGN KEY (ModKey)
        REFERENCES Plugins(ModKey)
        ON DELETE CASCADE,

    FOREIGN KEY (ParentModKey)
        REFERENCES Plugins(ModKey)
        ON DELETE RESTRICT,

    CHECK (MasterReferenceIndex >= 0)
);

CREATE INDEX IF NOT EXISTS IX_PluginMasterReferences_ModKey_ParentLoadOrderIndex
ON PluginMasterReferences(ModKey, ParentLoadOrderIndex);

CREATE INDEX IF NOT EXISTS IX_PluginMasterReferences_ParentModKey
ON PluginMasterReferences(ParentModKey);

CREATE UNIQUE INDEX IF NOT EXISTS UX_PluginMasterReferences_ModKey_MasterReferenceIndex
ON PluginMasterReferences(ModKey, MasterReferenceIndex);

CREATE VIEW IF NOT EXISTS PluginResolutionHierarchy AS
SELECT
    pmr.ModKey AS ChildModKey,
    pmr.ParentModKey AS HierarchyModKey,
    pmr.ParentLoadOrderIndex AS HierarchyLoadOrderIndex,
    pmr.MasterReferenceIndex,
    0 AS IsChild
FROM PluginMasterReferences pmr

UNION ALL

SELECT
    p.ModKey AS ChildModKey,
    p.ModKey AS HierarchyModKey,
    p.LoadOrderIndex AS HierarchyLoadOrderIndex,
    NULL AS MasterReferenceIndex,
    1 AS IsChild
FROM Plugins p;
