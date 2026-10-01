namespace CreationsForge.Mcp.Protocol;

/// <summary>Requests one registered field operation.</summary>
public sealed class McpFieldChangeDto
{
    /// <summary>Gets or sets the registered field path.</summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>Gets or sets the operation name: Set, Append, Insert, RemoveAt, or Clear.</summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>Gets or sets the value operand, when the operation requires one.</summary>
    public McpRecordValueDto? Value { get; set; }

    /// <summary>Gets or sets the collection index, when the operation requires one.</summary>
    public int? Index { get; set; }
}
