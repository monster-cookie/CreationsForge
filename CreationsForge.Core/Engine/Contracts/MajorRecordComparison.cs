using System.Text.Json;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Carries native before-and-after field trees, exact contexts, and typed semantic change paths.</summary>
public sealed class MajorRecordComparison
{
    /// <summary>Initializes an immutable major-record comparison.</summary>
    /// <param name="beforeContext">The exact prior context selection and outcome.</param>
    /// <param name="afterContext">The exact resulting context selection and outcome.</param>
    /// <param name="recordType">The common registered record family, when known.</param>
    /// <param name="before">The detached prior field tree, or <see langword="null"/> when absent.</param>
    /// <param name="after">The detached resulting field tree, or <see langword="null"/> when absent.</param>
    /// <param name="changes">Native-value semantic change descriptors.</param>
    /// <param name="warnings">Read and validation warnings from both contexts.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required context or collection is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when the contexts identify different records or a JSON value is undefined.</exception>
    public MajorRecordComparison(
        FormListContext beforeContext,
        FormListContext afterContext,
        string? recordType,
        JsonElement? before,
        JsonElement? after,
        IReadOnlyList<SemanticChangeDescriptor> changes,
        IReadOnlyList<EngineWarning> warnings)
    {
        ArgumentNullException.ThrowIfNull(beforeContext);
        ArgumentNullException.ThrowIfNull(afterContext);
        ArgumentNullException.ThrowIfNull(changes);
        ArgumentNullException.ThrowIfNull(warnings);
        if (beforeContext.Selection.FormKey != afterContext.Selection.FormKey)
        {
            throw new ArgumentException("A major-record comparison requires contexts for the same FormKey.", nameof(afterContext));
        }

        if (before is { ValueKind: JsonValueKind.Undefined } || after is { ValueKind: JsonValueKind.Undefined })
        {
            throw new ArgumentException("A major-record comparison cannot retain an undefined JSON value.");
        }

        BeforeContext = beforeContext;
        AfterContext = afterContext;
        RecordType = recordType;
        Before = before?.Clone();
        After = after?.Clone();
        Changes = Array.AsReadOnly(changes.ToArray());
        Warnings = Array.AsReadOnly(warnings.ToArray());
    }

    /// <summary>Gets the compared record identity.</summary>
    public FormKey FormKey => BeforeContext.Selection.FormKey;

    /// <summary>Gets the prior context selection and outcome.</summary>
    public FormListContext BeforeContext { get; }

    /// <summary>Gets the resulting context selection and outcome.</summary>
    public FormListContext AfterContext { get; }

    /// <summary>Gets the common registered record family, or <see langword="null"/> when both sides are absent.</summary>
    public string? RecordType { get; }

    /// <summary>Gets the complete detached prior field tree, or <see langword="null"/> when absent.</summary>
    public JsonElement? Before { get; }

    /// <summary>Gets the complete detached resulting field tree, or <see langword="null"/> when absent.</summary>
    public JsonElement? After { get; }

    /// <summary>Gets semantic changes derived from native typed values.</summary>
    public IReadOnlyList<SemanticChangeDescriptor> Changes { get; }

    /// <summary>Gets immutable read and validation warnings.</summary>
    public IReadOnlyList<EngineWarning> Warnings { get; }
}
