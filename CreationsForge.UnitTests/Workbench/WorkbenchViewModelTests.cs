using System.Text.Json.Nodes;
using CreationsForge.Workbench;

namespace CreationsForge.UnitTests.Workbench;

/// <summary>Verifies that the standalone Workbench keeps startup failures visible and cleans up its session.</summary>
public sealed class WorkbenchViewModelTests
{
    /// <summary>Reports a missing MCP executable without throwing out of the UI action.</summary>
    [Fact]
    public async Task ConnectWithMissingExecutableLeavesUsableDisconnectedState()
    {
        var viewModel = new WorkbenchViewModel(static async (path, token) => await StdioMcpClient.StartAsync(path, token).ConfigureAwait(false))
        {
            McpExecutablePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "missing.dll"),
        };

        await viewModel.ConnectAsync();

        Assert.True(viewModel.CanConnect);
        Assert.False(viewModel.CanDisconnect);
        Assert.Equal("Disconnected", viewModel.Status);
        Assert.Contains("Connection failed", viewModel.Diagnostics, StringComparison.Ordinal);
    }

    /// <summary>Reports a file that exists but cannot start as an MCP host, and leaves Connect available.</summary>
    [Fact]
    public async Task ConnectWithUnstartableFileLeavesUsableDisconnectedState()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".txt");
        await File.WriteAllTextAsync(path, "not an executable", TestContext.Current.CancellationToken);
        try
        {
            var viewModel = new WorkbenchViewModel(static async (executable, token) => await StdioMcpClient.StartAsync(executable, token).ConfigureAwait(false))
            {
                McpExecutablePath = path,
            };

            await viewModel.ConnectAsync();

            Assert.True(viewModel.CanConnect);
            Assert.False(viewModel.CanDisconnect);
            Assert.Equal("Disconnected", viewModel.Status);
            Assert.Contains("Connection failed", viewModel.Diagnostics, StringComparison.Ordinal);
        }
        finally
        {
            File.Delete(path);
        }
    }

    /// <summary>A second Connect while startup is in flight does not start another session.</summary>
    [Fact]
    public async Task SecondConnectWhileStartingDoesNotStartAnotherSession()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var starts = 0;
        var viewModel = new WorkbenchViewModel(async (_, cancellationToken) =>
        {
            Interlocked.Increment(ref starts);
            entered.TrySetResult();
            await release.Task.WaitAsync(cancellationToken);
            return new FakeWorkbenchSession();
        });

        var first = viewModel.ConnectAsync();
        await entered.Task;
        var second = viewModel.ConnectAsync();
        Assert.False(viewModel.CanConnect);
        release.TrySetResult();

        await first;
        await second;

        Assert.Equal(1, starts);
        Assert.True(viewModel.CanDisconnect);
        Assert.Contains("Connected to CreationsForge MCP", viewModel.Status, StringComparison.Ordinal);
        await viewModel.CloseAsync();
    }

    /// <summary>Closing during startup cancels the child and restores a usable disconnected screen.</summary>
    [Fact]
    public async Task CloseDuringStartupCancelsAndRestoresDisconnectedState()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var viewModel = new WorkbenchViewModel(async (_, cancellationToken) =>
        {
            entered.TrySetResult();
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return new FakeWorkbenchSession();
        });

        var connect = viewModel.ConnectAsync();
        await entered.Task;
        await viewModel.CloseAsync();
        await connect;

        Assert.Equal("Disconnected", viewModel.Status);
        Assert.True(viewModel.CanConnect);
        Assert.False(viewModel.CanDisconnect);
        Assert.Contains("canceled", viewModel.Diagnostics, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>A session that finishes starting after close begins is disposed instead of kept.</summary>
    [Fact]
    public async Task SessionThatStartsAfterCloseIsDisposed()
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        FakeWorkbenchSession? session = null;
        var viewModel = new WorkbenchViewModel(async (_, _) =>
        {
            entered.TrySetResult();
            await release.Task;
            session = new FakeWorkbenchSession();
            return session;
        });

        var connect = viewModel.ConnectAsync();
        await entered.Task;
        var close = viewModel.CloseAsync();
        release.TrySetResult();
        await connect;
        await close;

        Assert.NotNull(session);
        Assert.True(session.Disposed);
        Assert.True(viewModel.CanConnect);
        Assert.Equal("Disconnected", viewModel.Status);
    }

    /// <summary>Opening a workspace freezes the source selections and close stops that session.</summary>
    [Fact]
    public async Task WorkspaceOpenFreezesSourcesAndCloseDisposesSession()
    {
        var session = new FakeWorkbenchSession();
        var viewModel = new WorkbenchViewModel((_, _) => Task.FromResult<IWorkbenchMcpSession>(session))
        {
            DataDirectory = Path.GetTempPath(),
            SelectedPlugins = "Skyrim.esm, Update.esm",
            OutputPath = Path.Combine(Path.GetTempPath(), "CreationsForgeOutput.esp"),
            MasterStyle = "Small",
            TextStorageMode = "Localized",
            Language = "French",
        };

        await viewModel.ConnectAsync();
        await viewModel.OpenWorkspaceAsync();

        Assert.False(viewModel.CanEditSources);
        Assert.True(viewModel.CanCreateOutput);
        var open = Assert.Single(session.Calls, call => call.Tool == "workspace_open");
        Assert.Contains("Skyrim.esm", open.Arguments, StringComparison.Ordinal);
        Assert.Contains("Update.esm", open.Arguments, StringComparison.Ordinal);

        session.FailTools.Add("output_create");
        await viewModel.OpenOutputAsync(createNew: true);

        Assert.Contains("Output operation failed", viewModel.Diagnostics, StringComparison.Ordinal);
        Assert.Equal("Output operation failed.", viewModel.Status);
        Assert.True(viewModel.CanCreateOutput);
        Assert.DoesNotContain(session.Calls, call => call.Tool == "workspace_save");
        var output = Assert.Single(session.Calls, call => call.Tool == "output_create");
        Assert.Contains("\"language\":\"French\"", output.Arguments, StringComparison.Ordinal);
        Assert.Contains("\"masterStyle\":\"Small\"", output.Arguments, StringComparison.Ordinal);
        Assert.Contains("\"textStorageMode\":\"Localized\"", output.Arguments, StringComparison.Ordinal);

        await viewModel.CloseAsync();

        Assert.True(session.Disposed);
        Assert.Contains(session.Calls, call => call.Tool == "workspace_close" && call.Arguments.Contains("\"discardUnsaved\":true", StringComparison.Ordinal));
        Assert.True(viewModel.CanConnect);
        Assert.True(viewModel.CanEditSources);
        Assert.Equal("Disconnected", viewModel.Status);
    }

    /// <summary>Creating an output saves it and refuses a second create while that session is open.</summary>
    [Fact]
    public async Task CreateOutputSavesThePluginAndBlocksAnotherOutput()
    {
        var session = new FakeWorkbenchSession();
        var outputPath = Path.Combine(Path.GetTempPath(), "CreationsForgeOutput.esm");
        var viewModel = new WorkbenchViewModel((_, _) => Task.FromResult<IWorkbenchMcpSession>(session))
        {
            DataDirectory = Path.GetTempPath(),
            OutputPath = outputPath,
        };

        await viewModel.ConnectAsync();
        await viewModel.OpenWorkspaceAsync();
        await viewModel.OpenOutputAsync(createNew: true);

        Assert.Equal("Created " + outputPath + ".", viewModel.Status);
        Assert.False(viewModel.CanCreateOutput);
        Assert.False(viewModel.CanOpenOutput);
        Assert.Equal(
            ["output_create", "workspace_save"],
            session.Calls.Skip(session.Calls.Count - 2).Select(call => call.Tool).ToArray());

        await viewModel.OpenOutputAsync(createNew: true);

        Assert.Equal(1, session.Calls.Count(call => call.Tool == "output_create"));
        Assert.Equal(1, session.Calls.Count(call => call.Tool == "workspace_save"));
    }

    /// <summary>Drops a created output when the following save is rejected, so the shell can open a workspace again.</summary>
    [Fact]
    public async Task CreateOutputSaveFailureReleasesTheWorkspace()
    {
        var session = new FakeWorkbenchSession();
        var viewModel = new WorkbenchViewModel((_, _) => Task.FromResult<IWorkbenchMcpSession>(session))
        {
            DataDirectory = Path.GetTempPath(),
            OutputPath = Path.Combine(Path.GetTempPath(), "CreationsForgeOutput.esm"),
        };
        session.FailTools.Add("workspace_save");

        await viewModel.ConnectAsync();
        await viewModel.OpenWorkspaceAsync();
        await viewModel.OpenOutputAsync(createNew: true);

        Assert.Equal("Output operation failed.", viewModel.Status);
        Assert.Contains("Output operation failed", viewModel.Diagnostics, StringComparison.Ordinal);
        Assert.True(viewModel.CanOpenWorkspace);
        Assert.True(viewModel.CanEditSources);
        Assert.False(viewModel.CanCreateOutput);
        Assert.Contains(
            session.Calls,
            call => call.Tool == "workspace_close" && call.Arguments.Contains("\"discardUnsaved\":true", StringComparison.Ordinal));
    }

    /// <summary>Cancel stops the active tool call and leaves the connected host in place.</summary>
    [Fact]
    public async Task CancelDuringToolCallLeavesTheSessionConnected()
    {
        var session = new FakeWorkbenchSession { WaitForCancel = true, CallEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously) };
        var viewModel = new WorkbenchViewModel((_, _) => Task.FromResult<IWorkbenchMcpSession>(session))
        {
            DataDirectory = Path.GetTempPath(),
        };
        session.Tools =
        [
            new WorkbenchMcpTool("workspace_open", "Opens a workspace", new JsonObject { ["type"] = "object" }),
        ];

        await viewModel.ConnectAsync();
        var open = viewModel.OpenWorkspaceAsync();
        await session.CallEntered.Task;
        Assert.True(viewModel.CanCancel);
        Assert.Contains("Opening workspace", viewModel.ActivityText, StringComparison.Ordinal);

        await viewModel.CancelAsync();
        await open;

        Assert.False(session.Disposed);
        Assert.True(viewModel.CanDisconnect);
        Assert.False(viewModel.CanCancel);
        Assert.False(viewModel.HasActivity);
        Assert.Contains("canceled", viewModel.Status, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("workspace_open", Assert.Single(viewModel.DiscoveredTools).Name);
        await viewModel.CloseAsync();
    }

    /// <summary>A bounded tool call reports a timeout and keeps the host connected.</summary>
    [Fact]
    public async Task ToolCallTimeoutLeavesTheSessionConnected()
    {
        var session = new FakeWorkbenchSession { WaitForCancel = true };
        var viewModel = new WorkbenchViewModel(
            (_, _) => Task.FromResult<IWorkbenchMcpSession>(session),
            settings: null,
            findMcpExecutable: null,
            findDataDirectory: null,
            connectTimeout: TimeSpan.FromSeconds(2),
            operationTimeout: TimeSpan.FromMilliseconds(200))
        {
            DataDirectory = Path.GetTempPath(),
        };

        await viewModel.ConnectAsync();
        await viewModel.OpenWorkspaceAsync();

        Assert.False(session.Disposed);
        Assert.True(viewModel.CanDisconnect);
        Assert.Contains("timed out", viewModel.Status, StringComparison.OrdinalIgnoreCase);
        await viewModel.CloseAsync();
    }

    /// <summary>An unexpected host exit clears the session and leaves Connect available.</summary>
    [Fact]
    public async Task HostExitClearsTheSessionAndAllowsReconnect()
    {
        var first = new FakeWorkbenchSession();
        var second = new FakeWorkbenchSession
        {
            Tools = [new WorkbenchMcpTool("server_info", "Host identity", new JsonObject { ["type"] = "object" })],
        };
        var sessions = new Queue<IWorkbenchMcpSession>([first, second]);
        var viewModel = new WorkbenchViewModel((_, _) => Task.FromResult(sessions.Dequeue()))
        {
            DataDirectory = Path.GetTempPath(),
        };

        await viewModel.ConnectAsync();
        await viewModel.OpenWorkspaceAsync();
        first.RaiseHostExited();
        var disconnected = await WaitUntilAsync(() => viewModel.CanConnect && viewModel.Status.Contains("exited", StringComparison.OrdinalIgnoreCase));
        Assert.True(disconnected);
        Assert.True(first.Disposed);
        Assert.Empty(viewModel.DiscoveredTools);

        await viewModel.ConnectAsync();
        await viewModel.OpenWorkspaceAsync();

        Assert.Equal("server_info", Assert.Single(viewModel.DiscoveredTools).Name);
        Assert.Contains("workspace_open", Assert.Single(second.Calls).Tool, StringComparison.Ordinal);
        Assert.DoesNotContain(first.Calls.Skip(1), call => call.Tool == "workspace_open");
        await viewModel.CloseAsync();
        Assert.True(second.Disposed);
    }

    private static async Task<bool> WaitUntilAsync(Func<bool> condition)
    {
        var deadline = DateTime.UtcNow.AddSeconds(2);
        while (DateTime.UtcNow < deadline)
        {
            if (condition())
            {
                return true;
            }

            await Task.Delay(20);
        }

        return condition();
    }

    /// <summary>Records tool calls made by the view model without starting a process.</summary>
    private sealed class FakeWorkbenchSession : WorkbenchSessionDouble
    {
        public List<(string Tool, string Arguments)> Calls { get; } = [];

        public HashSet<string> FailTools { get; } = [];

        public TaskCompletionSource? CallEntered { get; set; }

        public bool WaitForCancel { get; set; }

        private string? _outputPath;

        public override async Task<JsonObject> CallAsync(string tool, JsonObject arguments, CancellationToken cancellationToken)
        {
            Calls.Add((tool, arguments.ToJsonString()));
            CallEntered?.TrySetResult();
            if (WaitForCancel)
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (FailTools.Contains(tool))
            {
                throw new InvalidOperationException($"MCP {tool} failed: rejected");
            }

            if (arguments["outputPath"] is JsonValue outputPath && outputPath.TryGetValue<string>(out var path))
            {
                _outputPath = path;
            }

            var content = new JsonObject
            {
                ["workspaceId"] = "session-1",
                ["revision"] = 0,
            };
            if (tool == "workspace_save")
            {
                content["status"] = "Succeeded";
                content["destinationPath"] = _outputPath;
            }

            if (tool == "workspace_close")
            {
                WorkspaceId = null;
                Revision = null;
            }
            else
            {
                WorkspaceId = "session-1";
                Revision = 0;
            }

            return new JsonObject { ["structuredContent"] = content };
        }
    }
}
