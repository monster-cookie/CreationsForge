using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Validates typed request-local command drafts without constructing record objects.</summary>
public interface IFormListDraftValidator
{
    /// <summary>Validates one draft under the supplied operation limits and publishes exact node issues.</summary>
    /// <param name="draft">The typed command draft.</param>
    /// <param name="limits">The per-operation resource limits.</param>
    /// <param name="cancellationToken">A token observed throughout traversal.</param>
    /// <returns>The immutable complete validation result.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    FormListDraftValidationResult Validate(FormListDraft draft, RecordWireReadLimits limits, CancellationToken cancellationToken = default);
}
