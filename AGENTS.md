# Repo: CreationsForge

Multi-game Bethesda plugin import and record persistence prototype. The current console harness is
`CreationsForge.Console`; the `CreationsForge` project name is reserved for the future cross-platform UI.

## Project Layout

- CreationsForge (Cross-platform, Uno Platform Skia Desktop Application)
- CreationsForge.Console (Temporary command-line import/validation harness)
- CreationsForge.Core (Game-agnostic wrapper models, DTOs, services, repositories, configuration stores, and factories)
- CreationsForge.Starfield (Starfield game-specific models, DTOs, services, and repositories)
- CreationsForge.Fallout4 (Fallout 4 game-specific models, DTOs, services, and repositories)
- CreationsForge.Skyrim (Skyrim game-specific models, DTOs, services, and repositories)
- CreationsForge.Migrations (DbUp migrations)
- CreationsForge.UnitTests (Unit tests)
- CreationsForge.PresentationTests (Presentation-layer unit tests for pure UI services, view models, commands, and render-data preparation. Do not start windows, app lifetimes, or GPU/OpenGL contexts in these tests.)

## HARD RULES

- Codex may run read-only git inspection commands listed in GIT BOUNDARIES. Codex must not run git commands that mutate
  repository state unless explicitly approved in a task-specific PLAN.
- NEVER modify repo history or open PRs.
- Default approved read/write scope after PLAN approval:
  - /CreationsForge/**/*
  - /CreationsForge.Console/**/*
  - /CreationsForge.Core/**/*
  - /CreationsForge.Starfield/**/*
  - /CreationsForge.Fallout4/**/*
  - /CreationsForge.Skyrim/**/*
  - /CreationsForge.Migrations/**/*
  - /CreationsForge.UnitTests/**/*
  - /Documentation/**/*
- Do not edit files outside these projects unless explicitly approved in the PLAN.
- ALWAYS show a PLAN first and wait for explicit approval before making the first repository edit for a task.
- After the PLAN is approved, Codex may edit files within the approved scope without asking again for each file.
- If new files, new projects, cross-cutting architecture changes, breaking changes, or edits outside the approved scope are needed, stop and ask for approval.
- Keep changes surgical and consistent with existing patterns and naming.
- Shared/Core workflow code must not branch on a specific supported game such as `plugin.Game == SupportedGame.Starfield`
  unless the PLAN explicitly documents why the behavior is truly game-specific. Prefer game adapter implementations,
  registered capability/support sets, or polymorphic services over hardcoded game checks.
- No breaking changes to existing services, factories, stores, repositories, view models, public interfaces, configuration, persistence formats, or workflows without explicit approval.
- NEVER edit AGENTS.md, AGENT-PLAN-TEMPLATE.md, or any `*.code-workspace` file; if you have suggestions for changes, propose them to the user.
- DO NOT wrap lines of code or comments that are not currently wrapped. Follow existing formatting and line breaks in the repo.
- For documentation, use Markdown format and wrap lines at 120 characters.

## REFERENCE & DOCUMENTATION

Use these as primary references:

- [Mutagen Documentation](https://mutagen-modding.github.io/Mutagen/)
- [Mutagen Code Repository](https://github.com/Mutagen-Modding/Mutagen)
- [Spriggit Code Repository - Uses mutagen to export plugins as YAML](https://github.com/Mutagen-Modding/Spriggit)
- [Fallout4.esm spriggit converted to YAML](C:\FalloutExtractions\Spriggit\Fallout4.esm)
- [Skyrim.esm spriggit converted to YAML](C:\SkyrimExtractions\Spriggit\Skyrim.esm)
- [Starfield.esm spriggit converted to YAML](C:\StarfieldExtractions\Spriggit\Starfield.esm)

## PROJECT KNOWLEDGE & DESIGN DOCUMENTATION

Codex must treat repo documentation as durable project knowledge.

Primary project knowledge files:

- /Documentation/SYSTEM-OVERVIEW.md - Current system purpose, major workflows, project boundaries, and high-level architecture.
- /Documentation/ARCHITECTURE.md - Layering rules, Core vs presentation responsibilities, dependency direction, DI composition, persistence boundaries, and logging conventions.
- /Documentation/DESIGN-DECISIONS.md - Important design decisions, tradeoffs, rejected alternatives, and rationale.
- /Documentation/DOMAIN-MODEL.md - Important domain concepts, record comparison terminology, Mutagen concepts used by the app, and project-specific naming.
- /Documentation/Database/DATABASE.md - SQLite, NPoco, DbUp migration behavior, schema ownership, and persistence conventions.
- /Documentation/Database/ERD.md - Entity-Relationship Diagram (ERD) of the database schema, including tables, relationships, and constraints.
- /Documentation/CHANGE-LOG.md - Log of the high level changes for each release. Do not modify or maintain this file human project mangers will maintain this file.
- /Documentation/KNOWN-ISSUES.md - List of current known issues and workarounds. Do not modify or maintain this file human project mangers will maintain this file.

Before planning a non-trivial change, Codex must read the relevant docs in /Documentation in addition to AGENTS.md.

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

If existing documentation conflicts with code, Codex must call out the conflict in the PLAN before editing either the code or the docs.

If no documentation update is needed, the PLAN must explicitly state: Documentation impacts: None.

### Database documentation maintenance

When a change adds, removes, renames, changes the type/nullability/default/constraint of, or changes the indexing or
foreign-key behavior of any application database table or column, Codex must update the database documentation in the
same approved task.

Required database documentation updates:

- `/Documentation/Database/DATABASE.md` must describe the complete current persisted schema shape.
- `/Documentation/Database/ERD.md` must include every application-schema table column in the Mermaid entity blocks.
- ERD relationship lines must show only declared SQLite foreign keys.
- Inferred record-reference columns must remain documented separately from declared SQLite foreign keys.
- DbUp-owned migration metadata tables, including `SchemaVersions`, must not be treated as application-schema tables
  in the ERD.
- `DATABASE.md` must continue to state that DbUp `SchemaVersions` is the migration-state source of truth.
- If a migration adds a column in a later script, the docs must reflect the final migrated schema, not only the initial
  create-table script.

For database schema changes, the PLAN must explicitly list `/Documentation/Database/DATABASE.md` and
`/Documentation/Database/ERD.md` under documentation impacts unless the change demonstrably does not affect persisted
schema documentation.

## ARCHITECTURE & CONVENTIONS

- Contracts-first for service/core changes: define or update interfaces, DTOs, validators, and applicable tests before implementation. Do not add unit tests for database access, repository implementations, or DbUp migration execution.
- Do not use C# primary constructors for classes. Use traditional explicit constructors instead.
- Use one class per file.
- No statics for application services or mutable app state. Prefer DI; register singletons only when appropriate. Constants, generated framework code, and existing static patterns may remain unless explicitly approved for refactor.
- No repeated code: Refactor existing methods as needed to avoid repeating code in new methods.
- Do not introduce new conventions or dependencies unless explicitly approved in the PLAN. But feel free to stop and suggest them if the new convention may improve code clarity, maintainability, or adhere to best practices.

## TECH CONSTRAINTS

- Dependency injection: Use Autofac.
- Database access: Use NPoco with parameterized SQL.
- Logging and Observability: Use existing Serilog conventions.
- Unit Tests: Use xUnit, Moq, and Shouldly.

## UI / MVVM BOUNDARIES

- UI framework code must stay out of CreationsForge.Core.
- Follow existing MVVM patterns in the repo when present; otherwise establish patterns in the PLAN before adding UI/MVVM code.
- CreationsForge.Core must not reference Uno Platform, WPF, MAUI, WinUI, Avalonia, CommunityToolkit.Maui, or any UI framework package.
- CreationsForge.Core must not contain pages, windows, controls, views, view models, UI commands, dialog services, navigation services, or UI-specific binding helpers.
- CreationsForge.Core must not expose or depend on UI binding primitives such as INotifyPropertyChanged, ObservableCollection<T>, ICommand, Dispatcher, SynchronizationContext-based UI dispatching, or platform UI thread helpers.
- Use plain DTOs, domain models, IReadOnlyList<T>, IEnumerable<T>, result objects, events, callbacks, or progress DTOs for Core-to-presentation communication.
- MVVM presentation code belongs in CreationsForge only, including:
  - WinUI pages/views
  - C# Markup UI classes
  - View models
  - Bindable UI state
  - UI commands
  - Dialog coordination
  - Navigation coordination
- Core services may expose async methods, CancellationToken parameters, DTOs, progress DTOs, domain models, and business results for presentation layers to consume.
- If a UI workflow needs reusable orchestration, place the UI-neutral business portion in Core and keep the UI-specific coordination in the presentation project.
- Do not move view models or UI command abstractions into Core without explicit approval in the PLAN.
- Long-running work must not block the UI thread.
- Use async commands in the presentation project where existing patterns support them.
- UI-bound collection updates must occur on the UI thread.

### Avalonia cross-platform UI conventions

- Prefer Avalonia property bindings for UI state: Text, SelectedItem, ItemsSource, IsVisible, Command.
- Prefer ICommand over Click += for user actions.
- Use GetObservable(...) for code-built reactions to Avalonia property changes.
- Avoid platform-specific launch/output assumptions; launch GUI with dotnet run --project ./CreationsForge/CreationsForge.csproj or the generated .exe on Windows.
- Avoid large fixed window sizes; prefer MinWidth/MinHeight, responsive layout, and WindowState.Maximized.
- Keep lifecycle hooks like OnAttachedToVisualTree only for view lifecycle/startup behavior, not ordinary input handling.

## MUTAGEN BOUNDARIES

- `CreationsForge` must remain a presentation/UI project and must not contain direct Mutagen calls or package references.
- `CreationsForge` and any future presentation projects must not reference Mutagen packages or Mutagen types directly.
- UI/MVVM code must interact with plugins, records, load order, imports, and comparison data only through `CreationsForge.Core` contracts,
  DTOs, view-model models, and application services.
- `CreationsForge.Core` may reference shared/game-agnostic Mutagen packages such as `Mutagen.Bethesda.Core` only for
  shared primitive mapping and game-agnostic abstractions.
- `CreationsForge.Core` may use shared Mutagen primitives internally, but UI-facing `CreationsForge.Core` contracts should prefer CreationsForge DTOs and primitive identity shapes over Mutagen types.
- `CreationsForge.Core` must not reference game-specific Mutagen packages such as Starfield, Fallout 4, or Skyrim
  Mutagen packages.
- Game-specific projects own all direct use of game-specific Mutagen APIs, records, headers, load-order behavior, and
  mapping from Mutagen types into approved `CreationsForge.Core` DTOs.
- Public contracts crossing from game projects into `CreationsForge.Core` or UI must use `CreationsForge.Core` DTOs, primitives, enums, or interfaces,
  not Mutagen game-specific types.

## DEPENDENCY INJECTION

- Use Autofac as the application composition container.
- Prefer constructor injection.
- Do not manually instantiate services, factories, stores, or repositories where DI is available.
- Keep container resolution in the composition root only.
- Register view models, services, factories, stores, and repositories according to existing patterns.
- Use SingleInstance only for app-wide shared state, stateless infrastructure, or services already treated as singletons.
- Avoid captive dependencies.

## LOGGING & DIAGNOSTICS

- Do not log secrets, credentials, tokens, connection strings, or large record payloads.
- Use existing logging conventions.
- Use structured logging templates, not string interpolation.
- No logs in repositories or stores.
- Log exceptions with the exception object.
- Prefer Information level in services (over Debug).
- Use Warning for recoverable unexpected states.
- Use Error for failures that prevent completion.

## DATABASE & PERSISTENCE

- Use NPoco for application database access.
- Use the ADO.NET SQLite provider as required by NPoco.
- Do not introduce or replace database providers/packages without explicit approval in the PLAN.
- Keep schema creation/migration centralized in a dedicated initializer or migration service.
- Enable SQLite foreign keys for every opened connection.
- Do not place business logic in repositories.
- Do not log from repositories or stores.
- Database path, schema changes, and persistence format changes must be called out in the PLAN.
- Runtime SQL must use named parameterized queries, such as `@Game`, `@ModKeyName`, and `@ImportedAtUTC`.
- Do not use positional NPoco SQL placeholders such as `@0`, `@1`, or `@2` in application runtime SQL.
- Pass SQL parameter values with anonymous objects, typed parameter objects, or equivalent named parameter APIs.

### DbUp migration versioning

- DbUp's `SchemaVersions` table is the only source of truth for database migration state.
- Do not add hardcoded application schema-version constants such as `CurrentSchemaVersion`.
- Do not return or log an app-defined schema version from migration runners or schema initializers.
- To verify schema state, query DbUp `SchemaVersions` for applied migration script names.
- New schema changes must be added as DbUp migrations and validated through `SchemaVersions`, not through numeric version fields.

## CODE QUALITY

- Analyzer warnings are treated as errors.
- Follow existing conventions in the repo. Do not introduce new naming or patterns.
- Ensure the latest stable versions of libraries are used
  - One caveat Mutagen has to remain on latest prerelease version

## TESTING

- Unit tests live in /CreationsForge.UnitTests (xUnit + Moq + Shouldly).
- For new features/bugfixes that affect testable service, factory, validator, DTO, or business logic, include tests in the PLAN and add them alongside code changes.
- Do not unit test database access, repository implementations, or DbUp migration execution.
- When a change is limited to repositories, database access, migrations, or workflows, the PLAN must explicitly state that no unit tests will be added and explain the validation approach.
- Presentation-layer tests live in /CreationsForge.PresentationTests.
- Presentation tests may cover view models, commands, selection/state logic, and pure render-data preparation.
- Do not unit test live Avalonia windows, OpenGL contexts, GPU output, timing, focus, or pixel-perfect rendering.

## DEFERRAL / INCOMPLETE WORK RULES

- Codex must not mark any discovered missing behavior, child record family, UI surface, persistence read path, comparison row,
  test coverage, or documentation update as "deferred", "follow-up", "out of scope", or "future work" unless the PLAN
  explicitly lists it under Out of scope and the user approves that PLAN.
- If existing code persists or imports data, the corresponding repository read path, comparison service output, UI render-data
  path, and applicable tests are in scope unless the PLAN explicitly excludes them.
- When adding or changing a typed record with child collections, the task is not complete until imported child data can be:
  - persisted,
  - read back into DTOs,
  - exposed through comparison/render services,
  - rendered by the UI/view model path when applicable,
  - covered by unit and/or presentation tests.
- If Codex finds documentation saying a behavior is deferred but code/schema/importers already support it, Codex must call
  out the conflict in the PLAN and propose either implementing the missing path or explicitly re-approving the deferral.
- TODO, deferred, follow-up, placeholder, and "not yet implemented" statements may not be added to docs or code comments
  without explicit approval in the PLAN.

## Multi-game typed record support

When adding, removing, or changing a shared typed record import, comparison, persistence, or UI browsing capability,
Codex must update Starfield, Fallout 4, and Skyrim in the same approved task unless the PLAN explicitly identifies the
record type or behavior as game-specific and documents why it cannot apply to all three supported games.

## GIT BOUNDARIES

- Read-only git commands are allowed for inspection and context, including common read-only arguments:
  - `git status`
  - `git diff`
  - `git log`
  - `git show`
  - `git blame`
  - `git ls-files`
- Codex may use read-only git commands to understand current changes, inspect history, avoid overwriting user work,
  and summarize diffs.
- Codex must NEVER run git commands that create, modify, delete, publish, or rewrite repository state unless explicitly
  approved in a task-specific PLAN.
- Always prohibited unless the user explicitly requests them:
  - `git add`
  - `git commit`
  - `git branch`
  - `git checkout`
  - `git switch`
  - `git restore`
  - `git reset`
  - `git merge`
  - `git rebase`
  - `git cherry-pick`
  - `git stash`
  - `git tag`
  - `git push`
  - `git pull`
  - `git fetch`
  - `gh pr create`
  - opening PRs, creating branches, rebasing, squashing, amending commits, or modifying repo history
- If a git command is not clearly read-only, Codex must ask before running it.

## LOCAL INSPECTION COMMANDS

- Codex may run read-only local inspection commands without a PLAN when they do not modify files:
  - `rg`
  - `rg --files`
  - `Get-Content`
  - `Get-ChildItem`
  - `Select-String`
  - `Test-Path`
  - `Resolve-Path`
  - `Get-Command`
  - `Get-Process`
  - `Get-CimInstance Win32_Process`
- Prefer `rg` and `rg --files` for search and file inventory.
- Exclude `bin`, `obj`, `.git`, and generated output directories

## Token Usage Reporting

After each plan and completed implementation, include a short token usage note.

- Report exact token usage only if Codex can directly see it.
- If exact usage is unavailable, provide a rough estimate and mark it as estimated.
- Do not invent exact numbers.
- Mention the biggest usage drivers.
- For Codex CLI, remind me to run `/status` for the live token/context report.
- For plans generating a large or very large token estimate suggest I drop to Codex GPT 5.4

Format:

Token usage: Exact / Estimated / Not available
Total:
Context remaining:
Main drivers:
Notes:

## PLAN → EXECUTE → VALIDATE

- For the plan, use AGENT-PLAN-TEMPLATE.md as the compact approval template. Expand only the sections that are relevant to the task.

### Plan size and conditional detail

Plans must be concise and task-specific. Do not paste the full project rules, validation policy, documentation policy,
or database policy into every plan.

Use the compact AGENT-PLAN-TEMPLATE.md by default.

Only include expanded detail when it applies to the task:

- Include database details only when schema, migrations, persistence behavior, repository behavior, or database docs are affected.
- Include design-decision details only when architecture, dependency direction, public interfaces, workflows, or persistence strategy changes.
- Include documentation details only when documentation will be added, updated, or intentionally left unchanged for a relevant reason.
- Include logging details only when new or changed logging is part of the implementation.
- Include Autofac details only when DI registrations or lifetimes change.
- Include config/environment details only when settings, paths, arguments, or environment variables change.

For unaffected areas, use one-line impact statements such as:

- Database/schema impacts: None
- Documentation impacts: None
- Autofac/DI impacts: None

Do not include empty scaffolding sections, placeholder text, or repeated policy text.

### EXECUTE, after approval only

- Make only the approved edits.
- Keep edits focused.
- Show minimal diffs per file or full files only when replacing/adding.
- Do not introduce new conventions or dependencies unless approved in the PLAN.
- If documentation updates were approved, update the relevant /Documentation files after code changes so the documentation reflects the implemented design.
- Do not create new documentation files unless they were listed in the approved PLAN.

### VALIDATE

- Run:
  - dotnet restore ./CreationsForge.sln
  - dotnet build ./CreationsForge.sln --no-restore
  - dotnet test ./CreationsForge.UnitTests/CreationsForge.UnitTests.csproj --no-build
  - dotnet test ./CreationsForge.PresentationTests/CreationsForge.PresentationTests.csproj --no-build
- Runtime validation, when relevant:
  - dotnet run --project ./CreationsForge.Console/CreationsForge.Console.csproj -- --game Starfield
  - dotnet run --project ./CreationsForge.Console/CreationsForge.Console.csproj -- --game Fallout4
  - dotnet run --project ./CreationsForge.Console/CreationsForge.Console.csproj -- --game Skyrim
- Summarize build/test results, public interface changes, config/persistence notes, and compatibility considerations.
- If validation cannot run due to environment limitations, report the exact command and failure.
