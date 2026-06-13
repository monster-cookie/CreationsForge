# UI MVVM

## UI Framework

The presentation project is an Avalonia desktop application in `CreationsForge`. UI is built in C# with Avalonia
controls, Avalonia compiled bindings enabled by the project, and theme resources configured at application startup.
The project references Avalonia `DataGrid`, Avalonia `TreeDataGrid`, Fluent styling, Semi.Avalonia styling, and the
Inter font package.

Presentation code belongs in `CreationsForge`, including windows, views, view models, presentation commands, dialog
coordination, navigation coordination, and UI-specific state. Presentation code consumes `CreationsForge.Core`
contracts, DTOs, result objects, and workflow services. It must not call Mutagen directly or depend on game-specific
reader services.

Core does not own Avalonia types, `INotifyPropertyChanged`, `ObservableCollection<T>`, `ICommand`, UI controls,
windowing behavior, dialog coordination, or navigation coordination.

## App Startup

`Program` starts the Avalonia desktop lifetime and delegates application behavior to `App`.

`App` configures:

- Serilog file logging through Bootstrap logging helpers
- Autofac container construction through Bootstrap composition helpers
- Core, migration, and game adapter module registration through Bootstrap
- presentation windows, views, view models, services, and command infrastructure
- Avalonia theme family and theme mode resources
- Avalonia Fluent, Semi.Avalonia, `DataGrid`, and `TreeDataGrid` styles

`App.OnFrameworkInitializationCompleted` logs UI startup, initializes the database schema through
`IDatabaseSchemaInitializer`, resolves `MainWindow`, and assigns it to the Avalonia desktop lifetime. Shutdown disposes
the Autofac container and flushes Serilog.

`MainWindow` registers itself with `IApplicationWindowService`, starts maximized, and asks
`IApplicationNavigationService` to show `MainView`. The first main-view navigation requests the configured active game
import flow when an active game is already saved.

## Views

`MainView` is the primary application workspace. It owns the active-game selector, active-plugin selector, toolbar
commands, record-tree pane, selected-record comparison workspace, and status area. The active-game selector is backed
by Core game selection services. The active-plugin selector is populated from imported openable plugins for the active
game.

The left record browser groups persisted records by record type. Leaf rows display FormID text, EditorID text, and
plugin usage counts where available. FormID and EditorID filters are presentation-level filters over the already loaded
tree items.

The selected-record comparison workspace uses Avalonia `TreeDataGrid` rows and plugin columns returned by Core
comparison services. The active plugin column receives a subtle border. Core assigns comparison value states for
neutral, identical, conflicting, and winning override values; presentation maps those states to transparent, green,
red, and yellow row/cell backgrounds.

`ImportProgressView` displays the running import status, detail text, progress state, and cancel command. It is used
for a selected-game import, a selected-game full reimport, and Reset & Import All.

`ActivePluginLoadView` is shown before returning to the main workspace when the selected plugin is large enough that
building its record browser should not happen inline in the main view.

`SettingsView` lets users choose the active game, theme family, and theme mode. On Windows, it also lets users
configure the fo76utils NifSkope executable path for external NIF inspection, either by typing a path or browsing for
the executable. Saving settings persists the selected configuration through Core services, applies the selected
Avalonia theme immediately, and returns to the main view.

## View Models

`ViewModelBase` implements `INotifyPropertyChanged` and `SetProperty`.

`MainViewModel` coordinates the main workspace. It uses `IGameSelectionService` to list and persist supported games,
`IGameImportReadinessService` to decide whether selection should warn before import, `IPluginSelectionService` to
search openable imported plugins, `IRecordTreeService` to load record browser rows, and `IRecordComparisonService` to
load selected-record comparison data. It exposes bindable selector text, suggestion lists, status text, record tree
items, comparison columns, comparison rows, and Avalonia `HierarchicalTreeDataGridSource` state.

When the user selects a new active game, `MainViewModel` clears the active plugin, refreshes plugin suggestions, and
routes through the guarded import flow when needed. `ReimportSelectedGameCommand` forces a full import for the active
game. `ResetAndImportAllCommand` warns the user and then runs the all-games reset/import workflow.

When the user selects an active plugin, `MainViewModel` either loads the record tree directly or navigates to
`ActivePluginLoadView` for plugins above the large-plugin threshold. Direct record-tree loading uses a request version
guard so stale async work does not overwrite a newer selection. The view model builds grouped
`RecordTreeItemViewModel` rows and applies FormID and EditorID filters in memory.

Concrete record selection calls `IRecordComparisonService.GetRecordComparison`, rebuilds comparison plugin columns,
and rebuilds hierarchical comparison rows. The comparison grid stays DTO-driven and does not query repositories,
database tables, or Mutagen from presentation code.

`ImportProgressViewModel` coordinates import progress UI state. It calls `IGameImportWorkflowService.ImportAsync` for
selected-game imports or `IAllGamesImportWorkflowService.ImportAllAsync` for Reset & Import All. Progress is reported
as Core `GameImportProgressDTO` values and mapped into bindable status, detail, progress value, maximum, and
indeterminate state. Cancellation flows through a `CancellationTokenSource`. Success, cancellation, and failure all
navigate back to the main view, with failures shown through `IUserDialogService`.

`ActivePluginLoadViewModel` builds large active-plugin record trees off the UI thread. It creates a child Autofac
lifetime scope on the worker path, resolves `IRecordTreeService` inside that scope, builds grouped record tree items,
and returns to `MainView` with the selected plugin plus the prebuilt tree. This keeps database-backed services scoped
to the background load instead of reusing the main view's scoped connection.

`SettingsViewModel` exposes supported game options, theme family options, theme mode options, the Windows-only
NifSkope executable path setting, Browse, and Save/Cancel commands. Browse asks `IApplicationWindowService` for an
executable path. Save persists the selected active game, theme, and NifSkope path through Core services, applies the
theme through `IApplicationWindowService`, and returns to `MainView`. Cancel returns to `MainView` without changing
settings.

`RecordTreeItemViewModel`, `RecordComparisonColumnViewModel`, and `RecordComparisonRowViewModel` are presentation
models used by the record browser and comparison workspace. They wrap Core DTO data into Avalonia-friendly state
without moving UI binding primitives into Core.

## Commands

`RelayCommand` and `AsyncRelayCommand` live in the presentation project. View models expose `ICommand` values for UI
actions such as settings navigation, selected-game reimport, Reset & Import All, record-tree pane toggling, saving
settings, canceling settings, and canceling imports.

Core does not expose UI command abstractions.

## Navigation And Dialogs

`ApplicationNavigationService` owns view transitions:

- shows `MainView`
- shows `SettingsView`
- shows `ActivePluginLoadView`
- shows selected-game import progress
- shows Reset & Import All progress
- quits the application

Each displayed view is resolved from a new Autofac child lifetime scope. The previous view scope is disposed before the
new view replaces the main window content. This keeps scoped database-backed services short-lived and lets the Reset &
Import All flow release main-workspace database connections before database files are deleted.

`ApplicationWindowService` stores the active `MainWindow`, swaps the window content, applies configured theme changes,
shows Avalonia modal `Window` dialogs against the main window, and shuts down the Avalonia desktop lifetime.

`UserDialogService` owns user-facing warning and error dialogs. It is used for long import warnings, Reset & Import
All confirmation, and import/load failure alerts.

## UI Thread And Long-Running Work

Import workflows are invoked asynchronously from `ImportProgressViewModel`. Progress updates flow through
`IProgress<GameImportProgressDTO>` and are consumed by bindable view-model properties.

Large active-plugin record browser loading runs on a background task in `ActivePluginLoadViewModel`. It resolves
database-backed services from a worker child lifetime scope, then returns prebuilt view-model rows to the main view.

Main-view active-plugin record loading also uses async flow and request-version checks to avoid stale results updating
the UI after the selected plugin changes. UI-bound collections are updated from the active view model path after the
background work returns.

## Current UI Limitations

- The UI currently browses imported persisted records and scalar comparison rows for the approved record types.
- Deep child comparison sections, patch generation, and conflict resolution workflows are deferred.
- Oblivion is not implemented.
