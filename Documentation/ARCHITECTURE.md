# Architecture

## Layering

The solution is split into presentation, core, migrations, and tests.

`SFRecordCompareEngine` is the presentation layer. It references Core and Migrations and contains MAUI pages, view models, 
commands, navigation services, dialog services, Windows window behavior, app startup, logging setup, and the Autofac 
composition root.

`SFRecordCompareEngine.Core` is UI-neutral. It contains DTOs, database models, configuration storage, database connection 
factories, schema initialization orchestration, Mutagen readers, import services, typed importers, repositories, and 
Core Autofac registrations.

`SFRecordCompareEngine.Migrations` contains DbUp migration infrastructure and embedded SQL scripts. Core depends on this 
project for `IDatabaseMigrationRunner`.

`SFRecordCompareEngine.UnitTests` tests Core behavior and model/DTO mapping without testing repository database access, 
DbUp execution, or MAUI UI-bound behavior.

## Dependency Direction

- Presentation depends on Core and Migrations.
- Core depends on Migrations for database migration execution.
- Migrations does not depend on Presentation or Core.
- UnitTests depend on Core and Migrations.
- Core does not reference MAUI pages, view models, commands, dialog services, or navigation services.

## Composition

`MauiProgram.BuildContainer` builds the Autofac container.

- `CoreModule` registers Core stores, importers, services, factories, initializers, repositories, `SqliteDatabaseOptions`, and NPoco `IDatabase`.
- `MigrationsModule` registers `DatabaseMigrationRunner` as `IDatabaseMigrationRunner`.
- The presentation project registers MAUI pages, view models, and presentation services.
- `UserDialogService`, `ApplicationNavigationService`, and `WindowsApplicationWindowService` are registered as singletons.

Most Core services, repositories, importers, stores, and initializers are registered by assembly scanning and interface 
suffix conventions.

## Import Architecture

`PluginImportService` is the main import orchestrator. It:

- Initializes schema through `IDatabaseSchemaInitializer`.
- Reads load order entries through `IPluginService`.
- Uses source fingerprints to skip unchanged plugin files.
- Saves plugin metadata through `IPluginRepository`.
- Saves master relationships through `IPluginMasterReferencesRepository`.
- Delegates record details to `IRecordImportService`.
- Reports progress through `IProgress<PluginImportProgressDTO>`.
- Runs work on a background task and honors cancellation tokens.

`RecordImportService` maps typed record importers by `(GameRelease, RecordType)` and currently imports Starfield `FLST` 
records when a matching `ITypedRecordDetailImporter` is registered.

## Persistence Architecture

NPoco is used for application database access. Repository classes translate between DTOs and NPoco database models and 
execute parameterized SQL where runtime values are used.

Schema creation and migration are centralized through:

- `DatabaseSchemaInitializer` in Core
- `DatabaseMigrationRunner` in Migrations
- embedded SQL scripts in `SFRecordCompareEngine.Migrations/Sql`

DbUp's `SchemaVersions` table is the migration state source of truth. The application does not define a hardcoded 
schema-version constant.

## Logging

Serilog is configured in `MauiProgram.CreateMauiApp`. Logs are written under the app data log directory with daily rolling files and a seven-day retention window. Services log workflow-level events and failures. Repositories and stores should not own logging decisions.

## UI Framework Note

The presentation layer is .NET MAUI for Windows. Any references describing the app as WPF are stale and should be 
updated in repo instruction/template files when those files are intentionally revised.
