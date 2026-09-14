using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Owns one detached revision-bound record seed without exposing its JSON to views.</summary>
public sealed class FormListDraftSeed
{
    /// <summary>Initializes one immutable seed and clones its detached record.</summary>
    /// <param name="workspaceId">The non-empty source workspace identity.</param>
    /// <param name="revision">The exact revision that produced the record.</param>
    /// <param name="recordContext">The exact record context.</param>
    /// <param name="catalogIdentity">The complete catalog identity used to interpret the record.</param>
    /// <param name="record">The detached expanded record object.</param>
    /// <exception cref="ArgumentException">Thrown when the workspace is empty or the record is not an object.</exception>
    internal FormListDraftSeed(Guid workspaceId, WorkspaceRevision revision, FormListContext recordContext, RecordWireSchemaCatalogIdentity catalogIdentity, JsonElement record)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("A record editor seed requires a non-empty workspace identifier.", nameof(workspaceId));
        }

        ArgumentNullException.ThrowIfNull(recordContext);
        ArgumentNullException.ThrowIfNull(catalogIdentity);
        if (record.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("A record editor seed record must be a detached JSON object.", nameof(record));
        }

        WorkspaceId = workspaceId;
        Revision = revision;
        FormKey = recordContext.Selection.FormKey;
        RecordContext = recordContext;
        CatalogIdentity = catalogIdentity;
        Record = record.Clone();
    }

    /// <summary>Gets the workspace that produced this seed.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the exact workspace revision that produced this seed.</summary>
    public WorkspaceRevision Revision { get; }

    /// <summary>Gets the record identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the exact requested and selected record context.</summary>
    public FormListContext RecordContext { get; }

    /// <summary>Gets the complete catalog identity used to interpret the seed.</summary>
    public RecordWireSchemaCatalogIdentity CatalogIdentity { get; }

    /// <summary>Gets the internally owned detached record for command-specific extraction.</summary>
    internal JsonElement Record { get; }
}
