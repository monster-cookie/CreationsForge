using System.Diagnostics;
using System.Globalization;

namespace CreationsForge.Bethesda.Assets.Nif;

public partial class NifPreviewModelReader : INifPreviewModelReader
{
    private const uint Fallout4Version = 0x14020007;
    private const uint SkyrimSpecialEditionUserVersion = 12;
    private const uint SkyrimSpecialEditionBethesdaVersion = 100;
    private const uint Fallout4BethesdaVersion = 130;
    private const int MaxReasonableBlockCount = 100000;
    private const int MaxReasonableBlockTypeCount = 10000;
    private const int MaxReasonableStringLength = 1000000;
    private const int MaxHeaderTableSearchBytes = 8192;
    private const float MaxReasonableCoordinate = 1000000f;
    private const int MaxReasonableExtraDataCount = 1024;
    private const int TriangleIndexByteCount = 6;
    private const int DiagnosticHexByteCount = 24;
    private const int BSTriShapeFieldsBeforeVertexDescriptor = (sizeof(float) * 4) + (sizeof(int) * 3);
    private const int MaxStarfieldGeometryCountProbeBytes = 16;
    private const int StarfieldGeometryMeshVersion = 2;
    private const int StarfieldGeometryMeshIndexDataOffset = 8;
    private const int StarfieldGeometryMeshVertexHeaderSize = 12;
    private const int StarfieldGeometryMeshPositionStride = sizeof(short) * 3;
    private const int StarfieldGeometryMeshUvStride = sizeof(ushort) * 2;
    private const float StarfieldGeometryMeshPositionScale = 1f / 32767f;
    private const ulong StarfieldMaterialDatabaseBethSignature = 0x0000000848544542UL;
    private const uint StarfieldMaterialDatabaseVersion = 4U;
    private const uint StarfieldMaterialDatabaseStringTableSignature = 0x54525453U;
    private const int MaxStarfieldMaterialParentDepth = 8;
    private static readonly byte[] StarfieldMaterialDatabaseBethSignatureBytes = [0x42, 0x45, 0x54, 0x48, 0x08, 0x00, 0x00, 0x00];
    private static readonly string[] StarfieldMaterialDatabasePaths =
    {
        "materials/materialsbeta.cdb",
        "materials/creations/sfbgs003/materialsbeta.cdb",
        "materials/creations/sfbgs007/materialsbeta.cdb",
        "materials/creations/sfbgs008/materialsbeta.cdb"
    };

    public NifPreviewReadResult TryRead(NifPreviewReadRequest request)
    {
        try
        {
            var reader = new NifBinaryReader(request.Data);
            var header = ReadHeader(reader);
            var model = new NifPreviewModel
            {
                DisplayName = request.DisplayName,
                SourcePath = request.SourcePath
            };
            var diagnostics = new List<string>
            {
                $"Header user {header.UserVersion}, Bethesda {header.BethesdaVersion}, blocks {header.Blocks.Count}"
            };
            var resolveExternalAsset = CreateCachingExternalAssetResolver(request.ResolveExternalAsset, diagnostics);

            string? rejectionReason = null;
            var materialMap = MeasurePreviewPhase(
                "material map",
                diagnostics,
                () => CreateMaterialMap(header.Blocks, header.Strings, diagnostics, resolveExternalAsset));
            var localTransforms = CreateLocalTransformMap(header.Blocks, diagnostics);
            var parentByChild = CreateParentMap(header.Blocks, diagnostics);
            var geometryStopwatch = Stopwatch.StartNew();
            for (var blockIndex = 0; blockIndex < header.Blocks.Count; blockIndex++)
            {
                var block = header.Blocks[blockIndex];
                var transformChain = GetTransformChain(blockIndex, localTransforms, parentByChild);
                var mesh = TryReadPreviewGeometry(block, blockIndex, model.Meshes.Count, transformChain, header.Strings, materialMap, header.BethesdaVersion, resolveExternalAsset, ref rejectionReason);
                if (mesh != null)
                {
                    model.Meshes.Add(mesh);
                    foreach (var diagnostic in mesh.Diagnostics)
                    {
                        diagnostics.Add(diagnostic);
                    }
                }
            }
            geometryStopwatch.Stop();
            diagnostics.Add($"NIF preview geometry pass completed in {geometryStopwatch.ElapsedMilliseconds.ToString("N0", CultureInfo.InvariantCulture)} ms");

            if (model.Meshes.Count == 0)
            {
                var statusMessage = "No supported preview geometry was found in this NIF.";
                var blockSummary = CreateBlockTypeSummary(header.Blocks);
                if (!string.IsNullOrWhiteSpace(blockSummary))
                {
                    statusMessage += $" Block types: {blockSummary}.";
                }

                if (!string.IsNullOrWhiteSpace(rejectionReason))
                {
                    statusMessage += $" Closest candidate: {rejectionReason}";
                }

                if (HasUnsupportedSkinnedGeometry(header.Blocks))
                {
                    statusMessage += " Skinned or partitioned NIF geometry is not supported by the first preview reader yet.";
                }

                return Failure(statusMessage, diagnostics);
            }

            return new NifPreviewReadResult
            {
                IsSuccess = true,
                Model = model,
                StatusMessage = $"Loaded {model.Meshes.Count} preview mesh(es).",
                Diagnostics = diagnostics
            };
        }
        catch (Exception exception) when (exception is InvalidDataException or EndOfStreamException or ArgumentOutOfRangeException)
        {
            return Failure(exception.Message);
        }
    }

    private static Func<string, byte[]?>? CreateCachingExternalAssetResolver(
        Func<string, byte[]?>? resolveExternalAsset,
        List<string> diagnostics)
    {
        if (resolveExternalAsset == null)
        {
            return null;
        }

        var cache = new Dictionary<string, byte[]?>(StringComparer.OrdinalIgnoreCase);
        return assetPath =>
        {
            var cacheKey = NormalizeExternalAssetCacheKey(assetPath);
            if (cache.TryGetValue(cacheKey, out var cachedData))
            {
                diagnostics.Add($"External asset cache hit for {assetPath}");
                return cachedData;
            }

            var stopwatch = Stopwatch.StartNew();
            var data = resolveExternalAsset(assetPath);
            stopwatch.Stop();
            cache[cacheKey] = data;
            diagnostics.Add(data == null || data.Length == 0
                ? $"External asset {assetPath} was not resolved in {stopwatch.ElapsedMilliseconds.ToString("N0", CultureInfo.InvariantCulture)} ms"
                : $"External asset {assetPath} resolved {data.Length.ToString("N0", CultureInfo.InvariantCulture)} byte(s) in {stopwatch.ElapsedMilliseconds.ToString("N0", CultureInfo.InvariantCulture)} ms");
            return data;
        };
    }

    private static string NormalizeExternalAssetCacheKey(string assetPath)
    {
        return assetPath.Trim().Replace('/', '\\');
    }

    private static T MeasurePreviewPhase<T>(string phaseName, List<string> diagnostics, Func<T> readPhase)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = readPhase();
        stopwatch.Stop();
        diagnostics.Add($"NIF preview {phaseName} completed in {stopwatch.ElapsedMilliseconds.ToString("N0", CultureInfo.InvariantCulture)} ms");
        return result;
    }






    private static string CreateBlockTypeSummary(IReadOnlyList<NifBlock> blocks)
    {
        return string.Join(
            ", ",
            blocks
                .GroupBy(block => block.TypeName)
                .OrderBy(group => group.Key, StringComparer.Ordinal)
                .Take(12)
                .Select(group => $"{group.Key} x{group.Count()}"));
    }

    private static NifPreviewReadResult Failure(string statusMessage, IList<string>? diagnostics = null)
    {
        return new NifPreviewReadResult
        {
            IsSuccess = false,
            StatusMessage = statusMessage,
            Diagnostics = diagnostics ?? []
        };
    }

}
