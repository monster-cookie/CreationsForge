using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Services;

namespace CreationsForge.RecordEditing;

/// <summary>Identifies one fixed presentation-owned command seed extraction policy.</summary>
public enum FormListCommandSeedPolicyKind
{
    /// <summary>The command arguments are an explicit empty object.</summary>
    EmptyArguments,
    /// <summary>One record property is copied to one command argument.</summary>
    DirectRecordProperty,
    /// <summary>The compressed Boolean is derived from the record raw major-record flag bit.</summary>
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
public sealed class FormListCommandSeedPolicy
{
    /// <summary>Initializes one immutable fixed command mapping.</summary>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <param name="kind">The closed extraction policy.</param>
    /// <param name="recordProperty">The fixed exact record property, when applicable.</param>
    /// <param name="argumentProperty">The fixed exact argument property, when applicable.</param>
    internal FormListCommandSeedPolicy(string commandName, FormListCommandSeedPolicyKind kind, string? recordProperty = null, string? argumentProperty = null)
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
    public FormListCommandSeedPolicyKind Kind { get; }

    /// <summary>Gets the fixed case-sensitive record property, when applicable.</summary>
    public string? RecordProperty { get; }

    /// <summary>Gets the fixed case-sensitive command argument property, when applicable.</summary>
    public string? ArgumentProperty { get; }
}

/// <summary>Admits the fixed seed mapping for every exact command in one catalog.</summary>
public static class FormListCommandSeedCatalog
{
    /// <summary>Resolves one command's fixed seed policy after exact catalog admission.</summary>
    /// <param name="context">The exact resolved wire context.</param>
    /// <param name="commandName">The exact stable command discriminator.</param>
    /// <returns>The fixed mapping or a typed failure for a foreign, unknown, or unsupported command.</returns>
    public static EngineResult<FormListCommandSeedPolicy> Resolve(FormListWireCatalogContext context, string commandName)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(commandName);
        var key = context.SchemaCatalog.Nodes.SingleOrDefault(candidate => candidate.Kind == RecordWireSchemaNodeKind.Command && string.Equals(candidate.Name, commandName, StringComparison.Ordinal));
        if (key is null)
        {
            return Failure($"Command '{commandName}' is not part of record wire catalog {context.Identity.CatalogId}.");
        }

        var policy = CreatePolicy(commandName);
        return policy is null
            ? Failure($"Command '{commandName}' has no admitted presentation seed mapping.")
            : EngineResult<FormListCommandSeedPolicy>.Success(policy);
    }

    /// <summary>Validates that every command has exactly one fixed mapping and that no mapping is foreign.</summary>
    /// <param name="context">The exact resolved wire context.</param>
    /// <returns>All policies in catalog order or a typed admission failure.</returns>
    public static EngineResult<IReadOnlyList<FormListCommandSeedPolicy>> ResolveAll(FormListWireCatalogContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        var policies = new List<FormListCommandSeedPolicy>();
        foreach (var key in context.SchemaCatalog.Nodes.Where(candidate => candidate.Kind == RecordWireSchemaNodeKind.Command))
        {
            var policy = CreatePolicy(key.Name);
            if (policy is null)
            {
                return Failure<IReadOnlyList<FormListCommandSeedPolicy>>($"Command '{key.Name}' has no admitted presentation seed mapping.");
            }

            policies.Add(policy);
        }

        return EngineResult<IReadOnlyList<FormListCommandSeedPolicy>>.Success(Array.AsReadOnly(policies.ToArray()));
    }

    /// <summary>Creates one policy from the closed 51-command discriminator set.</summary>
    /// <param name="commandName">The exact command discriminator.</param>
    /// <returns>The fixed policy, or <see langword="null"/> for a foreign command.</returns>
    private static FormListCommandSeedPolicy? CreatePolicy(string commandName)
    {
        if (commandName.EndsWith("form-list.clear-editor-id", StringComparison.Ordinal) ||
            commandName.EndsWith("form-list.clear-items", StringComparison.Ordinal) ||
            commandName.EndsWith("form-list.clear-name", StringComparison.Ordinal) ||
            commandName.EndsWith("form-list.clear-add-to-list", StringComparison.Ordinal) ||
            commandName.EndsWith("form-list.clear-conditional-entries", StringComparison.Ordinal))
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.EmptyArguments);
        }

        if (commandName.EndsWith("form-list.insert-item", StringComparison.Ordinal) || commandName == "starfield.form-list.add-component")
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.SelectedInsertionIndex);
        }

        if (commandName.EndsWith("form-list.move-item", StringComparison.Ordinal))
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.ExplicitMoveIndices);
        }

        if (commandName.EndsWith("form-list.remove-item", StringComparison.Ordinal) || commandName == "starfield.form-list.remove-component")
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.SelectedExistingIndex);
        }

        if (commandName.EndsWith("form-list.replace-items", StringComparison.Ordinal))
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.CompleteCollection, "Items", "items");
        }

        if (commandName.EndsWith("form-list.set-compressed", StringComparison.Ordinal))
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DerivedCompressed, "MajorRecordFlagsRaw", "isCompressed");
        }

        if (commandName.EndsWith("form-list.set-deleted", StringComparison.Ordinal))
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DerivedDeleted, null, "isDeleted");
        }

        if (commandName.EndsWith("form-list.set-editor-id", StringComparison.Ordinal))
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DirectRecordProperty, "EditorID", "editorId");
        }

        if (commandName.EndsWith("form-list.set-form-version", StringComparison.Ordinal))
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DirectRecordProperty, "FormVersion", "formVersion");
        }

        if (commandName.EndsWith("form-list.set-version-2", StringComparison.Ordinal))
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DirectRecordProperty, "Version2", "version2");
        }

        if (commandName.EndsWith("form-list.set-version-control", StringComparison.Ordinal))
        {
            return new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DirectRecordProperty, "VersionControl", "versionControl");
        }

        return commandName switch
        {
            "fallout4.form-list.set-major-record-flags" => new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DirectRecordProperty, "Fallout4MajorRecordFlags", "majorRecordFlags"),
            "fallout4.form-list.set-name" => new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DirectRecordProperty, "Name", "name"),
            "skyrim.form-list.set-major-record-flags" => new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DirectRecordProperty, "SkyrimMajorRecordFlags", "majorRecordFlags"),
            "starfield.form-list.replace-component" => new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.SelectedExistingComponent, "Components", "component"),
            "starfield.form-list.replace-components" => new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.CompleteCollection, "Components", "components"),
            "starfield.form-list.set-add-to-list" => new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DirectRecordProperty, "AddToList", "formList"),
            "starfield.form-list.set-conditional-entries" => new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.CompleteCollection, "ConditionalEntries", "conditionalEntries"),
            "starfield.form-list.set-major-flags" => new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DirectRecordProperty, "StarfieldMajorRecordFlags", "majorRecordFlags"),
            "starfield.form-list.set-name" => new FormListCommandSeedPolicy(commandName, FormListCommandSeedPolicyKind.DirectRecordProperty, "Name", "name"),
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
    private static EngineResult<FormListCommandSeedPolicy> Failure(string message)
    {
        return Failure<FormListCommandSeedPolicy>(message);
    }
}
