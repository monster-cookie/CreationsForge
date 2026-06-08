namespace CreationsForge.Core.DTOs.Results;

public class GameImportWorkflowResultDTO
{
    public bool MigrationsApplied { get; set; }

    public required GameImportResultDTO ImportResult { get; set; }
}
