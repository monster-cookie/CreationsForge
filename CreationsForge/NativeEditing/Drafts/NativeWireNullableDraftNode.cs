using System.Collections.ObjectModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents a value whose whole JSON identity may be null.</summary>
public sealed class NativeWireNullableDraftNode : NativeWireDraftNode
{
    /// <summary>Creates the present non-null value on explicit request.</summary>
    private readonly Func<string, CancellationToken, EngineResult<NativeWireDraftNode>> ValueFactory;

    /// <summary>The current present value, retained while the nullable wrapper is null.</summary>
    private NativeWireDraftNode? CurrentValue;

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
    internal NativeWireNullableDraftNode(
        string path,
        string displayName,
        NativeWireSchemaDescriptor descriptor,
        bool isRequired,
        bool isReadOnly,
        NativeWireDraftValueState valueState,
        bool isNull,
        NativeWireDraftNode? value,
        Func<string, CancellationToken, EngineResult<NativeWireDraftNode>> valueFactory)
        : base(NativeWireDraftNodeKind.Nullable, path, displayName, descriptor, isRequired, isReadOnly, valueState)
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
    public NativeWireDraftNode? Value => CurrentValue;

    /// <inheritdoc />
    public override IReadOnlyList<NativeWireDraftNode> Children => !WholeValueIsNull && CurrentValue is not null ? new[] { CurrentValue } : Array.Empty<NativeWireDraftNode>();

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
    public EngineResult<NativeWireDraftNode> SetPresent(CancellationToken cancellationToken = default)
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

        return EngineResult<NativeWireDraftNode>.Success(CurrentValue);
    }

    /// <inheritdoc />
    protected override void RebaseChildren()
    {
        CurrentValue?.RebasePath(Path);
    }
}
