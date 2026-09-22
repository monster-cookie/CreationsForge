using System.Diagnostics;
using System.Text.Json;
using CreationsForge.Mcp;

namespace CreationsForge.UnitTests.McpHost;

/// <summary>
/// Exercises the compiled production host across its physical standard streams.
/// </summary>
public sealed class McpHostProtocolTests
{
    /// <summary>Requires graceful host shutdown after cancellation to report the documented cancelled-process exit code.</summary>
    [Fact]
    public async Task HostRunReturns130WhenCancellationCompletesGracefully()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        using var shutdown = new CancellationTokenSource();
        var started = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        var runTask = McpHostRunner.RunHostAsync(
            async cancellationToken =>
            {
                started.SetResult(true);
                try
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                }
            },
            shutdown.Token);
        await started.Task.WaitAsync(timeout.Token);
        await shutdown.CancelAsync();

        var exitCode = await runTask.WaitAsync(timeout.Token);
        Assert.Equal(130, exitCode);
    }

    /// <summary>Initializes the real stdio process, verifies its server identity, and requires clean EOF shutdown without non-protocol stdout.</summary>
    [Fact]
    public async Task ProductionHostInitializesWithProtocolOnlyStandardOutput()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(20));
        var repositoryRoot = FindRepositoryRoot();
        var configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent?.Name
            ?? throw new InvalidOperationException("Could not determine the active build configuration.");
        var mcpAssemblyPath = Path.Combine(
            repositoryRoot,
            "CreationsForge.Mcp",
            "bin",
            configuration,
            "net10.0",
            "CreationsForge.Mcp.dll");
        Assert.True(File.Exists(mcpAssemblyPath), $"The MCP build output was not found at '{mcpAssemblyPath}'.");

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                WorkingDirectory = repositoryRoot,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
        process.StartInfo.ArgumentList.Add(mcpAssemblyPath);
        Assert.True(process.Start(), "The production MCP process did not start.");

        var standardErrorTask = process.StandardError.ReadToEndAsync(timeout.Token);
        try
        {
            const string initializeRequest = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"initialize\",\"params\":{\"protocolVersion\":\"2025-06-18\",\"capabilities\":{},\"clientInfo\":{\"name\":\"CreationsForge.UnitTests\",\"version\":\"1.0.0\"}}}";
            await process.StandardInput.WriteLineAsync(initializeRequest.AsMemory(), timeout.Token);
            await process.StandardInput.FlushAsync(timeout.Token);

            var responseLine = await process.StandardOutput.ReadLineAsync(timeout.Token);
            Assert.False(string.IsNullOrWhiteSpace(responseLine));
            using var response = JsonDocument.Parse(responseLine);
            var root = response.RootElement;
            Assert.Equal("2.0", root.GetProperty("jsonrpc").GetString());
            Assert.Equal(1, root.GetProperty("id").GetInt32());
            Assert.False(root.TryGetProperty("error", out _));
            Assert.Equal("CreationsForge", root.GetProperty("result").GetProperty("serverInfo").GetProperty("name").GetString());

            await process.StandardInput.WriteLineAsync("{\"jsonrpc\":\"2.0\",\"method\":\"notifications/initialized\"}".AsMemory(), timeout.Token);
            await process.StandardInput.FlushAsync(timeout.Token);
            process.StandardInput.Close();

            await process.WaitForExitAsync(timeout.Token);
            var remainingOutput = await process.StandardOutput.ReadToEndAsync(timeout.Token);
            var standardError = await standardErrorTask;
            Assert.Equal(0, process.ExitCode);
            Assert.True(string.IsNullOrWhiteSpace(remainingOutput), $"Unexpected standard output after initialization: {remainingOutput}");
            Assert.DoesNotContain("Unhandled exception", standardError, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
                await process.WaitForExitAsync(CancellationToken.None);
            }
        }
    }

    /// <summary>Finds the checkout that owns the active test output without relying on a machine-specific path.</summary>
    /// <returns>The absolute directory containing <c>CreationsForge.sln</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the solution cannot be found above the test output directory.</exception>
    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "CreationsForge.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate the CreationsForge repository root from the test output directory.");
    }
}
