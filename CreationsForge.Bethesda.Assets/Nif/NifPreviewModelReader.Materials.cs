using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CreationsForge.Bethesda.Assets.Nif;

public partial class NifPreviewModelReader
{
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
                var materialInfo = new NifMaterialInfo(materialName, texturePath, null, null, StarfieldPreviewColor.White, 1f, 1f, 1f, 1f, StarfieldPreviewUvTransform.Identity, false, false, false);
            if (IsMaterialPath(materialName) &&
                TryResolveMaterialPreviewInfo(materialName, texturePath, resolveExternalAsset, out var resolvedMaterialInfo, out var materialDiagnostic))
            {
                materialInfo = resolvedMaterialInfo;
                diagnostics.Add($"{block.TypeName} block {blockIndex}: {materialDiagnostic}");
            }
            else if (texturePath == null && IsMaterialPath(materialName))
            {
                diagnostics.Add($"{block.TypeName} block {blockIndex}: material {materialName} did not provide a preview texture");
            }

            materials[blockIndex] = materialInfo;
            diagnostics.Add($"{block.TypeName} block {blockIndex}: material {materialName}, texture {materialInfo.TexturePath ?? "none"}, overlay {materialInfo.OverlayTexturePath ?? "none"}, decal {materialInfo.IsDecal}, invisible {materialInfo.IsInvisible}");
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
        var materialDatabaseDiagnostics = new List<string>();
        texturePath = candidates[0];
        if (!IsPreviewColorTextureCandidate(texturePath) &&
            TryFindMaterialDatabaseTextureCandidate(candidates, resolveExternalAsset, materialDatabaseDiagnostics, out var databaseTexturePath))
        {
            texturePath = databaseTexturePath;
            textureSource = "material database";
        }
        else if (IsPreviewColorTextureCandidate(texturePath))
        {
            materialDatabaseDiagnostics.Add($"skipped because material already provided preview color texture {texturePath}");
        }

        diagnostic = string.IsNullOrWhiteSpace(unsupportedFeatures)
            ? $"material {materialPath} resolved preview texture {texturePath} from {textureSource}"
            : $"material {materialPath} resolved preview texture {texturePath} from {textureSource}; unsupported material features: {unsupportedFeatures}";
        if (materialDatabaseDiagnostics.Count > 0)
        {
            diagnostic += $"; material database probe: {string.Join("; ", materialDatabaseDiagnostics)}";
        }

        return true;
    }

    private static bool TryResolveMaterialPreviewInfo(
        string materialPath,
        string? existingTexturePath,
        Func<string, byte[]?>? resolveExternalAsset,
        out NifMaterialInfo materialInfo,
        out string diagnostic)
    {
        materialInfo = new NifMaterialInfo(materialPath, existingTexturePath, null, null, StarfieldPreviewColor.White, 1f, 1f, 1f, 1f, StarfieldPreviewUvTransform.Identity, false, false, false);
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

        var materialDataCandidates = new List<byte[]> { materialData };
        var materialParentDiagnostics = new List<string>();
        AddStarfieldMaterialParentData(
            materialPath,
            materialData,
            resolveExternalAsset,
            materialDataCandidates,
            materialParentDiagnostics,
            new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            0);
        var stringCandidates = materialDataCandidates
            .SelectMany(GetMaterialStringCandidates)
            .ToList();
        var rootStringCandidates = GetMaterialStringCandidates(materialData).ToList();
        var rootTextureCandidates = GetPreviewTextureCandidates(rootStringCandidates).ToList();
        var parentTextureCandidates = GetPreviewTextureCandidates(materialDataCandidates.Skip(1).SelectMany(GetMaterialStringCandidates)).ToList();
        var directTextureCandidates = rootTextureCandidates
            .Concat(parentTextureCandidates)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var unsupportedFeatures = GetUnsupportedMaterialFeatureSummary(stringCandidates);
        var isInvisible = IsInvisibleMaterial(materialData, rootStringCandidates);
        var isDecal = stringCandidates.Any(IsDecalMaterialToken);
        var useAdditiveBlend = stringCandidates.Any(IsAdditiveBlendToken);
        var materialDatabaseDiagnostics = materialParentDiagnostics;
        var texturePath = existingTexturePath;
        var overlayTexturePath = isDecal
            ? directTextureCandidates.FirstOrDefault(IsRenderableDecalOverlayTextureCandidate)
            : null;
        var decalOpacityTexturePath = isDecal && !isInvisible
            ? GetStarfieldDecalOpacityTexturePath(materialDataCandidates)
            : null;
        var decalTint = isDecal && !isInvisible
            ? GetStarfieldDecalTint(materialDataCandidates)
            : StarfieldPreviewColor.White;
        var materialTint = !isDecal && !isInvisible
            ? GetStarfieldMaterialTint(materialDataCandidates)
            : StarfieldPreviewColor.White;
        var decalUvTransform = isDecal && !isInvisible
            ? GetStarfieldDecalUvTransform(materialDataCandidates)
            : StarfieldPreviewUvTransform.Identity;
        var textureSource = string.IsNullOrWhiteSpace(texturePath)
            ? "none"
            : "material";

        if (string.IsNullOrWhiteSpace(texturePath) && !isDecal && directTextureCandidates.Count > 0)
        {
            texturePath = directTextureCandidates[0];
            textureSource = "material";
            if (!IsPreviewColorTextureCandidate(texturePath) &&
                TryFindMaterialDatabaseTextureCandidate(directTextureCandidates, resolveExternalAsset, materialDatabaseDiagnostics, out var databaseTexturePath))
            {
                texturePath = databaseTexturePath;
                textureSource = "material database";
            }
            else if (IsPreviewColorTextureCandidate(texturePath))
            {
                materialDatabaseDiagnostics.Add($"skipped because material already provided preview color texture {texturePath}");
            }
        }

        if (isDecal &&
            !isInvisible &&
            string.IsNullOrWhiteSpace(overlayTexturePath) &&
            string.IsNullOrWhiteSpace(decalOpacityTexturePath))
        {
            TryFindMaterialDatabaseTextureCandidateForMaterial(materialPath, stringCandidates, resolveExternalAsset, materialDatabaseDiagnostics, out overlayTexturePath);
        }

        if (string.IsNullOrWhiteSpace(texturePath) && !string.IsNullOrWhiteSpace(overlayTexturePath) && !isDecal)
        {
            texturePath = overlayTexturePath;
        }

        materialInfo = new NifMaterialInfo(
            materialPath,
            texturePath,
            overlayTexturePath,
            decalOpacityTexturePath,
            materialTint,
            decalTint.Red,
            decalTint.Green,
            decalTint.Blue,
            decalTint.Alpha,
            decalUvTransform,
            isDecal,
            isInvisible,
            useAdditiveBlend);
        diagnostic = GetMaterialPreviewDiagnostic(materialPath, materialInfo, textureSource, unsupportedFeatures, materialDatabaseDiagnostics);
        return true;
    }

    private static string GetMaterialPreviewDiagnostic(
        string materialPath,
        NifMaterialInfo materialInfo,
        string textureSource,
        string unsupportedFeatures,
        IReadOnlyList<string> materialDatabaseDiagnostics)
    {
        var diagnostic = $"material {materialPath} resolved preview texture {materialInfo.TexturePath ?? "none"} from {textureSource}, overlay {materialInfo.OverlayTexturePath ?? "none"}, and decal opacity {materialInfo.DecalOpacityTexturePath ?? "none"}";
        if (materialInfo.IsDecal)
        {
            diagnostic += "; decal material";
        }

        if (materialInfo.IsInvisible)
        {
            diagnostic += "; invisible material";
        }

        if (materialInfo.UseAdditiveBlend)
        {
            diagnostic += "; additive blend";
        }

        if (!materialInfo.DecalUvTransform.IsIdentity)
        {
            diagnostic += $"; decal UV {materialInfo.DecalUvTransform.Description}";
        }

        if (!materialInfo.MaterialTint.IsWhite)
        {
            diagnostic += $"; material tint {materialInfo.MaterialTint.Description}";
        }

        if (!string.IsNullOrWhiteSpace(unsupportedFeatures))
        {
            diagnostic += $"; unsupported material features: {unsupportedFeatures}";
        }

        if (materialDatabaseDiagnostics.Count > 0)
        {
            diagnostic += $"; material database probe: {string.Join("; ", materialDatabaseDiagnostics)}";
        }

        return diagnostic;
    }

    private static void AddStarfieldMaterialParentData(
        string materialPath,
        byte[] materialData,
        Func<string, byte[]?> resolveExternalAsset,
        List<byte[]> materialDataCandidates,
        List<string> diagnostics,
        HashSet<string> visitedMaterialPaths,
        int depth)
    {
        if (depth >= MaxStarfieldMaterialParentDepth)
        {
            diagnostics.Add($"skipped material parent walk for {materialPath} because depth limit was reached");
            return;
        }

        foreach (var parentPath in GetStarfieldMaterialParentPaths(materialPath, materialData).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (!visitedMaterialPaths.Add(parentPath))
            {
                continue;
            }

            var parentData = resolveExternalAsset(parentPath);
            if (parentData == null || parentData.Length == 0)
            {
                diagnostics.Add($"parent material {parentPath} was not resolved");
                continue;
            }

            diagnostics.Add($"parent material {parentPath} resolved {parentData.Length} byte(s)");
            materialDataCandidates.Add(parentData);
            AddStarfieldMaterialParentData(
                parentPath,
                parentData,
                resolveExternalAsset,
                materialDataCandidates,
                diagnostics,
                visitedMaterialPaths,
                depth + 1);
        }
    }

    private static IEnumerable<string> GetStarfieldMaterialParentPaths(string materialPath, byte[] materialData)
    {
        var normalizedMaterialPath = NormalizeMaterialPathForComparison(materialPath);
        foreach (var candidate in GetMaterialStringCandidates(materialData))
        {
            if (!TryNormalizeMaterialPathCandidate(candidate, out var parentPath) ||
                !IsPreviewMaterialParentPath(parentPath) ||
                string.Equals(NormalizeMaterialPathForComparison(parentPath), normalizedMaterialPath, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            yield return parentPath;
        }
    }

    private static bool IsPreviewMaterialParentPath(string materialPath)
    {
        return !materialPath.Contains("\\Layered\\ShaderModels\\", StringComparison.OrdinalIgnoreCase) &&
            !materialPath.Contains("\\Layered\\Root\\", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryNormalizeMaterialPathCandidate(string candidate, out string materialPath)
    {
        materialPath = string.Empty;
        var normalized = candidate.Trim().Replace('/', '\\');
        while (normalized.Contains("\\\\", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("\\\\", "\\", StringComparison.Ordinal);
        }

        var materialIndex = normalized.IndexOf(".mat", StringComparison.OrdinalIgnoreCase);
        if (materialIndex < 0)
        {
            return false;
        }

        var materialRootIndex = normalized.IndexOf("materials\\", StringComparison.OrdinalIgnoreCase);
        if (materialRootIndex < 0 || materialRootIndex > materialIndex)
        {
            return false;
        }

        materialPath = normalized[materialRootIndex..(materialIndex + 4)];
        return materialPath.EndsWith(".mat", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeMaterialPathForComparison(string materialPath)
    {
        var normalized = materialPath.Trim().Replace('/', '\\');
        while (normalized.Contains("\\\\", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("\\\\", "\\", StringComparison.Ordinal);
        }

        if (normalized.StartsWith("Data\\", StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized["Data\\".Length..];
        }

        return normalized;
    }

    private static bool IsPreviewColorTextureCandidate(string texturePath)
    {
        return GetPreviewTextureCandidateScore(texturePath) >= 100;
    }

    private static IEnumerable<string> GetPreviewTextureCandidates(IEnumerable<string> stringCandidates)
    {
        return stringCandidates
            .Select(candidate => TryNormalizeTexturePathCandidate(candidate, out var texturePath) ? texturePath : null)
            .Where(texturePath => !string.IsNullOrWhiteSpace(texturePath))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(texturePath => GetPreviewTextureCandidateScore(texturePath!))
            .ThenBy(texturePath => texturePath, StringComparer.OrdinalIgnoreCase)
            .Select(texturePath => texturePath!);
    }

    private static bool IsRenderableDecalOverlayTextureCandidate(string texturePath)
    {
        var fileName = Path.GetFileName(texturePath);
        return !fileName.Contains("opacity", StringComparison.OrdinalIgnoreCase) &&
            !fileName.Contains("mask", StringComparison.OrdinalIgnoreCase) &&
            !fileName.Contains("normal", StringComparison.OrdinalIgnoreCase) &&
            !fileName.Contains("rough", StringComparison.OrdinalIgnoreCase);
    }

    private static string? GetStarfieldDecalOpacityTexturePath(IEnumerable<byte[]> materialDataCandidates)
    {
        foreach (var materialData in materialDataCandidates)
        {
            if (!TryParseMaterialJson(materialData, out var document))
            {
                continue;
            }

            using (document)
            {
                if (TryGetStarfieldSummaryTexture(document.RootElement, "Opacity", out var texturePath) &&
                    TryNormalizeTexturePathCandidate(texturePath, out var normalizedTexturePath))
                {
                    return normalizedTexturePath;
                }
            }
        }

        return null;
    }

    private static StarfieldPreviewColor GetStarfieldDecalTint(IEnumerable<byte[]> materialDataCandidates)
    {
        foreach (var materialData in materialDataCandidates)
        {
            if (!TryParseMaterialJson(materialData, out var document))
            {
                continue;
            }

            using (document)
            {
                if (TryGetStarfieldSummaryReplacementColor(document.RootElement, "Albedo", out var color))
                {
                    return color;
                }
            }
        }

        return StarfieldPreviewColor.White;
    }

    private static StarfieldPreviewColor GetStarfieldMaterialTint(IEnumerable<byte[]> materialDataCandidates)
    {
        foreach (var materialData in materialDataCandidates)
        {
            if (!TryParseMaterialJson(materialData, out var document))
            {
                continue;
            }

            using (document)
            {
                if (TryGetStarfieldObjectsMaterialColor(document.RootElement, out var color))
                {
                    return color;
                }

                if (TryGetStarfieldSummaryReplacementColor(document.RootElement, "Albedo", out color))
                {
                    return color;
                }

                if (TryGetStarfieldSummaryMaterialColor(document.RootElement, out color))
                {
                    return color;
                }
            }
        }

        return StarfieldPreviewColor.White;
    }

    private static bool TryGetStarfieldObjectsMaterialColor(JsonElement root, out StarfieldPreviewColor color)
    {
        color = StarfieldPreviewColor.White;
        if (!root.TryGetProperty("Objects", out var objects) ||
            objects.ValueKind != JsonValueKind.Array)
        {
            return false;
        }

        foreach (var materialObject in objects.EnumerateArray())
        {
            if (materialObject.ValueKind != JsonValueKind.Object ||
                !materialObject.TryGetProperty("Components", out var components) ||
                components.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var component in components.EnumerateArray())
            {
                if (component.ValueKind == JsonValueKind.Object &&
                    component.TryGetProperty("Type", out var type) &&
                    type.ValueKind == JsonValueKind.String &&
                    string.Equals(type.GetString(), "BSMaterial::Color", StringComparison.Ordinal) &&
                    TryReadStarfieldObjectMaterialColor(component, out color))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool TryReadStarfieldObjectMaterialColor(JsonElement component, out StarfieldPreviewColor color)
    {
        color = StarfieldPreviewColor.White;
        return component.TryGetProperty("Data", out var data) &&
            data.ValueKind == JsonValueKind.Object &&
            data.TryGetProperty("Value", out var value) &&
            value.ValueKind == JsonValueKind.Object &&
            value.TryGetProperty("Data", out var colorData) &&
            TryReadStarfieldPreviewColor(colorData, out color);
    }

    private static StarfieldPreviewUvTransform GetStarfieldDecalUvTransform(IEnumerable<byte[]> materialDataCandidates)
    {
        foreach (var materialData in materialDataCandidates)
        {
            if (!TryParseMaterialJson(materialData, out var document))
            {
                continue;
            }

            using (document)
            {
                if (TryGetStarfieldSummaryUvTransform(document.RootElement, out var transform))
                {
                    return transform;
                }
            }
        }

        return StarfieldPreviewUvTransform.Identity;
    }

    private static bool TryGetStarfieldSummaryUvTransform(JsonElement root, out StarfieldPreviewUvTransform transform)
    {
        transform = StarfieldPreviewUvTransform.Identity;
        if (!root.TryGetProperty("Summary", out var summary) ||
            summary.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        foreach (var layer in summary.EnumerateObject())
        {
            if (layer.Value.ValueKind == JsonValueKind.Object &&
                TryGetStarfieldUvTransform(layer.Value, out transform))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryGetStarfieldUvTransform(JsonElement element, out StarfieldPreviewUvTransform transform)
    {
        transform = StarfieldPreviewUvTransform.Identity;
        if (TryReadStarfieldUvTransform(element, out transform))
        {
            return true;
        }

        if (element.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        foreach (var property in element.EnumerateObject())
        {
            if (IsStarfieldUvStreamProperty(property.Name) &&
                property.Value.ValueKind == JsonValueKind.Object &&
                TryReadStarfieldUvTransform(property.Value, out transform))
            {
                return true;
            }
        }

        foreach (var property in element.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Object &&
                TryGetStarfieldUvTransform(property.Value, out transform))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsStarfieldUvStreamProperty(string propertyName)
    {
        return propertyName.Contains("UV", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryReadStarfieldUvTransform(JsonElement element, out StarfieldPreviewUvTransform transform)
    {
        transform = StarfieldPreviewUvTransform.Identity;
        if (!TryReadStarfieldVector2(element, "Scale", out var scaleU, out var scaleV) ||
            !TryReadStarfieldVector2(element, "Offset", out var offsetU, out var offsetV))
        {
            return false;
        }

        if (!IsReasonableUvTransformValue(scaleU) ||
            !IsReasonableUvTransformValue(scaleV) ||
            !IsReasonableUvTransformValue(offsetU) ||
            !IsReasonableUvTransformValue(offsetV))
        {
            return false;
        }

        transform = new StarfieldPreviewUvTransform(scaleU, scaleV, offsetU, offsetV);
        return true;
    }

    private static bool TryReadStarfieldVector2(JsonElement element, string propertyName, out float x, out float y)
    {
        x = 0f;
        y = 0f;
        if (element.TryGetProperty(propertyName, out var property))
        {
            if (property.ValueKind == JsonValueKind.Object)
            {
                return TryGetJsonSingle(property, "x", out x) &&
                    TryGetJsonSingle(property, "y", out y);
            }

            if (property.ValueKind == JsonValueKind.Array && property.GetArrayLength() >= 2)
            {
                var values = property.EnumerateArray().Take(2).ToList();
                return TryGetJsonSingleValue(values[0], out x) &&
                    TryGetJsonSingleValue(values[1], out y);
            }
        }

        return TryGetJsonSingle(element, propertyName + "X", out x) &&
            TryGetJsonSingle(element, propertyName + "Y", out y);
    }

    private static bool TryGetStarfieldSummaryTexture(JsonElement root, string textureName, out string texturePath)
    {
        texturePath = string.Empty;
        if (!TryGetStarfieldSummaryTextures(root, out var textures) ||
            !textures.TryGetProperty(textureName, out var texture) ||
            !texture.TryGetProperty("File", out var file) ||
            file.ValueKind != JsonValueKind.String)
        {
            return false;
        }

        texturePath = file.GetString() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(texturePath);
    }

    private static bool TryGetStarfieldSummaryReplacementColor(JsonElement root, string textureName, out StarfieldPreviewColor color)
    {
        color = StarfieldPreviewColor.White;
        if (!TryGetStarfieldSummaryTextures(root, out var textures) ||
            !textures.TryGetProperty(textureName, out var texture) ||
            !texture.TryGetProperty("UseReplacement", out var useReplacement) ||
            useReplacement.ValueKind != JsonValueKind.True ||
            !texture.TryGetProperty("Replacement", out var replacement))
        {
            return false;
        }

        return TryReadStarfieldPreviewColor(replacement, out color);
    }

    private static bool TryGetStarfieldSummaryMaterialColor(JsonElement root, out StarfieldPreviewColor color)
    {
        color = StarfieldPreviewColor.White;
        if (!root.TryGetProperty("Summary", out var summary) ||
            summary.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        foreach (var layer in summary.EnumerateObject())
        {
            if (layer.Value.ValueKind != JsonValueKind.Object)
            {
                continue;
            }

            if (TryReadStarfieldMaterialColor(layer.Value, out color))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryReadStarfieldMaterialColor(JsonElement element, out StarfieldPreviewColor color)
    {
        color = StarfieldPreviewColor.White;
        if (element.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        if (element.TryGetProperty("Color", out var colorElement) &&
            TryReadStarfieldPreviewColor(colorElement, out color))
        {
            return true;
        }

        if (element.TryGetProperty("Material", out var material) &&
            material.ValueKind == JsonValueKind.Object &&
            material.TryGetProperty("Color", out colorElement) &&
            TryReadStarfieldPreviewColor(colorElement, out color))
        {
            return true;
        }

        return false;
    }

    private static bool TryGetStarfieldSummaryTextures(JsonElement root, out JsonElement textures)
    {
        textures = default;
        if (!root.TryGetProperty("Summary", out var summary) ||
            summary.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        foreach (var layer in summary.EnumerateObject())
        {
            if (layer.Value.ValueKind == JsonValueKind.Object &&
                layer.Value.TryGetProperty("Textures", out textures) &&
                textures.ValueKind == JsonValueKind.Object)
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryReadStarfieldPreviewColor(JsonElement element, out StarfieldPreviewColor color)
    {
        color = StarfieldPreviewColor.White;
        if (element.ValueKind == JsonValueKind.String)
        {
            return TryReadStarfieldPreviewColorHex(element.GetString(), out color);
        }

        if ((!TryGetJsonSingle(element, "x", out var red) &&
                !TryGetJsonSingle(element, "r", out red) &&
                !TryGetJsonSingle(element, "Red", out red)) ||
            (!TryGetJsonSingle(element, "y", out var green) &&
                !TryGetJsonSingle(element, "g", out green) &&
                !TryGetJsonSingle(element, "Green", out green)) ||
            (!TryGetJsonSingle(element, "z", out var blue) &&
                !TryGetJsonSingle(element, "b", out blue) &&
                !TryGetJsonSingle(element, "Blue", out blue)))
        {
            return false;
        }

        var alpha = TryGetJsonSingle(element, "w", out var parsedAlpha) ||
            TryGetJsonSingle(element, "a", out parsedAlpha) ||
            TryGetJsonSingle(element, "Alpha", out parsedAlpha)
            ? parsedAlpha
            : 1f;
        color = new StarfieldPreviewColor(red, green, blue, alpha);
        return true;
    }

    private static bool TryReadStarfieldPreviewColorHex(string? value, out StarfieldPreviewColor color)
    {
        color = StarfieldPreviewColor.White;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var hex = value.Trim().TrimStart('#');
        if (hex.Length is not 6 and not 8 ||
            !uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsedColor))
        {
            return false;
        }

        var red = ((parsedColor >> (hex.Length == 8 ? 24 : 16)) & 0xFF) / 255f;
        var green = ((parsedColor >> (hex.Length == 8 ? 16 : 8)) & 0xFF) / 255f;
        var blue = ((parsedColor >> (hex.Length == 8 ? 8 : 0)) & 0xFF) / 255f;
        var alpha = hex.Length == 8
            ? (parsedColor & 0xFF) / 255f
            : 1f;
        color = new StarfieldPreviewColor(red, green, blue, alpha);
        return true;
    }

    private static bool TryGetJsonSingle(JsonElement element, string propertyName, out float value)
    {
        value = 0f;
        if (!element.TryGetProperty(propertyName, out var property))
        {
            return false;
        }

        return TryGetJsonSingleValue(property, out value);
    }

    private static bool TryGetJsonSingleValue(JsonElement property, out float value)
    {
        value = 0f;
        if (property.ValueKind == JsonValueKind.Number && property.TryGetSingle(out value))
        {
            return true;
        }

        return property.ValueKind == JsonValueKind.String &&
            float.TryParse(property.GetString(), NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private static bool TryFindMaterialDatabaseTextureCandidate(
        IReadOnlyList<string> materialTextureCandidates,
        Func<string, byte[]?> resolveExternalAsset,
        List<string> diagnostics,
        out string texturePath)
    {
        texturePath = string.Empty;
        var materialFileNames = materialTextureCandidates
            .Select(GetTextureFileName)
            .Where(fileName => !string.IsNullOrWhiteSpace(fileName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (materialFileNames.Count == 0)
        {
            diagnostics.Add("skipped because no material DDS filenames were found");
            return false;
        }

        foreach (var databasePath in StarfieldMaterialDatabasePaths)
        {
            diagnostics.Add($"requested {databasePath}");
            var databaseData = resolveExternalAsset(databasePath);
            if (databaseData == null || databaseData.Length == 0)
            {
                diagnostics.Add($"{databasePath} was not resolved");
                continue;
            }

            diagnostics.Add($"{databasePath} resolved {databaseData.Length} byte(s)");
            if (!TryReadStarfieldMaterialDatabaseStrings(databaseData, out var strings, out var tableCount, out var readFailureReason))
            {
                diagnostics.Add($"{databasePath} was not a supported STRT table: {readFailureReason}");
                continue;
            }

            diagnostics.Add($"{databasePath} parsed {strings.Count} string(s) from {tableCount} STRT table(s)");
            var candidates = strings
                .SelectMany(value => GetMaterialTextureCandidates(Encoding.UTF8.GetBytes(value)))
                .Where(candidate => materialFileNames.Contains(GetTextureFileName(candidate)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(GetPreviewTextureCandidateScore)
                .ThenBy(candidate => candidate, StringComparer.OrdinalIgnoreCase)
                .ToList();
            diagnostics.Add($"{databasePath} matched {candidates.Count} DDS candidate(s) for {string.Join(", ", materialFileNames)}");
            var correctedCandidate = candidates.FirstOrDefault(candidate =>
                !materialTextureCandidates.Contains(candidate, StringComparer.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(correctedCandidate))
            {
                texturePath = correctedCandidate;
                diagnostics.Add($"{databasePath} selected {correctedCandidate}");
                return true;
            }

            if (candidates.Count > 0)
            {
                diagnostics.Add($"{databasePath} only matched existing material texture path(s)");
            }
        }

        return false;
    }

    private static bool TryFindMaterialDatabaseTextureCandidateForMaterial(
        string materialPath,
        IReadOnlyList<string> materialStringCandidates,
        Func<string, byte[]?> resolveExternalAsset,
        List<string> diagnostics,
        out string? texturePath)
    {
        texturePath = null;
        var searchTokens = GetMaterialSearchTokens(materialPath, materialStringCandidates).ToList();
        if (searchTokens.Count == 0)
        {
            diagnostics.Add("skipped material-name database probe because no material tokens were found");
            return false;
        }

        foreach (var databasePath in StarfieldMaterialDatabasePaths)
        {
            diagnostics.Add($"requested {databasePath} for material-name search");
            var databaseData = resolveExternalAsset(databasePath);
            if (databaseData == null || databaseData.Length == 0)
            {
                diagnostics.Add($"{databasePath} was not resolved");
                continue;
            }

            diagnostics.Add($"{databasePath} resolved {databaseData.Length} byte(s)");
            if (!TryReadStarfieldMaterialDatabaseStrings(databaseData, out var strings, out var tableCount, out var readFailureReason))
            {
                diagnostics.Add($"{databasePath} was not a supported STRT table: {readFailureReason}");
                continue;
            }

            diagnostics.Add($"{databasePath} parsed {strings.Count} string(s) from {tableCount} STRT table(s)");
            var candidates = strings
                .SelectMany(value => GetMaterialTextureCandidates(Encoding.UTF8.GetBytes(value)))
                .Select(candidate => new
                {
                    Path = candidate,
                    Score = GetMaterialNameTextureCandidateScore(candidate, searchTokens)
                })
                .Where(candidate => candidate.Score > 0)
                .DistinctBy(candidate => candidate.Path, StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(candidate => candidate.Score)
                .ThenByDescending(candidate => GetPreviewTextureCandidateScore(candidate.Path))
                .ThenBy(candidate => candidate.Path, StringComparer.OrdinalIgnoreCase)
                .ToList();
            diagnostics.Add($"{databasePath} matched {candidates.Count} material-name DDS candidate(s) for {string.Join(", ", searchTokens)}");
            if (candidates.Count == 0)
            {
                continue;
            }

            texturePath = candidates[0].Path;
            diagnostics.Add($"{databasePath} selected {texturePath}");
            return true;
        }

        return false;
    }

    private static IEnumerable<string> GetMaterialSearchTokens(string materialPath, IReadOnlyList<string> materialStringCandidates)
    {
        foreach (var token in SplitMaterialSearchTokens(Path.GetFileNameWithoutExtension(materialPath)))
        {
            yield return token;
        }

        foreach (var candidate in materialStringCandidates)
        {
            if (!candidate.Contains("Materials", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            foreach (var token in SplitMaterialSearchTokens(Path.GetFileNameWithoutExtension(candidate)))
            {
                yield return token;
            }
        }
    }

    private static IEnumerable<string> SplitMaterialSearchTokens(string value)
    {
        foreach (var token in value.Split(['_', '-', ' ', '\\', '/'], StringSplitOptions.RemoveEmptyEntries))
        {
            var trimmed = token.Trim();
            if (trimmed.Length >= 4)
            {
                yield return trimmed;
            }
        }
    }

    private static int GetMaterialNameTextureCandidateScore(string texturePath, IReadOnlyList<string> searchTokens)
    {
        var score = 0;
        foreach (var token in searchTokens.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (texturePath.Contains(token, StringComparison.OrdinalIgnoreCase))
            {
                score += token.Length >= 8 ? 2 : 1;
            }
        }

        return score;
    }

    private static bool TryReadStarfieldMaterialDatabaseStrings(byte[] data, out IReadOnlyList<string> strings, out int tableCount, out string failureReason)
    {
        strings = [];
        tableCount = 0;
        failureReason = string.Empty;
        if (data.Length < 24)
        {
            failureReason = $"length {data.Length} is shorter than the 24 byte header";
            return false;
        }

        var parsedStrings = new List<string>();
        var lastFailureReason = string.Empty;
        var searchPosition = 0;
        while (TryFindStarfieldMaterialDatabaseTable(data, searchPosition, out var tablePosition))
        {
            searchPosition = tablePosition + StarfieldMaterialDatabaseBethSignatureBytes.Length;
            if (!TryReadStarfieldMaterialDatabaseStringTable(data, tablePosition, parsedStrings, out lastFailureReason))
            {
                continue;
            }

            tableCount++;
        }

        strings = parsedStrings;
        if (strings.Count == 0)
        {
            failureReason = string.IsNullOrWhiteSpace(lastFailureReason)
                ? "no BETH/STRT table was found"
                : lastFailureReason;
        }

        return strings.Count > 0;
    }

    private static bool TryFindStarfieldMaterialDatabaseTable(byte[] data, int searchPosition, out int tablePosition)
    {
        tablePosition = -1;
        if (searchPosition > data.Length - StarfieldMaterialDatabaseBethSignatureBytes.Length)
        {
            return false;
        }

        var relativePosition = data.AsSpan(searchPosition).IndexOf(StarfieldMaterialDatabaseBethSignatureBytes);
        if (relativePosition < 0)
        {
            return false;
        }

        tablePosition = searchPosition + relativePosition;
        return true;
    }

    private static bool TryReadStarfieldMaterialDatabaseStringTable(byte[] data, int tablePosition, List<string> parsedStrings, out string failureReason)
    {
        failureReason = string.Empty;
        if (data.Length - tablePosition < 24)
        {
            failureReason = $"table at {tablePosition} is shorter than the 24 byte header";
            return false;
        }

        var position = tablePosition;
        var bethSignature = BinaryPrimitives.ReadUInt64LittleEndian(data.AsSpan(position, sizeof(ulong)));
        position += sizeof(ulong);
        if (bethSignature != StarfieldMaterialDatabaseBethSignature)
        {
            failureReason = $"BETH signature 0x{bethSignature:X16} at {tablePosition} did not match";
            return false;
        }

        var version = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position, sizeof(uint)));
        position += sizeof(uint);
        if (version != StarfieldMaterialDatabaseVersion)
        {
            failureReason = $"version {version} at {tablePosition} did not match {StarfieldMaterialDatabaseVersion}";
            return false;
        }

        position += sizeof(uint);
        var stringTableSignature = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position, sizeof(uint)));
        position += sizeof(uint);
        if (stringTableSignature != StarfieldMaterialDatabaseStringTableSignature)
        {
            failureReason = $"string table signature 0x{stringTableSignature:X8} at {tablePosition} did not match STRT";
            return false;
        }

        var byteCount = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(position, sizeof(uint)));
        position += sizeof(uint);
        if (byteCount > data.Length - position)
        {
            failureReason = $"STRT byte count {byteCount} at {tablePosition} exceeds remaining {data.Length - position} byte(s)";
            return false;
        }

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

        return true;
    }

    private static string GetTextureFileName(string texturePath)
    {
        return texturePath.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries).LastOrDefault() ?? string.Empty;
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
        return GetUnsupportedMaterialFeatureSummary(GetMaterialStringCandidates(data));
    }

    private static string GetUnsupportedMaterialFeatureSummary(IEnumerable<string> stringCandidates)
    {
        var features = new List<string>();
        foreach (var candidate in stringCandidates)
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

    private static bool IsInvisibleMaterialToken(string value)
    {
        return string.Equals(value, "Invisible", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsInvisibleMaterial(byte[] materialData, IReadOnlyList<string> rootStringCandidates)
    {
        if (TryParseMaterialJson(materialData, out var document))
        {
            using (document)
            {
                var root = document.RootElement;
                if (root.TryGetProperty("Summary", out var summary) &&
                    summary.ValueKind != JsonValueKind.Null &&
                    summary.ValueKind != JsonValueKind.Undefined)
                {
                    return JsonElementStringValues(summary).Any(IsInvisibleMaterialToken);
                }

                return JsonElementStringValues(root).Any(value =>
                    value.Contains("\\ShaderModels\\Invisible.mat", StringComparison.OrdinalIgnoreCase) ||
                    IsInvisibleMaterialToken(value));
            }
        }

        return rootStringCandidates.Any(IsInvisibleMaterialToken);
    }

    private static bool TryParseMaterialJson(byte[] materialData, out JsonDocument document)
    {
        try
        {
            document = JsonDocument.Parse(materialData);
            return true;
        }
        catch (JsonException)
        {
            document = null!;
            return false;
        }
    }

    private static IEnumerable<string> JsonElementStringValues(JsonElement element)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                var value = element.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    yield return value;
                }

                break;
            case JsonValueKind.Object:
                foreach (var property in element.EnumerateObject())
                {
                    foreach (var childValue in JsonElementStringValues(property.Value))
                    {
                        yield return childValue;
                    }
                }

                break;
            case JsonValueKind.Array:
                foreach (var item in element.EnumerateArray())
                {
                    foreach (var childValue in JsonElementStringValues(item))
                    {
                        yield return childValue;
                    }
                }

                break;
        }
    }

    private static bool IsDecalMaterialToken(string value)
    {
        return value.Contains("Decal", StringComparison.OrdinalIgnoreCase) ||
            value.Contains("StandardDecal", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAdditiveBlendToken(string value)
    {
        return string.Equals(value, "Additive", StringComparison.OrdinalIgnoreCase);
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
}
