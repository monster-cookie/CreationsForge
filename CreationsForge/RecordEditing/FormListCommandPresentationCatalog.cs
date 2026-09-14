using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Services;

namespace CreationsForge.RecordEditing;

/// <summary>Provides stable presentation metadata for one exact FormList edit command.</summary>
public sealed class FormListCommandPresentation
{
    /// <summary>Initializes one immutable command presentation entry.</summary>
    /// <param name="key">The exact catalog-bound command node key.</param>
    /// <param name="displayName">The concise user-facing action name.</param>
    /// <param name="groupName">The stable editor group name.</param>
    /// <param name="description">The complete user-facing action description.</param>
    internal FormListCommandPresentation(RecordWireSchemaNodeKey key, string displayName, string groupName, string description)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentException.ThrowIfNullOrWhiteSpace(groupName);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        Key = key;
        DisplayName = displayName;
        GroupName = groupName;
        Description = description;
    }

    /// <summary>Gets the exact catalog-bound command node key.</summary>
    public RecordWireSchemaNodeKey Key { get; }

    /// <summary>Gets the exact stable command discriminator.</summary>
    public string CommandName => Key.Name;

    /// <summary>Gets the concise user-facing action name.</summary>
    public string DisplayName { get; }

    /// <summary>Gets the stable editor group name.</summary>
    public string GroupName { get; }

    /// <summary>Gets the complete user-facing action description.</summary>
    public string Description { get; }
}

/// <summary>Enumerates every exact command in a resolved catalog with complete stable presentation metadata.</summary>
public static class FormListCommandPresentationCatalog
{
    /// <summary>Resolves presentation entries for every exact command node in catalog order.</summary>
    /// <param name="context">The exact resolved wire context.</param>
    /// <param name="cancellationToken">A token observed during catalog admission.</param>
    /// <returns>All exact command entries or a typed admission failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public static EngineResult<IReadOnlyList<FormListCommandPresentation>> Resolve(FormListWireCatalogContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        var entries = context.SchemaCatalog.Nodes
            .Where(key => key.Kind == RecordWireSchemaNodeKind.Command)
            .Select(CreateEntry)
            .ToArray();
        cancellationToken.ThrowIfCancellationRequested();
        return EngineResult<IReadOnlyList<FormListCommandPresentation>>.Success(Array.AsReadOnly(entries));
    }

    /// <summary>Creates one deterministic presentation entry from an admitted exact command.</summary>
    /// <param name="key">The exact command node key.</param>
    /// <returns>The immutable presentation entry.</returns>
    private static FormListCommandPresentation CreateEntry(RecordWireSchemaNodeKey key)
    {
        var name = key.Name;
        var verbStart = name.LastIndexOf("form-list.", StringComparison.Ordinal) + "form-list.".Length;
        var verb = verbStart >= "form-list.".Length && verbStart < name.Length ? name[verbStart..] : name;
        var words = verb.Split('-', StringSplitOptions.RemoveEmptyEntries);
        var displayName = string.Join(' ', words.Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
        var group = verb.Contains("item", StringComparison.Ordinal) ? "Items" :
            verb.Contains("component", StringComparison.Ordinal) ? "Components" :
            verb.Contains("conditional", StringComparison.Ordinal) ? "Conditional Entries" :
            verb.Contains("name", StringComparison.Ordinal) || verb.Contains("editor-id", StringComparison.Ordinal) ? "Identity" :
            "Record";
        return new FormListCommandPresentation(key, displayName, group, $"{displayName}. Changes remain in the workspace until saved.");
    }
}
