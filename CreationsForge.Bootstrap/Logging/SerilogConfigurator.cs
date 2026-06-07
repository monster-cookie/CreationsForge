using CreationsForge.Core.Configuration.Interfaces;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace CreationsForge.Bootstrap.Logging;

public static class SerilogConfigurator
{
    public static void Configure(IApplicationConfigurationStore configurationStore, bool writeToConsole)
    {
        Directory.CreateDirectory(configurationStore.Current.LoggingDirectory);
        var startupTime = DateTimeOffset.Now.ToString("yyyyMMdd-HHmmss-fff");
        var logPath = Path.Combine(configurationStore.Current.LoggingDirectory, $"CreationsForge-{startupTime}.log");

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
            shared: true,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
    }
}
