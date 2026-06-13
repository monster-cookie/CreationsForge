using Avalonia;
using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using CreationsForge.Core.Models.Configuration;

[assembly: AvaloniaTestApplication(typeof(CreationsForge.PresentationTests.Headless.HeadlessTestApp))]

namespace CreationsForge.PresentationTests.Headless;

public class HeadlessTestApp : Application
{
    public override void Initialize()
    {
        App.ApplyTheme(this, ApplicationThemeFamily.Fluent, ApplicationThemeMode.Dark);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<HeadlessTestApp>()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());
    }
}
