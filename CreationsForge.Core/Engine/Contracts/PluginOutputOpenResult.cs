namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Carries an acquired plugin output lifetime and its complete plugin-and-strings baseline.
/// </summary>
public sealed class PluginOutputOpenResult
{
    /// <summary>Initializes a plugin output-open result.</summary>
    /// <param name="output">The independently owned mutable plugin output state.</param>
    /// <param name="association">The canonical selected output association.</param>
    /// <param name="baseline">The complete observed output-set baseline.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is <see langword="null"/>.</exception>
    public PluginOutputOpenResult(
        IPluginOutputState output,
        OutputAssociation association,
        OutputArtifactSetBaseline baseline)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(association);
        ArgumentNullException.ThrowIfNull(baseline);
        Output = output;
        Association = association;
        Baseline = baseline;
    }

    /// <summary>Gets the independently owned mutable plugin output state.</summary>
    public IPluginOutputState Output { get; }

    /// <summary>Gets the canonical selected output association.</summary>
    public OutputAssociation Association { get; }

    /// <summary>Gets the complete observed output-set baseline.</summary>
    public OutputArtifactSetBaseline Baseline { get; }
}
