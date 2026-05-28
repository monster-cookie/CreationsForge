# Repo: SFRecordCompareEngine (.NET WPF Desktop Application)

Simple WPF desktop app that shows the record hierarchy of a given plugin in relation to its master plugins.

## Project Layout

- SFRecordCompareEngine (The WPF Presentation/UI Composition)
- SFRecordCompareEngine.Core (The shared models, DTOs, services, repositories, stores, and factories)
- SFRecordCompareEngine.UnitTests (The unit tests)

## HARD RULES

- NEVER run git commands of any kind.
- NEVER modify repo history or open PRs.
- Allowed read/write scope by default:
  - /SFRecordCompareEngine
  - /SFRecordCompareEngine.Core
  - /SFRecordCompareEngine.Migrations
  - /SFRecordCompareEngine.UnitTests
- Do not edit files outside these projects unless explicitly approved in the PLAN.
- ALWAYS show a PLAN first and wait for explicit approval before editing files.
- Keep changes surgical and consistent with existing patterns and naming.
- No breaking changes to existing services, factories, stores, repositories, view models, public interfaces, configuration, persistence formats, or UI workflows without explicit approval.
- NEVER edit AGENTS.md or AGENT-PLAN-TEMPLATE.md, if you have suggestions for changes, please propose them to the user.
- DO NOT wrap lines of code or comments that are not currently wrapped. Follow existing formatting and line breaks in the repo.

## REFERENCE & DOCUMENTATION

Use these as primary documentation references:

- [Mutagen Documentation](https://mutagen-modding.github.io/Mutagen/)
- [Mutagen Code Repository](https://github.com/Mutagen-Modding/Mutagen)
- [Spriggit Code Repository - Uses mutagen to export plugins as YAML](https://github.com/Mutagen-Modding/Spriggit)

## ARCHITECTURE & CONVENTIONS

- Contracts-first for service/core changes: define or update interfaces, DTOs, validators, and applicable tests before implementation. Do not add unit tests for database access, repository implementations, DbUp migration execution, or UI-bound code.
- Core stores must be UI-neutral. They must not expose bindable state, UI commands, UI-thread assumptions, dialog/navigation behavior, or presentation framework types.
- UI-only changes should avoid unnecessary interface, DTO, or validator churn.
- Do not use C# primary constructors for classes. Use traditional explicit constructors instead.
- Use one class per file.
- No statics for application services or mutable app state. Prefer DI; register singletons only when appropriate. Constants, generated framework code, and existing static patterns may remain unless explicitly approved for refactor.
- No repeated code: Refactor existing methods as needed to avoid repeating code in new methods.
- Do not introduce new conventions or dependencies unless explicitly approved in the PLAN.

## TECH CONSTRAINTS

- Dependency injection: Use Autofac.
- Database access: Use NPoco with parameterized SQL.
- Logging and Observability: Use existing Serilog conventions.
- Unit Tests: Use xUnit, Moq, and Shouldly.

## UI / MVVM BOUNDARIES

- UI framework code must stay out of SFRecordCompareEngine.Core.
- Follow existing MVVM patterns in the repo.
- SFRecordCompareEngine.Core must not reference WPF, MAUI, WinUI, Avalonia, CommunityToolkit.Maui, or any UI framework package.
- SFRecordCompareEngine.Core must not contain pages, windows, controls, views, view models, UI commands, dialog services, navigation services, or UI-specific binding helpers.
- SFRecordCompareEngine.Core must not expose or depend on UI binding primitives such as INotifyPropertyChanged, ObservableCollection<T>, ICommand, Dispatcher, SynchronizationContext-based UI dispatching, or platform UI thread helpers.
- Use plain DTOs, domain models, IReadOnlyList<T>, IEnumerable<T>, result objects, events, callbacks, or progress DTOs for Core-to-presentation communication.
- MVVM presentation code belongs in SFRecordCompareEngine only, including:
    - MAUI pages/views
    - C# Markup UI classes
    - View models
    - Bindable UI state
    - UI commands
    - Dialog coordination
    - Navigation coordination
- Core services may expose async methods, DTOs, progress DTOs, domain models, and business results for presentation layers to consume.
- If a UI workflow needs reusable orchestration, place the UI-neutral business portion in Core and keep the UI-specific coordination in the presentation project.
- Do not move view models or UI command abstractions into Core without explicit approval in the PLAN.
- Long-running work must not block the UI thread.
- Use async commands in the presentation project where existing patterns support them.
- UI-bound collection updates must occur on the UI thread.

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
- Use parameterized SQL for all runtime values.
- Keep schema creation/migration centralized in a dedicated initializer or migration service.
- Enable SQLite foreign keys for every opened connection.
- Do not place business logic in repositories.
- Do not log from repositories or stores.
- Repositories should not own UI behavior, import orchestration, or Serilog decisions.
- Database path, schema changes, and persistence format changes must be called out in the PLAN.

### DbUp migration versioning

- DbUp's `SchemaVersions` table is the only source of truth for database migration state.
- Do not add hardcoded application schema-version constants such as `CurrentSchemaVersion`.
- Do not return or log an app-defined schema version from migration runners or schema initializers.
- To verify schema state, query DbUp `SchemaVersions` for applied migration script names.
- New schema changes must be added as DbUp migrations and validated through `SchemaVersions`, not through numeric version fields.

## CODE QUALITY

- Analyzer warnings are treated as errors.
- Follow existing conventions in the repo. Do not introduce new naming or patterns.

## TESTING

- Unit tests live in /SFRecordCompareEngine.UnitTests (xUnit + Moq + Shouldly).
- For new features/bugfixes that affect testable service, factory, validator, DTO, or non-UI business logic, include tests in the PLAN and add them alongside code changes.
- Do not unit test database access, repository implementations, DbUp migration execution, or UI-bound code.
- When a change is limited to repositories, database access, migrations, or UI-bound code, the PLAN must explicitly state that no unit tests will be added and explain the validation approach.

## PLAN → EXECUTE → VALIDATE

- For the plan use AGENT-PLAN-TEMPLATE.md as the template

### PLAN, required before edits

- Scope
- Exact file paths
- Code-level checklist
- UI/XAML impacts
- Data model, persistence, or schema impacts
- If database migration code is touched, the PLAN must state explicitly that DbUp `SchemaVersions` remains the migration source of truth and that no hardcoded schema-version constants are being added.
- Config/environment changes
- Autofac registration changes
- Serilog logging additions/changes
- Risks and rollback notes
- Test plan

### EXECUTE, after approval only

- Make only the approved edits.
- Keep edits focused.
- Show minimal diffs per file or full files only when replacing/adding.
- Do not introduce new conventions or dependencies unless approved in the PLAN.

### VALIDATE

- Run:
  - dotnet restore ./SFRecordCompareEngine.sln
  - dotnet build ./SFRecordCompareEngine.sln --no-restore
  - dotnet test ./SFRecordCompareEngine.UnitTests/SFRecordCompareEngine.UnitTests.csproj --no-build
- Summarize build/test results, public interface changes, config/persistence notes, and compatibility considerations.
- If validation cannot run due to environment limitations, report the exact command and failure.
