# Creations Forge agent instructions

Creations Forge is a .NET desktop and console toolchain for inspecting, importing, comparing, and previewing Bethesda plugin data across Starfield, Fallout 4, and Skyrim. The repository uses Avalonia, Autofac, Serilog, SQLite, NPoco, bUp, Mutagen, and Spriggit-oriented validation workflows.

## Project layout

## Naming and terminology

- The project name is CreationsForge.
- Use CreationsForge in code comments, documentation, prompts, examples, paths, release notes, and user-facing text.
- Treat older project names as stale migration artifacts. When touched, replace them with CreationsForge unless preserving historical external references is explicitly required and approved.
- New files must not introduce legacy project names.

- CreationsForge: Avalonia presentation application, views, view models, UI services, navigation, dialogs, and asset preview UI.
- CreationsForge.Console: command-line entry points and console orchestration.
- CreationsForge.Bootstrap: Autofac composition, startup wiring, logging setup, and shared application registration.
- CreationsForge.Core: game-agnostic domain models, DTOs, services, repositories, stores, factories, and shared import/comparison behavior.
- CreationsForge.Migrations: DbUp migration runner and SQLite migration scripts.
- CreationsForge.Bethesda.Assets: BA2/BSA/archive and asset lookup behavior.
- CreationsForge.Starfield: Starfield-specific Mutagen mapping and import behavior.
- CreationsForge.Fallout4: Fallout 4-specific Mutagen mapping and import behavior.
- CreationsForge.Skyrim: Skyrim-specific Mutagen mapping and import behavior.
- CreationsForge.UnitTests: xUnit unit tests for testable non-UI behavior.
- CreationsForge.PresentationTests: Avalonia/headless presentation tests and manual harness-style validation.
- Documentation: durable architecture, domain, workflow, database, and design-decision documentation.
- .github: GitHub workflows, release packaging, and repository automation.

## Repo-wide safety rules

- Never run git commands unless the user explicitly asks for a specific git action.
- Never modify repository history, create commits, create branches, open pull requests, push, pull, fetch, merge, rebase, tag, stash, or reset unless explicitly requested.
- Never edit AGENTS.md, AGENTS.override.md, AGENT-PLAN-TEMPLATE.md, or other agent-instruction files directly. Propose the changes and wait for explicit approval.
- Always produce a plan and wait for explicit approval before editing files.
- After approval, make only the approved edits. Stop and ask before editing new paths or expanding the scope.
- Keep changes surgical and consistent with existing patterns and naming.
- Avoid unrelated formatting churn, project-wide cleanup, or broad rewrites.
- Do not introduce new third-party dependencies, frameworks, build tools, package managers, or CI actions without explicit approval in the plan.
- Do not claim build, test, packaging, migration, import, or validation success unless the command actually ran.
- If validation cannot run, report the exact command, the failure or blocker, and whether it appears environmental.
- Do not add secrets, credentials, tokens, connection strings, private keys, personal paths, or machine-specific data to source files, docs, test fixtures, logs, generated output, or workflow files.

## Reference and documentation

Use these as primary references:

- [Mutagen Documentation](https://mutagen-modding.github.io/Mutagen/)
- [Mutagen Code Repository](https://github.com/Mutagen-Modding/Mutagen)
- [Spriggit Code Repository - Uses Mutagen to export plugins as YAML](https://github.com/Mutagen-Modding/Spriggit)
- [Fallout4.esm Spriggit converted to YAML](C:\FalloutExtractions\Spriggit\Fallout4.esm)
- [Skyrim.esm Spriggit converted to YAML](C:\SkyrimExtractions\Spriggit\Skyrim.esm)
- [Starfield.esm Spriggit converted to YAML](C:\StarfieldExtractions\Spriggit\Starfield.esm)

When mapping Bethesda records, you must prefer these references over guessing from record names. If the local Spriggit extraction folders are not available on the current machine, you must call that out in the plan and use the available Mutagen/Spriggit source references instead of inventing fields.

## Project knowledge and design documentation

You must treat repo documentation as durable project knowledge.

Primary project knowledge files:

- /Documentation/SYSTEM-OVERVIEW.md - Current system purpose, major workflows, project boundaries, and high-level
  architecture.
- /Documentation/ARCHITECTURE.md - Layering rules, Core vs presentation responsibilities, dependency direction, DI
  composition, persistence boundaries, and logging conventions.
- /Documentation/DESIGN-DECISIONS.md - Important design decisions, tradeoffs, rejected alternatives, and rationale.
- /Documentation/DOMAIN-MODEL.md - Important domain concepts, record comparison terminology, Mutagen concepts used by
  the app, and project-specific naming.
- /Documentation/Database/DATABASE.md - SQLite, NPoco, DbUp migration behavior, schema ownership, and persistence
  conventions.
- /Documentation/Database/ERD.md - Entity-Relationship Diagram (ERD) of the database schema, including tables,
  relationships, and constraints.
- /Documentation/CHANGE-LOG.md - Log of the high level changes for each release. Do not modify or maintain this file;
  human project managers will maintain this file.
- /Documentation/KNOWN-ISSUES.md - List of current known issues and workarounds. Do not modify or maintain this file;
  human project managers will maintain this file.

Before planning a non-trivial change, you must read the relevant docs in /Documentation in addition to AGENTS.md.

When a change adds, removes, or meaningfully changes architecture, domain behavior, database schema, persistence behavior, dependency injection, logging behavior, workflow, or public interfaces, the PLAN must include a Documentation impacts section.

Documentation updates must be proposed in the PLAN and require approval before editing.

Documentation updates should be concise and factual. Do not write speculative documentation. Document the final approved design and observed repo behavior, not guesses.

When documenting design decisions, include:

- Date
- Status: Proposed, Accepted, Superseded, or Rejected
- Context
- Decision
- Rationale
- Alternatives considered
- Consequences
- Related files

Do not duplicate large blocks of code in documentation. Reference file paths, classes, interfaces, services, migrations, and tests instead.

If existing documentation conflicts with code, you must call out the conflict in the PLAN before editing either the code or the docs.

If no documentation update is needed, the PLAN must explicitly state: Documentation impacts: None.

### Database documentation maintenance

When a change adds, removes, renames, changes the type/nullability/default/constraint of, or changes the indexing or foreign-key behavior of any application database table or column, you must update the database documentation in the same approved task.

Required database documentation updates:

- /Documentation/Database/DATABASE.md must describe the complete current persisted schema shape.
- /Documentation/Database/ERD.md must include every application-schema table column in the Mermaid entity blocks.
- ERD relationship lines must show only declared SQLite foreign keys.
- Inferred record-reference columns must remain documented separately from declared SQLite foreign keys.
- DbUp-owned migration metadata tables, including SchemaVersions, must not be treated as application-schema tables in the ERD.
- DATABASE.md must continue to state that DbUp SchemaVersions is the migration-state source of truth.
- If a migration adds a column in a later script, the docs must reflect the final migrated schema, not only the initial create-table script.

For database schema changes, the PLAN must explicitly list /Documentation/Database/DATABASE.md and /Documentation/Database/ERD.md under documentation impacts unless the change demonstrably does not affect persisted schema documentation.

## Planning requirements

Before edits, produce a plan that includes:

- Scope and intent.
- Exact file paths expected to change.
- Code-level checklist.
- UI/Avalonia impacts, if any.
- Data model, persistence, or schema impacts, if any.
- Config, environment variable, path, logging, dependency injection, or workflow impacts, if any.
- Documentation impacts, or the exact statement: Documentation impacts: None.
- Risks and rollback notes.
- Validation plan with specific commands or manual checks.

Use AGENT-PLAN-TEMPLATE.md when present.

## Execution requirements

After approval:

- Make only the approved edits.
- Preserve existing conventions and file organization.
- Keep diffs minimal and focused.
- Prefer targeted changes over sweeping refactors.
- Do not rewrite files only to reformat them.
- Do not move types, rename public members, alter persistence formats, or change user workflows unless the plan explicitly
  covers it.
- Do not use primary constructors unless explicitly requested.
- Use one class per file for C# types unless an existing local pattern requires otherwise.
- Use braces for conditionals and loops.
- Prefer clear, boring code over clever code. This project has enough dragons already.

## Architecture and dependency rules

- Use Autofac for dependency injection.
- Prefer constructor injection.
- Keep container resolution in composition roots only.
- Use Serilog for logging.
- Use structured logging templates, not string interpolation.
- Do not log secrets, credentials, tokens, connection strings, full binary payloads, or large serialized records.
- Do not log from repositories or stores unless an existing local pattern explicitly does so.
- Use NPoco for SQLite database access.
- Use parameterized SQL for runtime values.
- Do not introduce EF Core, Dapper, another SQLite provider, or another database layer without explicit approval.
- Use DbUp migrations for schema changes.
- Treat DbUp SchemaVersions as the migration-state source of truth.
- Use Mutagen and Spriggit as primary references for Bethesda record shape and validation.
- Do not invent Mutagen properties or record collections. Inspect existing code, installed packages, docs/source, and
  Spriggit output before mapping records.
- Keep UI framework code out of CreationsForge.Core.
- Keep game-specific behavior out of CreationsForge.Core unless it is truly shared across supported games.

## Deferral / incomplete work rules

- You must not mark any discovered missing behavior, child record family, UI surface, persistence read path, comparison row, test coverage, or documentation update as "deferred", "follow-up", "out of scope", or "future work" unless the PLAN explicitly lists it under Out of scope and the user approves that PLAN.
- If existing code persists or imports data, the corresponding repository read path, comparison service output, UI render-data path, and applicable tests are in scope unless the PLAN explicitly excludes them.
- When adding or changing a typed record with child collections, the task is not complete until imported child data can be:
  - persisted,
  - read back into DTOs,
  - exposed through comparison/render services,
  - rendered by the UI/view model path when applicable,
  - covered by unit and/or presentation tests.
- If you find any documentation saying a behavior is deferred but code/schema/importers already support it, you must call out the conflict in the PLAN and propose either implementing the missing path or explicitly re-approving the deferral.
- TODO, deferred, follow-up, placeholder, and "not yet implemented" statements may not be added to docs or code comments without explicit approval in the PLAN.

## Record import and cross-game rules

- Starfield, Fallout 4, and Skyrim support should be handled consistently where a record type exists across games.
- If a record type is game-specific or intentionally excluded for a game, list the exclusion under Out of scope in the plan and wait for explicit approval.
- Preserve the existing plugin invalidation and replace-by-plugin import pattern.
- Write shared header/record instance data before detail and child rows.
- Delete stale detail and child rows for a plugin/record type before reinserting where the existing import pattern requires replace-by-plugin behavior.
- Preserve raw payload storage only where an existing approved pattern uses it or the plan explicitly justifies it.
- Do not leave stale rows after reimport.

## Database and schema rules

- SQLite schema changes must be implemented through CreationsForge.Migrations.
- Prefer additive migrations when practical.
- Avoid destructive schema changes unless the plan includes explicit approval, data-loss risk, and rollback notes.
- Keep foreign keys, indexes, nullability, defaults, collations, and checks deliberate and documented.
- Do not add indexes speculatively. Tie indexes to known import, lookup, comparison, search, relationship, or validation paths.
- Update database documentation for persisted schema changes:
  - Documentation/Database/DATABASE.md
  - Documentation/Database/ERD.md
- ERD relationship lines must show declared SQLite foreign keys only.
- DbUp-owned tables, including SchemaVersions, must not be treated as application-schema tables.

## Testing rules

- Use xUnit, Moq, and Shouldly according to existing patterns.
- Add or update tests for testable service, factory, validator, DTO, normalization, and business-rule changes.
- Do not add unit tests for repository implementations, database access, or DbUp migration execution unless explicitly approved.
- Use CreationsForge.PresentationTests for Avalonia/headless UI behavior and UI-facing workflows.
- Do not make tests depend on local game installations, ProgramData state, user profile paths, real private data, or machine-specific configuration unless the test is explicitly skipped or marked when prerequisites are missing.
- Use small deterministic fixtures.
- If tests are not added, explain why in the plan.

## Documentation rules

- Documentation is durable project knowledge, not a scratchpad.
- Use Markdown and wrap prose at 120 characters.
- Update documentation when architecture, domain behavior, database schema, persistence behavior, dependency injection,
  logging behavior, workflows, UI workflows, public interfaces, or validation behavior changes.
- If no documentation update is needed, state that in the plan.
- Do not create new documentation files unless they are listed in the approved plan.
- Keep docs aligned with current code behavior. If code and docs disagree, call out the conflict before editing.

## Standard validation

Use the current solution file name in the repo. Prefer these commands when applicable:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

When a specific project is changed, include targeted commands where useful, for example:

```powershell
dotnet test ./CreationsForge.UnitTests/CreationsForge.UnitTests.csproj --no-build
dotnet test ./CreationsForge.PresentationTests/CreationsForge.PresentationTests.csproj --no-build
```

For migration changes, include local SQLite validation when practical:

```sql
PRAGMA foreign_key_check;
PRAGMA integrity_check;
```

## Nested instructions

This root AGENTS.md provides repo-wide defaults. Nested AGENTS.md files in project folders may add stricter local rules for presentation, console, bootstrap, core, migrations, assets, game-specific imports, tests, documentation, or GitHub workflows. When rules conflict, the more specific nested file should win for work inside that folder.
