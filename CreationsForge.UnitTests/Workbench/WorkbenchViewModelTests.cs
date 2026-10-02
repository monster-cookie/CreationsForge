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
        var viewModel = new WorkbenchViewModel
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
            var viewModel = new WorkbenchViewModel { McpExecutablePath = path };

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
        Assert.DoesNotContain("attached", viewModel.Status, StringComparison.OrdinalIgnoreCase);
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

    /// <summary>Records tool calls made by the view model without starting a process.</summary>
    private sealed class FakeWorkbenchSession : IWorkbenchMcpSession
    {
        public string ServerVersion { get; } = "test";

        public List<(string Tool, string Arguments)> Calls { get; } = [];

        public HashSet<string> FailTools { get; } = [];

        public bool Disposed { get; private set; }

        public Task<JsonObject> CallAsync(string tool, JsonObject arguments, CancellationToken cancellationToken)
        {
            Calls.Add((tool, arguments.ToJsonString()));
            cancellationToken.ThrowIfCancellationRequested();
            if (FailTools.Contains(tool))
            {
                throw new InvalidOperationException($"MCP {tool} failed: rejected");
            }

            return Task.FromResult(new JsonObject
            {
                ["structuredContent"] = new JsonObject { ["workspaceId"] = "session-1" },
            });
        }

        public ValueTask DisposeAsync()
        {
            Disposed = true;
            return ValueTask.CompletedTask;
        }
    }
}
