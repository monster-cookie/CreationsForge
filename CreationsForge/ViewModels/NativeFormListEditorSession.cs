using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>Owns the detached identity and advancing expected revision for one active native FormList edit session.</summary>
public sealed class NativeFormListEditorSession
{
    /// <summary>The exact revision forwarded by the next typed Apply operation.</summary>
    private WorkspaceRevision ExpectedRevisionValue;

    /// <summary>Initializes one session from a successful Core edit receipt and its verified workspace context.</summary>
    /// <param name="workspaceId">The non-empty workspace identity.</param>
    /// <param name="game">The active supported game.</param>
    /// <param name="release">The exact native release.</param>
    /// <param name="output">The selected output identity.</param>
    /// <param name="catalogIdentity">The exact schema and codec catalog identity.</param>
    /// <param name="receipt">The successful edit receipt.</param>
    internal NativeFormListEditorSession(
        Guid workspaceId,
        SupportedGame game,
        GameRelease release,
        OutputAssociation output,
        NativeWireSchemaCatalogIdentity catalogIdentity,
        EditReceipt receipt)
    {
        if (workspaceId == Guid.Empty)
        {
            throw new ArgumentException("An editor session requires a non-empty workspace identity.", nameof(workspaceId));
        }

        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(catalogIdentity);
        ArgumentNullException.ThrowIfNull(receipt);
        WorkspaceId = workspaceId;
        Game = game;
        Release = release;
        Output = output;
        CatalogIdentity = catalogIdentity;
        EditId = receipt.EditId;
        FormKey = receipt.FormKey;
        OriginFormKey = receipt.OriginFormKey;
        Role = receipt.Role;
        ExpectedRevisionValue = receipt.Revision;
    }

    /// <summary>Gets the workspace identity that owns this edit session.</summary>
    public Guid WorkspaceId { get; }

    /// <summary>Gets the supported game verified when the session began.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact native release verified when the session began.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the selected output identity verified when the session began.</summary>
    public OutputAssociation Output { get; }

    /// <summary>Gets the exact schema and codec catalog identity bound to the session.</summary>
    public NativeWireSchemaCatalogIdentity CatalogIdentity { get; }

    /// <summary>Gets the Core-owned staged edit identity.</summary>
    public Guid EditId { get; }

    /// <summary>Gets the staged native FormList identity.</summary>
    public FormKey FormKey { get; }

    /// <summary>Gets the original source identity for an override session.</summary>
    public FormKey? OriginFormKey { get; }

    /// <summary>Gets how the staged edit was originally acquired.</summary>
    public FormListEditRole Role { get; }

    /// <summary>Gets the exact revision captured for the next Apply operation.</summary>
    public WorkspaceRevision ExpectedRevision => ExpectedRevisionValue;

    /// <summary>Advances the session only from a known successful Core mutation receipt.</summary>
    /// <param name="revision">The receipt's resulting workspace revision.</param>
    internal void Advance(WorkspaceRevision revision)
    {
        ExpectedRevisionValue = revision;
    }
}
