namespace CreationsForge.Core.DTOs.Records.Interfaces;

/// <summary>
/// Represents a record DTO that owns component reflection rows exported by Spriggit as <c>REFL</c> fields.
/// </summary>
public interface IHasReflectionDTO
{
    /// <summary>
    /// Gets or sets the component reflection rows imported for the parent record.
    /// </summary>
    IList<ReflectionDTO> Reflections { get; set; }
}
