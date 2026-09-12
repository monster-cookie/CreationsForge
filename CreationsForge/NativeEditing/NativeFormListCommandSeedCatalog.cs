using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Services;

namespace CreationsForge.NativeEditing;

/// <summary>Identifies one fixed presentation-owned command seed extraction policy.</summary>
public enum NativeFormListCommandSeedPolicyKind
{
    /// <summary>The command arguments are an explicit empty object.</summary>
    EmptyArguments,
    /// <summary>One record property is copied to one command argument.</summary>
    DirectRecordProperty,
    /// <summary>The compressed Boolean is derived from the native raw major-record flag bit.</summary>
    DerivedCompressed,
    /// <summary>The deleted Boolean is derived from the exact selected record context.</summary>
    DerivedDeleted,
    /// <summary>A complete record collection is copied to one command argument.</summary>
    CompleteCollection,
    /// <summary>One selected existing position supplies an index argument.</summary>
    SelectedExistingIndex,
    /// <summary>One selected insertion position supplies an index while the new value remains explicit.</summary>
    SelectedInsertionIndex,
    /// <summary>Explicit source and destination positions supply a move command.</summary>
    ExplicitMoveIndices,
    /// <summary>One selected position and its complete existing component graph supply replacement arguments.</summary>
    SelectedExistingComponent,
}

/// <summary>Describes one admitted fixed seed mapping without caller-provided paths.</summary>
public sealed class NativeFormListCommandSeedPolicy
{
    /// <summary>Initializes one immutable fixed command mapping.</summary>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <param name="kind">The closed extraction policy.</param>
    /// <param name="recordProperty">The fixed exact record property, when applicable.</param>
    /// <param name="argumentProperty">The fixed exact argument property, when applicable.</param>
    internal NativeFormListCommandSeedPolicy(string commandName, NativeFormListCommandSeedPolicyKind kind, string? recordProperty = null, string? argumentProperty = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        CommandName = commandName;
        Kind = kind;
        RecordProperty = recordProperty;
        ArgumentProperty = argumentProperty;
    }

    /// <summary>Gets the exact command discriminator.</summary>
    public string CommandName { get; }

    /// <summary>Gets the closed extraction policy.</summary>
    public NativeFormListCommandSeedPolicyKind Kind { get; }

    /// <summary>Gets the fixed case-sensitive record property, when applicable.</summary>
    public string? RecordProperty { get; }

    /// <summary>Gets the fixed case-sensitive command argument property, when applicable.</summary>
    public string? ArgumentProperty { get; }
}

/// <summary>Admits the fixed seed mapping for every exact command in one catalog.</summary>
public static class NativeFormListCommandSeedCatalog
{
    /// <summary>Resolves one command's fixed seed policy after exact catalog admission.</summary>
    /// <param name="context">The exact resolved wire context.</param>
    /// <param name="commandName">The exact stable command discriminator.</param>
    /// <returns>The fixed mapping or a typed failure for a foreign, unknown, or unsupported command.</returns>
    public static EngineResult<NativeFormListCommandSeedPolicy> Resolve(NativeFormListWireCatalogContext context, string commandName)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        var key = context.SchemaCatalog.Nodes.SingleOrDefault(candidate => candidate.Kind == NativeWireSchemaNodeKind.Command && string.Equals(candidate.Name, commandName, StringComparison.Ordinal));
        if (key is null)
        {
            return Failure($"Command '{commandName}' is not part of native wire catalog {context.Identity.CatalogId}.");
        }

        var policy = CreatePolicy(commandName);
        return policy is null
            ? Failure($"Command '{commandName}' has no admitted presentation seed mapping.")
            : EngineResult<NativeFormListCommandSeedPolicy>.Success(policy);
    }

    /// <summary>Validates that every command has exactly one fixed mapping and that no mapping is foreign.</summary>
    /// <param name="context">The exact resolved wire context.</param>
    /// <returns>All policies in catalog order or a typed admission failure.</returns>
    public static EngineResult<IReadOnlyList<NativeFormListCommandSeedPolicy>> ResolveAll(NativeFormListWireCatalogContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var policies = new List<NativeFormListCommandSeedPolicy>();
        foreach (var key in context.SchemaCatalog.Nodes.Where(candidate => candidate.Kind == NativeWireSchemaNodeKind.Command))
        {
            var policy = CreatePolicy(key.Name);
            if (policy is null)
            {
                return Failure<IReadOnlyList<NativeFormListCommandSeedPolicy>>($"Command '{key.Name}' has no admitted presentation seed mapping.");
            }

            policies.Add(policy);
        }

        return EngineResult<IReadOnlyList<NativeFormListCommandSeedPolicy>>.Success(Array.AsReadOnly(policies.ToArray()));
    }

    /// <summary>Creates one policy from the closed 51-command discriminator set.</summary>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <returns>The fixed policy, or <see langword="null"/> for a foreign command.</returns>
    private static NativeFormListCommandSeedPolicy? CreatePolicy(string commandName)
    {
        if (commandName.EndsWith("form-list.clear-editor-id", StringComparison.Ordinal) ||
            commandName.EndsWith("form-list.clear-items", StringComparison.Ordinal) ||
            commandName.EndsWith("form-list.clear-name", StringComparison.Ordinal) ||
            commandName.EndsWith("form-list.clear-add-to-list", StringComparison.Ordinal) ||
            commandName.EndsWith("form-list.clear-conditional-entries", StringComparison.Ordinal))
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.EmptyArguments);
        }

        if (commandName.EndsWith("form-list.insert-item", StringComparison.Ordinal) || commandName == "starfield.form-list.add-component")
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.SelectedInsertionIndex);
        }

        if (commandName.EndsWith("form-list.move-item", StringComparison.Ordinal))
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.ExplicitMoveIndices);
        }

        if (commandName.EndsWith("form-list.remove-item", StringComparison.Ordinal) || commandName == "starfield.form-list.remove-component")
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.SelectedExistingIndex);
        }

        if (commandName.EndsWith("form-list.replace-items", StringComparison.Ordinal))
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.CompleteCollection, "Items", "items");
        }

        if (commandName.EndsWith("form-list.set-compressed", StringComparison.Ordinal))
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DerivedCompressed, "MajorRecordFlagsRaw", "isCompressed");
        }

        if (commandName.EndsWith("form-list.set-deleted", StringComparison.Ordinal))
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DerivedDeleted, null, "isDeleted");
        }

        if (commandName.EndsWith("form-list.set-editor-id", StringComparison.Ordinal))
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DirectRecordProperty, "EditorID", "editorId");
        }

        if (commandName.EndsWith("form-list.set-form-version", StringComparison.Ordinal))
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DirectRecordProperty, "FormVersion", "formVersion");
        }

        if (commandName.EndsWith("form-list.set-version-2", StringComparison.Ordinal))
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DirectRecordProperty, "Version2", "version2");
        }

        if (commandName.EndsWith("form-list.set-version-control", StringComparison.Ordinal))
        {
            return new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DirectRecordProperty, "VersionControl", "versionControl");
        }

        return commandName switch
        {
            "fallout4.form-list.set-major-record-flags" => new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DirectRecordProperty, "Fallout4MajorRecordFlags", "majorRecordFlags"),
            "fallout4.form-list.set-name" => new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DirectRecordProperty, "Name", "name"),
            "skyrim.form-list.set-major-record-flags" => new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DirectRecordProperty, "SkyrimMajorRecordFlags", "majorRecordFlags"),
            "starfield.form-list.replace-component" => new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.SelectedExistingComponent, "Components", "component"),
            "starfield.form-list.replace-components" => new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.CompleteCollection, "Components", "components"),
            "starfield.form-list.set-add-to-list" => new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DirectRecordProperty, "AddToList", "formList"),
            "starfield.form-list.set-conditional-entries" => new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.CompleteCollection, "ConditionalEntries", "conditionalEntries"),
            "starfield.form-list.set-major-flags" => new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DirectRecordProperty, "StarfieldMajorRecordFlags", "majorRecordFlags"),
            "starfield.form-list.set-name" => new NativeFormListCommandSeedPolicy(commandName, NativeFormListCommandSeedPolicyKind.DirectRecordProperty, "Name", "name"),
            _ => null,
        };
    }

    /// <summary>Creates one typed seed-catalog admission failure.</summary>
    /// <param name="message">The complete diagnostic message.</param>
    /// <returns>A failed immutable engine result.</returns>
    private static EngineResult<T> Failure<T>(string message)
    {
        return EngineResult<T>.Failure(new EngineError(EngineErrorCode.ValidationFailed, message));
    }

    /// <summary>Creates one typed single-policy admission failure.</summary>
    /// <param name="message">The complete diagnostic message.</param>
    /// <returns>A failed immutable engine result.</returns>
    private static EngineResult<NativeFormListCommandSeedPolicy> Failure(string message)
    {
        return Failure<NativeFormListCommandSeedPolicy>(message);
    }
}
