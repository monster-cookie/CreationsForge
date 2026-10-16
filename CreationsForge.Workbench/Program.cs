using Avalonia;
using Serilog;

namespace CreationsForge.Workbench;

/// <summary>Starts the independently launchable CreationsForge MCP Workbench.</summary>
internal static class Program
{
    /// <summary>Builds the Avalonia application and starts its desktop lifetime.</summary>
    /// <param name="args">Command-line arguments passed to the Workbench.</param>
    [STAThread]
    public static void Main(string[] args)
    {
        WorkbenchLog.Configure();
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        try
        {
            Log.Information("Starting CreationsForge Workbench.");
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
            Log.Information("CreationsForge Workbench exited.");
        }
        catch (Exception exception)
        {
            Log.Fatal(exception, "CreationsForge Workbench stopped because startup failed.");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>Writes a fatal log entry for an exception that escaped the Avalonia lifetime.</summary>
    /// <param name="sender">The application domain.</param>
    /// <param name="eventArgs">The unhandled exception.</param>
    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
    {
        if (eventArgs.ExceptionObject is Exception exception)
        {
            Log.Fatal(exception, "Unhandled Workbench exception.");
        }
        else
        {
            Log.Fatal("Unhandled Workbench exception: {ExceptionObject}", eventArgs.ExceptionObject);
        }

        Log.CloseAndFlush();
    }

    /// <summary>Writes a task exception that nobody observed before the process continues.</summary>
    /// <param name="sender">The task scheduler.</param>
    /// <param name="eventArgs">The unobserved task exception.</param>
    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs eventArgs)
    {
        Log.Error(eventArgs.Exception, "Unobserved Workbench task exception.");
        eventArgs.SetObserved();
    }

    /// <summary>Creates the platform-specific Avalonia application builder.</summary>
    /// <returns>The configured Avalonia application builder.</returns>
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
    }
}
