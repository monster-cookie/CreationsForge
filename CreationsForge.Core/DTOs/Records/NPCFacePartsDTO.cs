namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents Skyrim NPC face part indices exported under Spriggit's <c>FaceParts</c> object.
/// </summary>
public class NPCFacePartsDTO
{
    /// <summary>
    /// Gets or sets the nose face part index.
    /// </summary>
    public long? Nose { get; set; }

    /// <summary>
    /// Gets or sets the unknown face part index.
    /// </summary>
    public long? Unknown { get; set; }

    /// <summary>
    /// Gets or sets the eyes face part index.
    /// </summary>
    public long? Eyes { get; set; }

    /// <summary>
    /// Gets or sets the mouth face part index.
    /// </summary>
    public long? Mouth { get; set; }
}
