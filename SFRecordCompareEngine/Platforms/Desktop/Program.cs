using Uno.UI.Hosting;

namespace SFRecordCompareEngine;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseX11()
            .UseWin32()
            .Build();

        host.Run();
    }
}
