# Naming Conventions

## Purpose

This document defines naming conventions for imported Bethesda record DTOs, persistence models, validation helpers, and future authoring surfaces. The goal is to keep CreationsForge aligned with Spriggit, Mutagen, xEdit, and Creation Kit terminology while still allowing the database to use storage-specific shapes.

These conventions are intended for new v2 reset work and for touched record paths. They do not claim every existing record DTO or repository already follows this shape.

## Canonical Naming Priority

When naming a record field, prefer terminology in this order:

1. Spriggit YAML field path.
2. Mutagen property name.
3. xEdit, SFCK, CK, or commonly understood Creation Engine terminology.
4. A CreationsForge-specific name only when the source tools disagree or the app needs a clearer domain concept.

If Spriggit and Mutagen disagree, document the chosen name in the approved task plan before implementing the mapping. Do not invent a different name only to describe the .NET value type.

## DTO Property Names

DTO property names should describe the source record concept, not the storage representation.

Use the canonical field name when the property type already communicates the shape:

```csharp
public FormKeyDTO? Race { get; set; }
```

Avoid adding type suffixes when the source field does not use them:

```csharp
public FormKeyDTO? RaceFormKey { get; set; }
```

The same rule applies to references such as `Voice`, `CombatStyle`, `CreatedObject`, `WorkbenchKeyword`, `Menu`, and `FurnitureTemplate`. The property type should tell readers that the value is a `FormKeyDTO`; the property name should stay aligned with Spriggit, Mutagen, xEdit, and CK terminology.

Suffixes such as `FormKey`, `Text`, `Raw`, `Data`, or `DTO` should be used only when:

- the source field uses that suffix;
- two source fields would otherwise collide;
- the suffix identifies a real domain concept rather than a storage detail;
- the approved plan explains why the canonical source name is not sufficient.

## Attributes And Boundary Mapping

Attributes are appropriate for boundary mapping metadata. They should make mappings explicit without turning aliases into permanent domain vocabulary.

Good uses include:

- mapping a canonical DTO property to a Spriggit path when the path is nested or otherwise not inferable;
- mapping a canonical DTO property to decomposed database columns;
- identifying translated fields for localized-string persistence;
- marking temporary compatibility aliases during an approved reset.

For example, a future mapping might express:

```csharp
[SpriggitPath("Race")]
[FormKeyColumns("Race")]
public FormKeyDTO? Race { get; set; }
```

The property remains `Race`, while database storage may still decompose the value into columns such as
`Race_ModKey_Name`, `Race_ModKey_Type`, `Race_ModKey_FileName`, and `Race_FormKey_ID`.

Do not use attributes to keep multiple competing names alive indefinitely. If a legacy alias is needed, the approved plan should explain why it exists, where it is consumed, and when it can be removed.

## Interfaces

Interfaces should describe shared Creation Engine capabilities consumed by shared infrastructure. They should not be decorative markers.

Good interface candidates include common identity and record capabilities such as:

- `IHasModKey`
- `IHasFormKey`
- `IHasEditorID`
- `IHasTranslatedFields`
- existing shared child capabilities such as model, keyword, sound, scripting-adapter, component, raw-payload, and condition capabilities

Use an interface when a shared importer, repository, comparison service, localization service, validation helper, asset preview service, or authoring workflow can act on that capability generically.

Avoid adding one-off interfaces for fields that only one game or one record type uses. Game-specific fields should remain plain properties unless shared infrastructure needs to consume them.

## Game-Specific Fields

Game-specific DTO fields should stay game-specific. Do not force a field into a shared interface or shared base shape only because one game exposes it.

For example, Starfield-only book fields such as data-slate headers or Starfield component payloads can remain normal Starfield-specific properties unless another supported game exposes the same concept or shared infrastructure needs a generic capability.

Shared DTOs and interfaces should represent real cross-game behavior. Game adapter projects remain responsible for game-specific Mutagen quirks and record-shape differences.

## Localized Fields

Translated text should be discoverable by shared localization infrastructure. Prefer a translated-field capability or localized-field metadata over bespoke per-record localization logic.

The source field path should remain canonical:

- `Name` should map to localized source field `Name`.
- `Description` should map to localized source field `Description`.
- If the source field is `BookText` but the shared capability is book/body text, the DTO may use `Text` with
  localized-field and Spriggit-path metadata mapping non-Starfield sources back to `BookText`.

Localized string persistence should store the source field path needed to round-trip or compare against Spriggit. If a UI label needs friendlier wording, keep that label in presentation code rather than renaming the DTO field.

## Database Naming

DTO and domain names should stay canonical. Database names may reflect relational storage needs.

For FormKey references, use decomposed columns based on the canonical source field name:

- `Race_ModKey_Name`
- `Race_ModKey_Type`
- `Race_ModKey_FileName`
- `Race_FormKey_ID`

Avoid making DTOs adopt database-specific suffixes solely because the database decomposes a value.

Avoid prefixing table names with `Record` unless the table stores generic record infrastructure, such as
`RecordInstances`.

Shared child tables should use the capability name:

- `Components`
- `ComponentItems`
- `Models`
- `RawRecordPayloads`

Use a `Mappings` suffix when the table primarily associates a parent record with another record/form key rather than
owning the child data:

- `KeywordMappings`
- `SoundMappings`

Imported plugin and typed-record data is currently cache data that can be rebuilt from source plugins. Future user-authored data must be kept separate from imported cache data so cache schema resets do not destroy user content.

## Validation Harness Naming

Validation should compare canonical Spriggit paths to canonical DTO fields. If the field is named `Race` in Spriggit and Mutagen, validation should prefer `dtoFields["Race"]`, not `dtoFields["RaceFormKey"]`.

Aliases in validation helpers are allowed only when they represent boundary mapping that is deliberately documented, such as:

- shared child storage for models, sounds, scripts, components, or raw payloads;
- database decomposition that is hidden from the DTO surface;
- temporary compatibility behavior approved by the task plan.

The validation harness should make divergence visible. It should not make alias drift feel normal.

## Comparison And UI Labels

Comparison row names should follow canonical DTO field names unless the UI intentionally presents a friendlier label. Presentation labels may be different from source field names, but those labels should not feed back into DTO, repository, importer, or validation naming.

The UI may display `Race` as `Race`, `BookText` as `Book Text`, or a grouped child section as `Models`. Those are presentation choices. Core record contracts should keep source-aligned names.

## Reset Guidance

For v2 reset work, prefer renaming the core contract toward canonical source names rather than adding more aliases. Use attributes and interfaces to describe mappings and shared behavior. Do not use them to preserve stale names as the primary model.

When a reset changes imported cache schema or persisted read-back behavior, existing local SQLite cache data should be treated as stale and reset or reimported before validation results are considered meaningful.

Reset migrations must create the final schema directly. Do not carry forward `ALTER TABLE`, data-copy, invalidation
`UPDATE`, or compatibility cleanup statements from earlier migrations unless the reset intentionally seeds data.
