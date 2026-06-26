using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents one actor-value property entry on a container record.
/// </summary>
public class ContainerPropertyDTO
{
    /// <summary>
    /// Gets or sets the game that owns the property row.
    /// </summary>
    public SupportedGame Game { get; set; }

    /// <summary>
    /// Gets or sets the plugin that supplied the parent container.
    /// </summary>
    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    /// <summary>
    /// Gets or sets the parent container form key.
    /// </summary>
    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    /// <summary>
    /// Gets or sets the zero-based property position from the source record.
    /// </summary>
    public int PropertyIndex { get; set; }

    /// <summary>
    /// Gets or sets the actor value referenced by the property, or <c>null</c> when the source omitted it.
    /// </summary>
    public FormKeyDTO? ActorValue { get; set; }

    /// <summary>
    /// Gets or sets the numeric property value, or <c>null</c> when the source omitted it.
    /// </summary>
    public double? Value { get; set; }

    /// <summary>
    /// Gets or sets the UTC timestamp for the import that produced this row.
    /// </summary>
    public DateTime ImportedAtUTC { get; set; }
}
