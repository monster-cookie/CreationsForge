using Avalonia;

namespace CreationsForge.Workbench;

/// <summary>Starts the independently launchable CreationsForge MCP Workbench.</summary>
internal static class Program
{
    /// <summary>Builds the Avalonia application and starts its desktop lifetime.</summary>
    /// <param name="args">Command-line arguments passed to the Workbench.</param>
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
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
