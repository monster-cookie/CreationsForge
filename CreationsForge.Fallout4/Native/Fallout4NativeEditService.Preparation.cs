using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using CreationsForge.Fallout4.Native.Edits;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Fallout4.Native;

/// <summary>Contains defensive preparation, canonical fingerprinting, and immutable private payload modeling for Fallout 4 edits.</summary>
public sealed partial class Fallout4NativeEditService
{
    /// <summary>Defensively copies and canonically fingerprints every reachable field of a typed Fallout 4 edit.</summary>
    /// <param name="edit">The caller-owned typed edit to prepare synchronously.</param>
    /// <returns>An immutable private native payload with its full canonical identity and deterministic validation result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="edit"/> is <see langword="null"/>.</exception>
    public PreparedFormListEdit PrepareEdit(FormListEdit edit)
    {
        ArgumentNullException.ThrowIfNull(edit);
        return edit switch
        {
            SetEditorIdEdit setEditorId => PrepareText(PreparedEditKind.SetEditorId, edit.CommandName, setEditorId.EditorId),
            ClearEditorIdEdit => PrepareEmpty(PreparedEditKind.ClearEditorId, edit.CommandName),
            SetVersionControlEdit setVersionControl => PrepareUnsigned(
                PreparedEditKind.SetVersionControl,
                edit.CommandName,
                "versionControl",
                setVersionControl.VersionControl),
            SetFormVersionEdit setFormVersion => PrepareUnsigned(
                PreparedEditKind.SetFormVersion,
                edit.CommandName,
                "formVersion",
                setFormVersion.FormVersion),
            SetVersion2Edit setVersion2 => PrepareUnsigned(
                PreparedEditKind.SetVersion2,
                edit.CommandName,
                "version2",
                setVersion2.Version2),
            SetCompressedEdit setCompressed => PrepareBoolean(
                PreparedEditKind.SetCompressed,
                edit.CommandName,
                "isCompressed",
                setCompressed.IsCompressed),
            SetDeletedEdit setDeleted => PrepareBoolean(
                PreparedEditKind.SetDeleted,
                edit.CommandName,
                "isDeleted",
                setDeleted.IsDeleted),
            ReplaceItemsEdit replaceItems => PrepareItems(
                PreparedEditKind.ReplaceItems,
                edit.CommandName,
                replaceItems.Items,
                firstIndex: 0,
                secondIndex: 0),
            InsertItemEdit insertItem => PrepareItems(
                PreparedEditKind.InsertItem,
                edit.CommandName,
                [insertItem.Item],
                insertItem.Index,
                secondIndex: 0),
            RemoveItemEdit removeItem => PrepareIndices(
                PreparedEditKind.RemoveItem,
                edit.CommandName,
                removeItem.Index,
                secondIndex: 0),
            MoveItemEdit moveItem => PrepareIndices(
                PreparedEditKind.MoveItem,
                edit.CommandName,
                moveItem.SourceIndex,
                moveItem.DestinationIndex),
            ClearItemsEdit => PrepareEmpty(PreparedEditKind.ClearItems, edit.CommandName),
            Fallout4SetNameEdit setName => PrepareName(setName),
            Fallout4ClearNameEdit => PrepareEmpty(PreparedEditKind.ClearName, edit.CommandName),
            Fallout4SetMajorRecordFlagsEdit setFlags => PrepareMajorRecordFlags(setFlags),
            _ => PrepareUnsupported(edit)
        };
    }

    /// <summary>Creates an immutable no-payload prepared command.</summary>
    /// <param name="kind">The supported internal command kind.</param>
    /// <param name="commandName">The stable caller-visible command discriminator.</param>
    /// <returns>A valid prepared command.</returns>
    private static PreparedFormListEdit PrepareEmpty(PreparedEditKind kind, string commandName)
    {
        var fingerprint = NativeEditFingerprintFactory.Create(commandName, static (writer, _) =>
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        });
        return new Fallout4PreparedFormListEdit(kind, fingerprint.Fingerprint, fingerprint.ValidationError);
    }

    /// <summary>Copies and prepares one exact string payload.</summary>
    /// <param name="kind">The supported internal command kind.</param>
    /// <param name="commandName">The stable caller-visible command discriminator.</param>
    /// <param name="text">The exact immutable string value.</param>
    /// <returns>A prepared string command, including malformed-UTF-16 rejection.</returns>
    private static PreparedFormListEdit PrepareText(PreparedEditKind kind, string commandName, string text)
    {
        var fingerprint = NativeEditFingerprintFactory.Create(commandName, (writer, context) =>
        {
            writer.WriteStartObject();
            writer.WritePropertyName("value");
            NativeJsonLeafWriter.WriteString(writer, text, context);
            writer.WriteEndObject();
        });
        return new Fallout4PreparedFormListEdit(
            kind,
            fingerprint.Fingerprint,
            fingerprint.ValidationError,
            text: text);
    }

    /// <summary>Copies and prepares one unsigned native scalar.</summary>
    /// <param name="kind">The supported internal command kind.</param>
    /// <param name="commandName">The stable caller-visible command discriminator.</param>
    /// <param name="fieldName">The canonical payload field name.</param>
    /// <param name="value">The exact unsigned value.</param>
    /// <returns>A valid prepared scalar command.</returns>
    private static PreparedFormListEdit PrepareUnsigned(
        PreparedEditKind kind,
        string commandName,
        string fieldName,
        uint value)
    {
        var fingerprint = NativeEditFingerprintFactory.Create(commandName, (writer, _) =>
        {
            writer.WriteStartObject();
            writer.WriteNumber(fieldName, value);
            writer.WriteEndObject();
        });
        return new Fallout4PreparedFormListEdit(
            kind,
            fingerprint.Fingerprint,
            fingerprint.ValidationError,
            unsignedValue: value);
    }

    /// <summary>Copies and prepares one Boolean native scalar.</summary>
    /// <param name="kind">The supported internal command kind.</param>
    /// <param name="commandName">The stable caller-visible command discriminator.</param>
    /// <param name="fieldName">The canonical payload field name.</param>
    /// <param name="value">The exact Boolean value.</param>
    /// <returns>A valid prepared Boolean command.</returns>
    private static PreparedFormListEdit PrepareBoolean(
        PreparedEditKind kind,
        string commandName,
        string fieldName,
        bool value)
    {
        var fingerprint = NativeEditFingerprintFactory.Create(commandName, (writer, _) =>
        {
            writer.WriteStartObject();
            writer.WriteBoolean(fieldName, value);
            writer.WriteEndObject();
        });
        return new Fallout4PreparedFormListEdit(
            kind,
            fingerprint.Fingerprint,
            fingerprint.ValidationError,
            booleanValue: value);
    }

    /// <summary>Copies and prepares ordered item payloads without normalizing nulls, duplicates, or order.</summary>
    /// <param name="kind">The replace or insert command kind.</param>
    /// <param name="commandName">The stable caller-visible command discriminator.</param>
    /// <param name="items">The exact ordered native identities.</param>
    /// <param name="firstIndex">The insertion position when applicable.</param>
    /// <param name="secondIndex">The reserved second index.</param>
    /// <returns>An immutable prepared item command.</returns>
    private static PreparedFormListEdit PrepareItems(
        PreparedEditKind kind,
        string commandName,
        IReadOnlyList<FormKey> items,
        int firstIndex,
        int secondIndex)
    {
        var copiedItems = Array.AsReadOnly(items.ToArray());
        var fingerprint = NativeEditFingerprintFactory.Create(commandName, (writer, context) =>
        {
            writer.WriteStartObject();
            if (kind == PreparedEditKind.InsertItem)
            {
                writer.WriteNumber("index", firstIndex);
            }

            writer.WriteStartArray("items");
            foreach (var item in copiedItems)
            {
                NativeJsonLeafWriter.WriteFormKey(writer, item, context);
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        });
        return new Fallout4PreparedFormListEdit(
            kind,
            fingerprint.Fingerprint,
            fingerprint.ValidationError,
            items: copiedItems,
            firstIndex: firstIndex,
            secondIndex: secondIndex);
    }

    /// <summary>Copies and prepares one or two ordered-list indices.</summary>
    /// <param name="kind">The remove or move command kind.</param>
    /// <param name="commandName">The stable caller-visible command discriminator.</param>
    /// <param name="firstIndex">The removal or source index.</param>
    /// <param name="secondIndex">The final destination index when moving.</param>
    /// <returns>An immutable prepared index command.</returns>
    private static PreparedFormListEdit PrepareIndices(
        PreparedEditKind kind,
        string commandName,
        int firstIndex,
        int secondIndex)
    {
        var fingerprint = NativeEditFingerprintFactory.Create(commandName, (writer, _) =>
        {
            writer.WriteStartObject();
            writer.WriteNumber(kind == PreparedEditKind.MoveItem ? "sourceIndex" : "index", firstIndex);
            if (kind == PreparedEditKind.MoveItem)
            {
                writer.WriteNumber("destinationIndex", secondIndex);
            }

            writer.WriteEndObject();
        });
        return new Fallout4PreparedFormListEdit(
            kind,
            fingerprint.Fingerprint,
            fingerprint.ValidationError,
            firstIndex: firstIndex,
            secondIndex: secondIndex);
    }

    /// <summary>Copies every localized entry and prepares a canonical native name payload.</summary>
    /// <param name="edit">The caller-owned Fallout 4 name command.</param>
    /// <returns>An immutable translated-string snapshot, including malformed-UTF-16 rejection.</returns>
    private static PreparedFormListEdit PrepareName(Fallout4SetNameEdit edit)
    {
        var translations = edit.Name
            .OrderBy(entry => (int)entry.Key)
            .ToArray();
        var copiedTranslations = Array.AsReadOnly(translations);
        var nativeCopy = new TranslatedString(edit.Name.TargetLanguage, copiedTranslations);
        var fingerprint = NativeEditFingerprintFactory.Create(edit.CommandName, (writer, context) =>
        {
            writer.WriteStartObject();
            writer.WritePropertyName("name");
            NativeJsonLeafWriter.WriteTranslatedString(writer, nativeCopy, CancellationToken.None, context);
            writer.WriteEndObject();
        });
        return new Fallout4PreparedFormListEdit(
            PreparedEditKind.SetName,
            fingerprint.Fingerprint,
            fingerprint.ValidationError,
            nameLanguage: edit.Name.TargetLanguage,
            translations: copiedTranslations);
    }

    /// <summary>Prepares typed flags and rejects bits outside the installed Fallout 4 native enum.</summary>
    /// <param name="edit">The typed Fallout 4 flag command.</param>
    /// <returns>A prepared flag command with deterministic unsupported-bit validation.</returns>
    private static PreparedFormListEdit PrepareMajorRecordFlags(Fallout4SetMajorRecordFlagsEdit edit)
    {
        var numericFlags = unchecked((uint)(int)edit.MajorRecordFlags);
        var fingerprint = NativeEditFingerprintFactory.Create(edit.CommandName, (writer, _) =>
        {
            writer.WriteStartObject();
            writer.WriteNumber("majorRecordFlags", numericFlags);
            writer.WriteEndObject();
        });
        var error = fingerprint.ValidationError;
        if ((numericFlags & ~GetSupportedMajorRecordFlagBits()) != 0)
        {
            error = new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Fallout 4 major-record flags contain unsupported bits 0x{numericFlags & ~GetSupportedMajorRecordFlagBits():X8}.");
        }

        return new Fallout4PreparedFormListEdit(
            PreparedEditKind.SetMajorRecordFlags,
            fingerprint.Fingerprint,
            error,
            majorRecordFlags: edit.MajorRecordFlags);
    }

    /// <summary>Fingerprints an unknown named command while retaining a deterministic typed rejection.</summary>
    /// <param name="edit">The unsupported caller-owned command.</param>
    /// <returns>An invalid prepared command whose identity includes its exact discriminator.</returns>
    private static PreparedFormListEdit PrepareUnsupported(FormListEdit edit)
    {
        var fingerprint = NativeEditFingerprintFactory.Create(edit.CommandName, static (writer, _) =>
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        });
        return new Fallout4PreparedFormListEdit(
            PreparedEditKind.Unsupported,
            fingerprint.Fingerprint,
            new EngineError(
                EngineErrorCode.UnsupportedOperation,
                $"Fallout 4 does not support FormList edit command '{edit.CommandName}'."));
    }

    /// <summary>Combines every declared installed native enum value into the supported flag mask.</summary>
    /// <returns>The complete unsigned supported-bit mask.</returns>
    private static uint GetSupportedMajorRecordFlagBits()
    {
        var supported = 0U;
        foreach (var value in Enum.GetValues<Fallout4MajorRecord.Fallout4MajorRecordFlag>())
        {
            supported |= unchecked((uint)(int)value);
        }

        return supported;
    }

    /// <summary>Identifies every supported private Fallout 4 prepared-command payload.</summary>
    private enum PreparedEditKind
    {
        /// <summary>An unsupported caller-supplied command.</summary>
        Unsupported,

        /// <summary>Sets EditorID.</summary>
        SetEditorId,

        /// <summary>Clears EditorID.</summary>
        ClearEditorId,

        /// <summary>Sets VersionControl.</summary>
        SetVersionControl,

        /// <summary>Sets FormVersion.</summary>
        SetFormVersion,

        /// <summary>Sets Version2.</summary>
        SetVersion2,

        /// <summary>Sets native compression state.</summary>
        SetCompressed,

        /// <summary>Sets native deletion state.</summary>
        SetDeleted,

        /// <summary>Replaces all ordered items.</summary>
        ReplaceItems,

        /// <summary>Inserts one ordered item.</summary>
        InsertItem,

        /// <summary>Removes one ordered item.</summary>
        RemoveItem,

        /// <summary>Moves one item to its final position.</summary>
        MoveItem,

        /// <summary>Clears every item.</summary>
        ClearItems,

        /// <summary>Sets the complete translated name.</summary>
        SetName,

        /// <summary>Clears the optional translated name.</summary>
        ClearName,

        /// <summary>Sets the typed native major-record flags.</summary>
        SetMajorRecordFlags
    }

    /// <summary>Holds one private immutable defensively copied Fallout 4 command payload.</summary>
    private sealed class Fallout4PreparedFormListEdit : PreparedFormListEdit
    {
        /// <summary>Initializes a private prepared payload whose mutable native values are represented as immutable scalars and arrays.</summary>
        /// <param name="kind">The internal typed command discriminator.</param>
        /// <param name="fingerprint">The complete canonical fingerprint.</param>
        /// <param name="error">A deterministic preparation failure, or <see langword="null"/>.</param>
        /// <param name="text">The optional exact string payload.</param>
        /// <param name="unsignedValue">The optional unsigned scalar payload.</param>
        /// <param name="booleanValue">The optional Boolean scalar payload.</param>
        /// <param name="items">The optional ordered FormKey payload.</param>
        /// <param name="firstIndex">The optional first ordered-list index.</param>
        /// <param name="secondIndex">The optional second ordered-list index.</param>
        /// <param name="nameLanguage">The translated-name target language.</param>
        /// <param name="translations">The ordered immutable translated-name values.</param>
        /// <param name="majorRecordFlags">The typed Fallout 4 major-record flags.</param>
        internal Fallout4PreparedFormListEdit(
            PreparedEditKind kind,
            OperationFingerprint fingerprint,
            EngineError? error,
            string? text = null,
            uint unsignedValue = 0,
            bool booleanValue = false,
            IReadOnlyList<FormKey>? items = null,
            int firstIndex = 0,
            int secondIndex = 0,
            Language nameLanguage = Language.English,
            IReadOnlyList<KeyValuePair<Language, string>>? translations = null,
            Fallout4MajorRecord.Fallout4MajorRecordFlag majorRecordFlags = default)
            : base(fingerprint, error)
        {
            Kind = kind;
            Text = text;
            UnsignedValue = unsignedValue;
            BooleanValue = booleanValue;
            Items = Array.AsReadOnly(items?.ToArray() ?? Array.Empty<FormKey>());
            FirstIndex = firstIndex;
            SecondIndex = secondIndex;
            NameLanguage = nameLanguage;
            Translations = Array.AsReadOnly(translations?.ToArray() ?? Array.Empty<KeyValuePair<Language, string>>());
            MajorRecordFlags = majorRecordFlags;
        }

        /// <summary>Gets the private typed command discriminator.</summary>
        internal PreparedEditKind Kind { get; }

        /// <summary>Gets the optional exact string payload.</summary>
        internal string? Text { get; }

        /// <summary>Gets the unsigned scalar payload.</summary>
        internal uint UnsignedValue { get; }

        /// <summary>Gets the Boolean scalar payload.</summary>
        internal bool BooleanValue { get; }

        /// <summary>Gets the immutable ordered FormKey payload.</summary>
        internal IReadOnlyList<FormKey> Items { get; }

        /// <summary>Gets the first ordered-list index.</summary>
        internal int FirstIndex { get; }

        /// <summary>Gets the second ordered-list index.</summary>
        internal int SecondIndex { get; }

        /// <summary>Gets the translated-name target language.</summary>
        internal Language NameLanguage { get; }

        /// <summary>Gets the immutable ordered translated-name values.</summary>
        internal IReadOnlyList<KeyValuePair<Language, string>> Translations { get; }

        /// <summary>Gets the typed Fallout 4 major-record flags.</summary>
        internal Fallout4MajorRecord.Fallout4MajorRecordFlag MajorRecordFlags { get; }
    }

}
