namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Reports whether one successfully applied typed command changed its unpublished plugin candidate.</summary>
public sealed class RecordEditMutationResult
{
    /// <summary>Initializes the result of one successfully applied plugin command.</summary>
    /// <param name="changed">Whether the candidate differs from its immediately prior plugin state because of this command.</param>
    public RecordEditMutationResult(bool changed)
    {
        Changed = changed;
    }

    /// <summary>Gets whether this command changed the candidate relative to its immediately prior plugin state.</summary>
    public bool Changed { get; }
}
