namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Reports whether one successfully applied typed command changed its unpublished native candidate.</summary>
public sealed class NativeEditMutationResult
{
    /// <summary>Initializes the result of one successfully applied native command.</summary>
    /// <param name="changed">Whether the candidate differs from its immediately prior native state because of this command.</param>
    public NativeEditMutationResult(bool changed)
    {
        Changed = changed;
    }

    /// <summary>Gets whether this command changed the candidate relative to its immediately prior native state.</summary>
    public bool Changed { get; }
}
