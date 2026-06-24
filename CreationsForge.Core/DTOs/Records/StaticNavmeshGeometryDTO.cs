namespace CreationsForge.Core.DTOs.Records;

public class StaticNavmeshGeometryDTO
{
    public string? GridMin { get; set; }

    public string? GridMax { get; set; }

    public string? GridMaxDistance { get; set; }

    public string? GridSize { get; set; }

    public StaticNavmeshParentDTO? Parent { get; set; }

    public IList<StaticNavmeshCoverDTO> Cover { get; set; } = new List<StaticNavmeshCoverDTO>();

    public IList<StaticNavmeshCoverTriangleMappingDTO> CoverTriangleMappings { get; set; } = new List<StaticNavmeshCoverTriangleMappingDTO>();

    public IList<StaticNavmeshGridArrayDTO> GridArrays { get; set; } = new List<StaticNavmeshGridArrayDTO>();

    public IList<StaticNavmeshTriangleDTO> Triangles { get; set; } = new List<StaticNavmeshTriangleDTO>();

    public IList<string> Versioning { get; set; } = new List<string>();

    public IList<StaticNavmeshVertexDTO> Vertices { get; set; } = new List<StaticNavmeshVertexDTO>();
}
