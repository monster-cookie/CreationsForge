using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ConstructibleObjectRecipeFilterDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required FormKeyDTO RecipeFilterFormKey { get; set; }

    public int RecipeFilterIndex { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
