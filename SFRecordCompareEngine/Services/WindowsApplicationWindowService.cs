using Microsoft.UI.Windowing;
using Microsoft.UI;
using SFRecordCompareEngine.Services.Interfaces;
using WinRT.Interop;

namespace SFRecordCompareEngine.Services;

public class WindowsApplicationWindowService : IApplicationWindowService
{
    public void MaximizeMainWindow()
    {
        var mauiWindow = Application.Current?.Windows.FirstOrDefault();
        var platformWindow = mauiWindow?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (platformWindow == null) return;

        var windowId = Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(platformWindow));
        var appWindow = AppWindow.GetFromWindowId(windowId);

        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.Maximize();
        }
    }
}
