# UI MVVM

## UI Framework

The presentation project is a WinUI 3 Windows application. UI is defined with WinUI XAML views and code-behind only for
view lifecycle wiring.

Presentation code belongs in `SFRecordCompareEngine`, including views, view models, commands, dialog services, 
navigation services, and Windows-specific window behavior.

## App Startup

`App` configures:

- WinUI app startup
- Serilog logging
- Autofac container construction
- Core and migration modules
- window, view, and view model registrations
- presentation service registrations

`App.OnLaunched` logs startup and resolves `MainWindow`. `MainWindow` initializes the database schema, registers itself
with `WindowsApplicationWindowService`, shows `StartupImportView`, and maximizes the window.

## Views

`StartupImportView` displays startup import status, current plugin and record-type text, a progress bar, and an activity
indicator. It starts import from `Loaded` and cancels import from `Unloaded`.

`MainView` is the current application shell after startup import. It has a native WinUI `MenuBar`, a WinUI `CommandBar`,
a filterable left-side record tree, a horizontally scrollable right-side selected-record comparison workspace, and a
status area that shows the active plugin selection. The tree groups persisted `FormList` and `GameSetting` records owned
by the active plugin. The active plugin remains visible in the status area and its comparison column has a subtle
yellow border.

`OpenPluginDialog` is a WinUI `ContentDialog` for selecting the active plugin. It provides an autocomplete plugin file
name search backed by imported openable plugin rows, plus Load and Cancel actions.

`SettingsDialog` is a WinUI `ContentDialog` for application options. It currently lets users select `Dark` or `Light`
theme and save the choice to application configuration.

## View Models

`ViewModelBase` implements `INotifyPropertyChanged` and `SetProperty`.

`StartupImportViewModel` coordinates startup import UI state. It calls `IPluginImportService.InitializeAndImportAsync`,
receives `PluginImportProgressDTO` updates, updates bindable status/progress properties, navigates to the main view on
success, shows an error dialog on failure, and cancels through a `CancellationTokenSource`.

`MainPageViewModel` exposes `OpenCommand`, `ExitCommand`, status text, FormID and EditorID filters, and the left-side
record tree. It listens to `IActivePluginSelectionService`, keeps the status text synchronized with the active plugin,
and rebuilds the tree when the active plugin changes. It keeps Core DTOs based on `FormKey` and uses Mutagen's
Starfield separated-master helpers for presentation-only `FormID` display and filtering. It loads records and
comparison data through typed Core services rather than repositories.

Concrete tree-leaf selection also loads normalized field rows and load-order-sorted plugin columns for the right-side
comparison workspace. Form list item rows remain ordered by persisted `Item_Index`, including duplicate references.
Comparable rows are highlighted green when all visible plugin values match and red when any visible value differs.
In conflicting rows, the far-right visible load-order winner is highlighted yellow. Informational identity rows and
single-column comparisons remain neutral. A persistent legend above the status area explains the green, red, and
yellow comparison states.

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

`UserDialogService` owns user-facing error alerts through WinUI `ContentDialog`.

`WindowsApplicationWindowService` stores the active `MainWindow`, swaps content, opens dialogs, closes dialogs, quits 
the app, applies the configured theme, and uses Windows App SDK APIs to maximize the main window.

`ActivePluginSelectionService` is presentation-layer shared state for the active plugin selected by the user. It stores
the active `PluginDTO` and raises a change event consumed by main-page UI state.

## UI Thread And Long-Running Work

Startup import is invoked asynchronously from the view model. `PluginImportService.InitializeAndImportAsync` uses
`Task.Run` to keep import work off the UI thread. Progress updates are reported through
`IProgress<PluginImportProgressDTO>` and consumed by the view model for binding updates.

Main record-tree construction runs on a background task after active-plugin selection. The view model applies the
resulting bindable tree collection on the UI thread.

## Current UI Limitations

- The main record tree currently shows only persisted `FormList` and `GameSetting` details.
