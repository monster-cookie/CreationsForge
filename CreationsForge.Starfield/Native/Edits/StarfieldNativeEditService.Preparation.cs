using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using CreationsForge.Starfield.Native.NativeInspection;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.Native.Edits;

/// <summary>Contains closed typed Starfield edit copying and canonical payload traversal.</summary>
public sealed partial class StarfieldNativeEditService
{
    /// <summary>Copies and prepares an EditorID assignment without normalizing its ordinary text.</summary>
    /// <param name="command">The typed EditorID command.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareSetEditorId(SetEditorIdEdit command)
    {
        var editorId = string.Concat(command.EditorId);
        return CreatePrepared(
            command.CommandName,
            (writer, context) =>
            {
                writer.WriteStartObject();
                writer.WritePropertyName("editorId");
                NativeJsonLeafWriter.WriteString(writer, editorId, context);
                writer.WriteEndObject();
            },
            formList => formList.EditorID = editorId);
    }

    /// <summary>Copies and prepares one exact ordered item insertion.</summary>
    /// <param name="command">The typed item insertion command.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareInsertItem(InsertItemEdit command)
    {
        var index = command.Index;
        var item = command.Item;
        var references = CreateItemReferences([item], "item");
        return CreatePrepared(
            command.CommandName,
            (writer, context) =>
            {
                writer.WriteStartObject();
                writer.WriteNumber("index", index);
                writer.WritePropertyName("item");
                NativeJsonLeafWriter.WriteFormKey(writer, item, context);
                writer.WriteEndObject();
            },
            formList => formList.Items.Insert(index, new FormLink<IStarfieldMajorRecordGetter>(item)),
            formList => index <= formList.Items.Count
                ? null
                : InvalidPosition($"Starfield FormList item insertion index {index} exceeds count {formList.Items.Count}."),
            references);
    }

    /// <summary>Copies and prepares one exact ordered item removal.</summary>
    /// <param name="command">The typed item removal command.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareRemoveItem(RemoveItemEdit command)
    {
        var index = command.Index;
        return CreatePrepared(
            command.CommandName,
            (writer, _) =>
            {
                writer.WriteStartObject();
                writer.WriteNumber("index", index);
                writer.WriteEndObject();
            },
            formList => formList.Items.RemoveAt(index),
            formList => index < formList.Items.Count
                ? null
                : InvalidPosition($"Starfield FormList item removal index {index} is outside count {formList.Items.Count}."));
    }

    /// <summary>Copies and prepares one item move whose destination is the final resulting index.</summary>
    /// <param name="command">The typed ordered item move command.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareMoveItem(MoveItemEdit command)
    {
        var sourceIndex = command.SourceIndex;
        var destinationIndex = command.DestinationIndex;
        return CreatePrepared(
            command.CommandName,
            (writer, _) =>
            {
                writer.WriteStartObject();
                writer.WriteNumber("sourceIndex", sourceIndex);
                writer.WriteNumber("destinationIndex", destinationIndex);
                writer.WriteEndObject();
            },
            formList =>
            {
                if (sourceIndex == destinationIndex)
                {
                    return;
                }

                var item = formList.Items[sourceIndex];
                formList.Items.RemoveAt(sourceIndex);
                formList.Items.Insert(destinationIndex, item);
            },
            formList => sourceIndex < formList.Items.Count && destinationIndex < formList.Items.Count
                ? null
                : InvalidPosition(
                    $"Starfield FormList move positions {sourceIndex} to {destinationIndex} require both indices within count {formList.Items.Count}."));
    }

    /// <summary>Copies and prepares a complete ordered item replacement, retaining duplicates and null keys.</summary>
    /// <param name="command">The typed full item replacement.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareReplaceItems(ReplaceItemsEdit command)
    {
        var items = command.Items.ToArray();
        var references = CreateItemReferences(items, "items");
        return CreatePrepared(
            command.CommandName,
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
            },
            formList =>
            {
                formList.Items.Clear();
                foreach (var item in items)
                {
                    formList.Items.Add(new FormLink<IStarfieldMajorRecordGetter>(item));
                }
            },
            references: references,
            getExistingReferenceCredits: static formList => formList.Items
                .Select(static item => item.FormKeyNullable)
                .Where(static item => item.HasValue)
                .Select(static item => new PreparedReference(
                    item!.Value,
                    "items",
                    typeof(IStarfieldMajorRecordGetter)))
                .ToArray());
    }

    /// <summary>Copies and prepares the complete typed Starfield flag set while rejecting unknown bits after fingerprinting.</summary>
    /// <param name="command">The typed Starfield major-record flags command.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareMajorFlags(StarfieldSetMajorFlagsEdit command)
    {
        var flags = command.Flags;
        var unknownFlags = flags & ~KnownMajorFlags;
        var validationError = unknownFlags == 0
            ? null
            : new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Starfield major-record flags contain unknown bits 0x{(int)unknownFlags:X8}.");
        return CreatePrepared(
            command.CommandName,
            (writer, _) =>
            {
                writer.WriteStartObject();
                writer.WriteNumber("flags", (int)flags);
                writer.WriteEndObject();
            },
            formList =>
            {
                var unrepresentedRawFlags = formList.MajorRecordFlagsRaw & ~(int)KnownMajorFlags;
                formList.StarfieldMajorRecordFlags = flags;
                formList.MajorRecordFlagsRaw |= unrepresentedRawFlags;
            },
            validationError: validationError);
    }

    /// <summary>Deeply copies and prepares one complete native translated name.</summary>
    /// <param name="command">The typed translated-name command.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareSetName(StarfieldSetNameEdit command)
    {
        var name = command.Name.DeepCopy();
        return CreatePrepared(
            command.CommandName,
            (writer, context) =>
            {
                writer.WriteStartObject();
                writer.WritePropertyName("name");
                NativeJsonLeafWriter.WriteTranslatedString(writer, name, CancellationToken.None, context);
                writer.WriteEndObject();
            },
            formList => formList.Name = name.DeepCopy());
    }

    /// <summary>Copies and prepares one AddToList identity that must resolve to a live FormList.</summary>
    /// <param name="command">The typed AddToList command.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareSetAddToList(StarfieldSetAddToListEdit command)
    {
        var formKey = command.FormList;
        return CreatePrepared(
            command.CommandName,
            (writer, context) =>
            {
                writer.WriteStartObject();
                writer.WritePropertyName("formList");
                NativeJsonLeafWriter.WriteFormKey(writer, formKey, context);
                writer.WriteEndObject();
            },
            formList => formList.AddToList.SetTo(formKey),
            references: [new PreparedReference(formKey, "formList", typeof(IFormListGetter))],
            getExistingReferenceCredits: static formList => formList.AddToList.FormKeyNullable is { } existing
                ? [new PreparedReference(existing, "formList", typeof(IFormListGetter))]
                : Array.Empty<PreparedReference>());
    }

    /// <summary>Deeply copies and prepares the complete ordered conditional-entry collection.</summary>
    /// <param name="command">The typed conditional-entry replacement.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareSetConditionalEntries(StarfieldSetConditionalEntriesEdit command)
    {
        var entries = command.Entries.Select(static entry => entry.DeepCopy()).ToArray();
        var references = EnumerateNestedReferences(
            formList =>
            {
                foreach (var entry in entries)
                {
                    formList.ConditionalEntries.Add(entry.DeepCopy());
                }
            },
            "conditionalEntries");
        return CreatePrepared(
            command.CommandName,
            (writer, context) =>
            {
                writer.WriteStartObject();
                StarfieldFormListNativeInspector.WriteConditionalEntries(
                    writer,
                    entries,
                    CancellationToken.None,
                    context);
                writer.WriteEndObject();
            },
            formList =>
            {
                formList.ConditionalEntries.Clear();
                foreach (var entry in entries)
                {
                    formList.ConditionalEntries.Add(entry.DeepCopy());
                }
            },
            references: references,
            getExistingReferenceCredits: static formList => EnumerateNestedCreditReferences(
                temporary =>
                {
                    foreach (var entry in formList.ConditionalEntries)
                    {
                        temporary.ConditionalEntries.Add(entry.DeepCopy());
                    }
                }));
    }

    /// <summary>Deeply copies and prepares one exact component insertion.</summary>
    /// <param name="command">The typed component insertion.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareAddComponent(StarfieldAddComponentEdit command)
    {
        var index = command.Index;
        var component = command.Component.DeepCopy();
        var references = EnumerateNestedReferences(
            formList => formList.Components.Add(component.DeepCopy()),
            "component");
        return CreatePrepared(
            command.CommandName,
            (writer, context) => WriteIndexedComponentPayload(writer, context, index, component),
            formList => formList.Components.Insert(index, component.DeepCopy()),
            formList => index <= formList.Components.Count
                ? null
                : InvalidPosition($"Starfield component insertion index {index} exceeds count {formList.Components.Count}."),
            references);
    }

    /// <summary>Deeply copies and prepares one exact component replacement.</summary>
    /// <param name="command">The typed component replacement.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareReplaceComponent(StarfieldReplaceComponentEdit command)
    {
        var index = command.Index;
        var component = command.Component.DeepCopy();
        var references = EnumerateNestedReferences(
            formList => formList.Components.Add(component.DeepCopy()),
            "component");
        return CreatePrepared(
            command.CommandName,
            (writer, context) => WriteIndexedComponentPayload(writer, context, index, component),
            formList => formList.Components[index] = component.DeepCopy(),
            formList => index < formList.Components.Count
                ? null
                : InvalidPosition($"Starfield component replacement index {index} is outside count {formList.Components.Count}."),
            references,
            formList => EnumerateNestedCreditReferences(
                temporary => temporary.Components.Add(formList.Components[index].DeepCopy())));
    }

    /// <summary>Copies and prepares one exact component removal.</summary>
    /// <param name="command">The typed component removal.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareRemoveComponent(StarfieldRemoveComponentEdit command)
    {
        var index = command.Index;
        return CreatePrepared(
            command.CommandName,
            (writer, _) =>
            {
                writer.WriteStartObject();
                writer.WriteNumber("index", index);
                writer.WriteEndObject();
            },
            formList => formList.Components.RemoveAt(index),
            formList => index < formList.Components.Count
                ? null
                : InvalidPosition($"Starfield component removal index {index} is outside count {formList.Components.Count}."));
    }

    /// <summary>Deeply copies and prepares the complete ordered component replacement.</summary>
    /// <param name="command">The typed full component replacement.</param>
    /// <returns>The immutable prepared command.</returns>
    private static StarfieldPreparedEdit PrepareReplaceComponents(StarfieldReplaceComponentsEdit command)
    {
        var components = command.Components.Select(static component => component.DeepCopy()).ToArray();
        var references = EnumerateNestedReferences(
            formList =>
            {
                foreach (var component in components)
                {
                    formList.Components.Add(component.DeepCopy());
                }
            },
            "components");
        return CreatePrepared(
            command.CommandName,
            (writer, context) =>
            {
                writer.WriteStartObject();
                StarfieldFormListNativeInspector.WriteComponents(
                    writer,
                    components,
                    CancellationToken.None,
                    context);
                writer.WriteEndObject();
            },
            formList =>
            {
                formList.Components.Clear();
                foreach (var component in components)
                {
                    formList.Components.Add(component.DeepCopy());
                }
            },
            references: references,
            getExistingReferenceCredits: static formList => EnumerateNestedCreditReferences(
                temporary =>
                {
                    foreach (var component in formList.Components)
                    {
                        temporary.Components.Add(component.DeepCopy());
                    }
                }));
    }

    /// <summary>Creates a canonically fingerprinted unsupported-operation result for an unknown typed command.</summary>
    /// <param name="command">The unknown command whose stable discriminator remains part of the fingerprint.</param>
    /// <returns>An invalid prepared payload that cannot be applied.</returns>
    private static StarfieldPreparedEdit PrepareUnsupported(FormListEdit command)
    {
        return CreatePrepared(
            command.CommandName,
            static (writer, _) =>
            {
                writer.WriteStartObject();
                writer.WriteEndObject();
            },
            static _ => { },
            validationError: new EngineError(
                EngineErrorCode.UnsupportedOperation,
                $"Starfield does not support typed FormList command '{command.CommandName}'."));
    }

    /// <summary>Writes one indexed concrete component payload through the complete generated nested traversal.</summary>
    /// <param name="writer">The canonical caller-owned writer.</param>
    /// <param name="context">The canonical fingerprint context.</param>
    /// <param name="index">The exact component position.</param>
    /// <param name="component">The defensively copied concrete native component.</param>
    private static void WriteIndexedComponentPayload(
        Utf8JsonWriter writer,
        NativeJsonWriteContext context,
        int index,
        IAComponentGetter component)
    {
        writer.WriteStartObject();
        writer.WriteNumber("index", index);
        writer.WritePropertyName("component");
        StarfieldNestedFieldCodec.WriteComponent(writer, component, CancellationToken.None, context);
        writer.WriteEndObject();
    }

    /// <summary>Creates reference metadata for a copied item sequence while preserving duplicate order and skipping intentional null keys.</summary>
    /// <param name="items">The exact copied native item identities.</param>
    /// <param name="fieldName">The canonical singular or collection field name.</param>
    /// <returns>Non-null links in deterministic payload order.</returns>
    private static IReadOnlyList<PreparedReference> CreateItemReferences(
        IReadOnlyList<FormKey> items,
        string fieldName)
    {
        var references = new List<PreparedReference>();
        for (var index = 0; index < items.Count; index++)
        {
            if (!items[index].IsNull)
            {
                var path = items.Count == 1 ? fieldName : $"{fieldName}[{index}]";
                references.Add(new PreparedReference(
                    items[index],
                    path,
                    typeof(IStarfieldMajorRecordGetter)));
            }
        }

        return Array.AsReadOnly(references.ToArray());
    }

    /// <summary>Enumerates every native link reachable from copied nested payloads without retaining a parallel record model.</summary>
    /// <param name="populate">The closed typed action that installs copied payloads into a temporary FormList.</param>
    /// <param name="fieldName">The command-relative diagnostic prefix.</param>
    /// <returns>Every non-null native link in Mutagen traversal order.</returns>
    private static IReadOnlyList<PreparedReference> EnumerateNestedReferences(
        Action<FormList> populate,
        string fieldName)
    {
        var links = EnumerateNestedLinks(populate, fieldName);
        return links;
    }

    /// <summary>Enumerates native identity and declared getter-family pairs reachable from one copied nested payload.</summary>
    /// <param name="populate">The closed typed action that installs copied payloads into a temporary FormList.</param>
    /// <param name="fieldName">The command-relative diagnostic prefix.</param>
    /// <returns>Typed links in Mutagen traversal order, including duplicate occurrences.</returns>
    private static IReadOnlyList<PreparedReference> EnumerateNestedLinks(
        Action<FormList> populate,
        string fieldName)
    {
        var temporary = new FormList(
            new FormKey((ModKey)"CreationsForgeFingerprint.esm", 0x00000800),
            StarfieldRelease.Starfield);
        populate(temporary);
        var references = new List<PreparedReference>();
        var index = 0;
        foreach (var link in temporary.EnumerateFormLinks(false))
        {
            if (link.FormKeyNullable is { } formKey && !formKey.IsNull)
            {
                references.Add(new PreparedReference(
                    formKey,
                    $"{fieldName}.formLinks[{index}]",
                    link.Type));
            }

            index++;
        }

        return Array.AsReadOnly(references.ToArray());
    }

    /// <summary>Enumerates typed non-null links reachable from one current nested payload for retained multiplicity credit.</summary>
    /// <param name="populate">The closed typed action that installs copied payloads into a temporary FormList.</param>
    /// <returns>Native identity and declared getter-family pairs in Mutagen traversal order.</returns>
    private static IReadOnlyList<PreparedReference> EnumerateNestedCreditReferences(Action<FormList> populate)
    {
        return EnumerateNestedLinks(populate, "retained");
    }
}
