namespace CreationsForge.UnitTests.Workbench;

/// <summary>Verifies that the standalone Workbench keeps configuration failures visible and recoverable.</summary>
public sealed class WorkbenchViewModelTests
{
    /// <summary>Reports a missing MCP executable without throwing out of the UI action.</summary>
    [Fact]
    public async Task ConnectWithMissingExecutableLeavesUsableDisconnectedState()
    {
        var viewModel = new CreationsForge.Workbench.WorkbenchViewModel
        {
            McpExecutablePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "missing.dll"),
        };

        await viewModel.ConnectAsync();

        Assert.True(viewModel.CanConnect);
        Assert.False(viewModel.CanDisconnect);
        Assert.Equal("Disconnected", viewModel.Status);
        Assert.Contains("Connection failed", viewModel.Diagnostics, StringComparison.Ordinal);
    }
}
