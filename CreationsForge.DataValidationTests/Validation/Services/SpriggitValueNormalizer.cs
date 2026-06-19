using System.Globalization;
using System.Text.RegularExpressions;

namespace CreationsForge.DataValidationTests.Validation.Services;

public class SpriggitValueNormalizer
{
    private static readonly Regex FormKeyRegex = new(
        @"^(?<id>[0-9A-Fa-f]{1,8})\s*:\s*(?<file>[^:]+)$",
        RegexOptions.Compiled);

    public string Normalize(string fieldPath, string value)
    {
        var trimmed = value.Replace("\r\n", "\n").Trim();
        if (string.Equals(trimmed, "Null", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        var formKeyMatch = FormKeyRegex.Match(trimmed);
        if (formKeyMatch.Success)
        {
            var id = uint.Parse(formKeyMatch.Groups["id"].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            return id.ToString("X6", CultureInfo.InvariantCulture) + ":" + formKeyMatch.Groups["file"].Value.Trim().ToUpperInvariant();
        }

        if (fieldPath.EndsWith("ModKey", StringComparison.OrdinalIgnoreCase) ||
            fieldPath.EndsWith("Plugin", StringComparison.OrdinalIgnoreCase))
        {
            return trimmed.ToUpperInvariant();
        }

        if (double.TryParse(trimmed, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var numeric))
        {
            return numeric.ToString("G17", CultureInfo.InvariantCulture);
        }

        if (bool.TryParse(trimmed, out var boolean))
        {
            return boolean.ToString(CultureInfo.InvariantCulture);
        }

        return trimmed;
    }
}
