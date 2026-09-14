using System.Text.Json;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Carries detached typed before-and-after views, exact record contexts, and ephemeral semantic change locations.
/// </summary>
public sealed class FormListComparison
{
    /// <summary>Initializes an immutable FormList comparison.</summary>
    /// <param name="formKey">The FormList identity.</param>
    /// <param name="beforeContext">The exact prior context selection and outcome.</param>
    /// <param name="afterContext">The exact resulting context selection and outcome.</param>
    /// <param name="before">The detached typed prior JSON view, or <see langword="null"/> when no inspectable context exists.</param>
    /// <param name="after">The detached typed resulting JSON view, or <see langword="null"/> when no inspectable context exists.</param>
    /// <param name="changes">The semantic change descriptors.</param>
    /// <param name="warnings">Plugin writer-normalization and validation warnings.</param>
    /// <exception cref="ArgumentNullException">Thrown when a context, <paramref name="changes"/>, or <paramref name="warnings"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when contexts identify different records or a supplied JSON value is undefined.</exception>
    public FormListComparison(
        FormListContext beforeContext,
        FormListContext afterContext,
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
            throw new ArgumentException("A FormList comparison requires contexts for the same FormKey.", nameof(afterContext));
        }

        if (before is { ValueKind: JsonValueKind.Undefined })
        {
            throw new ArgumentException("A FormList comparison cannot retain an undefined prior JSON value.", nameof(before));
        }

        if (after is { ValueKind: JsonValueKind.Undefined })
        {
            throw new ArgumentException("A FormList comparison cannot retain an undefined resulting JSON value.", nameof(after));
        }

        BeforeContext = beforeContext;
        AfterContext = afterContext;
        Before = before?.Clone();
        After = after?.Clone();
        Changes = Array.AsReadOnly(changes.ToArray());
        Warnings = Array.AsReadOnly(warnings.ToArray());
    }

    /// <summary>Gets the FormList identity.</summary>
    public Mutagen.Bethesda.Plugins.FormKey FormKey => BeforeContext.Selection.FormKey;

    /// <summary>Gets the exact prior record context selection and outcome.</summary>
    public FormListContext BeforeContext { get; }

    /// <summary>Gets the exact resulting record context selection and outcome.</summary>
    public FormListContext AfterContext { get; }

    /// <summary>Gets the detached typed prior JSON view, or <see langword="null"/> when no inspectable context exists.</summary>
    public JsonElement? Before { get; }

    /// <summary>Gets the detached typed resulting JSON view, or <see langword="null"/> when no inspectable context exists.</summary>
    public JsonElement? After { get; }

    /// <summary>Gets the immutable semantic change descriptors.</summary>
    public IReadOnlyList<SemanticChangeDescriptor> Changes { get; }

    /// <summary>Gets immutable comparison and writer-normalization warnings.</summary>
    public IReadOnlyList<EngineWarning> Warnings { get; }
}
