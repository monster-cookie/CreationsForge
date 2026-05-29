# Design Decisions

## 2026-05-29 - Revert Presentation Layer To WinUI

Status: Accepted

Context: The application is Windows-only and needs standard Windows desktop UI surfaces, including a menu bar, toolbar, 
and future grid-based browsing workflows. The MAUI presentation layer failed to reliably render menu and toolbar 
behavior after startup import navigation, and in-page workarounds behaved like normal page content instead of desktop 
chrome.

Decision: Replace the MAUI presentation layer with WinUI 3 and Windows App SDK while keeping 
`SFRecordCompareEngine.Core` UI-neutral.

Rationale: WinUI directly owns the Windows desktop controls and shell patterns the application needs. It avoids 
cross-platform abstraction issues for a Windows-only tool and keeps standard controls such as `MenuBar`, `CommandBar`, 
`ContentDialog`, and future grid controls in the native presentation framework.

Alternatives considered:

- Continue MAUI and add more page-level workarounds.
- Embed WinUI controls around MAUI content.
- Adopt Microsoft.UI.Reactor.
- Move to Uno Platform.
- Move to Avalonia.
- Move to WPF.

Consequences:

- The presentation project uses WinUI XAML, `App`, `MainWindow`, views, and Windows App SDK services.
- MAUI project settings, pages, and resources are removed from the active build.
- Core remains independent from presentation UI framework references.
- Documentation uses WinUI terminology for the implemented presentation framework.

Related files:

- `SFRecordCompareEngine/SFRecordCompareEngine.csproj`
- `SFRecordCompareEngine/App.xaml`
- `SFRecordCompareEngine/App.xaml.cs`
- `SFRecordCompareEngine/MainWindow.xaml`
- `SFRecordCompareEngine/MainWindow.xaml.cs`
- `SFRecordCompareEngine/Views/StartupImportView.xaml`
- `SFRecordCompareEngine/Views/MainView.xaml`
- `SFRecordCompareEngine/Views/OpenPluginDialog.xaml`
- `SFRecordCompareEngine/Services/ApplicationNavigationService.cs`
- `SFRecordCompareEngine/Services/WindowsApplicationWindowService.cs`

## 2026-05-29 - Persist Application Theme In Configuration

Status: Accepted

Context: The WinUI shell needs consistent light and dark theme behavior. Partial per-control brush overrides caused
menu and toolbar visual states to become inconsistent.

Decision: Store the selected theme in `ApplicationConfiguration` and apply it through the WinUI shell root. The default
theme is `Dark`. The setting is edited through the Options dialog opened from `File -> Options` or the toolbar
`Settings` command.

Rationale: Theme is application state and should be handled through one persisted setting rather than scattered visual
workarounds.

Alternatives considered:

- Force light mode only.
- Keep per-control resource overrides.
- Defer theme support until later.

Consequences:

- Application configuration JSON includes a `Theme` field.
- Existing configuration files without a theme continue to load and default to `Dark`.
- The presentation shell applies `ElementTheme.Dark` or `ElementTheme.Light` at runtime.

Related files:

- `SFRecordCompareEngine.Core/Models/Configuration/ApplicationConfiguration.cs`
- `SFRecordCompareEngine.Core/Models/Configuration/ApplicationThemeMode.cs`
- `SFRecordCompareEngine.Core/Configuration/ApplicationConfigurationStore.cs`
- `SFRecordCompareEngine/ViewModels/SettingsViewModel.cs`
- `SFRecordCompareEngine/Views/SettingsDialog.xaml`
- `SFRecordCompareEngine/MainWindow.xaml`

## 2026-05-28 - Document Current .NET MAUI Architecture

Status: Superseded by `2026-05-29 - Revert Presentation Layer To WinUI`

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

Decision: Import plugin metadata, master references, and selected typed record details into a local SQLite database 
under the application data directory.

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
