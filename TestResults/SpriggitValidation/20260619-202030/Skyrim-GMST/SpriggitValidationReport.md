# Spriggit Data Validation Report

- Run date UTC: 2026-06-19T20:20:30.4253901Z
- Scope: Skyrim GMST
- Records compared: 3
- Field comparisons: 87
- JSON details: C:\Repositories\Personal\CreationsForge\TestResults\SpriggitValidation\20260619-202030\Skyrim-GMST\SpriggitValidationReport.json

## Category Counts

- Match: 26
- MissingInCreationsForge: 9
- MissingInSpriggit: 12
- ValueMismatch: 40

## Failed Records

### Skyrim GMST 0D4C40:Skyrim.esm

- EditorID: sAbortText
- Spriggit file: `GameSettings/sAbortText - 0D4C40_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 14

Top failing fields:
- `MutagenObjectType` MissingInCreationsForge - Expected DTO path 'MutagenObjectType' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Data[0].Language` ValueMismatch - Spriggit `German` vs DTO `English`.
- `Data[0].String` ValueMismatch - Spriggit `Abbrechen` vs DTO `Abort`.
- `Data[1].Language` ValueMismatch - Spriggit `English` vs DTO `German`.
- `Data[1].String` ValueMismatch - Spriggit `Abort` vs DTO `Abbrechen`.
- `Data[2].Language` ValueMismatch - Spriggit `Spanish` vs DTO `Italian`.
- 9 more failing fields in the JSON report.

### Skyrim GMST 0D4DC4:Skyrim.esm

- EditorID: sAccept
- Spriggit file: `GameSettings/sAccept - 0D4DC4_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 14

Top failing fields:
- `MutagenObjectType` MissingInCreationsForge - Expected DTO path 'MutagenObjectType' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Data[0].Language` ValueMismatch - Spriggit `German` vs DTO `English`.
- `Data[0].String` ValueMismatch - Spriggit `Akzeptieren` vs DTO `Accept`.
- `Data[1].Language` ValueMismatch - Spriggit `English` vs DTO `German`.
- `Data[1].String` ValueMismatch - Spriggit `Accept` vs DTO `Akzeptieren`.
- `Data[2].Language` ValueMismatch - Spriggit `Spanish` vs DTO `Italian`.
- 9 more failing fields in the JSON report.

### Skyrim GMST 0D4B96:Skyrim.esm

- EditorID: sActionMapping
- Spriggit file: `GameSettings/sActionMapping - 0D4B96_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 12

Top failing fields:
- `MutagenObjectType` MissingInCreationsForge - Expected DTO path 'MutagenObjectType' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Data[0].Language` ValueMismatch - Spriggit `German` vs DTO `English`.
- `Data[1].Language` ValueMismatch - Spriggit `English` vs DTO `German`.
- `Data[2].Language` ValueMismatch - Spriggit `Spanish` vs DTO `Italian`.
- `Data[2].String` ValueMismatch - Spriggit `Asignación de acciones` vs DTO `Assegnazione azioni`.
- `Data[3].Language` ValueMismatch - Spriggit `Italian` vs DTO `Spanish`.
- 7 more failing fields in the JSON report.


## Highest Priority Findings

- Skyrim GMST 0D4C40:Skyrim.esm `Data[0].Language` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[0].String` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[1].Language` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[1].String` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[2].Language` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[2].String` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[3].Language` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[3].String` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[4].Language` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[4].String` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[6].Language` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[6].String` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[7].Language` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `Data[7].String` ValueMismatch
- Skyrim GMST 0D4C40:Skyrim.esm `MutagenObjectType` MissingInCreationsForge
- Skyrim GMST 0D4C40:Skyrim.esm `Version2` MissingInCreationsForge
- Skyrim GMST 0D4C40:Skyrim.esm `VersionControl` MissingInCreationsForge
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[0].Language` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[0].String` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[1].Language` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[1].String` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[2].Language` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[2].String` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[3].Language` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[3].String` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[4].Language` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[4].String` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[6].Language` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[6].String` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[7].Language` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `Data[7].String` ValueMismatch
- Skyrim GMST 0D4DC4:Skyrim.esm `MutagenObjectType` MissingInCreationsForge
- Skyrim GMST 0D4DC4:Skyrim.esm `Version2` MissingInCreationsForge
- Skyrim GMST 0D4DC4:Skyrim.esm `VersionControl` MissingInCreationsForge
- Skyrim GMST 0D4B96:Skyrim.esm `Data[0].Language` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[1].Language` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[2].Language` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[2].String` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[3].Language` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[3].String` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[4].Language` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[4].String` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[6].Language` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[6].String` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[7].Language` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `Data[7].String` ValueMismatch
- Skyrim GMST 0D4B96:Skyrim.esm `MutagenObjectType` MissingInCreationsForge
- Skyrim GMST 0D4B96:Skyrim.esm `Version2` MissingInCreationsForge
- Skyrim GMST 0D4B96:Skyrim.esm `VersionControl` MissingInCreationsForge
