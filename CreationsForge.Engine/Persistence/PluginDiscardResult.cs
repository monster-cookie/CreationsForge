namespace CreationsForge.Engine.Persistence;

/// <summary>Reports the result of restoring a workspace's native saved baseline.</summary>
public sealed class PluginDiscardResult
{
    /// <summary>Initializes a discard result.</summary>
    internal PluginDiscardResult(ulong revision, bool changed, bool isDirty, string diagnostic)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(diagnostic);
        Revision = revision;
        Changed = changed;
        IsDirty = isDirty;
        Diagnostic = diagnostic;
    }

    /// <summary>Gets the workspace revision after discard.</summary>
    public ulong Revision { get; }

    /// <summary>Gets whether staged native output was replaced by the saved baseline.</summary>
    public bool Changed { get; }

    /// <summary>Gets whether the restored output still needs a physical first save.</summary>
    public bool IsDirty { get; }

    /// <summary>Gets a bounded human-readable terminal diagnostic.</summary>
    public string Diagnostic { get; }
}
