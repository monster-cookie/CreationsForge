# Spriggit Data Validation Report

- Run date UTC: 2026-06-19T20:20:34.2853362Z
- Scope: Starfield GMST
- Records compared: 3
- Field comparisons: 87
- JSON details: C:\Repositories\Personal\CreationsForge\TestResults\SpriggitValidation\20260619-202034\Starfield-GMST\SpriggitValidationReport.json

## Category Counts

- Match: 21
- MissingInCreationsForge: 9
- MissingInSpriggit: 9
- ValueMismatch: 48

## Failed Records

### Starfield GMST 0657E0:Starfield.esm

- EditorID: sAbort
- Spriggit file: `GameSettings/sAbort - 0657E0_Starfield.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 16

Top failing fields:
- `MutagenObjectType` MissingInCreationsForge - Expected DTO path 'MutagenObjectType' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Data[0].Language` ValueMismatch - Spriggit `German` vs DTO `English`.
- `Data[0].String` ValueMismatch - Spriggit `Abbrechen` vs DTO `Abort`.
- `Data[1].Language` ValueMismatch - Spriggit `English` vs DTO `German`.
- `Data[1].String` ValueMismatch - Spriggit `Abort` vs DTO `Abbrechen`.
- `Data[2].Language` ValueMismatch - Spriggit `Spanish` vs DTO `Italian`.
- 11 more failing fields in the JSON report.

### Starfield GMST 0D4DFC:Starfield.esm

- EditorID: sActivate
- Spriggit file: `GameSettings/sActivate - 0D4DFC_Starfield.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 16

Top failing fields:
- `MutagenObjectType` MissingInCreationsForge - Expected DTO path 'MutagenObjectType' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Data[0].Language` ValueMismatch - Spriggit `German` vs DTO `English`.
- `Data[0].String` ValueMismatch - Spriggit `Aktivieren` vs DTO `Activate`.
- `Data[1].Language` ValueMismatch - Spriggit `English` vs DTO `German`.
- `Data[1].String` ValueMismatch - Spriggit `Activate` vs DTO `Aktivieren`.
- `Data[2].Language` ValueMismatch - Spriggit `Spanish` vs DTO `Italian`.
- 11 more failing fields in the JSON report.

### Starfield GMST 0D4DEB:Starfield.esm

- EditorID: sActivateCreatureCalmed
- Spriggit file: `GameSettings/sActivateCreatureCalmed - 0D4DEB_Starfield.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 16

Top failing fields:
- `MutagenObjectType` MissingInCreationsForge - Expected DTO path 'MutagenObjectType' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Data[0].Language` ValueMismatch - Spriggit `German` vs DTO `English`.
- `Data[0].String` ValueMismatch - Spriggit `Die Kreatur wurde besänftigt und kann nicht reagieren.` vs DTO `The creature is calmed and cannot respond.`.
- `Data[1].Language` ValueMismatch - Spriggit `English` vs DTO `German`.
- `Data[1].String` ValueMismatch - Spriggit `The creature is calmed and cannot respond.` vs DTO `Die Kreatur wurde besänftigt und kann nicht reagieren.`.
- `Data[2].Language` ValueMismatch - Spriggit `Spanish` vs DTO `Italian`.
- 11 more failing fields in the JSON report.


## Highest Priority Findings

- Starfield GMST 0657E0:Starfield.esm `Data[0].Language` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[0].String` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[1].Language` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[1].String` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[2].Language` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[2].String` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[3].Language` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[3].String` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[4].Language` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[4].String` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[5].Language` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[5].String` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[6].Language` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[6].String` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[7].Language` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `Data[7].String` ValueMismatch
- Starfield GMST 0657E0:Starfield.esm `MutagenObjectType` MissingInCreationsForge
- Starfield GMST 0657E0:Starfield.esm `Version2` MissingInCreationsForge
- Starfield GMST 0657E0:Starfield.esm `VersionControl` MissingInCreationsForge
- Starfield GMST 0D4DFC:Starfield.esm `Data[0].Language` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[0].String` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[1].Language` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[1].String` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[2].Language` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[2].String` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[3].Language` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[3].String` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[4].Language` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[4].String` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[5].Language` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[5].String` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[6].Language` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[6].String` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[7].Language` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `Data[7].String` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `MutagenObjectType` MissingInCreationsForge
- Starfield GMST 0D4DFC:Starfield.esm `Version2` MissingInCreationsForge
- Starfield GMST 0D4DFC:Starfield.esm `VersionControl` MissingInCreationsForge
- Starfield GMST 0D4DEB:Starfield.esm `Data[0].Language` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[0].String` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[1].Language` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[1].String` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[2].Language` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[2].String` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[3].Language` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[3].String` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[4].Language` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[4].String` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[5].Language` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[5].String` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[6].Language` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[6].String` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[7].Language` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `Data[7].String` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `MutagenObjectType` MissingInCreationsForge
- Starfield GMST 0D4DEB:Starfield.esm `Version2` MissingInCreationsForge
- Starfield GMST 0D4DEB:Starfield.esm `VersionControl` MissingInCreationsForge
