using System.Text.Json;
using CreationsForge.Core.Configuration;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>
/// Verifies persisted process-session lifecycle and heartbeat diagnostics.
/// </summary>
public class ProcessTerminationDiagnosticsServiceTests
{
    /// <summary>
    /// Verifies starting a session writes an unclean marker and resource snapshot.
    /// </summary>
    [Fact]
    public void StartSession_WritesUncleanSessionMarker()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var service = CreateService(tempDirectory.FullName);

            service.StartSession("UnitTest", "test.log");

            using var document = ReadSession(tempDirectory.FullName);
            document.RootElement.GetProperty("SurfaceName").GetString().ShouldBe("UnitTest");
            document.RootElement.GetProperty("LogPath").GetString().ShouldBe("test.log");
            document.RootElement.GetProperty("CleanShutdown").GetBoolean().ShouldBeFalse();
            document.RootElement.GetProperty("HandleCount").GetInt32().ShouldBeGreaterThan(0);
            document.RootElement.GetProperty("ThreadCount").GetInt32().ShouldBeGreaterThan(0);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>
    /// Verifies a heartbeat records its phase and process resources without import-specific payload fields.
    /// </summary>
    [Fact]
    public void UpdateHeartbeat_WritesPhaseAndMemorySnapshot()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var service = CreateService(tempDirectory.FullName);
            service.StartSession("UnitTest", "test.log");

            service.UpdateHeartbeat("Native workspace save");

            using var document = ReadSession(tempDirectory.FullName);
            document.RootElement.GetProperty("LastPhase").GetString().ShouldBe("Native workspace save");
            document.RootElement.TryGetProperty("LastStatusText", out _).ShouldBeFalse();
            document.RootElement.TryGetProperty("LastDetailText", out _).ShouldBeFalse();
            document.RootElement.TryGetProperty("LastGame", out _).ShouldBeFalse();
            document.RootElement.GetProperty("ManagedBytes").GetInt64().ShouldBeGreaterThan(0);
            document.RootElement.GetProperty("WorkingSetBytes").GetInt64().ShouldBeGreaterThan(0);
            document.RootElement.GetProperty("PrivateBytes").GetInt64().ShouldBeGreaterThan(0);
            document.RootElement.GetProperty("HandleCount").GetInt32().ShouldBeGreaterThan(0);
            document.RootElement.GetProperty("ThreadCount").GetInt32().ShouldBeGreaterThan(0);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>
    /// Verifies a completed session records a clean shutdown and completion timestamp.
    /// </summary>
    [Fact]
    public void MarkCleanShutdown_MarksSessionClean()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var service = CreateService(tempDirectory.FullName);
            service.StartSession("UnitTest", "test.log");

            service.MarkCleanShutdown("test completed");

            using var document = ReadSession(tempDirectory.FullName);
            document.RootElement.GetProperty("CleanShutdown").GetBoolean().ShouldBeTrue();
            document.RootElement.GetProperty("ShutdownReason").GetString().ShouldBe("test completed");
            document.RootElement.GetProperty("CompletedAtUTC").ValueKind.ShouldBe(JsonValueKind.String);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    /// <summary>
    /// Creates diagnostics backed by an isolated application-data directory.
    /// </summary>
    /// <param name="applicationDataDirectory">The isolated directory used for configuration and session files.</param>
    /// <returns>The diagnostics service under test.</returns>
    private static ProcessTerminationDiagnosticsService CreateService(string applicationDataDirectory)
    {
        var configurationPath = Path.Combine(applicationDataDirectory, "CreationsForge.Config.json");
        var configurationStore = new ApplicationConfigurationStore(configurationPath);
        configurationStore.Save(new ApplicationConfiguration
        {
            ApplicationDataDirectory = applicationDataDirectory,
            LoggingDirectory = Path.Combine(applicationDataDirectory, "Logs")
        });
        return new ProcessTerminationDiagnosticsService(configurationStore);
    }

    /// <summary>
    /// Reads the persisted diagnostic session document.
    /// </summary>
    /// <param name="applicationDataDirectory">The isolated directory containing the session file.</param>
    /// <returns>The parsed session document.</returns>
    private static JsonDocument ReadSession(string applicationDataDirectory)
    {
        var sessionPath = Path.Combine(applicationDataDirectory, "CreationsForge.Session.json");
        return JsonDocument.Parse(File.ReadAllText(sessionPath));
    }
}
