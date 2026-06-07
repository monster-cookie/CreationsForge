namespace CreationsForge.Core.DTOs.Results;

public class AllGamesImportWorkflowResultDTO
{
    public bool DatabaseReset { get; set; }

    public bool MigrationsApplied { get; set; }

    public IReadOnlyList<GameImportResultDTO> ImportResults { get; set; } = [];
}
