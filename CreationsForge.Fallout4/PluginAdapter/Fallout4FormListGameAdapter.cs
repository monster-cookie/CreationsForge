using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordReading;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Fallout4.PluginAdapter;

/// <summary>Composes Fallout 4 plugin source, output, edit, read, preview, and staged-write operations for the shared FormList engine.</summary>
public sealed class Fallout4FormListGameAdapter : IFormListGameAdapter
{
    /// <summary>The typed source acquisition service.</summary>
    private readonly Fallout4PluginSourceLoader _sourceLoader;

    /// <summary>The typed complete-output service.</summary>
    private readonly Fallout4PluginOutputService _outputService;

    /// <summary>The typed edit and preview service.</summary>
    private readonly Fallout4RecordEditService _editService;

    /// <summary>The private staged-write and Mutagen validation service.</summary>
    private readonly Fallout4PluginWriteService _writeService;

    /// <summary>Initializes the complete Fallout 4 FormList adapter from its existing typed services.</summary>
    /// <param name="sourceLoader">The plugin source acquisition service.</param>
    /// <param name="outputService">The plugin output selection and cloning service.</param>
    /// <param name="editService">The typed edit and preview service.</param>
    /// <param name="writeService">The private staged-write and validation service.</param>
    /// <exception cref="ArgumentNullException">Thrown when any required service is <see langword="null"/>.</exception>
    public Fallout4FormListGameAdapter(
        Fallout4PluginSourceLoader sourceLoader,
        Fallout4PluginOutputService outputService,
        Fallout4RecordEditService editService,
        Fallout4PluginWriteService writeService)
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
    public IFormListInspector Inspector => _outputService.Inspector;

    /// <inheritdoc />
    public IMajorRecordInspector MajorRecordInspector => _outputService.MajorRecordInspector;

    /// <inheritdoc />
    public bool SupportsRelease(GameRelease release)
    {
        return release == GameRelease.Fallout4;
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<PluginSourceOpenResult>> OpenSourcesAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _sourceLoader.OpenAsync(request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<PluginOutputOpenResult>> OpenOutputAsync(
        IPluginSourceSet sources,
        SelectOutputRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetSources(sources, out var falloutSources, out var failure))
        {
            return EngineResult<PluginOutputOpenResult>.Failure(failure!);
        }

        return await _outputService.OpenAsync(falloutSources!, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<PluginOutputOpenResult>> ReopenOutputAsync(
        IPluginSourceSet sources,
        OutputAssociation association,
        OutputArtifactSetBaseline expectedBaseline,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetSources(sources, out var falloutSources, out var failure))
        {
            return EngineResult<PluginOutputOpenResult>.Failure(failure!);
        }

        return await _outputService.ReopenAsync(
            falloutSources!,
            association,
            expectedBaseline,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IPluginOutputState CloneOutput(IPluginOutputState output, CancellationToken cancellationToken)
    {
        return _outputService.Clone(RequireOutput(output), cancellationToken);
    }

    /// <inheritdoc />
    public EngineResult<RecordEditIdentity> BeginEdit(
        IPluginSourceSet sources,
        IPluginOutputState candidate,
        BeginEditRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<RecordEditIdentity>.Failure(sourceFailure!);
        }

        if (!TryGetOutput(candidate, out var falloutOutput, out var outputFailure))
        {
            return Failure<RecordEditIdentity>(falloutSources!, outputFailure!);
        }

        return _outputService.BeginEdit(falloutSources!, falloutOutput!, request, cancellationToken);
    }

    /// <inheritdoc />
    public PreparedFormListEdit PrepareEdit(FormListEdit edit)
    {
        return _editService.PrepareEdit(edit);
    }

    /// <inheritdoc />
    public EngineResult<RecordEditMutationResult> ApplyEdit(
        IPluginSourceSet sources,
        IPluginOutputState candidate,
        FormKey target,
        PreparedFormListEdit edit)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<RecordEditMutationResult>.Failure(sourceFailure!);
        }

        if (!TryGetOutput(candidate, out var falloutOutput, out var outputFailure))
        {
            return Failure<RecordEditMutationResult>(falloutSources!, outputFailure!);
        }

        return _editService.ApplyEdit(falloutSources!, falloutOutput!, target, edit);
    }

    /// <inheritdoc />
    public EngineResult<IReadOnlyList<PluginSummary>> ListPlugins(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<IReadOnlyList<PluginSummary>>.Failure(sourceFailure!);
        }

        var validatedSources = falloutSources!;

        if (output is null)
        {
            return validatedSources.ListPlugins(cancellationToken);
        }

        if (!TryGetOutput(output, out var falloutOutput, out var outputFailure))
        {
            return Failure<IReadOnlyList<PluginSummary>>(validatedSources, outputFailure!);
        }

        var result = PluginSummaryBuilder.Build(
            validatedSources.GetMutagenMods(),
            validatedSources.BorrowInputs().Plugins,
            falloutOutput!.BorrowMod(),
            falloutOutput.Association,
            cancellationToken);
        return Success(validatedSources, result.Value!, result.Warnings);
    }

    /// <inheritdoc />
    public EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        IPluginSourceSet sources,
        IPluginOutputState? output,
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
            var pluginSources = CreatePluginSources(falloutSources!, falloutOutput!);
            var participatingMods = falloutSources!.GetMutagenMods()
                .Append<IFallout4ModGetter>(falloutOutput!.BorrowMod())
                .ToArray();
            var reader = new ReferenceReader(pluginSources, EnumerateFormListLinks);
            var summaries = new List<FormListSummary>();
            for (var sourceIndex = 0; sourceIndex < pluginSources.Count; sourceIndex++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var pluginSource = pluginSources[sourceIndex];
                if (!IncludesSource(scope, pluginSource.Role))
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
                        pluginSource.ModKey,
                        pluginSource.Path,
                        pluginSource.LoadOrderIndex,
                        pluginSource.Role));
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
    public EngineResult<RecordRead> ReadFormListContext(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<RecordRead>.Failure(sourceFailure!);
        }

        if (output is null)
        {
            return falloutSources!.ReadFormListContext(request, cancellationToken);
        }

        var readerResult = CreateReader(falloutSources!, output);
        if (!readerResult.Succeeded)
        {
            return Failure<RecordRead>(falloutSources!, readerResult.Error!);
        }

        var readResult = readerResult.Value!.Read(request, cancellationToken);
        if (!readResult.Succeeded)
        {
            return Failure<RecordRead>(falloutSources!, readResult.Error!, readResult.Warnings);
        }

        var read = readResult.Value!;
        if (read.Record is not null && read.Record is not IFormListGetter)
        {
            var context = read.Context;
            read = new RecordRead(
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
    public EngineResult<RecordRead> ReadRecordContext(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<RecordRead>.Failure(sourceFailure!);
        }

        if (output is null)
        {
            return falloutSources!.ReadRecordContext(request, cancellationToken);
        }

        var readerResult = CreateReader(falloutSources!, output);
        return readerResult.Succeeded
            ? Bind(falloutSources!, readerResult.Value!.Read(request, cancellationToken))
            : Failure<RecordRead>(falloutSources!, readerResult.Error!);
    }

    /// <inheritdoc />
    public EngineResult<ReferenceSearchPage> SearchReferences(
        IPluginSourceSet sources,
        IPluginOutputState? output,
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
    public EngineResult<int> VisitWinningRecordSummaries(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        Action<ReferenceSearchMatch> onRecord,
        Action<ModKey, int>? onProgress,
        CancellationToken cancellationToken)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<int>.Failure(sourceFailure!);
        }

        if (output is null)
        {
            return falloutSources!.VisitWinningRecordSummaries(onRecord, onProgress, cancellationToken);
        }

        var readerResult = CreateReader(falloutSources!, output);
        return readerResult.Succeeded
            ? Bind(falloutSources!, EngineResult<int>.Success(
                readerResult.Value!.VisitWinningRecordSummaries(onRecord, onProgress, cancellationToken)))
            : Failure<int>(falloutSources!, readerResult.Error!);
    }

    /// <inheritdoc />
    public EngineResult<ReferenceResolution> ResolveReference(
        IPluginSourceSet sources,
        IPluginOutputState? output,
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
    public EngineResult<WorkspacePreview> Preview(IPluginSourceSet sources, IPluginOutputState output)
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
    public async ValueTask<EngineResult<StagedPluginOutputSet>> WriteAndValidateAsync(
        IPluginSourceSet sources,
        IPluginOutputState output,
        PluginWriteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!TryGetSources(sources, out var falloutSources, out var sourceFailure))
        {
            return EngineResult<StagedPluginOutputSet>.Failure(sourceFailure!);
        }

        if (!TryGetOutput(output, out var falloutOutput, out var outputFailure))
        {
            return Failure<StagedPluginOutputSet>(falloutSources!, outputFailure!);
        }

        return await _writeService.WriteAndValidateAsync(
            falloutSources!,
            falloutOutput!,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Creates a combined ephemeral Mutagen reader when staged output participates in the current view.</summary>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="output">The selected output handle.</param>
    /// <returns>The combined reader or a typed output-handle failure.</returns>
    private static EngineResult<ReferenceReader> CreateReader(
        Fallout4PluginSourceSet sources,
        IPluginOutputState output)
    {
        if (!TryGetOutput(output, out var falloutOutput, out var failure))
        {
            return EngineResult<ReferenceReader>.Failure(failure!);
        }

        try
        {
            return EngineResult<ReferenceReader>.Success(new ReferenceReader(
                CreatePluginSources(sources, falloutOutput!),
                EnumerateFormListLinks));
        }
        catch (ObjectDisposedException exception)
        {
            return EngineResult<ReferenceReader>.Failure(
                new EngineError(EngineErrorCode.WorkspaceDisposed, exception.Message));
        }
    }

    /// <summary>Creates source descriptors over borrowed typed mods followed by the selected staged output.</summary>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="output">The typed selected output.</param>
    /// <returns>Ephemeral descriptors in exact participating load-order order.</returns>
    private static IReadOnlyList<ReferenceSource> CreatePluginSources(
        Fallout4PluginSourceSet sources,
        Fallout4PluginOutputState output)
    {
        var inputs = sources.BorrowInputs();
        var mods = sources.GetMutagenMods();
        var pluginSources = inputs.Plugins
            .Select((plugin, index) => new ReferenceSource(
                mods[index],
                plugin.Path,
                plugin.LoadOrderIndex,
                plugin.Role))
            .ToList();
        pluginSources.Add(new ReferenceSource(
            output.BorrowMod(),
            output.Association.PluginPath,
            pluginSources.Count,
            PluginRole.Output));
        return Array.AsReadOnly(pluginSources.ToArray());
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
    /// <param name="mods">The complete participating plugin list.</param>
    /// <param name="sourceIndex">The current source position.</param>
    /// <param name="formKey">The FormList identity.</param>
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

    /// <summary>Enumerates non-null Fallout 4 FormList item targets for shared reference diagnostics.</summary>
    /// <param name="record">The borrowed record.</param>
    /// <returns>Immutable real FormList targets with exact item positions; Mutagen null sentinels are omitted.</returns>
    private static IReadOnlyList<FormLinkReference> EnumerateFormListLinks(IMajorRecordGetter record)
    {
        if (record is not IFormListGetter formList)
        {
            return Array.Empty<FormLinkReference>();
        }

        var references = new List<FormLinkReference>();
        for (var index = 0; index < formList.Items.Count; index++)
        {
            if (formList.Items[index].FormKeyNullable is { } formKey
                && !formKey.IsNull)
            {
                references.Add(new FormLinkReference(formKey, $"Items[{index}]"));
            }
        }

        return Array.AsReadOnly(references.ToArray());
    }

    /// <summary>Requires the adapter's concrete Fallout 4 output state.</summary>
    /// <param name="output">The opaque engine output handle.</param>
    /// <returns>The same handle as the concrete Fallout 4 type.</returns>
    /// <exception cref="ArgumentException">Thrown when the handle belongs to another adapter.</exception>
    private static Fallout4PluginOutputState RequireOutput(IPluginOutputState output)
    {
        ArgumentNullException.ThrowIfNull(output);
        return output as Fallout4PluginOutputState
            ?? throw new ArgumentException("The plugin output state was not created by the Fallout 4 adapter.", nameof(output));
    }

    /// <summary>Validates an opaque source handle without throwing for a cross-adapter call.</summary>
    /// <param name="sources">The opaque engine source handle.</param>
    /// <param name="typed">The typed Fallout 4 source handle on success.</param>
    /// <param name="failure">The typed validation failure on mismatch.</param>
    /// <returns><see langword="true"/> when the handle is a Fallout 4 source lifetime.</returns>
    private static bool TryGetSources(
        IPluginSourceSet sources,
        out Fallout4PluginSourceSet? typed,
        out EngineError? failure)
    {
        ArgumentNullException.ThrowIfNull(sources);
        typed = sources as Fallout4PluginSourceSet;
        failure = typed is null
            ? new EngineError(EngineErrorCode.InvalidRequest, "The plugin source set was not created by the Fallout 4 adapter.")
            : null;
        return typed is not null;
    }

    /// <summary>Validates an opaque output handle without throwing for a cross-adapter call.</summary>
    /// <param name="output">The opaque engine output handle.</param>
    /// <param name="typed">The typed Fallout 4 output on success.</param>
    /// <param name="failure">The typed validation failure on mismatch.</param>
    /// <returns><see langword="true"/> when the handle is a Fallout 4 output state.</returns>
    private static bool TryGetOutput(
        IPluginOutputState output,
        out Fallout4PluginOutputState? typed,
        out EngineError? failure)
    {
        ArgumentNullException.ThrowIfNull(output);
        typed = output as Fallout4PluginOutputState;
        failure = typed is null
            ? new EngineError(EngineErrorCode.InvalidRequest, "The plugin output state was not created by the Fallout 4 adapter.")
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
        Fallout4PluginSourceSet sources,
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
        Fallout4PluginSourceSet sources,
        EngineError error,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        return EngineResult<T>.Failure(
            error,
            workspaceId: sources.WorkspaceId,
            resultRevision: sources.Revision,
            warnings: warnings);
    }

    /// <summary>Binds a shared Mutagen reader result to the typed source lifetime and selected workspace revision.</summary>
    /// <typeparam name="T">The shared reader value type.</typeparam>
    /// <param name="sources">The typed source lifetime.</param>
    /// <param name="result">The unbound shared-reader result.</param>
    /// <param name="revision">The result revision, or the source-only revision when omitted.</param>
    /// <returns>An equivalent source-bound result.</returns>
    private static EngineResult<T> Bind<T>(
        Fallout4PluginSourceSet sources,
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
