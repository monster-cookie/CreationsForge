namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Requests creation and plugin reopen of a complete output set in a private staging directory.
/// </summary>
public sealed class PluginWriteRequest
{
    /// <summary>Initializes a plugin staged-write request.</summary>
    /// <param name="stagingDirectoryPath">The private initially empty artifact directory owned by the save coordinator.</param>
    /// <param name="output">The selected output association.</param>
    /// <param name="expectedOutputBaseline">The workspace's current complete destination baseline, including any fresh identity established by explicit recovery adoption.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="stagingDirectoryPath"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> or <paramref name="expectedOutputBaseline"/> is <see langword="null"/>.</exception>
    public PluginWriteRequest(
        string stagingDirectoryPath,
        OutputAssociation output,
        OutputArtifactSetBaseline expectedOutputBaseline)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(stagingDirectoryPath);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(expectedOutputBaseline);
        StagingDirectoryPath = stagingDirectoryPath;
        Output = output;
        ExpectedOutputBaseline = expectedOutputBaseline;
    }

    /// <summary>Gets the private initially empty artifact directory owned by the save coordinator.</summary>
    public string StagingDirectoryPath { get; }

    /// <summary>Gets the selected output association.</summary>
    public OutputAssociation Output { get; }

    /// <summary>Gets the current complete destination baseline used for freshness validation rather than the candidate's original file identities.</summary>
    public OutputArtifactSetBaseline ExpectedOutputBaseline { get; }
}
