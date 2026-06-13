using System.Diagnostics;
using Autofac;
using CreationsForge.Bethesda.Assets.Files;
using CreationsForge.Bethesda.Assets.Temp;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.Services;

public class ExternalAssetOpenService : IExternalAssetOpenService, IDisposable
{
    private readonly IApplicationConfigurationStore ConfigurationStore;
    private readonly ILifetimeScope LifetimeScope;
    private readonly IAssetTempFileSession TempFileSession;
    private readonly ILogger Logger;

    public ExternalAssetOpenService(
        IApplicationConfigurationStore configurationStore,
        ILifetimeScope lifetimeScope,
        ILogger logger)
    {
        ConfigurationStore = configurationStore;
        LifetimeScope = lifetimeScope;
        TempFileSession = new AssetTempFileSession();
        Logger = logger.ForContext<ExternalAssetOpenService>();
    }

    public bool OpenExternally(AssetPreviewCandidateDTO candidate)
    {
        if (!OperatingSystem.IsWindows())
        {
            Logger.Warning("NifSkope external open is only available on Windows");
            return false;
        }

        var assetPath = ResolveLocalAssetPath(candidate);
        if (string.IsNullOrWhiteSpace(assetPath))
        {
            return false;
        }

        var nifSkopePath = ConfigurationStore.Current.NifSkopeExecutablePath;
        return string.IsNullOrWhiteSpace(nifSkopePath)
            ? OpenWithShellAssociation(assetPath)
            : OpenWithNifSkope(nifSkopePath, assetPath);
    }

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

    public void Dispose()
    {
        TempFileSession.Dispose();
    }

    private string? ResolveLocalAssetPath(AssetPreviewCandidateDTO candidate)
    {
        using var scope = LifetimeScope.BeginLifetimeScope();
        var assetFileResolverService = scope.Resolve<IAssetFileResolverService>();
        var resolution = assetFileResolverService.ResolveAssetFile(candidate);
        if (resolution.Status == AssetFileResolutionStatus.ResolvedLooseFile && !string.IsNullOrWhiteSpace(resolution.ResolvedPath))
        {
            return resolution.ResolvedPath;
        }

        if (resolution.Status == AssetFileResolutionStatus.ResolvedArchiveEntryInMemory && resolution.Data != null)
        {
            return WriteArchiveBackedAssetToTempFile(candidate, resolution);
        }

        Logger.Warning(
            "Unable to resolve local NifSkope asset path for {MeshPath}: {StatusMessage}",
            candidate.MeshPath,
            resolution.StatusMessage);
        return null;
    }

    private string? WriteArchiveBackedAssetToTempFile(AssetPreviewCandidateDTO candidate, AssetFileResolutionDTO resolution)
    {
        try
        {
            var directory = TempFileSession.CreateExtractionDirectory($"{candidate.Game}_{candidate.RecordType}_{candidate.FormKey.Id}");
            var fileName = GetSafeFileName(resolution.NormalizedEntryPath ?? candidate.MeshPath);
            var filePath = Path.Combine(directory, fileName);
            File.WriteAllBytes(filePath, resolution.Data ?? []);
            Logger.Information(
                "Wrote archive-backed NIF {MeshPath} to temp path {TempPath} for NifSkope",
                candidate.MeshPath,
                filePath);
            return filePath;
        }
        catch (Exception exception)
        {
            Logger.Warning(
                exception,
                "Unable to write archive-backed NIF {MeshPath} to a temp path for NifSkope",
                candidate.MeshPath);
            return null;
        }
    }

    private bool OpenWithShellAssociation(string assetPath)
    {
        if (!IsSafeExistingFilePath(assetPath))
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

    private bool OpenWithNifSkope(string nifSkopePath, string assetPath)
    {
        if (!IsSafeExistingFilePath(nifSkopePath))
        {
            Logger.Warning("Cannot open asset path {AssetPath} because configured NifSkope executable path {NifSkopePath} is unsafe or missing", assetPath, nifSkopePath);
            return false;
        }

        if (!IsSafeExistingFilePath(assetPath))
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

    private static bool IsSafeExistingFilePath(string path)
    {
        return !string.IsNullOrWhiteSpace(path) &&
            Path.IsPathRooted(path) &&
            (!Uri.TryCreate(path, UriKind.Absolute, out var uri) || uri.IsFile) &&
            File.Exists(path);
    }

    private static string GetSafeFileName(string path)
    {
        var fileName = Path.GetFileName(path);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = "Preview.nif";
        }

        var safeFileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrWhiteSpace(Path.GetExtension(safeFileName))
            ? safeFileName + ".nif"
            : safeFileName;
    }
}
