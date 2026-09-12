using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.Native.Wire;

/// <summary>Decodes the complete closed Skyrim Special Edition FormList edit command surface into typed engine commands.</summary>
public sealed class SkyrimFormListEditWireCodec : IFormListEditWireCodec
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

    /// <summary>The Skyrim command that replaces supported typed major-record flags.</summary>
    internal const string SetMajorRecordFlagsCommand = "skyrim.form-list.set-major-record-flags";

    /// <summary>All Skyrim major-record bits exposed by the installed typed native enum.</summary>
    private const SkyrimMajorRecord.SkyrimMajorRecordFlag SupportedMajorRecordFlags =
        SkyrimMajorRecord.SkyrimMajorRecordFlag.ESM
        | SkyrimMajorRecord.SkyrimMajorRecordFlag.NotPlayable
        | SkyrimMajorRecord.SkyrimMajorRecordFlag.Deleted
        | SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled
        | SkyrimMajorRecord.SkyrimMajorRecordFlag.Ignored
        | SkyrimMajorRecord.SkyrimMajorRecordFlag.VisibleWhenDistant
        | SkyrimMajorRecord.SkyrimMajorRecordFlag.Dangerous_OffLimits_InteriorCell
        | SkyrimMajorRecord.SkyrimMajorRecordFlag.Compressed
        | SkyrimMajorRecord.SkyrimMajorRecordFlag.CantWait;

    /// <summary>The exact argument shape for commands without payload fields.</summary>
    private static readonly NativeWireObjectShape EmptyArgumentsShape = new(
        "empty command arguments",
        []);

    /// <summary>The exact argument shape for setting an EditorID.</summary>
    private static readonly NativeWireObjectShape EditorIdArgumentsShape = new(
        "set-editor-id arguments",
        ["editorId"]);

    /// <summary>The exact argument shape for replacing every ordered FormList item.</summary>
    private static readonly NativeWireObjectShape ItemsArgumentsShape = new(
        "replace-items arguments",
        ["items"]);

    /// <summary>The exact argument shape for inserting one ordered FormList item.</summary>
    private static readonly NativeWireObjectShape InsertArgumentsShape = new(
        "insert-item arguments",
        ["index", "item"]);

    /// <summary>The exact argument shape for one source-list index.</summary>
    private static readonly NativeWireObjectShape IndexArgumentsShape = new(
        "item-index arguments",
        ["index"]);

    /// <summary>The exact argument shape for moving one ordered FormList item.</summary>
    private static readonly NativeWireObjectShape MoveArgumentsShape = new(
        "move-item arguments",
        ["sourceIndex", "destinationIndex"]);

    /// <summary>The exact argument shape for setting the version-control field.</summary>
    private static readonly NativeWireObjectShape VersionControlArgumentsShape = new(
        "set-version-control arguments",
        ["versionControl"]);

    /// <summary>The exact argument shape for setting the form-version field.</summary>
    private static readonly NativeWireObjectShape FormVersionArgumentsShape = new(
        "set-form-version arguments",
        ["formVersion"]);

    /// <summary>The exact argument shape for setting the secondary-version field.</summary>
    private static readonly NativeWireObjectShape Version2ArgumentsShape = new(
        "set-version-2 arguments",
        ["version2"]);

    /// <summary>The exact argument shape for setting native compression state.</summary>
    private static readonly NativeWireObjectShape CompressedArgumentsShape = new(
        "set-compressed arguments",
        ["isCompressed"]);

    /// <summary>The exact argument shape for setting native deletion state.</summary>
    private static readonly NativeWireObjectShape DeletedArgumentsShape = new(
        "set-deleted arguments",
        ["isDeleted"]);

    /// <summary>The exact argument shape for setting Skyrim typed major-record flags.</summary>
    private static readonly NativeWireObjectShape MajorRecordFlagsArgumentsShape = new(
        "set-major-record-flags arguments",
        ["majorRecordFlags"]);

    /// <summary>The exact readable native FormLink shape used by Skyrim FormList items.</summary>
    private static readonly NativeWireObjectShape FormLinkShape = new(
        "native FormLink",
        ["isNull", "formKey"]);

    /// <summary>Initializes the stateless Skyrim Special Edition FormList wire codec.</summary>
    public SkyrimFormListEditWireCodec()
    { }

    /// <summary>Gets the supported CreationsForge game.</summary>
    public SupportedGame Game => SupportedGame.Skyrim;

    /// <summary>Gets the exact Skyrim Special Edition native release.</summary>
    public GameRelease Release => GameRelease.SkyrimSE;

    /// <summary>Decodes one closed command payload without retaining caller-owned JSON.</summary>
    /// <param name="commandName">The exact stable FormList command discriminator.</param>
    /// <param name="arguments">The request-local closed command arguments.</param>
    /// <param name="limits">The per-operation resource limits applied before collection allocation.</param>
    /// <param name="cancellationToken">A token observed throughout traversal and before typed construction.</param>
    /// <returns>A complete typed edit or a path-specific invalid-request failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="limits"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public NativeWireDecodeResult<FormListEdit> Decode(
        string commandName,
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(limits);
        cancellationToken.ThrowIfCancellationRequested();
        if (string.IsNullOrWhiteSpace(commandName))
        {
            return InvalidCommand("A Skyrim FormList wire command name is required.");
        }

        return commandName switch
        {
            SetEditorIdCommand => DecodeSetEditorId(arguments, limits, cancellationToken),
            ClearEditorIdCommand => DecodeEmpty(arguments, limits, cancellationToken, static () => new ClearEditorIdEdit()),
            ReplaceItemsCommand => DecodeReplaceItems(arguments, limits, cancellationToken),
            InsertItemCommand => DecodeInsertItem(arguments, limits, cancellationToken),
            RemoveItemCommand => DecodeIndex(arguments, limits, cancellationToken, static index => new RemoveItemEdit(index)),
            ClearItemsCommand => DecodeEmpty(arguments, limits, cancellationToken, static () => new ClearItemsEdit()),
            MoveItemCommand => DecodeMoveItem(arguments, limits, cancellationToken),
            SetVersionControlCommand => DecodeVersionControl(arguments, limits, cancellationToken),
            SetFormVersionCommand => DecodeFormVersion(arguments, limits, cancellationToken),
            SetVersion2Command => DecodeVersion2(arguments, limits, cancellationToken),
            SetCompressedCommand => DecodeBoolean(arguments, limits, cancellationToken, CompressedArgumentsShape, "isCompressed", static value => new SetCompressedEdit(value)),
            SetDeletedCommand => DecodeBoolean(arguments, limits, cancellationToken, DeletedArgumentsShape, "isDeleted", static value => new SetDeletedEdit(value)),
            SetMajorRecordFlagsCommand => DecodeMajorRecordFlags(arguments, limits, cancellationToken),
            _ => InvalidCommand("Skyrim Special Edition does not support the requested FormList wire command.")
        };
    }

    /// <summary>Decodes and validates an EditorID assignment.</summary>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The bounded traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <returns>A complete EditorID edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeSetEditorId(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            static (context, root) =>
            {
                var container = context.ReadObject(root, EditorIdArgumentsShape);
                var editorIdValue = container.GetRequiredProperty("editorId");
                var editorId = context.ReadString(editorIdValue);
                if (string.IsNullOrWhiteSpace(editorId))
                {
                    context.Reject(editorIdValue, "EditorID must contain at least one non-whitespace character.");
                }

                return new SetEditorIdEdit(editorId);
            });
    }

    /// <summary>Decodes a command whose argument object must be empty.</summary>
    /// <param name="arguments">The request-local argument object.</param>
    /// <param name="limits">The bounded traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <param name="create">The existing typed parameterless command constructor.</param>
    /// <returns>A complete empty-payload edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeEmpty(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken,
        Func<FormListEdit> create)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            (context, root) =>
            {
                context.ReadObject(root, EmptyArgumentsShape);
                return create();
            });
    }

    /// <summary>Decodes a complete ordered item replacement, retaining duplicates and explicit null links.</summary>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The bounded traversal limits applied before item allocation.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <returns>A complete ordered replacement edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeReplaceItems(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            static (context, root) =>
            {
                var container = context.ReadObject(root, ItemsArgumentsShape);
                var itemsValue = container.GetRequiredProperty("items");
                var items = context.ReadArray(itemsValue);
                var decodedItems = new FormKey[items.Count];
                for (var index = 0; index < items.Count; index++)
                {
                    decodedItems[index] = DecodeFormLink(context, items.GetElement(index));
                }

                return new ReplaceItemsEdit(decodedItems);
            });
    }

    /// <summary>Decodes an exact insertion index and one explicit native FormLink.</summary>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The bounded traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <returns>A complete insertion edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeInsertItem(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            static (context, root) =>
            {
                var container = context.ReadObject(root, InsertArgumentsShape);
                var indexValue = container.GetRequiredProperty("index");
                var index = DecodeNonNegativeIndex(context, indexValue);
                var item = DecodeFormLink(context, container.GetRequiredProperty("item"));
                return new InsertItemEdit(index, item);
            });
    }

    /// <summary>Decodes one command with a non-negative source-list index.</summary>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The bounded traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <param name="create">The existing typed indexed command constructor.</param>
    /// <returns>A complete indexed edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeIndex(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken,
        Func<int, FormListEdit> create)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            (context, root) =>
            {
                var container = context.ReadObject(root, IndexArgumentsShape);
                var index = DecodeNonNegativeIndex(context, container.GetRequiredProperty("index"));
                return create(index);
            });
    }

    /// <summary>Decodes an exact non-negative source index and final destination index.</summary>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The bounded traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <returns>A complete ordered move edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeMoveItem(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            static (context, root) =>
            {
                var container = context.ReadObject(root, MoveArgumentsShape);
                var sourceIndex = DecodeNonNegativeIndex(context, container.GetRequiredProperty("sourceIndex"));
                var destinationIndex = DecodeNonNegativeIndex(context, container.GetRequiredProperty("destinationIndex"));
                return new MoveItemEdit(sourceIndex, destinationIndex);
            });
    }

    /// <summary>Decodes an exact unsigned native version-control value.</summary>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The bounded traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <returns>A complete version-control edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeVersionControl(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            static (context, root) =>
            {
                var container = context.ReadObject(root, VersionControlArgumentsShape);
                return new SetVersionControlEdit(context.ReadUInt32(container.GetRequiredProperty("versionControl")));
            });
    }

    /// <summary>Decodes an exact unsigned native form-version value.</summary>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The bounded traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <returns>A complete form-version edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeFormVersion(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            static (context, root) =>
            {
                var container = context.ReadObject(root, FormVersionArgumentsShape);
                return new SetFormVersionEdit(context.ReadUInt16(container.GetRequiredProperty("formVersion")));
            });
    }

    /// <summary>Decodes an exact unsigned native secondary-version value.</summary>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The bounded traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <returns>A complete secondary-version edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeVersion2(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            static (context, root) =>
            {
                var container = context.ReadObject(root, Version2ArgumentsShape);
                return new SetVersion2Edit(context.ReadUInt16(container.GetRequiredProperty("version2")));
            });
    }

    /// <summary>Decodes one Boolean native header command.</summary>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The bounded traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <param name="shape">The exact accepted Boolean argument shape.</param>
    /// <param name="propertyName">The exact Boolean property to read.</param>
    /// <param name="create">The existing typed Boolean command constructor.</param>
    /// <returns>A complete Boolean header edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeBoolean(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken,
        NativeWireObjectShape shape,
        string propertyName,
        Func<bool, FormListEdit> create)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            (context, root) =>
            {
                var container = context.ReadObject(root, shape);
                return create(context.ReadBoolean(container.GetRequiredProperty(propertyName)));
            });
    }

    /// <summary>Decodes a supported combination of the installed signed Skyrim major-record flag enum.</summary>
    /// <param name="arguments">The request-local closed argument object.</param>
    /// <param name="limits">The bounded traversal limits.</param>
    /// <param name="cancellationToken">A token observed throughout decoding.</param>
    /// <returns>A complete typed flag edit or a path-specific invalid-request failure.</returns>
    private static NativeWireDecodeResult<FormListEdit> DecodeMajorRecordFlags(
        JsonElement arguments,
        NativeWireReadLimits limits,
        CancellationToken cancellationToken)
    {
        return NativeWireReadContext.Decode<FormListEdit>(
            arguments,
            limits,
            cancellationToken,
            static (context, root) =>
            {
                var container = context.ReadObject(root, MajorRecordFlagsArgumentsShape);
                var flagsValue = container.GetRequiredProperty("majorRecordFlags");
                var flags = (SkyrimMajorRecord.SkyrimMajorRecordFlag)context.ReadInt32(flagsValue);
                var unsupportedBits = flags & ~SupportedMajorRecordFlags;
                if (unsupportedBits != default)
                {
                    context.Reject(
                        flagsValue,
                        $"Skyrim major-record flags contain unsupported bits 0x{unchecked((uint)(int)unsupportedBits):X8}.");
                }

                return new SkyrimSetMajorRecordFlagsEdit(flags);
            });
    }

    /// <summary>Decodes one non-negative zero-based list index before constructing its typed command.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="value">The admitted index value.</param>
    /// <returns>The exact non-negative signed 32-bit index.</returns>
    private static int DecodeNonNegativeIndex(NativeWireReadContext context, NativeWireValue value)
    {
        var index = context.ReadInt32(value);
        if (index < 0)
        {
            context.Reject(value, "Item index must be non-negative.");
        }

        return index;
    }

    /// <summary>Decodes the explicit null state and canonical identity of one native FormLink.</summary>
    /// <param name="context">The request-local bounded read context.</param>
    /// <param name="value">The admitted closed FormLink object.</param>
    /// <returns>The exact non-null FormKey or <see cref="FormKey.Null"/> for an explicit null link.</returns>
    private static FormKey DecodeFormLink(NativeWireReadContext context, NativeWireValue value)
    {
        var container = context.ReadObject(value, FormLinkShape);
        var isNullValue = container.GetRequiredProperty("isNull");
        var formKeyValue = container.GetRequiredProperty("formKey");
        var isNull = context.ReadBoolean(isNullValue);
        var hasNullFormKey = context.IsNull(formKeyValue);
        if (isNull)
        {
            if (hasNullFormKey)
            {
                return FormKey.Null;
            }

            var explicitNullFormKey = context.ReadFormKey(formKeyValue);
            if (!explicitNullFormKey.IsNull)
            {
                context.Reject(
                    formKeyValue,
                    "A null native FormLink requires JSON null or the canonical FormKey.Null identity.");
            }

            return FormKey.Null;
        }

        if (hasNullFormKey)
        {
            context.Reject(formKeyValue, "A non-null native FormLink requires a canonical formKey.");
        }

        var formKey = context.ReadFormKey(formKeyValue);
        if (formKey.IsNull)
        {
            context.Reject(formKeyValue, "A non-null native FormLink cannot use FormKey.Null.");
        }

        return formKey;
    }

    /// <summary>Creates a stable typed failure for an absent or unsupported command discriminator.</summary>
    /// <param name="message">The non-empty invalid-command diagnostic.</param>
    /// <returns>An invalid-request result without a typed edit.</returns>
    private static NativeWireDecodeResult<FormListEdit> InvalidCommand(string message)
    {
        return NativeWireDecodeResult<FormListEdit>.Failure(
            new EngineError(EngineErrorCode.InvalidRequest, message));
    }
}
