namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Describes whether plugin serialization produced a changed private artifact set.</summary>
public enum PluginWriteDisposition
{
    /// <summary>A complete changed artifact set was written, reopened, and mapped to its destinations.</summary>
    StagedChanges,

    /// <summary>The complete existing output was proved semantically unchanged and no staging or destination write occurred.</summary>
    Unchanged
}
