namespace CreationsForge.Console.Mcp;

/// <summary>Classifies operation admissions whose identifiers may be reused across distinct Core workflows.</summary>
internal enum McpMetadataOperationKind
{
    /// <summary>An output selection, edit, discard, reopen, or recovery-adoption workspace mutation.</summary>
    WorkspaceMutation,

    /// <summary>A fresh read-only coordinator recovery observation.</summary>
    Recover,

    /// <summary>An explicit coordinator repair that may produce newly observed evidence even on replay.</summary>
    Repair,
}
