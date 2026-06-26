using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class TerminalMenuItemDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public int MenuItemIndex { get; set; }

    public TranslatedStringDTO? ItemText { get; set; }

    public string? Type { get; set; }

    public int? ItemId { get; set; }

    public FormKeyDTO? Submenu { get; set; }

    public TranslatedStringDTO? DisplayText { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
