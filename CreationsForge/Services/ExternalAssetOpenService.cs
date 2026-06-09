using System.Diagnostics;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.Services;

public class ExternalAssetOpenService : IExternalAssetOpenService
{
    private readonly ILogger Logger;

    public ExternalAssetOpenService(ILogger logger)
    {
        Logger = logger.ForContext<ExternalAssetOpenService>();
    }

    public bool OpenExternally(string assetPath)
    {
        if (string.IsNullOrWhiteSpace(assetPath))
        {
            Logger.Warning("Cannot open an empty asset path externally");
            return false;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = assetPath,
                UseShellExecute = true
            });
            return true;
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Unable to open asset path {AssetPath} externally", assetPath);
            return false;
        }
    }
}
