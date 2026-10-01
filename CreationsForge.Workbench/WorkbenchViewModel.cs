using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace CreationsForge.Workbench;

/// <summary>Owns explicit Workbench selections, status, and asynchronous MCP lifecycle operations.</summary>
public sealed class WorkbenchViewModel : INotifyPropertyChanged
{
    private StdioMcpClient? _client;
    private string _mcpExecutablePath = string.Empty;
    private string _dataDirectory = string.Empty;
    private string _selectedPlugins = string.Empty;
    private string _outputPath = string.Empty;
    private string _release = "Starfield";
    private string _masterStyle = "Full";
    private string _textStorageMode = "Embedded";
    private string _status = "Disconnected";
    private string _diagnostics = "Select an MCP executable and connect to begin.";
    private string? _workspaceId;
    private bool _closing;

    public event PropertyChangedEventHandler? PropertyChanged;

    public IReadOnlyList<string> Releases { get; } = ["Starfield", "Fallout4", "SkyrimSE"];
    public IReadOnlyList<string> MasterStyles { get; } = ["Full", "Small", "Medium"];
    public IReadOnlyList<string> TextStorageModes { get; } = ["Embedded", "Localized"];

    public string McpExecutablePath { get => _mcpExecutablePath; set => Set(ref _mcpExecutablePath, value); }
    public string DataDirectory { get => _dataDirectory; set => Set(ref _dataDirectory, value); }
    public string SelectedPlugins { get => _selectedPlugins; set => Set(ref _selectedPlugins, value); }
    public string OutputPath { get => _outputPath; set => Set(ref _outputPath, value); }
    public string Release { get => _release; set => Set(ref _release, value); }
    public string MasterStyle { get => _masterStyle; set => Set(ref _masterStyle, value); }
    public string TextStorageMode { get => _textStorageMode; set => Set(ref _textStorageMode, value); }
    public string Status { get => _status; private set => Set(ref _status, value); }
    public string Diagnostics { get => _diagnostics; private set => Set(ref _diagnostics, value); }
    public bool CanConnect => _client is null && !_closing;
    public bool CanDisconnect => _client is not null && !_closing;
    public bool CanOpenWorkspace => _client is not null && _workspaceId is null && !_closing;
    public bool CanCreateOutput => _client is not null && _workspaceId is not null && !_closing;
    public bool CanOpenOutput => CanCreateOutput;

    public async Task ConnectAsync()
    {
        if (_client is not null || _closing)
        {
            return;
        }

        try
        {
            Status = "Connecting...";
            Diagnostics = string.Empty;
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            _client = await StdioMcpClient.StartAsync(McpExecutablePath, timeout.Token).ConfigureAwait(true);
            Status = $"Connected to CreationsForge MCP {_client.ServerVersion}.";
            Diagnostics = "MCP initialize completed. No workspace is open.";
        }
        catch (Exception exception)
        {
            _client = null;
            Status = "Disconnected";
            Diagnostics = FormatError("Connection failed", exception);
        }

        NotifyCommands();
    }

    public async Task OpenWorkspaceAsync()
    {
        if (_client is null || _closing)
        {
            return;
        }

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
            }).ConfigureAwait(true);
            _workspaceId = result["structuredContent"]?["workspaceId"]?.GetValue<string>()
                ?? result["workspaceId"]?.GetValue<string>();
            Status = _workspaceId is null ? "Workspace opened, but no session identifier was returned." : "Workspace session ready.";
            Diagnostics = result.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception exception)
        {
            Diagnostics = FormatError("Workspace open failed", exception);
        }

        NotifyCommands();
    }

    public async Task OpenOutputAsync(bool createNew)
    {
        if (_client is null || _workspaceId is null || _closing)
        {
            return;
        }

        try
        {
            ValidateFilePath(OutputPath, "output path");
            var tool = createNew ? "output_create" : "output_open";
            var result = await CallAsync(tool, new JsonObject
            {
                ["operationId"] = "workbench-output-" + Guid.NewGuid().ToString("N"),
                ["workspaceId"] = _workspaceId,
                ["outputPath"] = OutputPath,
                ["masterStyle"] = MasterStyle,
                ["textStorageMode"] = TextStorageMode,
                ["language"] = "English",
            }).ConfigureAwait(true);
            Status = createNew ? "New output attached to the workspace." : "Existing output attached to the workspace.";
            Diagnostics = result.ToJsonString(new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception exception)
        {
            Diagnostics = FormatError("Output operation failed", exception);
        }
    }

    public async Task CloseAsync()
    {
        if (_closing)
        {
            return;
        }

        _closing = true;
        NotifyCommands();
        try
        {
            if (_client is not null && _workspaceId is not null)
            {
                try
                {
                    await CallAsync("workspace_close", new JsonObject
                    {
                        ["operationId"] = "workbench-close-" + Guid.NewGuid().ToString("N"),
                        ["workspaceId"] = _workspaceId,
                        ["discardUnsaved"] = true,
                    }, TimeSpan.FromSeconds(5)).ConfigureAwait(true);
                }
                catch (Exception exception)
                {
                    Diagnostics = FormatError("Workspace close failed; the child process will still be stopped", exception);
                }
            }

            if (_client is not null)
            {
                await _client.DisposeAsync().ConfigureAwait(true);
            }
        }
        finally
        {
            _client = null;
            _workspaceId = null;
            _closing = false;
            Status = "Disconnected";
            NotifyCommands();
        }
    }

    private async Task<JsonObject> CallAsync(
        string tool,
        JsonObject arguments,
        TimeSpan? timeout = null)
    {
        using var cancellation = new CancellationTokenSource(timeout ?? TimeSpan.FromSeconds(30));
        return await _client!.CallAsync(tool, arguments, cancellation.Token).ConfigureAwait(true);
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

    private void NotifyCommands()
    {
        OnPropertyChanged(nameof(CanConnect));
        OnPropertyChanged(nameof(CanDisconnect));
        OnPropertyChanged(nameof(CanOpenWorkspace));
        OnPropertyChanged(nameof(CanCreateOutput));
        OnPropertyChanged(nameof(CanOpenOutput));
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
