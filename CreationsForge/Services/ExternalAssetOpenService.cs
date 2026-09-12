using System.Diagnostics;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.Services;

/// <summary>
/// Opens an existing local asset with NifSkope when configured, or with the Windows shell association otherwise.
/// </summary>
public class ExternalAssetOpenService : IExternalAssetOpenService
{
    /// <summary>
    /// Provides the configured NifSkope executable path.
    /// </summary>
    private readonly IApplicationConfigurationStore ConfigurationStore;

    /// <summary>
    /// Records failures that prevent an external viewer from opening the asset.
    /// </summary>
    private readonly ILogger Logger;

    /// <summary>
    /// Initializes a service that opens local asset files outside CreationsForge.
    /// </summary>
    /// <param name="configurationStore">The application configuration containing the optional NifSkope executable path.</param>
    /// <param name="logger">The logger used to report rejected paths and process-launch failures.</param>
    public ExternalAssetOpenService(
        IApplicationConfigurationStore configurationStore,
        ILogger logger)
    {
        ConfigurationStore = configurationStore;
        Logger = logger.ForContext<ExternalAssetOpenService>();
    }

    /// <inheritdoc />
    public bool OpenExternally(string assetPath)
    {
        if (!OperatingSystem.IsWindows())
        {
            Logger.Warning("NifSkope external open is only available on Windows");
            return false;
        }

        var nifSkopePath = ConfigurationStore.Current.NifSkopeExecutablePath;
        return string.IsNullOrWhiteSpace(nifSkopePath)
            ? OpenWithShellAssociation(assetPath)
            : OpenWithNifSkope(nifSkopePath, assetPath);
    }

    /// <summary>
    /// Opens a validated local asset path through its Windows shell association.
    /// </summary>
    /// <param name="assetPath">The absolute path of the existing local asset.</param>
    /// <returns><see langword="true" /> when process creation succeeds; otherwise, <see langword="false" />.</returns>
    private bool OpenWithShellAssociation(string assetPath)
    {
        if (!ExternalAssetPathPolicy.IsSafeExistingAssetPath(assetPath))
        {
            Logger.Warning("Cannot open unsafe or missing asset path {AssetPath} externally", assetPath);
            return false;
        }

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Path.GetFullPath(assetPath),
                UseShellExecute = true
            });
            return true;
        }
        catch (Exception exception)
        {
            Logger.Warning(exception, "Unable to open asset path {AssetPath} externally", assetPath);
            return false;
        }
    }

    /// <summary>
    /// Opens a validated local asset path with the configured NifSkope executable.
    /// </summary>
    /// <param name="nifSkopePath">The absolute path of the configured NifSkope executable.</param>
    /// <param name="assetPath">The absolute path of the existing local asset.</param>
    /// <returns><see langword="true" /> when process creation succeeds; otherwise, <see langword="false" />.</returns>
    private bool OpenWithNifSkope(string nifSkopePath, string assetPath)
    {
        if (!ExternalAssetPathPolicy.IsSafeExistingExecutablePath(nifSkopePath))
        {
            Logger.Warning("Cannot open asset path {AssetPath} because configured NifSkope executable path {NifSkopePath} is unsafe or missing", assetPath, nifSkopePath);
            return false;
        }

        if (!ExternalAssetPathPolicy.IsSafeExistingAssetPath(assetPath))
        {
            Logger.Warning("Cannot open unsafe or missing asset path {AssetPath} in NifSkope", assetPath);
            return false;
        }

        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = Path.GetFullPath(nifSkopePath),
                WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(nifSkopePath)) ?? string.Empty,
                UseShellExecute = false
            };
            processStartInfo.ArgumentList.Add(Path.GetFullPath(assetPath));
            Process.Start(processStartInfo);
            return true;
        }
        catch (Exception exception)
        {
            Logger.Warning(exception, "Unable to open asset path {AssetPath} with NifSkope executable {NifSkopePath}", assetPath, nifSkopePath);
            return false;
        }
    }
}
