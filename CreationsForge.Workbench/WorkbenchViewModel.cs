using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using CreationsForge.Engine.Installs;
using Serilog;

namespace CreationsForge.Workbench;

/// <summary>Owns explicit Workbench selections, visible status, and the MCP child lifetime.</summary>
public sealed class WorkbenchViewModel : INotifyPropertyChanged
{
    private static readonly System.Text.Json.JsonSerializerOptions IndentedJson = new() { WriteIndented = true };
    private static readonly HashSet<string> TrackedSettings = new(StringComparer.Ordinal)
    {
        nameof(McpExecutablePath),
        nameof(DataDirectory),
        nameof(SelectedPlugins),
        nameof(OutputPath),
        nameof(MasterStyle),
        nameof(TextStorageMode),
        nameof(Language),
    };
    private readonly Func<string, CancellationToken, Task<IWorkbenchMcpSession>> _startSession;
    private readonly IWorkbenchSettingsStore? _settings;
    private readonly Func<string?>? _findMcpExecutable;
    private readonly Func<string, string?>? _findDataDirectory;
    private readonly Dictionary<string, WorkbenchReleaseSettings> _profiles = new(StringComparer.Ordinal);
    private readonly SynchronizationContext? _uiContext;
    private readonly EventHandler _hostExitedHandler;
    private readonly TimeSpan _connectTimeout;
    private readonly TimeSpan _operationTimeout;
    private readonly object _state = new();
    private bool _applyingSettings;
    private IWorkbenchMcpSession? _client;
    private CancellationTokenSource? _activeCancellation;
    private CancellationTokenSource? _userCancellation;
    private CancellationTokenSource? _timeoutCancellation;
    private CancellationTokenSource? _linkedCancellation;
    private TaskCompletionSource? _operationCompleted;
    private Task? _closeTask;
    private string _mcpExecutablePath = string.Empty;
    private string _dataDirectory = string.Empty;
    private string _selectedPlugins = string.Empty;
    private string _outputPath = string.Empty;
    private string _release = "Starfield";
    private string _masterStyle = "Full";
    private string _textStorageMode = "Embedded";
    private string _language = "English";
    private string _status = "Disconnected";
    private string _diagnostics = "Select an MCP executable and connect to begin.";
    private string _activityText = string.Empty;
    private string _protocolVersion = string.Empty;
    private string _serverName = string.Empty;
    private string _selectedToolDescription = string.Empty;
    private string _selectedToolSchema = string.Empty;
    private string? _workspaceId;
    private WorkbenchMcpTool? _selectedTool;
    private int _activityGeneration;
    private bool _outputAttached;
    private bool _connecting;
    private bool _busy;
    private bool _closing;
    private bool _hostExitObserved;

    /// <summary>Creates a view model that launches the production MCP executable and remembers selections.</summary>
    public WorkbenchViewModel()
        : this(
            static async (path, token) => await StdioMcpClient.StartAsync(path, token).ConfigureAwait(false),
            new WorkbenchSettingsStore(),
            WorkbenchExecutableLocator.Find,
            InstalledGameData.TryGetDirectory)
    {
    }

    /// <summary>Creates a view model that starts sessions through a caller-supplied factory.</summary>
    /// <param name="startSession">Starts one MCP session for an executable path. Canceled startup must not leave a running child.</param>
    /// <remarks>This constructor does not read or write <c>CreationsForge.Config.json</c>.</remarks>
    internal WorkbenchViewModel(Func<string, CancellationToken, Task<IWorkbenchMcpSession>> startSession)
        : this(startSession, settings: null, findMcpExecutable: null, findDataDirectory: null)
    {
    }

    /// <summary>Creates a view model with explicit session startup and settings dependencies.</summary>
    /// <param name="startSession">Starts one MCP session for an executable path. Canceled startup must not leave a running child.</param>
    /// <param name="settings">The settings cache. <see langword="null"/> keeps every selection in memory for the process only.</param>
    /// <param name="findMcpExecutable">Supplies an MCP host when the saved path is empty or missing.</param>
    /// <param name="findDataDirectory">Supplies an installed Data directory for a release whose saved directory is empty or missing.</param>
    /// <param name="connectTimeout">The limit for initialize and tool discovery. A non-positive value uses 15 seconds.</param>
    /// <param name="operationTimeout">The limit for one workspace or output call. A non-positive value uses 30 seconds.</param>
    internal WorkbenchViewModel(
        Func<string, CancellationToken, Task<IWorkbenchMcpSession>> startSession,
        IWorkbenchSettingsStore? settings,
        Func<string?>? findMcpExecutable,
        Func<string, string?>? findDataDirectory,
        TimeSpan? connectTimeout = null,
        TimeSpan? operationTimeout = null)
    {
        ArgumentNullException.ThrowIfNull(startSession);
        _startSession = startSession;
        _settings = settings;
        _findMcpExecutable = findMcpExecutable;
        _findDataDirectory = findDataDirectory;
        _hostExitedHandler = OnHostExited;
        _connectTimeout = Positive(connectTimeout, TimeSpan.FromSeconds(15));
        _operationTimeout = Positive(operationTimeout, TimeSpan.FromSeconds(30));
        _uiContext = SynchronizationContext.Current;
        if (settings is not null)
        {
            ApplyLoadedSettings(settings.Load());
        }
    }

    /// <summary>Raised when a bound property changes.</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Gets the release names accepted by the production MCP host.</summary>
    public IReadOnlyList<string> Releases { get; } = ["Starfield", "Fallout4", "SkyrimSE"];

    /// <summary>Gets the master styles accepted by output tools.</summary>
    public IReadOnlyList<string> MasterStyles { get; } = ["Full", "Medium", "Small"];

    /// <summary>Gets the text storage modes accepted by output tools.</summary>
    public IReadOnlyList<string> TextStorageModes { get; } = ["Embedded", "Localized"];

    /// <summary>Gets the translated-string languages accepted by output tools, in MCP enum order with English first.</summary>
    public IReadOnlyList<string> Languages { get; } =
    [
        "English",
        "German",
        "Italian",
        "Spanish",
        "Spanish_Mexico",
        "French",
        "Polish",
        "Portuguese_Brazil",
        "Chinese",
        "Russian",
        "Japanese",
        "Czech",
        "Hungarian",
        "Danish",
        "Finnish",
        "Greek",
        "Norwegian",
        "Swedish",
        "Turkish",
        "Arabic",
        "Korean",
        "Thai",
        "ChineseSimplified",
    ];

    /// <summary>Gets or sets the production MCP executable or <c>.dll</c> path.</summary>
    public string McpExecutablePath { get => _mcpExecutablePath; set => Set(ref _mcpExecutablePath, value); }

    /// <summary>Gets or sets the game Data directory that contains the selected source plugins.</summary>
    public string DataDirectory { get => _dataDirectory; set => Set(ref _dataDirectory, value); }

    /// <summary>Gets or sets comma-separated source plugin names, from low to high priority.</summary>
    public string SelectedPlugins { get => _selectedPlugins; set => Set(ref _selectedPlugins, value); }

    /// <summary>Gets or sets the explicit output plugin path.</summary>
    public string OutputPath { get => _outputPath; set => Set(ref _outputPath, value); }

    /// <summary>Gets or sets the selected game release. Each release keeps its own directory, plugins, and output.</summary>
    public string Release
    {
        get => _release;
        set => ChangeRelease(value);
    }

    /// <summary>Gets or sets the requested output master style.</summary>
    public string MasterStyle
    {
        get => _masterStyle;
        set => Set(ref _masterStyle, NormalizeChoice(value, MasterStyles, "Full"));
    }

    /// <summary>Gets or sets the requested output text storage mode.</summary>
    public string TextStorageMode
    {
        get => _textStorageMode;
        set => Set(ref _textStorageMode, NormalizeChoice(value, TextStorageModes, "Embedded"));
    }

    /// <summary>Gets or sets the active translated-string language sent to output tools.</summary>
    public string Language
    {
        get => _language;
        set => Set(ref _language, NormalizeChoice(value, Languages, "English"));
    }

    /// <summary>Gets the short connection state shown beside the controls.</summary>
    public string Status { get => _status; private set => Set(ref _status, value); }

    /// <summary>Gets copyable diagnostics for the latest connection, workspace, protocol, or tool result.</summary>
    public string Diagnostics { get => _diagnostics; private set => Set(ref _diagnostics, value); }

    /// <summary>Gets the active phase and elapsed time, or an empty string when no call is waiting.</summary>
    public string ActivityText => _activityText;

    /// <summary>Gets whether <see cref="ActivityText"/> should be shown.</summary>
    public bool HasActivity => _activityText.Length > 0;

    /// <summary>Gets the tools and schemas discovered from the connected host.</summary>
    public ObservableCollection<WorkbenchMcpTool> DiscoveredTools { get; } = [];

    /// <summary>Gets a short description of the negotiated protocol and discovered tool count.</summary>
    public string DiscoverySummary
    {
        get
        {
            if (DiscoveredTools.Count == 0)
            {
                return "Connect to discover the production tools and their schemas.";
            }

            var noun = DiscoveredTools.Count == 1 ? "tool" : "tools";
            return $"{DiscoveredTools.Count} {noun} from {_serverName} MCP {_protocolVersion}.";
        }
    }

    /// <summary>Gets or sets the discovered tool whose schema is shown.</summary>
    public WorkbenchMcpTool? SelectedTool
    {
        get => _selectedTool;
        set
        {
            if (ReferenceEquals(_selectedTool, value))
            {
                return;
            }

            _selectedTool = value;
            OnPropertyChanged();
            SelectedToolDescription = value?.Description ?? string.Empty;
            SelectedToolSchema = value?.InputSchemaJson ?? string.Empty;
        }
    }

    /// <summary>Gets the description of <see cref="SelectedTool"/>, or an empty string when none is selected.</summary>
    public string SelectedToolDescription
    {
        get => _selectedToolDescription;
        private set => Set(ref _selectedToolDescription, value);
    }

    /// <summary>Gets the indented input schema of <see cref="SelectedTool"/>, or an empty string when none is selected.</summary>
    public string SelectedToolSchema
    {
        get => _selectedToolSchema;
        private set => Set(ref _selectedToolSchema, value);
    }

    /// <summary>Gets the active log file path, or the reason file logging could not start.</summary>
    public string LogFilePath
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(WorkbenchLog.CurrentLogPath))
            {
                return "Log file: " + WorkbenchLog.CurrentLogPath;
            }

            return string.IsNullOrWhiteSpace(WorkbenchLog.StartupError)
                ? string.Empty
                : "Logging failed to start: " + WorkbenchLog.StartupError;
        }
    }

    /// <summary>Gets whether <see cref="LogFilePath"/> should be shown above the diagnostics box.</summary>
    public bool HasLogFile => LogFilePath.Length > 0;

    /// <summary>Gets whether Connect can start a new child process.</summary>
    public bool CanConnect => _client is null && !_connecting && !_closing && !_hostExitObserved;

    /// <summary>Gets whether Disconnect can cancel startup or stop the current child.</summary>
    public bool CanDisconnect => (_client is not null || _connecting) && !_closing;

    /// <summary>Gets whether Cancel can stop the active call without closing a connected host.</summary>
    public bool CanCancel => (_connecting || _busy) && !_closing;

    /// <summary>Gets whether the source release, data directory, and plugin list still apply to the next workspace open.</summary>
    public bool CanEditSources => !_connecting && !_busy && _workspaceId is null && !_closing;

    /// <summary>Gets whether output path, master style, text storage, and language can change.</summary>
    public bool CanEditOutput => !_connecting && !_busy && !_outputAttached && !_closing;

    /// <summary>Gets whether Open workspace can send <c>workspace_open</c>.</summary>
    public bool CanOpenWorkspace => _client is not null && _workspaceId is null && !_busy && !_closing;

    /// <summary>Gets whether Create output can send <c>output_create</c> and then <c>workspace_save</c>.</summary>
    public bool CanCreateOutput => _client is not null && _workspaceId is not null && !_outputAttached && !_busy && !_closing;

    /// <summary>Gets whether Open output can send <c>output_open</c>.</summary>
    public bool CanOpenOutput => CanCreateOutput;

    /// <summary>Starts the selected MCP executable, negotiates the protocol, and lists tools. A missing file or failed startup stays on the disconnected screen.</summary>
    /// <returns>A task that completes when the session is connected, canceled, timed out, or visibly failed.</returns>
    public Task ConnectAsync()
    {
        CancellationTokenSource linked;
        int generation;
        lock (_state)
        {
            if (_closing || _connecting || _client is not null || _hostExitObserved)
            {
                return Task.CompletedTask;
            }

            _connecting = true;
            generation = ++_activityGeneration;
            linked = BeginOperation(_connectTimeout);
            _operationCompleted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        var connect = ConnectCoreAsync(linked, generation);
        Publish(NotifyCommands);
        return connect;
    }

    /// <summary>Cancels the active startup or tool call. A connected host keeps running; startup cancellation stops the child.</summary>
    /// <returns>A completed task after the cancellation request is delivered.</returns>
    public Task CancelAsync()
    {
        CancellationTokenSource? source;
        lock (_state)
        {
            source = _userCancellation;
        }

        Cancel(source);
        return Task.CompletedTask;
    }

    /// <summary>Sends <c>workspace_open</c> for the explicit release, data directory, and source plugins.</summary>
    /// <returns>A task that completes when the session id is stored or the failure is visible.</returns>
    public Task OpenWorkspaceAsync() => RunSessionOperationAsync(OpenWorkspaceCoreAsync, "Opening workspace");

    /// <summary>Sends <c>output_create</c> or <c>output_open</c> for the explicit output settings.</summary>
    /// <param name="createNew"><see langword="true"/> to create and save a new plugin; <see langword="false"/> to open an existing one.</param>
    /// <returns>A task that completes when the result or failure is visible. A tool error does not claim the output file was written.</returns>
    public Task OpenOutputAsync(bool createNew) =>
        RunSessionOperationAsync(token => OpenOutputCoreAsync(createNew, token), createNew ? "Creating output" : "Opening output");

    /// <summary>Cancels startup or the active call, closes the workspace when one is open, and stops the child process.</summary>
    /// <returns>A task that completes after the child has been disposed and the screen is disconnected.</returns>
    public Task CloseAsync()
    {
        Task close;
        lock (_state)
        {
            if (_closeTask is { IsCompleted: false })
            {
                return _closeTask;
            }

            _closing = true;
            Cancel(_activeCancellation);
            close = _closeTask = CloseCoreAsync();
        }

        Publish(NotifyCommands);
        return close;
    }

    private async Task ConnectCoreAsync(CancellationTokenSource timeout, int generation)
    {
        using var elapsedCancellation = new CancellationTokenSource();
        var elapsedTask = PublishElapsedAsync("Connecting", elapsedCancellation.Token, generation);
        IWorkbenchMcpSession? session = null;
        try
        {
            Log.ForContext<WorkbenchViewModel>().Information("Connecting to MCP executable {ExecutablePath}.", McpExecutablePath);
            session = await _startSession(McpExecutablePath, timeout.Token).ConfigureAwait(false);
            session.HostExited += _hostExitedHandler;
            var discard = false;
            var closing = false;
            var hostAlreadyExited = false;
            lock (_state)
            {
                closing = _closing;
                hostAlreadyExited = session.HasExited;
                if (closing || _client is not null || _hostExitObserved || hostAlreadyExited)
                {
                    discard = true;
                }
                else
                {
                    _client = session;
                    _protocolVersion = session.ProtocolVersion;
                    _serverName = session.ServerName;
                }
            }

            if (discard)
            {
                session.HostExited -= _hostExitedHandler;
                await session.DisposeAsync().ConfigureAwait(false);
                if (hostAlreadyExited && !closing)
                {
                    Publish(() =>
                    {
                        Status = "Host exited.";
                        Diagnostics = "The MCP host process ended before the shell could keep the session. Connect starts a new host.";
                        ClearDiscovery();
                    });
                }
                else if (!ShouldReportHostExit())
                {
                    Publish(() =>
                    {
                        Status = "Disconnected";
                        Diagnostics = StopText("Connection canceled.", "Connection timed out.");
                        ClearDiscovery();
                    });
                }

                return;
            }

            var version = session.ServerVersion;
            var protocol = session.ProtocolVersion;
            var serverName = session.ServerName;
            var toolCount = session.Tools.Count;
            var capabilities = string.Join(", ", session.Capabilities.Select(pair => pair.Key));
            if (capabilities.Length == 0)
            {
                capabilities = "none";
            }

            Log.ForContext<WorkbenchViewModel>().Information(
                "Connected to MCP server {ServerName} {ServerVersion} using protocol {ProtocolVersion}. {ToolCount} tools discovered.",
                serverName,
                version,
                protocol,
                toolCount);
            Publish(() =>
            {
                ShowDiscovery(session);
                Status = $"Connected to {serverName} MCP {version}.";
                Diagnostics = $"MCP initialize completed using protocol {protocol}. Capabilities: {capabilities}. {toolCount} tools discovered. No workspace is open.";
            });
        }
        catch (OperationCanceledException)
        {
            Log.ForContext<WorkbenchViewModel>().Information("Connection to {ExecutablePath} was canceled.", McpExecutablePath);
            await DisposeIfUnownedAsync(session).ConfigureAwait(false);
            if (!ShouldReportHostExit())
            {
                Publish(() =>
                {
                    Status = "Disconnected";
                    Diagnostics = StopText("Connection canceled.", "Connection timed out.");
                    ClearDiscovery();
                });
            }
        }
        catch (Exception exception)
        {
            Log.ForContext<WorkbenchViewModel>().Warning(exception, "Connection to {ExecutablePath} failed.", McpExecutablePath);
            await DisposeIfUnownedAsync(session).ConfigureAwait(false);
            if (!ShouldReportHostExit())
            {
                Publish(() =>
                {
                    Status = "Disconnected";
                    Diagnostics = FormatError("Connection failed", exception);
                    ClearDiscovery();
                });
            }
        }
        finally
        {
            await StopElapsedAsync(elapsedCancellation, elapsedTask).ConfigureAwait(false);
            ReleaseOperation(timeout, connecting: true);
            Publish(NotifyCommands);
            CompleteOperation();
        }
    }

    private Task RunSessionOperationAsync(Func<CancellationToken, Task> operation, string phase)
    {
        CancellationTokenSource linked;
        int generation;
        lock (_state)
        {
            if (_closing || _busy || _client is null || _hostExitObserved)
            {
                return Task.CompletedTask;
            }

            _busy = true;
            generation = ++_activityGeneration;
            linked = BeginOperation(_operationTimeout);
            _operationCompleted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        var running = RunSessionOperationCoreAsync(operation, linked, generation, phase);
        Publish(NotifyCommands);
        return running;
    }

    private async Task RunSessionOperationCoreAsync(
        Func<CancellationToken, Task> operation,
        CancellationTokenSource timeout,
        int generation,
        string phase)
    {
        using var elapsedCancellation = new CancellationTokenSource();
        var elapsedTask = PublishElapsedAsync(phase, elapsedCancellation.Token, generation);
        try
        {
            await operation(timeout.Token).ConfigureAwait(false);
        }
        finally
        {
            await StopElapsedAsync(elapsedCancellation, elapsedTask).ConfigureAwait(false);
            ReleaseOperation(timeout, connecting: false);
            Publish(NotifyCommands);
            CompleteOperation();
        }
    }

    private async Task OpenWorkspaceCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            Log.ForContext<WorkbenchViewModel>().Information(
                "Opening workspace for {Release} in {DataDirectory} with plugins {SelectedPlugins}.",
                Release,
                DataDirectory,
                SelectedPlugins);
            ValidateDirectory(DataDirectory, "data directory");
            var plugins = SelectedPlugins
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var pluginNodes = new JsonArray();
            foreach (var plugin in plugins)
            {
                pluginNodes.Add(plugin);
            }

            var result = await CallAsync("workspace_open", new JsonObject
            {
                ["operationId"] = "workbench-open-" + Guid.NewGuid().ToString("N"),
                ["release"] = Release,
                ["dataDirectory"] = DataDirectory,
                ["selectedPlugins"] = pluginNodes,
            }, cancellationToken).ConfigureAwait(false);
            var workspaceId = ReadString(result["structuredContent"]?["workspaceId"])
                ?? ReadString(result["workspaceId"]);
            var store = false;
            lock (_state)
            {
                if (!_closing && !string.IsNullOrWhiteSpace(workspaceId))
                {
                    _workspaceId = workspaceId;
                    store = true;
                }
            }

            if (!store)
            {
                Log.ForContext<WorkbenchViewModel>().Warning(
                    "Workspace open returned no session identifier. Result: {Result}",
                    Truncate(result.ToJsonString(IndentedJson)));
            }
            else
            {
                Log.ForContext<WorkbenchViewModel>().Information("Workspace session {WorkspaceId} is ready.", workspaceId);
            }

            Publish(() =>
            {
                if (!store)
                {
                    Status = "Workspace open failed.";
                    Diagnostics = "The MCP host did not return a workspace session identifier."
                        + Environment.NewLine
                        + result.ToJsonString(IndentedJson);
                    return;
                }

                Status = "Workspace session ready." + RevisionSuffix(result);
                Diagnostics = result.ToJsonString(IndentedJson);
            });
        }
        catch (OperationCanceledException)
        {
            Log.ForContext<WorkbenchViewModel>().Information("Workspace open was canceled.");
            if (ShouldReportHostExit())
            {
                return;
            }

            Publish(() =>
            {
                Status = StopText("Workspace open canceled.", "Workspace open timed out.");
                Diagnostics = StopText("Workspace open canceled.", "Workspace open timed out.");
            });
        }
        catch (Exception exception)
        {
            Log.ForContext<WorkbenchViewModel>().Warning(exception, "Workspace open failed for {DataDirectory}.", DataDirectory);
            if (ShouldReportHostExit())
            {
                return;
            }

            Publish(() =>
            {
                Status = "Workspace open failed.";
                Diagnostics = FormatError("Workspace open failed", exception);
            });
        }
    }

    private async Task OpenOutputCoreAsync(bool createNew, CancellationToken cancellationToken)
    {
        try
        {
            string? workspaceId;
            var outputAttached = false;
            var closing = false;
            lock (_state)
            {
                workspaceId = _workspaceId;
                outputAttached = _outputAttached;
                closing = _closing;
            }

            var tool = createNew ? "output_create" : "output_open";
            if (workspaceId is null || outputAttached || closing)
            {
                Log.ForContext<WorkbenchViewModel>().Warning("Skipped {Tool} because no pending workspace can accept an output.", tool);
                return;
            }

            ValidateFilePath(OutputPath, "output path");
            Log.ForContext<WorkbenchViewModel>().Information(
                "Calling {Tool} for workspace {WorkspaceId} at {OutputPath} with master style {MasterStyle}, text storage {TextStorageMode}, and language {Language}.",
                tool,
                workspaceId,
                OutputPath,
                MasterStyle,
                TextStorageMode,
                Language);
            var result = await CallAsync(tool, new JsonObject
            {
                ["operationId"] = "workbench-output-" + Guid.NewGuid().ToString("N"),
                ["workspaceId"] = workspaceId,
                ["outputPath"] = OutputPath,
                ["masterStyle"] = MasterStyle,
                ["textStorageMode"] = TextStorageMode,
                ["language"] = Language,
            }, cancellationToken).ConfigureAwait(false);
            lock (_state)
            {
                _outputAttached = true;
            }

            if (!createNew)
            {
                Log.ForContext<WorkbenchViewModel>().Information("Opened existing output {OutputPath}.", OutputPath);
                Publish(() =>
                {
                    Status = "Existing output attached to the workspace.";
                    Diagnostics = result.ToJsonString(IndentedJson);
                });
                return;
            }

            Log.ForContext<WorkbenchViewModel>().Information("Saving new output {OutputPath} for workspace {WorkspaceId}.", OutputPath, workspaceId);
            var save = await CallAsync("workspace_save", new JsonObject
            {
                ["operationId"] = "workbench-save-" + Guid.NewGuid().ToString("N"),
                ["workspaceId"] = workspaceId,
            }, cancellationToken).ConfigureAwait(false);
            var saveStatus = ReadString(save["structuredContent"]?["status"]);
            var destination = ReadString(save["structuredContent"]?["destinationPath"]) ?? OutputPath;
            if (!string.Equals(saveStatus, "Succeeded", StringComparison.Ordinal))
            {
                Log.ForContext<WorkbenchViewModel>().Warning(
                    "Save of {OutputPath} returned status {SaveStatus}. Result: {Result}",
                    OutputPath,
                    saveStatus,
                    Truncate(save.ToJsonString(IndentedJson)));
                await AbandonUnsavedOutputAsync().ConfigureAwait(false);
                Publish(() =>
                {
                    Status = "Output save failed.";
                    Diagnostics = save.ToJsonString(IndentedJson);
                });
                return;
            }

            Log.ForContext<WorkbenchViewModel>().Information("Created output file {DestinationPath}.", destination);
            Publish(() =>
            {
                Status = "Created " + destination + ".";
                Diagnostics = save.ToJsonString(IndentedJson);
            });
        }
        catch (OperationCanceledException)
        {
            Log.ForContext<WorkbenchViewModel>().Information("Output operation was canceled for {OutputPath}.", OutputPath);
            if (!ShouldReportHostExit())
            {
                await AbandonUnsavedOutputAsync().ConfigureAwait(false);
                Publish(() =>
                {
                    Status = StopText("Output operation canceled.", "Output operation timed out.");
                    Diagnostics = StopText("Output operation canceled.", "Output operation timed out.");
                });
            }
        }
        catch (Exception exception)
        {
            Log.ForContext<WorkbenchViewModel>().Warning(exception, "Output operation failed for {OutputPath}.", OutputPath);
            if (!ShouldReportHostExit())
            {
                await AbandonUnsavedOutputAsync().ConfigureAwait(false);
                Publish(() =>
                {
                    Status = "Output operation failed.";
                    Diagnostics = FormatError("Output operation failed", exception);
                });
            }
        }
    }

    /// <summary>Closes a workspace whose new output was created but not saved, and returns the shell to a connected idle state.</summary>
    private async Task AbandonUnsavedOutputAsync()
    {
        string? workspaceId;
        lock (_state)
        {
            if (!_outputAttached)
            {
                return;
            }

            workspaceId = _workspaceId;
            _outputAttached = false;
            _workspaceId = null;
        }

        if (workspaceId is null)
        {
            return;
        }

        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await CallAsync("workspace_close", new JsonObject
            {
                ["operationId"] = "workbench-abandon-" + Guid.NewGuid().ToString("N"),
                ["workspaceId"] = workspaceId,
                ["discardUnsaved"] = true,
            }, timeout.Token).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Log.ForContext<WorkbenchViewModel>().Warning(
                exception,
                "Could not close workspace {WorkspaceId} after the new output was not saved.",
                workspaceId);
        }
    }

    private async Task CloseCoreAsync()
    {
        // Yield so callers can publish this task before its body takes the state lock.
        await Task.Yield();
        try
        {
            Task? pending;
            lock (_state)
            {
                pending = _operationCompleted?.Task;
            }

            if (pending is not null)
            {
                try
                {
                    await pending.ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }

            IWorkbenchMcpSession? client;
            string? workspaceId;
            lock (_state)
            {
                client = _client;
                workspaceId = _workspaceId;
                _client = null;
                _workspaceId = null;
            }

            if (client is not null && workspaceId is not null)
            {
                try
                {
                    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    await client.CallAsync("workspace_close", new JsonObject
                    {
                        ["operationId"] = "workbench-close-" + Guid.NewGuid().ToString("N"),
                        ["workspaceId"] = workspaceId,
                        ["discardUnsaved"] = true,
                    }, timeout.Token).ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    Log.ForContext<WorkbenchViewModel>().Warning(
                        exception,
                        "Workspace close failed for {WorkspaceId}. The child process will still be stopped.",
                        workspaceId);
                    Publish(() => Diagnostics = FormatError("Workspace close failed; the child process will still be stopped", exception));
                }
            }

            if (client is not null)
            {
                client.HostExited -= _hostExitedHandler;
                await client.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            lock (_state)
            {
                _client = null;
                _workspaceId = null;
                _outputAttached = false;
                _connecting = false;
                _busy = false;
                _closing = false;
                _hostExitObserved = false;
                _protocolVersion = string.Empty;
                _serverName = string.Empty;
                _closeTask = null;
            }

            Log.ForContext<WorkbenchViewModel>().Information("Workbench session closed.");
            Publish(() =>
            {
                Status = "Disconnected";
                ClearDiscovery();
                NotifyCommands();
            });
        }
    }

    private async Task<JsonObject> CallAsync(string tool, JsonObject arguments, CancellationToken cancellationToken)
    {
        IWorkbenchMcpSession? client;
        lock (_state)
        {
            client = _client;
        }

        if (client is null)
        {
            throw new InvalidOperationException("The MCP session is not connected.");
        }

        return await client.CallAsync(tool, arguments, cancellationToken).ConfigureAwait(false);
    }

    private async Task DisposeIfUnownedAsync(IWorkbenchMcpSession? session)
    {
        if (session is null)
        {
            return;
        }

        var owned = false;
        lock (_state)
        {
            owned = ReferenceEquals(_client, session);
            if (!owned && ReferenceEquals(_client, null))
            {
                _client = null;
            }
        }

        if (!owned)
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static void ValidateDirectory(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value) || !Directory.Exists(value))
        {
            throw new DirectoryNotFoundException($"The selected {name} does not exist: '{value}'.");
        }
    }

    private static void ValidateFilePath(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"Select an explicit {name} before continuing.");
        }
    }

    private static string FormatError(string phase, Exception exception)
    {
        if (exception is WorkbenchMcpException mcp)
        {
            var code = string.IsNullOrWhiteSpace(mcp.ErrorCode) ? string.Empty : $" ({mcp.ErrorCode})";
            return $"{phase}: {mcp.Kind} error{code}: {mcp.Message}";
        }

        return $"{phase}: {exception.Message}";
    }

    /// <summary>Returns the status suffix for a revision carried on a tool result.</summary>
    /// <param name="result">The tool result object.</param>
    /// <returns>A revision sentence, or an empty string when the result has no revision.</returns>
    private static string RevisionSuffix(JsonObject result)
    {
        return WorkbenchSessionReceipts.TryReadUInt64(result["structuredContent"]?["revision"], out var revision)
            ? $" Revision {revision}."
            : string.Empty;
    }

    /// <summary>Creates the linked user and timeout tokens for one bounded call.</summary>
    /// <param name="limit">The maximum wait for this call.</param>
    /// <returns>The linked token source passed to the call.</returns>
    private CancellationTokenSource BeginOperation(TimeSpan limit)
    {
        _userCancellation = new CancellationTokenSource();
        _timeoutCancellation = new CancellationTokenSource(limit);
        _linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(_userCancellation.Token, _timeoutCancellation.Token);
        _activeCancellation = _userCancellation;
        return _linkedCancellation;
    }

    /// <summary>Drops the operation tokens when they still belong to <paramref name="timeout"/>.</summary>
    /// <param name="timeout">The linked token source started for this operation.</param>
    /// <param name="connecting"><see langword="true"/> when the operation is startup rather than a tool call.</param>
    private void ReleaseOperation(CancellationTokenSource timeout, bool connecting)
    {
        CancellationTokenSource? user;
        CancellationTokenSource? limit;
        CancellationTokenSource? linked;
        lock (_state)
        {
            if (connecting)
            {
                _connecting = false;
            }
            else
            {
                _busy = false;
            }

            if (!ReferenceEquals(_linkedCancellation, timeout))
            {
                return;
            }

            user = _userCancellation;
            limit = _timeoutCancellation;
            linked = _linkedCancellation;
            _userCancellation = null;
            _timeoutCancellation = null;
            _linkedCancellation = null;
            _activeCancellation = null;
        }

        user?.Dispose();
        limit?.Dispose();
        linked?.Dispose();
    }

    /// <summary>Marks the active operation complete so close and host-exit can continue.</summary>
    private void CompleteOperation()
    {
        lock (_state)
        {
            _operationCompleted?.TrySetResult();
            _operationCompleted = null;
        }
    }

    /// <summary>Chooses the canceled or timed-out text for the active operation.</summary>
    /// <param name="canceled">The text used when the user canceled the call.</param>
    /// <param name="timedOut">The text used when the call reached its time limit.</param>
    /// <returns><paramref name="timedOut"/> when the timeout fired without a user cancellation.</returns>
    private string StopText(string canceled, string timedOut)
    {
        lock (_state)
        {
            var timedOutNow = _timeoutCancellation is { IsCancellationRequested: true }
                && _userCancellation is not { IsCancellationRequested: true };
            return timedOutNow ? timedOut : canceled;
        }
    }

    /// <summary>Gets whether an unexpected host exit already owns the status text.</summary>
    /// <returns><see langword="true"/> when the host-exit path will report the failure.</returns>
    private bool ShouldReportHostExit()
    {
        lock (_state)
        {
            return _hostExitObserved;
        }
    }

    /// <summary>Stops the elapsed-time loop for one operation.</summary>
    /// <param name="elapsedCancellation">The token source that owns the loop.</param>
    /// <param name="elapsedTask">The loop task.</param>
    /// <returns>A task that completes after the loop has cleared its activity text.</returns>
    private static async Task StopElapsedAsync(CancellationTokenSource elapsedCancellation, Task elapsedTask)
    {
        elapsedCancellation.Cancel();
        try
        {
            await elapsedTask.ConfigureAwait(false);
        }
        catch (Exception)
        {
        }
    }

    /// <summary>Publishes the active phase and whole elapsed seconds until <paramref name="cancellationToken"/> is canceled.</summary>
    /// <param name="phase">The call phase shown beside the elapsed time.</param>
    /// <param name="cancellationToken">Cancels the loop when the call finishes.</param>
    /// <param name="generation">The activity generation that owns this loop.</param>
    /// <returns>A task that completes after the activity text is cleared or a newer call has replaced it.</returns>
    private async Task PublishElapsedAsync(string phase, CancellationToken cancellationToken, int generation)
    {
        var watch = Stopwatch.StartNew();
        PublishActivity(generation, phase, watch.Elapsed);
        try
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
                PublishActivity(generation, phase, watch.Elapsed);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            Publish(() =>
            {
                if (_activityGeneration != generation)
                {
                    return;
                }

                _activityText = string.Empty;
                OnPropertyChanged(nameof(ActivityText));
                OnPropertyChanged(nameof(HasActivity));
            });
        }
    }

    /// <summary>Shows one elapsed activity line when it still belongs to the current call.</summary>
    /// <param name="generation">The activity generation that owns the line.</param>
    /// <param name="phase">The call phase.</param>
    /// <param name="elapsed">The time since the call started.</param>
    private void PublishActivity(int generation, string phase, TimeSpan elapsed)
    {
        var seconds = Math.Max(0, (int)elapsed.TotalSeconds);
        Publish(() =>
        {
            if (_activityGeneration != generation)
            {
                return;
            }

            _activityText = $"{phase} ({seconds}s)";
            OnPropertyChanged(nameof(ActivityText));
            OnPropertyChanged(nameof(HasActivity));
        });
    }

    /// <summary>Starts cleanup when the connected child exits before the shell disposes it.</summary>
    /// <param name="sender">The session whose process exited.</param>
    /// <param name="eventArgs">The exit arguments. They are unused.</param>
    private void OnHostExited(object? sender, EventArgs eventArgs)
    {
        if (sender is not IWorkbenchMcpSession session)
        {
            return;
        }

        lock (_state)
        {
            if (_closing || _hostExitObserved || !ReferenceEquals(_client, session))
            {
                return;
            }

            _hostExitObserved = true;
            Cancel(_userCancellation);
        }

        _ = FinishHostExitAsync(session);
    }

    /// <summary>Disposes an exited host and returns the shell to a usable disconnected screen.</summary>
    /// <param name="session">The session whose process exited.</param>
    /// <returns>A task that completes after the child is released and the screen is updated.</returns>
    private async Task FinishHostExitAsync(IWorkbenchMcpSession session)
    {
        Task? pending;
        lock (_state)
        {
            pending = _operationCompleted?.Task;
        }

        if (pending is not null)
        {
            try
            {
                await pending.ConfigureAwait(false);
            }
            catch (Exception)
            {
            }
        }

        var owned = false;
        lock (_state)
        {
            if (_closing || !ReferenceEquals(_client, session))
            {
                _hostExitObserved = false;
                return;
            }

            _client = null;
            _workspaceId = null;
            _outputAttached = false;
            _connecting = false;
            _busy = false;
            _protocolVersion = string.Empty;
            _serverName = string.Empty;
            owned = true;
        }

        if (!owned)
        {
            return;
        }

        session.HostExited -= _hostExitedHandler;
        try
        {
            await session.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Log.ForContext<WorkbenchViewModel>().Warning(exception, "Disposing an exited MCP host failed.");
        }

        lock (_state)
        {
            _hostExitObserved = false;
        }

        Log.ForContext<WorkbenchViewModel>().Warning("MCP host exited. Session and revision identifiers were cleared.");
        Publish(() =>
        {
            Status = "Host exited.";
            Diagnostics = "The MCP host process ended. Session and revision identifiers were cleared. Connect starts a new host.";
            ClearDiscovery();
            NotifyCommands();
        });
    }

    /// <summary>Replaces the discovered tool list with the tools from <paramref name="session"/>.</summary>
    /// <param name="session">The connected session.</param>
    private void ShowDiscovery(IWorkbenchMcpSession session)
    {
        DiscoveredTools.Clear();
        foreach (var tool in session.Tools)
        {
            DiscoveredTools.Add(tool);
        }

        SelectedTool = DiscoveredTools.FirstOrDefault();
        OnPropertyChanged(nameof(DiscoverySummary));
    }

    /// <summary>Clears the discovered tool list and the selected schema.</summary>
    private void ClearDiscovery()
    {
        DiscoveredTools.Clear();
        SelectedTool = null;
        OnPropertyChanged(nameof(DiscoverySummary));
    }

    /// <summary>Returns <paramref name="value"/> when it is positive; otherwise <paramref name="fallback"/>.</summary>
    /// <param name="value">The caller-supplied limit.</param>
    /// <param name="fallback">The limit used when <paramref name="value"/> is missing or not positive.</param>
    /// <returns>A positive time limit.</returns>
    private static TimeSpan Positive(TimeSpan? value, TimeSpan fallback)
    {
        return value is TimeSpan timeout && timeout > TimeSpan.Zero ? timeout : fallback;
    }

    /// <summary>Limits a tool result copied into the log so a large payload does not fill the file.</summary>
    /// <param name="value">The serialized tool result.</param>
    /// <returns>At most the first 4000 characters of <paramref name="value"/>.</returns>
    private static string Truncate(string value)
    {
        const int limit = 4000;
        return value.Length <= limit ? value : value[..limit];
    }

    private static string? ReadString(JsonNode? node)
    {
        return node is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;
    }

    private static void Cancel(CancellationTokenSource? source)
    {
        if (source is null)
        {
            return;
        }

        try
        {
            source.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
    }

    private void NotifyCommands()
    {
        OnPropertyChanged(nameof(CanConnect));
        OnPropertyChanged(nameof(CanDisconnect));
        OnPropertyChanged(nameof(CanCancel));
        OnPropertyChanged(nameof(CanEditSources));
        OnPropertyChanged(nameof(CanEditOutput));
        OnPropertyChanged(nameof(CanOpenWorkspace));
        OnPropertyChanged(nameof(CanCreateOutput));
        OnPropertyChanged(nameof(CanOpenOutput));
    }

    private void Publish(Action action)
    {
        var context = _uiContext;
        if (context is null || SynchronizationContext.Current == context)
        {
            action();
            return;
        }

        context.Send(_ => action(), null);
    }

    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
        if (!_applyingSettings && propertyName is not null && TrackedSettings.Contains(propertyName))
        {
            Persist();
        }
    }

    /// <summary>Restores saved selections and fills an MCP host or Data directory that is still blank or missing.</summary>
    /// <param name="state">The settings read from <c>CreationsForge.Config.json</c>.</param>
    private void ApplyLoadedSettings(WorkbenchSettingsState state)
    {
        _applyingSettings = true;
        try
        {
            _profiles.Clear();
            foreach (var pair in state.Releases)
            {
                _profiles[pair.Key] = CopyRelease(pair.Value);
            }

            var release = NormalizeChoice(state.Release, Releases, "Starfield");
            if (!string.Equals(_release, release, StringComparison.Ordinal))
            {
                _release = release;
                OnPropertyChanged(nameof(Release));
            }

            Language = NormalizeChoice(state.Language, Languages, "English");
            McpExecutablePath = ResolveMcpPath(state.McpExecutablePath);
            ApplyProfileFields(release);
            EnsureDataDirectory();
        }
        finally
        {
            _applyingSettings = false;
        }

        Persist();
    }

    /// <summary>Stores the current release profile, then shows the profile for <paramref name="value"/>.</summary>
    /// <param name="value">The release selected in the shell.</param>
    private void ChangeRelease(string? value)
    {
        var normalized = NormalizeChoice(value, Releases, "Starfield");
        if (string.Equals(_release, normalized, StringComparison.Ordinal))
        {
            return;
        }

        if (!_applyingSettings)
        {
            RememberCurrentProfile();
        }

        _applyingSettings = true;
        try
        {
            _release = normalized;
            OnPropertyChanged(nameof(Release));
            ApplyProfileFields(normalized);
            EnsureDataDirectory();
        }
        finally
        {
            _applyingSettings = false;
        }

        Persist();
    }

    /// <summary>Copies one release profile into the bound fields.</summary>
    /// <param name="release">The release whose profile should become visible.</param>
    private void ApplyProfileFields(string release)
    {
        if (!_profiles.TryGetValue(release, out var profile))
        {
            profile = new WorkbenchReleaseSettings();
            _profiles[release] = profile;
        }

        DataDirectory = profile.DataDirectory;
        SelectedPlugins = profile.SelectedPlugins;
        OutputPath = profile.OutputPath;
        MasterStyle = profile.MasterStyle;
        TextStorageMode = profile.TextStorageMode;
    }

    /// <summary>Fills the Data directory when the current release does not already point at a directory.</summary>
    private void EnsureDataDirectory()
    {
        if (!string.IsNullOrWhiteSpace(_dataDirectory) && Directory.Exists(_dataDirectory))
        {
            return;
        }

        var found = _findDataDirectory?.Invoke(_release);
        if (string.IsNullOrWhiteSpace(found) || !Directory.Exists(found))
        {
            return;
        }

        Log.ForContext<WorkbenchViewModel>().Information(
            "Using installed Data directory {DataDirectory} for {Release}.",
            found,
            _release);
        DataDirectory = found;
    }

    /// <summary>Keeps a saved MCP path that still exists. Otherwise uses the located host.</summary>
    /// <param name="saved">The path stored in the configuration file.</param>
    /// <returns>The path the shell should show.</returns>
    private string ResolveMcpPath(string? saved)
    {
        if (!string.IsNullOrWhiteSpace(saved) && File.Exists(saved))
        {
            return saved;
        }

        var found = _findMcpExecutable?.Invoke();
        if (string.IsNullOrWhiteSpace(found) || !File.Exists(found))
        {
            return saved ?? string.Empty;
        }

        Log.ForContext<WorkbenchViewModel>().Information("Using MCP executable {ExecutablePath}.", found);
        return found;
    }

    /// <summary>Writes the visible selections when a settings store is attached.</summary>
    private void Persist()
    {
        if (_settings is null || _applyingSettings)
        {
            return;
        }

        RememberCurrentProfile();
        try
        {
            _settings.Save(ToState());
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Log.ForContext<WorkbenchViewModel>().Warning(exception, "Could not save Workbench settings.");
        }
    }

    /// <summary>Copies the visible release fields into the profile map.</summary>
    private void RememberCurrentProfile()
    {
        _profiles[_release] = new WorkbenchReleaseSettings
        {
            DataDirectory = _dataDirectory,
            SelectedPlugins = _selectedPlugins,
            OutputPath = _outputPath,
            MasterStyle = _masterStyle,
            TextStorageMode = _textStorageMode,
        };
    }

    /// <summary>Copies the profile map into the object the settings store saves.</summary>
    /// <returns>The current selections.</returns>
    private WorkbenchSettingsState ToState()
    {
        var state = new WorkbenchSettingsState
        {
            Release = _release,
            Language = _language,
            McpExecutablePath = _mcpExecutablePath,
        };
        foreach (var pair in _profiles)
        {
            state.Releases[pair.Key] = CopyRelease(pair.Value);
        }

        return state;
    }

    /// <summary>Copies one release profile so a later edit does not change a saved snapshot.</summary>
    /// <param name="source">The profile to copy.</param>
    /// <returns>A separate profile with the same selections.</returns>
    private static WorkbenchReleaseSettings CopyRelease(WorkbenchReleaseSettings source)
    {
        return new WorkbenchReleaseSettings
        {
            DataDirectory = source.DataDirectory,
            SelectedPlugins = source.SelectedPlugins,
            OutputPath = source.OutputPath,
            MasterStyle = source.MasterStyle,
            TextStorageMode = source.TextStorageMode,
        };
    }

    /// <summary>Returns <paramref name="value"/> when it is one of <paramref name="allowed"/>; otherwise <paramref name="fallback"/>.</summary>
    /// <param name="value">The stored or selected value.</param>
    /// <param name="allowed">The exact names the MCP host accepts.</param>
    /// <param name="fallback">The value used when <paramref name="value"/> is not one of those names.</param>
    /// <returns>A name the shell can send to the MCP host.</returns>
    private static string NormalizeChoice(string? value, IReadOnlyList<string> allowed, string fallback)
    {
        return value is not null && allowed.Contains(value, StringComparer.Ordinal) ? value : fallback;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
