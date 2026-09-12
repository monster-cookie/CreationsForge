using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Core.Enums;
using CreationsForge.Fallout4.Native.Edits;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Fallout4.Native.Wire;

/// <summary>Decodes the closed Fallout 4 FormList wire command catalog into the existing typed edit domain.</summary>
public sealed class Fallout4FormListEditWireCodec : IFormListEditWireCodec
{
    /// <summary>The shared command that clears an optional EditorID.</summary>
    internal const string ClearEditorIdCommand = "form-list.clear-editor-id";

    /// <summary>The shared command that clears every ordered FormList item.</summary>
    internal const string ClearItemsCommand = "form-list.clear-items";

    /// <summary>The shared command that inserts one native FormList link.</summary>
    internal const string InsertItemCommand = "form-list.insert-item";

    /// <summary>The shared command that moves one ordered FormList item.</summary>
    internal const string MoveItemCommand = "form-list.move-item";

    /// <summary>The shared command that removes one ordered FormList item.</summary>
    internal const string RemoveItemCommand = "form-list.remove-item";

    /// <summary>The shared command that replaces the complete ordered FormList link sequence.</summary>
    internal const string ReplaceItemsCommand = "form-list.replace-items";

    /// <summary>The shared command that sets native record compression state.</summary>
    internal const string SetCompressedCommand = "form-list.set-compressed";

    /// <summary>The shared command that sets native deletion state.</summary>
    internal const string SetDeletedCommand = "form-list.set-deleted";

    /// <summary>The shared command that sets a non-empty EditorID.</summary>
    internal const string SetEditorIdCommand = "form-list.set-editor-id";

    /// <summary>The shared command that sets the unsigned native form version.</summary>
    internal const string SetFormVersionCommand = "form-list.set-form-version";

    /// <summary>The shared command that sets the unsigned secondary version.</summary>
    internal const string SetVersion2Command = "form-list.set-version-2";

    /// <summary>The shared command that sets native version-control data.</summary>
    internal const string SetVersionControlCommand = "form-list.set-version-control";

    /// <summary>The Fallout 4 command that clears the optional translated name.</summary>
    internal const string ClearNameCommand = "fallout4.form-list.clear-name";

    /// <summary>The Fallout 4 command that replaces supported typed major-record flags.</summary>
    internal const string SetMajorRecordFlagsCommand = "fallout4.form-list.set-major-record-flags";

    /// <summary>The Fallout 4 command that sets the complete translated name.</summary>
    internal const string SetNameCommand = "fallout4.form-list.set-name";

    /// <summary>The exact supported bit mask in Mutagen.Bethesda.Fallout4 0.55.0-alpha.48.</summary>
    internal const int SupportedMajorRecordFlagMask =
        (int)(Fallout4MajorRecord.Fallout4MajorRecordFlag.ESM
            | Fallout4MajorRecord.Fallout4MajorRecordFlag.NotPlayable
            | Fallout4MajorRecord.Fallout4MajorRecordFlag.Deleted
            | Fallout4MajorRecord.Fallout4MajorRecordFlag.InitiallyDisabled
            | Fallout4MajorRecord.Fallout4MajorRecordFlag.Ignored
            | Fallout4MajorRecord.Fallout4MajorRecordFlag.VisibleWhenDistant
            | Fallout4MajorRecord.Fallout4MajorRecordFlag.Dangerous_OffLimits_InteriorCell
            | Fallout4MajorRecord.Fallout4MajorRecordFlag.Compressed
            | Fallout4MajorRecord.Fallout4MajorRecordFlag.CantWait);

    /// <summary>The closed no-argument command shape.</summary>
    private static readonly NativeWireObjectShape EmptyArgumentsShape = new(
        "empty FormList edit arguments",
        Array.Empty<string>());

    /// <summary>The closed EditorID setter shape.</summary>
    private static readonly NativeWireObjectShape EditorIdArgumentsShape = new(
        "set EditorID arguments",
        new[] { "editorId" });

    /// <summary>The closed version-control setter shape.</summary>
    private static readonly NativeWireObjectShape VersionControlArgumentsShape = new(
        "set version-control arguments",
        new[] { "versionControl" });

    /// <summary>The closed form-version setter shape.</summary>
    private static readonly NativeWireObjectShape FormVersionArgumentsShape = new(
        "set form-version arguments",
        new[] { "formVersion" });

    /// <summary>The closed secondary-version setter shape.</summary>
    private static readonly NativeWireObjectShape Version2ArgumentsShape = new(
        "set secondary-version arguments",
        new[] { "version2" });

    /// <summary>The closed compression setter shape.</summary>
    private static readonly NativeWireObjectShape CompressedArgumentsShape = new(
        "set compression arguments",
        new[] { "isCompressed" });

    /// <summary>The closed deletion setter shape.</summary>
    private static readonly NativeWireObjectShape DeletedArgumentsShape = new(
        "set deletion arguments",
        new[] { "isDeleted" });

    /// <summary>The closed complete item replacement shape.</summary>
    private static readonly NativeWireObjectShape ReplaceItemsArgumentsShape = new(
        "replace FormList items arguments",
        new[] { "items" });

    /// <summary>The closed single-item insertion shape.</summary>
    private static readonly NativeWireObjectShape InsertItemArgumentsShape = new(
        "insert FormList item arguments",
        new[] { "index", "item" });

    /// <summary>The closed single-item removal shape.</summary>
    private static readonly NativeWireObjectShape RemoveItemArgumentsShape = new(
        "remove FormList item arguments",
        new[] { "index" });

    /// <summary>The closed ordered item move shape.</summary>
    private static readonly NativeWireObjectShape MoveItemArgumentsShape = new(
        "move FormList item arguments",
        new[] { "sourceIndex", "destinationIndex" });

    /// <summary>The closed translated-name setter shape.</summary>
    private static readonly NativeWireObjectShape NameArgumentsShape = new(
        "set Fallout 4 FormList name arguments",
        new[] { "name" });

    /// <summary>The inspector-compatible translated-string shape.</summary>
    private static readonly NativeWireObjectShape TranslatedStringShape = new(
        "translated string",
        new[] { "targetLanguage", "value", "translations" });

    /// <summary>The inspector-compatible translated-string entry shape.</summary>
    private static readonly NativeWireObjectShape TranslationShape = new(
        "translated string entry",
        new[] { "language", "value" });

    /// <summary>The inspector-compatible native form-link shape.</summary>
    private static readonly NativeWireObjectShape FormLinkShape = new(
        "native form link",
        new[] { "isNull", "formKey" });

    /// <summary>The closed typed major-record flag setter shape.</summary>
    private static readonly NativeWireObjectShape MajorRecordFlagsArgumentsShape = new(
        "set Fallout 4 major-record flags arguments",
        new[] { "majorRecordFlags" });

    /// <summary>Gets the Fallout 4 game identity.</summary>
    public SupportedGame Game => SupportedGame.Fallout4;

    /// <summary>Gets the exact Fallout 4 native release.</summary>
    public GameRelease Release => GameRelease.Fallout4;

    /// <summary>Decodes one closed Fallout 4 FormList command without retaining caller-owned JSON.</summary>
    /// <param name="commandName">The exact stable command discriminator.</param>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The resource limits enforced before native collection allocation.</param>
    /// <param name="cancellationToken">A token observed throughout decoding and before typed edit construction.</param>
    /// <returns>A complete typed edit or a path-specific invalid-request failure.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public NativeWireDecodeResult<FormListEdit> Decode(
        string commandName,
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken = default)
    {
        return NativeWireReadContext.Decode(
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
        NativeWireReadContext context,
        NativeWireValue root,
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
            ClearNameCommand => DecodeEmpty(context, root, static () => new Fallout4ClearNameEdit()),
            SetMajorRecordFlagsCommand => DecodeSetMajorRecordFlags(context, root),
            _ => RejectUnknownCommand(context, root, commandName),
        };
    }

    /// <summary>Decodes a no-payload command after proving that the argument object is empty.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <param name="factory">The closed typed edit constructor.</param>
    /// <returns>The constructed no-payload edit.</returns>
    private static FormListEdit DecodeEmpty(
        NativeWireReadContext context,
        NativeWireValue root,
        Func<FormListEdit> factory)
    {
        context.ReadObject(root, EmptyArgumentsShape);
        return factory();
    }

    /// <summary>Decodes a non-empty EditorID without trimming or normalizing it.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The exact typed EditorID edit.</returns>
    private static FormListEdit DecodeSetEditorId(NativeWireReadContext context, NativeWireValue root)
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
    private static FormListEdit DecodeSetVersionControl(NativeWireReadContext context, NativeWireValue root)
    {
        var arguments = context.ReadObject(root, VersionControlArgumentsShape);
        return new SetVersionControlEdit(context.ReadUInt32(arguments.GetRequiredProperty("versionControl")));
    }

    /// <summary>Decodes an exact unsigned 16-bit form version.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed form-version edit.</returns>
    private static FormListEdit DecodeSetFormVersion(NativeWireReadContext context, NativeWireValue root)
    {
        var arguments = context.ReadObject(root, FormVersionArgumentsShape);
        return new SetFormVersionEdit(context.ReadUInt16(arguments.GetRequiredProperty("formVersion")));
    }

    /// <summary>Decodes an exact unsigned 16-bit secondary version.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed secondary-version edit.</returns>
    private static FormListEdit DecodeSetVersion2(NativeWireReadContext context, NativeWireValue root)
    {
        var arguments = context.ReadObject(root, Version2ArgumentsShape);
        return new SetVersion2Edit(context.ReadUInt16(arguments.GetRequiredProperty("version2")));
    }

    /// <summary>Decodes an exact native compression state.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed compression edit.</returns>
    private static FormListEdit DecodeSetCompressed(NativeWireReadContext context, NativeWireValue root)
    {
        var arguments = context.ReadObject(root, CompressedArgumentsShape);
        return new SetCompressedEdit(context.ReadBoolean(arguments.GetRequiredProperty("isCompressed")));
    }

    /// <summary>Decodes an exact native deletion state.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed deletion edit.</returns>
    private static FormListEdit DecodeSetDeleted(NativeWireReadContext context, NativeWireValue root)
    {
        var arguments = context.ReadObject(root, DeletedArgumentsShape);
        return new SetDeletedEdit(context.ReadBoolean(arguments.GetRequiredProperty("isDeleted")));
    }

    /// <summary>Decodes a complete ordered FormList link replacement, including native null links.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed ordered replacement edit.</returns>
    private static FormListEdit DecodeReplaceItems(NativeWireReadContext context, NativeWireValue root)
    {
        var arguments = context.ReadObject(root, ReplaceItemsArgumentsShape);
        return new ReplaceItemsEdit(ReadItems(context, arguments.GetRequiredProperty("items")));
    }

    /// <summary>Decodes one zero-based insertion position and exact native form link.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed insertion edit.</returns>
    private static FormListEdit DecodeInsertItem(NativeWireReadContext context, NativeWireValue root)
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
    private static FormListEdit DecodeRemoveItem(NativeWireReadContext context, NativeWireValue root)
    {
        var arguments = context.ReadObject(root, RemoveItemArgumentsShape);
        return new RemoveItemEdit(ReadNonNegativeIndex(context, arguments.GetRequiredProperty("index")));
    }

    /// <summary>Decodes exact source and final destination positions for one ordered move.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed move edit.</returns>
    private static FormListEdit DecodeMoveItem(NativeWireReadContext context, NativeWireValue root)
    {
        var arguments = context.ReadObject(root, MoveItemArgumentsShape);
        var sourceIndex = ReadNonNegativeIndex(context, arguments.GetRequiredProperty("sourceIndex"));
        var destinationIndex = ReadNonNegativeIndex(context, arguments.GetRequiredProperty("destinationIndex"));
        return new MoveItemEdit(sourceIndex, destinationIndex);
    }

    /// <summary>Decodes a complete inspector-compatible translated string without adding missing entries.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed Fallout 4 translated-name edit.</returns>
    private static FormListEdit DecodeSetName(NativeWireReadContext context, NativeWireValue root)
    {
        var arguments = context.ReadObject(root, NameArgumentsShape);
        return new Fallout4SetNameEdit(ReadTranslatedString(context, arguments.GetRequiredProperty("name")));
    }

    /// <summary>Decodes signed Int32 typed flags and rejects every bit absent from the installed native enum.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="root">The admitted root argument value.</param>
    /// <returns>The typed Fallout 4 major-record flag edit.</returns>
    private static FormListEdit DecodeSetMajorRecordFlags(NativeWireReadContext context, NativeWireValue root)
    {
        var arguments = context.ReadObject(root, MajorRecordFlagsArgumentsShape);
        var flagsValue = arguments.GetRequiredProperty("majorRecordFlags");
        var flags = context.ReadInt32(flagsValue);
        var unsupported = unchecked((uint)flags & ~(uint)SupportedMajorRecordFlagMask);
        if (unsupported != 0)
        {
            context.Reject(flagsValue, $"Fallout 4 major-record flags contain unsupported bits 0x{unsupported:X8}.");
        }

        return new Fallout4SetMajorRecordFlagsEdit(
            (Fallout4MajorRecord.Fallout4MajorRecordFlag)flags);
    }

    /// <summary>Reads a bounded ordered native form-link array without deduplication or null filtering.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="value">The admitted array value.</param>
    /// <returns>The exact ordered native FormKey sequence.</returns>
    private static IReadOnlyList<FormKey> ReadItems(NativeWireReadContext context, NativeWireValue value)
    {
        var array = context.ReadArray(value);
        var items = new FormKey[array.Count];
        for (var index = 0; index < array.Count; index++)
        {
            items[index] = ReadFormLink(context, array.GetElement(index));
        }

        return Array.AsReadOnly(items);
    }

    /// <summary>Reads the exact inspector form-link shape and preserves explicit native null-link identity.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="value">The admitted form-link object.</param>
    /// <returns>The non-null native FormKey or <see cref="FormKey.Null"/>.</returns>
    private static FormKey ReadFormLink(NativeWireReadContext context, NativeWireValue value)
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
                    "A native null form link requires JSON null or canonical FormKey.Null.");
            }

            return nullFormKey;
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
    /// <returns>A new exact native translated-string value.</returns>
    private static TranslatedString ReadTranslatedString(NativeWireReadContext context, NativeWireValue value)
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
    private static Language ReadLanguage(NativeWireReadContext context, NativeWireValue value)
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
    private static int ReadNonNegativeIndex(NativeWireReadContext context, NativeWireValue value)
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
        NativeWireReadContext context,
        NativeWireValue root,
        string? commandName)
    {
        var displayedName = commandName is null ? "<null>" : $"'{commandName}'";
        context.Reject(root, $"Fallout 4 does not support FormList wire command {displayedName}.");
        throw new InvalidOperationException("Native wire rejection returned unexpectedly.");
    }
}
