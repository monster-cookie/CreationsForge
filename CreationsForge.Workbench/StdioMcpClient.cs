using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CreationsForge.Workbench;

/// <summary>Owns one production MCP child process and speaks its newline-delimited JSON-RPC transport.</summary>
internal sealed class StdioMcpClient : IWorkbenchMcpSession
{
    private readonly Process _process;
    private readonly Task<string> _standardError;
    private readonly SemaphoreSlim _transaction = new(1, 1);
    private int _nextId;
    private bool _disposed;

    private StdioMcpClient(Process process)
    {
        _process = process;
        _standardError = process.StandardError.ReadToEndAsync();
    }

    /// <summary>Gets the process id of the most recently started child, including one that failed initialize.</summary>
    internal static int LastProcessId { get; private set; }

    /// <summary>Gets the child process id owned by this session.</summary>
    internal int ProcessId => _process.Id;

    /// <inheritdoc />
    public string ServerVersion { get; private set; } = string.Empty;

    /// <summary>Starts the production MCP executable and completes initialize.</summary>
    /// <param name="executablePath">A <c>.dll</c> launched with <c>dotnet</c>, or a native apphost path.</param>
    /// <param name="cancellationToken">Cancels startup. The child is stopped before this method throws.</param>
    /// <returns>The initialized session.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="executablePath"/> is blank.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the executable path does not exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the process or initialize handshake fails.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public static async Task<StdioMcpClient> StartAsync(string executablePath, CancellationToken cancellationToken)
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

        var process = new Process { StartInfo = startInfo };
        try
        {
            if (!process.Start())
            {
                throw new InvalidOperationException("The MCP process did not start.");
            }
        }
        catch
        {
            process.Dispose();
            throw;
        }

        LastProcessId = process.Id;
        var client = new StdioMcpClient(process);
        try
        {
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
        ObjectDisposedException.ThrowIf(_disposed, this);
        var acquired = false;
        var id = 0;
        var sent = false;
        try
        {
            await _transaction.WaitAsync(cancellationToken).ConfigureAwait(false);
            acquired = true;
            id = Interlocked.Increment(ref _nextId);
            await WriteLineAsync(new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id,
                ["method"] = "tools/call",
                ["params"] = new JsonObject
                {
                    ["name"] = tool,
                    ["arguments"] = arguments.DeepClone(),
                },
            }, cancellationToken).ConfigureAwait(false);
            sent = true;
            var response = await ReadResponseAsync(id, cancellationToken).ConfigureAwait(false);
            return RequireToolResult(tool, response);
        }
        catch (OperationCanceledException) when (sent)
        {
            await TryNotifyCancelledAsync(id).ConfigureAwait(false);
            throw;
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
        if (_disposed)
        {
            return;
        }

        _disposed = true;
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
            _process.Dispose();
            _transaction.Dispose();
        }
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var acquired = false;
        var id = 0;
        var sent = false;
        try
        {
            await _transaction.WaitAsync(cancellationToken).ConfigureAwait(false);
            acquired = true;
            id = Interlocked.Increment(ref _nextId);
            await WriteLineAsync(new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id,
                ["method"] = "initialize",
                ["params"] = new JsonObject
                {
                    ["protocolVersion"] = "2025-06-18",
                    ["capabilities"] = new JsonObject(),
                    ["clientInfo"] = new JsonObject
                    {
                        ["name"] = "CreationsForge.Workbench",
                        ["version"] = "1.0.0",
                    },
                },
            }, cancellationToken).ConfigureAwait(false);
            sent = true;
            var response = await ReadResponseAsync(id, cancellationToken).ConfigureAwait(false);
            if (response["error"] is JsonObject error)
            {
                throw new InvalidOperationException($"MCP initialize failed: {error.ToJsonString()}");
            }

            var result = response["result"] as JsonObject
                ?? throw new InvalidOperationException("MCP initialize returned no result.");
            ServerVersion = ReadString(result["serverInfo"]?["version"]) ?? "unknown";
            await WriteLineAsync(
                new JsonObject
                {
                    ["jsonrpc"] = "2.0",
                    ["method"] = "notifications/initialized",
                },
                cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (sent)
        {
            await TryNotifyCancelledAsync(id).ConfigureAwait(false);
            throw;
        }
        finally
        {
            if (acquired)
            {
                _transaction.Release();
            }
        }
    }

    private async Task TryNotifyCancelledAsync(int id)
    {
        if (_disposed || _process.HasExited)
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
            var line = await _process.StandardOutput.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null)
            {
                var diagnostics = _process.HasExited ? await _standardError.ConfigureAwait(false) : string.Empty;
                throw new InvalidOperationException($"The MCP process closed stdout before response {id}. {diagnostics}");
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
                throw new InvalidOperationException($"The MCP process wrote invalid JSON: {line}", exception);
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

    private static JsonObject RequireToolResult(string tool, JsonObject response)
    {
        if (response["error"] is JsonObject error)
        {
            throw new InvalidOperationException($"MCP {tool} failed: {error.ToJsonString()}");
        }

        var result = response["result"] as JsonObject
            ?? throw new InvalidOperationException($"MCP {tool} returned no result: {response.ToJsonString()}");
        if (IsToolError(result))
        {
            throw new InvalidOperationException($"MCP {tool} failed: {ToolErrorMessage(result)}");
        }

        return result;
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
