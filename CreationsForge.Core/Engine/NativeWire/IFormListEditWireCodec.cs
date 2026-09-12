using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Core.Engine.NativeWire;

/// <summary>Decodes bounded closed wire commands for one exact game and native release.</summary>
public interface IFormListEditWireCodec
{
    /// <summary>Gets the supported CreationsForge game.</summary>
    SupportedGame Game { get; }

    /// <summary>Gets the exact native release whose compiled constructors and edit types this codec uses.</summary>
    GameRelease Release { get; }

    /// <summary>Decodes one command into an existing typed FormList edit without retaining caller JSON.</summary>
    /// <param name="commandName">The stable command discriminator.</param>
    /// <param name="arguments">The request-local closed command arguments.</param>
    /// <param name="limits">The per-operation resource limits enforced before native collection and byte allocation.</param>
    /// <param name="cancellationToken">A token observed throughout traversal and before construction.</param>
    /// <returns>A complete typed edit or a path-specific input failure; no partial native value is returned.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    NativeWireDecodeResult<FormListEdit> Decode(
        string commandName,
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken = default);
}
