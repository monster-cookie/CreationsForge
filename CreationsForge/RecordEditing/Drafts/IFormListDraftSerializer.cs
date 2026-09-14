using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Serializes a valid typed command draft to detached transient command arguments.</summary>
public interface IFormListDraftSerializer
{
    /// <summary>Validates and serializes one typed draft without exposing mutable JSON to controls.</summary>
    /// <param name="draft">The typed command draft.</param>
    /// <param name="limits">The per-operation resource limits.</param>
    /// <param name="cancellationToken">A token observed throughout traversal and writing.</param>
    /// <returns>The immutable serialization result and exact issues.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    FormListDraftSerializationResult Serialize(FormListDraft draft, RecordWireReadLimits limits, CancellationToken cancellationToken = default);
}
