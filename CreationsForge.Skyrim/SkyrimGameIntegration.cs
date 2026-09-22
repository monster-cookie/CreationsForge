using System.Reflection;
using CreationsForge.Engine;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim;

/// <summary>
/// Identifies the production Skyrim Special Edition Mutagen package boundary without defining workspace or editing behavior.
/// </summary>
public sealed class SkyrimGameIntegration : IGameIntegration
{
    /// <inheritdoc />
    public GameRelease Release => GameRelease.SkyrimSE;

    /// <inheritdoc />
    public string MutagenPackageId => "Mutagen.Bethesda.Skyrim";

    /// <inheritdoc />
    public Assembly MutagenAssembly => typeof(SkyrimMod).Assembly;
}
