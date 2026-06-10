using CreationsForge.Bethesda.Assets.Files;
using CreationsForge.Bethesda.Assets.Nif;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.Services;

public class BethesdaAssetPreviewGeometryReader : IAssetPreviewGeometryReader
{
    private readonly IAssetFileResolverService AssetFileResolverService;
    private readonly INifPreviewModelReader NifPreviewModelReader;
    private readonly ILogger Logger;

    public BethesdaAssetPreviewGeometryReader(
        IAssetFileResolverService assetFileResolverService,
        INifPreviewModelReader nifPreviewModelReader,
        ILogger logger)
    {
        AssetFileResolverService = assetFileResolverService;
        NifPreviewModelReader = nifPreviewModelReader;
        Logger = logger.ForContext<BethesdaAssetPreviewGeometryReader>();
    }

    public bool TryRead(AssetPreviewCandidateDTO candidate, out AssetPreviewModelDTO? previewModel, out string statusMessage)
    {
        previewModel = null;
        if (!candidate.MeshPath.EndsWith(".nif", StringComparison.OrdinalIgnoreCase))
        {
            statusMessage = "Only NIF preview parsing is supported.";
            return false;
        }

        var resolution = AssetFileResolverService.ResolveAssetFile(candidate);
        if (resolution.Data == null || !IsReadableResolution(resolution))
        {
            statusMessage = resolution.StatusMessage;
            return false;
        }

        var sourcePath = resolution.ResolvedPath ?? resolution.NormalizedEntryPath ?? candidate.MeshPath;
        var readResult = NifPreviewModelReader.TryRead(new NifPreviewReadRequest
        {
            SourcePath = sourcePath,
            DisplayName = candidate.DisplayName,
            Data = resolution.Data
        });

        if (!readResult.IsSuccess || readResult.Model == null)
        {
            statusMessage = readResult.StatusMessage;
            Logger.Information("NIF preview parser skipped {MeshPath}: {StatusMessage}", candidate.MeshPath, statusMessage);
            return false;
        }

        foreach (var diagnostic in readResult.Diagnostics)
        {
            Logger.Information("NIF preview parser diagnostic for {MeshPath}: {Diagnostic}", candidate.MeshPath, diagnostic);
        }

        previewModel = MapModel(readResult.Model);
        statusMessage = GetStatusMessage(readResult.StatusMessage, previewModel);
        return true;
    }

    private static bool IsReadableResolution(AssetFileResolutionDTO resolution)
    {
        return resolution.Status is AssetFileResolutionStatus.ResolvedLooseFile or AssetFileResolutionStatus.ResolvedArchiveEntryInMemory;
    }

    private static string GetStatusMessage(string statusMessage, AssetPreviewModelDTO previewModel)
    {
        var textureCount = previewModel.Meshes
            .Select(mesh => mesh.TexturePath)
            .Where(texturePath => !string.IsNullOrWhiteSpace(texturePath))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();
        if (textureCount == 0)
        {
            return statusMessage;
        }

        return $"{statusMessage} Textures were found but are not shown in the preview yet.";
    }

    private static AssetPreviewModelDTO MapModel(NifPreviewModel model)
    {
        var previewModel = new AssetPreviewModelDTO
        {
            DisplayName = model.DisplayName,
            SourcePath = model.SourcePath
        };

        foreach (var mesh in model.Meshes)
        {
            previewModel.Meshes.Add(MapMesh(mesh));
        }

        return previewModel;
    }

    private static AssetPreviewMeshDTO MapMesh(NifPreviewMesh mesh)
    {
        var previewMesh = new AssetPreviewMeshDTO
        {
            Name = mesh.Name,
            MaterialName = mesh.MaterialName,
            TexturePath = mesh.TexturePath
        };

        foreach (var vertex in mesh.Vertices)
        {
            previewMesh.Vertices.Add(new AssetPreviewVertexDTO
            {
                Position = MapVector(vertex.Position),
                Normal = MapVector(vertex.Normal),
                UV = new AssetPreviewUVDTO
                {
                    U = vertex.UV.U,
                    V = vertex.UV.V
                }
            });
        }

        foreach (var index in mesh.Indices)
        {
            previewMesh.Indices.Add(index);
        }

        return previewMesh;
    }

    private static AssetPreviewVector3DTO MapVector(NifPreviewVector3 vector)
    {
        return new AssetPreviewVector3DTO
        {
            X = vector.X,
            Y = vector.Y,
            Z = vector.Z
        };
    }
}
