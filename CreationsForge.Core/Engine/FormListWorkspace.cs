using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.Internal;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Serilog;

namespace CreationsForge.Core.Engine;

/// <summary>
/// Owns one isolated native workspace and serializes reads, mutations, saves, and disposal through one gate.
/// </summary>
public sealed partial class FormListWorkspace : IFormListWorkspace
{
    /// <summary>Stores the canonical explicit inputs used to open this workspace.</summary>
    private readonly WorkspaceOpenRequest Request;

    /// <summary>Stores the selected game adapter for all native operations.</summary>
    private readonly IFormListGameAdapter Adapter;

    /// <summary>Stores the recoverable output-set save coordinator.</summary>
    private readonly IWorkspaceSaveCoordinator SaveCoordinator;

    /// <summary>Acquires exclusive output-directory leases for native open and recovery publication.</summary>
    private readonly IOutputDirectoryLeaseProvider OutputDirectoryLeaseProvider;

    /// <summary>Stores the structured diagnostic logger.</summary>
    private readonly ILogger Logger;

    /// <summary>Serializes all native operations and asynchronous disposal.</summary>
    private readonly SemaphoreSlim OperationGate = new(1, 1);

    /// <summary>Protects synchronous revision reads from concurrent mutation publication.</summary>
    private readonly object RevisionSync = new();

    /// <summary>Tracks immutable operation results for exact replay.</summary>
    private readonly OperationReplayStore ReplayStore;

    /// <summary>Creates canonical Core-owned operation fingerprints.</summary>
    private readonly OperationFingerprintFactory FingerprintFactory = new();

    /// <summary>Stores the immutable native source-set baseline established while opening.</summary>
    private readonly Guid SourceBaselineId;

    /// <summary>Tracks the native target of each staged edit identifier.</summary>
    private readonly Dictionary<Guid, NativeEditIdentity> Edits = new();

    /// <summary>Owns the native source/load-order/string lifetime until disposal.</summary>
    private INativeSourceSet? Sources;

    /// <summary>Owns the current complete mutable native output state when selected.</summary>
    private INativeOutputState? Output;

    /// <summary>Stores the selected canonical output association when present.</summary>
    private OutputAssociation? SelectedOutput;

    /// <summary>Stores the complete selected output-set baseline when present.</summary>
    private OutputArtifactSetBaseline? SelectedOutputBaseline;

    /// <summary>Stores the current exact native baseline composition and mutation sequence.</summary>
    private WorkspaceRevision CurrentRevision;

    /// <summary>Stores the atomic output synchronization state published with save and recovery transitions.</summary>
    private OutputSynchronizationState CurrentOutputSynchronization = new(
        OutputSynchronizationStatus.Ready,
        null);

    /// <summary>Tracks whether ownership has been released.</summary>
    private bool Disposed;

    /// <summary>
    /// Initializes an independently owned workspace and transfers ownership of the opened native source handle.
    /// </summary>
    /// <param name="request">The validated canonical open request.</param>
    /// <param name="adapter">The exact game and release adapter selected by the factory.</param>
    /// <param name="sourceOpenResult">The acquired native sources and adapter-derived deterministic baseline.</param>
    /// <param name="saveCoordinator">The recoverable multi-file save coordinator.</param>
    /// <param name="outputDirectoryLeaseProvider">The exclusive output-directory lease provider.</param>
    /// <param name="logger">The structured diagnostic logger.</param>
    /// <param name="operationReplayCapacity">The positive maximum number of replayable mutation results retained by this workspace.</param>
    internal FormListWorkspace(
        WorkspaceOpenRequest request,
        IFormListGameAdapter adapter,
        NativeSourceOpenResult sourceOpenResult,
        IWorkspaceSaveCoordinator saveCoordinator,
        IOutputDirectoryLeaseProvider outputDirectoryLeaseProvider,
        ILogger logger,
        int operationReplayCapacity = OperationReplayStore.DefaultMaximumEntryCount)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(adapter);
        ArgumentNullException.ThrowIfNull(sourceOpenResult);
        ArgumentNullException.ThrowIfNull(saveCoordinator);
        ArgumentNullException.ThrowIfNull(outputDirectoryLeaseProvider);
        ArgumentNullException.ThrowIfNull(logger);
        Request = request;
        Adapter = adapter;
        SaveCoordinator = saveCoordinator;
        OutputDirectoryLeaseProvider = outputDirectoryLeaseProvider;
        Logger = logger;
        ReplayStore = new OperationReplayStore(operationReplayCapacity);
        Sources = sourceOpenResult.Sources;
        SourceBaselineId = sourceOpenResult.BaselineId;
        CurrentRevision = new WorkspaceRevision(SourceBaselineId, 0);
    }

    /// <inheritdoc />
    public Guid WorkspaceId => Request.WorkspaceId;

    /// <inheritdoc />
    public WorkspaceRevision Revision
    {
        get
        {
            lock (RevisionSync)
            {
                return CurrentRevision;
            }
        }
    }

    /// <inheritdoc />
    public OutputSynchronizationState OutputSynchronization
    {
        get
        {
            lock (RevisionSync)
            {
                return CurrentOutputSynchronization;
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<OutputSelectionReceipt>> SelectOutputAsync(
        SelectOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        SelectOutputRequest fingerprintRequest;
        EngineError? canonicalizationError = null;
        try
        {
            var canonicalPath = Path.GetFullPath(request.Output.PluginPath);
            var output = new OutputAssociation(
                canonicalPath,
                request.Output.ModKey,
                request.Output.LocalizedOutputMode,
                request.Output.MasterStyle);
            fingerprintRequest = new SelectOutputRequest(
                request.OperationId,
                request.ExpectedRevision,
                request.Mode,
                output);
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            fingerprintRequest = request;
            canonicalizationError = new EngineError(
                EngineErrorCode.InvalidRequest,
                $"The output path is invalid: {exception.Message}");
        }

        var fingerprint = FingerprintFactory.Create(WorkspaceId, fingerprintRequest);
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (TryReplay(fingerprintRequest.OperationId, fingerprint, out EngineResult<OutputSelectionReceipt>? replay, out var conflict, out var expired))
            {
                return replay!;
            }

            if (expired)
            {
                return CreateOperationReplayExpiredFailure<OutputSelectionReceipt>(fingerprintRequest.OperationId, fingerprintRequest.ExpectedRevision);
            }

            if (conflict)
            {
                return CreateReuseFailure<OutputSelectionReceipt>(fingerprintRequest.OperationId, fingerprintRequest.ExpectedRevision);
            }

            if (!CanStoreOperation(fingerprintRequest.OperationId))
            {
                return CreateOperationCapacityFailure<OutputSelectionReceipt>(fingerprintRequest.OperationId, fingerprintRequest.ExpectedRevision);
            }

            if (canonicalizationError is not null)
            {
                return Store(fingerprintRequest.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    canonicalizationError,
                    WorkspaceId,
                    fingerprintRequest.OperationId,
                    fingerprintRequest.ExpectedRevision,
                    CurrentRevision));
            }

            var canonicalRequest = fingerprintRequest;
            var guardFailure = ValidateMutation(canonicalRequest.OperationId, canonicalRequest.ExpectedRevision);
            if (guardFailure is not null)
            {
                return Store(canonicalRequest.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    guardFailure,
                    WorkspaceId,
                    canonicalRequest.OperationId,
                    canonicalRequest.ExpectedRevision,
                    CurrentRevision));
            }

            if (IsSourcePath(canonicalRequest.Output.PluginPath))
            {
                return Store(canonicalRequest.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    new EngineError(EngineErrorCode.InvalidRequest, "The output path must be distinct from every source and load-order plugin path."),
                    WorkspaceId,
                    canonicalRequest.OperationId,
                    canonicalRequest.ExpectedRevision,
                    CurrentRevision));
            }

            var leaseResult = await AcquireOutputDirectoryLeaseAsync(
                canonicalRequest.Output,
                cancellationToken).ConfigureAwait(false);
            if (!leaseResult.Succeeded || leaseResult.Value is null)
            {
                return Store(canonicalRequest.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    leaseResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The output-directory lease provider returned no lease."),
                    WorkspaceId,
                    canonicalRequest.OperationId,
                    canonicalRequest.ExpectedRevision,
                    CurrentRevision,
                    leaseResult.Warnings));
            }

            await using var outputLease = leaseResult.Value;
            var admissionResult = await InspectOutputAdmissionAsync(
                outputLease,
                canonicalRequest.Output,
                cancellationToken).ConfigureAwait(false);
            var admissionWarnings = CombineWarnings(leaseResult.Warnings, admissionResult.Warnings);
            if (!admissionResult.Succeeded || admissionResult.Value is null)
            {
                return Store(canonicalRequest.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    admissionResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "Output admission returned no result."),
                    WorkspaceId,
                    canonicalRequest.OperationId,
                    canonicalRequest.ExpectedRevision,
                    CurrentRevision,
                    admissionWarnings));
            }

            if (admissionResult.Value.Status == OutputSynchronizationStatus.RecoveryRequired)
            {
                if (Output is null
                    || SelectedOutput is null
                    || OutputAssociationsMatch(SelectedOutput, canonicalRequest.Output))
                {
                    SetOutputSynchronization(OutputSynchronizationStatus.RecoveryRequired, admissionResult.Value.UnresolvedSave!);
                }

                return Store(canonicalRequest.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    new EngineError(EngineErrorCode.RepairRequired, "The selected output has an unresolved save journal that must be recovered before native opening."),
                    WorkspaceId,
                    canonicalRequest.OperationId,
                    canonicalRequest.ExpectedRevision,
                    CurrentRevision,
                    admissionWarnings));
            }

            EngineResult<NativeOutputOpenResult> openResult;
            try
            {
                openResult = await Task.Run(
                    async () => await Adapter.OpenOutputAsync(Sources!, canonicalRequest, cancellationToken).ConfigureAwait(false))
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Failed to open native output for workspace {WorkspaceId} and operation {OperationId}", WorkspaceId, canonicalRequest.OperationId);
                return Store(canonicalRequest.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    new EngineError(EngineErrorCode.OutputOpenFailed, "The selected native output could not be opened."),
                    WorkspaceId,
                    canonicalRequest.OperationId,
                    canonicalRequest.ExpectedRevision,
                    CurrentRevision,
                    admissionWarnings));
            }

            if (!openResult.Succeeded || openResult.Value is null)
            {
                return Store(canonicalRequest.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Failure(
                    openResult.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The selected native output could not be opened."),
                    WorkspaceId,
                    canonicalRequest.OperationId,
                    canonicalRequest.ExpectedRevision,
                    CurrentRevision,
                    CombineWarnings(admissionWarnings, openResult.Warnings)));
            }

            await DisposeOpenedOutputIfCanceledAsync(openResult.Value, cancellationToken).ConfigureAwait(false);
            var baseRevision = CurrentRevision;
            var resultRevision = CreateOutputRevision(openResult.Value.Association, openResult.Value.Baseline);
            var disposalWarnings = await PublishOutputAsync(openResult.Value, true, resultRevision).ConfigureAwait(false);
            var warnings = CombineWarnings(
                CombineWarnings(admissionWarnings, openResult.Warnings),
                disposalWarnings);
            var receipt = new OutputSelectionReceipt(SelectedOutput!, SelectedOutputBaseline!, resultRevision);
            return Store(canonicalRequest.OperationId, fingerprint, EngineResult<OutputSelectionReceipt>.Success(
                receipt,
                WorkspaceId,
                canonicalRequest.OperationId,
                baseRevision,
                resultRevision,
                warnings));
        }
        finally
        {
            OperationGate.Release();
        }
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<PluginSummary>>> ListPluginsAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteReadAsync(
            () => Adapter.ListPlugins(Sources!, Output, cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IReadOnlyList<FormListSummary>>> ListFormListsAsync(
        RecordScope scope,
        CancellationToken cancellationToken = default)
    {
        return ExecuteReadAsync(
            () => Adapter.ListFormLists(Sources!, Output, scope, cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<IMajorRecordGetter>> ReadFormListAsync(
        FormKey formKey,
        RecordScope scope,
        CancellationToken cancellationToken = default)
    {
        return ExecuteReadAsync(
            () => ReadLegacyFormList(new ReferenceRequest(formKey, scope), cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<FormListReadView>> ReadFormListViewAsync(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteReadAsync(
            () => CreateReadView(request, cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<ReferenceSearchPage>> SearchReferencesAsync(
        ReferenceSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteReadAsync(
            () => Adapter.SearchReferences(
                Sources!,
                Output,
                request,
                WorkspaceId,
                CurrentRevision,
                cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<ReferenceResolution>> ResolveReferenceAsync(
        ReferenceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteReadAsync(
            () => Adapter.ResolveReference(Sources!, Output, request, cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<EditReceipt>> BeginEditAsync(
        BeginEditRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var fingerprint = FingerprintFactory.Create(WorkspaceId, request);
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (TryReplay(request.OperationId, fingerprint, out EngineResult<EditReceipt>? replay, out var conflict, out var expired))
            {
                return replay!;
            }

            if (expired)
            {
                return CreateOperationReplayExpiredFailure<EditReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (conflict)
            {
                return CreateReuseFailure<EditReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (!CanStoreOperation(request.OperationId))
            {
                return CreateOperationCapacityFailure<EditReceipt>(request.OperationId, request.ExpectedRevision);
            }

            var guardFailure = ValidateMutation(request.OperationId, request.ExpectedRevision);
            if (guardFailure is not null)
            {
                return Store(request.OperationId, fingerprint, EngineResult<EditReceipt>.Failure(
                    guardFailure,
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            if (Output is null)
            {
                return Store(request.OperationId, fingerprint, EngineResult<EditReceipt>.Failure(
                    new EngineError(EngineErrorCode.OutputNotSelected, "Select an output before beginning a FormList edit."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            if (request.Role == FormListEditRole.ExistingOutput && request.TargetFormKey is { } targetFormKey)
            {
                var existingEdit = Edits.Values.FirstOrDefault(edit => edit.FormKey == targetFormKey);
                if (existingEdit is not null)
                {
                    var receipt = new EditReceipt(
                        existingEdit.EditId,
                        existingEdit.FormKey,
                        existingEdit.OriginFormKey,
                        existingEdit.Role,
                        CurrentRevision);
                    return Store(request.OperationId, fingerprint, EngineResult<EditReceipt>.Success(
                        receipt,
                        WorkspaceId,
                        request.OperationId,
                        CurrentRevision,
                        CurrentRevision));
                }
            }

            INativeOutputState? candidate = null;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                candidate = await Task.Run(() => Adapter.CloneOutput(Output, cancellationToken)).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                var nativeResult = await Task.Run(
                    () => Adapter.BeginEdit(Sources!, candidate, request, cancellationToken))
                    .ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                if (!nativeResult.Succeeded || nativeResult.Value is null)
                {
                    await candidate.DisposeAsync().ConfigureAwait(false);
                    candidate = null;
                    return Store(request.OperationId, fingerprint, EngineResult<EditReceipt>.Failure(
                        nativeResult.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The native adapter rejected the begin-edit request."),
                        WorkspaceId,
                        request.OperationId,
                        request.ExpectedRevision,
                        CurrentRevision,
                        nativeResult.Warnings));
                }

                if (Edits.ContainsKey(nativeResult.Value.EditId))
                {
                    await candidate.DisposeAsync().ConfigureAwait(false);
                    candidate = null;
                    return Store(request.OperationId, fingerprint, EngineResult<EditReceipt>.Failure(
                        new EngineError(EngineErrorCode.ValidationFailed, "The native adapter returned a duplicate staged edit identifier."),
                        WorkspaceId,
                        request.OperationId,
                        request.ExpectedRevision,
                        CurrentRevision));
                }

                if (Edits.Values.Any(edit => edit.FormKey == nativeResult.Value.FormKey))
                {
                    await candidate.DisposeAsync().ConfigureAwait(false);
                    candidate = null;
                    return Store(request.OperationId, fingerprint, EngineResult<EditReceipt>.Failure(
                        new EngineError(EngineErrorCode.ValidationFailed, "The native FormList already has a staged edit session in this workspace."),
                        WorkspaceId,
                        request.OperationId,
                        request.ExpectedRevision,
                        CurrentRevision));
                }

                cancellationToken.ThrowIfCancellationRequested();
                var baseRevision = CurrentRevision;
                var resultRevision = baseRevision.Next();
                var disposalWarnings = await PublishCandidateAsync(candidate, resultRevision).ConfigureAwait(false);
                candidate = null;
                Edits.Add(nativeResult.Value.EditId, nativeResult.Value);
                var receipt = new EditReceipt(
                    nativeResult.Value.EditId,
                    nativeResult.Value.FormKey,
                    nativeResult.Value.OriginFormKey,
                    nativeResult.Value.Role,
                    resultRevision);
                return Store(request.OperationId, fingerprint, EngineResult<EditReceipt>.Success(
                    receipt,
                    WorkspaceId,
                    request.OperationId,
                    baseRevision,
                    resultRevision,
                    CombineWarnings(nativeResult.Warnings, disposalWarnings)));
            }
            catch (OperationCanceledException exception)
            {
                if (candidate is not null)
                {
                    await DisposeCandidateAfterFailureAsync(candidate, exception).ConfigureAwait(false);
                }

                throw;
            }
            catch (Exception exception)
            {
                if (candidate is not null)
                {
                    await DisposeCandidateAfterFailureAsync(candidate, exception).ConfigureAwait(false);
                }

                Logger.Error(exception, "Failed to begin native FormList edit in workspace {WorkspaceId} for operation {OperationId}", WorkspaceId, request.OperationId);
                return Store(request.OperationId, fingerprint, EngineResult<EditReceipt>.Failure(
                    new EngineError(EngineErrorCode.UnexpectedFailure, "The FormList edit could not be started."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }
        }
        finally
        {
            OperationGate.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<OperationReceipt>> ApplyFormListEditAsync(
        FormListEditRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        PreparedFormListEdit preparedEdit;
        try
        {
            preparedEdit = Adapter.PrepareEdit(request.Edit);
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Failed to snapshot native FormList command {CommandName} for workspace {WorkspaceId} and operation {OperationId}", request.Edit.CommandName, WorkspaceId, request.OperationId);
            return EngineResult<OperationReceipt>.Failure(
                new EngineError(EngineErrorCode.UnexpectedFailure, "The native edit payload could not be defensively prepared."),
                WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                Revision);
        }

        var fingerprint = FingerprintFactory.Create(WorkspaceId, request, preparedEdit);
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (TryReplay(request.OperationId, fingerprint, out EngineResult<OperationReceipt>? replay, out var conflict, out var expired))
            {
                return replay!;
            }

            if (expired)
            {
                return CreateOperationReplayExpiredFailure<OperationReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (conflict)
            {
                return CreateReuseFailure<OperationReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (!CanStoreOperation(request.OperationId))
            {
                return CreateOperationCapacityFailure<OperationReceipt>(request.OperationId, request.ExpectedRevision);
            }

            var guardFailure = ValidateMutation(request.OperationId, request.ExpectedRevision);
            if (guardFailure is not null)
            {
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    guardFailure,
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            if (!preparedEdit.IsValid)
            {
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    preparedEdit.Error!,
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            if (Output is null)
            {
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    new EngineError(EngineErrorCode.OutputNotSelected, "Select an output before applying a FormList edit."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            if (!Edits.TryGetValue(request.EditId, out var editIdentity))
            {
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    new EngineError(EngineErrorCode.EditNotFound, "The staged FormList edit identifier is not part of this workspace."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            INativeOutputState? candidate = null;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                candidate = await Task.Run(() => Adapter.CloneOutput(Output, cancellationToken)).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                var nativeResult = await Task.Run(
                    () => Adapter.ApplyEdit(Sources!, candidate, editIdentity.FormKey, preparedEdit))
                    .ConfigureAwait(false);
                if (!nativeResult.Succeeded)
                {
                    await candidate.DisposeAsync().ConfigureAwait(false);
                    candidate = null;
                    return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                        nativeResult.Error ?? new EngineError(EngineErrorCode.ValidationFailed, "The native adapter rejected the typed FormList edit."),
                        WorkspaceId,
                        request.OperationId,
                        request.ExpectedRevision,
                        CurrentRevision,
                        nativeResult.Warnings));
                }

                cancellationToken.ThrowIfCancellationRequested();
                var baseRevision = CurrentRevision;
                var resultRevision = baseRevision.Next();
                var disposalWarnings = await PublishCandidateAsync(candidate, resultRevision).ConfigureAwait(false);
                candidate = null;
                var receipt = new OperationReceipt(request.OperationId, resultRevision);
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Success(
                    receipt,
                    WorkspaceId,
                    request.OperationId,
                    baseRevision,
                    resultRevision,
                    CombineWarnings(nativeResult.Warnings, disposalWarnings)));
            }
            catch (OperationCanceledException exception)
            {
                if (candidate is not null)
                {
                    await DisposeCandidateAfterFailureAsync(candidate, exception).ConfigureAwait(false);
                }

                throw;
            }
            catch (Exception exception)
            {
                if (candidate is not null)
                {
                    await DisposeCandidateAfterFailureAsync(candidate, exception).ConfigureAwait(false);
                }

                Logger.Error(exception, "Failed to apply native FormList edit in workspace {WorkspaceId} for operation {OperationId}", WorkspaceId, request.OperationId);
                return Store(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    new EngineError(EngineErrorCode.UnexpectedFailure, "The typed FormList edit could not be applied."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }
        }
        finally
        {
            OperationGate.Release();
        }
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<FormListComparison>> CompareFormListAsync(
        CompareFormListRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ExecuteReadAsync(
            () => CreateComparison(request, cancellationToken),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspacePreview>> PreviewAsync(CancellationToken cancellationToken = default)
    {
        return ExecuteReadAsync(
            () => Output is null
                ? EngineResult<WorkspacePreview>.Failure(new EngineError(EngineErrorCode.OutputNotSelected, "Select an output before previewing staged changes."))
                : Adapter.Preview(Sources!, Output),
            cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<OutputSelectionReceipt>> ReopenOutputAsync(
        ReopenOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return ReplaceWithReopenedOutputAsync(request, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<OperationReceipt>> DiscardChangesAsync(
        DiscardChangesRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        var fingerprint = FingerprintFactory.Create(WorkspaceId, request);
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (TryReplay(request.OperationId, fingerprint, out EngineResult<OperationReceipt>? replay, out var conflict, out var expired))
            {
                return replay!;
            }

            if (expired)
            {
                return CreateOperationReplayExpiredFailure<OperationReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (conflict)
            {
                return CreateReuseFailure<OperationReceipt>(request.OperationId, request.ExpectedRevision);
            }

            if (!CanStoreFinalizationOperation(request.OperationId))
            {
                return CreateOperationCapacityFailure<OperationReceipt>(request.OperationId, request.ExpectedRevision);
            }

            var guardFailure = ValidateOutputMutation(request.OperationId, request.ExpectedRevision, request.ExpectedBaseline);
            if (guardFailure is not null)
            {
                return StoreFinalization(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    guardFailure,
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision));
            }

            var leaseResult = await AcquireOutputDirectoryLeaseAsync(
                SelectedOutput!,
                cancellationToken).ConfigureAwait(false);
            if (!leaseResult.Succeeded || leaseResult.Value is null)
            {
                return StoreFinalization(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    leaseResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The output-directory lease provider returned no lease."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision,
                    leaseResult.Warnings));
            }

            await using var outputLease = leaseResult.Value;
            var admissionResult = await InspectOutputAdmissionAsync(
                outputLease,
                SelectedOutput!,
                cancellationToken).ConfigureAwait(false);
            var admissionWarnings = CombineWarnings(leaseResult.Warnings, admissionResult.Warnings);
            if (!admissionResult.Succeeded || admissionResult.Value is null)
            {
                return StoreFinalization(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    admissionResult.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "Output admission returned no result."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision,
                    admissionWarnings));
            }

            if (admissionResult.Value.Status == OutputSynchronizationStatus.RecoveryRequired)
            {
                SetOutputSynchronization(OutputSynchronizationStatus.RecoveryRequired, admissionResult.Value.UnresolvedSave!);
                return StoreFinalization(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    new EngineError(EngineErrorCode.RepairRequired, "The selected output has an unresolved save journal that must be recovered before discarding staged changes."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision,
                    admissionWarnings));
            }

            var openResult = await Task.Run(
                async () => await Adapter.ReopenOutputAsync(
                    Sources!,
                    SelectedOutput!,
                    SelectedOutputBaseline!,
                    cancellationToken).ConfigureAwait(false))
                .ConfigureAwait(false);
            if (!openResult.Succeeded || openResult.Value is null)
            {
                return StoreFinalization(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                    openResult.Error ?? new EngineError(EngineErrorCode.OutputOpenFailed, "The selected output could not be reopened for discard."),
                    WorkspaceId,
                    request.OperationId,
                    request.ExpectedRevision,
                    CurrentRevision,
                    CombineWarnings(admissionWarnings, openResult.Warnings)));
            }

            await DisposeOpenedOutputIfCanceledAsync(openResult.Value, cancellationToken).ConfigureAwait(false);
            var baseRevision = CurrentRevision;
            var resultRevision = CreateOutputRevision(openResult.Value.Association, openResult.Value.Baseline);
            var disposalWarnings = await PublishOutputAsync(openResult.Value, true, resultRevision).ConfigureAwait(false);
            var receipt = new OperationReceipt(request.OperationId, resultRevision);
            return StoreFinalization(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Success(
                receipt,
                WorkspaceId,
                request.OperationId,
                baseRevision,
                resultRevision,
                CombineWarnings(
                    CombineWarnings(admissionWarnings, openResult.Warnings),
                    disposalWarnings)));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Failed to discard staged native output in workspace {WorkspaceId} for operation {OperationId}", WorkspaceId, request.OperationId);
            return StoreFinalization(request.OperationId, fingerprint, EngineResult<OperationReceipt>.Failure(
                new EngineError(EngineErrorCode.UnexpectedFailure, "Staged output changes could not be discarded."),
                WorkspaceId,
                request.OperationId,
                request.ExpectedRevision,
                CurrentRevision));
        }
        finally
        {
            OperationGate.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await OperationGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;
            var output = Output;
            var sources = Sources;
            Output = null;
            Sources = null;
            SelectedOutput = null;
            SelectedOutputBaseline = null;
            Edits.Clear();

            List<Exception>? failures = null;
            if (output is not null)
            {
                try
                {
                    await output.DisposeAsync().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    failures = [exception];
                    Logger.Error(exception, "Failed to dispose native output for workspace {WorkspaceId}", WorkspaceId);
                }
            }

            if (sources is not null)
            {
                try
                {
                    await sources.DisposeAsync().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    failures ??= [];
                    failures.Add(exception);
                    Logger.Error(exception, "Failed to dispose native sources for workspace {WorkspaceId}", WorkspaceId);
                }
            }

            Logger.Debug("Disposed native FormList workspace {WorkspaceId}", WorkspaceId);
            if (failures is { Count: 1 })
            {
                throw failures[0];
            }

            if (failures is { Count: > 1 })
            {
                throw new AggregateException("Multiple native workspace resources failed to dispose.", failures);
            }
        }
        finally
        {
            OperationGate.Release();
        }
    }

}
