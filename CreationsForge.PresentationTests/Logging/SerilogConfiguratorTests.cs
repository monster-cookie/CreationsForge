using System.Diagnostics;
using CreationsForge.Bootstrap.Logging;
using CreationsForge.Core.Configuration;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.PresentationTests.Headless;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Logging;

/// <summary>Verifies the production desktop logger writes and flushes events through its file sink.</summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class SerilogConfiguratorTests
{
    /// <summary>Verifies static and injected logger events reach the configured production log file.</summary>
    [Fact]
    public void Configure_WritesStaticAndInjectedEventsToConfiguredFile()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "CreationsForge-PresentationTests", Guid.NewGuid().ToString("N"));
        var loggingPath = Path.Combine(rootPath, "Logs");
        var configurationStore = new ApplicationConfigurationStore(Path.Combine(rootPath, "CreationsForge.Config.json"));
        configurationStore.Save(new ApplicationConfiguration
        {
            ApplicationDataDirectory = rootPath,
            LoggingDirectory = loggingPath
        });
        var originalLogger = Log.Logger;
        ILogger? configuredLogger = null;

        try
        {
            SerilogConfigurator.Configure(configurationStore, writeToConsole: false);
            configuredLogger = Log.Logger;

            Log.Information("Static desktop logging verification {VerificationId}", 17);
            configuredLogger.Warning("Injected desktop logging verification {VerificationId}", 23);

            SerilogConfigurator.CurrentLogPath.ShouldNotBeNullOrWhiteSpace();
            Path.GetDirectoryName(SerilogConfigurator.CurrentLogPath).ShouldBe(loggingPath);
            File.Exists(SerilogConfigurator.CurrentLogPath).ShouldBeTrue();
            WaitForNonEmptyLog(SerilogConfigurator.CurrentLogPath).ShouldBeTrue();

            Log.CloseAndFlush();
            var flushedContents = File.ReadAllText(SerilogConfigurator.CurrentLogPath);
            flushedContents.ShouldContain("[INF] Static desktop logging verification 17");
            flushedContents.ShouldContain("[WRN] Injected desktop logging verification 23");
        }
        finally
        {
            Log.CloseAndFlush();
            if (configuredLogger is IDisposable disposableLogger)
            {
                disposableLogger.Dispose();
            }

            Log.Logger = originalLogger;
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }
        }
    }

    /// <summary>Waits briefly for the configured periodic disk flush to make the active log non-empty.</summary>
    /// <param name="logPath">The active production log path.</param>
    /// <returns><see langword="true"/> when the active log reports durable bytes before the deadline.</returns>
    private static bool WaitForNonEmptyLog(string logPath)
    {
        var startedTimestamp = Stopwatch.GetTimestamp();
        while (Stopwatch.GetElapsedTime(startedTimestamp) < TimeSpan.FromSeconds(3))
        {
            var file = new FileInfo(logPath);
            file.Refresh();
            if (file.Exists && file.Length > 0)
            {
                return true;
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(50));
        }

        return false;
    }
}
