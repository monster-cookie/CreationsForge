using System.Reflection;
using CreationsForge.Engine;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield;

/// <summary>
/// Identifies the production Starfield Mutagen package boundary without defining workspace or editing behavior.
/// </summary>
public sealed class StarfieldGameIntegration : IGameIntegration
{
    /// <inheritdoc />
    public GameRelease Release => GameRelease.Starfield;

    /// <inheritdoc />
    public string MutagenPackageId => "Mutagen.Bethesda.Starfield";

    /// <inheritdoc />
    public Assembly MutagenAssembly => typeof(StarfieldMod).Assembly;
}
