# System Overview

## Purpose

SFRecordCompareEngine is an Uno Platform Skia Desktop application for inspecting Starfield plugin data with Mutagen. The
app discovers the local Starfield load order, imports plugin metadata and selected record details into a local SQLite 
database, and provides a presentation shell for record comparison workflows.

The current implementation supports startup import, persistence, active-plugin browsing, and a selected-record
comparison workspace. The open-plugin dialog selects the active imported plugin for the main UI.

## Projects

- `SFRecordCompareEngine` is the Uno Platform Skia Desktop presentation project. It owns views, view models, commands,
  navigation, dialogs, window behavior, Serilog setup, and Autofac composition.
- `SFRecordCompareEngine.Core` owns UI-neutral domain DTOs, database models, configuration storage, database connection
  setup, import services, readers, importers, repositories, and Autofac core registrations.
- `SFRecordCompareEngine.Migrations` owns DbUp migration execution and embedded SQL migration scripts.
- `SFRecordCompareEngine.UnitTests` owns xUnit unit tests for DTO/model mapping, configuration, import services,
  importers, helpers, and selected reader/service behavior.

## Runtime Flow

1. `App` configures Serilog and the Autofac container.
2. `MainWindow` initializes the database schema and opens `StartupImportView`.
3. `StartupImportViewModel.StartImportAsync` starts `IPluginImportService.InitializeAndImportAsync`.
4. `PluginImportService` initializes the database schema, discovers the Starfield load order, opens a database
   transaction, and processes each load order entry.
5. Plugin metadata is read through Mutagen by `StarfieldPluginReaderService`.
6. Plugin rows and master references are saved through repositories.
7. Record details are imported through `RecordImportService` and typed record importers. The active record detail path
   reads supported typed DTOs in bulk from Mutagen, then persists them through explicit per-record-type importers.
8. Progress is reported to the startup view for plugin-level phases and supported record-type import phases. On
   successful completion, the app navigates to `MainView`.

## Current Capabilities

- Discovers Starfield plugins from Mutagen's typical Starfield environment.
- Reads plugin metadata including mod key, header flags, form version, author, interior cell count, and header master
  references.
- Tracks plugin import state as `Current`, `Changed`, `Missing`, `Failed`, or `Unsupported`.
- Skips unchanged plugins by comparing source last-write ticks and source file size.
- Stores plugin metadata, master references, and supported typed record details in SQLite.
- Initializes and migrates the database with DbUp.
- Lets users select an active imported plugin from an autocomplete open-plugin dialog.
- Shows imported supported records owned by the active plugin in a filterable main-view record tree.
- Shows every imported plugin containing a selected supported record in a load-order-sorted comparison grid.
- Routes presentation browsing workflows through typed Core services instead of direct repository access.
- Logs app startup, shutdown, schema initialization, plugin import activity, and record-type import checkpoints through
  Serilog.

## Current Limitations

- Nested child structures for newly supported records are intentionally deferred until they can be modeled with
  normalized typed fields or child tables.
- Unsupported `BlueprintShips*.esm` plugins are skipped during import.

## Framework Note

This application is implemented with Uno Platform Skia Desktop and WinUI-compatible XAML. The desktop host selects
Win32 on Windows and X11 on Linux. Older repo instructions and templates may still refer to WPF, MAUI, or WinUI-only
behavior; those references are stale and should be updated to Uno terminology when those instruction files are next
revised.
