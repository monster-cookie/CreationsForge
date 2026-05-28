# Design Decisions

## 2026-05-28 - Document Current .NET MAUI Architecture

Status: Accepted

Context: The current presentation project uses .NET MAUI for Windows. The project file enables `UseMaui`, references 
`Microsoft.Maui.Controls`, and the UI is implemented with MAUI `Application`, `Window`, `ContentPage`, and code-built 
MAUI controls. Some repo instruction text still refers to a WPF application.

Decision: Treat .NET MAUI Windows as the current presentation framework in project documentation. Stale WPF wording 
should be replaced with .NET MAUI wording when instruction and template files are explicitly approved for editing.

Rationale: Durable project documentation must describe the implemented system instead of older framework terminology.

Alternatives considered:

- Preserve WPF wording for consistency with existing instructions.
- Describe the app as both WPF and MAUI.

Consequences:

- New documentation uses .NET MAUI terminology.
- Future planning should treat MAUI pages, view models, commands, and services as the presentation boundary.
- Instruction/template files that still say WPF remain known stale references until explicitly edited.

Related files:

- `SFRecordCompareEngine/SFRecordCompareEngine.csproj`
- `SFRecordCompareEngine/App.cs`
- `SFRecordCompareEngine/MauiProgram.cs`
- `SFRecordCompareEngine/Pages/StartupImportPage.cs`
- `SFRecordCompareEngine/Pages/MainPage.cs`
- `SFRecordCompareEngine/Pages/OpenPluginDialogPage.cs`

## 2026-05-28 - Keep Core UI-Neutral

Status: Accepted

Context: The solution separates the MAUI presentation project from `SFRecordCompareEngine.Core`. Core contains import, 
persistence, Mutagen, DTO, and repository logic.

Decision: Keep MAUI pages, view models, commands, navigation, dialogs, and platform window behavior in 
`SFRecordCompareEngine`. Keep Core focused on UI-neutral services, DTOs, models, repositories, importers, and database 
support.

Rationale: This preserves testability and keeps business and persistence behavior independent from the presentation 
framework.

Alternatives considered:

- Move reusable UI orchestration into Core.
- Let Core expose bindable state or command abstractions.

Consequences:

- Presentation-specific services coordinate navigation and dialogs.
- Core services communicate through DTOs, result objects, progress DTOs, and async methods.
- Core must not reference MAUI or other UI framework packages.

Related files:

- `SFRecordCompareEngine.Core/CoreModule.cs`
- `SFRecordCompareEngine/MauiProgram.cs`
- `SFRecordCompareEngine/ViewModels/StartupImportViewModel.cs`
- `SFRecordCompareEngine/Services/ApplicationNavigationService.cs`

## 2026-05-28 - Use DbUp Migrations For SQLite Schema State

Status: Accepted

Context: SQLite schema is created through embedded SQL scripts executed by DbUp. DbUp maintains its migration history 
in `SchemaVersions`.

Decision: Use DbUp `SchemaVersions` as the migration state source of truth. Do not add application-defined 
schema-version constants.

Rationale: A single migration-state mechanism avoids drift between code constants and applied SQL scripts.

Alternatives considered:

- Track a separate numeric schema version in application code.
- Create tables directly in repository or initializer code.

Consequences:

- New schema changes belong in `SFRecordCompareEngine.Migrations/Sql`.
- Schema state should be verified through DbUp migration history.
- `DatabaseSchemaInitializer` delegates migration execution to `IDatabaseMigrationRunner`.

Related files:

- `SFRecordCompareEngine.Core/Database/DatabaseSchemaInitializer.cs`
- `SFRecordCompareEngine.Migrations/DatabaseMigrationRunner.cs`
- `SFRecordCompareEngine.Migrations/Sql/001_CreatePluginSchema.sql`

## 2026-05-28 - Import Plugin Data Into Local SQLite Cache

Status: Accepted

Context: Mutagen reads plugin files from the local Starfield installation. The application needs durable local data for 
browsing and comparison workflows.

Decision: Import plugin metadata, master references, and selected typed record details into a local SQLite database under 
the application data directory.

Rationale: A local cache supports startup discovery, change detection, later browsing, and comparison workflows without 
repeatedly parsing every plugin for every UI interaction.

Alternatives considered:

- Read all plugin data directly from files on demand.
- Store imported data in JSON files.

Consequences:

- Plugin source fingerprints are used to skip unchanged imports.
- Repository methods own data access but not business decisions.
- Database schema changes require migrations.

Related files:

- `SFRecordCompareEngine.Core/Services/PluginImportService.cs`
- `SFRecordCompareEngine.Core/Services/RecordImportService.cs`
- `SFRecordCompareEngine.Core/Repositories/PluginRepository.cs`
- `SFRecordCompareEngine.Core/Repositories/FormListRepository.cs`
- `SFRecordCompareEngine.Core/Models/Database/SqliteDatabaseOptions.cs`
