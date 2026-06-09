using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.Services;

public class AssetPreviewSceneService : IAssetPreviewSceneService
{
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
            "Asset preview using sample placeholder for {MeshPath}: {StatusMessage}",
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
                    Name = "Experimental sample mesh",
                    MaterialName = candidate.ModelSlot,
                    Vertices =
                    {
                        CreateVertex(-0.9f, -0.6f, 0f, 0f, 0f),
                        CreateVertex(0.9f, -0.6f, 0f, 1f, 0f),
                        CreateVertex(0f, 0.9f, 0f, 0.5f, 1f),
                        CreateVertex(0f, 0f, 1.1f, 0.5f, 0.5f)
                    },
                    Indices =
                    {
                        0,
                        1,
                        2,
                        0,
                        3,
                        1,
                        1,
                        3,
                        2,
                        2,
                        3,
                        0
                    }
                }
            }
        };
    }

    private static string CreateFallbackStatus(AssetPreviewCandidateDTO candidate, IReadOnlyList<string> fallbackReasons)
    {
        if (!Path.IsPathRooted(candidate.MeshPath))
        {
            return $"Sample placeholder for archive-backed asset path {candidate.MeshPath}. BA2/BSA extraction is not implemented yet.";
        }

        if (fallbackReasons.Count == 0)
        {
            return $"Sample placeholder for {candidate.MeshPath}. No preview geometry reader handled this asset.";
        }

        return $"Sample placeholder for {candidate.MeshPath}. {fallbackReasons[0]}";
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
