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

- Contracts-first for service/core changes: define or update interfaces, DTOs, validators, and tests before implementation when applicable.
- UI-only changes should avoid unnecessary interface, DTO, or validator churn.
- Class-per-file. Primary constructors for services, factories, stores, and repositories where possible.
- No statics for application services or mutable app state. Prefer DI; register singletons only when appropriate. Constants, generated framework code, and existing static patterns may remain unless explicitly approved for refactor.
- No repeated code: Refactor existing methods as needed to avoid repeating code in new methods.

## TECH CONSTRAINTS

- Dependency injection: Use Autofac.
- Database access: Use NPoco with parameterized SQL.
- Logging and Observability: Use existing Serilog conventions.
- Unit Tests: Use xUnit, Moq, and Shouldly.

## WPF & UI CONVENTIONS

- Follow existing MVVM patterns in the repo.
- Keep code-behind minimal. Do not place business logic in views or code-behind.
- View models should expose bindable state, commands, and UI coordination only.
- Business logic belongs in services, factories, stores, or repositories as appropriate.
- Long-running work must not block the UI thread.
- Use async commands where existing patterns support them.
- UI-bound collection updates must occur on the UI thread.
- Do not call MessageBox, file pickers, dialogs, or window APIs from SFRecordCompareEngine.Core.
- Preserve existing XAML resource, style, and binding conventions.
- Avoid broad XAML rewrites unless explicitly approved in the PLAN.

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
- For new features/bugfixes, include tests in the PLAN and add them alongside code changes.

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
