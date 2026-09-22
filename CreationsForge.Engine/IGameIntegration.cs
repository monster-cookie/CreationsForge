using System.Reflection;
using Mutagen.Bethesda;

namespace CreationsForge.Engine;

/// <summary>
/// Describes one game-specific Mutagen integration available to a CreationsForge host.
/// </summary>
public interface IGameIntegration
{
    /// <summary>Gets the exact Mutagen game release supported by this integration.</summary>
    GameRelease Release { get; }

    /// <summary>Gets the NuGet package identifier that supplies the game-specific Mutagen types.</summary>
    string MutagenPackageId { get; }

    /// <summary>Gets the loaded game-specific Mutagen assembly used by the integration.</summary>
    Assembly MutagenAssembly { get; }
}
