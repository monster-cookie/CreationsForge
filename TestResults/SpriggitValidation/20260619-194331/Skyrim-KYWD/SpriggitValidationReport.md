# Spriggit Data Validation Report

- Run date UTC: 2026-06-19T19:43:31.6789891Z
- Scope: Skyrim KYWD
- Records compared: 4
- Field comparisons: 36
- JSON details: C:\Repositories\Personal\CreationsForge\TestResults\SpriggitValidation\20260619-194331\Skyrim-KYWD\SpriggitValidationReport.json

## Category Counts

- Match: 12
- MissingInCreationsForge: 8
- MissingInSpriggit: 13
- ValueMismatch: 3

## Failed Records

### Skyrim KYWD 10EAD7:Skyrim.esm

- EditorID: ActorTypeFamiliar
- Spriggit file: `Keywords/ActorTypeFamiliar - 10EAD7_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 2, ValueMismatch: 1

Top failing fields:
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Color` ValueMismatch - Spriggit `#00FFFFFF` vs DTO `Color [A=0, R=255, G=255, B=255]`.

### Skyrim KYWD 10E984:Skyrim.esm

- EditorID: ActorTypeGiant
- Spriggit file: `Keywords/ActorTypeGiant - 10E984_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 2, ValueMismatch: 1

Top failing fields:
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Color` ValueMismatch - Spriggit `#00380047` vs DTO `Color [A=0, R=56, G=0, B=71]`.

### Skyrim KYWD 0F5D16:Skyrim.esm

- EditorID: ActorTypeTroll
- Spriggit file: `Keywords/ActorTypeTroll - 0F5D16_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 2, ValueMismatch: 1

Top failing fields:
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.
- `Color` ValueMismatch - Spriggit `#000D0000` vs DTO `Color [A=0, R=13, G=0, B=0]`.

### Skyrim KYWD 06DEAD:Skyrim.esm

- EditorID: ActivatorLever
- Spriggit file: `Keywords/ActivatorLever - 06DEAD_Skyrim.esm.yaml`
- Failing counts: MissingInCreationsForge: 2

Top failing fields:
- `Version2` MissingInCreationsForge - Expected DTO path 'Version2' was not populated.
- `VersionControl` MissingInCreationsForge - Expected DTO path 'VersionControl' was not populated.


## Highest Priority Findings

- Skyrim KYWD 10EAD7:Skyrim.esm `Color` ValueMismatch
- Skyrim KYWD 10EAD7:Skyrim.esm `Version2` MissingInCreationsForge
- Skyrim KYWD 10EAD7:Skyrim.esm `VersionControl` MissingInCreationsForge
- Skyrim KYWD 10E984:Skyrim.esm `Color` ValueMismatch
- Skyrim KYWD 10E984:Skyrim.esm `Version2` MissingInCreationsForge
- Skyrim KYWD 10E984:Skyrim.esm `VersionControl` MissingInCreationsForge
- Skyrim KYWD 0F5D16:Skyrim.esm `Color` ValueMismatch
- Skyrim KYWD 0F5D16:Skyrim.esm `Version2` MissingInCreationsForge
- Skyrim KYWD 0F5D16:Skyrim.esm `VersionControl` MissingInCreationsForge
- Skyrim KYWD 06DEAD:Skyrim.esm `Version2` MissingInCreationsForge
- Skyrim KYWD 06DEAD:Skyrim.esm `VersionControl` MissingInCreationsForge
