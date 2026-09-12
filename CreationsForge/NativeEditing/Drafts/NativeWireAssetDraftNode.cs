using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents an asset path graph with authoritative given-path and null state.</summary>
public sealed class NativeWireAssetDraftNode : NativeWireObjectDraftNode
{
    /// <summary>The optional read-only projections invalidated by an authoritative asset edit.</summary>
    private IReadOnlyList<NativeWireDraftNode> DerivedProjections = Array.Empty<NativeWireDraftNode>();

    /// <summary>Initializes one asset draft.</summary>
    internal NativeWireAssetDraftNode(string path, string displayName, NativeWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, NativeWireDraftValueState valueState, IEnumerable<NativeWireObjectDraftProperty> properties)
        : base(NativeWireDraftNodeKind.Asset, path, displayName, descriptor, isRequired, isReadOnly, valueState, properties)
    {
    }

    /// <summary>Binds the authoritative asset fields to their optional normalized projections.</summary>
    /// <param name="isNull">The required editable native null-state field.</param>
    /// <param name="givenPath">The required editable caller-supplied path field.</param>
    /// <param name="derivedProjections">The optional read-only path projections to omit after either authority changes.</param>
    internal void BindAuthoritativeFields(NativeWireBooleanDraftNode isNull, NativeWireStringDraftNode givenPath, IEnumerable<NativeWireDraftNode> derivedProjections)
    {
        ArgumentNullException.ThrowIfNull(isNull);
        ArgumentNullException.ThrowIfNull(givenPath);
        ArgumentNullException.ThrowIfNull(derivedProjections);
        DerivedProjections = Array.AsReadOnly(derivedProjections.ToArray());
        isNull.PropertyChanged += AuthoritativeFieldChanged;
        givenPath.PropertyChanged += AuthoritativeFieldChanged;
    }

    /// <summary>Invalidates stale normalized projections after the null state or supplied path changes.</summary>
    /// <param name="sender">The changed authoritative field.</param>
    /// <param name="eventArgs">The observable property change details.</param>
    private void AuthoritativeFieldChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName != nameof(NativeWireBooleanDraftNode.Value) && eventArgs.PropertyName != nameof(NativeWireStringDraftNode.Value))
        {
            return;
        }

        foreach (var projection in DerivedProjections)
        {
            projection.InvalidateOptionalReadOnlyProjection();
        }
    }
}
