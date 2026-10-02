using System.Diagnostics;
using System.Text.Json.Nodes;
using CreationsForge.Mcp.Protocol;
using CreationsForge.UnitTests.McpHost;
using CreationsForge.Workbench;

namespace CreationsForge.UnitTests.Workbench;

/// <summary>Verifies the Workbench stdio client against a blocking child and the production MCP host.</summary>
public sealed class StdioMcpClientTests
{
    /// <summary>Canceling initialize stops the child process that was started for the handshake.</summary>
    [Fact]
    public async Task CanceledStartupStopsTheChildProcess()
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            StdioMcpClient.StartAsync(BlockingExecutable(), timeout.Token));
        Assert.NotNull(exception);
        Assert.True(ProcessHasExited(StdioMcpClient.LastProcessId));
    }

    /// <summary>Initialize, a domain tool failure, overlapping calls, and disposal all use the production host.</summary>
    [Fact]
    public async Task ProductionHostInitializeFailureAndDisposeStayBounded()
    {
        var assemblyPath = McpHostPaths.McpAssembly();
        Assert.True(File.Exists(assemblyPath), $"The MCP build output was not found at '{assemblyPath}'.");
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var client = await StdioMcpClient.StartAsync(assemblyPath, timeout.Token);
        var processId = client.ProcessId;
        try
        {
            Assert.False(string.IsNullOrWhiteSpace(client.ServerVersion));
            var first = client.CallAsync(McpAuthoringContract.ServerInfoTool, new JsonObject(), timeout.Token);
            var second = client.CallAsync(McpAuthoringContract.ServerInfoTool, new JsonObject(), timeout.Token);
            var info = await Task.WhenAll(first, second);
            Assert.All(info, result => Assert.NotNull(result["structuredContent"] ?? result["content"]));

            var failure = await Assert.ThrowsAsync<InvalidOperationException>(() => client.CallAsync(
                McpAuthoringContract.RecordTypesTool,
                new JsonObject { ["release"] = "NotAGame" },
                timeout.Token));
            Assert.Contains("not supported", failure.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            await client.DisposeAsync();
        }

        Assert.True(ProcessHasExited(processId));
    }

    private static string BlockingExecutable()
    {
        if (OperatingSystem.IsWindows())
        {
            var sort = Path.Combine(Environment.SystemDirectory, "sort.exe");
            Assert.True(File.Exists(sort), "Windows sort.exe was not found.");
            return sort;
        }

        var bash = File.Exists("/bin/bash") ? "/bin/bash" : "/usr/bin/bash";
        Assert.True(File.Exists(bash), "bash was not found.");
        return bash;
    }

    private static bool ProcessHasExited(int processId)
    {
        try
        {
            using var process = Process.GetProcessById(processId);
            return process.HasExited;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }
}
