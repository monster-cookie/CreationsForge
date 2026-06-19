# Spriggit Data Validation Report

- Run date UTC: 2026-06-19T20:21:30.2170084Z
- Scope: Fallout4 GMST
- Records compared: 3
- Field comparisons: 99
- JSON details: C:\Repositories\Personal\CreationsForge\TestResults\SpriggitValidation\20260619-202130\Fallout4-GMST\SpriggitValidationReport.json

## Category Counts

- Match: 18
- MissingInCreationsForge: 9
- MissingInSpriggit: 12
- ValueMismatch: 60

## Failed Records

### Fallout4 GMST 0D4C40:Fallout4.esm

- EditorID: sAbortText
- Spriggit file: `GameSettings/sAbortText - 0D4C40_Fallout4.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 20

Top failing fields:
- `MutagenObjectType` MissingInCreationsForge - Expected DTO path 'MutagenObjectType' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Data[0].Language` ValueMismatch - Spriggit `Chinese` vs DTO `German`.
- `Data[0].String` ValueMismatch - Spriggit `中止` vs DTO `Abbrechen`.
- `Data[10].Language` ValueMismatch - Spriggit `Russian` vs DTO `Spanish_Mexico`.
- `Data[10].String` ValueMismatch - Spriggit `Отмена` vs DTO `Detener`.
- `Data[1].Language` ValueMismatch - Spriggit `German` vs DTO `English`.
- 15 more failing fields in the JSON report.

### Fallout4 GMST 0D4DC4:Fallout4.esm

- EditorID: sAccept
- Spriggit file: `GameSettings/sAccept - 0D4DC4_Fallout4.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 20

Top failing fields:
- `MutagenObjectType` MissingInCreationsForge - Expected DTO path 'MutagenObjectType' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Data[0].Language` ValueMismatch - Spriggit `Chinese` vs DTO `German`.
- `Data[0].String` ValueMismatch - Spriggit `接受` vs DTO `Akzeptieren`.
- `Data[10].Language` ValueMismatch - Spriggit `Russian` vs DTO `Spanish_Mexico`.
- `Data[10].String` ValueMismatch - Spriggit `Принять` vs DTO `Aceptar`.
- `Data[1].Language` ValueMismatch - Spriggit `German` vs DTO `English`.
- 15 more failing fields in the JSON report.

### Fallout4 GMST 0D4DFC:Fallout4.esm

- EditorID: sActivate
- Spriggit file: `GameSettings/sActivate - 0D4DFC_Fallout4.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 20

Top failing fields:
- `MutagenObjectType` MissingInCreationsForge - Expected DTO path 'MutagenObjectType' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Data[0].Language` ValueMismatch - Spriggit `Chinese` vs DTO `German`.
- `Data[0].String` ValueMismatch - Spriggit `動作` vs DTO `Aktivieren`.
- `Data[10].Language` ValueMismatch - Spriggit `Russian` vs DTO `Spanish_Mexico`.
- `Data[10].String` ValueMismatch - Spriggit `Активировать` vs DTO `Activar`.
- `Data[1].Language` ValueMismatch - Spriggit `German` vs DTO `English`.
- 15 more failing fields in the JSON report.


## Highest Priority Findings

- Fallout4 GMST 0D4C40:Fallout4.esm `Data[0].Language` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[0].String` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[10].Language` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[10].String` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[1].Language` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[1].String` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[2].Language` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[2].String` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[3].Language` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[3].String` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[4].Language` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[4].String` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[5].Language` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[5].String` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[6].Language` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[6].String` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[7].Language` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[7].String` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[8].Language` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `Data[8].String` ValueMismatch
- Fallout4 GMST 0D4C40:Fallout4.esm `MutagenObjectType` MissingInCreationsForge
- Fallout4 GMST 0D4C40:Fallout4.esm `Version2` MissingInCreationsForge
- Fallout4 GMST 0D4C40:Fallout4.esm `VersionControl` MissingInCreationsForge
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[0].Language` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[0].String` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[10].Language` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[10].String` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[1].Language` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[1].String` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[2].Language` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[2].String` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[3].Language` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[3].String` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[4].Language` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[4].String` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[5].Language` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[5].String` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[6].Language` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[6].String` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[7].Language` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[7].String` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[8].Language` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `Data[8].String` ValueMismatch
- Fallout4 GMST 0D4DC4:Fallout4.esm `MutagenObjectType` MissingInCreationsForge
- Fallout4 GMST 0D4DC4:Fallout4.esm `Version2` MissingInCreationsForge
- Fallout4 GMST 0D4DC4:Fallout4.esm `VersionControl` MissingInCreationsForge
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[0].Language` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[0].String` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[10].Language` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[10].String` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[1].Language` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[1].String` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[2].Language` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[2].String` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[3].Language` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[3].String` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[4].Language` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[4].String` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[5].Language` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[5].String` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[6].Language` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[6].String` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[7].Language` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[7].String` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[8].Language` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `Data[8].String` ValueMismatch
- Fallout4 GMST 0D4DFC:Fallout4.esm `MutagenObjectType` MissingInCreationsForge
- Fallout4 GMST 0D4DFC:Fallout4.esm `Version2` MissingInCreationsForge
- Fallout4 GMST 0D4DFC:Fallout4.esm `VersionControl` MissingInCreationsForge
