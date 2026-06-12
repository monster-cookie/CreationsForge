namespace CreationsForge.Core.DTOs.Results;

public class AssetArchiveIndexResultDTO
{
    public int ArchivesDiscovered { get; set; }

    public int ArchivesIndexed { get; set; }

    public int ArchivesSkippedCurrent { get; set; }

    public int ArchivesFailed { get; set; }

    public long EntriesIndexed { get; set; }
}
