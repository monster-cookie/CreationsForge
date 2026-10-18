using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Serilog;

namespace CreationsForge.Workbench;

/// <summary>Owns one production MCP child process and speaks its newline-delimited JSON-RPC transport.</summary>
internal sealed class StdioMcpClient : IWorkbenchMcpSession
{
    /// <summary>The only protocol version this Workbench negotiates.</summary>
    internal const string SupportedProtocol = "2025-06-18";

    private readonly Process _process;
    private readonly StringBuilder _standardError = new();
    private readonly Task _standardErrorPump;
    private readonly SemaphoreSlim _transaction = new(1, 1);
    private readonly object _receipts = new();
    private readonly List<WorkbenchMcpTool> _tools = [];
    private int _nextId;
    private int _disposeState;
    private string? _workspaceId;
    private ulong? _revision;

    private StdioMcpClient(Process process)
    {
        _process = process;
        _standardErrorPump = PumpStandardErrorAsync();
    }

    /// <summary>Gets the process id of the most recently started child, including one that failed initialize.</summary>
    internal static int LastProcessId { get; private set; }

    /// <summary>Gets the child process id owned by this session.</summary>
    internal int ProcessId => _process.Id;

    /// <inheritdoc />
    public string ProtocolVersion { get; private set; } = string.Empty;

    /// <inheritdoc />
    public string ServerName { get; private set; } = string.Empty;

    /// <inheritdoc />
    public string ServerVersion { get; private set; } = string.Empty;

    /// <inheritdoc />
    public JsonObject Capabilities { get; private set; } = new();

    /// <inheritdoc />
    public IReadOnlyList<WorkbenchMcpTool> Tools => _tools;

    /// <inheritdoc />
    public string? WorkspaceId
    {
        get
        {
            lock (_receipts)
            {
                return _workspaceId;
            }
        }
    }

    /// <inheritdoc />
    public ulong? Revision
    {
        get
        {
            lock (_receipts)
            {
                return _revision;
            }
        }
    }

    /// <inheritdoc />
    public bool HasExited
    {
        get
        {
            try
            {
                return _process.HasExited;
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }
    }

    /// <inheritdoc />
    public event EventHandler? HostExited;

    /// <summary>Gets the retained MCP stderr tail. Protocol frames are not included.</summary>
    internal string StandardError => StandardErrorSnapshot();

    /// <summary>Starts the production MCP executable, negotiates initialize, and lists tools.</summary>
    /// <param name="executablePath">A <c>.dll</c> launched with <c>dotnet</c>, or a native apphost path.</param>
    /// <param name="cancellationToken">Cancels startup. The child is stopped before this method throws.</param>
    /// <returns>The initialized session.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="executablePath"/> is blank.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the executable path does not exist.</exception>
    /// <exception cref="WorkbenchMcpException">Thrown when the process or initialize handshake fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the process or initialize handshake fails.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static Task<StdioMcpClient> StartAsync(string executablePath, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);
        var fullPath = Path.GetFullPath(executablePath);
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"The MCP executable was not found: {fullPath}", fullPath);
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = string.Equals(Path.GetExtension(fullPath), ".dll", StringComparison.OrdinalIgnoreCase) ? "dotnet" : fullPath,
            WorkingDirectory = Path.GetDirectoryName(fullPath) ?? Environment.CurrentDirectory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        if (string.Equals(Path.GetExtension(fullPath), ".dll", StringComparison.OrdinalIgnoreCase))
        {
            startInfo.ArgumentList.Add(fullPath);
        }

        return StartAsync(startInfo, cancellationToken);
    }

    /// <summary>Starts a child from prepared process settings, negotiates initialize, and lists tools.</summary>
    /// <param name="startInfo">The redirected process settings. Standard input, output, and error must already be redirected.</param>
    /// <param name="cancellationToken">Cancels startup. The child is stopped before this method throws.</param>
    /// <returns>The initialized session.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="startInfo"/> is <see langword="null"/>.</exception>
    /// <exception cref="WorkbenchMcpException">Thrown when the process or initialize handshake fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the process settings cannot host the protocol.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    internal static async Task<StdioMcpClient> StartAsync(ProcessStartInfo startInfo, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(startInfo);
        if (!startInfo.RedirectStandardInput || !startInfo.RedirectStandardOutput || !startInfo.RedirectStandardError)
        {
            throw new InvalidOperationException("The MCP process must redirect standard input, output, and error.");
        }

        var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        Log.ForContext<StdioMcpClient>().Information(
            "Starting MCP child {FileName} {Arguments} from {WorkingDirectory}.",
            startInfo.FileName,
            string.Join(' ', startInfo.ArgumentList),
            startInfo.WorkingDirectory);
        try
        {
            if (!process.Start())
            {
                process.Dispose();
                throw new WorkbenchMcpException(WorkbenchMcpFailureKind.Transport, "The MCP process did not start.");
            }
        }
        catch (Exception exception) when (exception is not WorkbenchMcpException)
        {
            process.Dispose();
            throw new WorkbenchMcpException(
                WorkbenchMcpFailureKind.Transport,
                "The MCP process did not start: " + exception.Message,
                innerException: exception);
        }

        LastProcessId = process.Id;
        Log.ForContext<StdioMcpClient>().Information("MCP child started with process id {ProcessId}.", process.Id);
        var client = new StdioMcpClient(process);
        process.Exited += client.OnProcessExited;
        try
        {
            if (process.HasExited)
            {
                client.OnProcessExited(process, EventArgs.Empty);
            }

            await client.InitializeAsync(cancellationToken).ConfigureAwait(false);
            return client;
        }
        catch
        {
            await client.DisposeAsync().ConfigureAwait(false);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<JsonObject> CallAsync(string tool, JsonObject arguments, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposeState != 0, this);
        var acquired = false;
        try
        {
            await _transaction.WaitAsync(cancellationToken).ConfigureAwait(false);
            acquired = true;
            var argumentsToSend = arguments.DeepClone().AsObject();
            JsonObject? schema;
            string? workspaceId;
            ulong? revision;
            lock (_receipts)
            {
                schema = FindSchema(tool);
                workspaceId = _workspaceId;
                revision = _revision;
            }

            WorkbenchSessionReceipts.Apply(argumentsToSend, schema, workspaceId, revision);
            var result = await SendRequestAsync(
                "tools/call",
                new JsonObject
                {
                    ["name"] = tool,
                    ["arguments"] = argumentsToSend,
                },
                cancellationToken).ConfigureAwait(false);
            var toolResult = RequireToolResult(tool, result);
            lock (_receipts)
            {
                WorkbenchSessionReceipts.Capture(tool, toolResult, ref _workspaceId, ref _revision);
            }

            return toolResult;
        }
        finally
        {
            if (acquired)
            {
                _transaction.Release();
            }
        }
    }

    /// <summary>Closes stdin, waits briefly, and then stops any remaining child process.</summary>
    /// <returns>A completed task after the child has exited and its handles are released.</returns>
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposeState, 1) != 0)
        {
            return;
        }

        _process.Exited -= OnProcessExited;
        try
        {
            if (!_process.HasExited)
            {
                _process.StandardInput.Close();
                await _process.WaitForExitAsync().WaitAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false);
            }
        }
        catch (Exception) when (!_process.HasExited)
        {
            _process.Kill(entireProcessTree: true);
            await _process.WaitForExitAsync().ConfigureAwait(false);
        }
        finally
        {
            try
            {
                await _standardErrorPump.WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Log.ForContext<StdioMcpClient>().Warning(exception, "MCP stderr was not fully read before the child was released.");
            }

            if (_process.HasExited)
            {
                Log.ForContext<StdioMcpClient>().Information(
                    "MCP child {ProcessId} exited with code {ExitCode}.",
                    _process.Id,
                    _process.ExitCode);
            }

            _process.Dispose();
            _transaction.Dispose();
        }
    }

    /// <summary>Reports an unexpected child exit to the session owner.</summary>
    /// <param name="sender">The child process.</param>
    /// <param name="eventArgs">The exit arguments. They are unused.</param>
    private void OnProcessExited(object? sender, EventArgs eventArgs)
    {
        if (_disposeState != 0)
        {
            return;
        }

        Log.ForContext<StdioMcpClient>().Warning("MCP child {ProcessId} exited before the Workbench released it.", _process.Id);
        HostExited?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Copies MCP host stderr into the Workbench log as each line arrives.</summary>
    /// <returns>A task that completes when stderr closes or the read fails.</returns>
    private async Task PumpStandardErrorAsync()
    {
        try
        {
            while (true)
            {
                var line = await _process.StandardError.ReadLineAsync().ConfigureAwait(false);
                if (line is null)
                {
                    return;
                }

                if (line.Length == 0)
                {
                    continue;
                }

                lock (_standardError)
                {
                    if (_standardError.Length > 0)
                    {
                        _standardError.AppendLine();
                    }

                    var remaining = 8192 - _standardError.Length;
                    if (remaining > 0)
                    {
                        _standardError.Append(line.Length <= remaining ? line : line[..remaining]);
                    }
                }

                Log.ForContext<StdioMcpClient>().Information("MCP host: {StandardErrorLine}", line);
            }
        }
        catch (Exception exception)
        {
            Log.ForContext<StdioMcpClient>().Warning(exception, "Stopped reading stderr for MCP process {ProcessId}.", _process.Id);
        }
    }

    /// <summary>Returns the retained tail of MCP stderr for an exception message.</summary>
    /// <returns>At most the first 8192 characters written to stderr.</returns>
    private string StandardErrorSnapshot()
    {
        lock (_standardError)
        {
            return _standardError.ToString();
        }
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var acquired = false;
        try
        {
            await _transaction.WaitAsync(cancellationToken).ConfigureAwait(false);
            acquired = true;
            var result = await SendRequestAsync(
                "initialize",
                new JsonObject
                {
                    ["protocolVersion"] = SupportedProtocol,
                    ["capabilities"] = new JsonObject(),
                    ["clientInfo"] = new JsonObject
                    {
                        ["name"] = "CreationsForge.Workbench",
                        ["version"] = "1.0.0",
                    },
                },
                cancellationToken).ConfigureAwait(false);
            var protocolVersion = ReadString(result["protocolVersion"]);
            if (!string.Equals(protocolVersion, SupportedProtocol, StringComparison.Ordinal))
            {
                throw new WorkbenchMcpException(
                    WorkbenchMcpFailureKind.Protocol,
                    $"MCP initialize negotiated protocol '{protocolVersion ?? "none"}', which this Workbench does not support. The supported protocol is {SupportedProtocol}.");
            }

            var serverInfo = result["serverInfo"] as JsonObject
                ?? throw new WorkbenchMcpException(WorkbenchMcpFailureKind.Protocol, "MCP initialize did not return serverInfo.");
            var serverName = ReadString(serverInfo["name"]);
            if (string.IsNullOrWhiteSpace(serverName))
            {
                throw new WorkbenchMcpException(WorkbenchMcpFailureKind.Protocol, "MCP initialize did not return a server name.");
            }

            if (result["capabilities"] is not JsonObject capabilities || capabilities["tools"] is not JsonObject)
            {
                throw new WorkbenchMcpException(
                    WorkbenchMcpFailureKind.Protocol,
                    "MCP initialize did not advertise the tools capability.");
            }

            ProtocolVersion = SupportedProtocol;
            ServerName = serverName;
            ServerVersion = ReadString(serverInfo["version"]) ?? "unknown";
            Capabilities = capabilities;
            await WriteLineAsync(
                new JsonObject
                {
                    ["jsonrpc"] = "2.0",
                    ["method"] = "notifications/initialized",
                },
                cancellationToken).ConfigureAwait(false);
            var tools = await ListToolsAsync(cancellationToken).ConfigureAwait(false);
            _tools.AddRange(tools);
            Log.ForContext<StdioMcpClient>().Information(
                "MCP initialize negotiated protocol {ProtocolVersion} with {ServerName} {ServerVersion} and listed {ToolCount} tools.",
                ProtocolVersion,
                ServerName,
                ServerVersion,
                _tools.Count);
        }
        finally
        {
            if (acquired)
            {
                _transaction.Release();
            }
        }
    }

    private async Task<IReadOnlyList<WorkbenchMcpTool>> ListToolsAsync(CancellationToken cancellationToken)
    {
        var tools = new List<WorkbenchMcpTool>();
        var names = new HashSet<string>(StringComparer.Ordinal);
        string? cursor = null;
        for (var page = 0; page < 20; page++)
        {
            var parameters = new JsonObject();
            if (cursor is not null)
            {
                parameters["cursor"] = cursor;
            }

            var result = await SendRequestAsync("tools/list", parameters, cancellationToken).ConfigureAwait(false);
            if (result["tools"] is not JsonArray listed)
            {
                throw new WorkbenchMcpException(WorkbenchMcpFailureKind.Protocol, "MCP tools/list did not return a tools array.");
            }

            foreach (var item in listed)
            {
                if (item is not JsonObject tool)
                {
                    throw new WorkbenchMcpException(WorkbenchMcpFailureKind.Protocol, "MCP tools/list returned a tool that was not an object.");
                }

                var name = ReadString(tool["name"]);
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new WorkbenchMcpException(WorkbenchMcpFailureKind.Protocol, "MCP tools/list returned a tool without a name.");
                }

                if (!names.Add(name))
                {
                    throw new WorkbenchMcpException(WorkbenchMcpFailureKind.Protocol, $"MCP tools/list returned '{name}' more than once.");
                }

                if (tool["inputSchema"] is not JsonObject schema)
                {
                    throw new WorkbenchMcpException(WorkbenchMcpFailureKind.Protocol, $"MCP tools/list did not return an input schema for '{name}'.");
                }

                tools.Add(new WorkbenchMcpTool(name, ReadString(tool["description"]) ?? string.Empty, schema));
            }

            cursor = ReadString(result["nextCursor"]);
            if (string.IsNullOrWhiteSpace(cursor))
            {
                return tools;
            }
        }

        throw new WorkbenchMcpException(WorkbenchMcpFailureKind.Protocol, "MCP tools/list did not finish.");
    }

    private async Task<JsonObject> SendRequestAsync(string method, JsonObject parameters, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _nextId);
        var sent = false;
        try
        {
            await WriteLineAsync(new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id,
                ["method"] = method,
                ["params"] = parameters,
            }, cancellationToken).ConfigureAwait(false);
            sent = true;
            var response = await ReadResponseAsync(id, cancellationToken).ConfigureAwait(false);
            if (response["error"] is JsonObject error)
            {
                throw new WorkbenchMcpException(
                    WorkbenchMcpFailureKind.Protocol,
                    $"MCP {method} failed: {error.ToJsonString()}",
                    ReadErrorCode(error));
            }

            return response["result"] as JsonObject
                ?? throw new WorkbenchMcpException(WorkbenchMcpFailureKind.Protocol, $"MCP {method} returned no result.");
        }
        catch (OperationCanceledException) when (sent)
        {
            await TryNotifyCancelledAsync(id).ConfigureAwait(false);
            throw;
        }
    }

    private async Task TryNotifyCancelledAsync(int id)
    {
        if (_disposeState != 0 || HasExited)
        {
            return;
        }

        try
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            await WriteLineAsync(new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["method"] = "notifications/cancelled",
                ["params"] = new JsonObject
                {
                    ["requestId"] = id,
                    ["reason"] = "client canceled",
                },
            }, timeout.Token).ConfigureAwait(false);
        }
        catch (Exception)
        {
        }
    }

    private async Task WriteLineAsync(JsonObject message, CancellationToken cancellationToken)
    {
        var line = message.ToJsonString();
        await _process.StandardInput.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
        await _process.StandardInput.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task<JsonObject> ReadResponseAsync(int id, CancellationToken cancellationToken)
    {
        while (true)
        {
            string? line;
            try
            {
                line = await _process.StandardOutput.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new WorkbenchMcpException(
                    WorkbenchMcpFailureKind.Transport,
                    $"The MCP process stopped sending response {id}. {StandardErrorSnapshot()}",
                    innerException: exception);
            }

            if (line is null)
            {
                if (_process.HasExited)
                {
                    try
                    {
                        await _standardErrorPump.WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
                    }
                    catch (Exception exception)
                    {
                        Log.ForContext<StdioMcpClient>().Warning(exception, "MCP stderr was not fully read after stdout closed.");
                    }
                }

                var diagnostics = StandardErrorSnapshot();
                Log.ForContext<StdioMcpClient>().Error(
                    "MCP process {ProcessId} closed stdout before response {RequestId}. {StandardError}",
                    _process.Id,
                    id,
                    diagnostics);
                throw new WorkbenchMcpException(
                    WorkbenchMcpFailureKind.Transport,
                    $"The MCP process closed stdout before response {id}. {diagnostics}");
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            JsonObject message;
            try
            {
                message = JsonNode.Parse(line)?.AsObject()
                    ?? throw new JsonException("The response was not a JSON object.");
            }
            catch (JsonException exception)
            {
                throw new WorkbenchMcpException(
                    WorkbenchMcpFailureKind.Transport,
                    $"The MCP process wrote invalid JSON: {line}",
                    innerException: exception);
            }

            if (!message.ContainsKey("id"))
            {
                continue;
            }

            if (MatchesId(message, id))
            {
                return message;
            }
        }
    }

    private JsonObject? FindSchema(string tool)
    {
        foreach (var candidate in _tools)
        {
            if (string.Equals(candidate.Name, tool, StringComparison.Ordinal))
            {
                return candidate.InputSchema;
            }
        }

        return null;
    }

    private static JsonObject RequireToolResult(string tool, JsonObject result)
    {
        if (!IsToolError(result))
        {
            return result;
        }

        throw new WorkbenchMcpException(
            WorkbenchMcpFailureKind.Tool,
            $"MCP {tool} failed: {ToolErrorMessage(result)}",
            ReadString(result["structuredContent"]?["code"]));
    }

    private static bool IsToolError(JsonObject result)
    {
        return result["isError"] is JsonValue flag
            && flag.TryGetValue<bool>(out var isError)
            && isError;
    }

    private static string ToolErrorMessage(JsonObject result)
    {
        var structured = ReadString(result["structuredContent"]?["message"]);
        if (!string.IsNullOrWhiteSpace(structured))
        {
            return structured;
        }

        var text = ReadString(result["content"]?.AsArray().FirstOrDefault()?["text"]);
        if (!string.IsNullOrWhiteSpace(text))
        {
            return text;
        }

        return result.ToJsonString();
    }

    private static string? ReadErrorCode(JsonObject error)
    {
        if (error["code"] is not JsonValue value)
        {
            return null;
        }

        if (value.TryGetValue<int>(out var number))
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }

        if (value.TryGetValue<long>(out var wide))
        {
            return wide.ToString(CultureInfo.InvariantCulture);
        }

        return value.TryGetValue<string>(out var text) ? text : null;
    }

    private static bool MatchesId(JsonObject message, int id)
    {
        if (message["id"] is not JsonValue value)
        {
            return false;
        }

        if (value.TryGetValue<int>(out var asInt))
        {
            return asInt == id;
        }

        return value.TryGetValue<long>(out var asLong) && asLong == id;
    }

    private static string? ReadString(JsonNode? node)
    {
        return node is JsonValue value && value.TryGetValue<string>(out var text) ? text : null;
    }
}
