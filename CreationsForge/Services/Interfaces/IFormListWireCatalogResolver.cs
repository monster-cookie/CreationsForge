using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using CreationsForge.Services;
using Mutagen.Bethesda;

namespace CreationsForge.Services.Interfaces;

/// <summary>Resolves the single exact wire schema and codec pair for an active workspace.</summary>
public interface IFormListWireCatalogResolver
{
    /// <summary>Resolves the immutable presentation context for one supported game and plugin release.</summary>
    /// <param name="game">The active CreationsForge game.</param>
    /// <param name="release">The exact plugin release used by the active workspace.</param>
    /// <returns>The unique matching catalog context, or a typed failure when registration is missing, duplicated, or inconsistent.</returns>
    EngineResult<FormListWireCatalogContext> Resolve(
        SupportedGame game,
        GameRelease release);
}
