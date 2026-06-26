using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents a Starfield condition form record and its shared condition rules.
/// </summary>
public class ConditionFormDTO : RecordDTO, Interfaces.IHasConditionsDTO
{
    /// <summary>
    /// Gets or sets the quest that owns this condition form when Spriggit exposes an <c>OwnerQuest</c> link.
    /// </summary>
    public FormKeyDTO? OwnerQuest { get; set; }

    /// <summary>
    /// Gets or sets the ordered condition rules attached to the condition form.
    /// </summary>
    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();
}
