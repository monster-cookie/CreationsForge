namespace CreationsForge.Mcp;

/// <summary>Identifies the exact immutable Core metadata type retained behind an MCP handle.</summary>
internal enum McpMetadataKind
{
    /// <summary>An exact native output association.</summary>
    OutputAssociation,

    /// <summary>A complete observed output artifact baseline.</summary>
    OutputBaseline,

    /// <summary>Terminal save-recovery evidence suitable for explicit workspace adoption.</summary>
    ResolvedOutputEvidence,

    /// <summary>The complete detail object returned by a workspace save.</summary>
    SaveResult,

    /// <summary>The complete detail object returned by read-only save recovery.</summary>
    RecoverSaveResult,

    /// <summary>The complete detail object returned by explicit save repair.</summary>
    RepairSaveResult,
}
