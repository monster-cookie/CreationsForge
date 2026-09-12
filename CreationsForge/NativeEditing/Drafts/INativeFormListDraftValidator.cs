using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Validates typed request-local command drafts without constructing native objects.</summary>
public interface INativeFormListDraftValidator
{
    /// <summary>Validates one draft under the supplied operation limits and publishes exact node issues.</summary>
    /// <param name="draft">The typed command draft.</param>
    /// <param name="limits">The per-operation resource limits.</param>
    /// <param name="cancellationToken">A token observed throughout traversal.</param>
    /// <returns>The immutable complete validation result.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    NativeFormListDraftValidationResult Validate(NativeFormListDraft draft, NativeWireReadLimits limits, CancellationToken cancellationToken = default);
}
