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

            var failure = await Assert.ThrowsAsync<WorkbenchMcpException>(() => client.CallAsync(
                McpAuthoringContract.RecordTypesTool,
                new JsonObject { ["release"] = "NotAGame" },
                timeout.Token));
            Assert.Equal(WorkbenchMcpFailureKind.Tool, failure.Kind);
            Assert.Contains("not supported", failure.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            await client.DisposeAsync();
        }

        Assert.True(ProcessHasExited(processId));
    }

    /// <summary>Negotiates the production protocol, lists tool schemas, carries receipts, and leaves no child after disposal.</summary>
    [Fact]
    public async Task ProductionHostHandshakeDiscoveryAndReceiptsUseTheRealProcess()
    {
        var assemblyPath = McpHostPaths.McpAssembly();
        Assert.True(File.Exists(assemblyPath), $"The MCP build output was not found at '{assemblyPath}'.");
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var directory = Path.Combine(Path.GetTempPath(), "cf-workbench-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var client = await StdioMcpClient.StartAsync(assemblyPath, timeout.Token);
        var processId = client.ProcessId;
        try
        {
            Assert.Equal(StdioMcpClient.SupportedProtocol, client.ProtocolVersion);
            Assert.Equal("CreationsForge", client.ServerName);
            Assert.False(string.IsNullOrWhiteSpace(client.ServerVersion));
            Assert.NotNull(client.Capabilities["tools"]);
            Assert.Equal(
                McpAuthoringContract.ToolNames.OrderBy(name => name, StringComparer.Ordinal).ToArray(),
                client.Tools.Select(tool => tool.Name).OrderBy(name => name, StringComparer.Ordinal).ToArray());
            Assert.All(client.Tools, tool => Assert.False(string.IsNullOrWhiteSpace(tool.InputSchemaJson)));
            var create = Assert.Single(client.Tools, tool => tool.Name == McpAuthoringContract.RecordCreateTool);
            Assert.Contains("workspaceId", create.InputSchemaJson, StringComparison.Ordinal);
            Assert.Contains("expectedRevision", create.InputSchemaJson, StringComparison.Ordinal);

            var info = await client.CallAsync(McpAuthoringContract.ServerInfoTool, new JsonObject(), timeout.Token);
            Assert.NotNull(info["structuredContent"] ?? info["content"]);
            var stderr = client.StandardError;
            if (stderr.Length > 0)
            {
                Assert.DoesNotContain(stderr, info.ToJsonString(), StringComparison.Ordinal);
            }

            var opened = await client.CallAsync(
                McpAuthoringContract.WorkspaceOpenTool,
                new JsonObject
                {
                    ["operationId"] = "workbench-open-" + Guid.NewGuid().ToString("N"),
                    ["release"] = "Starfield",
                    ["dataDirectory"] = directory,
                    ["selectedPlugins"] = new JsonArray(),
                },
                timeout.Token);
            Assert.False(string.IsNullOrWhiteSpace(client.WorkspaceId));
            Assert.Equal(0UL, client.Revision);
            Assert.Equal(client.WorkspaceId, opened["structuredContent"]?["workspaceId"]?.GetValue<string>());

            var rejected = await Assert.ThrowsAsync<WorkbenchMcpException>(() => client.CallAsync(
                McpAuthoringContract.RecordTypesTool,
                new JsonObject { ["release"] = "NotAGame" },
                timeout.Token));
            Assert.Equal(WorkbenchMcpFailureKind.Tool, rejected.Kind);
            Assert.Contains("not supported", rejected.Message, StringComparison.OrdinalIgnoreCase);

            var carried = await Assert.ThrowsAsync<WorkbenchMcpException>(() => client.CallAsync(
                McpAuthoringContract.RecordCreateTool,
                new JsonObject
                {
                    ["operationId"] = "workbench-create-" + Guid.NewGuid().ToString("N"),
                    ["familyId"] = "Keyword",
                    ["changes"] = new JsonArray(),
                },
                timeout.Token));
            Assert.Equal(WorkbenchMcpFailureKind.Tool, carried.Kind);
            Assert.Contains("open native workspace", carried.Message, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("expectedRevision is required", carried.Message, StringComparison.Ordinal);
            Assert.DoesNotContain("workspaceId is required", carried.Message, StringComparison.OrdinalIgnoreCase);

            await client.CallAsync(
                McpAuthoringContract.WorkspaceCloseTool,
                new JsonObject
                {
                    ["operationId"] = "workbench-close-" + Guid.NewGuid().ToString("N"),
                    ["discardUnsaved"] = true,
                },
                timeout.Token);
            Assert.Null(client.WorkspaceId);
            Assert.Null(client.Revision);
        }
        finally
        {
            await client.DisposeAsync();
            Directory.Delete(directory, recursive: true);
        }

        Assert.True(ProcessHasExited(processId));
    }

    /// <summary>Canceling a hung tool call notifies the child and leaves that child running until disposal.</summary>
    [Fact]
    public async Task CanceledToolCallNotifiesTheChildWithoutOrphaningIt()
    {
        var directory = Path.Combine(Path.GetTempPath(), "cf-pause-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var scriptPath = Path.Combine(directory, "pause-host.ps1");
        var cancelFile = Path.Combine(directory, "canceled.txt");
        var callFile = Path.Combine(directory, "called.txt");
        await File.WriteAllTextAsync(scriptPath, PauseHostScript(), TestContext.Current.CancellationToken);
        var client = await StdioMcpClient.StartAsync(PauseStartInfo(directory, scriptPath, cancelFile, callFile, protocolVersion: null, omitTools: false), TestContext.Current.CancellationToken);
        var processId = client.ProcessId;
        try
        {
            using var callCancel = new CancellationTokenSource();
            var call = client.CallAsync("server_info", new JsonObject(), callCancel.Token);
            await WaitForFileAsync(callFile);
            await callCancel.CancelAsync();
            await Assert.ThrowsAnyAsync<OperationCanceledException>(() => call);
            var notified = await WaitForFileAsync(cancelFile);
            Assert.Contains("notifications/cancelled", notified, StringComparison.Ordinal);
            Assert.False(ProcessHasExited(processId));
        }
        finally
        {
            await client.DisposeAsync();
            Directory.Delete(directory, recursive: true);
        }

        Assert.True(ProcessHasExited(processId));
    }

    /// <summary>An unsupported protocol version stops the child instead of continuing the session.</summary>
    [Fact]
    public async Task UnsupportedProtocolStopsTheChild()
    {
        var directory = Path.Combine(Path.GetTempPath(), "cf-protocol-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var scriptPath = Path.Combine(directory, "pause-host.ps1");
        await File.WriteAllTextAsync(scriptPath, PauseHostScript(), TestContext.Current.CancellationToken);
        try
        {
            var exception = await Assert.ThrowsAsync<WorkbenchMcpException>(() => StdioMcpClient.StartAsync(
                PauseStartInfo(directory, scriptPath, cancelFile: null, callFile: null, protocolVersion: "1999-01-01", omitTools: false),
                TestContext.Current.CancellationToken));
            Assert.Equal(WorkbenchMcpFailureKind.Protocol, exception.Kind);
            Assert.Contains("1999-01-01", exception.Message, StringComparison.Ordinal);
            Assert.True(ProcessHasExited(StdioMcpClient.LastProcessId));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    /// <summary>A host that does not advertise tools is stopped instead of being treated as connected.</summary>
    [Fact]
    public async Task MissingToolsCapabilityStopsTheChild()
    {
        var directory = Path.Combine(Path.GetTempPath(), "cf-tools-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        var scriptPath = Path.Combine(directory, "pause-host.ps1");
        await File.WriteAllTextAsync(scriptPath, PauseHostScript(), TestContext.Current.CancellationToken);
        try
        {
            var exception = await Assert.ThrowsAsync<WorkbenchMcpException>(() => StdioMcpClient.StartAsync(
                PauseStartInfo(directory, scriptPath, cancelFile: null, callFile: null, protocolVersion: null, omitTools: true),
                TestContext.Current.CancellationToken));
            Assert.Equal(WorkbenchMcpFailureKind.Protocol, exception.Kind);
            Assert.Contains("tools capability", exception.Message, StringComparison.OrdinalIgnoreCase);
            Assert.True(ProcessHasExited(StdioMcpClient.LastProcessId));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    /// <summary>Killing the production host is reported and disposal does not leave that process running.</summary>
    [Fact]
    public async Task ProductionHostTerminationIsObservedAndLeavesNoOrphan()
    {
        var assemblyPath = McpHostPaths.McpAssembly();
        Assert.True(File.Exists(assemblyPath), $"The MCP build output was not found at '{assemblyPath}'.");
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        var client = await StdioMcpClient.StartAsync(assemblyPath, timeout.Token);
        var processId = client.ProcessId;
        var exited = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        client.HostExited += (_, _) => exited.TrySetResult();
        try
        {
            using var process = Process.GetProcessById(processId);
            process.Kill(entireProcessTree: true);
            await exited.Task.WaitAsync(timeout.Token);
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

    private static ProcessStartInfo PauseStartInfo(
        string directory,
        string scriptPath,
        string? cancelFile,
        string? callFile,
        string? protocolVersion,
        bool omitTools)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = FindPowerShell(),
            WorkingDirectory = directory,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-File");
        startInfo.ArgumentList.Add(scriptPath);
        if (!string.IsNullOrWhiteSpace(protocolVersion))
        {
            startInfo.Environment["WORKBENCH_PROTOCOL_VERSION"] = protocolVersion;
        }

        if (omitTools)
        {
            startInfo.Environment["WORKBENCH_OMIT_TOOLS"] = "1";
        }

        if (!string.IsNullOrWhiteSpace(cancelFile))
        {
            startInfo.Environment["WORKBENCH_CANCEL_FILE"] = cancelFile;
        }

        if (!string.IsNullOrWhiteSpace(callFile))
        {
            startInfo.Environment["WORKBENCH_CALL_FILE"] = callFile;
        }

        return startInfo;
    }

    private static string FindPowerShell()
    {
        var names = OperatingSystem.IsWindows() ? new[] { "pwsh.exe", "powershell.exe" } : ["pwsh"];
        var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        foreach (var name in names)
        {
            foreach (var directory in path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                var candidate = Path.Combine(directory.Trim(), name);
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        throw new InvalidOperationException("PowerShell was not found on PATH.");
    }

    private static async Task<string> WaitForFileAsync(string path)
    {
        var deadline = DateTime.UtcNow.AddSeconds(5);
        while (DateTime.UtcNow < deadline)
        {
            if (File.Exists(path))
            {
                return await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken);
            }

            await Task.Delay(50, TestContext.Current.CancellationToken);
        }

        throw new InvalidOperationException($"The cancellation file was not written: {path}");
    }

    private static string PauseHostScript()
    {
        return """
            $ErrorActionPreference = 'Stop'
            $protocol = $env:WORKBENCH_PROTOCOL_VERSION
            if ([string]::IsNullOrWhiteSpace($protocol)) { $protocol = '2025-06-18' }
            $includeTools = $env:WORKBENCH_OMIT_TOOLS -ne '1'
            $utf8 = New-Object System.Text.UTF8Encoding $false
            [Console]::OutputEncoding = $utf8
            $OutputEncoding = $utf8

            function Get-RequestId([string] $line) {
                $match = [regex]::Match($line, '"id"\s*:\s*(\d+)')
                if ($match.Success) { return [int]$match.Groups[1].Value }
                return 1
            }

            function Write-Response([int] $id, [string] $resultJson) {
                [Console]::Out.WriteLine(('{{"jsonrpc":"2.0","id":{0},"result":{1}}}' -f $id, $resultJson))
                [Console]::Out.Flush()
            }

            while ($null -ne ($line = [Console]::In.ReadLine())) {
                if ($line.Contains('"method":"initialize"')) {
                    if ($includeTools) { $capabilities = '{"tools":{"listChanged":false}}' } else { $capabilities = '{}' }
                    $result = '{"protocolVersion":"' + $protocol + '","capabilities":' + $capabilities + ',"serverInfo":{"name":"pause","version":"0"}}'
                    Write-Response (Get-RequestId $line) $result
                    continue
                }

                if ($line.Contains('"method":"tools/list"')) {
                    Write-Response (Get-RequestId $line) '{"tools":[]}'
                    continue
                }

                if ($line.Contains('"method":"tools/call"')) {
                    if (-not [string]::IsNullOrWhiteSpace($env:WORKBENCH_CALL_FILE)) {
                        Set-Content -LiteralPath $env:WORKBENCH_CALL_FILE -Value $line -Encoding utf8
                    }

                    continue
                }

                if ($line.Contains('"method":"notifications/cancelled"')) {
                    if (-not [string]::IsNullOrWhiteSpace($env:WORKBENCH_CANCEL_FILE)) {
                        Set-Content -LiteralPath $env:WORKBENCH_CANCEL_FILE -Value $line -Encoding utf8
                    }

                    continue
                }
            }
            """;
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
