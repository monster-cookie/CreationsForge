using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using CreationsForge.Skyrim.Native.NativeInspection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.Native;

/// <summary>Prepares, applies, and previews bounded typed Skyrim FormList edits without writing output artifacts.</summary>
public sealed partial class SkyrimNativeEditService
{
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

    /// <summary>The shared stateless Skyrim field writer and semantic comparer.</summary>
    private readonly SkyrimFormListNativeInspector _inspector;

    /// <summary>Initializes a Skyrim native edit service with the adapter's complete FormList inspector.</summary>
    /// <param name="inspector">The stateless typed Skyrim field writer and comparer.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inspector"/> is <see langword="null"/>.</exception>
    public SkyrimNativeEditService(SkyrimFormListNativeInspector inspector)
    {
        ArgumentNullException.ThrowIfNull(inspector);
        _inspector = inspector;
    }

    /// <summary>Defensively copies and canonically fingerprints one typed FormList command.</summary>
    /// <param name="edit">The caller-owned typed command to prepare.</param>
    /// <returns>An immutable Skyrim-owned payload and complete canonical identity.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="edit"/> is <see langword="null"/>.</exception>
    public PreparedFormListEdit PrepareEdit(FormListEdit edit)
    {
        ArgumentNullException.ThrowIfNull(edit);

        switch (edit)
        {
            case SetEditorIdEdit setEditorId:
            {
                var editorId = new string(setEditorId.EditorId.AsSpan());
                var fingerprint = NativeEditFingerprintFactory.Create(
                    edit.CommandName,
                    (writer, context) =>
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("editorId");
                        NativeJsonLeafWriter.WriteString(writer, editorId, context);
                        writer.WriteEndObject();
                    });
                return new SkyrimPreparedFormListEdit(
                    SkyrimPreparedEditKind.SetEditorId,
                    fingerprint,
                    editorId: editorId);
            }
            case ClearEditorIdEdit:
                return PrepareEmpty(edit.CommandName, SkyrimPreparedEditKind.ClearEditorId);
            case ReplaceItemsEdit replaceItems:
            {
                var items = replaceItems.Items.ToArray();
                var fingerprint = NativeEditFingerprintFactory.Create(
                    edit.CommandName,
                    (writer, context) =>
                    {
                        writer.WriteStartObject();
                        writer.WriteStartArray("items");
                        foreach (var item in items)
                        {
                            NativeJsonLeafWriter.WriteFormKey(writer, item, context);
                        }

                        writer.WriteEndArray();
                        writer.WriteEndObject();
                    });
                return new SkyrimPreparedFormListEdit(
                    SkyrimPreparedEditKind.ReplaceItems,
                    fingerprint,
                    items: items);
            }
            case InsertItemEdit insertItem:
            {
                var fingerprint = NativeEditFingerprintFactory.Create(
                    edit.CommandName,
                    (writer, context) =>
                    {
                        writer.WriteStartObject();
                        writer.WriteNumber("index", insertItem.Index);
                        writer.WritePropertyName("item");
                        NativeJsonLeafWriter.WriteFormKey(writer, insertItem.Item, context);
                        writer.WriteEndObject();
                    });
                return new SkyrimPreparedFormListEdit(
                    SkyrimPreparedEditKind.InsertItem,
                    fingerprint,
                    index: insertItem.Index,
                    item: insertItem.Item);
            }
            case RemoveItemEdit removeItem:
                return PrepareIndex(edit.CommandName, SkyrimPreparedEditKind.RemoveItem, removeItem.Index);
            case ClearItemsEdit:
                return PrepareEmpty(edit.CommandName, SkyrimPreparedEditKind.ClearItems);
            case MoveItemEdit moveItem:
            {
                var fingerprint = NativeEditFingerprintFactory.Create(
                    edit.CommandName,
                    (writer, _) =>
                    {
                        writer.WriteStartObject();
                        writer.WriteNumber("sourceIndex", moveItem.SourceIndex);
                        writer.WriteNumber("destinationIndex", moveItem.DestinationIndex);
                        writer.WriteEndObject();
                    });
                return new SkyrimPreparedFormListEdit(
                    SkyrimPreparedEditKind.MoveItem,
                    fingerprint,
                    index: moveItem.SourceIndex,
                    destinationIndex: moveItem.DestinationIndex);
            }
            case SetVersionControlEdit setVersionControl:
                return PrepareUnsigned(
                    edit.CommandName,
                    SkyrimPreparedEditKind.SetVersionControl,
                    setVersionControl.VersionControl);
            case SetFormVersionEdit setFormVersion:
                return PrepareUnsigned(
                    edit.CommandName,
                    SkyrimPreparedEditKind.SetFormVersion,
                    setFormVersion.FormVersion);
            case SetVersion2Edit setVersion2:
                return PrepareUnsigned(
                    edit.CommandName,
                    SkyrimPreparedEditKind.SetVersion2,
                    setVersion2.Version2);
            case SetCompressedEdit setCompressed:
                return PrepareBoolean(
                    edit.CommandName,
                    SkyrimPreparedEditKind.SetCompressed,
                    setCompressed.IsCompressed);
            case SetDeletedEdit setDeleted:
                return PrepareBoolean(
                    edit.CommandName,
                    SkyrimPreparedEditKind.SetDeleted,
                    setDeleted.IsDeleted);
            case SkyrimSetMajorRecordFlagsEdit setMajorRecordFlags:
            {
                var flags = setMajorRecordFlags.MajorRecordFlags;
                var fingerprint = NativeEditFingerprintFactory.Create(
                    edit.CommandName,
                    (writer, _) =>
                    {
                        writer.WriteStartObject();
                        writer.WriteNumber("majorRecordFlags", unchecked((uint)(int)flags));
                        writer.WriteEndObject();
                    });
                var unsupportedBits = flags & ~SupportedMajorRecordFlags;
                var error = unsupportedBits == default
                    ? null
                    : new EngineError(
                        EngineErrorCode.ValidationFailed,
                        $"Skyrim major-record flags contain unsupported bits 0x{unchecked((uint)(int)unsupportedBits):X8}.");
                return new SkyrimPreparedFormListEdit(
                    SkyrimPreparedEditKind.SetMajorRecordFlags,
                    fingerprint,
                    error,
                    majorRecordFlags: flags);
            }
            default:
                return PrepareUnsupported(edit);
        }
    }

    /// <summary>Prepares a command with no payload fields.</summary>
    /// <param name="commandName">The stable command discriminator.</param>
    /// <param name="kind">The exact private mutation discriminator.</param>
    /// <returns>The immutable prepared command.</returns>
    private static SkyrimPreparedFormListEdit PrepareEmpty(
        string commandName,
        SkyrimPreparedEditKind kind)
    {
        var fingerprint = NativeEditFingerprintFactory.Create(
            commandName,
            (writer, _) =>
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
            });
        return new SkyrimPreparedFormListEdit(kind, fingerprint);
    }

    /// <summary>Prepares one zero-based index payload.</summary>
    /// <param name="commandName">The stable command discriminator.</param>
    /// <param name="kind">The exact private mutation discriminator.</param>
    /// <param name="index">The copied zero-based item index.</param>
    /// <returns>The immutable prepared command.</returns>
    private static SkyrimPreparedFormListEdit PrepareIndex(
        string commandName,
        SkyrimPreparedEditKind kind,
        int index)
    {
        var fingerprint = NativeEditFingerprintFactory.Create(
            commandName,
            (writer, _) =>
            {
                writer.WriteStartObject();
                writer.WriteNumber("index", index);
                writer.WriteEndObject();
            });
        return new SkyrimPreparedFormListEdit(kind, fingerprint, index: index);
    }

    /// <summary>Prepares one unsigned native header value.</summary>
    /// <param name="commandName">The stable command discriminator.</param>
    /// <param name="kind">The exact private mutation discriminator.</param>
    /// <param name="value">The copied unsigned header value.</param>
    /// <returns>The immutable prepared command.</returns>
    private static SkyrimPreparedFormListEdit PrepareUnsigned(
        string commandName,
        SkyrimPreparedEditKind kind,
        uint value)
    {
        var fingerprint = NativeEditFingerprintFactory.Create(
            commandName,
            (writer, _) =>
            {
                writer.WriteStartObject();
                writer.WriteNumber("value", value);
                writer.WriteEndObject();
            });
        return new SkyrimPreparedFormListEdit(kind, fingerprint, unsignedValue: value);
    }

    /// <summary>Prepares one Boolean native header value.</summary>
    /// <param name="commandName">The stable command discriminator.</param>
    /// <param name="kind">The exact private mutation discriminator.</param>
    /// <param name="value">The copied Boolean header value.</param>
    /// <returns>The immutable prepared command.</returns>
    private static SkyrimPreparedFormListEdit PrepareBoolean(
        string commandName,
        SkyrimPreparedEditKind kind,
        bool value)
    {
        var fingerprint = NativeEditFingerprintFactory.Create(
            commandName,
            (writer, _) =>
            {
                writer.WriteStartObject();
                writer.WriteBoolean("value", value);
                writer.WriteEndObject();
            });
        return new SkyrimPreparedFormListEdit(kind, fingerprint, booleanValue: value);
    }

    /// <summary>Prepares a deterministic typed failure for a command outside Skyrim's supported field surface.</summary>
    /// <param name="edit">The unsupported command whose stable name remains part of replay identity.</param>
    /// <returns>An immutable rejected payload.</returns>
    private static SkyrimPreparedFormListEdit PrepareUnsupported(FormListEdit edit)
    {
        var commandName = new string(edit.CommandName.AsSpan());
        var fingerprint = NativeEditFingerprintFactory.Create(
            commandName,
            (writer, context) =>
            {
                writer.WriteStartObject();
                writer.WritePropertyName("unsupportedCommandName");
                NativeJsonLeafWriter.WriteString(writer, commandName, context);
                writer.WriteEndObject();
            });
        return new SkyrimPreparedFormListEdit(
            SkyrimPreparedEditKind.Unsupported,
            fingerprint,
            new EngineError(
                EngineErrorCode.UnsupportedOperation,
                $"Skyrim UnsupportedGameField: command '{commandName}' is not supported."));
    }

    /// <summary>Distinguishes every supported immutable Skyrim prepared payload.</summary>
    private enum SkyrimPreparedEditKind
    {
        /// <summary>The command assigns a non-null EditorID.</summary>
        SetEditorId,

        /// <summary>The command clears the EditorID.</summary>
        ClearEditorId,

        /// <summary>The command replaces all ordered items.</summary>
        ReplaceItems,

        /// <summary>The command inserts one ordered item.</summary>
        InsertItem,

        /// <summary>The command removes one ordered item.</summary>
        RemoveItem,

        /// <summary>The command clears all items.</summary>
        ClearItems,

        /// <summary>The command moves one item to its final position.</summary>
        MoveItem,

        /// <summary>The command assigns native version-control state.</summary>
        SetVersionControl,

        /// <summary>The command assigns native form-version state.</summary>
        SetFormVersion,

        /// <summary>The command assigns native secondary-version state.</summary>
        SetVersion2,

        /// <summary>The command assigns native compression state.</summary>
        SetCompressed,

        /// <summary>The command assigns native deletion state.</summary>
        SetDeleted,

        /// <summary>The command assigns supported typed Skyrim major-record flags.</summary>
        SetMajorRecordFlags,

        /// <summary>The command is outside Skyrim's supported field surface.</summary>
        Unsupported
    }

    /// <summary>Stores one private immutable copied Skyrim payload and its complete canonical identity.</summary>
    private sealed class SkyrimPreparedFormListEdit : PreparedFormListEdit
    {
        /// <summary>Initializes one immutable prepared Skyrim payload.</summary>
        /// <param name="kind">The private typed command discriminator.</param>
        /// <param name="fingerprint">The complete canonical payload fingerprint and any leaf validation failure.</param>
        /// <param name="error">An optional command-specific validation failure.</param>
        /// <param name="editorId">The copied EditorID payload.</param>
        /// <param name="items">The copied ordered item payload.</param>
        /// <param name="index">The copied source, insert, or removal index.</param>
        /// <param name="destinationIndex">The copied final move destination.</param>
        /// <param name="item">The copied inserted FormKey.</param>
        /// <param name="unsignedValue">The copied unsigned native header value.</param>
        /// <param name="booleanValue">The copied Boolean native header value.</param>
        /// <param name="majorRecordFlags">The copied typed Skyrim major-record flag value.</param>
        internal SkyrimPreparedFormListEdit(
            SkyrimPreparedEditKind kind,
            NativeEditFingerprintResult fingerprint,
            EngineError? error = null,
            string? editorId = null,
            IReadOnlyList<FormKey>? items = null,
            int index = 0,
            int destinationIndex = 0,
            FormKey item = default,
            uint unsignedValue = 0,
            bool booleanValue = false,
            SkyrimMajorRecord.SkyrimMajorRecordFlag majorRecordFlags = default)
            : base(fingerprint.Fingerprint, fingerprint.ValidationError ?? error)
        {
            Kind = kind;
            EditorId = editorId;
            Items = Array.AsReadOnly(items?.ToArray() ?? Array.Empty<FormKey>());
            Index = index;
            DestinationIndex = destinationIndex;
            Item = item;
            UnsignedValue = unsignedValue;
            BooleanValue = booleanValue;
            MajorRecordFlags = majorRecordFlags;
        }

        /// <summary>Gets the private typed command discriminator.</summary>
        internal SkyrimPreparedEditKind Kind { get; }

        /// <summary>Gets the copied EditorID payload.</summary>
        internal string? EditorId { get; }

        /// <summary>Gets the copied immutable ordered item payload.</summary>
        internal IReadOnlyList<FormKey> Items { get; }

        /// <summary>Gets the copied source, insert, or removal index.</summary>
        internal int Index { get; }

        /// <summary>Gets the copied final move destination.</summary>
        internal int DestinationIndex { get; }

        /// <summary>Gets the copied inserted FormKey.</summary>
        internal FormKey Item { get; }

        /// <summary>Gets the copied unsigned native header value.</summary>
        internal uint UnsignedValue { get; }

        /// <summary>Gets the copied Boolean native header value.</summary>
        internal bool BooleanValue { get; }

        /// <summary>Gets the copied typed Skyrim major-record flag value.</summary>
        internal SkyrimMajorRecord.SkyrimMajorRecordFlag MajorRecordFlags { get; }
    }
}
