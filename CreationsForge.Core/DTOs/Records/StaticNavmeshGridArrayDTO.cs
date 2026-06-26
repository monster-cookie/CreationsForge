namespace CreationsForge.Core.DTOs.Records;

public class StaticNavmeshGridArrayDTO
{
    public int GridArrayIndex { get; set; }

    public IList<string> GridCell { get; set; } = new List<string>();
}
