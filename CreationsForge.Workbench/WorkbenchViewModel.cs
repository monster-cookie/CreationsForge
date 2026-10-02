using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace CreationsForge.Workbench;

/// <summary>Owns explicit Workbench selections, visible status, and the MCP child lifetime.</summary>
public sealed class WorkbenchViewModel : INotifyPropertyChanged
{
    private static readonly System.Text.Json.JsonSerializerOptions IndentedJson = new() { WriteIndented = true };
    private readonly Func<string, CancellationToken, Task<IWorkbenchMcpSession>> _startSession;
    private readonly SynchronizationContext? _uiContext;
    private readonly object _state = new();
    private IWorkbenchMcpSession? _client;
    private CancellationTokenSource? _activeCancellation;
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
    private string? _workspaceId;
    private bool _connecting;
    private bool _busy;
    private bool _closing;

    /// <summary>Creates a view model that launches the production MCP executable.</summary>
    public WorkbenchViewModel()
        : this(static async (path, token) => await StdioMcpClient.StartAsync(path, token).ConfigureAwait(false))
    {
    }

    /// <summary>Creates a view model that starts sessions through a caller-supplied factory.</summary>
    /// <param name="startSession">Starts one MCP session for an executable path. Canceled startup must not leave a running child.</param>
    internal WorkbenchViewModel(Func<string, CancellationToken, Task<IWorkbenchMcpSession>> startSession)
    {
        ArgumentNullException.ThrowIfNull(startSession);
        _startSession = startSession;
        _uiContext = SynchronizationContext.Current;
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

    /// <summary>Gets or sets the selected game release.</summary>
    public string Release { get => _release; set => Set(ref _release, value); }

    /// <summary>Gets or sets the requested output master style.</summary>
    public string MasterStyle { get => _masterStyle; set => Set(ref _masterStyle, value); }

    /// <summary>Gets or sets the requested output text storage mode.</summary>
    public string TextStorageMode { get => _textStorageMode; set => Set(ref _textStorageMode, value); }

    /// <summary>Gets or sets the active translated-string language sent to output tools.</summary>
    public string Language { get => _language; set => Set(ref _language, value); }

    /// <summary>Gets the short connection state shown beside the controls.</summary>
    public string Status { get => _status; private set => Set(ref _status, value); }

    /// <summary>Gets copyable diagnostics for the latest connection, workspace, or output result.</summary>
    public string Diagnostics { get => _diagnostics; private set => Set(ref _diagnostics, value); }

    /// <summary>Gets whether Connect can start a new child process.</summary>
    public bool CanConnect => _client is null && !_connecting && !_closing;

    /// <summary>Gets whether Disconnect can cancel startup or stop the current child.</summary>
    public bool CanDisconnect => (_client is not null || _connecting) && !_closing;

    /// <summary>Gets whether the source release, data directory, and plugin list still apply to the next workspace open.</summary>
    public bool CanEditSources => _workspaceId is null && !_closing;

    /// <summary>Gets whether Open workspace can send <c>workspace_open</c>.</summary>
    public bool CanOpenWorkspace => _client is not null && _workspaceId is null && !_busy && !_closing;

    /// <summary>Gets whether Create output can send <c>output_create</c>.</summary>
    public bool CanCreateOutput => _client is not null && _workspaceId is not null && !_busy && !_closing;

    /// <summary>Gets whether Open output can send <c>output_open</c>.</summary>
    public bool CanOpenOutput => CanCreateOutput;

    /// <summary>Starts the selected MCP executable. A missing file or failed startup stays on the disconnected screen.</summary>
    /// <returns>A task that completes when the session is connected, canceled, or visibly failed.</returns>
    public Task ConnectAsync()
    {
        CancellationTokenSource timeout;
        lock (_state)
        {
            if (_closing || _connecting || _client is not null)
            {
                return Task.CompletedTask;
            }

            _connecting = true;
            timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            _activeCancellation = timeout;
            _operationCompleted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        var connect = ConnectCoreAsync(timeout);
        Publish(NotifyCommands);
        return connect;
    }

    /// <summary>Sends <c>workspace_open</c> for the explicit release, data directory, and source plugins.</summary>
    /// <returns>A task that completes when the session id is stored or the failure is visible.</returns>
    public Task OpenWorkspaceAsync() => RunSessionOperationAsync(OpenWorkspaceCoreAsync);

    /// <summary>Sends <c>output_create</c> or <c>output_open</c> for the explicit output settings.</summary>
    /// <param name="createNew"><see langword="true"/> to create a new plugin; <see langword="false"/> to open an existing one.</param>
    /// <returns>A task that completes when the result or failure is visible. A tool error does not claim the output was attached.</returns>
    public Task OpenOutputAsync(bool createNew) => RunSessionOperationAsync(token => OpenOutputCoreAsync(createNew, token));

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

    private async Task ConnectCoreAsync(CancellationTokenSource timeout)
    {
        IWorkbenchMcpSession? session = null;
        try
        {
            session = await _startSession(McpExecutablePath, timeout.Token).ConfigureAwait(false);
            var discard = false;
            lock (_state)
            {
                if (_closing || _client is not null)
                {
                    discard = true;
                }
                else
                {
                    _client = session;
                }
            }

            if (discard)
            {
                await session.DisposeAsync().ConfigureAwait(false);
                Publish(() =>
                {
                    Status = "Disconnected";
                    Diagnostics = "Connection canceled.";
                });
                return;
            }

            var version = session.ServerVersion;
            Publish(() =>
            {
                Status = $"Connected to CreationsForge MCP {version}.";
                Diagnostics = "MCP initialize completed. No workspace is open.";
            });
        }
        catch (OperationCanceledException)
        {
            await DisposeIfUnownedAsync(session).ConfigureAwait(false);
            Publish(() =>
            {
                Status = "Disconnected";
                Diagnostics = "Connection canceled.";
            });
        }
        catch (Exception exception)
        {
            await DisposeIfUnownedAsync(session).ConfigureAwait(false);
            Publish(() =>
            {
                Status = "Disconnected";
                Diagnostics = FormatError("Connection failed", exception);
            });
        }
        finally
        {
            lock (_state)
            {
                _connecting = false;
                if (ReferenceEquals(_activeCancellation, timeout))
                {
                    _activeCancellation = null;
                }

                _operationCompleted?.TrySetResult();
                _operationCompleted = null;
            }

            timeout.Dispose();
            Publish(NotifyCommands);
        }
    }

    private Task RunSessionOperationAsync(Func<CancellationToken, Task> operation)
    {
        CancellationTokenSource timeout;
        lock (_state)
        {
            if (_closing || _busy || _client is null)
            {
                return Task.CompletedTask;
            }

            _busy = true;
            timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            _activeCancellation = timeout;
            _operationCompleted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        var running = RunSessionOperationCoreAsync(operation, timeout);
        Publish(NotifyCommands);
        return running;
    }

    private async Task RunSessionOperationCoreAsync(Func<CancellationToken, Task> operation, CancellationTokenSource timeout)
    {
        try
        {
            await operation(timeout.Token).ConfigureAwait(false);
        }
        finally
        {
            lock (_state)
            {
                _busy = false;
                if (ReferenceEquals(_activeCancellation, timeout))
                {
                    _activeCancellation = null;
                }

                _operationCompleted?.TrySetResult();
                _operationCompleted = null;
            }

            timeout.Dispose();
            Publish(NotifyCommands);
        }
    }

    private async Task OpenWorkspaceCoreAsync(CancellationToken cancellationToken)
    {
        try
        {
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

                Status = "Workspace session ready.";
                Diagnostics = result.ToJsonString(IndentedJson);
            });
        }
        catch (OperationCanceledException)
        {
            Publish(() => Diagnostics = "Workspace open canceled.");
        }
        catch (Exception exception)
        {
            Publish(() => Diagnostics = FormatError("Workspace open failed", exception));
        }
    }

    private async Task OpenOutputCoreAsync(bool createNew, CancellationToken cancellationToken)
    {
        try
        {
            string? workspaceId;
            lock (_state)
            {
                workspaceId = _workspaceId;
            }

            if (workspaceId is null || _closing)
            {
                return;
            }

            ValidateFilePath(OutputPath, "output path");
            var tool = createNew ? "output_create" : "output_open";
            var result = await CallAsync(tool, new JsonObject
            {
                ["operationId"] = "workbench-output-" + Guid.NewGuid().ToString("N"),
                ["workspaceId"] = workspaceId,
                ["outputPath"] = OutputPath,
                ["masterStyle"] = MasterStyle,
                ["textStorageMode"] = TextStorageMode,
                ["language"] = Language,
            }, cancellationToken).ConfigureAwait(false);
            Publish(() =>
            {
                Status = createNew ? "New output attached to the workspace." : "Existing output attached to the workspace.";
                Diagnostics = result.ToJsonString(IndentedJson);
            });
        }
        catch (OperationCanceledException)
        {
            Publish(() => Diagnostics = "Output operation canceled.");
        }
        catch (Exception exception)
        {
            Publish(() => Diagnostics = FormatError("Output operation failed", exception));
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
                    Publish(() => Diagnostics = FormatError("Workspace close failed; the child process will still be stopped", exception));
                }
            }

            if (client is not null)
            {
                await client.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            lock (_state)
            {
                _client = null;
                _workspaceId = null;
                _connecting = false;
                _busy = false;
                _closing = false;
                _closeTask = null;
            }

            Publish(() =>
            {
                Status = "Disconnected";
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

    private static string FormatError(string phase, Exception exception) => $"{phase}: {exception.Message}";

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
        OnPropertyChanged(nameof(CanEditSources));
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
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
