# Spriggit Data Validation Report

- Run date UTC: 2026-06-19T19:43:31.6789920Z
- Scope: Skyrim STAT
- Records compared: 5
- Field comparisons: 181
- JSON details: C:\Repositories\Personal\CreationsForge\TestResults\SpriggitValidation\20260619-194331\Skyrim-STAT\SpriggitValidationReport.json

## Category Counts

- Match: 34
- MissingInCreationsForge: 31
- MissingInSpriggit: 106
- ValueMismatch: 10

## Failed Records

### Skyrim STAT 0D19F9:Skyrim.esm

- EditorID: BlackreachECeiling01_GlowLichen
- Spriggit file: `Statics/BlackreachECeiling01_GlowLichen - 0D19F9_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 8, ValueMismatch: 2

Top failing fields:
- `Lod.Level0` MissingInCreationsForge - Expected DTO path 'Lod.Level0' was not populated.
- `Lod.Level1` MissingInCreationsForge - Expected DTO path 'Lod.Level1' was not populated.
- `Lod.Level2` MissingInCreationsForge - Expected DTO path 'Lod.Level2' was not populated.
- `Lod.Level3` MissingInCreationsForge - Expected DTO path 'Lod.Level3' was not populated.
- `MajorRecordFlagsRaw` MissingInCreationsForge - Expected DTO path 'MajorRecordFlagsRaw' was not populated.
- `Material` MissingInCreationsForge - Expected DTO path 'Material' was not populated.
- `Model.Data` MissingInCreationsForge - Expected DTO path 'Model.Data' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- 2 more failing fields in the JSON report.

### Skyrim STAT 06DD69:Skyrim.esm

- EditorID: DweFacadeTowerSpacer01Snow
- Spriggit file: `Statics/DweFacadeTowerSpacer01Snow - 06DD69_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 8, ValueMismatch: 2

Top failing fields:
- `Lod.Level0` MissingInCreationsForge - Expected DTO path 'Lod.Level0' was not populated.
- `Lod.Level1` MissingInCreationsForge - Expected DTO path 'Lod.Level1' was not populated.
- `Lod.Level2` MissingInCreationsForge - Expected DTO path 'Lod.Level2' was not populated.
- `Lod.Level3` MissingInCreationsForge - Expected DTO path 'Lod.Level3' was not populated.
- `MajorRecordFlagsRaw` MissingInCreationsForge - Expected DTO path 'MajorRecordFlagsRaw' was not populated.
- `Material` MissingInCreationsForge - Expected DTO path 'Material' was not populated.
- `Model.Data` MissingInCreationsForge - Expected DTO path 'Model.Data' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- 2 more failing fields in the JSON report.

### Skyrim STAT 090E82:Skyrim.esm

- EditorID: HHMountainRidge01
- Spriggit file: `Statics/HHMountainRidge01 - 090E82_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 8, ValueMismatch: 2

Top failing fields:
- `Lod.Level0` MissingInCreationsForge - Expected DTO path 'Lod.Level0' was not populated.
- `Lod.Level1` MissingInCreationsForge - Expected DTO path 'Lod.Level1' was not populated.
- `Lod.Level2` MissingInCreationsForge - Expected DTO path 'Lod.Level2' was not populated.
- `Lod.Level3` MissingInCreationsForge - Expected DTO path 'Lod.Level3' was not populated.
- `MajorRecordFlagsRaw` MissingInCreationsForge - Expected DTO path 'MajorRecordFlagsRaw' was not populated.
- `Material` MissingInCreationsForge - Expected DTO path 'Material' was not populated.
- `Model.Data` MissingInCreationsForge - Expected DTO path 'Model.Data' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- 2 more failing fields in the JSON report.

### Skyrim STAT 0946B2:Skyrim.esm

- EditorID: CaveGRockPileS01IceBlend
- Spriggit file: `Statics/CaveGRockPileS01IceBlend - 0946B2_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 4, ValueMismatch: 2

Top failing fields:
- `MajorRecordFlagsRaw` MissingInCreationsForge - Expected DTO path 'MajorRecordFlagsRaw' was not populated.
- `Material` MissingInCreationsForge - Expected DTO path 'Material' was not populated.
- `Model.Data` MissingInCreationsForge - Expected DTO path 'Model.Data' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Model.File` ValueMismatch - Spriggit `Dungeons\Caves\Green\Rocks\CaveGRockPileS01.nif` vs DTO `Meshes\Dungeons\Caves\Green\Rocks\CaveGRockPileS01.nif`.
- `Unused` ValueMismatch - Spriggit `[]` vs DTO `(none)`.

### Skyrim STAT 078DC0:Skyrim.esm

- EditorID: XMarkerSnow
- Spriggit file: `Statics/XMarkerSnow - 078DC0_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 2

Top failing fields:
- `MajorRecordFlagsRaw` MissingInCreationsForge - Expected DTO path 'MajorRecordFlagsRaw' was not populated.
- `Material` MissingInCreationsForge - Expected DTO path 'Material' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Model.File` ValueMismatch - Spriggit `MarkerX.nif` vs DTO `Meshes\MarkerX.nif`.
- `Unused` ValueMismatch - Spriggit `[]` vs DTO `(none)`.


## Highest Priority Findings

- Skyrim STAT 0D19F9:Skyrim.esm `Lod.Level0` MissingInCreationsForge
- Skyrim STAT 0D19F9:Skyrim.esm `Lod.Level1` MissingInCreationsForge
- Skyrim STAT 0D19F9:Skyrim.esm `Lod.Level2` MissingInCreationsForge
- Skyrim STAT 0D19F9:Skyrim.esm `Lod.Level3` MissingInCreationsForge
- Skyrim STAT 0D19F9:Skyrim.esm `MajorRecordFlagsRaw` MissingInCreationsForge
- Skyrim STAT 0D19F9:Skyrim.esm `Material` MissingInCreationsForge
- Skyrim STAT 0D19F9:Skyrim.esm `Model.Data` MissingInCreationsForge
- Skyrim STAT 0D19F9:Skyrim.esm `Model.File` ValueMismatch
- Skyrim STAT 0D19F9:Skyrim.esm `Unused` ValueMismatch
- Skyrim STAT 0D19F9:Skyrim.esm `VersionControl` MissingInCreationsForge
- Skyrim STAT 06DD69:Skyrim.esm `Lod.Level0` MissingInCreationsForge
- Skyrim STAT 06DD69:Skyrim.esm `Lod.Level1` MissingInCreationsForge
- Skyrim STAT 06DD69:Skyrim.esm `Lod.Level2` MissingInCreationsForge
- Skyrim STAT 06DD69:Skyrim.esm `Lod.Level3` MissingInCreationsForge
- Skyrim STAT 06DD69:Skyrim.esm `MajorRecordFlagsRaw` MissingInCreationsForge
- Skyrim STAT 06DD69:Skyrim.esm `Material` MissingInCreationsForge
- Skyrim STAT 06DD69:Skyrim.esm `Model.Data` MissingInCreationsForge
- Skyrim STAT 06DD69:Skyrim.esm `Model.File` ValueMismatch
- Skyrim STAT 06DD69:Skyrim.esm `Unused` ValueMismatch
- Skyrim STAT 06DD69:Skyrim.esm `VersionControl` MissingInCreationsForge
- Skyrim STAT 090E82:Skyrim.esm `Lod.Level0` MissingInCreationsForge
- Skyrim STAT 090E82:Skyrim.esm `Lod.Level1` MissingInCreationsForge
- Skyrim STAT 090E82:Skyrim.esm `Lod.Level2` MissingInCreationsForge
- Skyrim STAT 090E82:Skyrim.esm `Lod.Level3` MissingInCreationsForge
- Skyrim STAT 090E82:Skyrim.esm `MajorRecordFlagsRaw` MissingInCreationsForge
- Skyrim STAT 090E82:Skyrim.esm `Material` MissingInCreationsForge
- Skyrim STAT 090E82:Skyrim.esm `Model.Data` MissingInCreationsForge
- Skyrim STAT 090E82:Skyrim.esm `Model.File` ValueMismatch
- Skyrim STAT 090E82:Skyrim.esm `Unused` ValueMismatch
- Skyrim STAT 090E82:Skyrim.esm `VersionControl` MissingInCreationsForge
- Skyrim STAT 0946B2:Skyrim.esm `MajorRecordFlagsRaw` MissingInCreationsForge
- Skyrim STAT 0946B2:Skyrim.esm `Material` MissingInCreationsForge
- Skyrim STAT 0946B2:Skyrim.esm `Model.Data` MissingInCreationsForge
- Skyrim STAT 0946B2:Skyrim.esm `Model.File` ValueMismatch
- Skyrim STAT 0946B2:Skyrim.esm `Unused` ValueMismatch
- Skyrim STAT 0946B2:Skyrim.esm `VersionControl` MissingInCreationsForge
- Skyrim STAT 078DC0:Skyrim.esm `MajorRecordFlagsRaw` MissingInCreationsForge
- Skyrim STAT 078DC0:Skyrim.esm `Material` MissingInCreationsForge
- Skyrim STAT 078DC0:Skyrim.esm `Model.File` ValueMismatch
- Skyrim STAT 078DC0:Skyrim.esm `Unused` ValueMismatch
- Skyrim STAT 078DC0:Skyrim.esm `VersionControl` MissingInCreationsForge
