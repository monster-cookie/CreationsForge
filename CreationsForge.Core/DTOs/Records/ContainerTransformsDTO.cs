using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents Starfield container transform links exposed by Spriggit under the <c>Transforms</c> object.
/// </summary>
public class ContainerTransformsDTO
{
    /// <summary>
    /// Gets or sets the outpost transform form key, or <c>null</c> when Spriggit omits it.
    /// </summary>
    public FormKeyDTO? Outpost { get; set; }

    /// <summary>
    /// Gets or sets the preview transform form key, or <c>null</c> when Spriggit omits it.
    /// </summary>
    public FormKeyDTO? Preview { get; set; }
}
