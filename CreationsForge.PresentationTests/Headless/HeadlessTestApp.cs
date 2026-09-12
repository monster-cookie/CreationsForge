using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using CreationsForge.Core.Models.Configuration;

[assembly: AvaloniaTestApplication(typeof(CreationsForge.PresentationTests.Headless.HeadlessTestApp))]

namespace CreationsForge.PresentationTests.Headless;

/// <summary>Provides the Skia-backed headless Avalonia application used by presentation tests and rendered artifacts.</summary>
public class HeadlessTestApp : Application
{
    /// <summary>The opt-in environment variable for a separate Skia-backed rendered-artifact test process.</summary>
    private const string RenderedArtifactEnvironmentVariable = "CREATIONSFORGE_RENDER_HEADLESS_ARTIFACT";

    /// <summary>Gets whether this test process is isolated for real rendered-frame capture.</summary>
    internal static bool UsesRenderedArtifactRenderer => string.Equals(
        Environment.GetEnvironmentVariable(RenderedArtifactEnvironmentVariable),
        "1",
        StringComparison.Ordinal);

    /// <summary>Applies the same application theme used by the presentation controls under test.</summary>
    public override void Initialize()
    {
        App.ApplyTheme(this, ApplicationThemeFamily.Fluent, ApplicationThemeMode.Dark);
    }

    /// <summary>Creates the isolated fake-renderer test application or the explicitly requested Skia artifact renderer.</summary>
    /// <returns>The configured headless Avalonia application builder.</returns>
    public static AppBuilder BuildAvaloniaApp()
    {
        var builder = AppBuilder.Configure<HeadlessTestApp>();
        if (UsesRenderedArtifactRenderer)
        {
            return builder
                .UseSkia()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions
                {
                    UseHeadlessDrawing = false
                });
        }

        return builder.UseHeadless(new AvaloniaHeadlessPlatformOptions
        {
            UseHeadlessDrawing = true
        });
    }
}
