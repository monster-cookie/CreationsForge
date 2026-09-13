using System.Collections.ObjectModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents an ordered mutable array draft with schema-bound element creation.</summary>
public sealed class RecordWireArrayDraftNode : RecordWireDraftNode
{
    /// <summary>The mutable owned item collection.</summary>
    private readonly ObservableCollection<RecordWireDraftNode> MutableItems;

    /// <summary>The public read-only observable item collection.</summary>
    private readonly ReadOnlyObservableCollection<RecordWireDraftNode> PublicItems;

    /// <summary>Creates a new explicit item for one array position.</summary>
    private readonly Func<int, CancellationToken, EngineResult<RecordWireDraftNode>> ItemFactory;

    /// <summary>Initializes one ordered array draft.</summary>
    /// <param name="path">The exact JSON path.</param>
    /// <param name="displayName">The user-facing field name.</param>
    /// <param name="descriptor">The resolved array schema.</param>
    /// <param name="isRequired">Whether the containing object requires the array.</param>
    /// <param name="isReadOnly">Whether the array is derived.</param>
    /// <param name="valueState">How the array was initialized.</param>
    /// <param name="items">The initial ordered items.</param>
    /// <param name="itemFactory">The bounded explicit item factory.</param>
    internal RecordWireArrayDraftNode(
        string path,
        string displayName,
        RecordWireSchemaDescriptor descriptor,
        bool isRequired,
        bool isReadOnly,
        RecordWireDraftValueState valueState,
        IEnumerable<RecordWireDraftNode> items,
        Func<int, CancellationToken, EngineResult<RecordWireDraftNode>> itemFactory)
        : base(RecordWireDraftNodeKind.Array, path, displayName, descriptor, isRequired, isReadOnly, valueState)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(itemFactory);
        MutableItems = new ObservableCollection<RecordWireDraftNode>(items);
        PublicItems = new ReadOnlyObservableCollection<RecordWireDraftNode>(MutableItems);
        ItemFactory = itemFactory;
        foreach (var child in MutableItems)
        {
            ObserveChild(child);
        }
    }

    /// <summary>Gets the ordered typed items.</summary>
    public ReadOnlyObservableCollection<RecordWireDraftNode> Items => PublicItems;

    /// <inheritdoc />
    public override IReadOnlyList<RecordWireDraftNode> Children => PublicItems;

    /// <summary>Creates and inserts one explicit schema-bound item.</summary>
    /// <param name="index">The insertion position from zero through the current count.</param>
    /// <param name="cancellationToken">A token observed during lazy schema and default materialization.</param>
    /// <returns>The inserted node or a typed input failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public EngineResult<RecordWireDraftNode> Insert(int index, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (index < 0 || index > MutableItems.Count)
        {
            return Failure($"{Path}: insertion index {index} is outside the valid range 0 through {MutableItems.Count}.");
        }

        var created = ItemFactory(index, cancellationToken);
        if (!created.Succeeded || created.Value is null)
        {
            return created;
        }

        MutableItems.Insert(index, created.Value);
        ObserveChild(created.Value);
        RebaseChildren();
        MarkChanged(nameof(Items));
        return created;
    }

    /// <summary>Removes one existing item while preserving the order of its siblings.</summary>
    /// <param name="index">The zero-based item position.</param>
    /// <returns><see langword="true"/> when an item was removed; otherwise <see langword="false"/>.</returns>
    public bool Remove(int index)
    {
        if (index < 0 || index >= MutableItems.Count)
        {
            return false;
        }

        var removed = MutableItems[index];
        StopObservingChild(removed);
        MutableItems.RemoveAt(index);
        RebaseChildren();
        MarkChanged(nameof(Items));
        return true;
    }

    /// <summary>Moves one item without changing any item value or duplicate identity.</summary>
    /// <param name="sourceIndex">The existing zero-based position.</param>
    /// <param name="destinationIndex">The destination zero-based position.</param>
    /// <returns><see langword="true"/> when a move occurred; otherwise <see langword="false"/>.</returns>
    public bool Move(int sourceIndex, int destinationIndex)
    {
        if (sourceIndex < 0 || sourceIndex >= MutableItems.Count || destinationIndex < 0 || destinationIndex >= MutableItems.Count || sourceIndex == destinationIndex)
        {
            return false;
        }

        MutableItems.Move(sourceIndex, destinationIndex);
        RebaseChildren();
        MarkChanged(nameof(Items));
        return true;
    }

    /// <summary>Creates and replaces one existing item with a fresh explicit schema-bound value.</summary>
    /// <param name="index">The existing zero-based position.</param>
    /// <param name="cancellationToken">A token observed during lazy schema and default materialization.</param>
    /// <returns>The replacement node or a typed input failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public EngineResult<RecordWireDraftNode> Replace(int index, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (index < 0 || index >= MutableItems.Count)
        {
            return Failure($"{Path}: replacement index {index} is outside the valid range 0 through {Math.Max(0, MutableItems.Count - 1)}.");
        }

        var created = ItemFactory(index, cancellationToken);
        if (!created.Succeeded || created.Value is null)
        {
            return created;
        }

        StopObservingChild(MutableItems[index]);
        MutableItems[index] = created.Value;
        ObserveChild(created.Value);
        RebaseChildren();
        MarkChanged(nameof(Items));
        return created;
    }

    /// <summary>Explicitly removes every item from this array.</summary>
    public void Clear()
    {
        if (MutableItems.Count == 0)
        {
            return;
        }

        foreach (var child in MutableItems)
        {
            StopObservingChild(child);
        }

        MutableItems.Clear();
        MarkChanged(nameof(Items));
    }

    /// <inheritdoc />
    protected override void RebaseChildren()
    {
        for (var index = 0; index < MutableItems.Count; index++)
        {
            MutableItems[index].RebasePath($"{Path}[{index}]");
        }
    }

    /// <summary>Creates one typed array-operation failure.</summary>
    /// <param name="message">The path-specific diagnostic message.</param>
    /// <returns>A failed immutable engine result.</returns>
    private static EngineResult<RecordWireDraftNode> Failure(string message)
    {
        return EngineResult<RecordWireDraftNode>.Failure(new EngineError(EngineErrorCode.InvalidRequest, message));
    }
}
