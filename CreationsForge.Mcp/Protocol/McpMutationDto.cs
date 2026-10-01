namespace CreationsForge.Mcp.Protocol;

/// <summary>Requests one generic record create or exact-context override.</summary>
public sealed class McpMutationDto
{
    /// <summary>Gets or sets Create or Override.</summary>
    public string Kind { get; set; } = string.Empty;

    /// <summary>Gets or sets the declared family identifier.</summary>
    public string FamilyId { get; set; } = string.Empty;

    /// <summary>Gets or sets the origin FormKey text required by an override.</summary>
    public string? FormKey { get; set; }

    /// <summary>Gets or sets the exact containing plugin required by an override.</summary>
    public string? ContainingModKey { get; set; }

    /// <summary>Gets or sets the ordered field changes.</summary>
    public List<McpFieldChangeDto> Changes { get; set; } = [];
}
