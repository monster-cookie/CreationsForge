using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.Services;

public class AssetPreviewSceneService : IAssetPreviewSceneService
{
    private const string FallbackMaterialName = "PreviewFallback";
    private readonly IReadOnlyList<IAssetPreviewGeometryReader> GeometryReaders;
    private readonly ILogger Logger;

    public AssetPreviewSceneService(IEnumerable<IAssetPreviewGeometryReader> geometryReaders, ILogger logger)
    {
        GeometryReaders = geometryReaders.ToList();
        Logger = logger;
    }

    public AssetPreviewModelDTO CreatePreview(AssetPreviewCandidateDTO candidate, out string statusMessage)
    {
        var fallbackReasons = new List<string>();
        foreach (var geometryReader in GeometryReaders)
        {
            if (geometryReader.TryRead(candidate, out var previewModel, out var readerStatusMessage) && previewModel != null)
            {
                statusMessage = readerStatusMessage;
                return previewModel;
            }

            if (!string.IsNullOrWhiteSpace(readerStatusMessage))
            {
                fallbackReasons.Add(readerStatusMessage);
                Logger.Information("Asset preview geometry reader skipped {MeshPath}: {StatusMessage}", candidate.MeshPath, readerStatusMessage);
            }
        }

        statusMessage = CreateFallbackStatus(candidate, fallbackReasons);
        Logger.Information(
            "Asset preview using fallback geometry for {MeshPath}: {StatusMessage}",
            candidate.MeshPath,
            statusMessage);
        return new AssetPreviewModelDTO
        {
            DisplayName = candidate.DisplayName,
            SourcePath = candidate.MeshPath,
            Meshes =
            {
                new AssetPreviewMeshDTO
                {
                    Name = "Preview fallback stop sign",
                    MaterialName = FallbackMaterialName,
                    Vertices =
                    {
                        CreateVertex(0f, 0f, 0f, 0.5f, 0.5f),
                        CreateVertex(-0.35f, 0.85f, 0f, 0.3f, 1f),
                        CreateVertex(0.35f, 0.85f, 0f, 0.7f, 1f),
                        CreateVertex(0.85f, 0.35f, 0f, 1f, 0.7f),
                        CreateVertex(0.85f, -0.35f, 0f, 1f, 0.3f),
                        CreateVertex(0.35f, -0.85f, 0f, 0.7f, 0f),
                        CreateVertex(-0.35f, -0.85f, 0f, 0.3f, 0f),
                        CreateVertex(-0.85f, -0.35f, 0f, 0f, 0.3f),
                        CreateVertex(-0.85f, 0.35f, 0f, 0f, 0.7f)
                    },
                    Indices =
                    {
                        0,
                        1,
                        2,
                        0,
                        2,
                        3,
                        0,
                        3,
                        4,
                        0,
                        4,
                        5,
                        0,
                        5,
                        6,
                        0,
                        6,
                        7,
                        0,
                        7,
                        8,
                        0,
                        8,
                        1
                    }
                }
            }
        };
    }

    private static string CreateFallbackStatus(AssetPreviewCandidateDTO candidate, IReadOnlyList<string> fallbackReasons)
    {
        if (!Path.IsPathRooted(candidate.MeshPath))
        {
            return fallbackReasons.Count == 0
                ? $"Preview fallback for archive-backed asset path {candidate.MeshPath}. No preview geometry reader handled this asset."
                : $"Preview fallback for archive-backed asset path {candidate.MeshPath}. {fallbackReasons[0]}";
        }

        if (fallbackReasons.Count == 0)
        {
            return $"Preview fallback for {candidate.MeshPath}. No preview geometry reader handled this asset.";
        }

        return $"Preview fallback for {candidate.MeshPath}. {fallbackReasons[0]}";
    }

    private static AssetPreviewVertexDTO CreateVertex(float x, float y, float z, float u, float v)
    {
        return new AssetPreviewVertexDTO
        {
            Position = new AssetPreviewVector3DTO
            {
                X = x,
                Y = y,
                Z = z
            },
            Normal = new AssetPreviewVector3DTO
            {
                X = 0f,
                Y = 0f,
                Z = 1f
            },
            UV = new AssetPreviewUVDTO
            {
                U = u,
                V = v
            }
        };
    }
}
