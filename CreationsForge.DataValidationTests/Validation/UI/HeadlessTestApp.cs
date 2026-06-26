using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using CreationsForge;
using CreationsForge.Core.Models.Configuration;

[assembly: AvaloniaTestApplication(typeof(CreationsForge.DataValidationTests.Validation.UI.HeadlessTestApp))]

namespace CreationsForge.DataValidationTests.Validation.UI;

/// <summary>
/// Provides the Avalonia application bootstrap used by headless data-validation UI tests.
/// </summary>
public class HeadlessTestApp : Application
{
    /// <summary>
    /// Applies the same base theme resources used by the desktop comparison UI.
    /// </summary>
    public override void Initialize()
    {
        App.ApplyTheme(this, ApplicationThemeFamily.Fluent, ApplicationThemeMode.Dark);
    }

    /// <summary>
    /// Builds the headless Avalonia app consumed by <c>AvaloniaFact</c> tests.
    /// </summary>
    /// <returns>The configured headless Avalonia app builder.</returns>
    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<HeadlessTestApp>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}
