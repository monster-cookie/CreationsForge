using System.Buffers.Binary;
using System.Globalization;

namespace CreationsForge.Bethesda.Assets.Nif;

public partial class NifPreviewModelReader
{
    private static NifPreviewMesh? TryReadPreviewGeometry(
        NifBlock block,
        int blockIndex,
        int meshIndex,
        IReadOnlyList<NifObjectTransform> transformChain,
        IReadOnlyList<string> strings,
        IReadOnlyDictionary<int, NifMaterialInfo> materialMap,
        uint bethesdaVersion,
        Func<string, byte[]?>? resolveExternalAsset,
        ref string? rejectionReason)
    {
        if (!IsPreviewGeometryBlock(block))
        {
            return null;
        }

        string? externalRejectionReason = null;
        if (string.Equals(block.TypeName, "BSGeometry", StringComparison.Ordinal) &&
            TryReadStarfieldExternalGeometry(block, blockIndex, meshIndex, transformChain, strings, materialMap, resolveExternalAsset, out var externalMesh, out externalRejectionReason))
        {
            return externalMesh;
        }

        if (rejectionReason == null && externalRejectionReason != null)
        {
            rejectionReason = externalRejectionReason;
        }

        string? skyrimRejectionReason = null;
        if (bethesdaVersion == SkyrimSpecialEditionBethesdaVersion &&
            TryReadSkyrimSpecialEditionBSTriShape(block, blockIndex, meshIndex, transformChain, strings, materialMap, out var skyrimMesh, out skyrimRejectionReason))
        {
            return skyrimMesh;
        }

        if (rejectionReason == null && skyrimRejectionReason != null)
        {
            rejectionReason = skyrimRejectionReason;
        }

        var candidates = new List<NifPreviewMesh>();
        string? candidateRejectionReason = null;
        var anchoredOffsetCount = 0;
        foreach (var offset in GetCandidateGeometryOffsets(block, out anchoredOffsetCount))
        {
            foreach (var countLayout in GetCandidateCountLayouts(block))
            {
                if (TryReadBSTriShapeAt(block, blockIndex, offset, meshIndex, transformChain, strings, materialMap, countLayout, BSVertexPositionFormat.DescriptorDefault, out var mesh, out var currentRejectionReason) &&
                    mesh != null)
                {
                    candidates.Add(mesh);
                }

                if (candidateRejectionReason == null && currentRejectionReason != null)
                {
                    candidateRejectionReason = $"{block.TypeName} offset {offset} {countLayout}: {currentRejectionReason}";
                }
            }
        }

        var selected = candidates
            .OrderByDescending(GetMeshShapeScore)
            .FirstOrDefault();
        if (selected != null)
        {
            selected.Diagnostics.Add($"{block.TypeName} block {blockIndex}: selected from {candidates.Count} candidate mesh layout(s), anchored offsets {anchoredOffsetCount}");
        }

        if (rejectionReason == null && candidateRejectionReason != null)
        {
            rejectionReason = candidateRejectionReason;
        }

        return selected;
    }

    private static bool TryReadSkyrimSpecialEditionBSTriShape(
        NifBlock block,
        int blockIndex,
        int meshIndex,
        IReadOnlyList<NifObjectTransform> transformChain,
        IReadOnlyList<string> strings,
        IReadOnlyDictionary<int, NifMaterialInfo> materialMap,
        out NifPreviewMesh? mesh,
        out string? rejectionReason)
    {
        mesh = null;
        rejectionReason = null;
        if (!TryReadNiAVObjectHeader(block.Data, out _, out var position, out var headerFailureReason))
        {
            rejectionReason = $"{block.TypeName} Skyrim SSE layout: object header not parsed ({headerFailureReason})";
            return false;
        }

        var descriptorOffset = position + BSTriShapeFieldsBeforeVertexDescriptor;
        if (descriptorOffset > block.Data.Length - 18)
        {
            rejectionReason = $"{block.TypeName} Skyrim SSE layout: descriptor offset {descriptorOffset} exceeds block length";
            return false;
        }

        var candidates = new List<NifPreviewMesh>();
        string? candidateRejectionReason = null;
        for (var offset = 0; offset <= block.Data.Length - 18; offset++)
        {
            foreach (var positionFormat in GetSkyrimSpecialEditionPositionFormats())
            {
                if (TryReadBSTriShapeAt(block, blockIndex, offset, meshIndex, transformChain, strings, materialMap, BSTriShapeCountLayout.SkyrimSpecialEdition, positionFormat, out var candidate, out var currentRejectionReason) &&
                    candidate != null)
                {
                    candidates.Add(candidate);
                }

                if (candidateRejectionReason == null && currentRejectionReason != null)
                {
                    candidateRejectionReason = $"{block.TypeName} Skyrim SSE layout offset {offset}: {currentRejectionReason}";
                }
            }
        }

        mesh = candidates
            .OrderByDescending(GetMeshShapeScore)
            .FirstOrDefault();
        if (mesh == null)
        {
            rejectionReason = candidateRejectionReason ?? $"{block.TypeName} Skyrim SSE layout offset {descriptorOffset}: not a supported vertex descriptor";
            return false;
        }

        mesh.Diagnostics.Add($"{block.TypeName} block {blockIndex}: parsed with Skyrim SSE layout from {candidates.Count} candidate mesh layout(s)");
        return true;
    }

    private static bool TryReadStarfieldExternalGeometry(
        NifBlock block,
        int blockIndex,
        int meshIndex,
        IReadOnlyList<NifObjectTransform> transformChain,
        IReadOnlyList<string> strings,
        IReadOnlyDictionary<int, NifMaterialInfo> materialMap,
        Func<string, byte[]?>? resolveExternalAsset,
        out NifPreviewMesh? mesh,
        out string? rejectionReason)
    {
        mesh = null;
        rejectionReason = null;
        var geometryPaths = FindStarfieldGeometryPaths(block.Data).ToList();
        if (geometryPaths.Count == 0)
        {
            return false;
        }

        if (resolveExternalAsset == null)
        {
            rejectionReason = $"{block.TypeName} external geometry resolver was not provided";
            return false;
        }

        foreach (var geometryPath in geometryPaths)
        {
            var geometryData = resolveExternalAsset(geometryPath);
            if (geometryData == null)
            {
                rejectionReason ??= $"{block.TypeName} external geometry {geometryPath} could not be resolved";
                continue;
            }

            if (TryReadStarfieldGeometryMesh(geometryData, block, blockIndex, meshIndex, transformChain, strings, materialMap, geometryPath, out mesh, out var meshRejectionReason))
            {
                return true;
            }

            rejectionReason ??= $"{block.TypeName} external geometry {geometryPath}: {meshRejectionReason}";
        }

        return false;
    }

    private static IEnumerable<string> FindStarfieldGeometryPaths(byte[] data)
    {
        var paths = new List<string>();
        for (var position = 0; position <= data.Length - sizeof(uint); position++)
        {
            var candidatePosition = position;
            if (!TryReadSizedString(data, ref candidatePosition, out var value) ||
                !TryCreateStarfieldGeometryPath(value, out var geometryPath) ||
                paths.Contains(geometryPath, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            paths.Add(geometryPath);
        }

        return paths;
    }

    private static bool TryCreateStarfieldGeometryPath(string value, out string geometryPath)
    {
        geometryPath = string.Empty;
        var parts = value.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2 ||
            parts[0].Length != 20 ||
            parts[1].Length != 20 ||
            !parts[0].All(IsLowerHexCharacter) ||
            !parts[1].All(IsLowerHexCharacter))
        {
            return false;
        }

        geometryPath = $"geometries\\{parts[0]}\\{parts[1]}.mesh";
        return true;
    }

    private static bool IsLowerHexCharacter(char value)
    {
        return value is >= '0' and <= '9' or >= 'a' and <= 'f';
    }

    private static bool TryReadStarfieldGeometryMesh(
        byte[] data,
        NifBlock block,
        int blockIndex,
        int meshIndex,
        IReadOnlyList<NifObjectTransform> transformChain,
        IReadOnlyList<string> strings,
        IReadOnlyDictionary<int, NifMaterialInfo> materialMap,
        string geometryPath,
        out NifPreviewMesh? mesh,
        out string rejectionReason)
    {
        mesh = null;
        rejectionReason = string.Empty;
        if (data.Length < StarfieldGeometryMeshIndexDataOffset + sizeof(ushort))
        {
            rejectionReason = "mesh data ended before the index header";
            return false;
        }

        var version = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(0, sizeof(uint)));
        if (version != StarfieldGeometryMeshVersion)
        {
            rejectionReason = $"mesh version {version} is not supported";
            return false;
        }

        var indexCount = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(sizeof(uint), sizeof(uint)));
        if (indexCount == 0 || indexCount % 3 != 0 || indexCount > 1000000)
        {
            rejectionReason = $"mesh index count {indexCount} is outside the supported range";
            return false;
        }

        var vertexHeaderOffset = checked(StarfieldGeometryMeshIndexDataOffset + ((int)indexCount * sizeof(ushort)));
        if (data.Length - vertexHeaderOffset < StarfieldGeometryMeshVertexHeaderSize)
        {
            rejectionReason = "mesh data ended before the vertex header";
            return false;
        }

        var vertexCount = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(vertexHeaderOffset + (sizeof(uint) * 2), sizeof(uint)));
        if (vertexCount == 0 || vertexCount > 1000000)
        {
            rejectionReason = $"mesh vertex count {vertexCount} is outside the supported range";
            return false;
        }

        var vertexDataOffset = vertexHeaderOffset + StarfieldGeometryMeshVertexHeaderSize;
        if (data.Length - vertexDataOffset < checked((int)vertexCount * StarfieldGeometryMeshPositionStride))
        {
            rejectionReason = "mesh data ended before the first supported vertex buffer";
            return false;
        }

        var hasBounds = TryReadStarfieldGeometryBounds(block.Data, out var geometryBounds);
        var indices = new List<int>();
        var maxIndex = 0;
        var indexPosition = StarfieldGeometryMeshIndexDataOffset;
        for (var index = 0; index < indexCount; index++)
        {
            var vertexIndex = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(indexPosition, sizeof(ushort)));
            indexPosition += sizeof(ushort);
            if (vertexIndex >= vertexCount)
            {
                rejectionReason = $"mesh index {vertexIndex} was outside the vertex count {vertexCount}";
                return false;
            }

            maxIndex = Math.Max(maxIndex, vertexIndex);
            indices.Add(vertexIndex);
        }

        if (maxIndex + 1 > vertexCount)
        {
            rejectionReason = $"mesh max index {maxIndex} was outside the vertex count {vertexCount}";
            return false;
        }

        var positionDataEnd = vertexDataOffset + checked((int)vertexCount * StarfieldGeometryMeshPositionStride);
        var hasUvStream = TryReadStarfieldGeometryUvStream(data, positionDataEnd, vertexCount, out var uvs, out var uvDiagnostic, out var vertexAttributeStreamEnd);
        var hasVertexAlphaStream = TryReadStarfieldGeometryVertexAlphaStream(data, vertexAttributeStreamEnd, vertexCount, out var vertexAlphas, out var vertexAlphaDiagnostic);
        var useTransform = transformChain.Any(transform => !transform.IsIdentity);
        var vertices = new List<NifPreviewVertex>();
        var bounds = new NifMeshBounds();
        for (var index = 0; index < vertexCount; index++)
        {
            var vertexOffset = vertexDataOffset + (index * StarfieldGeometryMeshPositionStride);
            var position = new NifPreviewVector3
            {
                X = BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(vertexOffset, sizeof(short))) * StarfieldGeometryMeshPositionScale,
                Y = BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(vertexOffset + sizeof(short), sizeof(short))) * StarfieldGeometryMeshPositionScale,
                Z = BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(vertexOffset + (sizeof(short) * 2), sizeof(short))) * StarfieldGeometryMeshPositionScale
            };

            if (useTransform)
            {
                position = ApplyTransformChain(position, transformChain);
            }

            if (!bounds.TryInclude(position, out rejectionReason))
            {
                return false;
            }

            vertices.Add(new NifPreviewVertex
            {
                Position = position,
                Normal = new NifPreviewVector3(),
                UV = hasUvStream ? uvs[index] : new NifPreviewUV(),
                Alpha = hasVertexAlphaStream ? vertexAlphas[index] : 1f
            });
        }

        var material = GetStarfieldExternalGeometryMaterial(block.Data, block.TypeName, blockIndex, meshIndex, strings, materialMap);
        mesh = new NifPreviewMesh
        {
            Name = material.MeshName,
            MaterialName = material.MaterialName,
            TexturePath = material.TexturePath,
            OverlayTexturePath = material.OverlayTexturePath,
            DecalOpacityTexturePath = material.DecalOpacityTexturePath,
            MaterialTintRed = material.MaterialTint.Red,
            MaterialTintGreen = material.MaterialTint.Green,
            MaterialTintBlue = material.MaterialTint.Blue,
            MaterialTintAlpha = material.MaterialTint.Alpha,
            DecalTintRed = material.DecalTintRed,
            DecalTintGreen = material.DecalTintGreen,
            DecalTintBlue = material.DecalTintBlue,
            DecalOpacity = material.DecalOpacity,
            DecalUvScaleU = material.DecalUvTransform.ScaleU,
            DecalUvScaleV = material.DecalUvTransform.ScaleV,
            DecalUvOffsetU = material.DecalUvTransform.OffsetU,
            DecalUvOffsetV = material.DecalUvTransform.OffsetV,
            IsDecal = material.IsDecal,
            IsInvisible = material.IsInvisible,
            UseAdditiveBlend = material.UseAdditiveBlend,
            Vertices = vertices,
            Indices = indices,
            Diagnostics =
            {
                $"{block.TypeName} block {blockIndex}: external Starfield geometry {geometryPath}, {vertexCount} vertices, {indexCount / 3} triangles, position stride {StarfieldGeometryMeshPositionStride}, geometry bounds metadata {(hasBounds ? geometryBounds.Description : "unavailable")}, decoded bounds {bounds.Description}",
                $"{block.TypeName} block {blockIndex}: external Starfield geometry UV stream {uvDiagnostic}",
                $"{block.TypeName} block {blockIndex}: external Starfield geometry vertex alpha stream {vertexAlphaDiagnostic}",
                $"{block.TypeName} block {blockIndex} external material: {material.MaterialName}, texture {material.TexturePath ?? "none"}, overlay {material.OverlayTexturePath ?? "none"}, decal opacity {material.DecalOpacityTexturePath ?? "none"}, decal {material.IsDecal}, invisible {material.IsInvisible}",
                $"{block.TypeName} block {blockIndex} triangle sample: {CreateTriangleSample(indices)}"
            }
        };
        if (material.DecalOpacityTexturePath != null && !hasUvStream)
        {
            mesh.Diagnostics.Add($"{block.TypeName} block {blockIndex}: decal opacity texture resolved, but external Starfield geometry UV and vertex alpha streams are not decoded yet");
        }

        return true;
    }

    private static bool TryReadStarfieldGeometryUvStream(
        byte[] data,
        int position,
        uint vertexCount,
        out IReadOnlyList<NifPreviewUV> uvs,
        out string diagnostic,
        out int streamEnd)
    {
        uvs = [];
        streamEnd = position;
        if (data.Length - position < sizeof(uint))
        {
            diagnostic = "not present";
            return false;
        }

        var uvCount = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position, sizeof(uint)));
        if (uvCount != vertexCount)
        {
            diagnostic = $"not decoded because stream count {uvCount} did not match vertex count {vertexCount}";
            return false;
        }

        position += sizeof(uint);
        var requiredLength = checked((int)vertexCount * StarfieldGeometryMeshUvStride);
        if (data.Length - position < requiredLength)
        {
            diagnostic = "not decoded because stream ended before all UVs";
            return false;
        }

        var parsedUvs = new List<NifPreviewUV>();
        var minU = float.PositiveInfinity;
        var maxU = float.NegativeInfinity;
        var minV = float.PositiveInfinity;
        var maxV = float.NegativeInfinity;
        for (var index = 0; index < vertexCount; index++)
        {
            var uv = ReadHalfTexCoord(data, ref position);
            if (!IsReasonableUvCoordinate(uv.U) || !IsReasonableUvCoordinate(uv.V))
            {
                diagnostic = $"not decoded because UV {index} was outside the supported range";
                return false;
            }

            minU = MathF.Min(minU, uv.U);
            maxU = MathF.Max(maxU, uv.U);
            minV = MathF.Min(minV, uv.V);
            maxV = MathF.Max(maxV, uv.V);
            parsedUvs.Add(uv);
        }

        if (MathF.Abs(maxU - minU) <= 0.0001f &&
            MathF.Abs(maxV - minV) <= 0.0001f)
        {
            diagnostic = "not decoded because all UVs were identical";
            return false;
        }

        uvs = parsedUvs;
        streamEnd = position;
        diagnostic = $"decoded {vertexCount} half-precision UVs, U {minU:N3}..{maxU:N3}, V {minV:N3}..{maxV:N3}";
        return true;
    }

    private static bool TryReadStarfieldGeometryVertexAlphaStream(
        byte[] data,
        int position,
        uint vertexCount,
        out IReadOnlyList<float> vertexAlphas,
        out string diagnostic)
    {
        vertexAlphas = [];
        if (data.Length - position < sizeof(uint) * 2)
        {
            diagnostic = "not present";
            return false;
        }

        var countOffsets = new[] { sizeof(uint), sizeof(uint) * 2 };
        var mismatchDiagnostics = new List<string>();
        foreach (var countOffset in countOffsets)
        {
            if (data.Length - position < countOffset + sizeof(uint))
            {
                continue;
            }

            var streamCount = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position + countOffset, sizeof(uint)));
            if (streamCount != vertexCount)
            {
                mismatchDiagnostics.Add($"count at offset {countOffset} was {streamCount}");
                continue;
            }

            if (TryReadStarfieldGeometryVertexAlphaValues(data, position + countOffset + sizeof(uint), vertexCount, out vertexAlphas, out diagnostic))
            {
                diagnostic = $"{diagnostic} after {countOffset} byte header";
                return true;
            }

            return false;
        }

        diagnostic = $"not decoded because {string.Join(", ", mismatchDiagnostics)}";
        return false;
    }

    private static bool TryReadStarfieldGeometryVertexAlphaValues(
        byte[] data,
        int position,
        uint vertexCount,
        out IReadOnlyList<float> vertexAlphas,
        out string diagnostic)
    {
        vertexAlphas = [];
        var requiredLength = checked((int)vertexCount * sizeof(uint));
        if (data.Length - position < requiredLength)
        {
            diagnostic = "not decoded because stream ended before all alpha values";
            return false;
        }

        var parsedAlphas = new List<float>();
        var minAlpha = float.PositiveInfinity;
        var maxAlpha = float.NegativeInfinity;
        for (var index = 0; index < vertexCount; index++)
        {
            var alpha = data[position] / 255f;
            position += sizeof(uint);
            minAlpha = MathF.Min(minAlpha, alpha);
            maxAlpha = MathF.Max(maxAlpha, alpha);
            parsedAlphas.Add(alpha);
        }

        if (MathF.Abs(maxAlpha - minAlpha) <= 0.0001f)
        {
            diagnostic = $"not decoded because stream had uniform alpha {minAlpha:N3}";
            return false;
        }

        vertexAlphas = parsedAlphas;
        diagnostic = $"decoded {vertexCount} packed alpha values, alpha {minAlpha:N3}..{maxAlpha:N3}";
        return true;
    }

    private static bool TryReadStarfieldGeometryBounds(byte[] data, out StarfieldGeometryBounds bounds)
    {
        bounds = default;
        if (!TryReadNiAVObjectHeader(data, out _, out var position, out _))
        {
            return false;
        }

        if (data.Length - position < (sizeof(float) * 10))
        {
            return false;
        }

        var firstCenter = ReadVector3(data, ref position);
        _ = ReadSingle(data, ref position);
        var center = ReadVector3(data, ref position);
        var extents = ReadVector3(data, ref position);
        bounds = new StarfieldGeometryBounds(center, extents);
        if (!bounds.IsReasonable)
        {
            bounds = new StarfieldGeometryBounds(firstCenter, extents);
        }

        return bounds.IsReasonable;
    }

    private static bool TryReadBSTriShapeAt(
        NifBlock block,
        int blockIndex,
        int offset,
        int meshIndex,
        IReadOnlyList<NifObjectTransform> transformChain,
        IReadOnlyList<string> strings,
        IReadOnlyDictionary<int, NifMaterialInfo> materialMap,
        BSTriShapeCountLayout countLayout,
        BSVertexPositionFormat positionFormat,
        out NifPreviewMesh? mesh,
        out string? rejectionReason)
    {
        mesh = null;
        rejectionReason = null;
        var data = block.Data;
        var vertexDesc = BinaryPrimitives.ReadUInt64LittleEndian(data.AsSpan(offset, sizeof(ulong)));
        var descriptor = new BSVertexDescriptor(vertexDesc);
        if (descriptor.StrideWords < 4 || descriptor.StrideWords > 16 || (descriptor.Attributes & 0x1) == 0)
        {
            return false;
        }

        var countStartPosition = offset + sizeof(ulong);
        var position = countStartPosition;
        uint triangleCount;
        ushort vertexCount;
        uint dataSize = 0U;
        var hasExplicitDataSize = true;
        var countProbeOffset = 0;
        if (countLayout == BSTriShapeCountLayout.SkyrimSpecialEdition)
        {
            if (!TryReadUInt16(data, ref position, out var skyrimTriangleCount) ||
                !TryReadUInt16(data, ref position, out vertexCount) ||
                !TryReadUInt32(data, ref position, out dataSize))
            {
                rejectionReason = "shape header ended before counts could be read";
                return false;
            }

            triangleCount = skyrimTriangleCount;
        }
        else if (countLayout == BSTriShapeCountLayout.StarfieldGeometry)
        {
            if (!TryReadStarfieldGeometryCounts(data, countStartPosition, descriptor.VertexStride, out var counts, out rejectionReason))
            {
                return false;
            }

            triangleCount = counts.TriangleCount;
            vertexCount = counts.VertexCount;
            position = counts.VertexDataOffset;
            countProbeOffset = counts.CountProbeOffset;
            hasExplicitDataSize = false;
        }
        else if (!TryReadUInt32(data, ref position, out triangleCount) ||
            !TryReadUInt16(data, ref position, out vertexCount) ||
            !TryReadUInt32(data, ref position, out dataSize))
        {
            rejectionReason = "shape header ended before counts could be read";
            return false;
        }

        if (vertexCount == 0 || triangleCount == 0 || triangleCount > 1000000)
        {
            rejectionReason = $"invalid counts ({vertexCount} vertices, {triangleCount} triangles)";
            return false;
        }

        var vertexStride = descriptor.VertexStride;
        var expectedDataSize = checked(((uint)vertexStride * vertexCount) + (triangleCount * TriangleIndexByteCount));
        if (hasExplicitDataSize && (dataSize != expectedDataSize || data.Length - position < dataSize))
        {
            rejectionReason = $"data size {dataSize} did not match expected {expectedDataSize} for stride {vertexStride}";
            return false;
        }

        if (!hasExplicitDataSize && data.Length - position < expectedDataSize)
        {
            rejectionReason = $"inferred data size {expectedDataSize} exceeded remaining block data for stride {vertexStride}";
            return false;
        }

        var vertexDataOffset = position;
        var rawBounds = new NifMeshBounds();
        var transformedBounds = new NifMeshBounds();
        var descriptorAlignedTransformChain = TryGetDescriptorAlignedTransformChain(data, offset, transformChain);
        var transformSource = descriptorAlignedTransformChain.Count == transformChain.Count
            ? "scene"
            : "descriptor-aligned";
        var useTransform = descriptorAlignedTransformChain.Any(transform => !transform.IsIdentity);
        var vertices = new List<NifPreviewVertex>();
        for (var index = 0; index < vertexCount; index++)
        {
            var vertex = ReadBSVertex(data, vertexDataOffset + (index * vertexStride), descriptor, countLayout, positionFormat);
            if (!rawBounds.TryInclude(vertex.Position, out rejectionReason))
            {
                return false;
            }

            if (useTransform)
            {
                var transformedPosition = ApplyTransformChain(vertex.Position, descriptorAlignedTransformChain);
                if (!transformedBounds.TryInclude(transformedPosition, out _))
                {
                    useTransform = false;
                }

                vertex.Position = transformedPosition;
            }

            vertices.Add(vertex);
        }

        if (useTransform && !transformedBounds.IsUsefulPreviewBounds(rawBounds))
        {
            useTransform = false;
        }

        if (!useTransform)
        {
            for (var index = 0; index < vertices.Count; index++)
            {
                vertices[index] = ReadBSVertex(data, vertexDataOffset + (index * vertexStride), descriptor, countLayout, positionFormat);
            }
        }

        position = vertexDataOffset + (vertexStride * vertexCount);
        var indices = new List<int>();
        for (var index = 0; index < triangleCount; index++)
        {
            if (!TryReadUInt16(data, ref position, out var first) ||
                !TryReadUInt16(data, ref position, out var second) ||
                !TryReadUInt16(data, ref position, out var third) ||
                first >= vertexCount ||
                second >= vertexCount ||
                third >= vertexCount)
            {
                rejectionReason = "triangle indices were outside the vertex range";
                return false;
            }

            indices.Add(first);
            indices.Add(second);
            indices.Add(third);
        }

        var triangleQuality = GetTriangleQuality(vertices, indices, rawBounds);
        var normalQuality = GetNormalQuality(vertices);
        var material = GetShapeMaterial(data, offset, block.TypeName, blockIndex, meshIndex, strings, materialMap);
        mesh = new NifPreviewMesh
        {
            Name = material.MeshName,
            MaterialName = material.MaterialName,
            TexturePath = material.TexturePath,
            OverlayTexturePath = material.OverlayTexturePath,
            DecalOpacityTexturePath = material.DecalOpacityTexturePath,
            MaterialTintRed = material.MaterialTint.Red,
            MaterialTintGreen = material.MaterialTint.Green,
            MaterialTintBlue = material.MaterialTint.Blue,
            MaterialTintAlpha = material.MaterialTint.Alpha,
            DecalTintRed = material.DecalTintRed,
            DecalTintGreen = material.DecalTintGreen,
            DecalTintBlue = material.DecalTintBlue,
            DecalOpacity = material.DecalOpacity,
            DecalUvScaleU = material.DecalUvTransform.ScaleU,
            DecalUvScaleV = material.DecalUvTransform.ScaleV,
            DecalUvOffsetU = material.DecalUvTransform.OffsetU,
            DecalUvOffsetV = material.DecalUvTransform.OffsetV,
            IsDecal = material.IsDecal,
            IsInvisible = material.IsInvisible,
            UseAdditiveBlend = material.UseAdditiveBlend,
            Vertices = vertices,
            Indices = indices,
            Diagnostics =
            {
                $"{block.TypeName} block {blockIndex} offset {offset}: {vertexCount} vertices, {triangleCount} triangles, count layout {countLayout}, count offset {countProbeOffset}, position format {positionFormat}, descriptor 0x{vertexDesc:X16}, {descriptor.Description}, transform {GetTransformDescription(useTransform, descriptorAlignedTransformChain, transformSource)}, raw {rawBounds.Description}, {triangleQuality.Description}",
                $"{block.TypeName} block {blockIndex} normals: {normalQuality.Description}",
                $"{block.TypeName} block {blockIndex} material: {material.MaterialName}, texture {material.TexturePath ?? "none"}, overlay {material.OverlayTexturePath ?? "none"}, decal opacity {material.DecalOpacityTexturePath ?? "none"}, decal {material.IsDecal}, invisible {material.IsInvisible}",
                $"{block.TypeName} block {blockIndex} bytes before descriptor: {CreateHexSample(data, Math.Max(0, offset - DiagnosticHexByteCount), Math.Min(DiagnosticHexByteCount, offset))}",
                $"{block.TypeName} block {blockIndex} first vertex bytes: {CreateVertexByteSample(data, vertexDataOffset, vertexStride, Math.Min(3, (int)vertexCount))}",
                $"{block.TypeName} block {blockIndex} vertex sample: {CreateVertexSample(vertices)}",
                $"{block.TypeName} block {blockIndex} triangle sample: {CreateTriangleSample(indices)}"
            }
        };
        return true;
    }

    private static bool TryReadStarfieldGeometryCounts(
        byte[] data,
        int countStartPosition,
        int vertexStride,
        out BSGeometryCountProbe counts,
        out string? rejectionReason)
    {
        counts = default;
        rejectionReason = "shape header ended before counts could be read";
        var maxProbeOffset = Math.Min(MaxStarfieldGeometryCountProbeBytes, data.Length - countStartPosition - sizeof(uint) - sizeof(ushort));
        for (var countProbeOffset = 0; countProbeOffset <= maxProbeOffset; countProbeOffset += sizeof(ushort))
        {
            var position = countStartPosition + countProbeOffset;
            if (!TryReadUInt32(data, ref position, out var triangleCount) ||
                !TryReadUInt16(data, ref position, out var vertexCount))
            {
                continue;
            }

            if (vertexCount == 0 || triangleCount == 0 || triangleCount > 1000000)
            {
                rejectionReason = $"invalid counts ({vertexCount} vertices, {triangleCount} triangles)";
                continue;
            }

            var expectedDataSize = checked(((uint)vertexStride * vertexCount) + (triangleCount * TriangleIndexByteCount));
            if (data.Length - position < expectedDataSize)
            {
                rejectionReason = $"inferred data size {expectedDataSize} exceeded remaining block data for stride {vertexStride}";
                continue;
            }

            counts = new BSGeometryCountProbe(triangleCount, vertexCount, position, countProbeOffset);
            return true;
        }

        return false;
    }

    private static NifShapeMaterial GetShapeMaterial(
        byte[] data,
        int descriptorOffset,
        string blockTypeName,
        int blockIndex,
        int meshIndex,
        IReadOnlyList<string> strings,
        IReadOnlyDictionary<int, NifMaterialInfo> materialMap)
    {
        var meshName = TryReadStringRef(data, 0, strings, out var parsedMeshName)
            ? parsedMeshName
            : $"{blockTypeName} {meshIndex + 1}";
        var shaderPropertyPosition = descriptorOffset - (sizeof(int) * 2);
        if (shaderPropertyPosition >= 0 &&
            data.Length - shaderPropertyPosition >= sizeof(int))
        {
            var shaderPropertyBlockIndex = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(shaderPropertyPosition, sizeof(int)));
            if (materialMap.TryGetValue(shaderPropertyBlockIndex, out var material))
            {
                return new NifShapeMaterial(meshName, material);
            }
        }

        return new NifShapeMaterial(meshName, new NifMaterialInfo($"{blockTypeName} {blockIndex}", null, null, null, StarfieldPreviewColor.White, 1f, 1f, 1f, 1f, StarfieldPreviewUvTransform.Identity, false, false, false));
    }

    private static NifShapeMaterial GetStarfieldExternalGeometryMaterial(
        byte[] data,
        string blockTypeName,
        int blockIndex,
        int meshIndex,
        IReadOnlyList<string> strings,
        IReadOnlyDictionary<int, NifMaterialInfo> materialMap)
    {
        var meshName = TryReadStringRef(data, 0, strings, out var parsedMeshName)
            ? parsedMeshName
            : $"{blockTypeName} {meshIndex + 1}";
        if (!TryReadNiAVObjectHeader(data, out _, out var position, out _) ||
            data.Length - position < (sizeof(float) * 10) + (sizeof(int) * 3))
        {
            return new NifShapeMaterial(meshName, new NifMaterialInfo($"{blockTypeName} {blockIndex}", null, null, null, StarfieldPreviewColor.White, 1f, 1f, 1f, 1f, StarfieldPreviewUvTransform.Identity, false, false, false));
        }

        var shaderPropertyPosition = position + (sizeof(float) * 10) + sizeof(int);
        var shaderPropertyBlockIndex = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(shaderPropertyPosition, sizeof(int)));
        if (materialMap.TryGetValue(shaderPropertyBlockIndex, out var material))
        {
            return new NifShapeMaterial(meshName, material);
        }

        return new NifShapeMaterial(meshName, new NifMaterialInfo($"{blockTypeName} {blockIndex}", null, null, null, StarfieldPreviewColor.White, 1f, 1f, 1f, 1f, StarfieldPreviewUvTransform.Identity, false, false, false));
    }

    private static NifNormalQuality GetNormalQuality(IReadOnlyList<NifPreviewVertex> vertices)
    {
        var validCount = 0;
        var minLength = float.MaxValue;
        var maxLength = 0f;
        foreach (var vertex in vertices)
        {
            var length = GetLength(vertex.Normal);
            if (float.IsNaN(length) || float.IsInfinity(length))
            {
                continue;
            }

            minLength = MathF.Min(minLength, length);
            maxLength = MathF.Max(maxLength, length);
            if (length >= 0.5f && length <= 1.5f)
            {
                validCount++;
            }
        }

        if (vertices.Count == 0 || minLength == float.MaxValue)
        {
            return new NifNormalQuality(0, 0, 0, 0);
        }

        return new NifNormalQuality(vertices.Count, validCount, minLength, maxLength);
    }

    private static IReadOnlyList<NifObjectTransform> TryGetDescriptorAlignedTransformChain(
        byte[] data,
        int descriptorOffset,
        IReadOnlyList<NifObjectTransform> transformChain)
    {
        if (transformChain.Any(transform => !transform.IsIdentity) ||
            !TryReadNiAVObjectHeader(data, out var transform, out var position, out _) ||
            position + BSTriShapeFieldsBeforeVertexDescriptor != descriptorOffset)
        {
            return transformChain;
        }

        return [transform];
    }

    private static NifTriangleQuality GetTriangleQuality(
        IReadOnlyList<NifPreviewVertex> vertices,
        IReadOnlyList<int> indices,
        NifMeshBounds bounds)
    {
        var triangleCount = 0;
        var degenerateCount = 0;
        var totalRatio = 0f;
        var maxRatio = 0f;
        var longestAxis = bounds.LongestAxis;
        if (longestAxis <= 0f)
        {
            return new NifTriangleQuality(0, 0, 0, 0);
        }

        for (var index = 0; index + 2 < indices.Count; index += 3)
        {
            var first = vertices[indices[index]].Position;
            var second = vertices[indices[index + 1]].Position;
            var third = vertices[indices[index + 2]].Position;
            var firstEdge = GetDistance(first, second);
            var secondEdge = GetDistance(second, third);
            var thirdEdge = GetDistance(third, first);
            var maxEdge = MathF.Max(firstEdge, MathF.Max(secondEdge, thirdEdge));
            var ratio = maxEdge / longestAxis;
            totalRatio += ratio;
            maxRatio = MathF.Max(maxRatio, ratio);
            triangleCount++;
            if (firstEdge <= 0.0001f || secondEdge <= 0.0001f || thirdEdge <= 0.0001f)
            {
                degenerateCount++;
            }
        }

        return new NifTriangleQuality(
            triangleCount,
            triangleCount == 0 ? 0f : totalRatio / triangleCount,
            maxRatio,
            degenerateCount);
    }

    private static float GetDistance(NifPreviewVector3 first, NifPreviewVector3 second)
    {
        var x = first.X - second.X;
        var y = first.Y - second.Y;
        var z = first.Z - second.Z;
        return MathF.Sqrt((x * x) + (y * y) + (z * z));
    }

    private static float GetLength(NifPreviewVector3 vector)
    {
        return MathF.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y) + (vector.Z * vector.Z));
    }

    private static string CreateVertexSample(IReadOnlyList<NifPreviewVertex> vertices)
    {
        return string.Join(
            "; ",
            vertices
                .Take(4)
                .Select((vertex, index) => $"v{index} {FormatVector(vertex.Position)}"));
    }

    private static string CreateTriangleSample(IReadOnlyList<int> indices)
    {
        var triangles = new List<string>();
        for (var index = 0; index + 2 < indices.Count && triangles.Count < 4; index += 3)
        {
            triangles.Add($"t{triangles.Count} ({indices[index]},{indices[index + 1]},{indices[index + 2]})");
        }

        return string.Join("; ", triangles);
    }

    private static string CreateVertexByteSample(byte[] data, int vertexDataOffset, int vertexStride, int vertexCount)
    {
        var samples = new List<string>();
        for (var index = 0; index < vertexCount; index++)
        {
            samples.Add($"v{index} {CreateHexSample(data, vertexDataOffset + (index * vertexStride), vertexStride)}");
        }

        return string.Join("; ", samples);
    }

    private static string CreateHexSample(byte[] data, int offset, int count)
    {
        if (offset < 0 || offset >= data.Length || count <= 0)
        {
            return string.Empty;
        }

        var safeCount = Math.Min(count, data.Length - offset);
        return Convert.ToHexString(data.AsSpan(offset, safeCount));
    }

    private static string FormatVector(NifPreviewVector3 vector)
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"({vector.X:0.###},{vector.Y:0.###},{vector.Z:0.###})");
    }

    private static NifPreviewVector3 ApplyTransformChain(NifPreviewVector3 position, IReadOnlyList<NifObjectTransform> transformChain)
    {
        var transformed = position;
        foreach (var transform in transformChain)
        {
            transformed = transform.Apply(transformed);
        }

        return transformed;
    }

    private static string GetTransformDescription(bool useTransform, IReadOnlyList<NifObjectTransform> transformChain, string transformSource)
    {
        if (!useTransform)
        {
            return "raw";
        }

        return transformChain.Count == 1
            ? $"applied {transformSource}"
            : $"applied {transformChain.Count} inherited {transformSource}";
    }

    private static bool TryReadNiNodeChildren(byte[] data, out IReadOnlyList<int> children, out string failureReason)
    {
        children = [];
        failureReason = string.Empty;
        if (!TryReadNiAVObjectHeader(data, out _, out var position, out failureReason))
        {
            return false;
        }

        if (!TryReadUInt32(data, ref position, out var childCount) ||
            childCount > MaxReasonableBlockCount ||
            data.Length - position < childCount * sizeof(int))
        {
            failureReason = $"child list at {position} is outside the supported range";
            return false;
        }

        var parsedChildren = new List<int>();
        for (var index = 0; index < childCount; index++)
        {
            if (!TryReadInt32(data, ref position, out var child))
            {
                failureReason = $"child reference {index} could not be read";
                return false;
            }

            parsedChildren.Add(child);
        }

        children = parsedChildren;
        return true;
    }

    private static bool TryReadNiAVObjectHeader(byte[] data, out NifObjectTransform transform, out int position, out string failureReason)
    {
        transform = NifObjectTransform.Identity;
        position = 0;
        failureReason = string.Empty;
        if (!TryReadUInt32(data, ref position, out _))
        {
            failureReason = "name string index could not be read";
            return false;
        }

        if (!TryReadUInt32(data, ref position, out var extraDataCount))
        {
            failureReason = "extra data count could not be read";
            return false;
        }

        if (extraDataCount > MaxReasonableExtraDataCount)
        {
            failureReason = $"extra data count {extraDataCount} is outside the supported range";
            return false;
        }

        if (data.Length - position < extraDataCount * sizeof(int))
        {
            failureReason = $"extra data refs for count {extraDataCount} exceed block length";
            return false;
        }

        position += (int)extraDataCount * sizeof(int);
        if (data.Length - position < sizeof(int) + sizeof(uint) + (sizeof(float) * 13) + sizeof(int))
        {
            failureReason = $"object transform fields at {position} exceed block length";
            return false;
        }

        position += sizeof(int);
        position += sizeof(uint);
        var translation = ReadVector3(data, ref position);
        var rotation = new NifRotation3x3
        {
            M11 = ReadSingle(data, ref position),
            M12 = ReadSingle(data, ref position),
            M13 = ReadSingle(data, ref position),
            M21 = ReadSingle(data, ref position),
            M22 = ReadSingle(data, ref position),
            M23 = ReadSingle(data, ref position),
            M31 = ReadSingle(data, ref position),
            M32 = ReadSingle(data, ref position),
            M33 = ReadSingle(data, ref position)
        };
        var scale = ReadSingle(data, ref position);
        position += sizeof(int);
        if (!IsReasonableCoordinate(translation.X) ||
            !IsReasonableCoordinate(translation.Y) ||
            !IsReasonableCoordinate(translation.Z) ||
            !IsReasonableScale(scale) ||
            !rotation.IsReasonable)
        {
            failureReason = $"object transform values were outside the supported range ({FormatVector(translation)}, scale {scale.ToString("0.###", CultureInfo.InvariantCulture)})";
            return false;
        }

        transform = new NifObjectTransform(translation, rotation, scale);
        return true;
    }

    private static IEnumerable<BSVertexPositionFormat> GetSkyrimSpecialEditionPositionFormats()
    {
        yield return BSVertexPositionFormat.Float3;
        yield return BSVertexPositionFormat.Half3;
    }

    private static IEnumerable<int> GetCandidateGeometryOffsets(NifBlock block, out int anchoredOffsetCount)
    {
        var offsets = new List<int>();
        if (string.Equals(block.TypeName, "BSGeometry", StringComparison.Ordinal) &&
            TryReadNiAVObjectHeader(block.Data, out _, out var position, out _))
        {
            AddCandidateOffset(offsets, position + BSTriShapeFieldsBeforeVertexDescriptor, block.Data.Length);
        }

        anchoredOffsetCount = offsets.Count;
        for (var offset = 0; offset <= block.Data.Length - 18; offset++)
        {
            AddCandidateOffset(offsets, offset, block.Data.Length);
        }

        return offsets;
    }

    private static void AddCandidateOffset(ICollection<int> offsets, int offset, int dataLength)
    {
        if (offset >= 0 && offset <= dataLength - 18 && !offsets.Contains(offset))
        {
            offsets.Add(offset);
        }
    }

    private static IEnumerable<BSTriShapeCountLayout> GetCandidateCountLayouts(NifBlock block)
    {
        if (string.Equals(block.TypeName, "BSGeometry", StringComparison.Ordinal))
        {
            yield return BSTriShapeCountLayout.StarfieldGeometry;
        }

        yield return BSTriShapeCountLayout.Fallout4;
    }

    private static bool HasUnsupportedSkinnedGeometry(IReadOnlyList<NifBlock> blocks)
    {
        return blocks.Any(block =>
            block.TypeName.Contains("NiSkinInstance", StringComparison.Ordinal) ||
            block.TypeName.Contains("NiSkinPartition", StringComparison.Ordinal));
    }

    private static NifPreviewVertex ReadBSVertex(byte[] data, int offset, BSVertexDescriptor descriptor, BSTriShapeCountLayout countLayout, BSVertexPositionFormat positionFormat)
    {
        var position = offset;
        var vertex = positionFormat switch
        {
            BSVertexPositionFormat.Float3 => ReadVector3(data, ref position),
            BSVertexPositionFormat.Half3 => ReadHalfVector3(data, ref position),
            _ => descriptor.HasFullPrecisionPositions || countLayout == BSTriShapeCountLayout.SkyrimSpecialEdition && descriptor.VertexStride >= 32
                ? ReadVector3(data, ref position)
                : ReadHalfVector3(data, ref position)
        };

        position = offset + descriptor.UVOffset;
        var uv = descriptor.HasUV
            ? ReadHalfTexCoord(data, ref position)
            : new NifPreviewUV();

        position = offset + descriptor.NormalOffset;
        var normal = descriptor.HasNormals
            ? ReadByteVector3(data, ref position)
            : new NifPreviewVector3
            {
                Z = 1f
            };

        return new NifPreviewVertex
        {
            Position = vertex,
            Normal = normal,
            UV = uv
        };
    }

    private static float GetMeshShapeScore(NifPreviewMesh mesh)
    {
        var bounds = new NifMeshBounds();
        foreach (var vertex in mesh.Vertices)
        {
            if (!bounds.TryInclude(vertex.Position, out _))
            {
                return 0f;
            }
        }

        var vertices = mesh.Vertices.ToList();
        var indices = mesh.Indices.ToList();
        var triangleQuality = GetTriangleQuality(vertices, indices, bounds);
        if (triangleQuality.TriangleCount == 0)
        {
            return 0f;
        }

        var degenerateRatio = (float)triangleQuality.DegenerateCount / triangleQuality.TriangleCount;
        if (degenerateRatio >= 0.5f)
        {
            return 0f;
        }

        var compactnessScore = Math.Clamp(bounds.ShapeScore * 4f, 0.05f, 1f);
        var triangleScore = 1f - degenerateRatio;
        var sizeScore = bounds.LongestAxis > 10000f ? 0.1f : 1f;
        return compactnessScore * triangleScore * sizeScore;
    }
}
