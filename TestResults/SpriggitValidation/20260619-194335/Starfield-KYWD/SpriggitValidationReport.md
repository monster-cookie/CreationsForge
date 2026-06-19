# Spriggit Data Validation Report

- Run date UTC: 2026-06-19T19:43:35.0739775Z
- Scope: Starfield KYWD
- Records compared: 5
- Field comparisons: 145
- JSON details: C:\Repositories\Personal\CreationsForge\TestResults\SpriggitValidation\20260619-194335\Starfield-KYWD\SpriggitValidationReport.json

## Category Counts

- Match: 21
- MissingInCreationsForge: 101
- MissingInSpriggit: 18
- ValueMismatch: 5

## Failed Records

### Starfield KYWD 200AEB:Starfield.esm

- EditorID: CCT_Enviro_AmbusherSurface
- Spriggit file: `Keywords/CCT_Enviro_AmbusherSurface - 200AEB_Starfield.esm.yaml`
- Failing counts: MissingInCreationsForge: 24, ValueMismatch: 1

Top failing fields:
- `AttractionRule` MissingInCreationsForge - Expected DTO path 'AttractionRule' was not populated.
- `FNAM` MissingInCreationsForge - Expected DTO path 'FNAM' was not populated.
- `Name.Count` MissingInCreationsForge - Expected DTO path 'Name.Count' was not populated.
- `Name.TargetLanguage` MissingInCreationsForge - Expected DTO path 'Name.TargetLanguage' was not populated.
- `Name[0].Language` MissingInCreationsForge - Expected DTO path 'Name[0].Language' was not populated.
- `Name[0].String` MissingInCreationsForge - Expected DTO path 'Name[0].String' was not populated.
- `Name[1].Language` MissingInCreationsForge - Expected DTO path 'Name[1].Language' was not populated.
- `Name[1].String` MissingInCreationsForge - Expected DTO path 'Name[1].String' was not populated.
- 17 more failing fields in the JSON report.

### Starfield KYWD 145388:Starfield.esm

- EditorID: CCT_Enviro_AmbusherUnderground
- Spriggit file: `Keywords/CCT_Enviro_AmbusherUnderground - 145388_Starfield.esm.yaml`
- Failing counts: MissingInCreationsForge: 24, ValueMismatch: 1

Top failing fields:
- `AttractionRule` MissingInCreationsForge - Expected DTO path 'AttractionRule' was not populated.
- `FNAM` MissingInCreationsForge - Expected DTO path 'FNAM' was not populated.
- `Name.Count` MissingInCreationsForge - Expected DTO path 'Name.Count' was not populated.
- `Name.TargetLanguage` MissingInCreationsForge - Expected DTO path 'Name.TargetLanguage' was not populated.
- `Name[0].Language` MissingInCreationsForge - Expected DTO path 'Name[0].Language' was not populated.
- `Name[0].String` MissingInCreationsForge - Expected DTO path 'Name[0].String' was not populated.
- `Name[1].Language` MissingInCreationsForge - Expected DTO path 'Name[1].Language' was not populated.
- `Name[1].String` MissingInCreationsForge - Expected DTO path 'Name[1].String' was not populated.
- 17 more failing fields in the JSON report.

### Starfield KYWD 200ADF:Starfield.esm

- EditorID: CCT_Enviro_Basking
- Spriggit file: `Keywords/CCT_Enviro_Basking - 200ADF_Starfield.esm.yaml`
- Failing counts: MissingInCreationsForge: 24, ValueMismatch: 1

Top failing fields:
- `AttractionRule` MissingInCreationsForge - Expected DTO path 'AttractionRule' was not populated.
- `FNAM` MissingInCreationsForge - Expected DTO path 'FNAM' was not populated.
- `Name.Count` MissingInCreationsForge - Expected DTO path 'Name.Count' was not populated.
- `Name.TargetLanguage` MissingInCreationsForge - Expected DTO path 'Name.TargetLanguage' was not populated.
- `Name[0].Language` MissingInCreationsForge - Expected DTO path 'Name[0].Language' was not populated.
- `Name[0].String` MissingInCreationsForge - Expected DTO path 'Name[0].String' was not populated.
- `Name[1].Language` MissingInCreationsForge - Expected DTO path 'Name[1].Language' was not populated.
- `Name[1].String` MissingInCreationsForge - Expected DTO path 'Name[1].String' was not populated.
- 17 more failing fields in the JSON report.

### Starfield KYWD 1C84DD:Starfield.esm

- EditorID: WeaponTypeDisplay_ElectromagneticRifle
- Spriggit file: `Keywords/WeaponTypeDisplay_ElectromagneticRifle - 1C84DD_Starfield.esm.yaml`
- Failing counts: MissingInCreationsForge: 25, ValueMismatch: 1

Top failing fields:
- `FNAM` MissingInCreationsForge - Expected DTO path 'FNAM' was not populated.
- `Name.Count` MissingInCreationsForge - Expected DTO path 'Name.Count' was not populated.
- `Name.TargetLanguage` MissingInCreationsForge - Expected DTO path 'Name.TargetLanguage' was not populated.
- `Name[0].Language` MissingInCreationsForge - Expected DTO path 'Name[0].Language' was not populated.
- `Name[0].String` MissingInCreationsForge - Expected DTO path 'Name[0].String' was not populated.
- `Name[1].Language` MissingInCreationsForge - Expected DTO path 'Name[1].Language' was not populated.
- `Name[1].String` MissingInCreationsForge - Expected DTO path 'Name[1].String' was not populated.
- `Name[2].Language` MissingInCreationsForge - Expected DTO path 'Name[2].Language' was not populated.
- 18 more failing fields in the JSON report.

### Starfield KYWD 200AE9:Starfield.esm

- EditorID: CCT_Enviro_Spook
- Spriggit file: `Keywords/CCT_Enviro_Spook - 200AE9_Starfield.esm.yaml`
- Failing counts: MissingInCreationsForge: 4, ValueMismatch: 1

Top failing fields:
- `AttractionRule` MissingInCreationsForge - Expected DTO path 'AttractionRule' was not populated.
- `FNAM` MissingInCreationsForge - Expected DTO path 'FNAM' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Color` ValueMismatch - Spriggit `#00FFFFFF` vs DTO `Color [A=0, R=255, G=255, B=255]`.


## Highest Priority Findings

- Starfield KYWD 200AEB:Starfield.esm `AttractionRule` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Color` ValueMismatch
- Starfield KYWD 200AEB:Starfield.esm `FNAM` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name.Count` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name.TargetLanguage` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[0].Language` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[0].String` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[1].Language` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[1].String` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[2].Language` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[2].String` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[3].Language` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[3].String` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[4].Language` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[4].String` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[5].Language` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[5].String` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[6].Language` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[6].String` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[7].Language` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[7].String` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[8].Language` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Name[8].String` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `Version2` MissingInCreationsForge
- Starfield KYWD 200AEB:Starfield.esm `VersionControl` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `AttractionRule` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Color` ValueMismatch
- Starfield KYWD 145388:Starfield.esm `FNAM` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name.Count` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name.TargetLanguage` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[0].Language` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[0].String` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[1].Language` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[1].String` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[2].Language` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[2].String` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[3].Language` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[3].String` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[4].Language` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[4].String` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[5].Language` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[5].String` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[6].Language` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[6].String` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[7].Language` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[7].String` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[8].Language` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Name[8].String` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `Version2` MissingInCreationsForge
- Starfield KYWD 145388:Starfield.esm `VersionControl` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `AttractionRule` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Color` ValueMismatch
- Starfield KYWD 200ADF:Starfield.esm `FNAM` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name.Count` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name.TargetLanguage` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[0].Language` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[0].String` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[1].Language` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[1].String` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[2].Language` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[2].String` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[3].Language` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[3].String` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[4].Language` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[4].String` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[5].Language` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[5].String` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[6].Language` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[6].String` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[7].Language` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[7].String` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[8].Language` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Name[8].String` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `Version2` MissingInCreationsForge
- Starfield KYWD 200ADF:Starfield.esm `VersionControl` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Color` ValueMismatch
- Starfield KYWD 1C84DD:Starfield.esm `FNAM` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name.Count` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name.TargetLanguage` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[0].Language` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[0].String` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[1].Language` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[1].String` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[2].Language` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[2].String` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[3].Language` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[3].String` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[4].Language` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[4].String` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[5].Language` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[5].String` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[6].Language` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[6].String` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[7].Language` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[7].String` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[8].Language` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Name[8].String` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `Version2` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `VersionControl` MissingInCreationsForge
- Starfield KYWD 1C84DD:Starfield.esm `WAIM` MissingInCreationsForge
