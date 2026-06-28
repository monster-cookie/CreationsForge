# Contributing to CreationsForge

CreationsForge is a .NET desktop and console toolchain for inspecting, importing, comparing, and previewing
Bethesda plugin data across Starfield, Fallout 4, and Skyrim.

The project is still early enough that naming and data shape choices matter a lot. Contributions should preserve the
main design goal: model Bethesda plugin data using canonical Spriggit, Mutagen, xEdit, and Creation Kit terminology
wherever possible.

## Start Here

Before making a non-trivial change, please read the documentation that matches the area you are touching:

- `Documentation/SYSTEM-OVERVIEW.md`
- `Documentation/ARCHITECTURE.md`
- `Documentation/DOMAIN-MODEL.md`
- `Documentation/NAMING-CONVENTIONS.md`
- `Documentation/Database/DATABASE.md` for persistence changes
- `Documentation/DESIGN-DECISIONS.md` for architectural context

For record mapping, prefer source references over guesses:

- Mutagen documentation
- Mutagen source code
- Spriggit source code
- Local Spriggit YAML extractions, when available

## Naming

Use `CreationsForge` consistently in code comments, documentation, examples, release notes, paths, and user-facing text.

Record models, DTOs, database columns, importer fields, comparison output, and validation paths should use canonical
Bethesda, Spriggit, Mutagen, xEdit, or Creation Kit naming. Avoid replacing domain names with generic game terms.

Good examples:

- `FormKey`
- `EditorID`
- `FormList`
- `GameSetting`
- `Global`
- `ObjectBounds`

Avoid generic substitutions when the source format has a clear name. For example, avoid `Id` when the domain concept is
`FormKey`, or `Name` when the source field is `EditorID`.

## Project Layout

- `CreationsForge`: Avalonia presentation application, views, view models, dialogs, navigation, and asset preview UI.
- `CreationsForge.Console`: command-line entry points and console orchestration.
- `CreationsForge.Bootstrap`: Autofac composition, startup wiring, logging setup, and shared registration.
- `CreationsForge.Core`: shared domain models, DTOs, services, repositories, stores, factories, and comparison behavior.
- `CreationsForge.Migrations`: DbUp migration runner and SQLite migration scripts.
- `CreationsForge.Bethesda.Assets`: BA2/BSA archive and asset lookup behavior.
- `CreationsForge.Starfield`: Starfield-specific Mutagen mapping and import behavior.
- `CreationsForge.Fallout4`: Fallout 4-specific Mutagen mapping and import behavior.
- `CreationsForge.Skyrim`: Skyrim-specific Mutagen mapping and import behavior.
- `CreationsForge.UnitTests`: xUnit tests for testable non-UI behavior.
- `CreationsForge.PresentationTests`: Avalonia/headless presentation tests and manual harness-style validation.
- `Documentation`: durable architecture, domain, workflow, database, and design-decision documentation.

## Code Style

- Keep changes focused and avoid unrelated formatting churn.
- Do not introduce new third-party dependencies without maintainer discussion.
- Do not use C# primary constructors unless the project explicitly adopts them.
- Use meaningful XML documentation comments for new or modified C# types and members.
- Prefer clear, ordinary code over clever abstractions.
- Use Autofac for dependency injection.
- Use Serilog structured logging templates.
- Use NPoco for SQLite access.
- Use DbUp for schema migrations.

## Record Import Changes

When adding or changing support for a record family, make sure the complete path is considered:

- DTO and model shape
- importer mapping
- persistence schema
- repository save and readback behavior
- comparison and render output
- UI or view model exposure, when relevant
- validation coverage
- documentation updates

Do not store structured Spriggit-visible fields in a generic payload bucket. If Spriggit or Mutagen exposes a stable
structured value, it should usually be modeled as first-class data.

Starfield, Fallout 4, and Skyrim support should be consistent where a record type exists across games. If a record is
game-specific, make that clear in the implementation and documentation.

## Database Changes

Schema changes belong in `CreationsForge.Migrations` and should use DbUp migrations.

When a change affects persisted schema, update both database documentation files:

- `Documentation/Database/DATABASE.md`
- `Documentation/Database/ERD.md`

The documentation should describe the final migrated schema, not only the migration script that introduced the change.
ERD relationship lines should reflect declared SQLite foreign keys only.

## Tests and Validation

Use the existing test projects and patterns. Common validation commands are:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

For targeted changes, targeted test commands are often more useful:

```powershell
dotnet test ./CreationsForge.UnitTests/CreationsForge.UnitTests.csproj --no-build
dotnet test ./CreationsForge.PresentationTests/CreationsForge.PresentationTests.csproj --no-build
```

Please do not report validation as passing unless the command actually ran and passed. If a change requires local SQLite
data to be reset or records to be reimported before validation is meaningful, call that out clearly.

## Documentation

Documentation is treated as durable project knowledge. Update it when changing:

- architecture
- domain behavior
- public interfaces
- persistence behavior
- database schema
- dependency injection
- logging behavior
- workflows
- validation behavior
- UI workflows

`Documentation/CHANGE-LOG.md` and `Documentation/KNOWN-ISSUES.md` are maintained by project maintainers. Do not update
them unless specifically asked.

## Pull Request Checklist

Before opening a pull request, please check that:

- The change uses canonical Spriggit, Mutagen, xEdit, or Creation Kit terminology where applicable.
- No unrelated formatting churn was introduced.
- New or modified C# symbols have meaningful XML documentation.
- Database documentation was updated if persisted schema changed.
- Relevant tests were added or updated.
- Relevant validation commands were run.
- Any local data reset or reimport requirement is clearly described.
- No secrets, credentials, private paths, generated logs, or machine-specific data were included.
