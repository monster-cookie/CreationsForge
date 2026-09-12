using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeReading;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Fallout4.Native;

/// <summary>Composes Fallout 4 native source, output, edit, read, preview, and staged-write operations for the shared FormList engine.</summary>
public sealed class Fallout4FormListGameAdapter : IFormListGameAdapter
{
    /// <summary>The typed source acquisition service.</summary>
    private readonly Fallout4NativeSourceLoader _sourceLoader;

    /// <summary>The typed complete-output service.</summary>
    private readonly Fallout4NativeOutputService _outputService;

    /// <summary>The typed edit and preview service.</summary>
    private readonly Fallout4NativeEditService _editService;

    /// <summary>The private staged-write and native validation service.</summary>
    private readonly Fallout4NativeWriteService _writeService;

    /// <summary>Initializes the complete Fallout 4 FormList adapter from its existing typed services.</summary>
    /// <param name="sourceLoader">The native source acquisition service.</param>
    /// <param name="outputService">The native output selection and cloning service.</param>
    /// <param name="editService">The typed edit and preview service.</param>
    /// <param name="writeService">The private staged-write and validation service.</param>
    /// <exception cref="ArgumentNullException">Thrown when any required service is <see langword="null"/>.</exception>
    public Fallout4FormListGameAdapter(
        Fallout4NativeSourceLoader sourceLoader,
        Fallout4NativeOutputService outputService,
        Fallout4NativeEditService editService,
        Fallout4NativeWriteService writeService)
    {
        ArgumentNullException.ThrowIfNull(sourceLoader);
        ArgumentNullException.ThrowIfNull(outputService);
        ArgumentNullException.ThrowIfNull(editService);
        ArgumentNullException.ThrowIfNull(writeService);
        _sourceLoader = sourceLoader;
        _outputService = outputService;
        _editService = editService;
        _writeService = writeService;
    }

    /// <inheritdoc />
    public SupportedGame Game => SupportedGame.Fallout4;

    /// <inheritdoc />
    public IFormListNativeInspector Inspector => _outputService.Inspector;

    /// <inheritdoc />
    public bool SupportsRelease(GameRelease release)
    {
        return release == GameRelease.Fallout4;
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<NativeSourceOpenResult>> OpenSourcesAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _sourceLoader.OpenAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<NativeOutputOpenResult>> OpenOutputAsync(
        INativeSourceSet sources,
        SelectOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetSources(sources, out var falloutSources, out var failure))
        {
            return EngineResult<NativeOutputOpenResult>.Failure(failure!);
        }

        return await _outputService.OpenAsync(falloutSources!, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<NativeOutputOpenResult>> ReopenOutputAsync(
        INativeSourceSet sources,
        OutputAssociation association,
        OutputArtifactSetBaseline expectedBaseline,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetSources(sources, out var falloutSources, out var failure))
        {
            return EngineResult<NativeOutputOpenResult>.Failure(failure!);
        }

        return await _outputService.ReopenAsync(
            falloutSources!,
            association,
            expectedBaseline,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public INativeOutputState CloneOutput(INativeOutputState output, CancellationToken cancellationToken)
    {
        return _outputService.Clone(RequireOutput(output), cancellationToken);
    }

    /// <inheritdoc />
    public EngineResult<NativeEditIdentity> BeginEdit(
        INativeSourceSet sources,
        INativeOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<NativeEditIdentity>.Failure(sourceFailure!);
        }

        if (!TryGetOutput(candidate, out var falloutOutput, out var outputFailure))
        {
            return Failure<NativeEditIdentity>(falloutSources!, outputFailure!);
        }

        return _outputService.BeginEdit(falloutSources!, falloutOutput!, request, cancellationToken);
    }

    /// <inheritdoc />
    public PreparedFormListEdit PrepareEdit(FormListEdit edit)
    {
        return _editService.PrepareEdit(edit);
    }

    /// <inheritdoc />
    public EngineResult<NativeEditMutationResult> ApplyEdit(
        INativeSourceSet sources,
        INativeOutputState candidate,
        FormKey target,
        PreparedFormListEdit edit)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<NativeEditMutationResult>.Failure(sourceFailure!);
        }

        if (!TryGetOutput(candidate, out var falloutOutput, out var outputFailure))
        {
            return Failure<NativeEditMutationResult>(falloutSources!, outputFailure!);
        }

        return _editService.ApplyEdit(falloutSources!, falloutOutput!, target, edit);
    }

    /// <inheritdoc />
    public EngineResult<IReadOnlyList<PluginSummary>> ListPlugins(
        INativeSourceSet sources,
        INativeOutputState? output,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<IReadOnlyList<PluginSummary>>.Failure(sourceFailure!);
        }

        var sourceResult = falloutSources!.ListPlugins(cancellationToken);
        if (!sourceResult.Succeeded || output is null)
        {
            return sourceResult;
        }

        if (!TryGetOutput(output, out var falloutOutput, out var outputFailure))
        {
            return Failure<IReadOnlyList<PluginSummary>>(falloutSources, outputFailure!);
        }

        var summaries = sourceResult.Value!.ToList();
        summaries.Add(new PluginSummary(
            falloutOutput!.Association.ModKey,
            falloutOutput.Association.PluginPath,
            summaries.Count,
            PluginRole.Output));
        return Success<IReadOnlyList<PluginSummary>>(falloutSources, Array.AsReadOnly(summaries.ToArray()));
    }

    /// <inheritdoc />
    public EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        INativeSourceSet sources,
        INativeOutputState? output,
        RecordScope scope,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<IReadOnlyList<FormListSummary>>.Failure(sourceFailure!);
        }

        if (output is null)
        {
            return falloutSources!.ListFormLists(scope, cancellationToken);
        }

        if (!TryGetOutput(output, out var falloutOutput, out var outputFailure))
        {
            return Failure<IReadOnlyList<FormListSummary>>(falloutSources!, outputFailure!);
        }

        if (!Enum.IsDefined(scope))
        {
            return Failure<IReadOnlyList<FormListSummary>>(
                falloutSources!,
                new EngineError(EngineErrorCode.InvalidRequest, $"The Fallout 4 FormList scope '{scope}' is undefined."));
        }

        try
        {
            var nativeSources = CreateNativeSources(falloutSources!, falloutOutput!);
            var participatingMods = falloutSources!.GetNativeMods()
                .Append<IFallout4ModGetter>(falloutOutput!.BorrowMod())
                .ToArray();
            var reader = new NativeReferenceReader(nativeSources, EnumerateFormListLinks);
            var summaries = new List<FormListSummary>();
            for (var sourceIndex = 0; sourceIndex < nativeSources.Count; sourceIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var nativeSource = nativeSources[sourceIndex];
                if (!IncludesSource(scope, nativeSource.Role))
                {
                    continue;
                }

                var mod = participatingMods[sourceIndex];
                foreach (var formList in mod.FormLists)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (scope == RecordScope.WinningOverrides
                        && HasLaterFormList(participatingMods, sourceIndex, formList.FormKey, cancellationToken))
                    {
                        continue;
                    }

                    summaries.Add(new FormListSummary(
                        formList.FormKey,
                        formList.EditorID,
                        reader.CountOverrides(formList.FormKey, typeof(IFormListGetter), cancellationToken),
                        scope,
                        nativeSource.ModKey,
                        nativeSource.Path,
                        nativeSource.LoadOrderIndex,
                        nativeSource.Role));
                }
            }

            return Success<IReadOnlyList<FormListSummary>>(falloutSources!, Array.AsReadOnly(summaries.ToArray()));
        }
        catch (ObjectDisposedException exception)
        {
            return Failure<IReadOnlyList<FormListSummary>>(
                falloutSources!,
                new EngineError(EngineErrorCode.WorkspaceDisposed, exception.Message));
        }
    }

    /// <inheritdoc />
    public EngineResult<NativeRecordRead> ReadFormListContext(
        INativeSourceSet sources,
        INativeOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<NativeRecordRead>.Failure(sourceFailure!);
        }

        if (output is null)
        {
            return falloutSources!.ReadFormListContext(request, cancellationToken);
        }

        var readerResult = CreateReader(falloutSources!, output);
        if (!readerResult.Succeeded)
        {
            return Failure<NativeRecordRead>(falloutSources!, readerResult.Error!);
        }

        var readResult = readerResult.Value!.Read(request, cancellationToken);
        if (!readResult.Succeeded)
        {
            return Failure<NativeRecordRead>(falloutSources!, readResult.Error!, readResult.Warnings);
        }

        var read = readResult.Value!;
        if (read.Record is not null && read.Record is not IFormListGetter)
        {
            var context = read.Context;
            read = new NativeRecordRead(
                new FormListContext(
                    context.Selection,
                    ReferenceResolutionStatus.Unsupported,
                    context.ContainingModKey,
                    context.Path,
                    context.LoadOrderIndex,
                    context.Role),
                read.RecordType,
                record: null);
        }

        return Success(falloutSources!, read, readResult.Warnings);
    }

    /// <inheritdoc />
    public EngineResult<ReferenceSearchPage> SearchReferences(
        INativeSourceSet sources,
        INativeOutputState? output,
        ReferenceSearchRequest request,
        Guid workspaceId,
        WorkspaceRevision revision,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<ReferenceSearchPage>.Failure(sourceFailure!);
        }

        if (output is null)
        {
            return falloutSources!.Search(request, cancellationToken);
        }

        var readerResult = CreateReader(falloutSources!, output);
        if (!readerResult.Succeeded)
        {
            return Failure<ReferenceSearchPage>(falloutSources!, readerResult.Error!);
        }

        return Bind(
            falloutSources!,
            readerResult.Value!.Search(request, workspaceId, revision, cancellationToken),
            revision);
    }

    /// <inheritdoc />
    public EngineResult<ReferenceResolution> ResolveReference(
        INativeSourceSet sources,
        INativeOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<ReferenceResolution>.Failure(sourceFailure!);
        }

        if (output is null)
        {
            return falloutSources!.Resolve(request, cancellationToken);
        }

        var readerResult = CreateReader(falloutSources!, output);
        if (!readerResult.Succeeded)
        {
            return Failure<ReferenceResolution>(falloutSources!, readerResult.Error!);
        }

        return Bind(falloutSources!, readerResult.Value!.Resolve(request, cancellationToken));
    }

    /// <inheritdoc />
    public EngineResult<WorkspacePreview> Preview(INativeSourceSet sources, INativeOutputState output)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<WorkspacePreview>.Failure(sourceFailure!);
        }

        if (!TryGetOutput(output, out var falloutOutput, out var outputFailure))
        {
            return Failure<WorkspacePreview>(falloutSources!, outputFailure!);
        }

        return _editService.Preview(falloutSources!, falloutOutput!);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<NativeStagedOutputSet>> WriteAndValidateAsync(
        INativeSourceSet sources,
        INativeOutputState output,
        NativeWriteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<NativeStagedOutputSet>.Failure(sourceFailure!);
        }

        if (!TryGetOutput(output, out var falloutOutput, out var outputFailure))
        {
            return Failure<NativeStagedOutputSet>(falloutSources!, outputFailure!);
        }

        return await _writeService.WriteAndValidateAsync(
            falloutSources!,
            falloutOutput!,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Creates a combined ephemeral native reader when staged output participates in the current view.</summary>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="output">The selected output handle.</param>
    /// <returns>The combined reader or a typed output-handle failure.</returns>
    private static EngineResult<NativeReferenceReader> CreateReader(
        Fallout4NativeSourceSet sources,
        INativeOutputState output)
    {
        if (!TryGetOutput(output, out var falloutOutput, out var failure))
        {
            return EngineResult<NativeReferenceReader>.Failure(failure!);
        }

        try
        {
            return EngineResult<NativeReferenceReader>.Success(new NativeReferenceReader(
                CreateNativeSources(sources, falloutOutput!),
                EnumerateFormListLinks));
        }
        catch (ObjectDisposedException exception)
        {
            return EngineResult<NativeReferenceReader>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, exception.Message));
        }
    }

    /// <summary>Creates source descriptors over borrowed typed mods followed by the selected staged output.</summary>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="output">The typed selected output.</param>
    /// <returns>Ephemeral descriptors in exact participating load-order order.</returns>
    private static IReadOnlyList<NativeReferenceSource> CreateNativeSources(
        Fallout4NativeSourceSet sources,
        Fallout4NativeOutputState output)
    {
        var inputs = sources.BorrowInputs();
        var mods = sources.GetNativeMods();
        var nativeSources = inputs.Plugins
            .Select((plugin, index) => new NativeReferenceSource(
                mods[index],
                plugin.Path,
                plugin.LoadOrderIndex,
                plugin.Role))
            .ToList();
        nativeSources.Add(new NativeReferenceSource(
            output.BorrowMod(),
            output.Association.PluginPath,
            nativeSources.Count,
            PluginRole.Output));
        return Array.AsReadOnly(nativeSources.ToArray());
    }

    /// <summary>Determines whether one participating plugin belongs to the requested enumeration view.</summary>
    /// <param name="scope">The requested record scope.</param>
    /// <param name="role">The participating plugin role.</param>
    /// <returns><see langword="true"/> when the plugin contributes contexts to the scope.</returns>
    private static bool IncludesSource(RecordScope scope, PluginRole role)
    {
        return scope switch
        {
            RecordScope.Source => role == PluginRole.Source,
            RecordScope.StagedOutput => role == PluginRole.Output,
            RecordScope.AllContexts or RecordScope.WinningOverrides => true,
            _ => false
        };
    }

    /// <summary>Checks later typed groups for an overriding or deleting FormList context.</summary>
    /// <param name="mods">The complete participating native plugin list.</param>
    /// <param name="sourceIndex">The current source position.</param>
    /// <param name="formKey">The native FormList identity.</param>
    /// <param name="cancellationToken">A token observed throughout later-source traversal.</param>
    /// <returns><see langword="true"/> when a later FormList context exists.</returns>
    private static bool HasLaterFormList(
        IReadOnlyList<IFallout4ModGetter> mods,
        int sourceIndex,
        FormKey formKey,
        CancellationToken cancellationToken)
    {
        for (var laterIndex = sourceIndex + 1; laterIndex < mods.Count; laterIndex++)
        {
            foreach (var record in mods[laterIndex].FormLists)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (record.FormKey == formKey)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>Enumerates non-null Fallout 4 FormList item targets for shared native reference diagnostics.</summary>
    /// <param name="record">The borrowed native record.</param>
    /// <returns>Immutable real FormList targets with exact item positions; native null sentinels are omitted.</returns>
    private static IReadOnlyList<NativeFormLinkReference> EnumerateFormListLinks(IMajorRecordGetter record)
    {
        if (record is not IFormListGetter formList)
        {
            return Array.Empty<NativeFormLinkReference>();
        }

        var references = new List<NativeFormLinkReference>();
        for (var index = 0; index < formList.Items.Count; index++)
        {
            if (formList.Items[index].FormKeyNullable is { } formKey
                && !formKey.IsNull)
            {
                references.Add(new NativeFormLinkReference(formKey, $"Items[{index}]"));
            }
        }

        return Array.AsReadOnly(references.ToArray());
    }

    /// <summary>Requires the adapter's concrete Fallout 4 output state.</summary>
    /// <param name="output">The opaque engine output handle.</param>
    /// <returns>The same handle as the concrete Fallout 4 type.</returns>
    /// <exception cref="ArgumentException">Thrown when the handle belongs to another adapter.</exception>
    private static Fallout4NativeOutputState RequireOutput(INativeOutputState output)
    {
        ArgumentNullException.ThrowIfNull(output);
        return output as Fallout4NativeOutputState
            ?? throw new ArgumentException("The native output state was not created by the Fallout 4 adapter.", nameof(output));
    }

    /// <summary>Validates an opaque source handle without throwing for a cross-adapter call.</summary>
    /// <param name="sources">The opaque engine source handle.</param>
    /// <param name="typed">The typed Fallout 4 source handle on success.</param>
    /// <param name="failure">The typed validation failure on mismatch.</param>
    /// <returns><see langword="true"/> when the handle is a Fallout 4 source lifetime.</returns>
    private static bool TryGetSources(
        INativeSourceSet sources,
        out Fallout4NativeSourceSet? typed,
        out EngineError? failure)
    {
        ArgumentNullException.ThrowIfNull(sources);
        typed = sources as Fallout4NativeSourceSet;
        failure = typed is null
            ? new EngineError(EngineErrorCode.InvalidRequest, "The native source set was not created by the Fallout 4 adapter.")
            : null;
        return typed is not null;
    }

    /// <summary>Validates an opaque output handle without throwing for a cross-adapter call.</summary>
    /// <param name="output">The opaque engine output handle.</param>
    /// <param name="typed">The typed Fallout 4 output on success.</param>
    /// <param name="failure">The typed validation failure on mismatch.</param>
    /// <returns><see langword="true"/> when the handle is a Fallout 4 output state.</returns>
    private static bool TryGetOutput(
        INativeOutputState output,
        out Fallout4NativeOutputState? typed,
        out EngineError? failure)
    {
        ArgumentNullException.ThrowIfNull(output);
        typed = output as Fallout4NativeOutputState;
        failure = typed is null
            ? new EngineError(EngineErrorCode.InvalidRequest, "The native output state was not created by the Fallout 4 adapter.")
            : null;
        return typed is not null;
    }

    /// <summary>Creates an adapter result bound to the typed source lifetime.</summary>
    /// <typeparam name="T">The successful result type.</typeparam>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="value">The successful value.</param>
    /// <param name="warnings">Optional nonfatal warnings.</param>
    /// <returns>A successful source-bound engine result.</returns>
    private static EngineResult<T> Success<T>(
        Fallout4NativeSourceSet sources,
        T value,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<T>.Success(
            value,
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: warnings);
    }

    /// <summary>Creates an adapter failure bound to the typed source lifetime.</summary>
    /// <typeparam name="T">The requested result type.</typeparam>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="error">The stable typed failure.</param>
    /// <param name="warnings">Optional nonfatal warnings.</param>
    /// <returns>A failed source-bound engine result.</returns>
    private static EngineResult<T> Failure<T>(
        Fallout4NativeSourceSet sources,
        EngineError error,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<T>.Failure(
            error,
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: warnings);
    }

    /// <summary>Binds a shared native reader result to the typed source lifetime and selected workspace revision.</summary>
    /// <typeparam name="T">The shared reader value type.</typeparam>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="result">The unbound shared-reader result.</param>
    /// <param name="revision">The result revision, or the source-only revision when omitted.</param>
    /// <returns>An equivalent source-bound result.</returns>
    private static EngineResult<T> Bind<T>(
        Fallout4NativeSourceSet sources,
        EngineResult<T> result,
        WorkspaceRevision? revision = null)
    {
        return result.Succeeded
            ? EngineResult<T>.Success(
                result.Value!,
                workspaceId: sources.WorkspaceId,
                resultRevision: revision ?? sources.Revision,
                warnings: result.Warnings)
            : EngineResult<T>.Failure(
                result.Error!,
                workspaceId: sources.WorkspaceId,
                resultRevision: revision ?? sources.Revision,
                warnings: result.Warnings);
    }
}
