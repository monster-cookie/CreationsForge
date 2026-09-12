using System.ComponentModel;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Services;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Describes one closed command seed intent without accepting caller-supplied paths or callbacks.</summary>
public sealed class NativeFormListDraftSeedSelection
{
    /// <summary>Initializes one validated closed selection.</summary>
    private NativeFormListDraftSeedSelection(NativeFormListDraftSeedSelectionKind kind, int? index, int? sourceIndex, int? destinationIndex)
    {
        Kind = kind;
        Index = index;
        SourceIndex = sourceIndex;
        DestinationIndex = destinationIndex;
    }

    /// <summary>Gets the closed seed intent.</summary>
    public NativeFormListDraftSeedSelectionKind Kind { get; }

    /// <summary>Gets the selected or insertion index when applicable.</summary>
    public int? Index { get; }

    /// <summary>Gets the move source index when applicable.</summary>
    public int? SourceIndex { get; }

    /// <summary>Gets the move destination index when applicable.</summary>
    public int? DestinationIndex { get; }

    /// <summary>Creates the complete current-value seed intent.</summary>
    /// <returns>A current-value selection.</returns>
    public static NativeFormListDraftSeedSelection CurrentValue()
    {
        return new NativeFormListDraftSeedSelection(NativeFormListDraftSeedSelectionKind.CurrentValue, null, null, null);
    }

    /// <summary>Creates an existing-element selection.</summary>
    /// <param name="index">The non-negative existing element position.</param>
    /// <returns>An existing-element selection.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
    public static NativeFormListDraftSeedSelection AtIndex(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        return new NativeFormListDraftSeedSelection(NativeFormListDraftSeedSelectionKind.AtIndex, index, null, null);
    }

    /// <summary>Creates an explicit insertion-position selection.</summary>
    /// <param name="index">The non-negative insertion position.</param>
    /// <returns>An insertion selection.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative.</exception>
    public static NativeFormListDraftSeedSelection InsertAt(int index)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index);
        return new NativeFormListDraftSeedSelection(NativeFormListDraftSeedSelectionKind.InsertAt, index, null, null);
    }

    /// <summary>Creates an explicit collection move selection.</summary>
    /// <param name="sourceIndex">The non-negative existing source position.</param>
    /// <param name="destinationIndex">The non-negative destination position.</param>
    /// <returns>A move selection.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when either index is negative.</exception>
    public static NativeFormListDraftSeedSelection Move(int sourceIndex, int destinationIndex)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(sourceIndex);
        ArgumentOutOfRangeException.ThrowIfNegative(destinationIndex);
        return new NativeFormListDraftSeedSelection(NativeFormListDraftSeedSelectionKind.Move, null, sourceIndex, destinationIndex);
    }
}
