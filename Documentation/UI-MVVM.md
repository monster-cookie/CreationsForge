# UI MVVM

## UI Framework

The presentation project is an Uno Platform Skia Desktop application. UI is defined with WinUI-compatible XAML views
and code-behind only for view lifecycle wiring. The desktop host selects Win32 on Windows and X11 on Linux.

Presentation code belongs in `SFRecordCompareEngine`, including views, view models, commands, dialog services, 
navigation services, and Windows-specific window behavior.

## App Startup

`App` configures:

- Uno Skia Desktop app startup
- Serilog logging
- Autofac container construction
- Core and migration modules
- window, view, and view model registrations
- presentation service registrations

`App.OnLaunched` logs startup and resolves `MainWindow`. `MainWindow` registers itself with
`DesktopApplicationWindowService`, shows `StartupImportView`, and maximizes the window. The startup import service
initializes the database schema so it can force the same import pass when DbUp applies a migration.

## Views

`StartupImportView` displays startup import status, current plugin and record-type text, a progress bar, and an activity
indicator. It starts import from `Loaded` and cancels import from `Unloaded`.

`MainView` is the current application shell after startup import. It has a `MenuBar`, a `CommandBar`,
a filterable left-side record tree, a horizontally scrollable right-side selected-record comparison workspace, and a
status area that shows the total imported plugin header record count and active plugin selection. For the active
plugin, the status includes its plugin type and header record count. The tree groups persisted supported records owned
by the active plugin. The active plugin remains visible in the status area and its comparison column has a subtle
yellow border.

`OpenPluginDialog` is a `ContentDialog` for selecting the active plugin. It provides an autocomplete plugin file
name search backed by imported openable plugin rows, plus Load and Cancel actions.

`SettingsDialog` is a `ContentDialog` for application options. It currently lets users select `Dark` or `Light`
theme and save the choice to application configuration.

## View Models

`ViewModelBase` implements `INotifyPropertyChanged` and `SetProperty`.

`StartupImportViewModel` coordinates startup import UI state. It calls `IPluginImportService.InitializeAndImportAsync`,
receives `PluginImportProgressDTO` updates, updates bindable status/progress properties, navigates to the main view on
success, shows an error dialog on failure, and cancels through a `CancellationTokenSource`.

`MainPageViewModel` exposes a full-reimport command through the File menu and toolbar. The command clears the active
plugin selection, hides the main command surface, and navigates to a fresh startup import view with source-fingerprint
skips disabled for that import pass.

`MainPageViewModel` exposes `OpenCommand`, `ExitCommand`, status text, FormID and EditorID filters, and the left-side
record tree. It listens to `IActivePluginSelectionService`, keeps the status text synchronized with the active plugin,
and rebuilds the tree when the active plugin changes. It keeps Core DTOs based on `FormKey` and uses Mutagen's
Starfield separated-master helpers for presentation-only `FormID` display and filtering. The tree uses lightweight
typed Core service methods that return only `FormKey` and `EditorID`; selected-record comparison data uses the full
typed service detail methods.

Concrete tree-leaf selection also loads normalized field rows and load-order-sorted plugin columns for the right-side
comparison workspace. Form list item rows remain ordered by persisted `Item_Index`, including duplicate references.
Comparable rows are highlighted green when all visible plugin values match and red when any visible value differs.
In conflicting rows, the far-right visible load-order winner is highlighted yellow. Informational identity rows and
single-column comparisons remain neutral. A persistent legend above the status area explains the green, red, and
yellow comparison states.

For records with supported VMAD data, the selected-record comparison workspace keeps scalar fields in the standard
comparison grid and renders VMAD separately in a `Virtual Machine Adapter` section. VMAD scripts are shown as
collapsible script sections with load-order-sorted plugin value cells. The VMAD section can collapse all scripts,
expand changed scripts, and filter script properties to changed rows only. VMAD comparison rows reuse the existing
green, red, and yellow comparison state colors.

For Perk records with imported rank data, the selected-record comparison workspace keeps scalar Perk fields in the
standard comparison grid and renders ranks separately in a `Perk Ranks` section. Perk ranks are shown as collapsible
rank sections with load-order-sorted plugin value cells. The Perk rank section can collapse all ranks, expand changed
ranks, and filter rank rows to changed rows only. Perk rank comparison rows reuse the existing green, red, and yellow
comparison state colors.

MiscItem comparison keeps parent scalar fields in the standard comparison grid. All supported nested structures are
shown in expandable structured comparison groups with one labeled row per nested property or ordered item. The groups
include object bounds, object palette defaults, transforms, model data, sounds, ordered keywords, and destructible
data. The grouped rows reuse the existing green, red, and yellow comparison state colors.

`OpenPluginDialogViewModel` exposes plugin filename suggestions, selected-plugin status, `LoadCommand`, and
`CancelCommand`. It searches openable plugins through `IPluginService` and sets the active plugin through
`IActivePluginSelectionService` when Load is clicked.

`SettingsViewModel` exposes theme options, selected theme state, and saves the selected theme through
`IApplicationConfigurationStore`.

## Commands

`RelayCommand` and `AsyncRelayCommand` live in the presentation project. View models use these commands for UI actions.

Core does not expose UI command abstractions.

## Navigation And Dialogs

`ApplicationNavigationService` owns view transitions:

- replaces the main window content with `MainView`
- opens `OpenPluginDialog` as a WinUI `ContentDialog`
- opens `SettingsDialog` as a WinUI `ContentDialog`
- closes the active dialog
- quits the WinUI app

`UserDialogService` owns user-facing error alerts through `ContentDialog`.

`DesktopApplicationWindowService` stores the active `MainWindow`, swaps content, opens dialogs, closes dialogs, quits
the app, applies the configured theme, and uses Uno desktop `AppWindow` APIs to maximize the main window.

`ActivePluginSelectionService` is presentation-layer shared state for the active plugin selected by the user. It stores
the active `PluginDTO` and raises a change event consumed by main-page UI state.

## UI Thread And Long-Running Work

Startup import is invoked asynchronously from the view model. `PluginImportService.InitializeAndImportAsync` uses
`Task.Run` to keep import work off the UI thread. Progress updates are reported through
`IProgress<PluginImportProgressDTO>` and consumed by the view model for binding updates.

Main record-tree construction runs on a background task after active-plugin selection. The view model applies the
resulting bindable tree collection on the UI thread.

## Current UI Limitations

- Newly supported record types show modeled scalar fields and direct references. Deferred child objects are not yet
  displayed.
