using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeInspection;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.Native.NativeInspection;

/// <summary>
/// Writes and compares complete Skyrim FormList state through typed native getters without retaining workspace state.
/// </summary>
public sealed class SkyrimFormListNativeInspector : IFormListNativeInspector
{
    /// <summary>Initializes the stateless Skyrim FormList inspector.</summary>
    public SkyrimFormListNativeInspector()
    { }

    /// <summary>
    /// Writes every field declared by Skyrim's generated FormList field index in native index order.
    /// </summary>
    /// <param name="record">The detached Skyrim FormList getter to inspect.</param>
    /// <param name="writer">The caller-owned writer that receives one JSON object.</param>
    /// <param name="cancellationToken">A token observed between native field and item reads.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> or <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="record"/> is not a Skyrim FormList.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public void WriteReadView(
        IMajorRecordGetter record,
        Utf8JsonWriter writer,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentNullException.ThrowIfNull(writer);
        var formList = RequireFormList(record, nameof(record));

        WriteFields(formList, writer, cancellationToken, context: null);
    }

    /// <summary>Writes every Skyrim FormList field through the selected readable or canonical leaf representation.</summary>
    /// <param name="formList">The typed detached Skyrim FormList getter.</param>
    /// <param name="writer">The caller-owned writer that receives one JSON object.</param>
    /// <param name="cancellationToken">A token observed between native field and item reads.</param>
    /// <param name="context">The optional canonical fingerprint context; <see langword="null"/> preserves read-view JSON.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="formList"/> or <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    internal void WriteFields(
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
        cancellationToken.ThrowIfCancellationRequested();
        writer.WritePropertyName(nameof(IMajorRecordGetter.FormKey));
        NativeJsonLeafWriter.WriteFormKey(writer, formList.FormKey, context);
        cancellationToken.ThrowIfCancellationRequested();
        writer.WriteNumber(nameof(IMajorRecordGetter.VersionControl), formList.VersionControl);
        cancellationToken.ThrowIfCancellationRequested();
        writer.WritePropertyName(nameof(IMajorRecordGetter.EditorID));
        NativeJsonLeafWriter.WriteString(writer, formList.EditorID, context);
        cancellationToken.ThrowIfCancellationRequested();
        writer.WriteNumber(nameof(ISkyrimMajorRecordGetter.FormVersion), formList.FormVersion);
        cancellationToken.ThrowIfCancellationRequested();
        writer.WriteNumber(nameof(ISkyrimMajorRecordGetter.Version2), formList.Version2);
        cancellationToken.ThrowIfCancellationRequested();
        writer.WriteNumber(nameof(ISkyrimMajorRecordGetter.SkyrimMajorRecordFlags), (int)formList.SkyrimMajorRecordFlags);
        cancellationToken.ThrowIfCancellationRequested();
        writer.WriteStartArray(nameof(IFormListGetter.Items));
        foreach (var item in formList.Items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            NativeJsonLeafWriter.WriteFormLink(writer, item, context);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    /// <summary>
    /// Compares every Skyrim FormList field in generated native index order and preserves item positions, duplicates, and null identities.
    /// </summary>
    /// <param name="before">The detached prior Skyrim FormList, or <see langword="null"/> when absent.</param>
    /// <param name="after">The detached resulting Skyrim FormList, or <see langword="null"/> when absent.</param>
    /// <param name="cancellationToken">A token observed between native field and item comparisons.</param>
    /// <returns>Immutable semantic changes in generated native field order.</returns>
    /// <exception cref="ArgumentException">Thrown when either supplied record is not a Skyrim FormList.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public IReadOnlyList<SemanticChangeDescriptor> Compare(
        IMajorRecordGetter? before,
        IMajorRecordGetter? after,
        CancellationToken cancellationToken)
    {
        var beforeList = RequireOptionalFormList(before, nameof(before));
        var afterList = RequireOptionalFormList(after, nameof(after));
        var changes = new List<SemanticChangeDescriptor>();
        if (!NativeSemanticComparer.CompareRecordPresence(before, after, changes, cancellationToken))
        {
            return Array.AsReadOnly(changes.ToArray());
        }

        var presentBefore = beforeList!;
        var presentAfter = afterList!;
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
            nameof(ISkyrimMajorRecordGetter.FormVersion),
            presentBefore.FormVersion == presentAfter.FormVersion,
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareValue(
            nameof(ISkyrimMajorRecordGetter.Version2),
            presentBefore.Version2 == presentAfter.Version2,
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareValue(
            nameof(ISkyrimMajorRecordGetter.SkyrimMajorRecordFlags),
            presentBefore.SkyrimMajorRecordFlags == presentAfter.SkyrimMajorRecordFlags,
            changes,
            cancellationToken);
        NativeSemanticComparer.CompareOrdered(
            nameof(IFormListGetter.Items),
            presentBefore.Items,
            presentAfter.Items,
            NativeSemanticComparer.FormLinkEquals,
            changes,
            cancellationToken);

        return Array.AsReadOnly(changes.ToArray());
    }

    /// <summary>
    /// Requires one non-null common native getter to belong to Skyrim's FormList family.
    /// </summary>
    /// <param name="record">The common native getter.</param>
    /// <param name="parameterName">The public parameter name used for a typed rejection.</param>
    /// <returns>The same record through its typed Skyrim FormList getter.</returns>
    /// <exception cref="ArgumentException">Thrown when a supplied record is not a Skyrim FormList.</exception>
    private static IFormListGetter RequireFormList(IMajorRecordGetter record, string parameterName)
    {
        return record as IFormListGetter
            ?? throw new ArgumentException("The native record must be a Skyrim FormList getter.", parameterName);
    }

    /// <summary>
    /// Validates an optional comparison record without changing absent-record semantics.
    /// </summary>
    /// <param name="record">The optional common native getter.</param>
    /// <param name="parameterName">The public parameter name used for a typed rejection.</param>
    /// <returns>The typed Skyrim FormList getter, or <see langword="null"/> when the record is absent.</returns>
    /// <exception cref="ArgumentException">Thrown when a supplied record is not a Skyrim FormList.</exception>
    private static IFormListGetter? RequireOptionalFormList(
        IMajorRecordGetter? record,
        string parameterName)
    {
        return record is null ? null : RequireFormList(record, parameterName);
    }
}
