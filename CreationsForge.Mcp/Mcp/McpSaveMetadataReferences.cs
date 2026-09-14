namespace CreationsForge.Mcp;

/// <summary>Groups the exact metadata references published atomically for one completed save outcome.</summary>
internal sealed class McpSaveMetadataReferences
{
    /// <summary>Initializes one complete or deliberately unavailable save metadata group.</summary>
    /// <param name="committedBaseline">The committed output baseline reference, when present and published.</param>
    /// <param name="output">The terminal recovery output reference, when present and published.</param>
    /// <param name="resolvedEvidence">The terminal recovery evidence reference, when present and published.</param>
    /// <param name="details">The complete save-result reference when the group was published.</param>
    internal McpSaveMetadataReferences(
        McpMetadataReference? committedBaseline,
        McpMetadataReference? output,
        McpMetadataReference? resolvedEvidence,
        McpMetadataReference? details)
    {
        CommittedBaseline = committedBaseline;
        Output = output;
        ResolvedEvidence = resolvedEvidence;
        Details = details;
    }

    /// <summary>Gets the exact committed output baseline reference.</summary>
    internal McpMetadataReference? CommittedBaseline { get; }

    /// <summary>Gets the exact terminal recovery output association reference.</summary>
    internal McpMetadataReference? Output { get; }

    /// <summary>Gets the exact terminal recovery evidence reference.</summary>
    internal McpMetadataReference? ResolvedEvidence { get; }

    /// <summary>Gets the complete save-result detail reference.</summary>
    internal McpMetadataReference? Details { get; }
}
