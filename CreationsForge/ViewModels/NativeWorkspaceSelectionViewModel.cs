using System.Collections.ObjectModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using Mutagen.Bethesda.Plugins;
using Serilog;

namespace CreationsForge.ViewModels;

/// <summary>
/// Discovers installed plugins and activates a guarded native editing workspace asynchronously.
/// </summary>
public sealed partial class NativeWorkspaceSelectionViewModel : ViewModelBase
{
    /// <summary>Compares paths using the host file system's case semantics.</summary>
    private static readonly StringComparer PathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    /// <summary>Owns application-wide workspace acquisition and replacement.</summary>
    private readonly INativeWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>Acquires local paths without opening native records.</summary>
    private readonly INativeWorkspacePathPicker PathPicker;

    /// <summary>Reads and persists the configured active game while preserving other settings.</summary>
    private readonly IGameSelectionService GameSelectionService;

    /// <summary>Dispatches engine progress to bound presentation state.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>Records unexpected picker and activation failures.</summary>
    private readonly ILogger Logger;

    /// <summary>The cancellation source for the current acquisition.</summary>
    private CancellationTokenSource? OpenCancellationTokenSource;

    /// <summary>Signals when the current acquisition has finished all publication and cleanup work.</summary>
    private TaskCompletionSource<bool>? OpenCompletionSource;

    /// <summary>The currently selected game and native release.</summary>
    private NativeWorkspaceGameOption SelectedGameValue;

    /// <summary>The read-only source plugin path.</summary>
    private string SourcePluginPathValue = string.Empty;

    /// <summary>The explicit data directory.</summary>
    private string DataDirectoryPathValue = string.Empty;

    /// <summary>The requested output plugin path.</summary>
    private string OutputPluginPathValue = string.Empty;

    /// <summary>Whether the output must be created or already exist.</summary>
    private OutputSelectionMode OutputModeValue = OutputSelectionMode.CreateNew;

    /// <summary>How localized output strings are represented.</summary>
    private LocalizedOutputMode LocalizedOutputModeValue = LocalizedOutputMode.Embedded;

    /// <summary>The selected output master style.</summary>
    private OutputMasterStyle OutputMasterStyleValue = OutputMasterStyle.Full;

    /// <summary>The master styles supported by <see cref="SelectedGame"/>.</summary>
    private IReadOnlyList<OutputMasterStyle> OutputMasterStyleOptionsValue;

    /// <summary>The latest status or progress description.</summary>
    private string StatusTextValue;

    /// <summary>The actionable validation or engine failure.</summary>
    private string? ErrorTextValue;

    /// <summary>Whether acquisition is currently running.</summary>
    private bool IsBusyValue;

    /// <summary>Initializes native workspace selection from the persisted active-game preference.</summary>
    /// <param name="workspaceCoordinator">The application-wide native workspace owner.</param>
    /// <param name="pathPicker">The local path picker.</param>
    /// <param name="gameSelectionService">The persisted active-game service.</param>
    /// <param name="uiDispatcher">The dispatcher used for engine progress.</param>
    /// <param name="logger">The structured logger for unexpected presentation failures.</param>
    /// <param name="pluginDiscoveryService">The optional installed-plugin discovery boundary used by the product selection flow.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public NativeWorkspaceSelectionViewModel(
        INativeWorkspaceCoordinator workspaceCoordinator,
        INativeWorkspacePathPicker pathPicker,
        IGameSelectionService gameSelectionService,
        IUiDispatcher uiDispatcher,
        ILogger logger,
        INativePluginDiscoveryService? pluginDiscoveryService = null)
    {
        ArgumentNullException.ThrowIfNull(workspaceCoordinator);
        ArgumentNullException.ThrowIfNull(pathPicker);
        ArgumentNullException.ThrowIfNull(gameSelectionService);
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        ArgumentNullException.ThrowIfNull(logger);
        WorkspaceCoordinator = workspaceCoordinator;
        PathPicker = pathPicker;
        GameSelectionService = gameSelectionService;
        UiDispatcher = uiDispatcher;
        Logger = logger.ForContext<NativeWorkspaceSelectionViewModel>();
        PluginDiscoveryService = pluginDiscoveryService;
        Games = NativeWorkspaceGameOption.CreateSupportedGames();
        var configuredGame = GameSelectionService.GetActiveGame();
        SelectedGameValue = Games.FirstOrDefault(option => option.Game == configuredGame) ?? Games[0];
        OutputMasterStyleOptionsValue = SelectedGameValue.SupportedMasterStyles;
        StatusTextValue = WorkspaceCoordinator.CurrentWorkspace is null
            ? "Select an installed plugin to inspect or edit, or create a new plugin."
            : CreateReadyStatus(WorkspaceCoordinator.CurrentWorkspace);
        LoadOrderPluginPaths = new ObservableCollection<string>();
        StringDirectoryPaths = new ObservableCollection<string>();
    }

    /// <summary>Gets the three supported game and native-release choices.</summary>
    public IReadOnlyList<NativeWorkspaceGameOption> Games { get; }

    /// <summary>Gets the editable explicit plugin paths in load-order order.</summary>
    public ObservableCollection<string> LoadOrderPluginPaths { get; }

    /// <summary>Gets the explicit localized-string search directories.</summary>
    public ObservableCollection<string> StringDirectoryPaths { get; }

    /// <summary>Gets or sets the selected game and exact native release.</summary>
    public NativeWorkspaceGameOption SelectedGame
    {
        get => SelectedGameValue;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (!SetProperty(ref SelectedGameValue, value))
            {
                return;
            }

            ResetPluginCatalog();
            OutputMasterStyleOptions = value.SupportedMasterStyles;
            if (!OutputMasterStyleOptions.Contains(OutputMasterStyle))
            {
                OutputMasterStyle = OutputMasterStyle.Full;
            }
        }
    }

    /// <summary>Gets or sets the read-only source plugin path, which must also appear in the explicit load order.</summary>
    public string SourcePluginPath
    {
        get => SourcePluginPathValue;
        set => SetProperty(ref SourcePluginPathValue, value?.Trim() ?? string.Empty);
    }

    /// <summary>Gets or sets the explicit game data directory path.</summary>
    public string DataDirectoryPath
    {
        get => DataDirectoryPathValue;
        set => SetProperty(ref DataDirectoryPathValue, value?.Trim() ?? string.Empty);
    }

    /// <summary>Gets or sets the separate native output plugin path.</summary>
    public string OutputPluginPath
    {
        get => OutputPluginPathValue;
        set => SetProperty(ref OutputPluginPathValue, value?.Trim() ?? string.Empty);
    }

    /// <summary>Gets or sets whether the output must be created or already exist.</summary>
    public OutputSelectionMode OutputMode
    {
        get => OutputModeValue;
        set => SetProperty(ref OutputModeValue, value);
    }

    /// <summary>Gets all output selection modes supported by the engine.</summary>
    public IReadOnlyList<OutputSelectionMode> OutputModeOptions { get; } = Enum.GetValues<OutputSelectionMode>();

    /// <summary>Gets or sets how localized output strings are represented.</summary>
    public LocalizedOutputMode LocalizedOutputMode
    {
        get => LocalizedOutputModeValue;
        set => SetProperty(ref LocalizedOutputModeValue, value);
    }

    /// <summary>Gets all localized-output modes supported by the engine.</summary>
    public IReadOnlyList<LocalizedOutputMode> LocalizedOutputModeOptions { get; } = Enum.GetValues<LocalizedOutputMode>();

    /// <summary>Gets or sets the requested native output master style.</summary>
    public OutputMasterStyle OutputMasterStyle
    {
        get => OutputMasterStyleValue;
        set => SetProperty(ref OutputMasterStyleValue, value);
    }

    /// <summary>Gets the output master styles supported by the selected game release.</summary>
    public IReadOnlyList<OutputMasterStyle> OutputMasterStyleOptions
    {
        get => OutputMasterStyleOptionsValue;
        private set => SetProperty(ref OutputMasterStyleOptionsValue, value);
    }

    /// <summary>Gets the latest status or progress description.</summary>
    public string StatusText
    {
        get => StatusTextValue;
        private set => SetProperty(ref StatusTextValue, value);
    }

    /// <summary>Gets the actionable validation or engine failure, or <see langword="null"/> when none is present.</summary>
    public string? ErrorText
    {
        get => ErrorTextValue;
        private set
        {
            if (SetProperty(ref ErrorTextValue, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    /// <summary>Gets whether an actionable validation or engine failure is present.</summary>
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorText);

    /// <summary>Gets whether native source or output acquisition is currently running.</summary>
    public bool IsBusy
    {
        get => IsBusyValue;
        private set
        {
            if (SetProperty(ref IsBusyValue, value))
            {
                OnPropertyChanged(nameof(CanCancel));
                OnPropertyChanged(nameof(CanOpen));
                OnPropertyChanged(nameof(CanOpenSelectedPlugin));
            }
        }
    }

    /// <summary>Gets whether the current acquisition can be canceled.</summary>
    public bool CanCancel => IsBusy;

    /// <summary>Gets whether a new acquisition can be started.</summary>
    public bool CanOpen => !IsBusy;

    /// <summary>Prompts for the read-only source plugin without changing the explicit user-supplied load order.</summary>
    /// <returns>A task that completes after picker state is applied.</returns>
    public async Task BrowseSourcePluginAsync()
    {
        var path = await RunPickerAsync(() => PathPicker.PickSourcePluginAsync());
        if (!string.IsNullOrWhiteSpace(path))
        {
            SourcePluginPath = path;
        }
    }

    /// <summary>Prompts for a new explicit load order and preserves the returned user-supplied order.</summary>
    /// <returns>A task that completes after picker state is applied.</returns>
    public async Task BrowseLoadOrderAsync()
    {
        var paths = await RunPickerAsync(() => PathPicker.PickLoadOrderPluginsAsync());
        if (paths is null || paths.Count == 0)
        {
            return;
        }

        ReplacePaths(LoadOrderPluginPaths, paths);
    }

    /// <summary>Prompts for the explicit game data directory.</summary>
    /// <returns>A task that completes after picker state is applied.</returns>
    public async Task BrowseDataDirectoryAsync()
    {
        var path = await RunPickerAsync(() => PathPicker.PickDataDirectoryAsync());
        if (!string.IsNullOrWhiteSpace(path))
        {
            DataDirectoryPath = path;
        }
    }

    /// <summary>Prompts for the complete explicit localized-string directory list.</summary>
    /// <returns>A task that completes after picker state is applied.</returns>
    public async Task BrowseStringDirectoriesAsync()
    {
        var paths = await RunPickerAsync(() => PathPicker.PickStringDirectoriesAsync());
        if (paths is { Count: > 0 })
        {
            ReplacePaths(StringDirectoryPaths, paths);
        }
    }

    /// <summary>Prompts for an output path using the selected create-new or open-existing mode.</summary>
    /// <returns>A task that completes after picker state is applied.</returns>
    public async Task BrowseOutputPluginAsync()
    {
        var path = await RunPickerAsync(() => PathPicker.PickOutputPluginAsync(OutputMode));
        if (!string.IsNullOrWhiteSpace(path))
        {
            OutputPluginPath = path;
        }
    }

    /// <summary>Moves one load-order entry toward the beginning of the explicit order.</summary>
    /// <param name="index">The current zero-based entry index.</param>
    public void MoveLoadOrderPluginUp(int index)
    {
        if (index <= 0 || index >= LoadOrderPluginPaths.Count)
        {
            return;
        }

        LoadOrderPluginPaths.Move(index, index - 1);
    }

    /// <summary>Moves one load-order entry toward the end of the explicit order.</summary>
    /// <param name="index">The current zero-based entry index.</param>
    public void MoveLoadOrderPluginDown(int index)
    {
        if (index < 0 || index >= LoadOrderPluginPaths.Count - 1)
        {
            return;
        }

        LoadOrderPluginPaths.Move(index, index + 1);
    }

    /// <summary>Removes one load-order entry unless it is the selected source plugin.</summary>
    /// <param name="index">The zero-based entry index.</param>
    public void RemoveLoadOrderPlugin(int index)
    {
        if (index < 0 || index >= LoadOrderPluginPaths.Count || AreSamePath(LoadOrderPluginPaths[index], SourcePluginPath))
        {
            return;
        }

        LoadOrderPluginPaths.RemoveAt(index);
    }

    /// <summary>Removes one localized-string directory.</summary>
    /// <param name="index">The zero-based directory index.</param>
    public void RemoveStringDirectory(int index)
    {
        if (index < 0 || index >= StringDirectoryPaths.Count)
        {
            return;
        }

        StringDirectoryPaths.RemoveAt(index);
    }

    /// <summary>Validates and activates the selected native source and output without disturbing a prior workspace on failure.</summary>
    /// <param name="cancellationToken">A token linked with the dialog's explicit cancel action.</param>
    /// <returns><see langword="true"/> when a new workspace became active; otherwise <see langword="false"/>.</returns>
    public async Task<bool> OpenWorkspaceAsync(CancellationToken cancellationToken = default)
    {
        return await OpenWorkspaceAsync(includeOutput: true, cancellationToken);
    }

    /// <summary>Validates and activates native sources with an optional mutable output without disturbing a prior workspace on failure.</summary>
    /// <param name="includeOutput">Whether the populated output fields must be admitted for editing.</param>
    /// <param name="cancellationToken">A token linked with the dialog's explicit cancel action.</param>
    /// <returns><see langword="true"/> when a new workspace became active; otherwise <see langword="false"/>.</returns>
    private async Task<bool> OpenWorkspaceAsync(bool includeOutput, CancellationToken cancellationToken)
    {
        if (IsBusy)
        {
            return false;
        }

        ErrorText = ValidateSelection(includeOutput);
        if (ErrorText is not null)
        {
            StatusText = "Correct the workspace inputs and try again.";
            return false;
        }

        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var completionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        OpenCancellationTokenSource = linkedCancellation;
        OpenCompletionSource = completionSource;
        IsBusy = true;
        StatusText = "Opening native workspace...";
        var activationSucceeded = false;
        try
        {
            var progress = new UiDispatchingProgress<WorkspaceOpenProgress>(UiDispatcher, update => StatusText = update.Message);
            var sources = new WorkspaceOpenRequest(
                Guid.NewGuid(),
                SelectedGame.Game,
                SelectedGame.Release,
                SourcePluginPath,
                LoadOrderPluginPaths.ToArray(),
                DataDirectoryPath,
                StringDirectoryPaths.ToArray(),
                progress);
            var request = includeOutput
                ? new NativeWorkspaceOpenRequest(
                    sources,
                    OutputMode,
                    new OutputAssociation(
                        OutputPluginPath,
                        ModKey.FromNameAndExtension(Path.GetFileName(OutputPluginPath)),
                        LocalizedOutputMode,
                        OutputMasterStyle))
                : new NativeWorkspaceOpenRequest(sources);
            Logger.Information(
                "Opening native {AccessMode} workspace for {Game}; source: {SourcePluginPath}; output: {OutputPluginPath}; load-order plugin count: {LoadOrderPluginCount}",
                includeOutput ? "editing" : "read-only",
                SelectedGame.Game,
                SourcePluginPath,
                includeOutput ? OutputPluginPath : null,
                LoadOrderPluginPaths.Count);
            var result = await WorkspaceCoordinator.OpenAsync(
                request,
                linkedCancellation.Token);
            if (!result.Succeeded || result.Value is null)
            {
                Logger.Warning(
                    "Native {AccessMode} workspace opening failed for {Game}; source: {SourcePluginPath}; output: {OutputPluginPath}; error code: {ErrorCode}; message: {ErrorMessage}",
                    includeOutput ? "editing" : "read-only",
                    SelectedGame.Game,
                    SourcePluginPath,
                    includeOutput ? OutputPluginPath : null,
                    result.Error?.Code,
                    result.Error?.Message ?? "No error description was returned.");
                ErrorText = result.Error?.Message ?? "The native engine returned no workspace or failure reason.";
                StatusText = WorkspaceCoordinator.CurrentWorkspace is null
                    ? "Native workspace opening failed."
                    : "Native workspace opening failed; the previous workspace remains active.";
                return false;
            }

            activationSucceeded = true;
            Logger.Information(
                "Native {AccessMode} workspace {WorkspaceId} opened for {Game}; source: {SourcePluginPath}; output: {OutputPluginPath}",
                includeOutput ? "editing" : "read-only",
                result.Value.WorkspaceId,
                result.Value.Game,
                result.Value.SourcePluginPath,
                result.Value.Output?.PluginPath);
            ErrorText = null;
            StatusText = CreateReadyStatus(result.Value);
            try
            {
                GameSelectionService.SetActiveGame(SelectedGame.Game);
            }
            catch (Exception exception)
            {
                Logger.Warning(exception, "Native workspace {WorkspaceId} opened, but its active-game preference could not be saved.", result.Value.WorkspaceId);
                ErrorText = "The workspace opened, but the active-game preference could not be saved.";
                StatusText = $"{CreateReadyStatus(result.Value)} The game preference was not saved.";
            }

            return true;
        }
        catch (OperationCanceledException)
        {
            if (activationSucceeded)
            {
                ErrorText = "The workspace opened before cancellation completed.";
                StatusText = GetCurrentWorkspaceStatus("Cancellation arrived after activation committed.");
                return true;
            }

            ErrorText = null;
            StatusText = WorkspaceCoordinator.CurrentWorkspace is null
                ? "Workspace opening canceled."
                : "Workspace opening canceled; the previous workspace remains active.";
            return false;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "An unexpected presentation failure prevented native workspace activation.");
            if (activationSucceeded)
            {
                ErrorText = "The workspace opened, but the presentation could not finish updating its status.";
                StatusText = GetCurrentWorkspaceStatus("Some presentation state could not be updated.");
                return true;
            }

            ErrorText = "An unexpected failure prevented the native workspace from opening.";
            StatusText = WorkspaceCoordinator.CurrentWorkspace is null
                ? "Native workspace opening failed."
                : "Native workspace opening failed; the previous workspace remains active.";
            return false;
        }
        finally
        {
            OpenCancellationTokenSource = null;
            IsBusy = false;
            completionSource.TrySetResult(activationSucceeded);
            if (ReferenceEquals(OpenCompletionSource, completionSource))
            {
                OpenCompletionSource = null;
            }
        }
    }

    /// <summary>Formats truthful status for a workspace that has already crossed the coordinator commit boundary.</summary>
    /// <param name="detail">The nonfatal detail to append.</param>
    /// <returns>The active output status, or a generic committed-workspace status if descriptor projection is unavailable.</returns>
    private string GetCurrentWorkspaceStatus(string detail)
    {
        return WorkspaceCoordinator.CurrentWorkspace is { } workspace
            ? $"{CreateReadyStatus(workspace)} {detail}"
            : $"The workspace opened. {detail}";
    }

    /// <summary>Requests cancellation of the current native workspace acquisition.</summary>
    public void CancelOpen()
    {
        OpenCancellationTokenSource?.Cancel();
    }

    /// <summary>Cancels and drains the current acquisition so it cannot publish after a dialog has closed.</summary>
    /// <returns><see langword="true"/> when activation had already committed before cancellation; otherwise <see langword="false"/>.</returns>
    public async Task<bool> CancelAndWaitForOpenAsync()
    {
        var completion = OpenCompletionSource?.Task;
        CancelOpen();
        if (completion is null)
        {
            return false;
        }

        return await completion;
    }

    /// <summary>Closes the active native workspace and releases its engine lifetime.</summary>
    /// <returns>A task that completes after the workspace has been released.</returns>
    public async Task CloseWorkspaceAsync()
    {
        await CancelAndWaitForOpenAsync();
        await WorkspaceCoordinator.CloseAsync();
        ErrorText = null;
        StatusText = "Native workspace closed.";
    }

    /// <summary>Runs one picker operation and converts unexpected platform failures into bound error state.</summary>
    /// <typeparam name="T">The picker result type.</typeparam>
    /// <param name="picker">The platform picker operation.</param>
    /// <returns>The picker result, or the default value after a reported failure.</returns>
    private async Task<T?> RunPickerAsync<T>(Func<Task<T>> picker)
    {
        if (IsBusy)
        {
            return default;
        }

        try
        {
            ErrorText = null;
            return await picker();
        }
        catch (OperationCanceledException)
        {
            return default;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Unable to acquire a native workspace path from the platform picker.");
            ErrorText = "The selected path could not be read from the platform file picker.";
            return default;
        }
    }

    /// <summary>Validates the complete selection before any native workspace is acquired.</summary>
    /// <returns>An actionable error, or <see langword="null"/> when the selection is structurally complete.</returns>
    private string? ValidateSelection(bool requireOutput = true)
    {
        if (string.IsNullOrWhiteSpace(SourcePluginPath))
        {
            return "Select a read-only source plugin.";
        }

        if (LoadOrderPluginPaths.Count == 0 || !LoadOrderPluginPaths.Any(path => AreSamePath(path, SourcePluginPath)))
        {
            return "The explicit load order must include the selected source plugin.";
        }

        if (HasDuplicatePaths(LoadOrderPluginPaths))
        {
            return "The explicit load order cannot contain the same plugin path more than once.";
        }

        if (HasDuplicatePaths(StringDirectoryPaths))
        {
            return "The localized-string directory list cannot contain the same path more than once.";
        }

        if (string.IsNullOrWhiteSpace(DataDirectoryPath))
        {
            return "Select the game data directory.";
        }

        if (!requireOutput)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(OutputPluginPath))
        {
            return "Select a separate output plugin.";
        }

        if (!ModKey.TryFromNameAndExtension(Path.GetFileName(OutputPluginPath), out _, out var modKeyError))
        {
            return $"The output plugin filename is invalid: {modKeyError}";
        }

        if (LoadOrderPluginPaths.Any(path => AreSamePath(path, OutputPluginPath)))
        {
            return "The output plugin must be separate from every read-only load-order plugin.";
        }

        if (!Enum.IsDefined(OutputMode))
        {
            return "Select a valid output open mode.";
        }

        if (!Enum.IsDefined(LocalizedOutputMode))
        {
            return "Select a valid localized-output mode.";
        }

        if (!OutputMasterStyleOptions.Contains(OutputMasterStyle))
        {
            return $"{SelectedGame.DisplayName} does not support the selected output master style.";
        }

        return null;
    }

    /// <summary>Formats the active plugin and access mode after successful workspace publication.</summary>
    /// <param name="workspace">The published native workspace descriptor.</param>
    /// <returns>A complete ready-state sentence.</returns>
    private static string CreateReadyStatus(NativeWorkspaceDescriptor workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        return workspace.Output is null
            ? $"Read-only workspace ready: {Path.GetFileName(workspace.SourcePluginPath)}."
            : $"Editing workspace ready: {workspace.Output.ModKey.FileName}.";
    }

    /// <summary>Replaces an editable path collection with unique non-empty entries in caller order.</summary>
    /// <param name="target">The bound collection to replace.</param>
    /// <param name="paths">The caller-ordered path selection.</param>
    private static void ReplacePaths(ObservableCollection<string> target, IReadOnlyList<string> paths)
    {
        target.Clear();
        var uniquePaths = new HashSet<string>(PathComparer);
        foreach (var path in paths)
        {
            var normalized = path?.Trim();
            if (!string.IsNullOrWhiteSpace(normalized) &&
                !target.Any(existing => AreSamePath(existing, normalized)) &&
                uniquePaths.Add(normalized))
            {
                target.Add(normalized);
            }
        }
    }

    /// <summary>Determines whether a path collection contains canonical duplicates.</summary>
    /// <param name="paths">The paths to inspect.</param>
    /// <returns><see langword="true"/> when the same normalized path appears more than once.</returns>
    private static bool HasDuplicatePaths(IEnumerable<string> paths)
    {
        var observed = new List<string>();
        foreach (var path in paths)
        {
            if (observed.Any(existing => AreSamePath(existing, path)))
            {
                return true;
            }

            observed.Add(path);
        }

        return false;
    }

    /// <summary>Compares two paths after safe absolute normalization when possible.</summary>
    /// <param name="left">The first path.</param>
    /// <param name="right">The second path.</param>
    /// <returns><see langword="true"/> when both paths identify the same normalized location.</returns>
    private static bool AreSamePath(string left, string right)
    {
        try
        {
            return PathComparer.Equals(Path.GetFullPath(left), Path.GetFullPath(right));
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return PathComparer.Equals(left, right);
        }
    }
}
