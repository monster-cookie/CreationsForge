using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Mcp;

/// <summary>Retains exact immutable Core metadata behind bounded host-lifetime opaque handles.</summary>
internal sealed class McpMetadataStore : IDisposable
{
    /// <summary>The production maximum for published and operation-reserved handle slots.</summary>
    internal const int DefaultMaximumHandles = 4096;

    /// <summary>The maximum number of handle slots one operation invocation may reserve provisionally.</summary>
    internal const int DefaultOperationReservationSize = 16;

    /// <summary>Synchronizes reservations, entries, origins, capacity, and disposal.</summary>
    private readonly object SyncRoot = new();

    /// <summary>The maximum number of published and reserved handle slots.</summary>
    private readonly int MaximumHandles;

    /// <summary>The maximum provisional reservation size for one operation invocation.</summary>
    private readonly int OperationReservationSize;

    /// <summary>Indexes retained entries by their opaque handle.</summary>
    private readonly Dictionary<string, MetadataEntry> EntriesByHandle = new(StringComparer.Ordinal);

    /// <summary>Reuses one handle for repeated publication of the same Core object reference.</summary>
    private readonly Dictionary<object, MetadataEntry> EntriesByReference = new(ReferenceEqualityComparer.Instance);

    /// <summary>Retains operation gates only while an invocation or waiter still owns them.</summary>
    private readonly Dictionary<OperationKey, OperationReservation> Operations = [];

    /// <summary>Counts published handles plus unused reserved slots.</summary>
    private int AllocatedSlotCount;

    /// <summary>Indicates that host shutdown released all retained references.</summary>
    private bool IsDisposed;

    /// <summary>Initializes a host-scoped metadata store.</summary>
    /// <param name="maximumHandles">The maximum published and reserved handle slots retained by this host.</param>
    /// <param name="operationReservationSize">The number of slots reserved for every first-seen operation identity.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a bound is non-positive or one reservation exceeds the host maximum.</exception>
    internal McpMetadataStore(
        int maximumHandles = DefaultMaximumHandles,
        int operationReservationSize = DefaultOperationReservationSize)
    {
        if (maximumHandles <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumHandles));
        }

        if (operationReservationSize <= 0 || operationReservationSize > maximumHandles)
        {
            throw new ArgumentOutOfRangeException(nameof(operationReservationSize));
        }

        MaximumHandles = maximumHandles;
        OperationReservationSize = operationReservationSize;
    }

    /// <summary>Gets the number of published metadata handles.</summary>
    internal int HandleCount
    {
        get
        {
            lock (SyncRoot)
            {
                return EntriesByHandle.Count;
            }
        }
    }

    /// <summary>Gets the number of published and unused reserved slots charged to the host bound.</summary>
    internal int AllocatedSlots
    {
        get
        {
            lock (SyncRoot)
            {
                return AllocatedSlotCount;
            }
        }
    }

    /// <summary>Provisionally reserves publication capacity and serializes every active invocation sharing its identity.</summary>
    /// <param name="workspaceId">The non-empty workspace identity used by the Core operation.</param>
    /// <param name="operationId">The non-empty idempotency identity used by the Core operation.</param>
    /// <param name="kind">The calling workflow, used only to enforce publication admission policy.</param>
    /// <param name="requiredPublicationSlots">The maximum number of distinct new Core references the invocation may publish.</param>
    /// <param name="cancellationToken">A token that cancels while waiting for the same operation identity.</param>
    /// <returns>An acquired operation lease, or <see langword="null"/> when capacity is unavailable before invocation.</returns>
    /// <exception cref="ArgumentException">Thrown when either identifier is empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the kind or required publication count is invalid.</exception>
    /// <exception cref="ObjectDisposedException">Thrown after host shutdown disposes the store.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed while waiting.</exception>
    internal async ValueTask<McpMetadataOperationLease?> AcquireOperationAsync(
        Guid workspaceId,
        Guid operationId,
        McpMetadataOperationKind kind,
        int requiredPublicationSlots,
        CancellationToken cancellationToken = default)
    {
        if (workspaceId == Guid.Empty || operationId == Guid.Empty)
        {
            throw new ArgumentException("Metadata operation admission requires non-empty workspace and operation identifiers.");
        }

        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind));
        }

        if (requiredPublicationSlots < 0 || requiredPublicationSlots > OperationReservationSize)
        {
            throw new ArgumentOutOfRangeException(nameof(requiredPublicationSlots));
        }

        OperationReservation reservation;
        lock (SyncRoot)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            var key = new OperationKey(workspaceId, operationId);
            if (!Operations.TryGetValue(key, out reservation!))
            {
                reservation = new OperationReservation(workspaceId, operationId, kind);
                Operations.Add(key, reservation);
            }

            reservation.AcquisitionCount++;
        }

        try
        {
            await reservation.Gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            ReleaseFailedAcquisition(reservation);
            throw;
        }

        lock (SyncRoot)
        {
            if (IsDisposed)
            {
                reservation.Gate.Release();
                throw new ObjectDisposedException(nameof(McpMetadataStore));
            }

            var availableSlots = MaximumHandles - AllocatedSlotCount;
            if (availableSlots < requiredPublicationSlots && !reservation.HasPublishedValues)
            {
                ReleaseAcquisitionUnderLock(reservation);
                reservation.Gate.Release();
                return null;
            }

            reservation.UnusedSlots = Math.Min(requiredPublicationSlots, availableSlots);
            AllocatedSlotCount += reservation.UnusedSlots;
        }

        return new McpMetadataOperationLease(this, reservation);
    }

    /// <summary>Releases provisional unused capacity and operation serialization for one completed invocation.</summary>
    /// <param name="reservation">The acquired operation reservation.</param>
    internal void ReleaseOperation(OperationReservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        lock (SyncRoot)
        {
            if (!IsDisposed)
            {
                AllocatedSlotCount -= reservation.UnusedSlots;
                reservation.UnusedSlots = 0;
                ReleaseAcquisitionUnderLock(reservation);
            }
        }

        reservation.Gate.Release();
    }

    /// <summary>Releases ownership recorded before a canceled gate wait.</summary>
    /// <param name="reservation">The reservation whose gate was not acquired.</param>
    private void ReleaseFailedAcquisition(OperationReservation reservation)
    {
        lock (SyncRoot)
        {
            if (!IsDisposed)
            {
                ReleaseAcquisitionUnderLock(reservation);
            }
        }
    }

    /// <summary>Removes an idle operation gate while the store lock is held.</summary>
    /// <param name="reservation">The reservation losing one active invocation or waiter.</param>
    private void ReleaseAcquisitionUnderLock(OperationReservation reservation)
    {
        reservation.AcquisitionCount--;
        if (reservation.AcquisitionCount == 0 && !reservation.HasPublishedValues)
        {
            Operations.Remove(new OperationKey(reservation.WorkspaceId, reservation.OperationId));
        }
    }

    /// <summary>Publishes current workspace metadata without retaining a separate workspace-state wrapper.</summary>
    /// <param name="workspaceId">The workspace from which the exact reference was read.</param>
    /// <param name="value">The exact immutable output association.</param>
    /// <param name="reference">Receives the stable host-local reference.</param>
    /// <returns><see langword="true"/> when an existing handle was reused or one unreserved slot was available.</returns>
    internal bool TryPublishWorkspaceMetadata(
        Guid workspaceId,
        OutputAssociation value,
        out McpMetadataReference reference)
    {
        return TryPublishUnreserved(workspaceId, value, McpMetadataKind.OutputAssociation, out reference);
    }

    /// <summary>Publishes a current workspace baseline without retaining a separate workspace-state wrapper.</summary>
    /// <param name="workspaceId">The workspace from which the exact reference was read.</param>
    /// <param name="value">The exact immutable complete output baseline.</param>
    /// <param name="reference">Receives the stable host-local reference.</param>
    /// <returns><see langword="true"/> when an existing handle was reused or one unreserved slot was available.</returns>
    internal bool TryPublishWorkspaceMetadata(
        Guid workspaceId,
        OutputArtifactSetBaseline value,
        out McpMetadataReference reference)
    {
        return TryPublishUnreserved(workspaceId, value, McpMetadataKind.OutputBaseline, out reference);
    }

    /// <summary>Atomically publishes selected and pending output metadata from one workspace-state snapshot.</summary>
    /// <param name="workspaceId">The workspace from which the atomic snapshot was read.</param>
    /// <param name="output">The selected output association, or <see langword="null"/> before selection.</param>
    /// <param name="baseline">The selected output baseline, or <see langword="null"/> before selection.</param>
    /// <param name="pendingOutput">The exact journaled output for a blocked save, or <see langword="null"/> when synchronization is ready.</param>
    /// <param name="outputReference">Receives the association handle, or <see langword="null"/> before selection.</param>
    /// <param name="baselineReference">Receives the baseline handle, or <see langword="null"/> before selection.</param>
    /// <param name="pendingOutputReference">Receives the exact pending-save output handle, or <see langword="null"/> when no save is pending.</param>
    /// <returns><see langword="true"/> when every reference was reused or published without exceeding the host bound.</returns>
    /// <exception cref="ArgumentException">Thrown when association and baseline nullability disagree.</exception>
    internal bool TryPublishWorkspaceStateMetadata(
        Guid workspaceId,
        OutputAssociation? output,
        OutputArtifactSetBaseline? baseline,
        OutputAssociation? pendingOutput,
        out McpMetadataReference? outputReference,
        out McpMetadataReference? baselineReference,
        out McpMetadataReference? pendingOutputReference)
    {
        if ((output is null) != (baseline is null))
        {
            throw new ArgumentException("Workspace state output and baseline metadata must be present or absent together.");
        }

        outputReference = null;
        baselineReference = null;
        pendingOutputReference = null;

        lock (SyncRoot)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            var unpublished = new HashSet<object>(ReferenceEqualityComparer.Instance);
            if (output is not null && !EntriesByReference.ContainsKey(output))
            {
                unpublished.Add(output);
            }

            if (baseline is not null && !EntriesByReference.ContainsKey(baseline))
            {
                unpublished.Add(baseline);
            }

            if (pendingOutput is not null && !EntriesByReference.ContainsKey(pendingOutput))
            {
                unpublished.Add(pendingOutput);
            }

            if (MaximumHandles - AllocatedSlotCount < unpublished.Count)
            {
                return false;
            }

            outputReference = output is null
                ? null
                : PublishWorkspaceValue(workspaceId, output, McpMetadataKind.OutputAssociation);
            baselineReference = baseline is null
                ? null
                : PublishWorkspaceValue(workspaceId, baseline, McpMetadataKind.OutputBaseline);
            pendingOutputReference = pendingOutput is null
                ? null
                : PublishWorkspaceValue(workspaceId, pendingOutput, McpMetadataKind.OutputAssociation);
            return true;
        }
    }

    /// <summary>Atomically publishes every exact reference needed to inspect one completed save result.</summary>
    /// <param name="workspaceId">The live workspace that returned the save outcome.</param>
    /// <param name="result">The complete immutable save outcome.</param>
    /// <param name="references">Receives the complete reference group, or an all-null group when new entries cannot fit.</param>
    /// <returns><see langword="true"/> when every required reference was reused or published.</returns>
    internal bool TryPublishSaveMetadata(
        Guid workspaceId,
        SaveResult result,
        out McpSaveMetadataReferences references)
    {
        ArgumentNullException.ThrowIfNull(result);
        lock (SyncRoot)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            var unpublished = new HashSet<object>(ReferenceEqualityComparer.Instance);
            AddUnpublished(result, unpublished);
            if (result.CommittedBaseline is not null)
            {
                AddUnpublished(result.CommittedBaseline, unpublished);
            }

            if (result.ResolvedEvidence is not null)
            {
                AddUnpublished(result.ResolvedEvidence, unpublished);
                AddUnpublished(result.ResolvedEvidence.Output, unpublished);
            }

            if (MaximumHandles - AllocatedSlotCount < unpublished.Count)
            {
                references = new McpSaveMetadataReferences(null, null, null, null);
                return false;
            }

            var committedBaseline = result.CommittedBaseline is null
                ? null
                : PublishWorkspaceValue(workspaceId, result.CommittedBaseline, McpMetadataKind.OutputBaseline);
            var resolvedEvidence = result.ResolvedEvidence is null
                ? null
                : PublishWorkspaceValue(workspaceId, result.ResolvedEvidence, McpMetadataKind.ResolvedOutputEvidence);
            var output = result.ResolvedEvidence is null
                ? null
                : PublishWorkspaceValue(workspaceId, result.ResolvedEvidence.Output, McpMetadataKind.OutputAssociation);
            var details = PublishWorkspaceValue(workspaceId, result, McpMetadataKind.SaveResult);
            references = new McpSaveMetadataReferences(committedBaseline, output, resolvedEvidence, details);
            return true;
        }
    }

    /// <summary>Resolves an exact output baseline issued for the selected workspace without comparing it to current revision state.</summary>
    /// <param name="handle">The opaque host-local handle.</param>
    /// <param name="workspaceId">The workspace whose prior result must have published the baseline.</param>
    /// <param name="value">Receives the exact retained Core baseline.</param>
    /// <returns><see langword="true"/> only for a known baseline handle with the expected workspace origin.</returns>
    internal bool TryResolveOutputBaseline(
        string handle,
        Guid workspaceId,
        out OutputArtifactSetBaseline value)
    {
        return TryResolve(handle, McpMetadataKind.OutputBaseline, workspaceId, out value);
    }

    /// <summary>Resolves exact terminal recovery evidence without substituting current workspace metadata.</summary>
    /// <param name="handle">The opaque host-local handle.</param>
    /// <param name="value">Receives the exact retained Core evidence.</param>
    /// <returns><see langword="true"/> only for a known resolved-evidence handle.</returns>
    internal bool TryResolveResolvedEvidence(string handle, out ResolvedOutputEvidence value)
    {
        lock (SyncRoot)
        {
            if (!IsDisposed &&
                EntriesByHandle.TryGetValue(handle, out var entry) &&
                entry.Kind == McpMetadataKind.ResolvedOutputEvidence)
            {
                value = (ResolvedOutputEvidence)entry.Value;
                return true;
            }
        }

        value = null!;
        return false;
    }

    /// <summary>Resolves any retained value for bounded metadata inspection.</summary>
    /// <param name="handle">The opaque host-local handle.</param>
    /// <param name="reference">Receives its stable descriptor.</param>
    /// <param name="value">Receives the exact retained Core object.</param>
    /// <returns><see langword="true"/> only when the handle belongs to this live host.</returns>
    internal bool TryResolve(string handle, out McpMetadataReference reference, out object value)
    {
        lock (SyncRoot)
        {
            if (!IsDisposed && EntriesByHandle.TryGetValue(handle, out var entry))
            {
                reference = entry.Reference;
                value = entry.Value;
                return true;
            }
        }

        reference = null!;
        value = null!;
        return false;
    }

    /// <summary>Releases every retained Core reference and operation gate at host shutdown.</summary>
    public void Dispose()
    {
        lock (SyncRoot)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            EntriesByHandle.Clear();
            EntriesByReference.Clear();
            Operations.Clear();
            AllocatedSlotCount = 0;
        }
    }

    /// <summary>Publishes one exact operation result object, consuming only its pre-reserved slot.</summary>
    /// <param name="reservation">The acquired operation reservation.</param>
    /// <param name="value">The exact immutable Core object.</param>
    /// <param name="kind">The object's closed metadata kind.</param>
    /// <returns>The stable metadata reference, or <see langword="null"/> when no reserved slot remains.</returns>
    internal McpMetadataReference? TryPublish(OperationReservation reservation, object value, McpMetadataKind kind)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        ArgumentNullException.ThrowIfNull(value);
        lock (SyncRoot)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            if (EntriesByReference.TryGetValue(value, out var existing))
            {
                existing.Origins.Add(new MetadataOrigin(reservation.WorkspaceId, reservation.OperationId));
                reservation.HasPublishedValues = true;
                return existing.Reference;
            }

            if (reservation.UnusedSlots == 0)
            {
                return null;
            }

            reservation.UnusedSlots--;
            reservation.HasPublishedValues = true;
            return AddEntry(value, kind, new MetadataOrigin(reservation.WorkspaceId, reservation.OperationId));
        }
    }

    /// <summary>Publishes one exact state-read reference using unreserved host capacity.</summary>
    /// <param name="workspaceId">The workspace that issued the reference.</param>
    /// <param name="value">The exact immutable Core object.</param>
    /// <param name="kind">The object's closed metadata kind.</param>
    /// <param name="reference">Receives the stable metadata reference.</param>
    /// <returns><see langword="true"/> when publication succeeds without exceeding the host bound.</returns>
    private bool TryPublishUnreserved(
        Guid workspaceId,
        object value,
        McpMetadataKind kind,
        out McpMetadataReference reference)
    {
        ArgumentNullException.ThrowIfNull(value);
        lock (SyncRoot)
        {
            ObjectDisposedException.ThrowIf(IsDisposed, this);
            if (EntriesByReference.TryGetValue(value, out var existing))
            {
                existing.Origins.Add(new MetadataOrigin(workspaceId, null));
                reference = existing.Reference;
                return true;
            }

            if (AllocatedSlotCount == MaximumHandles)
            {
                reference = null!;
                return false;
            }

            AllocatedSlotCount++;
            reference = AddEntry(value, kind, new MetadataOrigin(workspaceId, null));
            return true;
        }
    }

    /// <summary>Publishes or reuses one workspace-origin value while the store lock is held and capacity is known.</summary>
    /// <param name="workspaceId">The originating workspace.</param>
    /// <param name="value">The exact immutable Core reference.</param>
    /// <param name="kind">The closed metadata kind.</param>
    /// <returns>The stable host-local metadata descriptor.</returns>
    private McpMetadataReference PublishWorkspaceValue(Guid workspaceId, object value, McpMetadataKind kind)
    {
        if (EntriesByReference.TryGetValue(value, out var existing))
        {
            existing.Origins.Add(new MetadataOrigin(workspaceId, null));
            return existing.Reference;
        }

        AllocatedSlotCount++;
        return AddEntry(value, kind, new MetadataOrigin(workspaceId, null));
    }

    /// <summary>Adds one exact reference to a prospective atomic publication only when the store does not already retain it.</summary>
    /// <param name="value">The exact immutable Core reference.</param>
    /// <param name="unpublished">The reference-identity set receiving missing values.</param>
    private void AddUnpublished(object value, HashSet<object> unpublished)
    {
        if (!EntriesByReference.ContainsKey(value))
        {
            unpublished.Add(value);
        }
    }

    /// <summary>Resolves one typed workspace-origin reference.</summary>
    /// <typeparam name="T">The exact immutable Core type.</typeparam>
    /// <param name="handle">The opaque handle.</param>
    /// <param name="kind">The required metadata kind.</param>
    /// <param name="workspaceId">The required originating workspace.</param>
    /// <param name="value">Receives the retained reference.</param>
    /// <returns><see langword="true"/> when handle, kind, and workspace origin all match.</returns>
    private bool TryResolve<T>(string handle, McpMetadataKind kind, Guid workspaceId, out T value)
        where T : class
    {
        lock (SyncRoot)
        {
            if (!IsDisposed &&
                EntriesByHandle.TryGetValue(handle, out var entry) &&
                entry.Kind == kind &&
                entry.Origins.Any(origin => origin.WorkspaceId == workspaceId))
            {
                value = (T)entry.Value;
                return true;
            }
        }

        value = null!;
        return false;
    }

    /// <summary>Adds one new opaque entry after its slot has already been charged.</summary>
    /// <param name="value">The exact immutable Core reference.</param>
    /// <param name="kind">The object's closed metadata kind.</param>
    /// <param name="origin">The first observed workspace and optional operation origin.</param>
    /// <returns>The stable reference descriptor.</returns>
    private McpMetadataReference AddEntry(object value, McpMetadataKind kind, MetadataOrigin origin)
    {
        var handle = $"cfmd_{Guid.NewGuid():N}";
        Guid? baselineId = value is OutputArtifactSetBaseline baseline ? baseline.BaselineId : null;
        var reference = new McpMetadataReference(handle, kind, baselineId);
        var entry = new MetadataEntry(reference, value, origin);
        EntriesByHandle.Add(handle, entry);
        EntriesByReference.Add(value, entry);
        return reference;
    }

    /// <summary>Identifies one host operation admission independently of its calling tool.</summary>
    private readonly struct OperationKey : IEquatable<OperationKey>
    {
        /// <summary>Initializes one operation key.</summary>
        /// <param name="workspaceId">The original or live workspace identifier.</param>
        /// <param name="operationId">The Core idempotency identifier.</param>
        internal OperationKey(Guid workspaceId, Guid operationId)
        {
            WorkspaceId = workspaceId;
            OperationId = operationId;
        }

        /// <summary>Gets the original or live workspace identifier.</summary>
        internal Guid WorkspaceId { get; }

        /// <summary>Gets the Core idempotency identifier.</summary>
        internal Guid OperationId { get; }

        /// <inheritdoc />
        public bool Equals(OperationKey other)
        {
            return WorkspaceId == other.WorkspaceId && OperationId == other.OperationId;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is OperationKey other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(WorkspaceId, OperationId);
        }
    }

    /// <summary>Records one workspace and optional operation origin for typed handle validation.</summary>
    private readonly struct MetadataOrigin : IEquatable<MetadataOrigin>
    {
        /// <summary>Initializes one metadata origin.</summary>
        /// <param name="workspaceId">The workspace that issued the reference.</param>
        /// <param name="operationId">The operation that issued the reference, or <see langword="null"/> for a state read.</param>
        internal MetadataOrigin(Guid workspaceId, Guid? operationId)
        {
            WorkspaceId = workspaceId;
            OperationId = operationId;
        }

        /// <summary>Gets the workspace that issued the reference.</summary>
        internal Guid WorkspaceId { get; }

        /// <summary>Gets the operation that issued the reference, or <see langword="null"/> for a state read.</summary>
        internal Guid? OperationId { get; }

        /// <inheritdoc />
        public bool Equals(MetadataOrigin other)
        {
            return WorkspaceId == other.WorkspaceId && OperationId == other.OperationId;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is MetadataOrigin other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(WorkspaceId, OperationId);
        }
    }

    /// <summary>Owns the provisional slot reservation and invocation gate for one active operation identity.</summary>
    internal sealed class OperationReservation
    {
        /// <summary>Initializes a permanent operation reservation.</summary>
        /// <param name="workspaceId">The operation workspace identity.</param>
        /// <param name="operationId">The Core idempotency identity.</param>
        /// <param name="ownerKind">The workflow that first reserved this cross-tool operation identity.</param>
        internal OperationReservation(
            Guid workspaceId,
            Guid operationId,
            McpMetadataOperationKind ownerKind)
        {
            WorkspaceId = workspaceId;
            OperationId = operationId;
            OwnerKind = ownerKind;
        }

        /// <summary>Gets the workspace identity.</summary>
        internal Guid WorkspaceId { get; }

        /// <summary>Gets the Core operation identity.</summary>
        internal Guid OperationId { get; }

        /// <summary>Gets the workflow that first reserved this cross-tool operation identity.</summary>
        internal McpMetadataOperationKind OwnerKind { get; }

        /// <summary>Gets the gate serializing engine invocation and metadata publication for this identity.</summary>
        internal SemaphoreSlim Gate { get; } = new(1, 1);

        /// <summary>Gets or sets the number of unused slots retained for future exact results.</summary>
        internal int UnusedSlots { get; set; }

        /// <summary>Gets or sets the number of active invocations and waiters sharing this operation identity.</summary>
        internal int AcquisitionCount { get; set; }

        /// <summary>Gets or sets whether this operation published at least one retained value and remains replay-addressable.</summary>
        internal bool HasPublishedValues { get; set; }

    }

    /// <summary>Retains one exact value, its stable descriptor, and every workspace origin that observed it.</summary>
    private sealed class MetadataEntry
    {
        /// <summary>Initializes one retained metadata entry.</summary>
        /// <param name="reference">The stable opaque descriptor.</param>
        /// <param name="value">The exact immutable Core value.</param>
        /// <param name="origin">The first workspace and optional operation origin.</param>
        internal MetadataEntry(McpMetadataReference reference, object value, MetadataOrigin origin)
        {
            Reference = reference;
            Value = value;
            Origins = [origin];
        }

        /// <summary>Gets the stable opaque descriptor.</summary>
        internal McpMetadataReference Reference { get; }

        /// <summary>Gets the retained exact Core reference.</summary>
        internal object Value { get; }

        /// <summary>Gets every workspace and operation origin that published this exact reference.</summary>
        internal HashSet<MetadataOrigin> Origins { get; }

        /// <summary>Gets the closed retained metadata kind.</summary>
        internal McpMetadataKind Kind => Reference.Kind;
    }
}
