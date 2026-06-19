# Spriggit Data Validation Report

- Run date UTC: 2026-06-19T19:43:33.3597371Z
- Scope: Fallout4 KYWD
- Records compared: 5
- Field comparisons: 124
- JSON details: C:\Repositories\Personal\CreationsForge\TestResults\SpriggitValidation\20260619-194333\Fallout4-KYWD\SpriggitValidationReport.json

## Category Counts

- Match: 18
- MissingInCreationsForge: 84
- MissingInSpriggit: 17
- ValueMismatch: 5

## Failed Records

### Fallout4 KYWD 119B9B:Fallout4.esm

- EditorID: 02Metal03Floor
- Spriggit file: `Keywords/02Metal03Floor - 119B9B_Fallout4.esm.yaml`
- Failing counts: MissingInCreationsForge: 26, ValueMismatch: 1

Top failing fields:
- `Name.Count` MissingInCreationsForge - Expected DTO path 'Name.Count' was not populated.
- `Name.TargetLanguage` MissingInCreationsForge - Expected DTO path 'Name.TargetLanguage' was not populated.
- `Name[0].Language` MissingInCreationsForge - Expected DTO path 'Name[0].Language' was not populated.
- `Name[0].String` MissingInCreationsForge - Expected DTO path 'Name[0].String' was not populated.
- `Name[10].Language` MissingInCreationsForge - Expected DTO path 'Name[10].Language' was not populated.
- `Name[10].String` MissingInCreationsForge - Expected DTO path 'Name[10].String' was not populated.
- `Name[1].Language` MissingInCreationsForge - Expected DTO path 'Name[1].Language' was not populated.
- `Name[1].String` MissingInCreationsForge - Expected DTO path 'Name[1].String' was not populated.
- 19 more failing fields in the JSON report.

### Fallout4 KYWD 119B9C:Fallout4.esm

- EditorID: 02Metal03Misc
- Spriggit file: `Keywords/02Metal03Misc - 119B9C_Fallout4.esm.yaml`
- Failing counts: MissingInCreationsForge: 26, ValueMismatch: 1

Top failing fields:
- `Name.Count` MissingInCreationsForge - Expected DTO path 'Name.Count' was not populated.
- `Name.TargetLanguage` MissingInCreationsForge - Expected DTO path 'Name.TargetLanguage' was not populated.
- `Name[0].Language` MissingInCreationsForge - Expected DTO path 'Name[0].Language' was not populated.
- `Name[0].String` MissingInCreationsForge - Expected DTO path 'Name[0].String' was not populated.
- `Name[10].Language` MissingInCreationsForge - Expected DTO path 'Name[10].Language' was not populated.
- `Name[10].String` MissingInCreationsForge - Expected DTO path 'Name[10].String' was not populated.
- `Name[1].Language` MissingInCreationsForge - Expected DTO path 'Name[1].Language' was not populated.
- `Name[1].String` MissingInCreationsForge - Expected DTO path 'Name[1].String' was not populated.
- 19 more failing fields in the JSON report.

### Fallout4 KYWD 119B9D:Fallout4.esm

- EditorID: 02Metal03Prefabs
- Spriggit file: `Keywords/02Metal03Prefabs - 119B9D_Fallout4.esm.yaml`
- Failing counts: MissingInCreationsForge: 26, ValueMismatch: 1

Top failing fields:
- `Name.Count` MissingInCreationsForge - Expected DTO path 'Name.Count' was not populated.
- `Name.TargetLanguage` MissingInCreationsForge - Expected DTO path 'Name.TargetLanguage' was not populated.
- `Name[0].Language` MissingInCreationsForge - Expected DTO path 'Name[0].Language' was not populated.
- `Name[0].String` MissingInCreationsForge - Expected DTO path 'Name[0].String' was not populated.
- `Name[10].Language` MissingInCreationsForge - Expected DTO path 'Name[10].Language' was not populated.
- `Name[10].String` MissingInCreationsForge - Expected DTO path 'Name[10].String' was not populated.
- `Name[1].Language` MissingInCreationsForge - Expected DTO path 'Name[1].Language' was not populated.
- `Name[1].String` MissingInCreationsForge - Expected DTO path 'Name[1].String' was not populated.
- 19 more failing fields in the JSON report.

### Fallout4 KYWD 0CF43E:Fallout4.esm

- EditorID: AO_BoS_ScribeCollectData
- Spriggit file: `Keywords/AO_BoS_ScribeCollectData - 0CF43E_Fallout4.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 1

Top failing fields:
- `AttractionRule` MissingInCreationsForge - Expected DTO path 'AttractionRule' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Color` ValueMismatch - Spriggit `#00FFFFFF` vs DTO `Color [A=0, R=255, G=255, B=255]`.

### Fallout4 KYWD 093BBE:Fallout4.esm

- EditorID: if_Armor_Combat_Freefall_Restricted
- Spriggit file: `Keywords/if_Armor_Combat_Freefall_Restricted - 093BBE_Fallout4.esm.yaml`
- Failing counts: MissingInCreationsForge: 3, ValueMismatch: 1

Top failing fields:
- `MajorRecordFlagsRaw` MissingInCreationsForge - Expected DTO path 'MajorRecordFlagsRaw' was not populated.
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Color` ValueMismatch - Spriggit `#00FFFFFF` vs DTO `Color [A=0, R=255, G=255, B=255]`.


## Highest Priority Findings

- Fallout4 KYWD 119B9B:Fallout4.esm `Color` ValueMismatch
- Fallout4 KYWD 119B9B:Fallout4.esm `Name.Count` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name.TargetLanguage` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[0].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[0].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[10].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[10].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[1].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[1].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[2].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[2].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[3].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[3].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[4].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[4].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[5].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[5].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[6].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[6].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[7].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[7].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[8].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[8].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[9].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Name[9].String` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `Version2` MissingInCreationsForge
- Fallout4 KYWD 119B9B:Fallout4.esm `VersionControl` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Color` ValueMismatch
- Fallout4 KYWD 119B9C:Fallout4.esm `Name.Count` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name.TargetLanguage` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[0].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[0].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[10].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[10].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[1].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[1].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[2].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[2].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[3].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[3].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[4].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[4].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[5].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[5].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[6].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[6].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[7].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[7].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[8].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[8].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[9].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Name[9].String` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `Version2` MissingInCreationsForge
- Fallout4 KYWD 119B9C:Fallout4.esm `VersionControl` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Color` ValueMismatch
- Fallout4 KYWD 119B9D:Fallout4.esm `Name.Count` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name.TargetLanguage` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[0].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[0].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[10].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[10].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[1].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[1].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[2].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[2].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[3].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[3].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[4].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[4].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[5].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[5].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[6].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[6].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[7].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[7].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[8].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[8].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[9].Language` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Name[9].String` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `Version2` MissingInCreationsForge
- Fallout4 KYWD 119B9D:Fallout4.esm `VersionControl` MissingInCreationsForge
- Fallout4 KYWD 0CF43E:Fallout4.esm `AttractionRule` MissingInCreationsForge
- Fallout4 KYWD 0CF43E:Fallout4.esm `Color` ValueMismatch
- Fallout4 KYWD 0CF43E:Fallout4.esm `Version2` MissingInCreationsForge
- Fallout4 KYWD 0CF43E:Fallout4.esm `VersionControl` MissingInCreationsForge
- Fallout4 KYWD 093BBE:Fallout4.esm `Color` ValueMismatch
- Fallout4 KYWD 093BBE:Fallout4.esm `MajorRecordFlagsRaw` MissingInCreationsForge
- Fallout4 KYWD 093BBE:Fallout4.esm `Version2` MissingInCreationsForge
- Fallout4 KYWD 093BBE:Fallout4.esm `VersionControl` MissingInCreationsForge
