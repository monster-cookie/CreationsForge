using System.Collections.ObjectModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents a value whose whole JSON identity may be null.</summary>
public sealed class RecordWireNullableDraftNode : RecordWireDraftNode
{
    /// <summary>Creates the present non-null value on explicit request.</summary>
    private readonly Func<string, CancellationToken, EngineResult<RecordWireDraftNode>> ValueFactory;

    /// <summary>The current present value, retained while the nullable wrapper is null.</summary>
    private RecordWireDraftNode? CurrentValue;

    /// <summary>Whether the whole wrapper is currently JSON null.</summary>
    private bool WholeValueIsNull;

    /// <summary>Initializes one whole-value nullable wrapper.</summary>
    /// <param name="path">The exact JSON path.</param>
    /// <param name="displayName">The user-facing field name.</param>
    /// <param name="descriptor">The resolved nullable schema.</param>
    /// <param name="isRequired">Whether the containing object requires the wrapper property.</param>
    /// <param name="isReadOnly">Whether the value is derived.</param>
    /// <param name="valueState">How the wrapper was initialized.</param>
    /// <param name="isNull">Whether the whole current value is JSON null.</param>
    /// <param name="value">The initially materialized non-null value, when present.</param>
    /// <param name="valueFactory">The bounded explicit present-value factory, supplied the wrapper's current path after structural edits.</param>
    internal RecordWireNullableDraftNode(
        string path,
        string displayName,
        RecordWireSchemaDescriptor descriptor,
        bool isRequired,
        bool isReadOnly,
        RecordWireDraftValueState valueState,
        bool isNull,
        RecordWireDraftNode? value,
        Func<string, CancellationToken, EngineResult<RecordWireDraftNode>> valueFactory)
        : base(RecordWireDraftNodeKind.Nullable, path, displayName, descriptor, isRequired, isReadOnly, valueState)
    {
        ArgumentNullException.ThrowIfNull(valueFactory);
        WholeValueIsNull = isNull;
        CurrentValue = value;
        ValueFactory = valueFactory;
        if (CurrentValue is not null)
        {
            ObserveChild(CurrentValue);
        }
    }

    /// <summary>Gets whether the whole wrapper is currently represented by JSON null.</summary>
    public bool IsNull => WholeValueIsNull;

    /// <summary>Gets the retained or present non-null typed value.</summary>
    public RecordWireDraftNode? Value => CurrentValue;

    /// <inheritdoc />
    public override IReadOnlyList<RecordWireDraftNode> Children => !WholeValueIsNull && CurrentValue is not null ? new[] { CurrentValue } : Array.Empty<RecordWireDraftNode>();

    /// <summary>Sets the whole wrapper to JSON null without normalizing a retained nested FormLink identity.</summary>
    public void SetNull()
    {
        if (WholeValueIsNull)
        {
            return;
        }

        WholeValueIsNull = true;
        OnPropertyChanged(nameof(IsNull));
        MarkChanged(nameof(Children));
    }

    /// <summary>Sets the wrapper present, creating its non-null typed value only when needed.</summary>
    /// <param name="cancellationToken">A token observed during bounded value materialization.</param>
    /// <returns>The present typed value or a typed input failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public EngineResult<RecordWireDraftNode> SetPresent(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (CurrentValue is null)
        {
            var created = ValueFactory(Path, cancellationToken);
            if (!created.Succeeded || created.Value is null)
            {
                return created;
            }

            CurrentValue = created.Value;
            ObserveChild(CurrentValue);
            OnPropertyChanged(nameof(Value));
        }

        if (WholeValueIsNull)
        {
            WholeValueIsNull = false;
            OnPropertyChanged(nameof(IsNull));
            MarkChanged(nameof(Children));
        }

        return EngineResult<RecordWireDraftNode>.Success(CurrentValue);
    }

    /// <inheritdoc />
    protected override void RebaseChildren()
    {
        CurrentValue?.RebasePath(Path);
    }
}
