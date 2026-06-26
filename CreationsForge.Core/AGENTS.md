# CreationsForge.Core rules

This folder contains game-agnostic domain models, DTOs, services, repository interfaces, repositories, stores, factories, and shared import/comparison behavior.

## Boundaries

- Core must stay UI-neutral. Do not reference Avalonia, WPF, WinUI, MAUI, view models, commands, dialogs, navigation, controls, Dispatcher, or UI binding primitives.
- Core must not reference console entry-point concerns such as command-line parsing or console output formatting.
- Game-specific Mutagen mapping belongs in CreationsForge.Starfield, CreationsForge.Fallout4, or CreationsForge.Skyrim unless the code is truly game-agnostic.
- Core may expose plain DTOs, domain models, result objects, progress DTOs, callbacks, events, async methods, and collection interfaces for presentation projects to consume.

## Services, stores, and repositories

- Services own orchestration, validation, transaction coordination when existing patterns do so, and Serilog summaries.
- Repositories must stay small and persistence-focused.
- Do not put business workflow logic in repositories.
- Do not log from repositories or stores.
- Use NPoco and parameterized SQL for runtime values.
- Do not concatenate plugin names, paths, ModKey values, FormKey values, FormID values, RecordType, EditorID, or search text into SQL.
- Do not introduce EF Core, Dapper, another SQLite provider, or another database package without explicit approval.

## Domain and records

- Preserve project terminology from Documentation/DOMAIN-MODEL.md.
- Treat shared record header data as canonical in the existing shared header/record instance model.
- Do not duplicate header fields into detail DTOs or tables unless an existing pattern requires it and the plan explains why.
- FormID values must remain stable six-character uppercase hexadecimal text where that convention is used.
- ModKey and path casing behavior must follow the repository's normalization strategy.

## First-class record modeling

Core DTOs and repositories must represent real Creation Engine/Spriggit concepts, not storage escape hatches.

Before adding a shared child table or DTO capability, identify the source Spriggit path and the shared behavior it models. Do not add generic key/value, raw payload, JSON, reflected value, or catch-all DTOs for known Spriggit fields.

VMAD must be treated as Virtual Machine Adapter data. Papyrus scripts, script properties, property list items, aliases, object references, and script fragments are script-domain data and must use VMAD/script DTOs and tables rather than `StructuredRecordValues` or `RawRecordPayloads`.

If an existing Core path reads data from `StructuredRecordValues`, comparison and repository work that touches that path must consider whether the touched data should be promoted to a typed DTO/table in the same approved task.

## Deferral reminder

The root Deferral / incomplete work rules apply here. If this project imports, persists, reads, compares, or renders a record path, the matching downstream path is in scope unless the approved plan explicitly excludes it.

## Validation

Unit tests should target testable service, factory, validator, DTO, and pure business logic. Do not add unit tests for repository implementations, database access, or DbUp migration execution unless the root rules change.
