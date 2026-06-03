# Design Decisions

## 2026-06-03 - Use Lightweight Typed Service Reads For Main Record Tree

Status: Accepted

Context: Main record-tree construction needs only each record's origin `FormKey` and `EditorID`, but some typed
`GetByModKey` service methods hydrate VMAD scripting child data. Hydrating script adapters, properties, and list items
for every record in the active plugin adds unnecessary database work before the tree can render.

Decision: Add `GetRecordTreeEntriesByModKey` methods to the existing typed service and repository contracts. These
methods return lightweight `RecordTreeEntryDTO` values and query only the origin `FormKey` columns and `EditorID`.
`MainPageViewModel.BuildRecordTree` uses these methods for the left-side tree. Existing typed `GetByModKey` and
`GetByFormKey` methods keep their existing full DTO behavior.

Rationale: This keeps presentation code routed through typed Core services, avoids a broad tree-only service, and
preserves VMAD hydration for selected-record detail workflows where child data is actually displayed.

Alternatives considered:

- Add a new tree-specific service and repository.
- Make existing typed `GetByModKey` methods unhydrated.
- Add optional hydration flags to existing typed service methods.
- Query repositories directly from `MainPageViewModel`.

Consequences:

- The main tree no longer hydrates VMAD child tables during active-plugin tree construction.
- Existing typed service and repository interfaces have a new lightweight read method.
- Selected-record comparison keeps using hydrated detail reads.
- Core continues to expose Mutagen `FormKey`; presentation continues converting to displayed `FormID`.

Related files:

- `SFRecordCompareEngine.Core/DTOs/Records/RecordTreeEntryDTO.cs`
- `SFRecordCompareEngine.Core/Services/Interfaces/IFormListService.cs`
- `SFRecordCompareEngine.Core/Repositories/Interfaces/IFormListRepository.cs`
- `SFRecordCompareEngine.Core/Repositories/GlobalRepository.cs`
- `SFRecordCompareEngine/ViewModels/MainPageViewModel.cs`

## 2026-06-03 - Persist Full Origin FormKey For Record Comparison

Status: Accepted

Context: Typed record tables stored the containing plugin `ModKey` and numeric `FormKey_ID`. That incorrectly grouped
unrelated records when different origin plugins used the same local numeric ID. True override comparison needs the
origin `FormKey` from Mutagen and the containing plugin row identity as separate concepts.

Decision: Persist the full origin `FormKey` on typed record and VMAD child tables as `FormKey_ModKey_Name`,
`FormKey_ModKey_Type`, `FormKey_ModKey_FileName`, and `FormKey_ID`. Keep containing-plugin `ModKey_*` columns for row
ownership, display, delete cascades, and load-order sorting. Compare selected records by record type plus the full
origin `FormKey`, not by numeric `FormKey_ID` alone. Squash the SQLite schema back into `001_CreatePluginSchema.sql`
and purge the local cache database so imports rebuild from the corrected schema.

Rationale: Mutagen `FormKey` carries both origin mod identity and local record ID. Persisting only the local ID loses
the distinction between true overrides and unrelated records. Keeping containing and origin identity separate lets a
plugin override a master's record legitimately while preventing false comparison groups.

Alternatives considered:

- Continue querying by `FormKey_ID` alone.
- Derive origin identity from the containing plugin `ModKey`.
- Add an incremental migration and preserve existing cached rows.

Consequences:

- Existing cache databases must be rebuilt because the primary keys and lookup indexes changed.
- DbUp `SchemaVersions` remains the migration-state source of truth.
- No hardcoded application schema-version constants are added.
- Comparison repositories filter by full origin `FormKey` and still sort/display by containing plugin metadata.
- VMAD and form-list child rows include the parent origin `FormKey` columns in their parent keys.

Related files:

- `SFRecordCompareEngine.Migrations/Sql/001_CreatePluginSchema.sql`
- `SFRecordCompareEngine.Core/Models/Database/FormList.cs`
- `SFRecordCompareEngine.Core/Models/Database/ScriptingAdapter.cs`
- `SFRecordCompareEngine.Core/Repositories/FormListRepository.cs`
- `SFRecordCompareEngine/ViewModels/MainPageViewModel.cs`
- `Documentation/Database/DATABASE.md`
- `Documentation/Database/ERD.md`

## 2026-06-03 - Display VMAD Scripts In Collapsible Comparison Sections

Status: Accepted

Context: Supported VMAD data is hierarchical, but the original selected-record comparison UI flattened scripts,
properties, and list items into long field labels. Records with several attached scripts become difficult to read in a
flat scalar row grid.

Decision: Keep scalar record fields in the existing selected-record comparison grid and display supported VMAD data in
a dedicated `Virtual Machine Adapter` section. Render each script as a collapsible presentation-layer section with
property rows and load-order-sorted plugin value cells.

Rationale: Script grouping matches the structure of the data without cloning xEdit's full tree UI. It keeps the scalar
comparison stable, gives VMAD its own expand/collapse workflow, and uses the existing Uno/WinUI control surface instead
of adding a new toolkit dependency.

Alternatives considered:

- Keep VMAD flattened into scalar comparison rows.
- Clone xEdit's full hierarchical tree table.
- Move VMAD details into a separate dialog.
- Add an explicit Uno Toolkit package before confirming the existing Uno/WinUI controls are sufficient.

Consequences:

- VMAD comparison state is presentation-specific and lives in the presentation project.
- Scalar comparison rows no longer include VMAD script/property/list-item labels.
- VMAD script sections can be collapsed, expanded for changed scripts, and filtered to changed properties.
- The same green, red, and yellow comparison state colors remain available for VMAD property values.

Related files:

- `SFRecordCompareEngine/Views/MainView.xaml`
- `SFRecordCompareEngine/ViewModels/MainPageViewModel.cs`
- `SFRecordCompareEngine/ViewModels/RecordComparisonScriptViewModel.cs`
- `SFRecordCompareEngine/ViewModels/RecordComparisonScriptPropertyViewModel.cs`

## 2026-06-02 - Normalize Supported VMAD Script Data In Shared Child Tables

Status: Accepted

Context: Already-supported Starfield record imports need to persist Mutagen `VirtualMachineAdapter` script data.
Multiple typed record tables can expose VMAD, but the current cache schema has no shared child-table design for that
data.

Decision: Persist supported VMAD script data in three shared tables: `ScriptingAdapters`,
`ScriptingAdapterProperties`, and `ScriptingAdapterPropertyListItems`. Key the shared rows by plugin `ModKey`,
typed-parent `RecordType`, parent `FormKey_ID`, and the child identity needed for script, property, and list-item
ordering.

Rationale: This keeps the cache normalized, avoids JSON blobs, preserves script/property/list ordering, and fits the
existing explicit DTO, repository, service, and importer pattern.

Alternatives considered:

- Store VMAD payloads as JSON.
- Create one VMAD table set per supported parent record type.
- Flatten list items into the property table.

Consequences:

- Supported records now hydrate persisted VMAD scripting data through Core services.
- The comparison workspace can display VMAD scripts, properties, and supported list items.
- `RecordType` and full origin `FormKey` identity are required on the shared VMAD tables because numeric
  `FormKey_ID` is not globally unique across typed record tables or origin plugins.
- Struct and variable VMAD property families remain out of scope until a deeper normalized schema is approved.

Related files:

- `SFRecordCompareEngine.Migrations/Sql/001_CreatePluginSchema.sql`
- `SFRecordCompareEngine.Core/Services/StarfieldRecordReaderService.cs`
- `SFRecordCompareEngine.Core/Services/ScriptingAdapterImportService.cs`
- `SFRecordCompareEngine.Core/Services/ScriptingAdapterHydrationService.cs`
- `SFRecordCompareEngine/ViewModels/MainPageViewModel.cs`

## 2026-06-01 - Use Release Tags As Package Version Source

Status: Accepted

Context: Release packages need a stable semantic version shared by ZIP files, installers, assembly metadata, and
Debian package metadata. A tracked `Tools/Release-Version.txt` counter cannot be updated by a tag-triggered build
without creating an additional commit.

Decision: Use the pushed `vmajor.minor.patch` Git tag as the release version source of truth. Strip the leading `v`
inside the GitHub Actions validation job and pass the resulting `major.minor.patch` value explicitly to every
packaging script. Keep the packaged changelog, known-issues document, and roadmap.

Rationale: Release tags are already unique, intentional release triggers. Using the same value for artifacts and
embedded metadata avoids mutable build-time version files and keeps local packaging deterministic.

Alternatives considered:

- Commit and increment `Tools/Release-Version.txt`.
- Use the GitHub Actions run number as the patch version.
- Rewrite the changelog during CI.

Consequences:

- `Tools/Release-Version.txt` is removed.
- Packaging scripts require an explicit semantic version.
- A `v1.2.3` tag produces artifacts and package metadata with version `1.2.3`.
- Repository release documents continue to be bundled with application packages.

Related files:

- `.github/workflows/package-release.yml`
- `Tools/Package-Application.ps1`
- `Tools/Build-Installer.ps1`
- `Tools/Build-DebianPackage.ps1`
- `Tools/Build-Release.ps1`

## 2026-06-01 - Reimport Plugin Cache After Schema Changes

Status: Accepted

Context: Plugin imports use source fingerprints to skip unchanged files. Additive cache schema migrations can require
new metadata to be populated even when plugin files did not change. Users also need an explicit way to rebuild cached
plugin metadata and supported record rows.

Decision: Return whether DbUp applied scripts from `DatabaseMigrationRunner` through `DatabaseSchemaInitializer`.
Consume that one-run result in `PluginImportService` and bypass source-fingerprint skips for the same import pass.
Expose the same behavior through a File-menu and toolbar full-reimport command. Persist Mutagen header record counts and
use the existing persisted header flags for status display.

Rationale: A direct return value keeps the schema-triggered reimport deterministic and UI-neutral. It avoids persistent
configuration state that could force repeated imports or be cleared before the importer consumes it.

Alternatives considered:

- Store a force-reimport flag in application configuration.
- Raise a global migration event.
- Require users to delete the cache manually after migrations.

Consequences:

- DbUp `SchemaVersions` remains the only migration-state source of truth.
- Successfully applied migrations force the current plugin import pass to refresh unchanged files.
- Users can force the same refresh from the main command surface.
- The `Plugins` table stores header record count and uses existing header flags for plugin classification.

Related files:

- `SFRecordCompareEngine.Migrations/DatabaseMigrationRunner.cs`
- `SFRecordCompareEngine.Migrations/Sql/001_CreatePluginSchema.sql`
- `SFRecordCompareEngine.Core/Database/DatabaseSchemaInitializer.cs`
- `SFRecordCompareEngine.Core/Services/PluginImportService.cs`
- `SFRecordCompareEngine/ViewModels/MainPageViewModel.cs`

## 2026-06-01 - Use Per-User Linux Application Data

Status: Accepted

Context: The Debian package installs application binaries under `/opt`, but launches the application as the current
desktop user. The existing default application-data location used `Environment.SpecialFolder.CommonApplicationData`,
which resolves to a system-owned location on Linux and prevents an unprivileged user from creating the SQLite database,
configuration file, and logs during startup.

Decision: Use `~/.SFRecordCompareEngine` as the default application-data directory on Linux. Continue using
`<CommonApplicationData>/SFRecordCompareEngine` on other platforms.

Rationale: Linux desktop application state must be writable by the user launching the installed application. Keeping
the existing non-Linux default avoids changing Windows persistence behavior.

Alternatives considered:

- Provision a world-writable shared directory from the Debian package.
- Require users to launch the application with elevated permissions.
- Use the existing common application-data path on every platform.

Consequences:

- Linux config JSON, SQLite database, and log files default to `~/.SFRecordCompareEngine`.
- Windows config JSON, SQLite database, and log locations remain unchanged.
- Explicitly configured paths remain supported.

Related files:

- `SFRecordCompareEngine.Core/Configuration/ApplicationConfigurationStore.cs`
- `SFRecordCompareEngine.Core/Models/Database/SqliteDatabaseOptions.cs`
- `Documentation/Database/DATABASE.md`

## 2026-06-01 - Replace WinUI-Only Presentation With Uno Skia Desktop

Status: Accepted

Context: The application needs to continue running on Windows while adding a native Linux desktop distribution path
for users running Starfield through Proton. The existing WinUI 3 presentation project is Windows-only.

Decision: Replace the WinUI-only application host with Uno Platform Skia Desktop. Keep the existing WinUI-compatible
XAML views during the platform migration. Configure the host for Win32 on Windows and X11 on Linux. Continue producing
a Windows ZIP and Inno Setup installer, and add Linux ZIP and Debian packages. Generate release packages through
GitHub Actions when a matching version tag is pushed from the current `master` HEAD.

Rationale: Uno preserves the current XAML, MVVM structure, and control model while enabling a shared Windows and Linux
desktop build. Keeping XAML during this phase separates platform migration issues from a later C# Markup refactor.

Alternatives considered:

- Continue distributing the WinUI build for Proton execution.
- Replace WinUI with Avalonia.
- Convert to Uno C# Markup during the platform migration.
- Return to MAUI.

Consequences:

- The presentation project targets Uno Skia Desktop and selects Win32 or X11 at runtime.
- Core, migrations, and unit tests target cross-platform .NET instead of Windows-specific TFMs.
- Linux SQLite packaging, app-data permissions, and Proton-aware Starfield discovery still require Linux validation.
- Matching `vmajor.minor.patch` tag pushes from the current `master` HEAD generate Windows ZIP, Inno Setup installer,
  Linux ZIP, and Debian artifacts.
- A later presentation-only change can migrate XAML views to Uno C# Markup incrementally.
- The `2026-05-29 - Revert Presentation Layer To WinUI` decision is superseded.

Related files:

- `SFRecordCompareEngine/SFRecordCompareEngine.csproj`
- `SFRecordCompareEngine/Platforms/Desktop/Program.cs`
- `SFRecordCompareEngine/Services/DesktopApplicationWindowService.cs`
- `Tools/Package-Application.ps1`
- `Tools/Build-Release.ps1`
- `Tools/Build-Installer.ps1`
- `Tools/Build-DebianPackage.ps1`
- `.github/workflows/package-release.yml`

## 2026-05-30 - Highlight Conflicts Across Visible Comparison Columns

Status: Accepted

Context: The selected-record comparison workspace shows load-order-sorted plugin columns but does not visually
distinguish identical values from conflicts. The current query returns every imported plugin containing the same typed
record, which is broader than a strict recursive-master hierarchy.

Decision: Highlight comparable rows green when all visible plugin values match and red when any visible value differs.
Treat blank values as values so a missing form-list occurrence conflicts with a populated occurrence. Keep
informational identity rows and single-column comparisons neutral. Highlight the far-right visible value yellow in a
conflicting row because it is the winning override within the displayed load-order-sorted set.

Rationale: Green, red, and yellow provide deterministic conflict detection for the implemented visible comparison set.
Yellow distinguishes the effective displayed winner without implying recursive-master hierarchy filtering that the
application does not yet calculate.

Alternatives considered:

- Add yellow highlighting to every lower load-order plugin.
- Treat blank values as neutral.
- Highlight identity rows such as `FormKey`.

Consequences:

- Comparison labels and cells share row-level conflict highlighting.
- The far-right value in a conflicting row uses yellow winning-override highlighting.
- A persistent legend above the status area explains the green, red, and yellow states.
- The active plugin column retains its golden border.
- A later hierarchy feature can narrow the displayed comparison set without changing the color meanings.

Related files:

- `SFRecordCompareEngine/ViewModels/MainPageViewModel.cs`
- `SFRecordCompareEngine/ViewModels/RecordComparisonFieldViewModel.cs`
- `SFRecordCompareEngine/ViewModels/RecordComparisonValueViewModel.cs`
- `SFRecordCompareEngine/Converters/RecordComparisonValueBackgroundBrushConverter.cs`
- `SFRecordCompareEngine/Views/MainView.xaml`

## 2026-05-30 - Keep Game Setting Comparison Focused On Mutagen-Backed Fields

Status: Accepted

Context: The game-setting schema included `TitleString`, but Mutagen's Starfield game-setting records do not expose or
populate that field. The comparison workspace also displayed internal version fields and raw diagnostic fields that
were not useful for routine record comparison.

Decision: Remove `TitleString` from game-setting DTOs, database models, and the initial schema. Hide `FormVersion`,
`Version2`, and `VersionControl` from all comparison views. Hide game-setting `RawData` and `XALG` from comparison
views while retaining their persisted diagnostic values. Display named Starfield major-record flags instead of raw
integers.

Rationale: Comparison views should show meaningful record differences without presenting unsupported or redundant
fields. Persisted diagnostics can remain available without adding noise to the main workflow.

Alternatives considered:

- Keep `TitleString` as an always-empty placeholder.
- Remove all hidden diagnostic fields from persistence.
- Continue displaying numeric record flags.

Consequences:

- Existing cache databases must be recreated because the initial schema changes.
- Comparison grids are smaller and show named major-record flags.
- `RawData` and `XALG` remain available in persistence for future diagnostics.

Related files:

- `SFRecordCompareEngine.Migrations/Sql/001_CreatePluginSchema.sql`
- `SFRecordCompareEngine.Core/DTOs/Records/GameSettingDTO.cs`
- `SFRecordCompareEngine.Core/Models/Database/GameSetting.cs`
- `SFRecordCompareEngine/ViewModels/MainPageViewModel.cs`

## 2026-05-30 - Store Plugin Master References As Relationship Edges

Status: Accepted

Context: `PluginMasterReferences` persisted load-order indexes for both the declared master and the declaring plugin.
Those values duplicated `Plugins.LoadOrderIndex`. The table also used ambiguous child and parent naming and a unique
index that rejected a master plugin referenced by multiple plugins.

Decision: Persist only the declared-master and declaring-plugin `ModKey` tuples plus the import timestamp. Name the
column groups `Master_ModKey_*` and `Plugin_ModKey_*`. Use the composite relationship primary key for uniqueness and
derive master ordering from `Plugins.LoadOrderIndex` when reading.

Rationale: The table models an edge in the plugin dependency graph. Storing load-order indexes on the relationship
duplicates plugin metadata and can reject valid relationships or drift when load order changes.

Alternatives considered:

- Expand the unique index to include both plugin keys.
- Rename and retain duplicated load-order columns.
- Store a header master-list ordinal.

Consequences:

- A master plugin can be referenced by multiple declaring plugins.
- Master-reference reads join `Plugins` to order results by the declared master's current load-order index.
- The initial schema script must recreate local cache databases that already applied its previous shape.

Related files:

- `SFRecordCompareEngine.Migrations/Sql/001_CreatePluginSchema.sql`
- `SFRecordCompareEngine.Core/Models/Database/PluginMasterReference.cs`
- `SFRecordCompareEngine.Core/DTOs/Plugins/PluginMasterReferenceDTO.cs`
- `SFRecordCompareEngine.Core/Repositories/PluginMasterReferencesRepository.cs`
- `SFRecordCompareEngine.Core/Services/PluginImportService.cs`

## 2026-05-30 - Route Presentation Browsing Through Typed Core Services

Status: Accepted

Context: Presentation view models need form list, game setting, and plugin data for the left tree, selected-record
comparison workspace, and open-plugin dialog. Direct repository access from view models couples UI coordination to
persistence boundaries and leaves no service layer for record-specific transformations or business rules.

Decision: Add `FormListService` and `GameSettingService` as UI-neutral typed Core services. Extend `PluginService` with
plugin browsing operations. Presentation view models call these services instead of repository interfaces.

Rationale: Typed services preserve the existing record-type organization while establishing a stable boundary between
MVVM code and persistence. Future record-specific transformations belong in the relevant service.

Alternatives considered:

- Keep direct repository access in view models.
- Add a broad record-browser service.
- Add presentation-layer repository wrappers.

Consequences:

- Presentation view models no longer depend on Core repository interfaces.
- Typed services own repository access for browsing workflows.
- New browsable record types should add corresponding typed services.

Related files:

- `SFRecordCompareEngine.Core/Services/FormListService.cs`
- `SFRecordCompareEngine.Core/Services/GameSettingService.cs`
- `SFRecordCompareEngine.Core/Services/PluginService.cs`
- `SFRecordCompareEngine/ViewModels/MainPageViewModel.cs`
- `SFRecordCompareEngine/ViewModels/OpenPluginDialogViewModel.cs`

## 2026-05-30 - Preserve Form List Item Order And Duplicate Occurrences

Status: Accepted

Context: Starfield form lists are ordered sequences. Duplicate item references are valid, and the selected-record
comparison workspace needs to display each occurrence in source order.

Decision: Persist `FormListItems.Item_Index` from source enumeration order. Include the index in the row identity,
delete existing item rows before rewriting a form list, and query item rows with `ORDER BY Item_Index`.

Rationale: SQLite does not guarantee insertion-order reads without `ORDER BY`. A persisted occurrence index preserves
deterministic display order, keeps valid duplicate references separate, and prevents stale trailing rows after a
shorter list is reimported.

Alternatives considered:

- Depend on SQLite's observed insertion order without an explicit sort.
- Deduplicate identical references.
- Use only referenced form identity as the row key.

Consequences:

- Form list item rows remain ordered and duplicate-preserving.
- Reimport replaces an owning form list's item sequence before saving its current entries.
- Selected-record comparison can align list occurrences by index and leave missing values blank.

Related files:

- `SFRecordCompareEngine.Migrations/Sql/001_CreatePluginSchema.sql`
- `SFRecordCompareEngine.Core/Models/Database/FormListItem.cs`
- `SFRecordCompareEngine.Core/Importers/Starfield/FormListImporter.cs`
- `SFRecordCompareEngine.Core/Repositories/FormListItemRepository.cs`

## 2026-05-30 - Use Existing Typed Record Identity For Cross-Plugin Comparison

Status: Superseded by `2026-06-03 - Persist Full Origin FormKey For Record Comparison`

Supersession note: This decision is historical and no longer describes the implemented comparison identity rule.

Context: The comparison workspace needs to locate every imported plugin containing a selected record. Typed record
tables already store the containing plugin's `ModKey` columns and the record's numeric `FormKey_ID`. Multiple plugin
rows can share the same `FormKey_ID`.

Decision: Query typed record tables by `FormKey_ID` to find cross-plugin comparison rows. Use each result row's
containing-plugin `ModKey` columns and plugin metadata to identify sources and order them by load order. Do not add
origin-plugin columns or persist `FormKey` as a second `ModKey` tuple for this workflow.

Rationale: The existing database shape already returns every containing plugin row needed for comparison. `ModKey`
identifies a plugin file, while `FormKey_ID` identifies the record across those rows. Additional origin-plugin columns
would duplicate concepts and add unnecessary migration and reimport work.

Alternatives considered:

- Add another persisted plugin-key tuple for `FormKey`.
- Reimport typed rows after introducing origin-plugin columns.
- Match records through display-only `FormID` values.

Consequences:

- No schema migration is required for cross-plugin typed-record comparison.
- Comparison repository queries should filter by `FormKey_ID` and order results through plugin load-order metadata.
- Presentation-only `FormID` conversion continues to use Mutagen helpers and active-plugin context.

Related files:

- `SFRecordCompareEngine.Migrations/Sql/001_CreatePluginSchema.sql`
- `SFRecordCompareEngine.Core/Models/Database/FormList.cs`
- `SFRecordCompareEngine.Core/Models/Database/GameSetting.cs`
- `SFRecordCompareEngine.Core/DTOs/Records/FormListDTO.cs`
- `SFRecordCompareEngine.Core/DTOs/Records/GameSettingDTO.cs`

## 2026-05-29 - Revert Presentation Layer To WinUI

Status: Superseded

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

