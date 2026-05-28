# System Overview

## Purpose

SFRecordCompareEngine is a .NET MAUI Windows desktop application for inspecting Starfield plugin data with Mutagen. The 
app discovers the local Starfield load order, imports plugin metadata and selected record details into a local SQLite 
database, and provides a presentation shell for record comparison workflows.

The current implementation focuses on startup import and persistence. The comparison workspace and open-plugin dialog 
exist as MAUI UI placeholders for later workflows.

## Projects

- `SFRecordCompareEngine` is the .NET MAUI Windows presentation project. It owns pages, view models, commands, navigation, dialogs, window behavior, Serilog setup, and Autofac composition.
- `SFRecordCompareEngine.Core` owns UI-neutral domain DTOs, database models, configuration storage, database connection setup, import services, readers, importers, repositories, and Autofac core registrations.
- `SFRecordCompareEngine.Migrations` owns DbUp migration execution and embedded SQL migration scripts.
- `SFRecordCompareEngine.UnitTests` owns xUnit unit tests for DTO/model mapping, configuration, import services, importers, helpers, and selected reader/service behavior.

## Runtime Flow

1. `MauiProgram.CreateMauiApp` configures MAUI, Serilog, and the Autofac container.
2. `App.CreateWindow` initializes the database schema and opens `StartupImportPage`.
3. `StartupImportViewModel.StartImportAsync` starts `IPluginImportService.InitializeAndImportAsync`.
4. `PluginImportService` initializes the database schema, discovers the Starfield load order, opens a database transaction, and processes each load order entry.
5. Plugin metadata is read through Mutagen by `StarfieldPluginReaderService`.
6. Plugin rows and master references are saved through repositories.
7. Record details are imported through `RecordImportService` and typed record importers. The active record detail path currently imports `FormList` records through `FormListImporter`.
8. Progress is reported to the startup page. On successful completion, the app navigates to `MainPage`.

## Current Capabilities

- Discovers Starfield plugins from Mutagen's typical Starfield environment.
- Reads plugin metadata including mod key, header flags, form version, author, interior cell count, and header master references.
- Tracks plugin import state as `Current`, `Changed`, `Missing`, `Failed`, or `Unsupported`.
- Skips unchanged plugins by comparing source last-write ticks and source file size.
- Stores plugin metadata, master references, form lists, and form list items in SQLite.
- Initializes and migrates the database with DbUp.
- Logs app startup, shutdown, schema initialization, and import activity through Serilog.

## Current Limitations

- The main record comparison workspace is a placeholder.
- `OpenPluginDialogPage` is a placeholder dialog.
- `RecordImportService` currently routes only Starfield `FLST` form lists to a typed importer.
- `GameSettingImporter` and the `GameSetting` database table exist, but the importer is not implemented.
- Unsupported `BlueprintShips*.esm` plugins are skipped during import.

## Framework Note

This application is currently implemented with .NET MAUI for Windows. Older repo instructions and templates may still 
refer to a WPF application; those references are stale and should be updated to .NET MAUI terminology when those 
instruction files are next revised.
