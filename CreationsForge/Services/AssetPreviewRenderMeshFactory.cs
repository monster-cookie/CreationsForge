using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.Services;

public class AssetPreviewRenderMeshFactory : IAssetPreviewRenderMeshFactory
{
    private const string FallbackMaterialName = "PreviewFallback";
    private const float TargetPreviewSize = 1.8f;
    private const float MaxReasonableCoordinate = 1000000f;
    private static readonly AssetPreviewRenderColor DefaultLoadedMeshColor = new(0.70f, 0.72f, 0.72f);
    private readonly ILogger Logger;

    public AssetPreviewRenderMeshFactory(ILogger logger)
    {
        Logger = logger.ForContext<AssetPreviewRenderMeshFactory>();
    }

    public AssetPreviewRenderMesh CreateRenderMesh(AssetPreviewModelDTO? previewModel)
    {
        return CreateRenderMesh(previewModel, new AssetPreviewRenderOptions());
    }

    public AssetPreviewRenderMesh CreateRenderMesh(AssetPreviewModelDTO? previewModel, AssetPreviewRenderOptions options)
    {
        if (previewModel is null || previewModel.Meshes.Count == 0)
        {
            Logger.Warning("Asset preview render mesh factory using fallback because no preview model meshes were provided");
            return CreateFallbackMesh();
        }

        var meshes = GetMeshes(previewModel, options)
            .Select(ToRenderSpace)
            .ToList();
        if (meshes.Count == 0)
        {
            Logger.Warning(
                "Asset preview model {DisplayName} has no mesh matching render filter {MeshIndex}",
                previewModel.DisplayName,
                options.MeshIndex);
            return CreateFallbackMesh();
        }

        if (!TryGetBounds(meshes, out var bounds) || !bounds.IsReasonable)
        {
            Logger.Warning(
                "Asset preview model {DisplayName} has unsupported bounds {Bounds}",
                previewModel.DisplayName,
                bounds.Description);
            return CreateFallbackMesh();
        }

        var renderMesh = new AssetPreviewRenderMesh();
        var textureIndexesByPath = AppendTextureMetadata(renderMesh, meshes);
        var transform = AssetPreviewBoundsTransform.Create(bounds);
        for (var meshIndex = 0; meshIndex < meshes.Count; meshIndex++)
        {
            AppendMesh(renderMesh, meshes[meshIndex], transform, textureIndexesByPath);
        }

        if (renderMesh.Indices.Count == 0)
        {
            Logger.Warning(
                "Asset preview model {DisplayName} produced no valid render triangles from {MeshCount} mesh(es)",
                previewModel.DisplayName,
                meshes.Count);
            return CreateFallbackMesh();
        }

        Logger.Information(
            "Asset preview render mesh created for {DisplayName}: {VertexCount} vertices, {IndexCount} indices, {LineIndexCount} line indices, {TextureCount} texture path(s), bounds {Bounds}",
            previewModel.DisplayName,
            renderMesh.Vertices.Count / 12,
            renderMesh.Indices.Count,
            renderMesh.LineIndices.Count,
            renderMesh.TexturePaths.Count,
            bounds.Description);
        return renderMesh;
    }

    private static Dictionary<string, int> AppendTextureMetadata(AssetPreviewRenderMesh renderMesh, IEnumerable<AssetPreviewMeshDTO> meshes)
    {
        var textureIndexesByPath = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var texturePath in meshes
            .SelectMany(mesh => new[] { mesh.TexturePath, mesh.OverlayTexturePath, mesh.DecalOpacityTexturePath })
            .Where(texturePath => !string.IsNullOrWhiteSpace(texturePath))
            .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            renderMesh.TexturePaths.Add(texturePath!);
        }

        foreach (var texture in meshes
            .SelectMany(mesh => new[] { mesh.Texture, mesh.OverlayTexture, mesh.DecalOpacityTexture })
            .Where(texture => texture != null)
            .DistinctBy(texture => texture!.Path, StringComparer.OrdinalIgnoreCase))
        {
            var textureIndex = renderMesh.Textures.Count;
            renderMesh.Textures.Add(new AssetPreviewRenderTexture
            {
                Path = texture!.Path,
                Data = texture.Data
            });
            textureIndexesByPath[texture.Path] = textureIndex;
        }

        return textureIndexesByPath;
    }

    private static IEnumerable<AssetPreviewMeshDTO> GetMeshes(AssetPreviewModelDTO previewModel, AssetPreviewRenderOptions options)
    {
        var meshes = previewModel.Meshes
            .Where(mesh => !mesh.IsInvisible)
            .ToList();
        if (options.MeshIndex == null)
        {
            return meshes;
        }

        return options.MeshIndex >= 0 && options.MeshIndex < meshes.Count
            ? [meshes[options.MeshIndex.Value]]
            : [];
    }

    private void AppendMesh(
        AssetPreviewRenderMesh renderMesh,
        AssetPreviewMeshDTO mesh,
        AssetPreviewBoundsTransform transform,
        IReadOnlyDictionary<string, int> textureIndexesByPath)
    {
        var baseVertex = (uint)(renderMesh.Vertices.Count / 12);
        var indexOffset = renderMesh.Indices.Count;
        var color = mesh.MaterialName == FallbackMaterialName
            ? new AssetPreviewRenderColor(0.85f, 0.10f, 0.08f)
            : DefaultLoadedMeshColor;
        var textureIndex = mesh.Texture == null
            ? null
            : textureIndexesByPath.TryGetValue(mesh.Texture.Path, out var matchedTextureIndex)
                ? matchedTextureIndex
                : (int?)null;
        var overlayTextureIndex = mesh.OverlayTexture == null
            ? null
            : textureIndexesByPath.TryGetValue(mesh.OverlayTexture.Path, out var matchedOverlayTextureIndex)
                ? matchedOverlayTextureIndex
                : (int?)null;
        var hasUsableTextureCoordinates = HasUsableTextureCoordinates(mesh.Vertices);
        var decalOpacityTextureIndex = mesh.DecalOpacityTexture == null || !hasUsableTextureCoordinates
            ? null
            : textureIndexesByPath.TryGetValue(mesh.DecalOpacityTexture.Path, out var matchedDecalOpacityTextureIndex)
                ? matchedDecalOpacityTextureIndex
                : (int?)null;

        var positions = mesh.Vertices
            .Select(vertex => transform.Apply(vertex.Position))
            .ToList();
        var geometryNormals = CreateGeometryNormals(positions, mesh.Indices);
        var decodedNormalValues = mesh.Vertices
            .Select(vertex => vertex.Normal)
            .ToList();
        var decodedNormals = decodedNormalValues
            .Select(Normalize)
            .ToList();
        var useDecodedNormals = HasUsableDecodedNormals(decodedNormalValues);
        Logger.Information(
            "Asset preview mesh {MeshName} using {NormalSource} normals, material {MaterialName}, texture {TexturePath}",
            mesh.Name,
            useDecodedNormals ? "decoded" : "generated",
            mesh.MaterialName,
            mesh.TexturePath ?? mesh.OverlayTexturePath ?? mesh.DecalOpacityTexturePath);
        if (mesh.DecalOpacityTexture != null && !hasUsableTextureCoordinates)
        {
            Logger.Warning(
                "Asset preview mesh {MeshName} skipped decal opacity texture {TexturePath} because the mesh has no usable UV coordinates",
                mesh.Name,
                mesh.DecalOpacityTexture.Path);
        }

        for (var index = 0; index < mesh.Vertices.Count; index++)
        {
            var position = positions[index];
            var normal = useDecodedNormals
                ? decodedNormals[index]
                : geometryNormals[index];
            renderMesh.Vertices.Add(position.X);
            renderMesh.Vertices.Add(position.Y);
            renderMesh.Vertices.Add(position.Z);
            renderMesh.Vertices.Add(color.Red);
            renderMesh.Vertices.Add(color.Green);
            renderMesh.Vertices.Add(color.Blue);
            renderMesh.Vertices.Add(normal.X);
            renderMesh.Vertices.Add(normal.Y);
            renderMesh.Vertices.Add(normal.Z);
            renderMesh.Vertices.Add(mesh.Vertices[index].UV.U);
            renderMesh.Vertices.Add(mesh.Vertices[index].UV.V);
            renderMesh.Vertices.Add(mesh.Vertices[index].Alpha);
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

        var indexCount = renderMesh.Indices.Count - indexOffset;
        if (indexCount > 0)
        {
            renderMesh.MeshParts.Add(new AssetPreviewRenderMeshPart
            {
                IndexOffset = indexOffset,
                IndexCount = indexCount,
                TextureIndex = textureIndex,
                OverlayTextureIndex = overlayTextureIndex,
                DecalOpacityTextureIndex = decalOpacityTextureIndex,
                MaterialTintRed = mesh.MaterialTintRed,
                MaterialTintGreen = mesh.MaterialTintGreen,
                MaterialTintBlue = mesh.MaterialTintBlue,
                MaterialTintAlpha = mesh.MaterialTintAlpha,
                DecalTintRed = mesh.DecalTintRed,
                DecalTintGreen = mesh.DecalTintGreen,
                DecalTintBlue = mesh.DecalTintBlue,
                DecalOpacity = mesh.DecalOpacity,
                DecalUvScaleU = mesh.DecalUvScaleU,
                DecalUvScaleV = mesh.DecalUvScaleV,
                DecalUvOffsetU = mesh.DecalUvOffsetU,
                DecalUvOffsetV = mesh.DecalUvOffsetV,
                IsDecal = mesh.IsDecal,
                UseAdditiveBlend = mesh.UseAdditiveBlend
            });
        }
    }

    private static bool HasUsableDecodedNormals(IReadOnlyList<AssetPreviewVector3DTO> normals)
    {
        var validCount = 0;
        foreach (var normal in normals)
        {
            var length = GetLength(normal);
            if (length is >= 0.5f and <= 1.5f)
            {
                validCount++;
            }
        }

        return normals.Count > 0 && validCount >= normals.Count / 2;
    }

    private static bool HasUsableTextureCoordinates(IList<AssetPreviewVertexDTO> vertices)
    {
        if (vertices.Count < 3)
        {
            return false;
        }

        var minU = float.PositiveInfinity;
        var maxU = float.NegativeInfinity;
        var minV = float.PositiveInfinity;
        var maxV = float.NegativeInfinity;
        foreach (var vertex in vertices)
        {
            var uv = vertex.UV;
            if (!IsFinite(uv.U) || !IsFinite(uv.V))
            {
                return false;
            }

            minU = MathF.Min(minU, uv.U);
            maxU = MathF.Max(maxU, uv.U);
            minV = MathF.Min(minV, uv.V);
            maxV = MathF.Max(maxV, uv.V);
        }

        return MathF.Abs(maxU - minU) > 0.0001f ||
            MathF.Abs(maxV - minV) > 0.0001f;
    }

    private static AssetPreviewMeshDTO ToRenderSpace(AssetPreviewMeshDTO mesh)
    {
        if (mesh.MaterialName == FallbackMaterialName)
        {
            return mesh;
        }

        var renderMesh = new AssetPreviewMeshDTO
        {
            Name = mesh.Name,
            MaterialName = mesh.MaterialName,
            TexturePath = mesh.TexturePath,
            Texture = mesh.Texture,
            OverlayTexturePath = mesh.OverlayTexturePath,
            OverlayTexture = mesh.OverlayTexture,
            DecalOpacityTexturePath = mesh.DecalOpacityTexturePath,
            DecalOpacityTexture = mesh.DecalOpacityTexture,
            MaterialTintRed = mesh.MaterialTintRed,
            MaterialTintGreen = mesh.MaterialTintGreen,
            MaterialTintBlue = mesh.MaterialTintBlue,
            MaterialTintAlpha = mesh.MaterialTintAlpha,
            DecalTintRed = mesh.DecalTintRed,
            DecalTintGreen = mesh.DecalTintGreen,
            DecalTintBlue = mesh.DecalTintBlue,
            DecalOpacity = mesh.DecalOpacity,
            DecalUvScaleU = mesh.DecalUvScaleU,
            DecalUvScaleV = mesh.DecalUvScaleV,
            DecalUvOffsetU = mesh.DecalUvOffsetU,
            DecalUvOffsetV = mesh.DecalUvOffsetV,
            IsDecal = mesh.IsDecal,
            IsInvisible = mesh.IsInvisible,
            UseAdditiveBlend = mesh.UseAdditiveBlend
        };
        foreach (var vertex in mesh.Vertices)
        {
            renderMesh.Vertices.Add(new AssetPreviewVertexDTO
            {
                Position = ToRenderSpace(vertex.Position),
                Normal = ToRenderSpace(vertex.Normal),
                UV = vertex.UV,
                Alpha = vertex.Alpha
            });
        }

        foreach (var index in mesh.Indices)
        {
            renderMesh.Indices.Add(index);
        }

        return renderMesh;
    }

    private static AssetPreviewVector3DTO ToRenderSpace(AssetPreviewVector3DTO vector)
    {
        return new AssetPreviewVector3DTO
        {
            X = vector.X,
            Y = vector.Z,
            Z = -vector.Y
        };
    }

    private static List<AssetPreviewVector3DTO> CreateGeometryNormals(IReadOnlyList<AssetPreviewVector3DTO> positions, IList<int> indices)
    {
        var normals = Enumerable
            .Range(0, positions.Count)
            .Select(_ => new AssetPreviewVector3DTO
            {
                X = 0f,
                Y = 0f,
                Z = 0f
            })
            .ToList();
        for (var index = 0; index + 2 < indices.Count; index += 3)
        {
            var first = indices[index];
            var second = indices[index + 1];
            var third = indices[index + 2];
            if (first < 0 || first >= positions.Count ||
                second < 0 || second >= positions.Count ||
                third < 0 || third >= positions.Count)
            {
                continue;
            }

            var normal = Cross(
                Subtract(positions[second], positions[first]),
                Subtract(positions[third], positions[first]));
            if (GetLength(normal) <= 0.0001f)
            {
                continue;
            }

            normals[first] = Add(normals[first], normal);
            normals[second] = Add(normals[second], normal);
            normals[third] = Add(normals[third], normal);
        }

        for (var index = 0; index < normals.Count; index++)
        {
            normals[index] = Normalize(normals[index]);
        }

        return normals;
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
        AppendLine(renderMesh, baseVertex, first, second);
        AppendLine(renderMesh, baseVertex, second, third);
        AppendLine(renderMesh, baseVertex, third, first);
        return true;
    }

    private static void AppendLine(AssetPreviewRenderMesh renderMesh, uint baseVertex, int first, int second)
    {
        renderMesh.LineIndices.Add(baseVertex + (uint)first);
        renderMesh.LineIndices.Add(baseVertex + (uint)second);
    }

    private static bool IsValidIndex(AssetPreviewMeshDTO mesh, int index)
    {
        return index >= 0 && index < mesh.Vertices.Count;
    }

    private static bool TryGetBounds(IEnumerable<AssetPreviewMeshDTO> meshes, out AssetPreviewBounds bounds)
    {
        bounds = new AssetPreviewBounds();
        var hasVertex = false;
        foreach (var mesh in meshes)
        {
            if (mesh.MaterialName == FallbackMaterialName)
            {
                continue;
            }

            foreach (var vertex in mesh.Vertices)
            {
                if (!IsFinite(vertex.Position.X) || !IsFinite(vertex.Position.Y) || !IsFinite(vertex.Position.Z))
                {
                    bounds = AssetPreviewBounds.Unreasonable("non-finite vertex position");
                    return false;
                }

                bounds.Include(vertex.Position);
                hasVertex = true;
            }
        }

        return hasVertex;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value) && MathF.Abs(value) <= MaxReasonableCoordinate;
    }

    private static AssetPreviewRenderMesh CreateFallbackMesh()
    {
        var mesh = new AssetPreviewRenderMesh
        {
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
            },
            LineIndices =
            {
                1,
                2,
                2,
                3,
                3,
                4,
                4,
                5,
                5,
                6,
                6,
                7,
                7,
                8,
                8,
                1
            }
        };

        AppendFallbackVertex(mesh, 0f, 0f, 0f);
        AppendFallbackVertex(mesh, -0.35f, 0.9f, 0f);
        AppendFallbackVertex(mesh, 0.35f, 0.9f, 0f);
        AppendFallbackVertex(mesh, 0.9f, 0.35f, 0f);
        AppendFallbackVertex(mesh, 0.9f, -0.35f, 0f);
        AppendFallbackVertex(mesh, 0.35f, -0.9f, 0f);
        AppendFallbackVertex(mesh, -0.35f, -0.9f, 0f);
        AppendFallbackVertex(mesh, -0.9f, -0.35f, 0f);
        AppendFallbackVertex(mesh, -0.9f, 0.35f, 0f);
        return mesh;
    }

    private static void AppendFallbackVertex(AssetPreviewRenderMesh mesh, float x, float y, float z)
    {
        mesh.Vertices.Add(x);
        mesh.Vertices.Add(y);
        mesh.Vertices.Add(z);
        mesh.Vertices.Add(0.85f);
        mesh.Vertices.Add(0.10f);
        mesh.Vertices.Add(0.08f);
        mesh.Vertices.Add(0f);
        mesh.Vertices.Add(0f);
        mesh.Vertices.Add(1f);
        mesh.Vertices.Add(0f);
        mesh.Vertices.Add(0f);
        mesh.Vertices.Add(1f);
    }

    private static AssetPreviewVector3DTO Normalize(AssetPreviewVector3DTO normal)
    {
        var length = GetLength(normal);
        if (length <= 0.0001f ||
            float.IsNaN(length) ||
            float.IsInfinity(length))
        {
            return new AssetPreviewVector3DTO
            {
                X = 0f,
                Y = 0f,
                Z = 1f
            };
        }

        return new AssetPreviewVector3DTO
        {
            X = normal.X / length,
            Y = normal.Y / length,
            Z = normal.Z / length
        };
    }

    private static AssetPreviewVector3DTO Subtract(AssetPreviewVector3DTO first, AssetPreviewVector3DTO second)
    {
        return new AssetPreviewVector3DTO
        {
            X = first.X - second.X,
            Y = first.Y - second.Y,
            Z = first.Z - second.Z
        };
    }

    private static AssetPreviewVector3DTO Add(AssetPreviewVector3DTO first, AssetPreviewVector3DTO second)
    {
        return new AssetPreviewVector3DTO
        {
            X = first.X + second.X,
            Y = first.Y + second.Y,
            Z = first.Z + second.Z
        };
    }

    private static AssetPreviewVector3DTO Cross(AssetPreviewVector3DTO first, AssetPreviewVector3DTO second)
    {
        return new AssetPreviewVector3DTO
        {
            X = (first.Y * second.Z) - (first.Z * second.Y),
            Y = (first.Z * second.X) - (first.X * second.Z),
            Z = (first.X * second.Y) - (first.Y * second.X)
        };
    }

    private static float GetLength(AssetPreviewVector3DTO vector)
    {
        return MathF.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y) + (vector.Z * vector.Z));
    }

    private readonly struct AssetPreviewRenderColor
    {
        public AssetPreviewRenderColor(float red, float green, float blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public float Red { get; }

        public float Green { get; }

        public float Blue { get; }
    }

    private struct AssetPreviewBounds
    {
        private string? FailureReason;
        private bool HasPosition;

        public float MinX { get; private set; }

        public float MinY { get; private set; }

        public float MinZ { get; private set; }

        public float MaxX { get; private set; }

        public float MaxY { get; private set; }

        public float MaxZ { get; private set; }

        public bool IsReasonable => FailureReason == null && LongestAxis > 0f;

        public float LongestAxis => MathF.Max(MaxX - MinX, MathF.Max(MaxY - MinY, MaxZ - MinZ));

        public float CenterZ => (MinZ + MaxZ) / 2f;

        public float CenterY => (MinY + MaxY) / 2f;

        public string Description => FailureReason ?? $"X {MinX:N3}..{MaxX:N3}, Y {MinY:N3}..{MaxY:N3}, Z {MinZ:N3}..{MaxZ:N3}";

        public static AssetPreviewBounds Unreasonable(string failureReason)
        {
            return new AssetPreviewBounds
            {
                FailureReason = failureReason
            };
        }

        public void Include(AssetPreviewVector3DTO position)
        {
            if (FailureReason != null)
            {
                return;
            }

            if (!HasPosition)
            {
                MinX = position.X;
                MaxX = position.X;
                MinY = position.Y;
                MaxY = position.Y;
                MinZ = position.Z;
                MaxZ = position.Z;
                HasPosition = true;
                return;
            }

            MinX = MathF.Min(MinX, position.X);
            MinY = MathF.Min(MinY, position.Y);
            MinZ = MathF.Min(MinZ, position.Z);
            MaxX = MathF.Max(MaxX, position.X);
            MaxY = MathF.Max(MaxY, position.Y);
            MaxZ = MathF.Max(MaxZ, position.Z);
        }
    }

    private readonly struct AssetPreviewBoundsTransform
    {
        private readonly float CenterX;
        private readonly float CenterY;
        private readonly float CenterZ;
        private readonly float Scale;

        private AssetPreviewBoundsTransform(float centerX, float centerY, float centerZ, float scale)
        {
            CenterX = centerX;
            CenterY = centerY;
            CenterZ = centerZ;
            Scale = scale;
        }

        public static AssetPreviewBoundsTransform Create(AssetPreviewBounds bounds)
        {
            return new AssetPreviewBoundsTransform(
                (bounds.MinX + bounds.MaxX) / 2f,
                (bounds.MinY + bounds.MaxY) / 2f,
                (bounds.MinZ + bounds.MaxZ) / 2f,
                TargetPreviewSize / bounds.LongestAxis);
        }

        public AssetPreviewVector3DTO Apply(AssetPreviewVector3DTO position)
        {
            return new AssetPreviewVector3DTO
            {
                X = (position.X - CenterX) * Scale,
                Y = (position.Y - CenterY) * Scale,
                Z = (position.Z - CenterZ) * Scale
            };
        }
    }
}
