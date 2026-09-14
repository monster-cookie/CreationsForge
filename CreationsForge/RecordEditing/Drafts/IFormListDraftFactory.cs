using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Captures revision-bound record seeds and creates typed command drafts.</summary>
public interface IFormListDraftFactory
{
    /// <summary>Captures one detached expanded record as an opaque revision-bound seed.</summary>
    /// <param name="context">The exact resolved catalog context.</param>
    /// <param name="workspaceId">The non-empty active workspace identity.</param>
    /// <param name="revision">The exact revision that produced the record.</param>
    /// <param name="recordContext">The exact selected record context.</param>
    /// <param name="record">The detached expanded record object.</param>
    /// <param name="limits">The per-operation resource limits.</param>
    /// <param name="cancellationToken">A token observed throughout bounded capture.</param>
    /// <returns>The opaque immutable seed or a typed input failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    EngineResult<FormListDraftSeed> CaptureSeed(FormListWireCatalogContext context, Guid workspaceId, WorkspaceRevision revision, FormListContext recordContext, JsonElement record, RecordWireReadLimits limits, CancellationToken cancellationToken = default);

    /// <summary>Creates one typed command draft from its exact schema and optional matching seed.</summary>
    /// <param name="context">The exact resolved catalog context.</param>
    /// <param name="commandKey">The exact catalog-bound command node key.</param>
    /// <param name="seed">An optional matching revision-bound seed.</param>
    /// <param name="selection">The closed command-specific seed intent.</param>
    /// <param name="limits">The per-operation resource limits.</param>
    /// <param name="cancellationToken">A token observed throughout bounded materialization.</param>
    /// <returns>The typed command draft or a typed schema, seed, or value failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    EngineResult<FormListDraft> Create(FormListWireCatalogContext context, RecordWireSchemaNodeKey commandKey, FormListDraftSeed? seed, FormListDraftSeedSelection selection, RecordWireReadLimits limits, CancellationToken cancellationToken = default);
}
