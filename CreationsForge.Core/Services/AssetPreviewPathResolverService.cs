using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using Serilog;

namespace CreationsForge.Core.Services;

public class AssetPreviewPathResolverService : IAssetPreviewPathResolverService
{
    private static readonly HashSet<string> ExternalOpenExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".nif",
        ".obj",
        ".fbx",
        ".dae",
        ".gltf",
        ".glb"
    };

    private static readonly HashSet<string> PreviewExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".nif",
        ".obj",
        ".fbx",
        ".dae",
        ".gltf",
        ".glb"
    };

    private readonly IModelRepository ModelRepository;
    private readonly IReadOnlyList<IGameMetadataService> GameMetadataServices;
    private readonly ILogger Logger = Log.ForContext<AssetPreviewPathResolverService>();

    public AssetPreviewPathResolverService(
        IModelRepository modelRepository,
        IEnumerable<IGameMetadataService>? gameMetadataServices = null)
    {
        ModelRepository = modelRepository;
        GameMetadataServices = gameMetadataServices?.ToList() ?? [];
    }

    public IReadOnlyList<AssetPreviewCandidateDTO> GetPreviewCandidates(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        return ModelRepository.GetByFormKey(game, recordType, formKey)
            .Where(model => !string.IsNullOrWhiteSpace(model.File))
            .Select(CreateCandidate)
            .GroupBy(candidate => candidate.MeshPath, StringComparer.OrdinalIgnoreCase)
            .Select(group => group
                .OrderByDescending(candidate => candidate.CanPreview)
                .ThenByDescending(candidate => candidate.CanOpenExternally)
                .First())
            .ToList();
    }

    public bool CanPreviewPath(string? meshPath)
    {
        return HasSupportedExtension(meshPath, PreviewExtensions);
    }

    public bool CanOpenExternally(string? meshPath)
    {
        return HasSupportedExtension(meshPath, ExternalOpenExtensions) && !IsUnsafeExternalPath(meshPath);
    }

    public string? ResolveExternalOpenPath(AssetPreviewCandidateDTO candidate)
    {
        if (!CanOpenExternally(candidate.MeshPath))
        {
            Logger.Warning(
                "Rejecting external asset open for unsafe or unsupported path {MeshPath} in {Game}",
                candidate.MeshPath,
                candidate.Game);
            return null;
        }

        var dataFolder = GetDataFolder(candidate.Game);
        if (string.IsNullOrWhiteSpace(dataFolder) || !Directory.Exists(dataFolder))
        {
            Logger.Warning(
                "Rejecting external asset open for {MeshPath} in {Game}; data folder is unavailable",
                candidate.MeshPath,
                candidate.Game);
            return null;
        }

        var fullDataFolder = Path.GetFullPath(dataFolder);
        foreach (var relativePath in GetRelativePathCandidates(candidate.MeshPath))
        {
            var resolvedPath = Path.GetFullPath(Path.Combine(fullDataFolder, relativePath));
            if (!IsUnderDirectory(resolvedPath, fullDataFolder))
            {
                continue;
            }

            if (File.Exists(resolvedPath))
            {
                return resolvedPath;
            }
        }

        Logger.Warning(
            "Rejecting external asset open for {MeshPath} in {Game}; no matching loose file was found under {DataFolder}",
            candidate.MeshPath,
            candidate.Game,
            fullDataFolder);
        return null;
    }

    private AssetPreviewCandidateDTO CreateCandidate(ModelDTO model)
    {
        var meshPath = NormalizeMeshPath(model.File ?? string.Empty);
        var canPreview = CanPreviewPath(meshPath);
        var canOpenExternally = CanOpenExternally(meshPath);
        return new AssetPreviewCandidateDTO
        {
            Game = model.Game,
            ModKey = model.ModKey,
            RecordType = model.RecordType,
            FormKey = model.FormKey,
            ModelSlot = model.ModelSlot,
            ModelGender = model.ModelGender,
            MeshPath = meshPath,
            DisplayName = GetDisplayName(model, meshPath),
            CanPreview = canPreview,
            CanOpenExternally = canOpenExternally,
            UnsupportedReason = canPreview
                ? null
                : "No experimental preview renderer is registered for this asset file type."
        };
    }

    private static string NormalizeMeshPath(string meshPath)
    {
        return meshPath.Trim()
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
    }

    private string? GetDataFolder(SupportedGame game)
    {
        var metadataService = GameMetadataServices.FirstOrDefault(service => service.Game == game);
        return metadataService?.GetGame().DataFolder;
    }

    private static IReadOnlyList<string> GetRelativePathCandidates(string meshPath)
    {
        var normalizedPath = NormalizeMeshPath(meshPath);
        var candidates = new List<string>
        {
            normalizedPath
        };

        if (!StartsWithDirectory(normalizedPath, "Meshes"))
        {
            AddPathCandidate(candidates, Path.Combine("Meshes", normalizedPath));
        }

        return candidates;
    }

    private static void AddPathCandidate(List<string> candidates, string path)
    {
        if (!string.IsNullOrWhiteSpace(path) &&
            !candidates.Contains(path, StringComparer.OrdinalIgnoreCase))
        {
            candidates.Add(path);
        }
    }

    private static bool StartsWithDirectory(string path, string directoryName)
    {
        return string.Equals(path, directoryName, StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith(directoryName + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsUnsafeExternalPath(string? meshPath)
    {
        if (string.IsNullOrWhiteSpace(meshPath))
        {
            return true;
        }

        var trimmedPath = meshPath.Trim();
        return Uri.TryCreate(trimmedPath, UriKind.Absolute, out var uri) && !uri.IsFile ||
            Path.IsPathRooted(trimmedPath) ||
            trimmedPath.StartsWith(@"\\", StringComparison.Ordinal) ||
            trimmedPath.StartsWith(@"\\?\", StringComparison.Ordinal) ||
            NormalizeMeshPath(trimmedPath).Split(Path.DirectorySeparatorChar).Any(part => part == "..");
    }

    private static bool IsUnderDirectory(string path, string directory)
    {
        var normalizedPath = Path.GetFullPath(path);
        var normalizedDirectory = Path.GetFullPath(directory);
        if (!normalizedDirectory.EndsWith(Path.DirectorySeparatorChar))
        {
            normalizedDirectory += Path.DirectorySeparatorChar;
        }

        return normalizedPath.StartsWith(normalizedDirectory, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetDisplayName(ModelDTO model, string meshPath)
    {
        var slot = string.IsNullOrWhiteSpace(model.ModelGender)
            ? model.ModelSlot
            : $"{model.ModelSlot} ({model.ModelGender})";
        var fileName = Path.GetFileName(meshPath);
        return string.IsNullOrWhiteSpace(fileName)
            ? slot
            : $"{slot}: {fileName}";
    }

    private static bool HasSupportedExtension(string? meshPath, ISet<string> supportedExtensions)
    {
        if (string.IsNullOrWhiteSpace(meshPath))
        {
            return false;
        }

        return supportedExtensions.Contains(Path.GetExtension(meshPath.Trim()));
    }
}
