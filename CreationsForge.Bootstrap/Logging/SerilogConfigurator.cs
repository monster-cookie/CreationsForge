using CreationsForge.Core.Configuration.Interfaces;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace CreationsForge.Bootstrap.Logging;

/// <summary>Configures the process-wide structured logger shared by desktop services and the headless engine.</summary>
public static class SerilogConfigurator
{
    /// <summary>Gets the active startup-specific desktop log path after configuration completes.</summary>
    public static string? CurrentLogPath { get; private set; }

    /// <summary>Creates the process-wide logger using the configured directory and optional diagnostic console output.</summary>
    /// <param name="configurationStore">The application configuration containing the logging directory.</param>
    /// <param name="writeToConsole">Whether log events are also written to the process console.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="configurationStore"/> is <see langword="null"/>.</exception>
    public static void Configure(IApplicationConfigurationStore configurationStore, bool writeToConsole)
    {
        ArgumentNullException.ThrowIfNull(configurationStore);
        Directory.CreateDirectory(configurationStore.Current.LoggingDirectory);
        var startupTime = DateTimeOffset.Now.ToString("yyyyMMdd-HHmmss-fff");
        var logPath = Path.Combine(configurationStore.Current.LoggingDirectory, $"CreationsForge-{startupTime}.log");
        CurrentLogPath = logPath;

        var loggerConfiguration = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName();

        ConfigureSinks(loggerConfiguration.WriteTo, logPath, writeToConsole);
        Log.Logger = loggerConfiguration.CreateLogger();
    }

    /// <summary>Adds the requested console sink and the durable startup-specific file sink.</summary>
    /// <param name="sinks">The Serilog sink configuration to populate.</param>
    /// <param name="logPath">The complete startup-specific file path.</param>
    /// <param name="writeToConsole">Whether to include a console sink.</param>
    private static void ConfigureSinks(LoggerSinkConfiguration sinks, string logPath, bool writeToConsole)
    {
        if (writeToConsole)
        {
            sinks.Console();
        }

        sinks.File(
            logPath,
            rollingInterval: RollingInterval.Infinite,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 1024 * 1024 * 100, // 100mb
            retainedFileCountLimit: 10,
            shared: false,
            buffered: false,
            flushToDiskInterval: TimeSpan.FromSeconds(1),
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
    }
}
