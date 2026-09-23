using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Engine.Records;

/// <summary>Provides family-independent record create, exact override, read, compare, and atomic apply operations for one workspace.</summary>
public sealed class RecordEditor
{
    private readonly object _mutationGate = new();
    private readonly PluginWorkspace _workspace;
    private readonly IReadOnlyDictionary<string, RecordFamily> _families;

    internal RecordEditor(PluginWorkspace workspace, IEnumerable<RecordFamily> families)
    {
        _workspace = workspace;
        try
        {
            _families = families.ToDictionary(family => family.Descriptor.FamilyId, StringComparer.Ordinal);
        }
        catch (ArgumentException exception)
        {
            throw new ArgumentException("Each record family identifier must be unique for a game integration.", nameof(families), exception);
        }
    }

    /// <summary>Gets the concrete first-tranche family descriptors for this workspace's game.</summary>
    public IReadOnlyList<RecordFamilyDescriptor> Families => _families.Values
        .Select(family => family.Descriptor)
        .OrderBy(descriptor => descriptor.FamilyId, StringComparer.Ordinal)
        .ToArray();

    /// <summary>Reads registered fields from one exact record version.</summary>
    /// <param name="locator">The exact family, origin identity, and containing plugin.</param>
    /// <returns>A transient projection of registered fields.</returns>
    public RecordSnapshot Read(RecordLocator locator)
    {
        ArgumentNullException.ThrowIfNull(locator);
        _workspace.ThrowIfDisposed();
        var family = GetFamily(locator.FamilyId);
        var record = ResolveExactRecord(family, locator.FormKey, locator.ContainingModKey);
        return CreateSnapshot(family, record, locator.ContainingModKey);
    }

    /// <summary>Compares registered fields from two exact versions of the same declared family.</summary>
    /// <param name="left">The left exact record locator.</param>
    /// <param name="right">The right exact record locator.</param>
    /// <returns>Both transient snapshots and descriptor-ordered differences.</returns>
    public RecordComparison Compare(RecordLocator left, RecordLocator right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);
        if (!string.Equals(left.FamilyId, right.FamilyId, StringComparison.Ordinal))
        {
            throw new RecordEditingException($"Cannot compare declared families '{left.FamilyId}' and '{right.FamilyId}'.");
        }

        var leftSnapshot = Read(left);
        var rightSnapshot = Read(right);
        var family = GetFamily(left.FamilyId);
        var differences = new List<RecordDifference>();
        foreach (var field in family.Descriptor.Fields)
        {
            var leftValue = leftSnapshot.Values[field.Path];
            var rightValue = rightSnapshot.Values[field.Path];
            if (!RecordValueComparer.Equals(leftValue, rightValue))
            {
                differences.Add(new RecordDifference(field.Path, leftValue, rightValue));
            }
        }

        return new RecordComparison(leftSnapshot, rightSnapshot, differences);
    }

    /// <summary>Validates and atomically publishes one record change set with exactly one workspace revision increment.</summary>
    /// <param name="changeSet">The caller's expected revision and ordered Mutagen mutations.</param>
    /// <returns>The resulting revision and published output snapshots.</returns>
    /// <exception cref="RecordEditingException">Thrown when validation, context resolution, Mutagen mutation, publication, or rollback fails.</exception>
    public RecordApplyResult Apply(RecordChangeSet changeSet)
    {
        ArgumentNullException.ThrowIfNull(changeSet);
        lock (_mutationGate)
        {
            _workspace.ThrowIfDisposed();
            if (_workspace.State.Revision != changeSet.ExpectedRevision)
            {
                throw new RecordEditingException($"Workspace revision is '{_workspace.State.Revision}', not expected '{changeSet.ExpectedRevision}'. Refresh before applying changes.");
            }

            if (_workspace.State.Revision == ulong.MaxValue)
            {
                throw new RecordEditingException("The workspace revision cannot be incremented beyond UInt64.MaxValue.");
            }

            var resolved = changeSet.Mutations
                .Select(mutation => (Mutation: mutation, Family: GetFamily(mutation.FamilyId)))
                .ToArray();
            foreach (var item in resolved)
            {
                item.Family.ValidateChanges(item.Mutation.Changes);
            }
            ValidateReferences(resolved);

            var initialNextFormId = _workspace.MutableOutput.NextFormID;
            PreparedMutation[] prepared;
            try
            {
                prepared = resolved.Select(Prepare).ToArray();
                var duplicateIdentity = prepared
                    .GroupBy(item => item.Candidate.FormKey)
                    .FirstOrDefault(group => group.Count() > 1);
                if (duplicateIdentity is not null)
                {
                    throw new RecordEditingException($"Change set targets record identity '{duplicateIdentity.Key}' more than once.");
                }

                foreach (var item in prepared)
                {
                    item.Family.ApplyChanges(item.Candidate, item.Mutation.Changes);
                }
            }
            catch
            {
                _workspace.MutableOutput.NextFormID = initialNextFormId;
                throw;
            }

            PublishWithRollback(prepared, initialNextFormId);
            _workspace.MarkOutputChanged();
            var snapshots = prepared
                .Select(item => CreateSnapshot(item.Family, item.Candidate, _workspace.Output.ModKey))
                .ToArray();
            return new RecordApplyResult(_workspace.State.Revision, snapshots);
        }
    }

    private PreparedMutation Prepare((RecordMutation Mutation, RecordFamily Family) item)
    {
        IMajorRecord candidate;
        switch (item.Mutation.Kind)
        {
            case RecordMutationKind.Create:
                candidate = item.Family.CreateCandidate(_workspace.MutableOutput);
                break;
            case RecordMutationKind.Override:
                if (item.Mutation.FormKey is not { } formKey || item.Mutation.ContainingModKey is not { } containingModKey)
                {
                    throw new RecordEditingException($"Override mutation for family '{item.Mutation.FamilyId}' is missing its exact record context.");
                }

                candidate = item.Family.CopyCandidate(ResolveExactRecord(item.Family, formKey, containingModKey));
                break;
            default:
                throw new RecordEditingException($"Unknown record mutation kind '{item.Mutation.Kind}'.");
        }

        var currentOutput = item.Family.FindOutputRecord(_workspace.MutableOutput, candidate.FormKey);
        var rollback = currentOutput is null ? null : item.Family.CopyCandidate(currentOutput);
        return new PreparedMutation(item.Mutation, item.Family, candidate, rollback);
    }

    private void PublishWithRollback(IReadOnlyList<PreparedMutation> prepared, uint initialNextFormId)
    {
        var published = new List<PreparedMutation>(prepared.Count);
        try
        {
            foreach (var item in prepared)
            {
                item.Family.Publish(_workspace.MutableOutput, item.Candidate);
                published.Add(item);
            }
        }
        catch (Exception publicationException)
        {
            var rollbackExceptions = new List<Exception>();
            for (var index = published.Count - 1; index >= 0; index--)
            {
                var item = published[index];
                try
                {
                    if (item.Rollback is null)
                    {
                        item.Family.Remove(_workspace.MutableOutput, item.Candidate.FormKey);
                    }
                    else
                    {
                        item.Family.Publish(_workspace.MutableOutput, item.Rollback);
                    }
                }
                catch (Exception rollbackException)
                {
                    rollbackExceptions.Add(rollbackException);
                }
            }

            if (rollbackExceptions.Count > 0)
            {
                throw new RecordEditingException(
                    "Record publication failed and rollback also failed; the in-memory output may be partially changed.",
                    new AggregateException([publicationException, .. rollbackExceptions]));
            }

            _workspace.MutableOutput.NextFormID = initialNextFormId;
            throw new RecordEditingException("Record publication failed; the prior in-memory output was restored.", publicationException);
        }
    }

    private void ValidateReferences(IReadOnlyList<(RecordMutation Mutation, RecordFamily Family)> resolved)
    {
        foreach (var item in resolved)
        {
            foreach (var reference in item.Family.GetReferences(item.Mutation.Changes))
            {
                var resolvesInWorkspace = reference.TargetTypes.Any(type =>
                    _workspace.LinkCache.TryResolve(reference.FormKey, type, out _, ResolveTarget.Winner));
                if (!resolvesInWorkspace)
                {
                    var targetNames = string.Join(", ", reference.TargetTypes.Select(type => type.Name));
                    throw new RecordEditingException($"FormKey reference '{reference.FormKey}' does not resolve to any allowed target ({targetNames}) in this workspace.");
                }
            }
        }
    }

    private IMajorRecordGetter ResolveExactRecord(RecordFamily family, FormKey formKey, ModKey containingModKey)
    {
        if (containingModKey == _workspace.Output.ModKey)
        {
            return family.FindOutputRecord(_workspace.MutableOutput, formKey)
                ?? throw new RecordEditingException($"Output '{containingModKey}' does not contain declared family '{family.Descriptor.FamilyId}' record '{formKey}'.");
        }

        try
        {
            var resolution = _workspace.ResolveRecord(formKey, family.GetterType, containingModKey);
            return resolution.ExactContext.Record as IMajorRecordGetter
                ?? throw new RecordEditingException($"Resolved context '{containingModKey}:{formKey}' is not a Mutagen major record.");
        }
        catch (RecordEditingException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new RecordEditingException($"Could not resolve exact '{family.Descriptor.FamilyId}' context '{containingModKey}:{formKey}': {exception.Message}", exception);
        }
    }

    private static RecordSnapshot CreateSnapshot(RecordFamily family, IMajorRecordGetter record, ModKey containingModKey)
    {
        return new RecordSnapshot(family.Descriptor.FamilyId, record.FormKey, containingModKey, family.ReadFields(record));
    }

    private RecordFamily GetFamily(string familyId)
    {
        return _families.TryGetValue(familyId, out var family)
            ? family
            : throw new RecordEditingException($"Record family '{familyId}' is not declared editable for {_workspace.State.Release}.");
    }

    private sealed class PreparedMutation
    {
        public PreparedMutation(
            RecordMutation mutation,
            RecordFamily family,
            IMajorRecord candidate,
            IMajorRecord? rollback)
        {
            Mutation = mutation;
            Family = family;
            Candidate = candidate;
            Rollback = rollback;
        }

        public RecordMutation Mutation { get; }

        public RecordFamily Family { get; }

        public IMajorRecord Candidate { get; }

        public IMajorRecord? Rollback { get; }
    }
}
