using Avalonia.Headless.XUnit;
using CreationsForge.PresentationTests.Headless;
using CreationsForge.PresentationTests.Support;
using Shouldly;

namespace CreationsForge.PresentationTests.Views;

/// <summary>
/// Verifies the desktop root window owns registration without triggering navigation.
/// </summary>
[Collection(AvaloniaControlTestCollection.Name)]
public sealed class MainWindowTests
{
    /// <summary>Verifies construction registers the window for content and dialog ownership.</summary>
    [AvaloniaFact]
    public void Constructor_WithWindowService_RegistersMainWindow()
    {
        var windowService = new FakeApplicationWindowService();

        var mainWindow = new MainWindow(windowService);

        try
        {
            windowService.MainWindow.ShouldBeSameAs(mainWindow);
            mainWindow.Content.ShouldBeNull();
        }
        finally
        {
            mainWindow.Close();
        }
    }
}
