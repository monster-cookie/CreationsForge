namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Confirms that a plugin output candidate became the workspace's selected output.
/// </summary>
public sealed class OutputSelectionReceipt
{
    /// <summary>Initializes an output-selection receipt.</summary>
    /// <param name="output">The selected output association.</param>
    /// <param name="baseline">The complete plugin output-set baseline established while opening.</param>
    /// <param name="revision">The resulting workspace revision.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> or <paramref name="baseline"/> is <see langword="null"/>.</exception>
    public OutputSelectionReceipt(
        OutputAssociation output,
        OutputArtifactSetBaseline baseline,
        WorkspaceRevision revision)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(baseline);
        Output = output;
        Baseline = baseline;
        Revision = revision;
    }

    /// <summary>Gets the selected output association.</summary>
    public OutputAssociation Output { get; }

    /// <summary>Gets the complete plugin output-set baseline.</summary>
    public OutputArtifactSetBaseline Baseline { get; }

    /// <summary>Gets the resulting workspace revision.</summary>
    public WorkspaceRevision Revision { get; }
}
