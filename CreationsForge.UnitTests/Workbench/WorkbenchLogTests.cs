using CreationsForge.Workbench;

namespace CreationsForge.UnitTests.Workbench;

/// <summary>Verifies the Workbench file logger writes a startup event that can be inspected after a failure.</summary>
public sealed class WorkbenchLogTests
{
    /// <summary>Writes the startup event to the requested directory and then releases the file.</summary>
    [Fact]
    public void Configure_WritesStartupEventToRequestedDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), "CreationsForgeWorkbenchLogs", Guid.NewGuid().ToString("N"));
        try
        {
            WorkbenchLog.Configure(directory);
            var path = WorkbenchLog.CurrentLogPath;
            WorkbenchLog.CloseAndReset();

            Assert.False(string.IsNullOrWhiteSpace(path));
            Assert.Equal(directory, Path.GetDirectoryName(path));
            Assert.StartsWith("CreationsForgeWorkbench-", Path.GetFileName(path), StringComparison.Ordinal);
            var contents = File.ReadAllText(path!);
            Assert.Contains("[INF] CreationsForge Workbench logging started.", contents, StringComparison.Ordinal);
            Assert.Contains(path, contents, StringComparison.Ordinal);
        }
        finally
        {
            WorkbenchLog.CloseAndReset();
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, recursive: true);
            }
        }
    }
}
