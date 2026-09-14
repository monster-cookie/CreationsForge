using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Owns one request-local typed command draft and its exact seed and catalog identities.</summary>
public sealed class FormListDraft : INotifyPropertyChanged
{
    /// <summary>The current immutable validation issue set.</summary>
    private IReadOnlyList<RecordWireDraftIssue> CurrentIssues = Array.Empty<RecordWireDraftIssue>();

    /// <summary>Initializes one typed command draft.</summary>
    /// <param name="commandKey">The exact catalog-bound command node key.</param>
    /// <param name="catalogIdentity">The complete catalog identity.</param>
    /// <param name="seed">The optional matching revision-bound seed.</param>
    /// <param name="root">The typed closed command-arguments object.</param>
    /// <param name="limits">The resource limits used to materialize this draft.</param>
    internal FormListDraft(RecordWireSchemaNodeKey commandKey, RecordWireSchemaCatalogIdentity catalogIdentity, FormListDraftSeed? seed, RecordWireDraftNode root, RecordWireReadLimits limits)
    {
        ArgumentNullException.ThrowIfNull(commandKey);
        ArgumentNullException.ThrowIfNull(catalogIdentity);
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(limits);
        CommandKey = commandKey;
        CatalogIdentity = catalogIdentity;
        Seed = seed;
        Root = root;
        Limits = limits;
        Root.Changed += RootChanged;
    }

    /// <summary>Raised when draft change or issue state changes.</summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>Gets the exact catalog-bound command key.</summary>
    public RecordWireSchemaNodeKey CommandKey { get; }

    /// <summary>Gets the exact stable command discriminator.</summary>
    public string CommandName => CommandKey.Name;

    /// <summary>Gets the complete game, release, schema-version, and catalog-content identity.</summary>
    public RecordWireSchemaCatalogIdentity CatalogIdentity { get; }

    /// <summary>Gets the optional matching revision-bound source seed.</summary>
    public FormListDraftSeed? Seed { get; }

    /// <summary>Gets the root typed command-arguments node.</summary>
    public RecordWireDraftNode Root { get; }

    /// <summary>Gets the immutable issues from the latest validation pass.</summary>
    public IReadOnlyList<RecordWireDraftIssue> Issues => CurrentIssues;

    /// <summary>Gets whether any typed value changed after initialization.</summary>
    public bool HasChanges => Root.HasChanges;

    /// <summary>Gets the resource limits under which the draft was materialized.</summary>
    public RecordWireReadLimits Limits { get; }

    /// <summary>Publishes one immutable validation result onto the draft and exact nodes.</summary>
    /// <param name="issues">The complete latest validation issue set.</param>
    internal void SetIssues(IEnumerable<RecordWireDraftIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(issues);
        CurrentIssues = Array.AsReadOnly(issues.ToArray());
        foreach (var node in EnumerateNodes(Root))
        {
            node.SetIssues(CurrentIssues.Where(issue => ReferenceEquals(issue.Node, node)));
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Issues)));
    }

    /// <summary>Enumerates the root and all currently materialized descendants.</summary>
    /// <param name="root">The root node.</param>
    /// <returns>The depth-first typed nodes.</returns>
    internal static IEnumerable<RecordWireDraftNode> EnumerateNodes(RecordWireDraftNode root)
    {
        yield return root;
        foreach (var child in root.Children)
        {
            foreach (var descendant in EnumerateNodes(child))
            {
                yield return descendant;
            }
        }
    }

    /// <summary>Publishes a root change through the draft.</summary>
    /// <param name="sender">The changed root.</param>
    /// <param name="eventArgs">The empty change event data.</param>
    private void RootChanged(object? sender, EventArgs eventArgs)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasChanges)));
    }
}
