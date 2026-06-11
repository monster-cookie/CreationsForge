using System.Buffers.Binary;
using System.Globalization;
using System.Text;

namespace CreationsForge.Bethesda.Assets.Nif;

public class NifPreviewModelReader : INifPreviewModelReader
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
    private const float StarfieldGeometryMeshPositionScale = 1f / 32767f;
    private const ulong StarfieldMaterialDatabaseBethSignature = 0x0000000848544542UL;
    private const uint StarfieldMaterialDatabaseVersion = 4U;
    private const uint StarfieldMaterialDatabaseStringTableSignature = 0x54525453U;
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

            string? rejectionReason = null;
            var materialMap = CreateMaterialMap(header.Blocks, header.Strings, diagnostics, request.ResolveExternalAsset);
            var localTransforms = CreateLocalTransformMap(header.Blocks, diagnostics);
            var parentByChild = CreateParentMap(header.Blocks, diagnostics);
            for (var blockIndex = 0; blockIndex < header.Blocks.Count; blockIndex++)
            {
                var block = header.Blocks[blockIndex];
                var transformChain = GetTransformChain(blockIndex, localTransforms, parentByChild);
                var mesh = TryReadPreviewGeometry(block, blockIndex, model.Meshes.Count, transformChain, header.Strings, materialMap, header.BethesdaVersion, request.ResolveExternalAsset, ref rejectionReason);
                if (mesh != null)
                {
                    model.Meshes.Add(mesh);
                    foreach (var diagnostic in mesh.Diagnostics)
                    {
                        diagnostics.Add(diagnostic);
                    }
                }
            }

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

    private static NifHeader ReadHeader(NifBinaryReader reader)
    {
        var headerString = reader.ReadLineString();
        if (!headerString.StartsWith("Gamebryo File Format", StringComparison.Ordinal) &&
            !headerString.StartsWith("NetImmerse File Format", StringComparison.Ordinal))
        {
            throw new InvalidDataException("The file is not a recognized NIF stream.");
        }

        var version = reader.ReadUInt32();
        if (version != Fallout4Version)
        {
            throw new InvalidDataException($"NIF version 0x{version:X8} is not supported by the first preview reader.");
        }

        var endianType = reader.ReadByte();
        if (endianType != 1)
        {
            throw new InvalidDataException("Only little-endian NIF streams are supported.");
        }

        var userVersion = reader.ReadUInt32();
        var blockCount = reader.ReadUInt32();
        if (blockCount > MaxReasonableBlockCount)
        {
            throw new InvalidDataException("NIF block count is outside the supported range.");
        }

        var bethesdaVersion = reader.ReadUInt32();
        if (!IsSupportedBethesdaStreamVersion(userVersion, bethesdaVersion))
        {
            throw new InvalidDataException($"Bethesda stream version {bethesdaVersion} is not supported by the first preview reader.");
        }

        if (TryFindHeaderTables(reader, userVersion, bethesdaVersion, blockCount, out var header, out var failureReason))
        {
            return header;
        }

        throw new InvalidDataException($"NIF header tables could not be located with a supported Bethesda header layout. {failureReason}");
    }

    private static bool IsSupportedBethesdaStreamVersion(uint userVersion, uint bethesdaVersion)
    {
        return bethesdaVersion >= Fallout4BethesdaVersion ||
            userVersion == SkyrimSpecialEditionUserVersion && bethesdaVersion == SkyrimSpecialEditionBethesdaVersion;
    }

    private static bool TryFindHeaderTables(
        NifBinaryReader reader,
        uint userVersion,
        uint bethesdaVersion,
        uint blockCount,
        out NifHeader header,
        out string failureReason)
    {
        header = new NifHeader(userVersion, bethesdaVersion, [], []);
        failureReason = "No plausible block type table was found.";
        var searchStart = reader.Position;
        var searchEnd = searchStart + Math.Min(reader.Remaining, MaxHeaderTableSearchBytes);

        for (var position = searchStart; position <= searchEnd - sizeof(ushort); position++)
        {
            var candidateReader = reader;
            candidateReader.Seek(position);
            if (TryReadHeaderTables(candidateReader, userVersion, bethesdaVersion, blockCount, out header, out var candidateFailureReason))
            {
                return true;
            }

            if (!string.IsNullOrWhiteSpace(candidateFailureReason))
            {
                failureReason = candidateFailureReason;
            }
        }

        return false;
    }

    private static bool TryReadHeaderTables(
        NifBinaryReader reader,
        uint userVersion,
        uint bethesdaVersion,
        uint blockCount,
        out NifHeader header,
        out string failureReason)
    {
        header = new NifHeader(userVersion, bethesdaVersion, [], []);
        failureReason = string.Empty;

        try
        {
            var blockTypeCount = reader.ReadUInt16();
            if (blockTypeCount == 0 || blockTypeCount > MaxReasonableBlockTypeCount)
            {
                return false;
            }

            var blockTypes = new List<string>();
            for (var index = 0; index < blockTypeCount; index++)
            {
                var blockType = reader.ReadSizedString();
                if (!IsPlausibleBlockTypeName(blockType))
                {
                    return false;
                }

                blockTypes.Add(blockType);
            }

            if (!blockTypes.Any(IsLikelyNifBlockTypeName))
            {
                return false;
            }

            var blockTypeIndexes = new List<ushort>();
            for (var index = 0; index < blockCount; index++)
            {
                var blockTypeIndex = reader.ReadUInt16();
                if (blockTypeIndex >= blockTypes.Count)
                {
                    throw new InvalidDataException("NIF block type index is outside the block type table.");
                }

                blockTypeIndexes.Add(blockTypeIndex);
            }

            var blockSizes = new List<uint>();
            for (var index = 0; index < blockCount; index++)
            {
                blockSizes.Add(reader.ReadUInt32());
            }

            if (!TryReadStringTable(ref reader, out var strings) || !TrySkipGroupTable(ref reader))
            {
                return false;
            }

            var blocks = new List<NifBlock>();
            for (var index = 0; index < blockCount; index++)
            {
                var blockSize = blockSizes[index];
                if (blockSize > int.MaxValue || reader.Remaining < blockSize)
                {
                    throw new InvalidDataException("NIF block payload size is outside the stream.");
                }

                blocks.Add(new NifBlock(blockTypes[blockTypeIndexes[index]], reader.ReadBytes((int)blockSize)));
            }

            header = new NifHeader(userVersion, bethesdaVersion, strings, blocks);
            return true;
        }
        catch (Exception exception) when (exception is InvalidDataException or EndOfStreamException or ArgumentOutOfRangeException)
        {
            failureReason = exception.Message;
            return false;
        }
    }

    private static bool IsPlausibleBlockTypeName(string value)
    {
        return value.Length > 0 &&
            value.Length <= 128 &&
            value.All(character => character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or ' ' or '_' or ':');
    }

    private static bool IsLikelyNifBlockTypeName(string value)
    {
        return value.StartsWith("Ni", StringComparison.Ordinal) ||
            value.StartsWith("BS", StringComparison.Ordinal) ||
            value.StartsWith("bhk", StringComparison.Ordinal);
    }

    private static bool TryReadStringTable(ref NifBinaryReader reader, out IReadOnlyList<string> strings)
    {
        strings = [];
        var stringCount = reader.ReadUInt32();
        var maxStringLength = reader.ReadUInt32();
        if (maxStringLength > MaxReasonableStringLength)
        {
            return false;
        }

        var parsedStrings = new List<string>();
        for (var index = 0; index < stringCount; index++)
        {
            if (!reader.TryReadSizedString(out var value))
            {
                return false;
            }

            parsedStrings.Add(value);
        }

        strings = parsedStrings;
        return true;
    }

    private static bool TrySkipGroupTable(ref NifBinaryReader reader)
    {
        var groupCount = reader.ReadUInt32();
        for (var index = 0; index < groupCount; index++)
        {
            reader.ReadUInt32();
        }

        return true;
    }

    private static Dictionary<int, NifObjectTransform> CreateLocalTransformMap(IReadOnlyList<NifBlock> blocks, List<string> diagnostics)
    {
        var transforms = new Dictionary<int, NifObjectTransform>();
        for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            var block = blocks[blockIndex];
            if (!IsTransformBearingBlock(block))
            {
                continue;
            }

            if (TryReadNiAVObjectHeader(block.Data, out var transform, out var position, out var failureReason))
            {
                transforms[blockIndex] = transform;
                diagnostics.Add($"{block.TypeName} block {blockIndex}: object transform header at {position}, {transform.Description}");
            }
            else
            {
                diagnostics.Add($"{block.TypeName} block {blockIndex}: object transform header not parsed ({failureReason}), first bytes {CreateHexSample(block.Data, 0, DiagnosticHexByteCount)}");
            }
        }

        return transforms;
    }

    private static Dictionary<int, int> CreateParentMap(IReadOnlyList<NifBlock> blocks, List<string> diagnostics)
    {
        var parents = new Dictionary<int, int>();
        for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            var block = blocks[blockIndex];
            var failureReason = string.Empty;
            if (!block.TypeName.Contains("Node", StringComparison.Ordinal) ||
                !TryReadNiNodeChildren(block.Data, out var children, out failureReason))
            {
                if (block.TypeName.Contains("Node", StringComparison.Ordinal))
                {
                    diagnostics.Add($"{block.TypeName} block {blockIndex}: children not parsed ({failureReason})");
                }

                continue;
            }

            diagnostics.Add($"{block.TypeName} block {blockIndex}: children {string.Join(", ", children)}");
            foreach (var child in children)
            {
                if (child >= 0 && child < blocks.Count && !parents.ContainsKey(child))
                {
                    parents[child] = blockIndex;
                }
            }
        }

        return parents;
    }

    private static bool IsTransformBearingBlock(NifBlock block)
    {
        return block.TypeName.Contains("Node", StringComparison.Ordinal) ||
            IsPreviewGeometryBlock(block);
    }

    private static bool IsPreviewGeometryBlock(NifBlock block)
    {
        return block.TypeName.Contains("TriShape", StringComparison.Ordinal) ||
            string.Equals(block.TypeName, "BSGeometry", StringComparison.Ordinal);
    }

    private static Dictionary<int, NifMaterialInfo> CreateMaterialMap(
        IReadOnlyList<NifBlock> blocks,
        IReadOnlyList<string> strings,
        List<string> diagnostics,
        Func<string, byte[]?>? resolveExternalAsset)
    {
        var textureSets = CreateTextureSetMap(blocks, diagnostics);
        var materials = new Dictionary<int, NifMaterialInfo>();
        for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            var block = blocks[blockIndex];
            if (!block.TypeName.Contains("ShaderProperty", StringComparison.Ordinal))
            {
                continue;
            }

            var materialName = TryReadStringRef(block.Data, 0, strings, out var parsedName)
                ? parsedName
                : $"{block.TypeName} {blockIndex}";
            var texturePath = TryFindTexturePath(block.Data, out var inlineTexturePath)
                ? inlineTexturePath
                : TryFindTextureSetReference(blocks, block.Data, out var textureSetBlockIndex) &&
                    textureSets.TryGetValue(textureSetBlockIndex, out var textureSetPath)
                    ? textureSetPath
                    : null;
            if (texturePath == null &&
                IsMaterialPath(materialName) &&
                TryResolveMaterialTexturePath(materialName, resolveExternalAsset, out var materialTexturePath, out var materialDiagnostic))
            {
                texturePath = materialTexturePath;
                diagnostics.Add($"{block.TypeName} block {blockIndex}: {materialDiagnostic}");
            }
            else if (texturePath == null && IsMaterialPath(materialName))
            {
                diagnostics.Add($"{block.TypeName} block {blockIndex}: material {materialName} did not provide a preview texture");
            }

            materials[blockIndex] = new NifMaterialInfo(materialName, texturePath);
            diagnostics.Add($"{block.TypeName} block {blockIndex}: material {materialName}, texture {texturePath ?? "none"}");
        }

        return materials;
    }

    private static Dictionary<int, string> CreateTextureSetMap(IReadOnlyList<NifBlock> blocks, List<string> diagnostics)
    {
        var textureSets = new Dictionary<int, string>();
        for (var blockIndex = 0; blockIndex < blocks.Count; blockIndex++)
        {
            var block = blocks[blockIndex];
            if (!block.TypeName.Contains("TextureSet", StringComparison.Ordinal))
            {
                continue;
            }

            if (TryReadTextureSetPath(block.Data, out var texturePath))
            {
                textureSets[blockIndex] = texturePath;
                diagnostics.Add($"{block.TypeName} block {blockIndex}: texture {texturePath}");
            }
            else
            {
                diagnostics.Add($"{block.TypeName} block {blockIndex}: no preview texture path parsed");
            }
        }

        return textureSets;
    }

    private static bool TryFindTextureSetReference(IReadOnlyList<NifBlock> blocks, byte[] data, out int textureSetBlockIndex)
    {
        textureSetBlockIndex = -1;
        for (var position = 0; position <= data.Length - sizeof(int); position += sizeof(int))
        {
            var candidate = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(position, sizeof(int)));
            if (candidate >= 0 &&
                candidate < blocks.Count &&
                blocks[candidate].TypeName.Contains("TextureSet", StringComparison.Ordinal))
            {
                textureSetBlockIndex = candidate;
                return true;
            }
        }

        return false;
    }

    private static bool TryReadTextureSetPath(byte[] data, out string texturePath)
    {
        texturePath = string.Empty;
        if (data.Length < sizeof(uint))
        {
            return false;
        }

        var position = 0;
        if (!TryReadUInt32(data, ref position, out var textureCount) || textureCount > 32)
        {
            return false;
        }

        for (var index = 0; index < textureCount; index++)
        {
            if (!TryReadSizedString(data, ref position, out var candidate))
            {
                return false;
            }

            if (IsTexturePath(candidate))
            {
                texturePath = candidate;
                return true;
            }
        }

        return false;
    }

    private static bool TryFindTexturePath(byte[] data, out string texturePath)
    {
        texturePath = string.Empty;
        for (var position = 0; position <= data.Length - sizeof(uint); position++)
        {
            var candidatePosition = position;
            if (TryReadSizedString(data, ref candidatePosition, out var candidate) && IsTexturePath(candidate))
            {
                texturePath = candidate;
                return true;
            }
        }

        return false;
    }

    private static bool TryResolveMaterialTexturePath(
        string materialPath,
        Func<string, byte[]?>? resolveExternalAsset,
        out string texturePath,
        out string diagnostic)
    {
        texturePath = string.Empty;
        diagnostic = string.Empty;
        if (resolveExternalAsset == null)
        {
            diagnostic = $"material {materialPath} could not be resolved because no external asset resolver is available";
            return false;
        }

        var materialData = resolveExternalAsset(materialPath);
        if (materialData == null || materialData.Length == 0)
        {
            diagnostic = $"material {materialPath} could not be resolved";
            return false;
        }

        var candidates = GetMaterialTextureCandidates(materialData)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(GetPreviewTextureCandidateScore)
            .ThenBy(candidate => candidate, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var unsupportedFeatures = GetUnsupportedMaterialFeatureSummary(materialData);
        if (candidates.Count == 0)
        {
            diagnostic = string.IsNullOrWhiteSpace(unsupportedFeatures)
                ? $"material {materialPath} resolved but no DDS preview texture reference was found"
                : $"material {materialPath} resolved but no DDS preview texture reference was found; unsupported material features: {unsupportedFeatures}";
            return false;
        }

        var textureSource = "material";
        if (TryFindMaterialDatabaseTextureCandidate(candidates, resolveExternalAsset, out var databaseTexturePath))
        {
            texturePath = databaseTexturePath;
            textureSource = "material database";
        }
        else
        {
            texturePath = candidates[0];
        }

        diagnostic = string.IsNullOrWhiteSpace(unsupportedFeatures)
            ? $"material {materialPath} resolved preview texture {texturePath} from {textureSource}"
            : $"material {materialPath} resolved preview texture {texturePath} from {textureSource}; unsupported material features: {unsupportedFeatures}";
        return true;
    }

    private static bool TryFindMaterialDatabaseTextureCandidate(
        IReadOnlyList<string> materialTextureCandidates,
        Func<string, byte[]?> resolveExternalAsset,
        out string texturePath)
    {
        texturePath = string.Empty;
        var materialFileNames = materialTextureCandidates
            .Select(Path.GetFileName)
            .Where(fileName => !string.IsNullOrWhiteSpace(fileName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (materialFileNames.Count == 0)
        {
            return false;
        }

        foreach (var databasePath in StarfieldMaterialDatabasePaths)
        {
            var databaseData = resolveExternalAsset(databasePath);
            if (databaseData == null ||
                !TryReadStarfieldMaterialDatabaseStrings(databaseData, out var strings))
            {
                continue;
            }

            var candidates = strings
                .SelectMany(value => GetMaterialTextureCandidates(Encoding.UTF8.GetBytes(value)))
                .Where(candidate => materialFileNames.Contains(Path.GetFileName(candidate)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(GetPreviewTextureCandidateScore)
                .ThenBy(candidate => candidate, StringComparer.OrdinalIgnoreCase)
                .ToList();
            var correctedCandidate = candidates.FirstOrDefault(candidate =>
                !materialTextureCandidates.Contains(candidate, StringComparer.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(correctedCandidate))
            {
                texturePath = correctedCandidate;
                return true;
            }
        }

        return false;
    }

    private static bool TryReadStarfieldMaterialDatabaseStrings(byte[] data, out IReadOnlyList<string> strings)
    {
        strings = [];
        if (data.Length < 24)
        {
            return false;
        }

        var position = 0;
        var bethSignature = BinaryPrimitives.ReadUInt64LittleEndian(data.AsSpan(position, sizeof(ulong)));
        position += sizeof(ulong);
        if (bethSignature != StarfieldMaterialDatabaseBethSignature)
        {
            return false;
        }

        var version = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position, sizeof(uint)));
        position += sizeof(uint);
        if (version != StarfieldMaterialDatabaseVersion)
        {
            return false;
        }

        position += sizeof(uint);
        var stringTableSignature = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position, sizeof(uint)));
        position += sizeof(uint);
        if (stringTableSignature != StarfieldMaterialDatabaseStringTableSignature)
        {
            return false;
        }

        var byteCount = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position, sizeof(uint)));
        position += sizeof(uint);
        if (byteCount > data.Length - position)
        {
            return false;
        }

        var parsedStrings = new List<string>();
        var start = position;
        var end = position + (int)byteCount;
        for (var index = position; index < end; index++)
        {
            if (data[index] != 0)
            {
                continue;
            }

            if (index > start)
            {
                parsedStrings.Add(Encoding.UTF8.GetString(data, start, index - start));
            }

            start = index + 1;
        }

        if (start < end)
        {
            parsedStrings.Add(Encoding.UTF8.GetString(data, start, end - start));
        }

        strings = parsedStrings;
        return strings.Count > 0;
    }

    private static IEnumerable<string> GetMaterialTextureCandidates(byte[] data)
    {
        foreach (var candidate in GetMaterialStringCandidates(data))
        {
            if (TryNormalizeTexturePathCandidate(candidate, out var texturePath))
            {
                yield return texturePath;
            }
        }
    }

    private static IEnumerable<string> GetMaterialStringCandidates(byte[] data)
    {
        for (var position = 0; position <= data.Length - sizeof(uint); position++)
        {
            var candidatePosition = position;
            if (TryReadSizedString(data, ref candidatePosition, out var candidate) &&
                !string.IsNullOrWhiteSpace(candidate))
            {
                yield return candidate;
            }
        }

        foreach (var candidate in GetPrintableAsciiStrings(data))
        {
            yield return candidate;
        }
    }

    private static IEnumerable<string> GetPrintableAsciiStrings(byte[] data)
    {
        var start = -1;
        for (var index = 0; index <= data.Length; index++)
        {
            var isPrintable = index < data.Length && data[index] is >= 0x20 and <= 0x7E;
            if (isPrintable && start < 0)
            {
                start = index;
            }
            else if (!isPrintable && start >= 0)
            {
                var length = index - start;
                if (length >= 4)
                {
                    yield return Encoding.ASCII.GetString(data, start, length);
                }

                start = -1;
            }
        }
    }

    private static bool TryNormalizeTexturePathCandidate(string candidate, out string texturePath)
    {
        texturePath = string.Empty;
        var normalized = candidate.Trim().Replace('/', '\\');
        while (normalized.Contains("\\\\", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("\\\\", "\\", StringComparison.Ordinal);
        }

        var ddsIndex = normalized.IndexOf(".dds", StringComparison.OrdinalIgnoreCase);
        if (ddsIndex < 0)
        {
            return false;
        }

        var textureRootIndex = normalized.IndexOf("textures\\", StringComparison.OrdinalIgnoreCase);
        if (textureRootIndex < 0 || textureRootIndex > ddsIndex)
        {
            return false;
        }

        texturePath = normalized[textureRootIndex..(ddsIndex + 4)];
        return IsTexturePath(texturePath);
    }

    private static int GetPreviewTextureCandidateScore(string texturePath)
    {
        var fileName = Path.GetFileName(texturePath);
        var score = 0;
        if (fileName.Contains("diffuse", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("albedo", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("basecolor", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("color", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("_d.", StringComparison.OrdinalIgnoreCase))
        {
            score += 100;
        }

        if (fileName.Contains("normal", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("_n.", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("rough", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("metal", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("mask", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("opacity", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("height", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("emissive", StringComparison.OrdinalIgnoreCase) ||
            fileName.Contains("glow", StringComparison.OrdinalIgnoreCase))
        {
            score -= 50;
        }

        return score;
    }

    private static string GetUnsupportedMaterialFeatureSummary(byte[] data)
    {
        var features = new List<string>();
        foreach (var candidate in GetMaterialStringCandidates(data))
        {
            AddFeatureIfPresent(features, candidate, "Decal", "decal");
            AddFeatureIfPresent(features, candidate, "Glass", "glass");
            AddFeatureIfPresent(features, candidate, "Effect", "effect");
            AddFeatureIfPresent(features, candidate, "Opacity", "opacity");
            AddFeatureIfPresent(features, candidate, "EdgeFalloff", "edge falloff");
            AddFeatureIfPresent(features, candidate, "Layered", "layered");
        }

        return string.Join(", ", features.Distinct(StringComparer.OrdinalIgnoreCase));
    }

    private static void AddFeatureIfPresent(List<string> features, string value, string token, string displayName)
    {
        if (value.Contains(token, StringComparison.OrdinalIgnoreCase))
        {
            features.Add(displayName);
        }
    }

    private static bool TryReadStringRef(byte[] data, int position, IReadOnlyList<string> strings, out string value)
    {
        value = string.Empty;
        if (data.Length - position < sizeof(uint))
        {
            return false;
        }

        var stringIndex = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position, sizeof(uint)));
        if (stringIndex >= strings.Count)
        {
            return false;
        }

        value = strings[(int)stringIndex];
        return !string.IsNullOrWhiteSpace(value);
    }

    private static bool TryReadSizedString(byte[] data, ref int position, out string value)
    {
        value = string.Empty;
        if (!TryReadUInt32(data, ref position, out var length) ||
            length > MaxReasonableStringLength ||
            data.Length - position < length)
        {
            return false;
        }

        value = Encoding.UTF8.GetString(data, position, (int)length);
        position += (int)length;
        return true;
    }

    private static bool IsTexturePath(string value)
    {
        return value.StartsWith("textures\\", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("textures/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsMaterialPath(string value)
    {
        return (value.StartsWith("materials\\", StringComparison.OrdinalIgnoreCase) ||
                value.StartsWith("materials/", StringComparison.OrdinalIgnoreCase)) &&
            value.EndsWith(".mat", StringComparison.OrdinalIgnoreCase);
    }

    private static IReadOnlyList<NifObjectTransform> GetTransformChain(
        int blockIndex,
        IReadOnlyDictionary<int, NifObjectTransform> localTransforms,
        IReadOnlyDictionary<int, int> parentByChild)
    {
        var transforms = new List<NifObjectTransform>();
        var visited = new HashSet<int>();
        var current = blockIndex;
        while (visited.Add(current))
        {
            if (localTransforms.TryGetValue(current, out var transform))
            {
                transforms.Add(transform);
            }

            if (!parentByChild.TryGetValue(current, out current))
            {
                break;
            }
        }

        transforms.Reverse();
        return transforms;
    }

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
                UV = new NifPreviewUV()
            });
        }

        var material = GetStarfieldExternalGeometryMaterial(block.Data, block.TypeName, blockIndex, meshIndex, strings, materialMap);
        mesh = new NifPreviewMesh
        {
            Name = material.MeshName,
            MaterialName = material.MaterialName,
            TexturePath = material.TexturePath,
            Vertices = vertices,
            Indices = indices,
            Diagnostics =
            {
                $"{block.TypeName} block {blockIndex}: external Starfield geometry {geometryPath}, {vertexCount} vertices, {indexCount / 3} triangles, position stride {StarfieldGeometryMeshPositionStride}, geometry bounds metadata {(hasBounds ? geometryBounds.Description : "unavailable")}, decoded bounds {bounds.Description}",
                $"{block.TypeName} block {blockIndex} external material: {material.MaterialName}, texture {material.TexturePath ?? "none"}",
                $"{block.TypeName} block {blockIndex} triangle sample: {CreateTriangleSample(indices)}"
            }
        };
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
            Vertices = vertices,
            Indices = indices,
            Diagnostics =
            {
                $"{block.TypeName} block {blockIndex} offset {offset}: {vertexCount} vertices, {triangleCount} triangles, count layout {countLayout}, count offset {countProbeOffset}, position format {positionFormat}, descriptor 0x{vertexDesc:X16}, {descriptor.Description}, transform {GetTransformDescription(useTransform, descriptorAlignedTransformChain, transformSource)}, raw {rawBounds.Description}, {triangleQuality.Description}",
                $"{block.TypeName} block {blockIndex} normals: {normalQuality.Description}",
                $"{block.TypeName} block {blockIndex} material: {material.MaterialName}, texture {material.TexturePath ?? "none"}",
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
                return new NifShapeMaterial(meshName, material.MaterialName, material.TexturePath);
            }
        }

        return new NifShapeMaterial(meshName, $"{blockTypeName} {blockIndex}", null);
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
            return new NifShapeMaterial(meshName, $"{blockTypeName} {blockIndex}", null);
        }

        var shaderPropertyPosition = position + (sizeof(float) * 10) + sizeof(int);
        var shaderPropertyBlockIndex = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(shaderPropertyPosition, sizeof(int)));
        if (materialMap.TryGetValue(shaderPropertyBlockIndex, out var material))
        {
            return new NifShapeMaterial(meshName, material.MaterialName, material.TexturePath);
        }

        return new NifShapeMaterial(meshName, $"{blockTypeName} {blockIndex}", null);
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

    private static NifPreviewVector3 ReadVector3(byte[] data, ref int position)
    {
        var vector = new NifPreviewVector3
        {
            X = ReadSingle(data, ref position),
            Y = ReadSingle(data, ref position),
            Z = ReadSingle(data, ref position)
        };
        return vector;
    }

    private static float ReadSingle(byte[] data, ref int position)
    {
        var value = BinaryPrimitives.ReadSingleLittleEndian(data.AsSpan(position, sizeof(float)));
        position += sizeof(float);
        return value;
    }

    private static NifPreviewVector3 ReadHalfVector3(byte[] data, ref int position)
    {
        var vector = new NifPreviewVector3
        {
            X = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position, sizeof(ushort)))),
            Y = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position + sizeof(ushort), sizeof(ushort)))),
            Z = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position + (sizeof(ushort) * 2), sizeof(ushort))))
        };
        position += sizeof(ushort) * 3;
        return vector;
    }

    private static NifPreviewUV ReadHalfTexCoord(byte[] data, ref int position)
    {
        var uv = new NifPreviewUV
        {
            U = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position, sizeof(ushort)))),
            V = (float)BitConverter.UInt16BitsToHalf(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position + sizeof(ushort), sizeof(ushort))))
        };
        position += sizeof(ushort) * 2;
        return uv;
    }

    private static NifPreviewVector3 ReadByteVector3(byte[] data, ref int position)
    {
        var vector = new NifPreviewVector3
        {
            X = ReadNormalizedByte(data[position]),
            Y = ReadNormalizedByte(data[position + 1]),
            Z = ReadNormalizedByte(data[position + 2])
        };
        position += 4;
        return vector;
    }

    private static float ReadNormalizedByte(byte value)
    {
        return (value / 127.5f) - 1f;
    }

    private static bool IsReasonableCoordinate(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value) && MathF.Abs(value) <= MaxReasonableCoordinate;
    }

    private static bool IsReasonableScale(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f && value <= 10000f;
    }

    private static bool TryReadUInt16(byte[] data, ref int position, out ushort value)
    {
        value = 0;
        if (data.Length - position < sizeof(ushort))
        {
            return false;
        }

        value = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(position, sizeof(ushort)));
        position += sizeof(ushort);
        return true;
    }

    private static bool TryReadUInt32(byte[] data, ref int position, out uint value)
    {
        value = 0;
        if (data.Length - position < sizeof(uint))
        {
            return false;
        }

        value = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position, sizeof(uint)));
        position += sizeof(uint);
        return true;
    }

    private static bool TryReadInt32(byte[] data, ref int position, out int value)
    {
        value = 0;
        if (data.Length - position < sizeof(int))
        {
            return false;
        }

        value = BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(position, sizeof(int)));
        position += sizeof(int);
        return true;
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

    private readonly struct NifHeader
    {
        public NifHeader(uint userVersion, uint bethesdaVersion, IReadOnlyList<string> strings, IReadOnlyList<NifBlock> blocks)
        {
            UserVersion = userVersion;
            BethesdaVersion = bethesdaVersion;
            Strings = strings;
            Blocks = blocks;
        }

        public uint UserVersion { get; }

        public uint BethesdaVersion { get; }

        public IReadOnlyList<string> Strings { get; }

        public IReadOnlyList<NifBlock> Blocks { get; }
    }

    private readonly struct NifMaterialInfo
    {
        public NifMaterialInfo(string materialName, string? texturePath)
        {
            MaterialName = materialName;
            TexturePath = texturePath;
        }

        public string MaterialName { get; }

        public string? TexturePath { get; }
    }

    private readonly struct NifShapeMaterial
    {
        public NifShapeMaterial(string meshName, string materialName, string? texturePath)
        {
            MeshName = meshName;
            MaterialName = materialName;
            TexturePath = texturePath;
        }

        public string MeshName { get; }

        public string MaterialName { get; }

        public string? TexturePath { get; }
    }

    private readonly struct NifBlock
    {
        public NifBlock(string typeName, byte[] data)
        {
            TypeName = typeName;
            Data = data;
        }

        public string TypeName { get; }

        public byte[] Data { get; }
    }

    private struct NifMeshBounds
    {
        private float MinX;
        private float MinY;
        private float MinZ;
        private float MaxX;
        private float MaxY;
        private float MaxZ;
        private bool HasPosition;

        public float LongestAxis => MathF.Max(MaxX - MinX, MathF.Max(MaxY - MinY, MaxZ - MinZ));

        public float ShapeScore
        {
            get
            {
                var x = MaxX - MinX;
                var y = MaxY - MinY;
                var z = MaxZ - MinZ;
                var longest = MathF.Max(x, MathF.Max(y, z));
                var shortest = MathF.Min(x, MathF.Min(y, z));
                return longest <= 0f ? 0f : shortest / longest;
            }
        }

        public string Description => HasPosition
            ? $"X {MinX:N3}..{MaxX:N3}, Y {MinY:N3}..{MaxY:N3}, Z {MinZ:N3}..{MaxZ:N3}"
            : "empty";

        public bool TryInclude(NifPreviewVector3 position, out string rejectionReason)
        {
            rejectionReason = string.Empty;
            if (!IsReasonableCoordinate(position.X) ||
                !IsReasonableCoordinate(position.Y) ||
                !IsReasonableCoordinate(position.Z))
            {
                rejectionReason = $"vertex position was outside the supported preview range ({position.X}, {position.Y}, {position.Z})";
                return false;
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
                return true;
            }

            MinX = MathF.Min(MinX, position.X);
            MinY = MathF.Min(MinY, position.Y);
            MinZ = MathF.Min(MinZ, position.Z);
            MaxX = MathF.Max(MaxX, position.X);
            MaxY = MathF.Max(MaxY, position.Y);
            MaxZ = MathF.Max(MaxZ, position.Z);
            return true;
        }

        public bool IsUsefulPreviewBounds(NifMeshBounds fallbackBounds)
        {
            return HasPosition &&
                LongestAxis > 0.0001f &&
                LongestAxis < fallbackBounds.LongestAxis * 1000f;
        }
    }

    private readonly struct NifTriangleQuality
    {
        public NifTriangleQuality(int triangleCount, float averageEdgeRatio, float maxEdgeRatio, int degenerateCount)
        {
            TriangleCount = triangleCount;
            AverageEdgeRatio = averageEdgeRatio;
            MaxEdgeRatio = maxEdgeRatio;
            DegenerateCount = degenerateCount;
        }

        public int TriangleCount { get; }

        public float AverageEdgeRatio { get; }

        public float MaxEdgeRatio { get; }

        public int DegenerateCount { get; }

        public string Description =>
            $"edge ratios avg {AverageEdgeRatio:N3}, max {MaxEdgeRatio:N3}, degenerate {DegenerateCount}/{TriangleCount}";
    }

    private readonly struct NifNormalQuality
    {
        public NifNormalQuality(int count, int validCount, float minLength, float maxLength)
        {
            Count = count;
            ValidCount = validCount;
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public int Count { get; }

        public int ValidCount { get; }

        public float MinLength { get; }

        public float MaxLength { get; }

        public string Description =>
            $"{ValidCount}/{Count} valid, length {MinLength:N3}..{MaxLength:N3}";
    }

    private readonly struct BSVertexDescriptor
    {
        public BSVertexDescriptor(ulong value)
        {
            StrideWords = (int)(value & 0xF);
            UVOffset = (int)((value >> 8) & 0xF) * sizeof(uint);
            NormalOffset = (int)((value >> 16) & 0xF) * sizeof(uint);
            Attributes = (ushort)(value >> 44);
        }

        public int StrideWords { get; }

        public int VertexStride => StrideWords * sizeof(uint);

        public int UVOffset { get; }

        public int NormalOffset { get; }

        public ushort Attributes { get; }

        public bool HasUV => (Attributes & 0x2) != 0 && UVOffset > 0 && UVOffset + sizeof(uint) <= VertexStride;

        public bool HasNormals => (Attributes & 0x8) != 0 && NormalOffset > 0 && NormalOffset + sizeof(uint) <= VertexStride;

        public bool HasFullPrecisionPositions => (Attributes & 0x400) == 0x400;

        public string Description =>
            $"stride words {StrideWords}, stride bytes {VertexStride}, attributes 0x{Attributes:X}, uv offset {UVOffset}, normal offset {NormalOffset}, full precision {HasFullPrecisionPositions}";
    }

    private readonly struct BSGeometryCountProbe
    {
        public BSGeometryCountProbe(uint triangleCount, ushort vertexCount, int vertexDataOffset, int countProbeOffset)
        {
            TriangleCount = triangleCount;
            VertexCount = vertexCount;
            VertexDataOffset = vertexDataOffset;
            CountProbeOffset = countProbeOffset;
        }

        public uint TriangleCount { get; }

        public ushort VertexCount { get; }

        public int VertexDataOffset { get; }

        public int CountProbeOffset { get; }
    }

    private readonly struct StarfieldGeometryBounds
    {
        private readonly NifPreviewVector3 Center;
        private readonly NifPreviewVector3 Extents;

        public StarfieldGeometryBounds(NifPreviewVector3 center, NifPreviewVector3 extents)
        {
            Center = center;
            Extents = extents;
        }

        public bool IsReasonable =>
            IsReasonableCoordinate(Center.X) &&
            IsReasonableCoordinate(Center.Y) &&
            IsReasonableCoordinate(Center.Z) &&
            IsReasonableExtent(Extents.X) &&
            IsReasonableExtent(Extents.Y) &&
            IsReasonableExtent(Extents.Z);

        public string Description => $"center {FormatVector(Center)}, extents {FormatVector(Extents)}";

        private static bool IsReasonableExtent(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && value > 0f && value <= MaxReasonableCoordinate;
        }
    }

    private readonly struct NifObjectTransform
    {
        public static readonly NifObjectTransform Identity = new NifObjectTransform(
            new NifPreviewVector3(),
            NifRotation3x3.Identity,
            1f);

        private readonly NifPreviewVector3 Translation;
        private readonly NifRotation3x3 Rotation;
        private readonly float Scale;

        public NifObjectTransform(NifPreviewVector3 translation, NifRotation3x3 rotation, float scale)
        {
            Translation = translation;
            Rotation = rotation;
            Scale = scale;
        }

        public bool IsIdentity =>
            Translation.X == 0f &&
            Translation.Y == 0f &&
            Translation.Z == 0f &&
            Scale == 1f &&
            Rotation.Equals(NifRotation3x3.Identity);

        public string Description =>
            $"translation {FormatVector(Translation)}, scale {Scale.ToString("0.###", CultureInfo.InvariantCulture)}";

        public NifPreviewVector3 Apply(NifPreviewVector3 position)
        {
            var scaled = new NifPreviewVector3
            {
                X = position.X * Scale,
                Y = position.Y * Scale,
                Z = position.Z * Scale
            };

            return new NifPreviewVector3
            {
                X = (scaled.X * Rotation.M11) + (scaled.Y * Rotation.M21) + (scaled.Z * Rotation.M31) + Translation.X,
                Y = (scaled.X * Rotation.M12) + (scaled.Y * Rotation.M22) + (scaled.Z * Rotation.M32) + Translation.Y,
                Z = (scaled.X * Rotation.M13) + (scaled.Y * Rotation.M23) + (scaled.Z * Rotation.M33) + Translation.Z
            };
        }
    }

    private readonly struct NifRotation3x3
    {
        public static readonly NifRotation3x3 Identity = new NifRotation3x3
        {
            M11 = 1f,
            M22 = 1f,
            M33 = 1f
        };

        public float M11 { get; init; }

        public float M12 { get; init; }

        public float M13 { get; init; }

        public float M21 { get; init; }

        public float M22 { get; init; }

        public float M23 { get; init; }

        public float M31 { get; init; }

        public float M32 { get; init; }

        public float M33 { get; init; }

        public bool IsReasonable =>
            IsReasonableRotationValue(M11) &&
            IsReasonableRotationValue(M12) &&
            IsReasonableRotationValue(M13) &&
            IsReasonableRotationValue(M21) &&
            IsReasonableRotationValue(M22) &&
            IsReasonableRotationValue(M23) &&
            IsReasonableRotationValue(M31) &&
            IsReasonableRotationValue(M32) &&
            IsReasonableRotationValue(M33);

        private static bool IsReasonableRotationValue(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value) && MathF.Abs(value) <= 2f;
        }
    }

    private enum BSTriShapeCountLayout
    {
        Fallout4,
        SkyrimSpecialEdition,
        StarfieldGeometry
    }

    private enum BSVertexPositionFormat
    {
        DescriptorDefault,
        Float3,
        Half3
    }

    private struct NifBinaryReader
    {
        private readonly byte[] Data;

        public NifBinaryReader(byte[] data)
        {
            Data = data;
            Position = 0;
        }

        public int Position { get; private set; }

        public int Remaining => Data.Length - Position;

        public void Seek(int position)
        {
            if (position < 0 || position > Data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            Position = position;
        }

        public byte ReadByte()
        {
            Require(sizeof(byte));
            return Data[Position++];
        }

        public ushort ReadUInt16()
        {
            Require(sizeof(ushort));
            var value = BinaryPrimitives.ReadUInt16LittleEndian(Data.AsSpan(Position, sizeof(ushort)));
            Position += sizeof(ushort);
            return value;
        }

        public uint ReadUInt32()
        {
            Require(sizeof(uint));
            var value = BinaryPrimitives.ReadUInt32LittleEndian(Data.AsSpan(Position, sizeof(uint)));
            Position += sizeof(uint);
            return value;
        }

        public string ReadLineString()
        {
            var start = Position;
            while (Position < Data.Length && Data[Position] != 0x0A)
            {
                Position++;
            }

            if (Position >= Data.Length)
            {
                throw new EndOfStreamException("NIF header string was not terminated.");
            }

            var value = Encoding.ASCII.GetString(Data, start, Position - start);
            Position++;
            return value;
        }

        public string ReadExportString()
        {
            return ReadSizedString();
        }

        public string ReadSizedString()
        {
            if (!TryReadSizedString(out var value))
            {
                throw new InvalidDataException("NIF string length is outside the supported range.");
            }

            return value;
        }

        public bool TryReadSizedString(out string value)
        {
            value = string.Empty;
            if (Remaining < sizeof(uint))
            {
                return false;
            }

            var length = ReadUInt32();
            if (length > MaxReasonableStringLength || Remaining < length)
            {
                return false;
            }

            var bytes = ReadBytes((int)length);
            value = Encoding.UTF8.GetString(bytes);
            return true;
        }

        public byte[] ReadBytes(int count)
        {
            Require(count);
            var bytes = Data.AsSpan(Position, count).ToArray();
            Position += count;
            return bytes;
        }

        private void Require(int count)
        {
            if (count < 0 || Remaining < count)
            {
                throw new EndOfStreamException("NIF stream ended before the expected data could be read.");
            }
        }
    }
}
