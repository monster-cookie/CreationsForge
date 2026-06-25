namespace CreationsForge.Core.DTOs.Records.Metadata;

/// <summary>
/// Marks a numeric DTO property whose comparison and display value should use a reduced number of decimal places.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NumericDisplayPrecisionAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NumericDisplayPrecisionAttribute"/> class.
    /// </summary>
    /// <param name="decimalPlaces">The number of decimal places to keep for display and comparison formatting.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="decimalPlaces"/> is less than zero.
    /// </exception>
    public NumericDisplayPrecisionAttribute(int decimalPlaces)
    {
        if (decimalPlaces < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(decimalPlaces), "Display precision cannot be negative.");
        }

        DecimalPlaces = decimalPlaces;
    }

    /// <summary>
    /// Gets the number of decimal places retained for display and comparison formatting.
    /// </summary>
    public int DecimalPlaces { get; }
}
