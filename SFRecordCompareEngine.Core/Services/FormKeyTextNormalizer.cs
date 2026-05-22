namespace SFRecordCompareEngine.Core.Services;

public static class FormKeyTextNormalizer
{
    public static string NormalizeReferenceValue(string referenceValue)
    {
        var normalizedReferenceValue = referenceValue.Trim();
        if (normalizedReferenceValue.StartsWith("FormID:", StringComparison.OrdinalIgnoreCase))
        {
            normalizedReferenceValue = normalizedReferenceValue["FormID:".Length..].Trim();
        }

        var mutagenTypeSuffixIndex = normalizedReferenceValue.LastIndexOf('<');
        if (mutagenTypeSuffixIndex > 0 && normalizedReferenceValue.EndsWith('>'))
        {
            normalizedReferenceValue = normalizedReferenceValue[..mutagenTypeSuffixIndex].Trim();
        }

        return normalizedReferenceValue;
    }
}
