using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.ViewModels;

/// <summary>
/// Maps one supported game to its exact native release and output master-style capabilities.
/// </summary>
public sealed class NativeWorkspaceGameOption
{
    /// <summary>Initializes a supported native workspace game option.</summary>
    /// <param name="game">The CreationsForge game identity.</param>
    /// <param name="displayName">The user-facing game name.</param>
    /// <param name="release">The exact Mutagen release used by the engine.</param>
    /// <param name="supportedMasterStyles">The output master styles representable by the release.</param>
    /// <exception cref="ArgumentException">Thrown when the display name or style list is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="supportedMasterStyles"/> is <see langword="null"/>.</exception>
    public NativeWorkspaceGameOption(
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

    /// <summary>Creates the fixed native workspace options supported by CreationsForge.</summary>
    /// <returns>Starfield, Fallout 4, and Skyrim Special Edition with their exact native releases.</returns>
    public static IReadOnlyList<NativeWorkspaceGameOption> CreateSupportedGames()
    {
        return
        [
            new NativeWorkspaceGameOption(
                SupportedGame.Starfield,
                "Starfield",
                GameRelease.Starfield,
                [OutputMasterStyle.Full, OutputMasterStyle.Small, OutputMasterStyle.Medium]),
            new NativeWorkspaceGameOption(
                SupportedGame.Fallout4,
                "Fallout 4",
                GameRelease.Fallout4,
                [OutputMasterStyle.Full, OutputMasterStyle.Small]),
            new NativeWorkspaceGameOption(
                SupportedGame.Skyrim,
                "Skyrim Special Edition",
                GameRelease.SkyrimSE,
                [OutputMasterStyle.Full, OutputMasterStyle.Small])
        ];
    }
}
