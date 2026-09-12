using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents an owner-mode-sensitive FormLink-or-index while retaining both exact projections.</summary>
public sealed class NativeWireFormLinkOrIndexDraftNode : NativeWireDraftNode
{
    /// <summary>Whether the exact retained index projection is JSON null.</summary>
    private bool CurrentIsIndexNull;

    /// <summary>The exact index text, including inactive projection data.</summary>
    private string CurrentIndexText;

    /// <summary>The current owner-selected active branch.</summary>
    private NativeWireFormLinkOrIndexActiveBranch CurrentActiveBranch;

    /// <summary>The containing object's alias-mode authority after factory binding.</summary>
    private NativeWireBooleanDraftNode? UsesAliasesOwner;

    /// <summary>The containing object's package-data-mode authority after factory binding.</summary>
    private NativeWireBooleanDraftNode? UsesPackageDataOwner;

    /// <summary>Initializes one FormLink-or-index draft.</summary>
    /// <param name="path">The exact JSON path.</param>
    /// <param name="displayName">The user-facing field name.</param>
    /// <param name="descriptor">The resolved FormLink-or-index schema.</param>
    /// <param name="isRequired">Whether the containing object requires the value.</param>
    /// <param name="isReadOnly">Whether the value is derived.</param>
    /// <param name="valueState">How the value was initialized.</param>
    /// <param name="isIndexNull">Whether the exact retained index is JSON null.</param>
    /// <param name="indexText">The exact retained unsigned index text when non-null.</param>
    /// <param name="link">The exact retained FormLink projection.</param>
    /// <param name="activeBranch">The initial owner-selected active branch.</param>
    internal NativeWireFormLinkOrIndexDraftNode(
        string path,
        string displayName,
        NativeWireSchemaDescriptor descriptor,
        bool isRequired,
        bool isReadOnly,
        NativeWireDraftValueState valueState,
        bool isIndexNull,
        string indexText,
        NativeWireFormLinkDraftNode link,
        NativeWireFormLinkOrIndexActiveBranch activeBranch)
        : base(NativeWireDraftNodeKind.FormLinkOrIndex, path, displayName, descriptor, isRequired, isReadOnly, valueState)
    {
        ArgumentNullException.ThrowIfNull(indexText);
        ArgumentNullException.ThrowIfNull(link);
        CurrentIsIndexNull = isIndexNull;
        CurrentIndexText = indexText;
        Link = link;
        CurrentActiveBranch = activeBranch;
        ObserveChild(Link);
    }

    /// <summary>Gets or sets whether the exact retained index projection is JSON null.</summary>
    public bool IsIndexNull
    {
        get => CurrentIsIndexNull;
        set
        {
            if (IsReadOnly || CurrentIsIndexNull == value)
            {
                return;
            }

            CurrentIsIndexNull = value;
            MarkChanged();
        }
    }

    /// <summary>Gets or sets the exact unsigned index text, retained while the index is null or the link branch is active.</summary>
    public string IndexText
    {
        get => CurrentIndexText;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (IsReadOnly || string.Equals(CurrentIndexText, value, StringComparison.Ordinal))
            {
                return;
            }

            CurrentIndexText = value;
            MarkChanged();
        }
    }

    /// <summary>Gets the exact FormLink projection, retained even while an index branch is active.</summary>
    public NativeWireFormLinkDraftNode Link { get; }

    /// <summary>Gets the owner-selected authoritative branch.</summary>
    public NativeWireFormLinkOrIndexActiveBranch ActiveBranch => CurrentActiveBranch;

    /// <summary>Gets whether the FormLink projection is authoritative.</summary>
    public bool UsesLink => CurrentActiveBranch == NativeWireFormLinkOrIndexActiveBranch.Link;

    /// <summary>Gets whether the index represents an alias.</summary>
    public bool UsesAlias => CurrentActiveBranch == NativeWireFormLinkOrIndexActiveBranch.AliasIndex;

    /// <summary>Gets whether the index represents package data.</summary>
    public bool UsesPackageData => CurrentActiveBranch == NativeWireFormLinkOrIndexActiveBranch.PackageDataIndex;

    /// <inheritdoc />
    public override IReadOnlyList<NativeWireDraftNode> Children => new NativeWireDraftNode[] { Link };

    /// <summary>Binds the exact containing-object flags that select this value's active projection.</summary>
    /// <param name="usesAliases">The direct Boolean <c>UseAliases</c> sibling.</param>
    /// <param name="usesPackageData">The direct Boolean <c>UsePackageData</c> sibling.</param>
    internal void BindOwnerMode(NativeWireBooleanDraftNode usesAliases, NativeWireBooleanDraftNode usesPackageData)
    {
        ArgumentNullException.ThrowIfNull(usesAliases);
        ArgumentNullException.ThrowIfNull(usesPackageData);
        if (UsesAliasesOwner is not null)
        {
            UsesAliasesOwner.PropertyChanged -= OwnerModeChanged;
        }

        if (UsesPackageDataOwner is not null)
        {
            UsesPackageDataOwner.PropertyChanged -= OwnerModeChanged;
        }

        UsesAliasesOwner = usesAliases;
        UsesPackageDataOwner = usesPackageData;
        UsesAliasesOwner.PropertyChanged += OwnerModeChanged;
        UsesPackageDataOwner.PropertyChanged += OwnerModeChanged;
        RefreshOwnerMode(UsesAliasesOwner.Value, UsesPackageDataOwner.Value);
    }

    /// <summary>Updates the derived active branch when the containing owner's flags change.</summary>
    /// <param name="usesAlias">Whether the owner selects alias-index semantics.</param>
    /// <param name="usesPackageData">Whether the owner selects package-data-index semantics when alias mode is not selected.</param>
    public void RefreshOwnerMode(bool usesAlias, bool usesPackageData)
    {
        var branch = usesAlias
            ? NativeWireFormLinkOrIndexActiveBranch.AliasIndex
            : usesPackageData
                ? NativeWireFormLinkOrIndexActiveBranch.PackageDataIndex
                : NativeWireFormLinkOrIndexActiveBranch.Link;
        if (CurrentActiveBranch == branch)
        {
            return;
        }

        CurrentActiveBranch = branch;
        OnPropertyChanged(nameof(ActiveBranch));
        OnPropertyChanged(nameof(UsesLink));
        OnPropertyChanged(nameof(UsesAlias));
        OnPropertyChanged(nameof(UsesPackageData));
        MarkChanged(nameof(Children));
    }

    /// <inheritdoc />
    protected override void RebaseChildren()
    {
        Link.RebasePath($"{Path}.link");
    }

    /// <summary>Refreshes the active branch after one bound owner flag changes.</summary>
    /// <param name="sender">The changed owner flag.</param>
    /// <param name="eventArgs">The observable property change details.</param>
    private void OwnerModeChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(NativeWireBooleanDraftNode.Value) && UsesAliasesOwner is not null && UsesPackageDataOwner is not null)
        {
            RefreshOwnerMode(UsesAliasesOwner.Value, UsesPackageDataOwner.Value);
        }
    }
}
