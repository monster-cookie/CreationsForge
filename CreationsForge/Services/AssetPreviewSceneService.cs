using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.Services;

public class AssetPreviewSceneService : IAssetPreviewSceneService
{
    public AssetPreviewModelDTO CreateSamplePreview(AssetPreviewCandidateDTO candidate)
    {
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
