using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Fallout4.PluginAdapter.Edits;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Fallout4.PluginAdapter;

/// <summary>Contains typed validation and concrete unpublished-candidate mutation for Fallout 4 edits.</summary>
public sealed partial class Fallout4RecordEditService
{
    /// <summary>Applies one immutable prepared command after all target, index, flag, and new-reference validation succeeds.</summary>
    /// <param name="sources">The immutable Fallout 4 source lifetime used to validate newly introduced references.</param>
    /// <param name="candidate">The unpublished complete output candidate to mutate.</param>
    /// <param name="targetFormKey">The exact staged FormList identity.</param>
    /// <param name="preparedEdit">The immutable payload produced by this service.</param>
    /// <returns>Whether the command changed immediate Mutagen candidate content, or a typed rejection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    public EngineResult<RecordEditMutationResult> ApplyEdit(
        Fallout4PluginSourceSet sources,
        Fallout4PluginOutputState candidate,
        FormKey targetFormKey,
        PreparedFormListEdit preparedEdit)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(preparedEdit);
        if (preparedEdit is not Fallout4PreparedFormListEdit prepared)
        {
            return MutationFailure(
                sources,
                EngineErrorCode.UnsupportedOperation,
                "The prepared FormList edit was not created by the Fallout 4 Mutagen adapter.");
        }

        if (!prepared.IsValid)
        {
            return MutationFailure(sources, prepared.Error!);
        }

        Fallout4Mod mod;
        try
        {
            mod = candidate.BorrowMod();
        }
        catch (ObjectDisposedException exception)
        {
            return MutationFailure(sources, EngineErrorCode.WorkspaceDisposed, exception.Message);
        }

        var matches = FindRecords(mod, targetFormKey);
        if (matches.Count == 0)
        {
            return MutationFailure(
                sources,
                EngineErrorCode.RecordNotFound,
                $"Fallout 4 output FormList '{targetFormKey}' was not found in the unpublished candidate.");
        }

        if (matches.Count != 1 || matches[0] is not FormList formList)
        {
            return MutationFailure(
                sources,
                EngineErrorCode.ValidationFailed,
                $"Fallout 4 output identity '{targetFormKey}' is duplicated or belongs to another record family.");
        }

        var validationError = ValidateCommand(sources, mod, formList, prepared);
        if (validationError is not null)
        {
            return MutationFailure(sources, validationError);
        }

        var changed = ApplyPrepared(formList, prepared);
        return EngineResult<RecordEditMutationResult>.Success(
            new RecordEditMutationResult(changed),
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision);
    }

    /// <summary>Validates every command precondition before candidate mutation begins.</summary>
    /// <param name="sources">The immutable plugin sources used for new-reference checks.</param>
    /// <param name="mod">The complete unpublished candidate.</param>
    /// <param name="formList">The exact mutable target.</param>
    /// <param name="prepared">The private immutable command payload.</param>
    /// <returns>A deterministic validation failure, or <see langword="null"/> when application may proceed.</returns>
    private static EngineError? ValidateCommand(
        Fallout4PluginSourceSet sources,
        Fallout4Mod mod,
        FormList formList,
        Fallout4PreparedFormListEdit prepared)
    {
        if (prepared.Kind == PreparedEditKind.InsertItem && prepared.FirstIndex > formList.Items.Count)
        {
            return new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Fallout 4 FormList insertion index {prepared.FirstIndex} exceeds item count {formList.Items.Count}.");
        }

        if (prepared.Kind == PreparedEditKind.RemoveItem && prepared.FirstIndex >= formList.Items.Count)
        {
            return new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Fallout 4 FormList removal index {prepared.FirstIndex} is outside item count {formList.Items.Count}.");
        }

        if (prepared.Kind == PreparedEditKind.MoveItem
            && (prepared.FirstIndex >= formList.Items.Count || prepared.SecondIndex >= formList.Items.Count))
        {
            return new EngineError(
                EngineErrorCode.ValidationFailed,
                $"Fallout 4 FormList move indices {prepared.FirstIndex} and {prepared.SecondIndex} require item count {formList.Items.Count} or greater.");
        }

        if (prepared.Kind is not PreparedEditKind.ReplaceItems and not PreparedEditKind.InsertItem)
        {
            return null;
        }

        var retainedCounts = prepared.Kind == PreparedEditKind.ReplaceItems
            ? formList.Items
                .Select(item => item.FormKey)
                .GroupBy(item => item)
                .ToDictionary(group => group.Key, group => group.Count())
            : new Dictionary<FormKey, int>();
        foreach (var item in prepared.Items)
        {
            if (item.IsNull)
            {
                continue;
            }

            if (retainedCounts.TryGetValue(item, out var retainedCount) && retainedCount > 0)
            {
                retainedCounts[item] = retainedCount - 1;
                continue;
            }

            var resolutionError = ValidateNewReference(sources, mod, item);
            if (resolutionError is not null)
            {
                return resolutionError;
            }
        }

        return null;
    }

    /// <summary>Validates one newly requested link against staged output first and then the immutable winning source context.</summary>
    /// <param name="sources">The immutable Fallout 4 sources.</param>
    /// <param name="mod">The complete unpublished output candidate.</param>
    /// <param name="formKey">The non-null record identity to validate.</param>
    /// <returns>A deterministic validation failure, or <see langword="null"/> for one live target of any Mutagen family.</returns>
    private static EngineError? ValidateNewReference(
        Fallout4PluginSourceSet sources,
        Fallout4Mod mod,
        FormKey formKey)
    {
        var outputMatches = FindRecords(mod, formKey);
        if (outputMatches.Count > 1)
        {
            return new EngineError(
                EngineErrorCode.ValidationFailed,
                $"New Fallout 4 FormList item '{formKey}' is ambiguous in staged output.");
        }

        if (outputMatches.Count == 1)
        {
            return outputMatches[0].IsDeleted
                ? new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"New Fallout 4 FormList item '{formKey}' resolves to a deleted staged-output record.")
                : null;
        }

        var resolution = sources.Resolve(new ReferenceRequest(formKey, RecordScope.WinningOverrides));
        if (!resolution.Succeeded)
        {
            return resolution.Error;
        }

        return resolution.Value!.Status == ReferenceResolutionStatus.Resolved
            ? null
            : new EngineError(
                EngineErrorCode.ValidationFailed,
                $"New Fallout 4 FormList item '{formKey}' does not resolve to a live record; status was {resolution.Value.Status}.");
    }

    /// <summary>Applies an already validated private payload through concrete Mutagen setters.</summary>
    /// <param name="formList">The exact mutable Mutagen target.</param>
    /// <param name="prepared">The immutable prepared command.</param>
    /// <returns><see langword="true"/> when Mutagen content changed.</returns>
    private static bool ApplyPrepared(FormList formList, Fallout4PreparedFormListEdit prepared)
    {
        switch (prepared.Kind)
        {
            case PreparedEditKind.SetEditorId:
                if (string.Equals(formList.EditorID, prepared.Text, StringComparison.Ordinal))
                {
                    return false;
                }

                formList.EditorID = prepared.Text;
                return true;
            case PreparedEditKind.ClearEditorId:
                if (formList.EditorID is null)
                {
                    return false;
                }

                formList.EditorID = null;
                return true;
            case PreparedEditKind.SetVersionControl:
                if (formList.VersionControl == prepared.UnsignedValue)
                {
                    return false;
                }

                formList.VersionControl = prepared.UnsignedValue;
                return true;
            case PreparedEditKind.SetFormVersion:
                var formVersion = checked((ushort)prepared.UnsignedValue);
                if (formList.FormVersion == formVersion)
                {
                    return false;
                }

                formList.FormVersion = formVersion;
                return true;
            case PreparedEditKind.SetVersion2:
                var version2 = checked((ushort)prepared.UnsignedValue);
                if (formList.Version2 == version2)
                {
                    return false;
                }

                formList.Version2 = version2;
                return true;
            case PreparedEditKind.SetCompressed:
                if (formList.IsCompressed == prepared.BooleanValue)
                {
                    return false;
                }

                formList.IsCompressed = prepared.BooleanValue;
                return true;
            case PreparedEditKind.SetDeleted:
                if (formList.IsDeleted == prepared.BooleanValue)
                {
                    return false;
                }

                formList.IsDeleted = prepared.BooleanValue;
                return true;
            case PreparedEditKind.ReplaceItems:
                return ReplaceItems(formList, prepared.Items);
            case PreparedEditKind.InsertItem:
                formList.Items.Insert(
                    prepared.FirstIndex,
                    new FormLink<IFallout4MajorRecordGetter>(prepared.Items[0]));
                return true;
            case PreparedEditKind.RemoveItem:
                formList.Items.RemoveAt(prepared.FirstIndex);
                return true;
            case PreparedEditKind.MoveItem:
                if (prepared.FirstIndex == prepared.SecondIndex)
                {
                    return false;
                }

                var beforeItems = formList.Items.Select(existing => existing.FormKey).ToArray();
                var afterItems = beforeItems.ToList();
                var movedFormKey = afterItems[prepared.FirstIndex];
                afterItems.RemoveAt(prepared.FirstIndex);
                afterItems.Insert(prepared.SecondIndex, movedFormKey);
                if (beforeItems.SequenceEqual(afterItems))
                {
                    return false;
                }

                var item = formList.Items[prepared.FirstIndex];
                formList.Items.RemoveAt(prepared.FirstIndex);
                formList.Items.Insert(prepared.SecondIndex, item);
                return true;
            case PreparedEditKind.ClearItems:
                if (formList.Items.Count == 0)
                {
                    return false;
                }

                formList.Items.Clear();
                return true;
            case PreparedEditKind.SetName:
                var name = new TranslatedString(prepared.NameLanguage, prepared.Translations);
                if (RecordSemanticComparer.TranslatedStringEquals(formList.Name, name, CancellationToken.None))
                {
                    return false;
                }

                formList.Name = name;
                return true;
            case PreparedEditKind.ClearName:
                if (formList.Name is null)
                {
                    return false;
                }

                formList.Name = null;
                return true;
            case PreparedEditKind.SetMajorRecordFlags:
                var beforeFlags = formList.MajorRecordFlagsRaw;
                var unrelatedRawBits = beforeFlags & ~unchecked((int)GetSupportedMajorRecordFlagBits());
                formList.Fallout4MajorRecordFlags = prepared.MajorRecordFlags;
                formList.MajorRecordFlagsRaw |= unrelatedRawBits;
                return beforeFlags != formList.MajorRecordFlagsRaw;
            default:
                throw new InvalidOperationException("A validated Fallout 4 prepared edit has an unsupported internal command kind.");
        }
    }

    /// <summary>Replaces the entire ordered Mutagen item list while preserving nulls and duplicates.</summary>
    /// <param name="formList">The mutable target FormList.</param>
    /// <param name="items">The exact desired FormKey sequence.</param>
    /// <returns><see langword="true"/> when the Mutagen sequence changed.</returns>
    private static bool ReplaceItems(FormList formList, IReadOnlyList<FormKey> items)
    {
        if (formList.Items.Count == items.Count
            && formList.Items.Select(item => item.FormKey).SequenceEqual(items))
        {
            return false;
        }

        formList.Items.Clear();
        foreach (var item in items)
        {
            formList.Items.Add(new FormLink<IFallout4MajorRecordGetter>(item));
        }

        return true;
    }

    /// <summary>Enumerates every Mutagen family to enforce unique output identity.</summary>
    /// <param name="mod">The complete staged output.</param>
    /// <param name="formKey">The exact identity to locate.</param>
    /// <returns>All matching records in Mutagen enumeration order.</returns>
    private static IReadOnlyList<IMajorRecordGetter> FindRecords(IFallout4ModGetter mod, FormKey formKey)
    {
        return Array.AsReadOnly(mod.EnumerateMajorRecords()
            .Where(record => record.FormKey == formKey)
            .ToArray());
    }

    /// <summary>Creates a typed Mutagen mutation failure with stable source metadata.</summary>
    /// <param name="sources">The immutable source lifetime.</param>
    /// <param name="code">The stable failure category.</param>
    /// <param name="message">The actionable failure detail.</param>
    /// <returns>A failed Mutagen mutation result.</returns>
    private static EngineResult<RecordEditMutationResult> MutationFailure(
        Fallout4PluginSourceSet sources,
        EngineErrorCode code,
        string message)
    {
        return MutationFailure(sources, new EngineError(code, message));
    }

    /// <summary>Propagates a typed Mutagen mutation failure with stable source metadata.</summary>
    /// <param name="sources">The immutable source lifetime.</param>
    /// <param name="error">The complete typed failure.</param>
    /// <returns>A failed Mutagen mutation result.</returns>
    private static EngineResult<RecordEditMutationResult> MutationFailure(
        Fallout4PluginSourceSet sources,
        EngineError error)
    {
        return EngineResult<RecordEditMutationResult>.Failure(
            error,
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision);
    }

}
