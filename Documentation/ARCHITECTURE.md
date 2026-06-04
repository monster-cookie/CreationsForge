# Architecture

## Layering

The solution is split into presentation, core, migrations, and tests.

`SFRecordCompareEngine` is the presentation layer. It references Core and Migrations and contains Uno Platform Skia
Desktop views, view models, commands, navigation services, dialog services, desktop window behavior, app startup,
logging setup, and the
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
- Core does not reference Uno or WinUI views, view models, commands, dialog services, or navigation services.

## Composition

`App.BuildContainer` builds the Autofac container.

- `CoreModule` registers Core stores, importers, services, factories, initializers, repositories,
  `SqliteDatabaseOptions`, and NPoco `IDatabase`.
- `MigrationsModule` registers `DatabaseMigrationRunner` as `IDatabaseMigrationRunner`.
- The presentation project registers Uno desktop views, view models, `MainWindow`, and presentation services.
- `UserDialogService`, `ApplicationNavigationService`, and `DesktopApplicationWindowService` are registered as
  singletons.

Most Core services, repositories, importers, stores, and initializers are registered by assembly scanning and interface 
suffix conventions.

## Import Architecture

`PluginImportService` is the main import orchestrator. It:

- Initializes schema through `IDatabaseSchemaInitializer`.
- Forces a full plugin reimport when schema initialization reports that DbUp applied one or more migrations.
- Reads load order entries through `IPluginService`.
- Uses source fingerprints to skip unchanged plugin files.
- Saves plugin metadata through `IPluginRepository`.
- Saves master relationships through `IPluginMasterReferencesRepository`.
- Delegates record details to `IRecordImportService`.
- Reports progress through `IProgress<PluginImportProgressDTO>`.
- Runs work on a background task and honors cancellation tokens.

`RecordImportService` maps typed record importers by `(GameRelease, RecordType)` and imports Starfield `FLST`, `GMST`,
`GLOB`, `MISC`, `KYWD`, `NPC_`, `AVIF`, `MGEF`, and `PERK` records when matching `ITypedRecordDetailImporter`
instances are registered.

MiscItem full-detail repository reads hydrate normalized optional and ordered child tables. The lightweight MiscItem
record-tree read continues to select only origin `FormKey` and `EditorID`.

## Persistence Architecture

NPoco is used for application database access. Repository classes translate between DTOs and NPoco database models and 
execute parameterized SQL where runtime values are used.

Schema creation and migration are centralized through:

- `DatabaseSchemaInitializer` in Core
- `DatabaseMigrationRunner` in Migrations
- embedded SQL scripts in `SFRecordCompareEngine.Migrations/Sql`

DbUp's `SchemaVersions` table is the migration state source of truth. The application does not define a hardcoded 
schema-version constant.

`DatabaseMigrationRunner` reports whether DbUp applied pending scripts successfully. `PluginImportService` consumes
that one-run result and bypasses source-fingerprint skips for the same import pass. The signal is not persisted as
application configuration.

## Main Record Tree

The main-view record tree reads lightweight persisted supported-record entries through typed Core services. The tree
path uses `GetRecordTreeEntriesByModKey` methods on the existing typed services and repositories so it only loads each
record's origin `FormKey` and `EditorID`.

The presentation view model builds record-type and record-leaf nodes for records owned by the active plugin. It uses a
Mutagen separated-master package for Starfield-aware conversion between stored `FormKey` values and
plugin-context-relative `FormID` values. The active plugin's masters provide conversion context but are not displayed
as tree nodes. FormID display and filtering stay in the presentation layer.

Typed services still expose full DTO reads for selected-record detail and comparison workflows. VMAD scripting child
data is hydrated on those detail paths rather than during tree construction.

## Selected Record Comparison

The main-view comparison workspace uses the selected concrete tree leaf's record type and full origin `FormKey` to
query every imported plugin containing the same typed record. Explicit per-record services provide typed read
boundaries over repository queries. `PluginService` provides imported plugin metadata for load-order sorting. The
presentation view model builds normalized field rows and plugin columns for WinUI binding.

Form list item rows are read by owning plugin and form list key in `Item_Index` order. The persisted index represents
source enumeration order and keeps duplicate item references as distinct occurrences.

Presentation view models do not call Core repositories directly. Typed Core services own repository access and provide
the location for record-specific transformations and business rules.

## Logging

Serilog is configured in `App`. Logs are written under the app data log directory with daily rolling files and a 
seven-day retention window. Services log workflow-level events and failures. Repositories and stores should not own 
logging decisions.

Mutagen plugin construction failures are enriched with the requested `ModKey`. Failures while mapping a Mutagen major
record are enriched with its originating `ModKey`, `FormKey`, EditorID, and Mutagen record type before reaching
workflow-level exception handling and logging.

## UI Framework Note

The presentation layer is Uno Platform Skia Desktop with WinUI-compatible XAML. The desktop host selects Win32 on
Windows and X11 on Linux. Any references describing the app as WPF, MAUI, or WinUI-only are stale and should be updated
in repo instruction/template files when those files are intentionally revised.
