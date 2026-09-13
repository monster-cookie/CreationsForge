using System.Collections.ObjectModel;
using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

/// <summary>Provides the installed-plugin selection workflow for <see cref="NativeWorkspaceSelectionViewModel"/>.</summary>
public sealed partial class NativeWorkspaceSelectionViewModel
{
    /// <summary>Discovers installed plugins through Mutagen without using imported repository state.</summary>
    private readonly INativePluginDiscoveryService? PluginDiscoveryService;

    /// <summary>Cancels the discovery generation currently allowed to publish presentation state.</summary>
    private CancellationTokenSource? PluginDiscoveryCancellationTokenSource;

    /// <summary>Identifies the most recent discovery request so superseded work cannot publish stale results.</summary>
    private long PluginDiscoveryGeneration;

    /// <summary>Tracks whether the shared busy state currently belongs to plugin discovery.</summary>
    private bool IsPluginDiscoveryBusy;

    /// <summary>The last complete detected catalog for the selected game.</summary>
    private NativePluginCatalog? PluginCatalogValue;

    /// <summary>All detached rows before the user search is applied.</summary>
    private IReadOnlyList<NativePluginSelectionRowViewModel> AllPluginRows = [];

    /// <summary>The currently selected installed plugin.</summary>
    private NativePluginSelectionRowViewModel? SelectedPluginValue;

    /// <summary>The current case-insensitive filename filter.</summary>
    private string PluginSearchTextValue = string.Empty;

    /// <summary>Gets the filtered installed plugins shown in load-order order.</summary>
    public ObservableCollection<NativePluginSelectionRowViewModel> PluginRows { get; } = [];

    /// <summary>Gets or sets the selected installed plugin.</summary>
    public NativePluginSelectionRowViewModel? SelectedPlugin
    {
        get => SelectedPluginValue;
        set
        {
            if (SetProperty(ref SelectedPluginValue, value))
            {
                OnPropertyChanged(nameof(CanOpenSelectedPluginReadOnly));
                OnPropertyChanged(nameof(CanOpenSelectedPlugin));
                OnPropertyChanged(nameof(SelectedPluginDetails));
            }
        }
    }

    /// <summary>Gets or sets the case-insensitive plugin filename filter.</summary>
    public string PluginSearchText
    {
        get => PluginSearchTextValue;
        set
        {
            if (SetProperty(ref PluginSearchTextValue, value ?? string.Empty))
            {
                ApplyPluginFilter();
            }
        }
    }

    /// <summary>Gets whether the selected plugin can be opened as an immutable inspection workspace.</summary>
    public bool CanOpenSelectedPluginReadOnly => !IsBusy && SelectedPlugin is not null;

    /// <summary>Gets whether the selected plugin can be opened for guarded editing.</summary>
    public bool CanOpenSelectedPlugin => !IsBusy && SelectedPlugin?.CanEdit == true;

    /// <summary>Gets the selected plugin's dependency and availability summary.</summary>
    public string SelectedPluginDetails => SelectedPlugin?.DetailsText ?? "Select a plugin to see its editing context.";

    /// <summary>Gets the detected data directory or a discovery placeholder.</summary>
    public string DetectedDataDirectoryText => PluginCatalogValue?.DataDirectoryPath ?? "No installed data directory detected yet.";

    /// <summary>Refreshes installed plugin discovery for the selected game.</summary>
    /// <param name="cancellationToken">A token that cancels discovery between filesystem reads.</param>
    /// <returns>A task that completes after bound rows and status are updated.</returns>
    public async Task RefreshPluginsAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy && !IsPluginDiscoveryBusy)
        {
            return;
        }

        if (PluginDiscoveryService is null)
        {
            ErrorText = "Installed plugin discovery is unavailable.";
            StatusText = "Plugin discovery could not start.";
            return;
        }

        PluginDiscoveryCancellationTokenSource?.Cancel();
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        PluginDiscoveryCancellationTokenSource = linkedCancellation;
        var generation = Interlocked.Increment(ref PluginDiscoveryGeneration);
        var selectedGame = SelectedGame;
        await UiDispatcher.InvokeAsync(() =>
        {
            if (!IsCurrentPluginDiscovery(generation, linkedCancellation))
            {
                return;
            }

            IsPluginDiscoveryBusy = true;
            IsBusy = true;
            ErrorText = null;
            StatusText = $"Finding installed {selectedGame.DisplayName} plugins...";
        });
        try
        {
            var result = await PluginDiscoveryService.DiscoverAsync(selectedGame.Game, linkedCancellation.Token);
            await UiDispatcher.InvokeAsync(() =>
            {
                if (!IsCurrentPluginDiscovery(generation, linkedCancellation))
                {
                    return;
                }

                if (!result.Succeeded || result.Value is null)
                {
                    Logger.Warning(
                        "Installed plugin discovery failed for {Game}; error code: {ErrorCode}; message: {ErrorMessage}",
                        selectedGame.Game,
                        result.Error?.Code,
                        result.Error?.Message ?? "No error description was returned.");
                    ResetPluginCatalog();
                    ErrorText = result.Error?.Message ?? "Plugin discovery returned no catalog or failure reason.";
                    StatusText = "Installed plugins could not be loaded.";
                    return;
                }

                PluginCatalogValue = result.Value;
                AllPluginRows = result.Value.Plugins
                    .Select(entry => new NativePluginSelectionRowViewModel(entry))
                    .ToArray();
                ApplyPluginFilter();
                StatusText = $"{AllPluginRows.Count} installed plugin(s) found.";
                OnPropertyChanged(nameof(DetectedDataDirectoryText));
            });
        }
        catch (OperationCanceledException)
        {
            await UiDispatcher.InvokeAsync(() =>
            {
                if (IsCurrentPluginDiscovery(generation, linkedCancellation))
                {
                    StatusText = "Plugin discovery canceled.";
                }
            });
        }
        finally
        {
            await UiDispatcher.InvokeAsync(() =>
            {
                if (!IsCurrentPluginDiscovery(generation, linkedCancellation))
                {
                    return;
                }

                PluginDiscoveryCancellationTokenSource = null;
                IsPluginDiscoveryBusy = false;
                IsBusy = false;
                OnPropertyChanged(nameof(CanOpenSelectedPluginReadOnly));
                OnPropertyChanged(nameof(CanOpenSelectedPlugin));
            });
        }
    }

    /// <summary>Determines whether a discovery generation still owns publication rights.</summary>
    /// <param name="generation">The generation captured by the running request.</param>
    /// <param name="cancellationTokenSource">The cancellation source owned by the running request.</param>
    /// <returns><see langword="true"/> when no newer discovery request has superseded the caller.</returns>
    private bool IsCurrentPluginDiscovery(long generation, CancellationTokenSource cancellationTokenSource)
    {
        return generation == PluginDiscoveryGeneration
            && ReferenceEquals(PluginDiscoveryCancellationTokenSource, cancellationTokenSource);
    }

    /// <summary>Opens the selected plugin and its declared masters as an immutable inspection workspace.</summary>
    /// <param name="cancellationToken">A token that cancels native workspace acquisition.</param>
    /// <returns><see langword="true"/> when the read-only plugin workspace became active; otherwise <see langword="false"/>.</returns>
    public async Task<bool> OpenSelectedPluginReadOnlyAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedPlugin is not { } row || PluginCatalogValue is null)
        {
            ErrorText = "Select a plugin to inspect.";
            return false;
        }

        var loadOrderPluginPaths = row.Entry.DependencyPluginPaths
            .Append(row.Entry.PluginPath)
            .ToArray();
        SourcePluginPath = row.Entry.PluginPath;
        ReplacePaths(LoadOrderPluginPaths, loadOrderPluginPaths);
        DataDirectoryPath = PluginCatalogValue.DataDirectoryPath;
        PopulateStringDirectoryPaths(PluginCatalogValue.DataDirectoryPath);
        OutputPluginPath = string.Empty;
        return await OpenWorkspaceAsync(includeOutput: false, cancellationToken);
    }

    /// <summary>Opens the selected plugin as the guarded mutable output over its declared read-only masters.</summary>
    /// <param name="cancellationToken">A token that cancels native workspace acquisition.</param>
    /// <returns><see langword="true"/> when the plugin became active; otherwise <see langword="false"/>.</returns>
    public async Task<bool> OpenSelectedPluginAsync(CancellationToken cancellationToken = default)
    {
        if (SelectedPlugin is not { CanEdit: true } row || PluginCatalogValue is null)
        {
            ErrorText = SelectedPlugin?.Entry.UnavailableReason ?? "Select an editable plugin.";
            return false;
        }

        PopulateWorkspaceInputs(
            row.Entry.DependencyPluginPaths,
            PluginCatalogValue.DataDirectoryPath,
            row.Entry.PluginPath,
            OutputSelectionMode.OpenExisting,
            row.Entry.LocalizedOutputMode,
            row.Entry.MasterStyle);
        return await OpenWorkspaceAsync(cancellationToken);
    }

    /// <summary>Prompts for a new plugin name and opens it over the detected enabled load order.</summary>
    /// <param name="cancellationToken">A token checked during path selection and native workspace acquisition.</param>
    /// <returns><see langword="true"/> when the new plugin workspace became active; otherwise <see langword="false"/>.</returns>
    public async Task<bool> CreateNewPluginAsync(CancellationToken cancellationToken = default)
    {
        if (PluginCatalogValue is null)
        {
            ErrorText = "Refresh the installed plugin list before creating a plugin.";
            return false;
        }

        var dependencies = PluginCatalogValue.Plugins
            .Where(plugin => plugin.Enabled)
            .Select(plugin => plugin.PluginPath)
            .ToArray();
        if (dependencies.Length == 0)
        {
            ErrorText = "No enabled installed plugins are available as the new plugin's read-only context.";
            return false;
        }

        var outputPath = await RunPickerAsync(() => PathPicker.PickOutputPluginAsync(OutputSelectionMode.CreateNew, cancellationToken));
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            return false;
        }

        var outputStyle = Path.GetExtension(outputPath).Equals(".esl", StringComparison.OrdinalIgnoreCase)
            && SelectedGame.SupportedMasterStyles.Contains(OutputMasterStyle.Small)
                ? OutputMasterStyle.Small
                : OutputMasterStyle.Full;
        PopulateWorkspaceInputs(
            dependencies,
            PluginCatalogValue.DataDirectoryPath,
            outputPath,
            OutputSelectionMode.CreateNew,
            LocalizedOutputMode.Embedded,
            outputStyle);
        return await OpenWorkspaceAsync(cancellationToken);
    }

    /// <summary>Clears discovered rows after the selected game changes or discovery fails.</summary>
    private void ResetPluginCatalog()
    {
        PluginCatalogValue = null;
        AllPluginRows = [];
        PluginRows.Clear();
        SelectedPlugin = null;
        OnPropertyChanged(nameof(DetectedDataDirectoryText));
    }

    /// <summary>Applies the current filename filter while preserving load-order order.</summary>
    private void ApplyPluginFilter()
    {
        var selectedPath = SelectedPlugin?.Entry.PluginPath;
        PluginRows.Clear();
        foreach (var row in AllPluginRows.Where(row =>
                     string.IsNullOrWhiteSpace(PluginSearchText)
                     || row.FileName.Contains(PluginSearchText.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            PluginRows.Add(row);
        }

        SelectedPlugin = selectedPath is null
            ? null
            : PluginRows.FirstOrDefault(row => PathComparer.Equals(row.Entry.PluginPath, selectedPath));
    }

    /// <summary>Translates one product-level plugin choice into the explicit engine request retained by the coordinator.</summary>
    /// <param name="dependencyPluginPaths">The immutable dependency paths in load-order order.</param>
    /// <param name="dataDirectoryPath">The detected installed data directory.</param>
    /// <param name="outputPluginPath">The existing or new plugin selected by the user.</param>
    /// <param name="outputMode">Whether the output already exists.</param>
    /// <param name="localizedOutputMode">The existing or default localization representation.</param>
    /// <param name="masterStyle">The existing or default master style.</param>
    private void PopulateWorkspaceInputs(
        IReadOnlyList<string> dependencyPluginPaths,
        string dataDirectoryPath,
        string outputPluginPath,
        OutputSelectionMode outputMode,
        LocalizedOutputMode localizedOutputMode,
        OutputMasterStyle masterStyle)
    {
        SourcePluginPath = dependencyPluginPaths[^1];
        ReplacePaths(LoadOrderPluginPaths, dependencyPluginPaths);
        DataDirectoryPath = dataDirectoryPath;
        PopulateStringDirectoryPaths(dataDirectoryPath);
        OutputPluginPath = outputPluginPath;
        OutputMode = outputMode;
        LocalizedOutputMode = localizedOutputMode;
        OutputMasterStyle = masterStyle;
    }

    /// <summary>Uses loose strings when present and retains the data directory as explicit archive-only lookup context otherwise.</summary>
    /// <param name="dataDirectoryPath">The detected installed game data directory.</param>
    private void PopulateStringDirectoryPaths(string dataDirectoryPath)
    {
        ReplacePaths(
            StringDirectoryPaths,
            Directory.Exists(Path.Combine(dataDirectoryPath, "Strings"))
                ? [Path.Combine(dataDirectoryPath, "Strings")]
                : [dataDirectoryPath]);
    }
}
