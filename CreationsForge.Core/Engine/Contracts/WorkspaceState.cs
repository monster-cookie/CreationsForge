using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Captures one atomic UI-neutral snapshot of workspace identity, output metadata, synchronization, and revision.</summary>
public sealed class WorkspaceState
{
    /// <summary>Initializes one immutable atomic workspace-state snapshot.</summary>
    /// <param name="game">The supported CreationsForge game selected when the workspace opened.</param>
    /// <param name="release">The exact plugin release selected when the workspace opened.</param>
    /// <param name="output">The selected output association, or <see langword="null"/> before output selection.</param>
    /// <param name="outputBaseline">The selected complete artifact baseline, or <see langword="null"/> before output selection.</param>
    /// <param name="outputSynchronization">The current ready, recovery-required, or reopen-required synchronization state.</param>
    /// <param name="revision">The exact workspace revision contextualizing every value in this snapshot.</param>
    /// <exception cref="ArgumentException">Thrown when output association and baseline nullability disagree.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="outputSynchronization"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a game or release value is undefined.</exception>
    public WorkspaceState(
        SupportedGame game,
        GameRelease release,
        OutputAssociation? output,
        OutputArtifactSetBaseline? outputBaseline,
        OutputSynchronizationState outputSynchronization,
        WorkspaceRevision revision)
    {
        if (!Enum.IsDefined(game))
        {
            throw new ArgumentOutOfRangeException(nameof(game));
        }

        if (!Enum.IsDefined(release))
        {
            throw new ArgumentOutOfRangeException(nameof(release));
        }

        if ((output is null) != (outputBaseline is null))
        {
            throw new ArgumentException(
                "A selected output association and its complete artifact baseline must be present or absent together.",
                nameof(outputBaseline));
        }

        ArgumentNullException.ThrowIfNull(outputSynchronization);
        Game = game;
        Release = release;
        Output = output;
        OutputBaseline = outputBaseline;
        OutputSynchronization = outputSynchronization;
        Revision = revision;
    }

    /// <summary>Gets the supported CreationsForge game selected when the workspace opened.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact plugin release selected when the workspace opened.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the selected output association, or <see langword="null"/> before output selection.</summary>
    public OutputAssociation? Output { get; }

    /// <summary>Gets the selected complete artifact baseline, or <see langword="null"/> before output selection.</summary>
    public OutputArtifactSetBaseline? OutputBaseline { get; }

    /// <summary>Gets whether ordinary output operations are ready or await explicit recovery or reopen.</summary>
    public OutputSynchronizationState OutputSynchronization { get; }

    /// <summary>Gets the exact workspace revision contextualizing every value in this snapshot.</summary>
    public WorkspaceRevision Revision { get; }
}
