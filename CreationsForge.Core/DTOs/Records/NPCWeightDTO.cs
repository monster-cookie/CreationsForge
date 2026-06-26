using CreationsForge.Core.DTOs.Records.Metadata;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents NPC body weight data in scalar or tri-shape form.
/// </summary>
public class NPCWeightDTO
{
    /// <summary>
    /// Gets or sets the scalar weight used by Skyrim records.
    /// </summary>
    [NumericDisplayPrecision(3)]
    public double? Value { get; set; }

    /// <summary>
    /// Gets or sets the thin body-shape contribution.
    /// </summary>
    [NumericDisplayPrecision(3)]
    public double? Thin { get; set; }

    /// <summary>
    /// Gets or sets the muscular body-shape contribution.
    /// </summary>
    [NumericDisplayPrecision(3)]
    public double? Muscular { get; set; }

    /// <summary>
    /// Gets or sets the fat body-shape contribution.
    /// </summary>
    [NumericDisplayPrecision(3)]
    public double? Fat { get; set; }
}
