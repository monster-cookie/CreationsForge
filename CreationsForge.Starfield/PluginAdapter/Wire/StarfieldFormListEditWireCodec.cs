using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Core.Enums;
using CreationsForge.Starfield.PluginAdapter.Edits;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;
using Noggog;

namespace CreationsForge.Starfield.PluginAdapter.Wire;

/// <summary>Decodes the closed Starfield FormList wire command catalog into the existing typed edit domain.</summary>
public sealed class StarfieldFormListEditWireCodec : IFormListEditWireCodec
{
    /// <summary>The shared command that clears an optional EditorID.</summary>
    internal const string ClearEditorIdCommand = "form-list.clear-editor-id";

    /// <summary>The shared command that clears every ordered FormList item.</summary>
    internal const string ClearItemsCommand = "form-list.clear-items";

    /// <summary>The shared command that inserts one FormList link.</summary>
    internal const string InsertItemCommand = "form-list.insert-item";

    /// <summary>The shared command that moves one ordered FormList item.</summary>
    internal const string MoveItemCommand = "form-list.move-item";

    /// <summary>The shared command that removes one ordered FormList item.</summary>
    internal const string RemoveItemCommand = "form-list.remove-item";

    /// <summary>The shared command that replaces the complete ordered FormList link sequence.</summary>
    internal const string ReplaceItemsCommand = "form-list.replace-items";

    /// <summary>The shared command that sets record compression state.</summary>
    internal const string SetCompressedCommand = "form-list.set-compressed";

    /// <summary>The shared command that sets Mutagen deletion state.</summary>
    internal const string SetDeletedCommand = "form-list.set-deleted";

    /// <summary>The shared command that sets a non-empty EditorID.</summary>
    internal const string SetEditorIdCommand = "form-list.set-editor-id";

    /// <summary>The shared command that sets the unsigned Mutagen form version.</summary>
    internal const string SetFormVersionCommand = "form-list.set-form-version";

    /// <summary>The shared command that sets the unsigned secondary version.</summary>
    internal const string SetVersion2Command = "form-list.set-version-2";

    /// <summary>The shared command that sets Mutagen version-control data.</summary>
    internal const string SetVersionControlCommand = "form-list.set-version-control";

    /// <summary>The Starfield command that clears the optional translated name.</summary>
    internal const string ClearNameCommand = "starfield.form-list.clear-name";

    /// <summary>The Starfield command that clears the optional AddToList link.</summary>
    internal const string ClearAddToListCommand = "starfield.form-list.clear-add-to-list";

    /// <summary>The Starfield command that replaces supported typed major-record flags.</summary>
    internal const string SetMajorFlagsCommand = "starfield.form-list.set-major-flags";

    /// <summary>The Starfield command that sets the complete translated name.</summary>
    internal const string SetNameCommand = "starfield.form-list.set-name";

    /// <summary>The Starfield command that sets the optional AddToList link to a non-null FormList identity.</summary>
    internal const string SetAddToListCommand = "starfield.form-list.set-add-to-list";

    /// <summary>The Starfield command that inserts one complete Mutagen component.</summary>
    internal const string AddComponentCommand = "starfield.form-list.add-component";

    /// <summary>The Starfield command that clears every conditional entry.</summary>
    internal const string ClearConditionalEntriesCommand = "starfield.form-list.clear-conditional-entries";

    /// <summary>The Starfield command that removes one Mutagen component.</summary>
    internal const string RemoveComponentCommand = "starfield.form-list.remove-component";

    /// <summary>The Starfield command that replaces one Mutagen component.</summary>
    internal const string ReplaceComponentCommand = "starfield.form-list.replace-component";

    /// <summary>The Starfield command that replaces the complete ordered component collection.</summary>
    internal const string ReplaceComponentsCommand = "starfield.form-list.replace-components";

    /// <summary>The Starfield command that replaces every ordered conditional entry.</summary>
    internal const string SetConditionalEntriesCommand = "starfield.form-list.set-conditional-entries";

    /// <summary>The exact supported bit mask in Mutagen.Bethesda.Starfield 0.55.0-alpha.53.</summary>
    internal static readonly int SupportedMajorRecordFlagMask = unchecked((int)Enum
        .GetValues<StarfieldMajorRecord.StarfieldMajorRecordFlag>()
        .Aggregate(
            (StarfieldMajorRecord.StarfieldMajorRecordFlag)0,
            static (mask, flag) => mask | flag));

    /// <summary>The closed no-argument command shape.</summary>
    private static readonly RecordWireObjectShape EmptyArgumentsShape = new(
        "empty FormList edit arguments",
        Array.Empty<string>());

    /// <summary>The closed EditorID setter shape.</summary>
    private static readonly RecordWireObjectShape EditorIdArgumentsShape = new(
        "set EditorID arguments",
        new[] { "editorId" });

    /// <summary>The closed version-control setter shape.</summary>
    private static readonly RecordWireObjectShape VersionControlArgumentsShape = new(
        "set version-control arguments",
        new[] { "versionControl" });

    /// <summary>The closed form-version setter shape.</summary>
    private static readonly RecordWireObjectShape FormVersionArgumentsShape = new(
        "set form-version arguments",
        new[] { "formVersion" });

    /// <summary>The closed secondary-version setter shape.</summary>
    private static readonly RecordWireObjectShape Version2ArgumentsShape = new(
        "set secondary-version arguments",
        new[] { "version2" });

    /// <summary>The closed compression setter shape.</summary>
    private static readonly RecordWireObjectShape CompressedArgumentsShape = new(
        "set compression arguments",
        new[] { "isCompressed" });

    /// <summary>The closed deletion setter shape.</summary>
    private static readonly RecordWireObjectShape DeletedArgumentsShape = new(
        "set deletion arguments",
        new[] { "isDeleted" });

    /// <summary>The closed complete item replacement shape.</summary>
    private static readonly RecordWireObjectShape ReplaceItemsArgumentsShape = new(
        "replace FormList items arguments",
        new[] { "items" });

    /// <summary>The closed single-item insertion shape.</summary>
    private static readonly RecordWireObjectShape InsertItemArgumentsShape = new(
        "insert FormList item arguments",
        new[] { "index", "item" });

    /// <summary>The closed single-item removal shape.</summary>
    private static readonly RecordWireObjectShape RemoveItemArgumentsShape = new(
        "remove FormList item arguments",
        new[] { "index" });

    /// <summary>The closed ordered item move shape.</summary>
    private static readonly RecordWireObjectShape MoveItemArgumentsShape = new(
        "move FormList item arguments",
        new[] { "sourceIndex", "destinationIndex" });

    /// <summary>The closed translated-name setter shape.</summary>
    private static readonly RecordWireObjectShape NameArgumentsShape = new(
        "set Starfield FormList name arguments",
        new[] { "name" });

    /// <summary>The closed AddToList setter shape.</summary>
    private static readonly RecordWireObjectShape AddToListArgumentsShape = new(
        "set Starfield AddToList arguments",
        new[] { "formList" });

    /// <summary>The closed indexed component insertion or replacement shape.</summary>
    private static readonly RecordWireObjectShape IndexedComponentArgumentsShape = new(
        "indexed Starfield component arguments",
        new[] { "index", "component" });

    /// <summary>The closed component-index shape.</summary>
    private static readonly RecordWireObjectShape ComponentIndexArgumentsShape = new(
        "Starfield component index arguments",
        new[] { "index" });

    /// <summary>The closed complete component replacement shape.</summary>
    private static readonly RecordWireObjectShape ComponentsArgumentsShape = new(
        "replace Starfield components arguments",
        new[] { "components" });

    /// <summary>The closed complete conditional-entry replacement shape.</summary>
    private static readonly RecordWireObjectShape ConditionalEntriesArgumentsShape = new(
        "set Starfield conditional entries arguments",
        new[] { "conditionalEntries" });

    /// <summary>The exact inspector-compatible conditional-entry shape.</summary>
    private static readonly RecordWireObjectShape ConditionalEntryShape = new(
        "Starfield conditional entry",
        new[] { "Index", "Conditions" });

    /// <summary>The inspector-compatible translated-string shape.</summary>
    private static readonly RecordWireObjectShape TranslatedStringShape = new(
        "translated string",
        new[] { "targetLanguage", "value", "translations" });

    /// <summary>The inspector-compatible translated-string entry shape.</summary>
    private static readonly RecordWireObjectShape TranslationShape = new(
        "translated string entry",
        new[] { "language", "value" });

    /// <summary>The inspector-compatible Mutagen form-link shape.</summary>
    private static readonly RecordWireObjectShape FormLinkShape = new(
        "plugin form link",
        new[] { "isNull", "formKey" });

    /// <summary>The closed typed major-record flag setter shape.</summary>
    private static readonly RecordWireObjectShape MajorRecordFlagsArgumentsShape = new(
        "set Starfield major-record flags arguments",
        new[] { "majorRecordFlags" });

    /// <summary>Gets the Starfield game identity.</summary>
    public SupportedGame Game => SupportedGame.Starfield;

    /// <summary>Gets the exact Starfield Mutagen release.</summary>
    public GameRelease Release => GameRelease.Starfield;

    /// <summary>Decodes one closed Starfield FormList command without retaining caller-owned JSON.</summary>
    /// <param name="commandName">The exact stable command discriminator.</param>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The resource limits enforced before Mutagen collection allocation.</param>
    /// <param name="cancellationToken">A token observed throughout decoding and before typed edit construction.</param>
    /// <returns>A complete typed edit or a path-specific invalid-request failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public RecordWireDecodeResult<FormListEdit> Decode(
        string commandName,
        JsonElement arguments,
        RecordWireReadLimits limits,
        CancellationToken cancellationToken = default)
    {
        return RecordWireReadContext.Decode(
            arguments,
            limits,
            cancellationToken,
            (context, root) => DecodeCore(context, root, commandName));
    }

    /// <summary>Dispatches one exact command discriminator after bounded root admission.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <param name="commandName">The caller-supplied exact discriminator.</param>
    /// <returns>The completely decoded typed edit.</returns>
    private static FormListEdit DecodeCore(
        RecordWireReadContext context,
        RecordWireValue root,
        string commandName)
    {
        return commandName switch
        {
            SetEditorIdCommand => DecodeSetEditorId(context, root),
            ClearEditorIdCommand => DecodeEmpty(context, root, static () => new ClearEditorIdEdit()),
            SetVersionControlCommand => DecodeSetVersionControl(context, root),
            SetFormVersionCommand => DecodeSetFormVersion(context, root),
            SetVersion2Command => DecodeSetVersion2(context, root),
            SetCompressedCommand => DecodeSetCompressed(context, root),
            SetDeletedCommand => DecodeSetDeleted(context, root),
            ReplaceItemsCommand => DecodeReplaceItems(context, root),
            InsertItemCommand => DecodeInsertItem(context, root),
            RemoveItemCommand => DecodeRemoveItem(context, root),
            MoveItemCommand => DecodeMoveItem(context, root),
            ClearItemsCommand => DecodeEmpty(context, root, static () => new ClearItemsEdit()),
            SetNameCommand => DecodeSetName(context, root),
            ClearNameCommand => DecodeEmpty(context, root, static () => new StarfieldClearNameEdit()),
            SetMajorFlagsCommand => DecodeSetMajorFlags(context, root),
            SetAddToListCommand => DecodeSetAddToList(context, root),
            ClearAddToListCommand => DecodeEmpty(context, root, static () => new StarfieldClearAddToListEdit()),
            AddComponentCommand => DecodeAddComponent(context, root),
            ReplaceComponentCommand => DecodeReplaceComponent(context, root),
            RemoveComponentCommand => DecodeRemoveComponent(context, root),
            ReplaceComponentsCommand => DecodeReplaceComponents(context, root),
            SetConditionalEntriesCommand => DecodeSetConditionalEntries(context, root),
            ClearConditionalEntriesCommand => DecodeEmpty(context, root, static () => new StarfieldClearConditionalEntriesEdit()),
            _ => RejectUnknownCommand(context, root, commandName),
        };
    }

    /// <summary>Decodes a no-payload command after proving that the argument object is empty.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <param name="factory">The closed typed edit constructor.</param>
    /// <returns>The constructed no-payload edit.</returns>
    private static FormListEdit DecodeEmpty(
        RecordWireReadContext context,
        RecordWireValue root,
        Func<FormListEdit> factory)
    {
        context.ReadObject(root, EmptyArgumentsShape);
        return factory();
    }

    /// <summary>Decodes a non-empty EditorID without trimming or normalizing it.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The exact typed EditorID edit.</returns>
    private static FormListEdit DecodeSetEditorId(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, EditorIdArgumentsShape);
        var editorIdValue = arguments.GetRequiredProperty("editorId");
        var editorId = context.ReadString(editorIdValue);
        if (string.IsNullOrWhiteSpace(editorId))
        {
            context.Reject(editorIdValue, "EditorID cannot be empty or whitespace.");
        }

        return new SetEditorIdEdit(editorId);
    }

    /// <summary>Decodes an exact unsigned 32-bit version-control value.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed version-control edit.</returns>
    private static FormListEdit DecodeSetVersionControl(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, VersionControlArgumentsShape);
        return new SetVersionControlEdit(context.ReadUInt32(arguments.GetRequiredProperty("versionControl")));
    }

    /// <summary>Decodes an exact unsigned 16-bit form version.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed form-version edit.</returns>
    private static FormListEdit DecodeSetFormVersion(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, FormVersionArgumentsShape);
        return new SetFormVersionEdit(context.ReadUInt16(arguments.GetRequiredProperty("formVersion")));
    }

    /// <summary>Decodes an exact unsigned 16-bit secondary version.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed secondary-version edit.</returns>
    private static FormListEdit DecodeSetVersion2(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, Version2ArgumentsShape);
        return new SetVersion2Edit(context.ReadUInt16(arguments.GetRequiredProperty("version2")));
    }

    /// <summary>Decodes an exact Mutagen compression state.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed compression edit.</returns>
    private static FormListEdit DecodeSetCompressed(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, CompressedArgumentsShape);
        return new SetCompressedEdit(context.ReadBoolean(arguments.GetRequiredProperty("isCompressed")));
    }

    /// <summary>Decodes an exact Mutagen deletion state.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed deletion edit.</returns>
    private static FormListEdit DecodeSetDeleted(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, DeletedArgumentsShape);
        return new SetDeletedEdit(context.ReadBoolean(arguments.GetRequiredProperty("isDeleted")));
    }

    /// <summary>Decodes a complete ordered FormList link replacement, including Mutagen null links.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed ordered replacement edit.</returns>
    private static FormListEdit DecodeReplaceItems(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, ReplaceItemsArgumentsShape);
        return new ReplaceItemsEdit(ReadItems(context, arguments.GetRequiredProperty("items")));
    }

    /// <summary>Decodes one zero-based insertion position and exact plugin form link.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed insertion edit.</returns>
    private static FormListEdit DecodeInsertItem(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, InsertItemArgumentsShape);
        var indexValue = arguments.GetRequiredProperty("index");
        var index = ReadNonNegativeIndex(context, indexValue);
        var item = ReadFormLink(context, arguments.GetRequiredProperty("item"));
        return new InsertItemEdit(index, item);
    }

    /// <summary>Decodes one zero-based removal position.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed removal edit.</returns>
    private static FormListEdit DecodeRemoveItem(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, RemoveItemArgumentsShape);
        return new RemoveItemEdit(ReadNonNegativeIndex(context, arguments.GetRequiredProperty("index")));
    }

    /// <summary>Decodes exact source and final destination positions for one ordered move.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed move edit.</returns>
    private static FormListEdit DecodeMoveItem(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, MoveItemArgumentsShape);
        var sourceIndex = ReadNonNegativeIndex(context, arguments.GetRequiredProperty("sourceIndex"));
        var destinationIndex = ReadNonNegativeIndex(context, arguments.GetRequiredProperty("destinationIndex"));
        return new MoveItemEdit(sourceIndex, destinationIndex);
    }

    /// <summary>Decodes a complete inspector-compatible translated string without adding missing entries.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed Starfield translated-name edit.</returns>
    private static FormListEdit DecodeSetName(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, NameArgumentsShape);
        return new StarfieldSetNameEdit(ReadTranslatedString(context, arguments.GetRequiredProperty("name")));
    }

    /// <summary>Decodes signed Int32 typed flags and rejects every bit absent from the installed Mutagen enum.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed Starfield major-record flag edit.</returns>
    private static FormListEdit DecodeSetMajorFlags(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, MajorRecordFlagsArgumentsShape);
        var flagsValue = arguments.GetRequiredProperty("majorRecordFlags");
        var flags = context.ReadInt32(flagsValue);
        var unsupported = unchecked((uint)flags & ~(uint)SupportedMajorRecordFlagMask);
        if (unsupported != 0)
        {
            context.Reject(flagsValue, $"Starfield major-record flags contain unsupported bits 0x{unsupported:X8}.");
        }

        return new StarfieldSetMajorFlagsEdit(
            (StarfieldMajorRecord.StarfieldMajorRecordFlag)flags);
    }

    /// <summary>Decodes the exact inspector-compatible AddToList link while requiring non-null identity.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed Starfield AddToList edit.</returns>
    private static FormListEdit DecodeSetAddToList(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, AddToListArgumentsShape);
        var formListValue = arguments.GetRequiredProperty("formList");
        var formList = ReadFormLink(context, formListValue);
        if (formList.IsNull)
        {
            context.Reject(formListValue, "AddToList requires a non-null FormList identity; use the clear command to remove the link.");
        }

        return new StarfieldSetAddToListEdit(formList);
    }

    /// <summary>Decodes one exact insertion position and complete generated component graph.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed component insertion edit.</returns>
    private static FormListEdit DecodeAddComponent(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, IndexedComponentArgumentsShape);
        var index = ReadNonNegativeIndex(context, arguments.GetRequiredProperty("index"));
        var component = StarfieldNestedFieldCodec.ReadComponent(
            context,
            arguments.GetRequiredProperty("component"));
        return new StarfieldAddComponentEdit(index, component);
    }

    /// <summary>Decodes one exact existing position and complete generated component graph.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed component replacement edit.</returns>
    private static FormListEdit DecodeReplaceComponent(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, IndexedComponentArgumentsShape);
        var index = ReadNonNegativeIndex(context, arguments.GetRequiredProperty("index"));
        var component = StarfieldNestedFieldCodec.ReadComponent(
            context,
            arguments.GetRequiredProperty("component"));
        return new StarfieldReplaceComponentEdit(index, component);
    }

    /// <summary>Decodes one exact existing component position.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed component removal edit.</returns>
    private static FormListEdit DecodeRemoveComponent(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, ComponentIndexArgumentsShape);
        return new StarfieldRemoveComponentEdit(
            ReadNonNegativeIndex(context, arguments.GetRequiredProperty("index")));
    }

    /// <summary>Decodes the complete ordered generated component collection without deduplication.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed complete component replacement edit.</returns>
    private static FormListEdit DecodeReplaceComponents(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, ComponentsArgumentsShape);
        var values = context.ReadArray(arguments.GetRequiredProperty("components"));
        var components = new AComponent[values.Count];
        for (var index = 0; index < values.Count; index++)
        {
            components[index] = StarfieldNestedFieldCodec.ReadComponent(context, values.GetElement(index));
        }

        return new StarfieldReplaceComponentsEdit(Array.AsReadOnly(components));
    }

    /// <summary>Decodes every ordered conditional entry while preserving nullable indices and condition collections.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed complete conditional-entry replacement edit.</returns>
    private static FormListEdit DecodeSetConditionalEntries(RecordWireReadContext context, RecordWireValue root)
    {
        var arguments = context.ReadObject(root, ConditionalEntriesArgumentsShape);
        var values = context.ReadArray(arguments.GetRequiredProperty("conditionalEntries"));
        var entries = new FormListConditionalEntry[values.Count];
        for (var index = 0; index < values.Count; index++)
        {
            var entry = context.ReadObject(values.GetElement(index), ConditionalEntryShape);
            var indexValue = entry.GetRequiredProperty("Index");
            var conditionsValue = entry.GetRequiredProperty("Conditions");
            entries[index] = new FormListConditionalEntry
            {
                Index = context.IsNull(indexValue) ? null : context.ReadUInt32(indexValue),
                Conditions = ReadConditions(context, conditionsValue),
            };
        }

        return new StarfieldSetConditionalEntriesEdit(Array.AsReadOnly(entries));
    }

    /// <summary>Reads one nullable ordered condition collection through generated closed readers.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="value">The admitted nullable condition collection.</param>
    /// <returns>A new ordered condition list, or <see langword="null"/> when explicitly absent.</returns>
    private static ExtendedList<Condition>? ReadConditions(
        RecordWireReadContext context,
        RecordWireValue value)
    {
        if (context.IsNull(value))
        {
            return null;
        }

        var values = context.ReadArray(value);
        var conditions = new ExtendedList<Condition>();
        for (var index = 0; index < values.Count; index++)
        {
            conditions.Add(StarfieldNestedFieldCodec.ReadCondition(context, values.GetElement(index)));
        }

        return conditions;
    }

    /// <summary>Reads a bounded ordered Mutagen form-link array without deduplication or null filtering.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="value">The admitted array value.</param>
    /// <returns>The exact ordered Mutagen FormKey sequence.</returns>
    private static IReadOnlyList<FormKey> ReadItems(RecordWireReadContext context, RecordWireValue value)
    {
        var array = context.ReadArray(value);
        var items = new FormKey[array.Count];
        for (var index = 0; index < array.Count; index++)
        {
            items[index] = ReadFormLink(context, array.GetElement(index));
        }

        return Array.AsReadOnly(items);
    }

    /// <summary>Reads the exact inspector form-link shape and preserves explicit Mutagen null-link identity.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="value">The admitted form-link object.</param>
    /// <returns>The non-null Mutagen FormKey or <see cref="FormKey.Null"/>.</returns>
    private static FormKey ReadFormLink(RecordWireReadContext context, RecordWireValue value)
    {
        var link = context.ReadObject(value, FormLinkShape);
        var isNullValue = link.GetRequiredProperty("isNull");
        var formKeyValue = link.GetRequiredProperty("formKey");
        var isNull = context.ReadBoolean(isNullValue);
        if (isNull)
        {
            if (context.IsNull(formKeyValue))
            {
                return FormKey.Null;
            }

            var nullFormKey = context.ReadFormKey(formKeyValue);
            if (!nullFormKey.IsNull)
            {
                context.Reject(
                    formKeyValue,
                    "A Mutagen null form link requires JSON null or the canonical FormKey.Null identity.");
            }

            return FormKey.Null;
        }

        if (context.IsNull(formKeyValue))
        {
            context.Reject(formKeyValue, "A non-null form link requires a canonical FormKey.");
        }

        var formKey = context.ReadFormKey(formKeyValue);
        if (formKey.IsNull)
        {
            context.Reject(formKeyValue, "A non-null form link cannot contain FormKey.Null.");
        }

        return formKey;
    }

    /// <summary>Reads a complete translated-string map and verifies its redundant target-language value.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="value">The admitted translated-string object.</param>
    /// <returns>A new exact record translated-string value.</returns>
    private static TranslatedString ReadTranslatedString(RecordWireReadContext context, RecordWireValue value)
    {
        var translatedString = context.ReadObject(value, TranslatedStringShape);
        var targetLanguage = ReadLanguage(
            context,
            translatedString.GetRequiredProperty("targetLanguage"));
        var declaredValueNode = translatedString.GetRequiredProperty("value");
        var declaredValue = context.IsNull(declaredValueNode)
            ? null
            : context.ReadString(declaredValueNode);
        var translationsNode = translatedString.GetRequiredProperty("translations");
        var translationsArray = context.ReadArray(translationsNode);
        var translations = new KeyValuePair<Language, string>[translationsArray.Count];
        var seenLanguages = new HashSet<Language>();
        for (var index = 0; index < translationsArray.Count; index++)
        {
            var entryNode = translationsArray.GetElement(index);
            var entry = context.ReadObject(entryNode, TranslationShape);
            var languageNode = entry.GetRequiredProperty("language");
            var language = ReadLanguage(context, languageNode);
            if (!seenLanguages.Add(language))
            {
                context.Reject(languageNode, $"Translated-string language '{language}' occurs more than once.");
            }

            translations[index] = new KeyValuePair<Language, string>(
                language,
                context.ReadString(entry.GetRequiredProperty("value")));
        }

        var result = new TranslatedString(targetLanguage, translations);
        if (!string.Equals(result.String, declaredValue, StringComparison.Ordinal))
        {
            context.Reject(
                declaredValueNode,
                "Translated-string value does not match the target-language translation.");
        }

        return result;
    }

    /// <summary>Reads one exact installed Mutagen language name without numeric or case-insensitive coercion.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="value">The admitted language-name string.</param>
    /// <returns>The exact installed language value.</returns>
    private static Language ReadLanguage(RecordWireReadContext context, RecordWireValue value)
    {
        var text = context.ReadString(value);
        if (!Enum.TryParse<Language>(text, ignoreCase: false, out var language)
            || !Enum.IsDefined(language)
            || !string.Equals(language.ToString(), text, StringComparison.Ordinal))
        {
            context.Reject(value, "Expected an exact installed Mutagen Language name.");
        }

        return language;
    }

    /// <summary>Reads one Int32 index and rejects negative positions before typed edit construction.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="value">The admitted index value.</param>
    /// <returns>The non-negative zero-based position.</returns>
    private static int ReadNonNegativeIndex(RecordWireReadContext context, RecordWireValue value)
    {
        var index = context.ReadInt32(value);
        if (index < 0)
        {
            context.Reject(value, "A FormList item index cannot be negative.");
        }

        return index;
    }

    /// <summary>Rejects an unknown or missing command discriminator at the request root.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <param name="commandName">The unsupported discriminator, which may be <see langword="null"/>.</param>
    /// <returns>This method never returns.</returns>
    private static FormListEdit RejectUnknownCommand(
        RecordWireReadContext context,
        RecordWireValue root,
        string? commandName)
    {
        var displayedName = commandName is null ? "<null>" : $"'{commandName}'";
        context.Reject(root, $"Starfield does not support FormList wire command {displayedName}.");
        throw new InvalidOperationException("Plugin wire rejection returned unexpectedly.");
    }
}
