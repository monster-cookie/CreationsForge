using System.Text.Json;
using CreationsForge.Core.Configuration;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class ProcessTerminationDiagnosticsServiceTests
{
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

    [Fact]
    public void UpdateHeartbeat_WritesLastProgressAndMemorySnapshot()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var service = CreateService(tempDirectory.FullName);
            service.StartSession("UnitTest", "test.log");

            service.UpdateHeartbeat(
                "Import Starfield",
                new GameImportProgressDTO
                {
                    StatusText = "Indexing Starfield asset archives",
                    DetailText = "Starfield - Meshes01.ba2",
                    ProgressValue = 1,
                    ProgressMaximum = 2,
                    IsIndeterminate = false
                });

            using var document = ReadSession(tempDirectory.FullName);
            document.RootElement.GetProperty("LastPhase").GetString().ShouldBe("Import Starfield");
            document.RootElement.GetProperty("LastStatusText").GetString().ShouldBe("Indexing Starfield asset archives");
            document.RootElement.GetProperty("LastDetailText").GetString().ShouldBe("Starfield - Meshes01.ba2");
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

    private static ProcessTerminationDiagnosticsService CreateService(string applicationDataDirectory)
    {
        var configurationPath = Path.Combine(applicationDataDirectory, "CreationsForge.Config.json");
        var configurationStore = new ApplicationConfigurationStore(configurationPath);
        configurationStore.Save(new ApplicationConfiguration
        {
            ApplicationDataDirectory = applicationDataDirectory,
            DatabaseDirectory = applicationDataDirectory,
            LoggingDirectory = Path.Combine(applicationDataDirectory, "Logs")
        });
        return new ProcessTerminationDiagnosticsService(configurationStore);
    }

    private static JsonDocument ReadSession(string applicationDataDirectory)
    {
        var sessionPath = Path.Combine(applicationDataDirectory, "CreationsForge.Session.json");
        return JsonDocument.Parse(File.ReadAllText(sessionPath));
    }
}
