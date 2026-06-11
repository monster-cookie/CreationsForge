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
            Data = resolution.Data,
            ResolveExternalAsset = externalAssetPath => ResolveExternalAsset(candidate, externalAssetPath)
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

        previewModel = MapModel(candidate, readResult.Model, out var loadedTextureCount);
        statusMessage = GetStatusMessage(readResult.StatusMessage, previewModel);
        if (loadedTextureCount > 0)
        {
            statusMessage = $"{statusMessage} Loaded {loadedTextureCount:N0} preview texture(s).";
        }

        return true;
    }

    private byte[]? ResolveExternalAsset(AssetPreviewCandidateDTO candidate, string assetPath)
    {
        var resolution = AssetFileResolverService.ResolveAssetFile(new AssetPreviewCandidateDTO
        {
            Game = candidate.Game,
            ModKey = candidate.ModKey,
            RecordType = candidate.RecordType,
            FormKey = candidate.FormKey,
            ModelSlot = candidate.ModelSlot,
            ModelGender = candidate.ModelGender,
            MeshPath = assetPath,
            DisplayName = assetPath,
            CanPreview = true,
            CanOpenExternally = false
        });

        if (resolution.Data != null && IsReadableResolution(resolution))
        {
            return resolution.Data;
        }

        Logger.Information(
            "NIF preview external geometry skipped for {MeshPath} external asset {ExternalAssetPath}: {StatusMessage}",
            candidate.MeshPath,
            assetPath,
            resolution.StatusMessage);
        return null;
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

            return $"{statusMessage} Textures were found.";
    }

    private AssetPreviewModelDTO MapModel(AssetPreviewCandidateDTO candidate, NifPreviewModel model, out int loadedTextureCount)
    {
        loadedTextureCount = 0;
        var previewModel = new AssetPreviewModelDTO
        {
            DisplayName = model.DisplayName,
            SourcePath = model.SourcePath
        };

        foreach (var mesh in model.Meshes)
        {
            previewModel.Meshes.Add(MapMesh(candidate, mesh, ref loadedTextureCount));
        }

        return previewModel;
    }

    private AssetPreviewMeshDTO MapMesh(AssetPreviewCandidateDTO candidate, NifPreviewMesh mesh, ref int loadedTextureCount)
    {
        var previewMesh = new AssetPreviewMeshDTO
        {
            Name = mesh.Name,
            MaterialName = mesh.MaterialName,
            TexturePath = mesh.TexturePath
        };
        if (!string.IsNullOrWhiteSpace(mesh.TexturePath))
        {
            var textureResolution = AssetFileResolverService.ResolveAssetFile(new AssetPreviewCandidateDTO
            {
                Game = candidate.Game,
                ModKey = candidate.ModKey,
                RecordType = candidate.RecordType,
                FormKey = candidate.FormKey,
                ModelSlot = "Texture",
                ModelGender = candidate.ModelGender,
                MeshPath = mesh.TexturePath,
                DisplayName = mesh.TexturePath,
                CanPreview = true,
                CanOpenExternally = false
            });
            if (textureResolution.Data != null && IsReadableResolution(textureResolution))
            {
                previewMesh.Texture = new AssetPreviewTextureDTO
                {
                    Path = textureResolution.NormalizedEntryPath ?? textureResolution.ResolvedPath ?? mesh.TexturePath,
                    Data = textureResolution.Data
                };
                loadedTextureCount++;
            }
            else
            {
                Logger.Information(
                    "NIF preview texture skipped for {MeshName} texture {TexturePath}: {StatusMessage}",
                    mesh.Name,
                    mesh.TexturePath,
                    textureResolution.StatusMessage);
            }
        }

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
