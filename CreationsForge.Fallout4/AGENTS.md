# CreationsForge.Fallout4 rules

This folder contains Fallout 4-specific Mutagen mapping, DTOs, services, repositories, importers, factories, and record
support.

## Boundaries

- Keep Fallout 4-specific Mutagen APIs and record quirks in this project.
- Do not leak Fallout 4-only assumptions into CreationsForge.Core.
- Shared game-agnostic abstractions belong in Core only after the plan explains why they apply to multiple games.
- Do not reference Avalonia or presentation-only types.

## Reference reminder

The root Reference and documentation section applies here. For record mapping work, inspect Mutagen documentation/source and the matching Spriggit YAML extraction for this game before choosing properties, child collections, or schema shape.

## Mutagen and Spriggit

- Inspect the installed Mutagen package, current repository usage, official Mutagen documentation/source, and Spriggit output before choosing APIs.
- Do not invent Mutagen record properties or collections from record type names.
- Preserve Mutagen/Spriggit terminology where it is already the project convention.
- For every new record type, the plan must identify:
  - Mutagen type/interface/class.
  - Collection/property used to enumerate records.
  - Non-header fields to persist.
  - Field shape: scalar, FormKey reference, enum/flags, structured value, or collection.
  - Child tables, if any.
  - Fields intentionally skipped and why.

## Record import behavior

- Import only from plugins whose import state allows record import according to existing behavior.
- Preserve the existing plugin invalidation and replace-by-plugin pattern.
- Write the shared header/record instance row before detail and child rows.
- Delete stale detail and child rows for the ModKey and record type before reinserting when using replace-by-plugin import.
- Continue importing other plugins or record types when safe, but log failures from the service layer.
- Do not leave stale detail rows after reimport.
- Do not add or expand raw payload storage unless the plan explicitly justifies it or preserves an existing approved pattern.

## Cross-game consistency

- When implementing a record type that exists across Starfield, Fallout 4, and Skyrim, either plan the equivalent game support or list the game-specific exclusion under Out of scope in the plan and wait for approval.
- If Fallout 4 differs from another game's record shape, preserve the difference in the game-specific DTO/schema/service rather than forcing a false shared abstraction.

## Deferral reminder

The root Deferral / incomplete work rules apply here. If this project imports, persists, reads, compares, or renders a record path, the matching downstream path is in scope unless the approved plan explicitly excludes it.

## Validation

Use existing converted record types as templates. Build/test validation is required, and manual comparison against Spriggit output should be planned for mapping-heavy record work when practical.
