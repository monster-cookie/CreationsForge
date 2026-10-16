using Serilog;
using Serilog.Events;

namespace CreationsForge.Workbench;

/// <summary>Configures the process-wide Serilog file shared by the Workbench and its MCP child diagnostics.</summary>
internal static class WorkbenchLog
{
    /// <summary>Gets the active startup log path after configuration completes.</summary>
    internal static string? CurrentLogPath { get; private set; }

    /// <summary>Gets a description of a configuration failure that left the process without a file sink.</summary>
    internal static string? StartupError { get; private set; }

    /// <summary>Creates the process-wide logger in the standard Creations Forge logs directory.</summary>
    /// <remarks>A directory that cannot be created falls forward to the user or temporary logs directory. Logging never prevents the shell from opening.</remarks>
    internal static void Configure()
    {
        try
        {
            Configure(ResolveLoggingDirectory());
        }
        catch (Exception exception)
        {
            StartupError = exception.ToString();
        }
    }

    /// <summary>Creates the process-wide logger in an explicit directory.</summary>
    /// <param name="loggingDirectory">The directory that receives one startup-specific log file.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="loggingDirectory"/> is blank.</exception>
    /// <exception cref="IOException">Thrown when the directory or file sink cannot be created.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the directory cannot be created.</exception>
    internal static void Configure(string loggingDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loggingDirectory);
        Directory.CreateDirectory(loggingDirectory);
        var startupTime = DateTimeOffset.Now.ToString("yyyyMMdd-HHmmss-fff");
        var logPath = Path.Combine(loggingDirectory, $"CreationsForgeWorkbench-{startupTime}.log");
        var configuration = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.File(
                logPath,
                rollingInterval: RollingInterval.Infinite,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 1024 * 1024 * 100,
                retainedFileCountLimit: 10,
                shared: false,
                buffered: false,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");

        Log.Logger = configuration.CreateLogger();
        CurrentLogPath = logPath;
        StartupError = null;
        Log.Information("CreationsForge Workbench logging started. Log file: {LogPath}", logPath);
    }

    /// <summary>Flushes the current logger and restores the silent logger so later tests do not keep the file open.</summary>
    internal static void CloseAndReset()
    {
        Log.CloseAndFlush();
        Log.Logger = Serilog.Core.Logger.None;
    }

    /// <summary>Selects the first writable logs directory, preferring the historical Creations Forge location.</summary>
    /// <returns>A directory that was created and accepted a probe file.</returns>
    /// <exception cref="IOException">Thrown when every candidate directory is unusable.</exception>
    private static string ResolveLoggingDirectory()
    {
        var preferred = OperatingSystem.IsWindows()
            ? CombineRoot(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "CreationsForge", "Logs")
            : CombineRoot(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".CreationsForge", "Logs");
        if (preferred is not null && CanWrite(preferred))
        {
            return preferred;
        }

        var local = CombineRoot(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CreationsForge", "Logs");
        if (local is not null && CanWrite(local))
        {
            return local;
        }

        var temporary = Path.Combine(Path.GetTempPath(), "CreationsForge", "Logs");
        if (!CanWrite(temporary))
        {
            throw new IOException($"Unable to create a Workbench log directory at '{temporary}'.");
        }

        return temporary;
    }

    /// <summary>Joins a non-empty root with the remaining path parts.</summary>
    /// <param name="root">The folder path returned by the operating system.</param>
    /// <param name="parts">The path parts appended to <paramref name="root"/>.</param>
    /// <returns>The combined path, or <see langword="null"/> when <paramref name="root"/> is blank.</returns>
    private static string? CombineRoot(string root, params string[] parts)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            return null;
        }

        var path = root;
        foreach (var part in parts)
        {
            path = Path.Combine(path, part);
        }

        return path;
    }

    /// <summary>Creates a directory and checks that a file can be written there.</summary>
    /// <param name="directory">The candidate logs directory.</param>
    /// <returns><see langword="true"/> when a probe file can be created and deleted.</returns>
    private static bool CanWrite(string directory)
    {
        try
        {
            Directory.CreateDirectory(directory);
            var probe = Path.Combine(directory, ".write-probe");
            File.WriteAllText(probe, "ok");
            File.Delete(probe);
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }
}
