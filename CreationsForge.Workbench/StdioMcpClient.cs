using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CreationsForge.Workbench;

/// <summary>Owns one production MCP child process and speaks its newline-delimited JSON-RPC transport.</summary>
internal sealed class StdioMcpClient : IAsyncDisposable
{
    private readonly Process _process;
    private readonly Task<string> _standardError;
    private readonly SemaphoreSlim _writeGate = new(1, 1);
    private int _nextId;
    private bool _disposed;

    private StdioMcpClient(Process process)
    {
        _process = process;
        _standardError = process.StandardError.ReadToEndAsync();
    }

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
        if (!process.Start())
        {
            process.Dispose();
            throw new InvalidOperationException("The MCP process did not start.");
        }

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

    public string ServerVersion { get; private set; } = string.Empty;

    public async Task<JsonObject> CallAsync(string tool, JsonObject arguments, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _nextId);
        await SendAsync(new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id,
            ["method"] = "tools/call",
            ["params"] = new JsonObject { ["name"] = tool, ["arguments"] = arguments.DeepClone() },
        }, cancellationToken).ConfigureAwait(false);
        var response = await ReadResponseAsync(id, cancellationToken).ConfigureAwait(false);
        if (response["error"] is JsonObject error)
        {
            throw new InvalidOperationException($"MCP {tool} failed: {error.ToJsonString()}");
        }

        return response["result"] as JsonObject
            ?? throw new InvalidOperationException($"MCP {tool} returned no result: {response.ToJsonString()}");
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
            _writeGate.Dispose();
        }
    }

    private async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _nextId);
        await SendAsync(new JsonObject
        {
            ["jsonrpc"] = "2.0",
            ["id"] = id,
            ["method"] = "initialize",
            ["params"] = new JsonObject
            {
                ["protocolVersion"] = "2025-06-18",
                ["capabilities"] = new JsonObject(),
                ["clientInfo"] = new JsonObject { ["name"] = "CreationsForge.Workbench", ["version"] = "1.0.0" },
            },
        }, cancellationToken).ConfigureAwait(false);
        var response = await ReadResponseAsync(id, cancellationToken).ConfigureAwait(false);
        if (response["error"] is JsonObject error)
        {
            throw new InvalidOperationException($"MCP initialize failed: {error.ToJsonString()}");
        }

        var result = response["result"] as JsonObject
            ?? throw new InvalidOperationException("MCP initialize returned no result.");
        ServerVersion = result["serverInfo"]?["version"]?.GetValue<string>() ?? "unknown";
        await SendAsync(new JsonObject { ["jsonrpc"] = "2.0", ["method"] = "notifications/initialized" }, cancellationToken).ConfigureAwait(false);
    }

    private async Task SendAsync(JsonObject message, CancellationToken cancellationToken)
    {
        await _writeGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var line = message.ToJsonString();
            await _process.StandardInput.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
            await _process.StandardInput.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _writeGate.Release();
        }
    }

    private async Task<JsonObject> ReadResponseAsync(int id, CancellationToken cancellationToken)
    {
        while (true)
        {
            var line = await _process.StandardOutput.ReadLineAsync(cancellationToken).ConfigureAwait(false);
            if (line is null)
            {
                var diagnostics = await _standardError.ConfigureAwait(false);
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

            if (message["id"]?.GetValue<int>() == id)
            {
                return message;
            }
        }
    }
}
