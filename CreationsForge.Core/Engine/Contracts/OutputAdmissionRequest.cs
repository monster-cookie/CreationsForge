using CreationsForge.Core.Engine.NativeInputs;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Requests journal admission for one exact output before native state is opened or published.</summary>
public sealed class OutputAdmissionRequest
{
    /// <summary>Initializes an immutable output admission request.</summary>
    /// <param name="workspaceId">The workspace attempting to select or reopen the output.</param>
    /// <param name="game">The exact CreationsForge game selected by the workspace.</param>
    /// <param name="release">The exact native release selected by the workspace.</param>
    /// <param name="sourceBaseline">The complete source baseline owned by the workspace.</param>
    /// <param name="output">The exact output association being admitted.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="workspaceId"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sourceBaseline"/> or <paramref name="output"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="game"/> or <paramref name="release"/> is undefined.</exception>
    public OutputAdmissionRequest(
        Guid workspaceId,
        SupportedGame game,
        GameRelease release,
        NativeSourceInputBaseline sourceBaseline,
        OutputAssociation output)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("Output admission requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        if (!Enum.IsDefined(game))
        {
            throw new ArgumentOutOfRangeException(nameof(game));
        }

        if (!Enum.IsDefined(release))
        {
            throw new ArgumentOutOfRangeException(nameof(release));
        }

        ArgumentNullException.ThrowIfNull(sourceBaseline);
        ArgumentNullException.ThrowIfNull(output);
        WorkspaceId = workspaceId;
        Game = game;
        Release = release;
        SourceBaseline = sourceBaseline;
        Output = output;
    }

    /// <summary>Gets the workspace attempting to select or reopen the output.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the exact CreationsForge game selected by the workspace.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact native release selected by the workspace.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the complete source baseline owned by the workspace.</summary>
    public NativeSourceInputBaseline SourceBaseline { get; }

    /// <summary>Gets the exact output association being admitted.</summary>
    public OutputAssociation Output { get; }
}
