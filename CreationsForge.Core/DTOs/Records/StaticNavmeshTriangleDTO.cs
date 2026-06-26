namespace CreationsForge.Core.DTOs.Records;

public class StaticNavmeshTriangleDTO
{
    public int TriangleIndex { get; set; }

    public string? EdgeLink_0_1 { get; set; }

    public string? EdgeLink_1_2 { get; set; }

    public string? EdgeLink_2_0 { get; set; }

    public string? Height { get; set; }

    public string? Vertices { get; set; }

    public string? CoverFlags { get; set; }

    public string? Flags { get; set; }
}
