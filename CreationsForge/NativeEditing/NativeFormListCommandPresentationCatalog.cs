using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Services;

namespace CreationsForge.NativeEditing;

/// <summary>Provides stable presentation metadata for one exact native FormList edit command.</summary>
public sealed class NativeFormListCommandPresentation
{
    /// <summary>Initializes one immutable command presentation entry.</summary>
    /// <param name="key">The exact catalog-bound command node key.</param>
    /// <param name="displayName">The concise user-facing action name.</param>
    /// <param name="groupName">The stable editor group name.</param>
    /// <param name="description">The complete user-facing action description.</param>
    internal NativeFormListCommandPresentation(NativeWireSchemaNodeKey key, string displayName, string groupName, string description)
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
    public NativeWireSchemaNodeKey Key { get; }

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
public static class NativeFormListCommandPresentationCatalog
{
    /// <summary>Resolves presentation entries for every exact command node in catalog order.</summary>
    /// <param name="context">The exact resolved wire context.</param>
    /// <param name="cancellationToken">A token observed during catalog admission.</param>
    /// <returns>All exact command entries or a typed admission failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public static EngineResult<IReadOnlyList<NativeFormListCommandPresentation>> Resolve(NativeFormListWireCatalogContext context, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();
        var entries = context.SchemaCatalog.Nodes
            .Where(key => key.Kind == NativeWireSchemaNodeKind.Command)
            .Select(CreateEntry)
            .ToArray();
        cancellationToken.ThrowIfCancellationRequested();
        return EngineResult<IReadOnlyList<NativeFormListCommandPresentation>>.Success(Array.AsReadOnly(entries));
    }

    /// <summary>Creates one deterministic presentation entry from an admitted exact command.</summary>
    /// <param name="key">The exact command node key.</param>
    /// <returns>The immutable presentation entry.</returns>
    private static NativeFormListCommandPresentation CreateEntry(NativeWireSchemaNodeKey key)
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
        return new NativeFormListCommandPresentation(key, displayName, group, $"{displayName}. Changes remain in the workspace until saved.");
    }
}
