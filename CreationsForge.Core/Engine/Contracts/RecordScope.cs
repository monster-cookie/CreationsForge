namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Identifies which native record contexts an enumeration or read operation considers.</summary>
public enum RecordScope
{
    /// <summary>Resolve each record to its winning override across sources and staged output.</summary>
    WinningOverrides,

    /// <summary>Return every matching source and output context in load-order order.</summary>
    AllContexts,

    /// <summary>Consider only plugins explicitly designated as selected read-only sources.</summary>
    Source,

    /// <summary>Consider only the selected mutable staged output.</summary>
    StagedOutput
}
