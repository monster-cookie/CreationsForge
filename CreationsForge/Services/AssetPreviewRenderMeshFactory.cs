using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.Services;

public class AssetPreviewRenderMeshFactory : IAssetPreviewRenderMeshFactory
{
    private readonly ILogger Logger;

    public AssetPreviewRenderMeshFactory(ILogger logger)
    {
        Logger = logger.ForContext<AssetPreviewRenderMeshFactory>();
    }

    public AssetPreviewRenderMesh CreateRenderMesh(AssetPreviewModelDTO? previewModel)
    {
        if (previewModel is null || previewModel.Meshes.Count == 0)
        {
            return CreateFallbackMesh();
        }

        var renderMesh = new AssetPreviewRenderMesh();
        foreach (var mesh in previewModel.Meshes)
        {
            AppendMesh(renderMesh, mesh);
        }

        return renderMesh.Indices.Count == 0
            ? CreateFallbackMesh()
            : renderMesh;
    }

    private void AppendMesh(AssetPreviewRenderMesh renderMesh, AssetPreviewMeshDTO mesh)
    {
        var baseVertex = (uint)(renderMesh.Vertices.Count / 6);
        foreach (var vertex in mesh.Vertices)
        {
            renderMesh.Vertices.Add(vertex.Position.X);
            renderMesh.Vertices.Add(vertex.Position.Y);
            renderMesh.Vertices.Add(vertex.Position.Z);
            renderMesh.Vertices.Add(0.30f);
            renderMesh.Vertices.Add(0.65f);
            renderMesh.Vertices.Add(1.00f);
        }

        for (var index = 0; index + 2 < mesh.Indices.Count; index += 3)
        {
            if (!TryAppendTriangle(renderMesh, mesh, baseVertex, index))
            {
                Logger.Warning(
                    "Skipping invalid asset preview triangle in mesh {MeshName} at index {TriangleIndex}",
                    mesh.Name,
                    index);
            }
        }
    }

    private static bool TryAppendTriangle(
        AssetPreviewRenderMesh renderMesh,
        AssetPreviewMeshDTO mesh,
        uint baseVertex,
        int triangleIndex)
    {
        var first = mesh.Indices[triangleIndex];
        var second = mesh.Indices[triangleIndex + 1];
        var third = mesh.Indices[triangleIndex + 2];
        if (!IsValidIndex(mesh, first) || !IsValidIndex(mesh, second) || !IsValidIndex(mesh, third))
        {
            return false;
        }

        renderMesh.Indices.Add(baseVertex + (uint)first);
        renderMesh.Indices.Add(baseVertex + (uint)second);
        renderMesh.Indices.Add(baseVertex + (uint)third);
        return true;
    }

    private static bool IsValidIndex(AssetPreviewMeshDTO mesh, int index)
    {
        return index >= 0 && index < mesh.Vertices.Count;
    }

    private static AssetPreviewRenderMesh CreateFallbackMesh()
    {
        return new AssetPreviewRenderMesh
        {
            Vertices =
            {
                -0.9f,
                -0.6f,
                0f,
                0.30f,
                0.65f,
                1.00f,
                0.9f,
                -0.6f,
                0f,
                0.30f,
                0.65f,
                1.00f,
                0f,
                0.9f,
                0f,
                0.30f,
                0.65f,
                1.00f,
                0f,
                0f,
                1.1f,
                1.00f,
                0.85f,
                0.25f
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
        };
    }
}
