using System.Collections;
using System.Numerics;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.Services;

public class NiflyAssetPreviewGeometryReader : IAssetPreviewGeometryReader
{
    private readonly ILogger Logger;
    private readonly IAssetFileResolverService AssetFileResolverService;

    public NiflyAssetPreviewGeometryReader(IAssetFileResolverService assetFileResolverService, ILogger logger)
    {
        AssetFileResolverService = assetFileResolverService;
        Logger = logger;
    }

    public bool TryRead(AssetPreviewCandidateDTO candidate, out AssetPreviewModelDTO? previewModel, out string statusMessage)
    {
        previewModel = null;
        statusMessage = string.Empty;
        if (!string.Equals(Path.GetExtension(candidate.MeshPath), ".nif", StringComparison.OrdinalIgnoreCase))
        {
            statusMessage = "Nifly reader supports NIF files only.";
            return false;
        }

        var resolution = AssetFileResolverService.ResolveAssetFile(candidate);
        if (!resolution.IsResolved || string.IsNullOrWhiteSpace(resolution.ResolvedPath))
        {
            statusMessage = resolution.StatusMessage;
            Logger.Information(
                "Asset preview NIF file could not be resolved for {MeshPath}: {StatusMessage}",
                candidate.MeshPath,
                statusMessage);
            return false;
        }

        try
        {
            return TryReadNif(candidate, resolution.ResolvedPath, out previewModel, out statusMessage);
        }
        catch (Exception ex)
        {
            statusMessage = "Nifly failed to read this NIF.";
            Logger.Warning(ex, "Nifly failed to read asset preview NIF: {MeshPath}", resolution.ResolvedPath);
            return false;
        }
    }

    private bool TryReadNif(AssetPreviewCandidateDTO candidate, string resolvedPath, out AssetPreviewModelDTO? previewModel, out string statusMessage)
    {
        previewModel = null;
        statusMessage = string.Empty;
        var nifFileType = Type.GetType("NiflySharp.NifFile, NiflySharp");
        if (nifFileType == null)
        {
            statusMessage = "NiflySharp.NifFile type was not available.";
            Logger.Warning("NiflySharp.NifFile type was not available for asset preview.");
            return false;
        }

        var nifFile = Activator.CreateInstance(nifFileType);
        if (nifFile == null)
        {
            statusMessage = "Nifly file object could not be created.";
            Logger.Warning("Nifly file object could not be created for asset preview.");
            return false;
        }

        var loadMethod = nifFileType.GetMethod("Load", new[] { typeof(string), Type.GetType("NiflySharp.NifFileLoadOptions, NiflySharp")! });
        if (loadMethod == null)
        {
            statusMessage = "Nifly load API was not available.";
            Logger.Warning("Nifly load API was not available for asset preview.");
            return false;
        }

        var loadOptionsType = Type.GetType("NiflySharp.NifFileLoadOptions, NiflySharp");
        var loadOptions = loadOptionsType == null ? null : Activator.CreateInstance(loadOptionsType);
        loadMethod.Invoke(nifFile, new[] { resolvedPath, loadOptions });
        if (nifFileType.GetProperty("Valid")?.GetValue(nifFile) is false)
        {
            statusMessage = "Nifly did not consider this NIF valid.";
            Logger.Warning("Nifly did not consider asset preview NIF valid: {MeshPath}", resolvedPath);
            return false;
        }

        var shapes = nifFileType.GetMethod("GetShapes")?.Invoke(nifFile, Array.Empty<object>());
        if (shapes is not IEnumerable shapeEnumerable)
        {
            statusMessage = "Nifly did not expose previewable shapes.";
            Logger.Warning("Nifly did not expose shapes for asset preview NIF: {MeshPath}", resolvedPath);
            return false;
        }

        var model = new AssetPreviewModelDTO
        {
            DisplayName = candidate.DisplayName,
            SourcePath = resolvedPath
        };

        foreach (var shape in shapeEnumerable)
        {
            var mesh = CreateMesh(candidate, shape);
            if (mesh != null)
            {
                model.Meshes.Add(mesh);
            }
        }

        if (model.Meshes.Count == 0)
        {
            statusMessage = "Nifly found no triangle meshes that Creations Forge can preview yet.";
            Logger.Warning("Nifly found no previewable triangle meshes in asset preview NIF: {MeshPath}", resolvedPath);
            return false;
        }

        statusMessage = "Nifly preview geometry loaded.";
        Logger.Information("Loaded asset preview NIF geometry from {MeshPath} with {MeshCount} meshes", resolvedPath, model.Meshes.Count);
        previewModel = model;
        return true;
    }

    private AssetPreviewMeshDTO? CreateMesh(AssetPreviewCandidateDTO candidate, object shape)
    {
        var positions = ReadVector3List(ReadMember(shape, "VertexPositions") ?? ReadNestedMember(shape, "GeometryData", "_vertices"));
        if (positions.Count == 0)
        {
            return null;
        }

        var indices = ReadTriangleIndices(ReadMember(shape, "Triangles") ?? ReadNestedMember(shape, "GeometryData", "_triangles"));
        if (indices.Count == 0)
        {
            return null;
        }

        var normals = ReadVector3List(ReadMember(shape, "Normals") ?? ReadNestedMember(shape, "GeometryData", "_normals"));
        var uvs = ReadUVList(ReadMember(shape, "UVs") ?? ReadNestedMember(shape, "GeometryData", "_uVSets"));
        var mesh = new AssetPreviewMeshDTO
        {
            Name = ReadShapeName(shape) ?? "NIF mesh",
            MaterialName = candidate.ModelSlot
        };

        for (var i = 0; i < positions.Count; i++)
        {
            var normal = i < normals.Count ? normals[i] : Vector3.UnitZ;
            var uv = i < uvs.Count ? uvs[i] : default;
            mesh.Vertices.Add(new AssetPreviewVertexDTO
            {
                Position = new AssetPreviewVector3DTO
                {
                    X = positions[i].X,
                    Y = positions[i].Y,
                    Z = positions[i].Z
                },
                Normal = new AssetPreviewVector3DTO
                {
                    X = normal.X,
                    Y = normal.Y,
                    Z = normal.Z
                },
                UV = new AssetPreviewUVDTO
                {
                    U = uv.X,
                    V = uv.Y
                }
            });
        }

        foreach (var index in indices)
        {
            if (index < positions.Count)
            {
                mesh.Indices.Add(index);
            }
        }

        return mesh.Indices.Count == 0 ? null : mesh;
    }

    private static object? ReadNestedMember(object source, string containerName, string memberName)
    {
        var container = ReadMember(source, containerName);
        return container == null ? null : ReadMember(container, memberName);
    }

    private static object? ReadMember(object source, string memberName)
    {
        var type = source.GetType();
        return type.GetProperty(memberName)?.GetValue(source) ??
            type.GetField(memberName)?.GetValue(source) ??
            type.GetField(memberName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.GetValue(source);
    }

    private static string? ReadShapeName(object shape)
    {
        var name = ReadMember(shape, "Name") ?? ReadMember(shape, "_name");
        return name?.ToString();
    }

    private static List<Vector3> ReadVector3List(object? value)
    {
        var vectors = new List<Vector3>();
        if (value is not IEnumerable enumerable)
        {
            return vectors;
        }

        foreach (var item in enumerable)
        {
            if (item is Vector3 vector)
            {
                vectors.Add(vector);
            }
        }

        return vectors;
    }

    private static List<Vector2> ReadUVList(object? value)
    {
        if (value is IEnumerable outerEnumerable && value is not string)
        {
            var first = true;
            var uvs = new List<Vector2>();
            foreach (var item in outerEnumerable)
            {
                if (item is Vector3 vector)
                {
                    uvs.Add(new Vector2(vector.X, vector.Y));
                    continue;
                }

                if (first && item is IEnumerable nestedEnumerable)
                {
                    return ReadUVList(nestedEnumerable);
                }

                first = false;
            }

            return uvs;
        }

        return new List<Vector2>();
    }

    private static List<int> ReadTriangleIndices(object? value)
    {
        var indices = new List<int>();
        if (value is not IEnumerable enumerable)
        {
            return indices;
        }

        foreach (var triangle in enumerable)
        {
            AddTriangleIndex(indices, triangle, "V1");
            AddTriangleIndex(indices, triangle, "V2");
            AddTriangleIndex(indices, triangle, "V3");
        }

        return indices;
    }

    private static void AddTriangleIndex(ICollection<int> indices, object? triangle, string memberName)
    {
        if (triangle == null)
        {
            return;
        }

        var value = ReadMember(triangle, memberName);
        if (value == null)
        {
            return;
        }

        indices.Add(Convert.ToInt32(value));
    }
}
