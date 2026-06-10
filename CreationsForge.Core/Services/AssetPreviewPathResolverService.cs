using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

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

    public AssetPreviewPathResolverService(IModelRepository modelRepository)
    {
        ModelRepository = modelRepository;
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
        return HasSupportedExtension(meshPath, ExternalOpenExtensions);
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
