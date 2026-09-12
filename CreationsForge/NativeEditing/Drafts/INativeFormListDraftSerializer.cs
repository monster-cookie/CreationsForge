using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Serializes a valid typed command draft to detached transient command arguments.</summary>
public interface INativeFormListDraftSerializer
{
    /// <summary>Validates and serializes one typed draft without exposing mutable JSON to controls.</summary>
    /// <param name="draft">The typed command draft.</param>
    /// <param name="limits">The per-operation resource limits.</param>
    /// <param name="cancellationToken">A token observed throughout traversal and writing.</param>
    /// <returns>The immutable serialization result and exact issues.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    NativeFormListDraftSerializationResult Serialize(NativeFormListDraft draft, NativeWireReadLimits limits, CancellationToken cancellationToken = default);
}
