# UI MVVM

## UI Framework

The presentation project is a .NET MAUI Windows application. UI is built in C# using MAUI pages and controls rather 
than XAML files.

Presentation code belongs in `SFRecordCompareEngine`, including pages, view models, commands, dialog services, 
navigation services, and Windows-specific window behavior.

## App Startup

`MauiProgram.CreateMauiApp` configures:

- MAUI app startup with `UseMauiApp<App>()`
- Serilog integration
- Autofac container construction
- Core and migration modules
- page and view model registrations
- presentation service registrations

`App.CreateWindow` logs startup, initializes the database schema, and creates the first window with `StartupImportPage`.

## Pages

`StartupImportPage` displays startup import status, current plugin text, a progress bar, and an activity indicator. It 
starts import from `OnAppearing` and cancels import from `OnDisappearing`.

`MainPage` is the current application shell after startup import. It has a File menu, an Open toolbar item, a 
placeholder workspace label, and a status area.

`OpenPluginDialogPage` is a modal placeholder dialog with a close command.

## View Models

`ViewModelBase` implements `INotifyPropertyChanged` and `SetProperty`.

`StartupImportViewModel` coordinates startup import UI state. It calls `IPluginImportService.InitializeAndImportAsync`, 
receives `PluginImportProgressDTO` updates, updates bindable status/progress properties, navigates to the main page on 
success, shows an error dialog on failure, and cancels through a `CancellationTokenSource`.

`MainPageViewModel` exposes `OpenCommand`, `ExitCommand`, and status text.

`OpenPluginDialogViewModel` exposes `CloseCommand`.

## Commands

`RelayCommand` and `AsyncRelayCommand` live in the presentation project. View models use these commands for UI actions.

Core does not expose UI command abstractions.

## Navigation And Dialogs

`ApplicationNavigationService` owns page transitions:

- replaces the first window page with `MainPage`
- opens `OpenPluginDialogPage` as a modal page
- closes the modal page
- quits the MAUI app

`UserDialogService` owns user-facing alerts through MAUI `DisplayAlertAsync`.

`WindowsApplicationWindowService` uses Windows App SDK APIs to maximize the main window after navigation to `MainPage`.

## UI Thread And Long-Running Work

Startup import is invoked asynchronously from the view model. `PluginImportService.InitializeAndImportAsync` uses 
`Task.Run` to keep import work off the UI thread. Progress updates are reported through 
`IProgress<PluginImportProgressDTO>` and consumed by the view model for binding updates.

## Current UI Limitations

- The main record comparison workspace is not implemented yet.
- The open plugin dialog is a placeholder.
- No XAML page files currently define the application UI.

## Stale Framework References

The implemented presentation framework is .NET MAUI. Older repo instruction and template text that describes the app 
as WPF should be updated to .NET MAUI wording when those files are approved for editing.
