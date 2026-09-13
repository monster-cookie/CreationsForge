using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordInspection;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Fallout4.PluginAdapter.RecordInspection;

/// <summary>
/// Writes and compares complete Fallout 4 FormList state through direct typed record getters.
/// </summary>
public sealed class Fallout4FormListInspector : IFormListInspector
{
    /// <summary>The installed record field name at index zero.</summary>
    private const string MajorRecordFlagsRawField = "MajorRecordFlagsRaw";

    /// <summary>The installed record field name at index one.</summary>
    private const string FormKeyField = "FormKey";

    /// <summary>The installed record field name at index two.</summary>
    private const string VersionControlField = "VersionControl";

    /// <summary>The installed record field name at index three.</summary>
    private const string EditorIdField = "EditorID";

    /// <summary>The installed record field name at index four.</summary>
    private const string FormVersionField = "FormVersion";

    /// <summary>The installed record field name at index five.</summary>
    private const string Version2Field = "Version2";

    /// <summary>The installed record field name at index six.</summary>
    private const string Fallout4MajorRecordFlagsField = "Fallout4MajorRecordFlags";

    /// <summary>The installed record field name at index seven.</summary>
    private const string NameField = "Name";

    /// <summary>The installed record field name at index eight.</summary>
    private const string ItemsField = "Items";

    /// <summary>Initializes the stateless Fallout 4 FormList inspector.</summary>
    public Fallout4FormListInspector()
    { }

    /// <summary>Writes every Fallout 4 FormList field in generated field-index order.</summary>
    /// <param name="record">The detached Mutagen Fallout 4 FormList getter.</param>
    /// <param name="writer">The caller-owned writer that receives exactly one JSON object.</param>
    /// <param name="cancellationToken">A token observed while traversing record fields and items.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> or <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="record"/> is not a Fallout 4 FormList.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public void WriteReadView(
        IMajorRecordGetter record,
        Utf8JsonWriter writer,
        CancellationToken cancellationToken)
    {
        WriteReadView(record, writer, cancellationToken, context: null);
    }

    /// <summary>Writes every Fallout 4 FormList field using readable or canonical Mutagen leaf representations.</summary>
    /// <param name="record">The detached Mutagen Fallout 4 FormList getter.</param>
    /// <param name="writer">The caller-owned writer that receives exactly one JSON object.</param>
    /// <param name="cancellationToken">A token observed while traversing record fields and items.</param>
    /// <param name="context">The optional record JSON representation and validation context.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> or <paramref name="writer"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="record"/> is not a Fallout 4 FormList.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    internal void WriteReadView(
        IMajorRecordGetter record,
        Utf8JsonWriter writer,
        CancellationToken cancellationToken,
        RecordJsonWriteContext? context)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentNullException.ThrowIfNull(writer);
        var formList = RequireFormList(record, nameof(record));
        cancellationToken.ThrowIfCancellationRequested();

        writer.WriteStartObject();
        writer.WriteNumber(MajorRecordFlagsRawField, formList.MajorRecordFlagsRaw);
        writer.WritePropertyName(FormKeyField);
        RecordJsonLeafWriter.WriteFormKey(writer, formList.FormKey, context);
        writer.WriteNumber(VersionControlField, formList.VersionControl);
        writer.WritePropertyName(EditorIdField);
        RecordJsonLeafWriter.WriteString(writer, formList.EditorID, context);
        writer.WriteNumber(FormVersionField, formList.FormVersion);
        writer.WriteNumber(Version2Field, formList.Version2);
        writer.WriteNumber(
            Fallout4MajorRecordFlagsField,
            (int)formList.Fallout4MajorRecordFlags);
        writer.WritePropertyName(NameField);
        RecordJsonLeafWriter.WriteTranslatedString(writer, formList.Name, cancellationToken, context);
        writer.WriteStartArray(ItemsField);
        foreach (var item in formList.Items)
        {
            cancellationToken.ThrowIfCancellationRequested();
            RecordJsonLeafWriter.WriteFormLink(writer, item, context);
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }

    /// <summary>Compares every Fallout 4 FormList field through direct typed record values.</summary>
    /// <param name="before">The detached prior Fallout 4 FormList getter, or <see langword="null"/> when absent.</param>
    /// <param name="after">The detached resulting Fallout 4 FormList getter, or <see langword="null"/> when absent.</param>
    /// <param name="cancellationToken">A token observed while comparing record fields and items.</param>
    /// <returns>Immutable semantic changes in generated Fallout 4 field-index order.</returns>
    /// <exception cref="ArgumentException">Thrown when a supplied record is not a Fallout 4 FormList.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    public IReadOnlyList<SemanticChangeDescriptor> Compare(
        IMajorRecordGetter? before,
        IMajorRecordGetter? after,
        CancellationToken cancellationToken)
    {
        var beforeFormList = RequireOptionalFormList(before, nameof(before));
        var afterFormList = RequireOptionalFormList(after, nameof(after));
        var changes = new List<SemanticChangeDescriptor>();
        if (!RecordSemanticComparer.CompareRecordPresence(before, after, changes, cancellationToken))
        {
            return Array.AsReadOnly(changes.ToArray());
        }

        var presentBefore = beforeFormList!;
        var presentAfter = afterFormList!;
        RecordSemanticComparer.CompareValue(
            MajorRecordFlagsRawField,
            presentBefore.MajorRecordFlagsRaw == presentAfter.MajorRecordFlagsRaw,
            changes,
            cancellationToken);
        RecordSemanticComparer.CompareValue(
            FormKeyField,
            RecordSemanticComparer.FormKeyEquals(presentBefore.FormKey, presentAfter.FormKey),
            changes,
            cancellationToken);
        RecordSemanticComparer.CompareValue(
            VersionControlField,
            presentBefore.VersionControl == presentAfter.VersionControl,
            changes,
            cancellationToken);
        RecordSemanticComparer.CompareValue(
            EditorIdField,
            RecordSemanticComparer.StringEquals(presentBefore.EditorID, presentAfter.EditorID),
            changes,
            cancellationToken);
        RecordSemanticComparer.CompareValue(
            FormVersionField,
            presentBefore.FormVersion == presentAfter.FormVersion,
            changes,
            cancellationToken);
        RecordSemanticComparer.CompareValue(
            Version2Field,
            presentBefore.Version2 == presentAfter.Version2,
            changes,
            cancellationToken);
        RecordSemanticComparer.CompareValue(
            Fallout4MajorRecordFlagsField,
            presentBefore.Fallout4MajorRecordFlags == presentAfter.Fallout4MajorRecordFlags,
            changes,
            cancellationToken);
        RecordSemanticComparer.CompareValue(
            NameField,
            RecordSemanticComparer.TranslatedStringEquals(presentBefore.Name, presentAfter.Name, cancellationToken),
            changes,
            cancellationToken);
        RecordSemanticComparer.CompareOrdered(
            ItemsField,
            presentBefore.Items,
            presentAfter.Items,
            RecordSemanticComparer.FormLinkEquals,
            changes,
            cancellationToken);

        return Array.AsReadOnly(changes.ToArray());
    }

    /// <summary>Requires one non-null record to expose the exact Fallout 4 FormList getter contract.</summary>
    /// <param name="record">The record to validate.</param>
    /// <param name="parameterName">The public parameter name used by validation diagnostics.</param>
    /// <returns>The same record through its typed Fallout 4 FormList getter.</returns>
    /// <exception cref="ArgumentException">Thrown when the record is not a Fallout 4 FormList.</exception>
    private static IFormListGetter RequireFormList(IMajorRecordGetter record, string parameterName)
    {
        return record as IFormListGetter
            ?? throw new ArgumentException("The record must be a Fallout 4 FormList getter.", parameterName);
    }

    /// <summary>Validates an optional comparison record without changing absent-record semantics.</summary>
    /// <param name="record">The optional record to validate.</param>
    /// <param name="parameterName">The public parameter name used by validation diagnostics.</param>
    /// <returns>The typed Fallout 4 FormList getter, or <see langword="null"/> when the record is absent.</returns>
    /// <exception cref="ArgumentException">Thrown when a supplied record is not a Fallout 4 FormList.</exception>
    private static IFormListGetter? RequireOptionalFormList(IMajorRecordGetter? record, string parameterName)
    {
        return record is null ? null : RequireFormList(record, parameterName);
    }
}
