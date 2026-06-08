using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Games;

public class SupportedGameDTO
{
    public required SupportedGame Game { get; set; }

    public required string Name { get; set; }

    public required string DisplayName { get; set; }
}
