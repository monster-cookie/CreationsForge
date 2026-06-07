using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Games;

public class GameDTO
{
    public required SupportedGame Game { get; set; }

    public required string DisplayName { get; set; }

    public string? InstallationFolder { get; set; }

    public string? DataFolder { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
