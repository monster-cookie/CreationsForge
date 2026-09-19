using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.ViewModels;

/// <summary>
/// Maps one supported game to its exact plugin release and output master-style capabilities.
/// </summary>
public sealed class WorkspaceGameOption
{
    /// <summary>Initializes a supported workspace game option.</summary>
    /// <param name="game">The CreationsForge game identity.</param>
    /// <param name="displayName">The user-facing game name.</param>
    /// <param name="release">The exact Mutagen release used by the engine.</param>
    /// <param name="supportedMasterStyles">The output master styles representable by the release.</param>
    /// <exception cref="ArgumentException">Thrown when the display name or style list is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="supportedMasterStyles"/> is <see langword="null"/>.</exception>
    public WorkspaceGameOption(
        SupportedGame game,
        string displayName,
        GameRelease release,
        IReadOnlyList<OutputMasterStyle> supportedMasterStyles)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(supportedMasterStyles);
        if (supportedMasterStyles.Count == 0)
        {
            throw new ArgumentException("At least one output master style is required.", nameof(supportedMasterStyles));
        }

        Game = game;
        DisplayName = displayName;
        Release = release;
        SupportedMasterStyles = Array.AsReadOnly(supportedMasterStyles.ToArray());
    }

    /// <summary>Gets the CreationsForge game identity.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the user-facing game name.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the exact Mutagen release used by the engine.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the output master styles representable by this release.</summary>
    public IReadOnlyList<OutputMasterStyle> SupportedMasterStyles { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return DisplayName;
    }

    /// <summary>Creates the fixed workspace options supported by CreationsForge.</summary>
    /// <returns>Starfield, Fallout 4, and Skyrim Special Edition with their exact plugin releases.</returns>
    public static IReadOnlyList<WorkspaceGameOption> CreateSupportedGames()
    {
        return
        [
            new WorkspaceGameOption(
                SupportedGame.Starfield,
                "Starfield",
                GameRelease.Starfield,
                [OutputMasterStyle.Small, OutputMasterStyle.Medium, OutputMasterStyle.Full]),
            new WorkspaceGameOption(
                SupportedGame.Fallout4,
                "Fallout 4",
                GameRelease.Fallout4,
                [OutputMasterStyle.Small, OutputMasterStyle.Full]),
            new WorkspaceGameOption(
                SupportedGame.Skyrim,
                "Skyrim Special Edition",
                GameRelease.SkyrimSE,
                [OutputMasterStyle.Small, OutputMasterStyle.Full])
        ];
    }
}
