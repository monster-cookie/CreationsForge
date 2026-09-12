using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace CreationsForge.Skyrim.Native;

/// <summary>Applies bounded typed Skyrim FormList edits to unpublished complete output candidates.</summary>
public sealed partial class SkyrimNativeEditService
{
    /// <summary>Applies one valid immutable payload to an unpublished complete Skyrim output candidate.</summary>
    /// <param name="sources">The borrowed immutable native source set used to validate new links.</param>
    /// <param name="candidate">The unpublished complete native output candidate.</param>
    /// <param name="targetFormKey">The exact staged Skyrim FormList identity.</param>
    /// <param name="preparedEdit">The immutable Skyrim-prepared payload.</param>
    /// <returns>Whether the command changed the immediate candidate, or a typed rejection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    public EngineResult<NativeEditMutationResult> ApplyEdit(
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputState candidate,
        FormKey targetFormKey,
        PreparedFormListEdit preparedEdit)
    {
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(preparedEdit);
        if (targetFormKey.IsNull)
        {
            return MutationFailure(
                EngineErrorCode.InvalidRequest,
                "A Skyrim FormList edit target cannot be the null FormKey.");
        }

        if (preparedEdit is not SkyrimPreparedFormListEdit edit)
        {
            return MutationFailure(
                EngineErrorCode.UnsupportedOperation,
                "Skyrim UnsupportedGameField: the prepared command belongs to another game adapter.");
        }

        if (!edit.IsValid)
        {
            return EngineResult<NativeEditMutationResult>.Failure(edit.Error!);
        }

        try
        {
            var targetResult = FindRecord(candidate.GetMutableMod(), targetFormKey);
            if (!targetResult.Succeeded)
            {
                return EngineResult<NativeEditMutationResult>.Failure(targetResult.Error!);
            }

            if (targetResult.Value is not FormList target)
            {
                return MutationFailure(
                    EngineErrorCode.ValidationFailed,
                    $"Native output identity '{targetFormKey}' is not a mutable Skyrim FormList.");
            }

            return edit.Kind switch
            {
                SkyrimPreparedEditKind.SetEditorId => ApplyEditorId(target, edit.EditorId),
                SkyrimPreparedEditKind.ClearEditorId => ApplyEditorId(target, editorId: null),
                SkyrimPreparedEditKind.ReplaceItems => ApplyReplaceItems(sources, candidate, target, edit.Items),
                SkyrimPreparedEditKind.InsertItem => ApplyInsertItem(sources, candidate, target, edit.Index, edit.Item),
                SkyrimPreparedEditKind.RemoveItem => ApplyRemoveItem(target, edit.Index),
                SkyrimPreparedEditKind.ClearItems => ApplyClearItems(target),
                SkyrimPreparedEditKind.MoveItem => ApplyMoveItem(target, edit.Index, edit.DestinationIndex),
                SkyrimPreparedEditKind.SetVersionControl => ApplyVersionControl(target, edit.UnsignedValue),
                SkyrimPreparedEditKind.SetFormVersion => ApplyFormVersion(target, checked((ushort)edit.UnsignedValue)),
                SkyrimPreparedEditKind.SetVersion2 => ApplyVersion2(target, checked((ushort)edit.UnsignedValue)),
                SkyrimPreparedEditKind.SetCompressed => ApplyCompressed(target, edit.BooleanValue),
                SkyrimPreparedEditKind.SetDeleted => ApplyDeleted(target, edit.BooleanValue),
                SkyrimPreparedEditKind.SetMajorRecordFlags => ApplyMajorRecordFlags(target, edit.MajorRecordFlags),
                _ => MutationFailure(
                    EngineErrorCode.UnsupportedOperation,
                    "Skyrim UnsupportedGameField: the prepared command is not supported by this adapter.")
            };
        }
        catch (ObjectDisposedException exception)
        {
            return MutationFailure(EngineErrorCode.WorkspaceDisposed, exception.Message);
        }
        catch (Exception exception)
        {
            return MutationFailure(
                EngineErrorCode.ValidationFailed,
                $"Skyrim rejected the native FormList edit: {exception.Message}");
        }
    }

    /// <summary>Applies an exact nullable EditorID and reports immediate native change.</summary>
    /// <param name="target">The mutable native target.</param>
    /// <param name="editorId">The exact replacement, or <see langword="null"/> to clear.</param>
    /// <returns>The successful immediate mutation result.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyEditorId(FormList target, string? editorId)
    {
        var before = target.EditorID;
        target.EditorID = editorId;
        return MutationSuccess(!string.Equals(before, target.EditorID, StringComparison.Ordinal));
    }

    /// <summary>Validates and replaces the complete ordered FormList item collection atomically.</summary>
    /// <param name="sources">The immutable source set used for new-link resolution.</param>
    /// <param name="candidate">The unpublished output used for staged-output-first resolution.</param>
    /// <param name="target">The mutable native FormList target.</param>
    /// <param name="items">The complete copied ordered identities.</param>
    /// <returns>The immediate mutation result or first typed link failure.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyReplaceItems(
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputState candidate,
        FormList target,
        IReadOnlyList<FormKey> items)
    {
        var retainedCounts = new Dictionary<FormKey, int>();
        foreach (var existingItem in target.Items)
        {
            var existingFormKey = GetFormKey(existingItem);
            retainedCounts[existingFormKey] = retainedCounts.GetValueOrDefault(existingFormKey) + 1;
        }

        foreach (var item in items)
        {
            var retainedCount = retainedCounts.GetValueOrDefault(item);
            if (retainedCount > 0)
            {
                retainedCounts[item] = retainedCount - 1;
                continue;
            }

            var validation = ValidateNewLink(sources, candidate.GetMutableMod(), item);
            if (!validation.Succeeded)
            {
                return validation;
            }
        }

        var changed = target.Items.Count != items.Count;
        if (!changed)
        {
            for (var index = 0; index < items.Count; index++)
            {
                if (GetFormKey(target.Items[index]) != items[index])
                {
                    changed = true;
                    break;
                }
            }
        }

        if (!changed)
        {
            return MutationSuccess(changed: false);
        }

        target.Items.Clear();
        foreach (var item in items)
        {
            target.Items.Add(new FormLink<ISkyrimMajorRecordGetter>(item));
        }

        return MutationSuccess(changed: true);
    }

    /// <summary>Validates and inserts one ordered item without deduplication.</summary>
    /// <param name="sources">The immutable source set used for new-link resolution.</param>
    /// <param name="candidate">The unpublished output used for staged-output-first resolution.</param>
    /// <param name="target">The mutable native FormList target.</param>
    /// <param name="index">The exact zero-based insertion position.</param>
    /// <param name="item">The copied native identity, including intentional null.</param>
    /// <returns>The changed result or a typed position or link failure.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyInsertItem(
        SkyrimNativeSourceSet sources,
        SkyrimNativeOutputState candidate,
        FormList target,
        int index,
        FormKey item)
    {
        if (index > target.Items.Count)
        {
            return InvalidPosition(index, target.Items.Count, allowEnd: true);
        }

        var validation = ValidateNewLink(sources, candidate.GetMutableMod(), item);
        if (!validation.Succeeded)
        {
            return validation;
        }

        target.Items.Insert(index, new FormLink<ISkyrimMajorRecordGetter>(item));
        return MutationSuccess(changed: true);
    }

    /// <summary>Removes one exact item position.</summary>
    /// <param name="target">The mutable native FormList target.</param>
    /// <param name="index">The exact zero-based removal position.</param>
    /// <returns>The changed result or a typed position failure.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyRemoveItem(FormList target, int index)
    {
        if (index >= target.Items.Count)
        {
            return InvalidPosition(index, target.Items.Count, allowEnd: false);
        }

        target.Items.RemoveAt(index);
        return MutationSuccess(changed: true);
    }

    /// <summary>Clears every item while retaining the FormList record.</summary>
    /// <param name="target">The mutable native FormList target.</param>
    /// <returns>Whether any item was removed.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyClearItems(FormList target)
    {
        var changed = target.Items.Count != 0;
        if (changed)
        {
            target.Items.Clear();
        }

        return MutationSuccess(changed);
    }

    /// <summary>Moves one item to its exact final position while preserving every other position.</summary>
    /// <param name="target">The mutable native FormList target.</param>
    /// <param name="sourceIndex">The zero-based position before removal.</param>
    /// <param name="destinationIndex">The zero-based position after the completed move.</param>
    /// <returns>Whether the ordered collection changed, or a typed position failure.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyMoveItem(
        FormList target,
        int sourceIndex,
        int destinationIndex)
    {
        if (sourceIndex >= target.Items.Count)
        {
            return InvalidPosition(sourceIndex, target.Items.Count, allowEnd: false);
        }

        if (destinationIndex >= target.Items.Count)
        {
            return InvalidPosition(destinationIndex, target.Items.Count, allowEnd: false);
        }

        if (sourceIndex == destinationIndex)
        {
            return MutationSuccess(changed: false);
        }

        var resultingKeys = target.Items.Select(GetFormKey).ToList();
        var resultingItem = resultingKeys[sourceIndex];
        resultingKeys.RemoveAt(sourceIndex);
        resultingKeys.Insert(destinationIndex, resultingItem);
        if (resultingKeys.SequenceEqual(target.Items.Select(GetFormKey)))
        {
            return MutationSuccess(changed: false);
        }

        var item = target.Items[sourceIndex];
        target.Items.RemoveAt(sourceIndex);
        target.Items.Insert(destinationIndex, item);
        return MutationSuccess(changed: true);
    }

    /// <summary>Sets native version-control state and reports its immediate typed change.</summary>
    /// <param name="target">The mutable native FormList target.</param>
    /// <param name="value">The exact unsigned replacement.</param>
    /// <returns>The successful immediate mutation result.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyVersionControl(FormList target, uint value)
    {
        var before = target.VersionControl;
        target.VersionControl = value;
        return MutationSuccess(before != target.VersionControl);
    }

    /// <summary>Sets native form-version state and reports its immediate typed change.</summary>
    /// <param name="target">The mutable native FormList target.</param>
    /// <param name="value">The exact unsigned replacement.</param>
    /// <returns>The successful immediate mutation result.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyFormVersion(FormList target, ushort value)
    {
        var before = target.FormVersion;
        target.FormVersion = value;
        return MutationSuccess(before != target.FormVersion);
    }

    /// <summary>Sets native secondary-version state and reports its immediate typed change.</summary>
    /// <param name="target">The mutable native FormList target.</param>
    /// <param name="value">The exact unsigned replacement.</param>
    /// <returns>The successful immediate mutation result.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyVersion2(FormList target, ushort value)
    {
        var before = target.Version2;
        target.Version2 = value;
        return MutationSuccess(before != target.Version2);
    }

    /// <summary>Sets native compression state and reports its immediate typed change.</summary>
    /// <param name="target">The mutable native FormList target.</param>
    /// <param name="value">Whether the native record must be compressed.</param>
    /// <returns>The successful immediate mutation result.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyCompressed(FormList target, bool value)
    {
        var before = target.IsCompressed;
        target.IsCompressed = value;
        return MutationSuccess(before != target.IsCompressed);
    }

    /// <summary>Sets native deletion state and reports its immediate typed change.</summary>
    /// <param name="target">The mutable native FormList target.</param>
    /// <param name="value">Whether the native record must be deleted.</param>
    /// <returns>The successful immediate mutation result.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyDeleted(FormList target, bool value)
    {
        var before = target.IsDeleted;
        target.IsDeleted = value;
        return MutationSuccess(before != target.IsDeleted);
    }

    /// <summary>Sets only Skyrim's typed major-record flags and retains any unrelated raw header bits.</summary>
    /// <param name="target">The mutable native FormList target.</param>
    /// <param name="value">The validated supported Skyrim flag combination.</param>
    /// <returns>Whether the full native raw flag field changed.</returns>
    private static EngineResult<NativeEditMutationResult> ApplyMajorRecordFlags(
        FormList target,
        SkyrimMajorRecord.SkyrimMajorRecordFlag value)
    {
        var before = target.MajorRecordFlagsRaw;
        var unrelatedRawBits = before & ~unchecked((int)SupportedMajorRecordFlags);
        target.SkyrimMajorRecordFlags = value;
        target.MajorRecordFlagsRaw |= unrelatedRawBits;
        return MutationSuccess(before != target.MajorRecordFlagsRaw);
    }

    /// <summary>Validates one newly introduced reference against staged output first and the source winner second.</summary>
    /// <param name="sources">The immutable native source set.</param>
    /// <param name="candidate">The complete unpublished native output.</param>
    /// <param name="formKey">The linked native identity, including intentional null.</param>
    /// <returns>A successful unchanged validation result or a typed missing/deleted/unsupported failure.</returns>
    private static EngineResult<NativeEditMutationResult> ValidateNewLink(
        SkyrimNativeSourceSet sources,
        ISkyrimModGetter candidate,
        FormKey formKey)
    {
        if (formKey.IsNull)
        {
            return MutationSuccess(changed: false);
        }

        var resolution = ResolveLink(sources, candidate, formKey);
        if (!resolution.Succeeded)
        {
            return EngineResult<NativeEditMutationResult>.Failure(resolution.Error!);
        }

        return resolution.Value switch
        {
            ReferenceResolutionStatus.Resolved => MutationSuccess(changed: false),
            ReferenceResolutionStatus.Unresolved => MutationFailure(
                EngineErrorCode.ValidationFailed,
                $"Skyrim FormList item reference '{formKey}' was not found in staged output or the native source winner."),
            ReferenceResolutionStatus.Deleted => MutationFailure(
                EngineErrorCode.ValidationFailed,
                $"Skyrim FormList item reference '{formKey}' resolves to a deleted native record."),
            _ => MutationFailure(
                EngineErrorCode.ValidationFailed,
                $"Skyrim FormList item reference '{formKey}' resolved as unsupported status {resolution.Value}.")
        };
    }

    /// <summary>Resolves one link against a singular staged record before consulting source winners.</summary>
    /// <param name="sources">The immutable native source set.</param>
    /// <param name="candidate">The complete current native output.</param>
    /// <param name="formKey">The non-null linked identity.</param>
    /// <returns>The live, deleted, or missing resolution status, or a typed duplicate-state failure.</returns>
    private static EngineResult<ReferenceResolutionStatus> ResolveLink(
        SkyrimNativeSourceSet sources,
        ISkyrimModGetter candidate,
        FormKey formKey)
    {
        var stagedResult = FindRecord(candidate, formKey);
        if (stagedResult.Succeeded)
        {
            return EngineResult<ReferenceResolutionStatus>.Success(
                stagedResult.Value!.IsDeleted
                    ? ReferenceResolutionStatus.Deleted
                    : ReferenceResolutionStatus.Resolved);
        }

        if (stagedResult.Error?.Code != EngineErrorCode.RecordNotFound)
        {
            return EngineResult<ReferenceResolutionStatus>.Failure(stagedResult.Error!);
        }

        var sourceResult = sources.Resolve(new ReferenceRequest(formKey, RecordScope.WinningOverrides));
        return sourceResult.Succeeded && sourceResult.Value is not null
            ? EngineResult<ReferenceResolutionStatus>.Success(sourceResult.Value.Status, warnings: sourceResult.Warnings)
            : EngineResult<ReferenceResolutionStatus>.Failure(
                sourceResult.Error
                    ?? new EngineError(EngineErrorCode.ValidationFailed, $"Skyrim reference '{formKey}' could not be resolved."),
                warnings: sourceResult.Warnings);
    }

    /// <summary>Finds exactly one native output record across every materialized record family.</summary>
    /// <param name="mod">The complete current or original native output.</param>
    /// <param name="formKey">The exact native identity.</param>
    /// <returns>The singular native record or a typed missing or duplicate failure.</returns>
    private static EngineResult<IMajorRecordGetter> FindRecord(ISkyrimModGetter mod, FormKey formKey)
    {
        IMajorRecordGetter? match = null;
        foreach (var record in mod.EnumerateMajorRecords())
        {
            if (record.FormKey != formKey)
            {
                continue;
            }

            if (match is not null)
            {
                return EngineResult<IMajorRecordGetter>.Failure(new EngineError(
                    EngineErrorCode.ValidationFailed,
                    $"Complete Skyrim output contains duplicate native FormKey '{formKey}'."));
            }

            match = record;
        }

        return match is null
            ? EngineResult<IMajorRecordGetter>.Failure(new EngineError(
                EngineErrorCode.RecordNotFound,
                $"Native output record '{formKey}' was not found."))
            : EngineResult<IMajorRecordGetter>.Success(match);
    }

    /// <summary>Reads an exact nullable FormKey from a native link without losing intentional null state.</summary>
    /// <param name="link">The native Skyrim major-record link.</param>
    /// <returns>The linked identity or <see cref="FormKey.Null"/>.</returns>
    private static FormKey GetFormKey(IFormLinkGetter link)
    {
        return link.FormKeyNullable ?? FormKey.Null;
    }

    /// <summary>Creates a successful immediate native mutation result.</summary>
    /// <param name="changed">Whether the command changed the candidate relative to its immediate prior state.</param>
    /// <returns>The successful typed mutation result.</returns>
    private static EngineResult<NativeEditMutationResult> MutationSuccess(bool changed)
    {
        return EngineResult<NativeEditMutationResult>.Success(new NativeEditMutationResult(changed));
    }

    /// <summary>Creates a typed native mutation failure.</summary>
    /// <param name="code">The stable engine failure category.</param>
    /// <param name="message">The precise Skyrim failure description.</param>
    /// <returns>The failed mutation result.</returns>
    private static EngineResult<NativeEditMutationResult> MutationFailure(
        EngineErrorCode code,
        string message)
    {
        return EngineResult<NativeEditMutationResult>.Failure(new EngineError(code, message));
    }

    /// <summary>Creates a typed invalid item-position failure.</summary>
    /// <param name="index">The rejected zero-based position.</param>
    /// <param name="count">The current item count.</param>
    /// <param name="allowEnd">Whether a position equal to count would have been accepted.</param>
    /// <returns>The failed mutation result.</returns>
    private static EngineResult<NativeEditMutationResult> InvalidPosition(
        int index,
        int count,
        bool allowEnd)
    {
        var range = allowEnd ? $"0 through {count}" : $"0 through {count - 1}";
        return MutationFailure(
            EngineErrorCode.ValidationFailed,
            $"Skyrim FormList item position {index} is outside the valid range {range}.");
    }
}
