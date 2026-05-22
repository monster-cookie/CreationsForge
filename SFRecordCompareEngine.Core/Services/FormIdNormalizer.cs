namespace SFRecordCompareEngine.Core.Services;

public static class FormIdNormalizer
{
    public static string NormalizeFromFormKey(string formKey)
    {
        if (string.IsNullOrWhiteSpace(formKey))
        {
            throw new ArgumentException("FormKey cannot be empty.", nameof(formKey));
        }

        var normalizedFormKey = FormKeyTextNormalizer.NormalizeReferenceValue(formKey);
        var separatorIndex = normalizedFormKey.IndexOf(':', StringComparison.Ordinal);
        var formId = separatorIndex >= 0 ? normalizedFormKey[..separatorIndex] : normalizedFormKey;
        formId = formId.Trim();

        if (formId.Length is < 1 or > 6 || !formId.All(Uri.IsHexDigit))
        {
            throw new ArgumentException($"FormKey '{formKey}' does not contain a valid six-character FormID.", nameof(formKey));
        }

        return formId.ToUpperInvariant().PadLeft(6, '0');
    }
}
