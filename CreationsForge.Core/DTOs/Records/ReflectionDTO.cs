using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Stores a component reflection payload exported by Spriggit as a <c>REFL</c> field.
/// </summary>
public class ReflectionDTO
{
    /// <summary>
    /// Gets or sets the game that owns the parent record.
    /// </summary>
    public required SupportedGame Game { get; set; }

    /// <summary>
    /// Gets or sets the plugin that contributed the parent record.
    /// </summary>
    public required ModKeyDTO ModKey { get; set; }

    /// <summary>
    /// Gets or sets the Bethesda record type identifier for the parent record.
    /// </summary>
    public required string RecordType { get; set; }

    /// <summary>
    /// Gets or sets the parent record form key.
    /// </summary>
    public required FormKeyDTO FormKey { get; set; }

    /// <summary>
    /// Gets or sets the zero-based Spriggit component index that contained the <c>REFL</c> field.
    /// </summary>
    public int ComponentIndex { get; set; }

    /// <summary>
    /// Gets or sets the Spriggit component type name associated with the <c>REFL</c> field.
    /// </summary>
    public required string ComponentType { get; set; }

    /// <summary>
    /// Gets or sets the original Spriggit path for the reflection field.
    /// </summary>
    public required string SourcePath { get; set; }

    /// <summary>
    /// Gets or sets the hexadecimal reflection payload without a leading <c>0x</c> prefix.
    /// </summary>
    public string? REFL { get; set; }

    /// <summary>
    /// Gets or sets when the reflection row was imported.
    /// </summary>
    public required DateTime ImportedAtUTC { get; set; }
}
