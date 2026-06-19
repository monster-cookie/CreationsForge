# Spriggit Data Validation Report

- Run date UTC: 2026-06-19T20:24:17.2092783Z
- Scope: Starfield GMST
- Records compared: 3
- Field comparisons: 87
- JSON details: C:\Repositories\Personal\CreationsForge\TestResults\SpriggitValidation\20260619-202417\Starfield-GMST\SpriggitValidationReport.json

## Category Counts

- Match: 75
- MissingInSpriggit: 9
- ValueMismatch: 3

## Failed Records

### Starfield GMST 0657E0:Starfield.esm

- EditorID: sAbort
- Spriggit file: `GameSettings/sAbort - 0657E0_Starfield.esm.yaml`
- Failing counts: ValueMismatch: 1

Top failing fields:
- `VersionControl` ValueMismatch - Spriggit `2110622` vs DTO `2110657`.

### Starfield GMST 0D4DFC:Starfield.esm

- EditorID: sActivate
- Spriggit file: `GameSettings/sActivate - 0D4DFC_Starfield.esm.yaml`
- Failing counts: ValueMismatch: 1

Top failing fields:
- `VersionControl` ValueMismatch - Spriggit `2110622` vs DTO `2110657`.

### Starfield GMST 0D4DEB:Starfield.esm

- EditorID: sActivateCreatureCalmed
- Spriggit file: `GameSettings/sActivateCreatureCalmed - 0D4DEB_Starfield.esm.yaml`
- Failing counts: ValueMismatch: 1

Top failing fields:
- `VersionControl` ValueMismatch - Spriggit `2110622` vs DTO `2110657`.


## Highest Priority Findings

- Starfield GMST 0657E0:Starfield.esm `VersionControl` ValueMismatch
- Starfield GMST 0D4DFC:Starfield.esm `VersionControl` ValueMismatch
- Starfield GMST 0D4DEB:Starfield.esm `VersionControl` ValueMismatch
