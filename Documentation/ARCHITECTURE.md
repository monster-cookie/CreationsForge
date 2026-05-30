# Architecture

## Layering

The solution is split into presentation, core, migrations, and tests.

`SFRecordCompareEngine` is the presentation layer. It references Core and Migrations and contains WinUI views, view 
models, commands, navigation services, dialog services, Windows window behavior, app startup, logging setup, and the 
Autofac composition root.

`SFRecordCompareEngine.Core` is UI-neutral. It contains DTOs, database models, configuration storage, database 
connection factories, schema initialization orchestration, Mutagen readers, import services, typed importers, 
repositories, and Core Autofac registrations.

`SFRecordCompareEngine.Migrations` contains DbUp migration infrastructure and embedded SQL scripts. Core depends on 
this project for `IDatabaseMigrationRunner`.

`SFRecordCompareEngine.UnitTests` tests Core behavior and model/DTO mapping without testing repository database access, 
DbUp execution, or WinUI UI-bound behavior.

## Dependency Direction

- Presentation depends on Core and Migrations.
- Core depends on Migrations for database migration execution.
- Migrations does not depend on Presentation or Core.
- UnitTests depend on Core and Migrations.
- Core does not reference WinUI views, view models, commands, dialog services, or navigation services.

## Composition

`App.BuildContainer` builds the Autofac container.

- `CoreModule` registers Core stores, importers, services, factories, initializers, repositories,
  `SqliteDatabaseOptions`, and NPoco `IDatabase`.
- `MigrationsModule` registers `DatabaseMigrationRunner` as `IDatabaseMigrationRunner`.
- The presentation project registers WinUI views, view models, `MainWindow`, and presentation services.
- `UserDialogService`, `ApplicationNavigationService`, and `WindowsApplicationWindowService` are registered as
  singletons.

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
and `GMST` records when matching `ITypedRecordDetailImporter` instances are registered.

## Persistence Architecture

NPoco is used for application database access. Repository classes translate between DTOs and NPoco database models and 
execute parameterized SQL where runtime values are used.

Schema creation and migration are centralized through:

- `DatabaseSchemaInitializer` in Core
- `DatabaseMigrationRunner` in Migrations
- embedded SQL scripts in `SFRecordCompareEngine.Migrations/Sql`

DbUp's `SchemaVersions` table is the migration state source of truth. The application does not define a hardcoded 
schema-version constant.

## Main Record Tree

The main-view record tree reads persisted `FormList` and `GameSetting` DTOs through their repositories. Repository
queries remain keyed by the owning plugin `ModKey`, and record DTOs continue to expose Mutagen `FormKey` values.

The presentation view model builds record-type and record-leaf nodes for records owned by the active plugin. It uses a
Mutagen separated-master package for Starfield-aware conversion between stored `FormKey` values and
plugin-context-relative `FormID` values. The active plugin's masters provide conversion context but are not displayed
as tree nodes. FormID display and filtering stay in the presentation layer.

## Logging

Serilog is configured in `App`. Logs are written under the app data log directory with daily rolling files and a 
seven-day retention window. Services log workflow-level events and failures. Repositories and stores should not own 
logging decisions.

## UI Framework Note

The presentation layer is WinUI 3 for Windows. Any references describing the app as WPF or MAUI are stale and should be 
updated in repo instruction/template files when those files are intentionally revised.
