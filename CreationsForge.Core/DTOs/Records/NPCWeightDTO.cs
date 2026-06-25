namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents NPC body weight data in scalar or tri-shape form.
/// </summary>
public class NPCWeightDTO
{
    /// <summary>
    /// Gets or sets the scalar weight used by Skyrim records.
    /// </summary>
    public double? Value { get; set; }

    /// <summary>
    /// Gets or sets the thin body-shape contribution.
    /// </summary>
    public double? Thin { get; set; }

    /// <summary>
    /// Gets or sets the muscular body-shape contribution.
    /// </summary>
    public double? Muscular { get; set; }

    /// <summary>
    /// Gets or sets the fat body-shape contribution.
    /// </summary>
    public double? Fat { get; set; }
}
