using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield.Native.NativeInspection;

/// <summary>
/// Writes and compares complete Starfield FormList state through direct typed native getters.
/// </summary>
public sealed class StarfieldFormListNativeInspector : IFormListNativeInspector
{
    /// <summary>Initializes the stateless Starfield FormList inspector.</summary>
    public StarfieldFormListNativeInspector()
    { }

    /// <summary>Writes every Starfield FormList field in generated field-index order.</summary>
    /// <param name="record">The detached native Starfield FormList getter.</param>
    /// <param name="writer">The caller-owned writer that receives exactly one JSON object.</param>
    /// <param name="cancellationToken">A token observed while traversing native fields and collections.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> or <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="record"/> is not a Starfield FormList.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public void WriteReadView(
        IMajorRecordGetter record,
        Utf8JsonWriter writer,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentNullException.ThrowIfNull(writer);
        var formList = RequireFormList(record, nameof(record));
        cancellationToken.ThrowIfCancellationRequested();

        WriteFormList(formList, writer, cancellationToken, null);
    }

    /// <summary>Writes every root and nested field with the requested read-view or canonical representation.</summary>
    /// <param name="formList">The typed native Starfield FormList getter.</param>
    /// <param name="writer">The caller-owned writer that receives exactly one JSON object.</param>
    /// <param name="cancellationToken">A token observed throughout native traversal.</param>
    /// <param name="context">The optional canonical write context; <see langword="null"/> preserves the public read view.</param>
    internal static void WriteFormList(
        IFormListGetter formList,
        Utf8JsonWriter writer,
        CancellationToken cancellationToken,
        NativeJsonWriteContext? context)
    {
        ArgumentNullException.ThrowIfNull(formList);
        ArgumentNullException.ThrowIfNull(writer);
        cancellationToken.ThrowIfCancellationRequested();

        writer.WriteStartObject();
        writer.WriteNumber(nameof(IMajorRecordGetter.MajorRecordFlagsRaw), formList.MajorRecordFlagsRaw);
        writer.WritePropertyName(nameof(IMajorRecordGetter.FormKey));
        NativeJsonLeafWriter.WriteFormKey(writer, formList.FormKey, context);
        writer.WriteNumber(nameof(IMajorRecordGetter.VersionControl), formList.VersionControl);
        writer.WritePropertyName(nameof(IMajorRecordGetter.EditorID));
        NativeJsonLeafWriter.WriteString(writer, formList.EditorID, context);
        writer.WriteNumber(nameof(IStarfieldMajorRecordGetter.FormVersion), formList.FormVersion);
        writer.WriteNumber(nameof(IStarfieldMajorRecordGetter.Version2), formList.Version2);
        writer.WriteNumber(
            nameof(IStarfieldMajorRecordGetter.StarfieldMajorRecordFlags),
            (int)formList.StarfieldMajorRecordFlags);
        WriteComponents(writer, formList.Components, cancellationToken, context);
        writer.WritePropertyName(nameof(IFormListGetter.Name));
        NativeJsonLeafWriter.WriteTranslatedString(writer, formList.Name, cancellationToken, context);
        writer.WriteStartArray(nameof(IFormListGetter.Items));
        foreach (var item in formList.Items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            NativeJsonLeafWriter.WriteFormLink(writer, item, context);
        }

        writer.WriteEndArray();
        WriteConditionalEntries(writer, formList.ConditionalEntries, cancellationToken, context);
        writer.WritePropertyName(nameof(IFormListGetter.AddToList));
        NativeJsonLeafWriter.WriteFormLink(writer, formList.AddToList, context);
        writer.WriteEndObject();
    }

    /// <summary>Compares every Starfield FormList field through direct typed native values.</summary>
    /// <param name="before">The detached prior Starfield FormList getter, or <see langword="null"/> when absent.</param>
    /// <param name="after">The detached resulting Starfield FormList getter, or <see langword="null"/> when absent.</param>
    /// <param name="cancellationToken">A token observed while comparing native fields and collections.</param>
    /// <returns>Immutable semantic changes in generated Starfield field-index order.</returns>
    /// <exception cref="ArgumentException">Thrown when a supplied record is not a Starfield FormList.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public IReadOnlyList<SemanticChangeDescriptor> Compare(
        IMajorRecordGetter? before,
        IMajorRecordGetter? after,
        CancellationToken cancellationToken)
    {
        var beforeFormList = RequireOptionalFormList(before, nameof(before));
        var afterFormList = RequireOptionalFormList(after, nameof(after));
        cancellationToken.ThrowIfCancellationRequested();

        var changes = new List<SemanticChangeDescriptor>();
        if (!NativeSemanticComparer.CompareRecordPresence(
            beforeFormList,
            afterFormList,
            changes,
            cancellationToken))
        {
            return Array.AsReadOnly(changes.ToArray());
        }

        var presentBefore = beforeFormList!;
        var presentAfter = afterFormList!;

        NativeSemanticComparer.CompareValue(
            nameof(IMajorRecordGetter.MajorRecordFlagsRaw),
            presentBefore.MajorRecordFlagsRaw == presentAfter.MajorRecordFlagsRaw,
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareValue(
            nameof(IMajorRecordGetter.FormKey),
            NativeSemanticComparer.FormKeyEquals(presentBefore.FormKey, presentAfter.FormKey),
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareValue(
            nameof(IMajorRecordGetter.VersionControl),
            presentBefore.VersionControl == presentAfter.VersionControl,
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareValue(
            nameof(IMajorRecordGetter.EditorID),
            NativeSemanticComparer.StringEquals(presentBefore.EditorID, presentAfter.EditorID),
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareValue(
            nameof(IStarfieldMajorRecordGetter.FormVersion),
            presentBefore.FormVersion == presentAfter.FormVersion,
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareValue(
            nameof(IStarfieldMajorRecordGetter.Version2),
            presentBefore.Version2 == presentAfter.Version2,
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareValue(
            nameof(IStarfieldMajorRecordGetter.StarfieldMajorRecordFlags),
            presentBefore.StarfieldMajorRecordFlags == presentAfter.StarfieldMajorRecordFlags,
            changes,
            cancellationToken);
        CompareComponents(presentBefore.Components, presentAfter.Components, changes, cancellationToken);
        NativeSemanticComparer.CompareValue(
            nameof(IFormListGetter.Name),
            NativeSemanticComparer.TranslatedStringEquals(presentBefore.Name, presentAfter.Name, cancellationToken),
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareOrdered(
            nameof(IFormListGetter.Items),
            presentBefore.Items,
            presentAfter.Items,
            NativeSemanticComparer.FormLinkEquals,
            changes,
            cancellationToken);
        CompareConditionalEntries(
            presentBefore.ConditionalEntries,
            presentAfter.ConditionalEntries,
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareValue(
            nameof(IFormListGetter.AddToList),
            NativeSemanticComparer.FormLinkEquals(presentBefore.AddToList, presentAfter.AddToList),
            changes,
            cancellationToken);

        return Array.AsReadOnly(changes.ToArray());
    }

    /// <summary>Writes the ordered polymorphic component collection as complete nested native objects.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="components">The native component getters in source order.</param>
    /// <param name="cancellationToken">A token observed before every nested component.</param>
    /// <param name="context">The optional canonical representation context.</param>
    internal static void WriteComponents(
        Utf8JsonWriter writer,
        IReadOnlyList<IAComponentGetter> components,
        CancellationToken cancellationToken,
        NativeJsonWriteContext? context)
    {
        writer.WriteStartArray(nameof(IFormListGetter.Components));
        foreach (var component in components)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StarfieldNestedFieldCodec.WriteComponent(writer, component, cancellationToken, context);
        }

        writer.WriteEndArray();
    }

    /// <summary>Writes ordered conditional entries while delegating each concrete condition graph.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="entries">The native conditional entries in source order.</param>
    /// <param name="cancellationToken">A token observed before every entry and condition.</param>
    /// <param name="context">The optional canonical representation context.</param>
    internal static void WriteConditionalEntries(
        Utf8JsonWriter writer,
        IReadOnlyList<IFormListConditionalEntryGetter> entries,
        CancellationToken cancellationToken,
        NativeJsonWriteContext? context)
    {
        writer.WriteStartArray(nameof(IFormListGetter.ConditionalEntries));
        foreach (var entry in entries)
        {
            cancellationToken.ThrowIfCancellationRequested();
            writer.WriteStartObject();
            if (entry.Index.HasValue)
            {
                writer.WriteNumber(nameof(IFormListConditionalEntryGetter.Index), entry.Index.Value);
            }
            else
            {
                writer.WriteNull(nameof(IFormListConditionalEntryGetter.Index));
            }

            writer.WritePropertyName(nameof(IFormListConditionalEntryGetter.Conditions));
            var conditions = entry.Conditions;
            if (conditions is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartArray();
                foreach (var condition in conditions)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    StarfieldNestedFieldCodec.WriteCondition(writer, condition, cancellationToken, context);
                }

                writer.WriteEndArray();
            }

            writer.WriteEndObject();
        }

        writer.WriteEndArray();
    }

    /// <summary>Compares component positions structurally and delegates same-type nested values.</summary>
    /// <param name="before">The prior component sequence.</param>
    /// <param name="after">The resulting component sequence.</param>
    /// <param name="changes">The ordered semantic change collector.</param>
    /// <param name="cancellationToken">A token observed at every position.</param>
    private static void CompareComponents(
        IReadOnlyList<IAComponentGetter> before,
        IReadOnlyList<IAComponentGetter> after,
        ICollection<SemanticChangeDescriptor> changes,
        CancellationToken cancellationToken)
    {
        var commonCount = Math.Min(before.Count, after.Count);
        for (var index = 0; index < commonCount; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StarfieldNestedFieldCodec.CompareComponent(
                before[index],
                after[index],
                $"{nameof(IFormListGetter.Components)}[{index}]",
                changes,
                cancellationToken);
        }

        AddStructuralChanges(nameof(IFormListGetter.Components), before.Count, after.Count, commonCount, changes, cancellationToken);
    }

    /// <summary>Compares conditional entry indices and their ordered concrete conditions.</summary>
    /// <param name="before">The prior conditional-entry sequence.</param>
    /// <param name="after">The resulting conditional-entry sequence.</param>
    /// <param name="changes">The ordered semantic change collector.</param>
    /// <param name="cancellationToken">A token observed at every entry and condition.</param>
    private static void CompareConditionalEntries(
        IReadOnlyList<IFormListConditionalEntryGetter> before,
        IReadOnlyList<IFormListConditionalEntryGetter> after,
        ICollection<SemanticChangeDescriptor> changes,
        CancellationToken cancellationToken)
    {
        var commonCount = Math.Min(before.Count, after.Count);
        for (var index = 0; index < commonCount; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var entryPath = $"{nameof(IFormListGetter.ConditionalEntries)}[{index}]";
            NativeSemanticComparer.CompareValue(
                $"{entryPath}.{nameof(IFormListConditionalEntryGetter.Index)}",
                before[index].Index == after[index].Index,
                changes,
                cancellationToken);
            var conditionsPath = $"{entryPath}.{nameof(IFormListConditionalEntryGetter.Conditions)}";
            var beforeConditions = before[index].Conditions;
            var afterConditions = after[index].Conditions;
            if (beforeConditions is null || afterConditions is null)
            {
                NativeSemanticComparer.CompareValue(
                    conditionsPath,
                    beforeConditions is null && afterConditions is null,
                    changes,
                    cancellationToken);
            }
            else
            {
                CompareConditions(
                    beforeConditions,
                    afterConditions,
                    conditionsPath,
                    changes,
                    cancellationToken);
            }
        }

        AddStructuralChanges(
            nameof(IFormListGetter.ConditionalEntries),
            before.Count,
            after.Count,
            commonCount,
            changes,
            cancellationToken);
    }

    /// <summary>Compares condition positions structurally and delegates same-type nested values.</summary>
    /// <param name="before">The prior condition sequence.</param>
    /// <param name="after">The resulting condition sequence.</param>
    /// <param name="path">The canonical native condition-collection path.</param>
    /// <param name="changes">The ordered semantic change collector.</param>
    /// <param name="cancellationToken">A token observed at every condition.</param>
    private static void CompareConditions(
        IReadOnlyList<IConditionGetter> before,
        IReadOnlyList<IConditionGetter> after,
        string path,
        ICollection<SemanticChangeDescriptor> changes,
        CancellationToken cancellationToken)
    {
        var commonCount = Math.Min(before.Count, after.Count);
        for (var index = 0; index < commonCount; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            StarfieldNestedFieldCodec.CompareCondition(
                before[index],
                after[index],
                $"{path}[{index}]",
                changes,
                cancellationToken);
        }

        AddStructuralChanges(path, before.Count, after.Count, commonCount, changes, cancellationToken);
    }

    /// <summary>Adds deterministic removal and insertion descriptors beyond a shared collection prefix.</summary>
    /// <param name="path">The canonical native collection path.</param>
    /// <param name="beforeCount">The prior collection count.</param>
    /// <param name="afterCount">The resulting collection count.</param>
    /// <param name="commonCount">The count already compared positionally.</param>
    /// <param name="changes">The ordered semantic change collector.</param>
    /// <param name="cancellationToken">A token observed before every descriptor.</param>
    private static void AddStructuralChanges(
        string path,
        int beforeCount,
        int afterCount,
        int commonCount,
        ICollection<SemanticChangeDescriptor> changes,
        CancellationToken cancellationToken)
    {
        for (var index = commonCount; index < beforeCount; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            changes.Add(new SemanticChangeDescriptor(path, SemanticChangeKind.ItemRemoved, index, null));
        }

        for (var index = commonCount; index < afterCount; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            changes.Add(new SemanticChangeDescriptor(path, SemanticChangeKind.ItemInserted, null, index));
        }
    }

    /// <summary>Requires one non-null record to expose the exact Starfield FormList getter contract.</summary>
    /// <param name="record">The native record to validate.</param>
    /// <param name="parameterName">The public parameter name used by validation diagnostics.</param>
    /// <returns>The same record through its typed Starfield FormList getter.</returns>
    /// <exception cref="ArgumentException">Thrown when the record is not a Starfield FormList.</exception>
    private static IFormListGetter RequireFormList(IMajorRecordGetter record, string parameterName)
    {
        return record as IFormListGetter
            ?? throw new ArgumentException("The native record must be a Starfield FormList getter.", parameterName);
    }

    /// <summary>Validates an optional comparison record without changing absent-record semantics.</summary>
    /// <param name="record">The optional native record to validate.</param>
    /// <param name="parameterName">The public parameter name used by validation diagnostics.</param>
    /// <returns>The typed Starfield FormList getter, or <see langword="null"/> when the record is absent.</returns>
    /// <exception cref="ArgumentException">Thrown when a supplied record is not a Starfield FormList.</exception>
    private static IFormListGetter? RequireOptionalFormList(IMajorRecordGetter? record, string parameterName)
    {
        return record is null ? null : RequireFormList(record, parameterName);
    }
}
