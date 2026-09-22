using System.Reflection;
using CreationsForge.Engine;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;

namespace CreationsForge.Fallout4;

/// <summary>
/// Identifies the production Fallout 4 Mutagen package boundary without defining workspace or editing behavior.
/// </summary>
public sealed class Fallout4GameIntegration : IGameIntegration
{
    /// <inheritdoc />
    public GameRelease Release => GameRelease.Fallout4;

    /// <inheritdoc />
    public string MutagenPackageId => "Mutagen.Bethesda.Fallout4";

    /// <inheritdoc />
    public Assembly MutagenAssembly => typeof(Fallout4Mod).Assembly;
}
