using System.Collections.ObjectModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents one lazily selected schema union.</summary>
public sealed class RecordWireUnionDraftNode : RecordWireDraftNode
{
    /// <summary>The complete lightweight option index.</summary>
    private readonly IReadOnlyList<RecordWireSchemaUnionOption> AllOptions;

    /// <summary>Materializes only the explicitly selected alternative.</summary>
    private readonly Func<string, RecordWireSchemaUnionOption, CancellationToken, EngineResult<RecordWireDraftNode>> OptionFactory;

    /// <summary>The selected lightweight option.</summary>
    private RecordWireSchemaUnionOption? CurrentOption;

    /// <summary>The selected materialized value.</summary>
    private RecordWireDraftNode? CurrentValue;

    /// <summary>Initializes one lazy union draft.</summary>
    /// <param name="path">The exact JSON path.</param>
    /// <param name="displayName">The user-facing field name.</param>
    /// <param name="descriptor">The resolved union schema.</param>
    /// <param name="isRequired">Whether the containing object requires the union.</param>
    /// <param name="isReadOnly">Whether the union is derived.</param>
    /// <param name="valueState">How the union was initialized.</param>
    /// <param name="options">The complete lightweight alternative index.</param>
    /// <param name="selectedOption">The initially selected option, when seeded or safely defaulted.</param>
    /// <param name="selectedValue">The initially materialized selected value.</param>
    /// <param name="optionFactory">The bounded lazy selected-option factory, supplied the wrapper's current path after structural edits.</param>
    internal RecordWireUnionDraftNode(
        string path,
        string displayName,
        RecordWireSchemaDescriptor descriptor,
        bool isRequired,
        bool isReadOnly,
        RecordWireDraftValueState valueState,
        IReadOnlyList<RecordWireSchemaUnionOption> options,
        RecordWireSchemaUnionOption? selectedOption,
        RecordWireDraftNode? selectedValue,
        Func<string, RecordWireSchemaUnionOption, CancellationToken, EngineResult<RecordWireDraftNode>> optionFactory)
        : base(RecordWireDraftNodeKind.Union, path, displayName, descriptor, isRequired, isReadOnly, valueState)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(optionFactory);
        AllOptions = Array.AsReadOnly(options.ToArray());
        CurrentOption = selectedOption;
        CurrentValue = selectedValue;
        OptionFactory = optionFactory;
        if (CurrentValue is not null)
        {
            ObserveChild(CurrentValue);
        }
    }

    /// <summary>Gets the complete lightweight option index without materializing option graphs.</summary>
    public IReadOnlyList<RecordWireSchemaUnionOption> Options => AllOptions;

    /// <summary>Gets the selected lightweight option, or <see langword="null"/> while explicitly unselected.</summary>
    public RecordWireSchemaUnionOption? SelectedOption => CurrentOption;

    /// <summary>Gets the selected materialized typed value, or <see langword="null"/> while explicitly unselected.</summary>
    public RecordWireDraftNode? SelectedValue => CurrentValue;

    /// <inheritdoc />
    public override IReadOnlyList<RecordWireDraftNode> Children => CurrentValue is null ? Array.Empty<RecordWireDraftNode>() : new[] { CurrentValue };

    /// <summary>Searches lightweight union identities without resolving their nested graphs.</summary>
    /// <param name="query">Optional ordinal-insensitive text matched against key, display name, and discriminator.</param>
    /// <param name="maximumResults">The positive maximum number of returned choices.</param>
    /// <returns>Matching alternatives in catalog order.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="maximumResults"/> is not positive.</exception>
    public IReadOnlyList<RecordWireSchemaUnionOption> SearchOptions(string? query, int maximumResults)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maximumResults);
        var normalized = query?.Trim();
        return Array.AsReadOnly(AllOptions
            .Where(option => string.IsNullOrEmpty(normalized) ||
                option.Key.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                option.DisplayName.Contains(normalized, StringComparison.OrdinalIgnoreCase) ||
                (option.Discriminator?.Contains(normalized, StringComparison.OrdinalIgnoreCase) ?? false))
            .Take(maximumResults)
            .ToArray());
    }

    /// <summary>Selects and materializes exactly one union alternative.</summary>
    /// <param name="optionKey">The exact stable alternative key.</param>
    /// <param name="cancellationToken">A token observed during lazy materialization.</param>
    /// <returns>The selected typed value or a typed input failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public EngineResult<RecordWireDraftNode> SelectOption(string optionKey, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(optionKey);
        cancellationToken.ThrowIfCancellationRequested();
        var option = AllOptions.SingleOrDefault(candidate => string.Equals(candidate.Key, optionKey, StringComparison.Ordinal));
        if (option is null)
        {
            return EngineResult<RecordWireDraftNode>.Failure(
                new EngineError(EngineErrorCode.InvalidRequest, $"{Path}: union option '{optionKey}' is not available in this exact catalog."));
        }

        var created = OptionFactory(Path, option, cancellationToken);
        if (!created.Succeeded || created.Value is null)
        {
            return created;
        }

        if (CurrentValue is not null)
        {
            StopObservingChild(CurrentValue);
        }

        CurrentOption = option;
        CurrentValue = created.Value;
        ObserveChild(CurrentValue);
        ValueState = RecordWireDraftValueState.Defaulted;
        OnPropertyChanged(nameof(SelectedOption));
        OnPropertyChanged(nameof(SelectedValue));
        MarkChanged(nameof(Children));
        return created;
    }

    /// <inheritdoc />
    protected override void RebaseChildren()
    {
        CurrentValue?.RebasePath(Path);
    }
}
