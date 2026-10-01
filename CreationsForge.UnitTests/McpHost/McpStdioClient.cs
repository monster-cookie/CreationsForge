using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>Speaks newline-delimited MCP to one production host process.</summary>
internal sealed class McpStdioClient : IAsyncDisposable
{
    private readonly Process _process;
    private readonly Task<string> _standardError;
    private readonly SemaphoreSlim _write = new(1, 1);
    private readonly HashSet<int> _ignoredResponseIds = [];
    private int _nextId;
    private bool _disposed;

    private McpStdioClient(Process process)
    {
        _process = process;
        _standardError = process.StandardError.ReadToEndAsync();
    }

    public string ProtocolVersion { get; private set; } = string.Empty;

    public string ServerName { get; private set; } = string.Empty;

    public string ServerVersion { get; private set; } = string.Empty;

    public static async Task<McpStdioClient> StartAsync(CancellationToken cancellationToken)
    {
        var assemblyPath = McpHostPaths.McpAssembly();
        Assert.True(File.Exists(assemblyPath), $"The MCP build output was not found at '{assemblyPath}'.");
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = McpHostPaths.RepositoryRoot(),
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
        process.StartInfo.ArgumentList.Add(assemblyPath);
        if (!process.Start())
        {
            process.Dispose();
            throw new InvalidOperationException("The production MCP process did not start.");
        }

        var client = new McpStdioClient(process);
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

    public async Task<McpCallResult> CallAsync(
        string tool,
        JsonObject arguments,
        CancellationToken cancellationToken)
    {
        var id = NextId();
        await SendRequestAsync(id, "tools/call", ToolCall(tool, arguments), cancellationToken).ConfigureAwait(false);
        return McpCallResult.FromRpc(await ReadResponseAsync(id, cancellationToken).ConfigureAwait(false));
    }

    public async Task<McpCallResult?> CallAfterCancellationAsync(
        string tool,
        JsonObject arguments,
        CancellationToken cancellationToken)
    {
        var id = NextId();
        await NotifyAsync(
            "notifications/cancelled",
            new JsonObject
            {
                ["requestId"] = id,
                ["reason"] = "client canceled",
            },
            cancellationToken).ConfigureAwait(false);
        await SendRequestAsync(id, "tools/call", ToolCall(tool, arguments), cancellationToken).ConfigureAwait(false);
        using var bounded = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        bounded.CancelAfter(TimeSpan.FromSeconds(3));
        try
        {
            return McpCallResult.FromRpc(await ReadResponseAsync(id, bounded.Token).ConfigureAwait(false));
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _ignoredResponseIds.Add(id);
            return null;
        }
    }

    public async Task<JsonObject> RequestAsync(
        string method,
        JsonObject parameters,
        CancellationToken cancellationToken)
    {
        var id = NextId();
        await SendRequestAsync(id, method, parameters, cancellationToken).ConfigureAwait(false);
        var response = await ReadResponseAsync(id, cancellationToken).ConfigureAwait(false);
        if (response["error"] is JsonObject error)
        {
            throw new InvalidOperationException($"MCP {method} failed: {error.ToJsonString()}");
        }

        return response["result"] as JsonObject
            ?? throw new InvalidOperationException($"MCP {method} returned no result object: {response.ToJsonString()}");
    }

    public async Task<string> ShutdownAsync(CancellationToken cancellationToken)
    {
        if (!_process.HasExited)
        {
            _process.StandardInput.Close();
            await _process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }

        var standardError = await _standardError.WaitAsync(cancellationToken).ConfigureAwait(false);
        Assert.True(
            _process.ExitCode == 0,
            $"The MCP host exited {_process.ExitCode}. {standardError}");
        Assert.DoesNotContain("Unhandled exception", standardError, StringComparison.OrdinalIgnoreCase);
        return standardError;
    }

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
                _process.Kill(entireProcessTree: true);
                await _process.WaitForExitAsync().ConfigureAwait(false);
            }
        }
        catch (InvalidOperationException)
        {
        }

        try
        {
            await _standardError.WaitAsync(TimeSpan.FromSeconds(2)).ConfigureAwait(false);
        }
        catch (Exception)
        {
        }

        _process.Dispose();
        _write.Dispose();
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var id = NextId();
        await SendRequestAsync(
            id,
            "initialize",
            new JsonObject
            {
                ["protocolVersion"] = "2025-06-18",
                ["capabilities"] = new JsonObject(),
                ["clientInfo"] = new JsonObject
                {
                    ["name"] = "CreationsForge.UnitTests",
                    ["version"] = "1.0.0",
                },
            },
            cancellationToken).ConfigureAwait(false);
        var response = await ReadResponseAsync(id, cancellationToken).ConfigureAwait(false);
        if (response["error"] is JsonObject error)
        {
            throw new InvalidOperationException($"MCP initialize failed: {error.ToJsonString()}");
        }

        var result = response["result"] as JsonObject
            ?? throw new InvalidOperationException($"MCP initialize returned no result: {response.ToJsonString()}");
        ProtocolVersion = result["protocolVersion"]?.GetValue<string>() ?? string.Empty;
        var serverInfo = result["serverInfo"] as JsonObject
            ?? throw new InvalidOperationException($"MCP initialize omitted serverInfo: {response.ToJsonString()}");
        ServerName = serverInfo["name"]?.GetValue<string>() ?? string.Empty;
        ServerVersion = serverInfo["version"]?.GetValue<string>() ?? string.Empty;
        await NotifyAsync("notifications/initialized", null, cancellationToken).ConfigureAwait(false);
    }

    private int NextId()
    {
        return Interlocked.Increment(ref _nextId);
    }

    private async Task SendRequestAsync(
        int id,
        string method,
        JsonObject parameters,
        CancellationToken cancellationToken)
    {
        await SendAsync(
            new JsonObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id,
                ["method"] = method,
                ["params"] = parameters,
            },
            cancellationToken).ConfigureAwait(false);
    }

    private async Task NotifyAsync(string method, JsonObject? parameters, CancellationToken cancellationToken)
    {
        var message = new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["method"] = method,
        };
        if (parameters is not null)
        {
            message["params"] = parameters;
        }

        await SendAsync(message, cancellationToken).ConfigureAwait(false);
    }

    private async Task SendAsync(JsonObject message, CancellationToken cancellationToken)
    {
        var line = message.ToJsonString();
        await _write.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await _process.StandardInput.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
            await _process.StandardInput.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _write.Release();
        }
    }

    private async Task<JsonObject> ReadResponseAsync(int id, CancellationToken cancellationToken)
    {
        while (true)
        {
            var line = await _process.StandardOutput.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null)
            {
                var standardError = _process.HasExited ? await _standardError.ConfigureAwait(false) : string.Empty;
                var exit = _process.HasExited ? _process.ExitCode.ToString() : "running";
                throw new InvalidOperationException(
                    $"The MCP host closed stdout before response {id}. Exit: {exit}. {standardError}");
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            JsonNode? parsed;
            try
            {
                parsed = JsonNode.Parse(line);
            }
            catch (JsonException exception)
            {
                throw new InvalidOperationException($"The MCP host wrote non-JSON stdout: {line}", exception);
            }

            if (parsed is not JsonObject message)
            {
                throw new InvalidOperationException($"The MCP host wrote a non-object stdout line: {line}");
            }

            if (!message.ContainsKey("id"))
            {
                continue;
            }

            if (!IdEquals(message, id))
            {
                if (TryGetId(message, out var otherId) && _ignoredResponseIds.Contains(otherId))
                {
                    continue;
                }

                throw new InvalidOperationException($"Unexpected MCP response id. Expected {id}. {line}");
            }

            return message;
        }
    }

    private static JsonObject ToolCall(string tool, JsonObject arguments)
    {
        return new JsonObject
        {
            ["name"] = tool,
            ["arguments"] = arguments.DeepClone(),
        };
    }

    private static bool IdEquals(JsonObject message, int id)
    {
        return TryGetId(message, out var parsed) && parsed == id;
    }

    private static bool TryGetId(JsonObject message, out int id)
    {
        id = 0;
        if (message["id"] is not JsonValue value)
        {
            return false;
        }

        if (value.TryGetValue<int>(out id))
        {
            return true;
        }

        if (value.TryGetValue<long>(out var wide) && wide is >= int.MinValue and <= int.MaxValue)
        {
            id = (int)wide;
            return true;
        }

        return false;
    }
}
