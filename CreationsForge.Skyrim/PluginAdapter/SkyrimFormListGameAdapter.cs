using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordReading;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Skyrim.PluginAdapter;

/// <summary>
/// Composes Skyrim Special Edition plugin source, output, edit, inspection, and staged-write services for the shared FormList engine.
/// </summary>
public sealed class SkyrimFormListGameAdapter : IFormListGameAdapter
{
    /// <summary>The complete Skyrim source loader.</summary>
    private readonly SkyrimPluginSourceLoader _sourceLoader;

    /// <summary>The complete Skyrim output and staged-write service.</summary>
    private readonly SkyrimPluginOutputService _outputService;

    /// <summary>The immutable-preparation and transactional Skyrim edit service.</summary>
    private readonly SkyrimRecordEditService _editService;

    /// <summary>Initializes the complete Skyrim Special Edition engine adapter.</summary>
    /// <param name="sourceLoader">The plugin source acquisition service.</param>
    /// <param name="outputService">The plugin output acquisition and staged-write service.</param>
    /// <param name="editService">The typed FormList edit and preview service.</param>
    /// <exception cref="ArgumentNullException">Thrown when any required service is <see langword="null"/>.</exception>
    public SkyrimFormListGameAdapter(
        SkyrimPluginSourceLoader sourceLoader,
        SkyrimPluginOutputService outputService,
        SkyrimRecordEditService editService)
    {
        ArgumentNullException.ThrowIfNull(sourceLoader);
        ArgumentNullException.ThrowIfNull(outputService);
        ArgumentNullException.ThrowIfNull(editService);
        _sourceLoader = sourceLoader;
        _outputService = outputService;
        _editService = editService;
    }

    /// <inheritdoc />
    public SupportedGame Game => SupportedGame.Skyrim;

    /// <inheritdoc />
    public IFormListInspector Inspector => _outputService.Inspector;

    /// <inheritdoc />
    public bool SupportsRelease(GameRelease release)
    {
        return release == GameRelease.SkyrimSE;
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
        if (sources is not SkyrimPluginSourceSet skyrimSources)
        {
            return WrongState<PluginOutputOpenResult>("source");
        }

        return await _outputService.OpenAsync(skyrimSources, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<PluginOutputOpenResult>> ReopenOutputAsync(
        IPluginSourceSet sources,
        OutputAssociation association,
        OutputArtifactSetBaseline expectedBaseline,
        CancellationToken cancellationToken = default)
    {
        if (sources is not SkyrimPluginSourceSet skyrimSources)
        {
            return WrongState<PluginOutputOpenResult>("source");
        }

        return await _outputService.ReopenAsync(
            skyrimSources,
            association,
            expectedBaseline,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IPluginOutputState CloneOutput(
        IPluginOutputState output,
        CancellationToken cancellationToken)
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
        if (sources is not SkyrimPluginSourceSet skyrimSources
            || candidate is not SkyrimPluginOutputState skyrimCandidate)
        {
            return WrongState<RecordEditIdentity>("source or output");
        }

        return _outputService.BeginEdit(skyrimSources, skyrimCandidate, request, cancellationToken);
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
        Mutagen.Bethesda.Plugins.FormKey target,
        PreparedFormListEdit edit)
    {
        if (sources is not SkyrimPluginSourceSet skyrimSources
            || candidate is not SkyrimPluginOutputState skyrimCandidate)
        {
            return WrongState<RecordEditMutationResult>("source or output");
        }

        return _editService.ApplyEdit(skyrimSources, skyrimCandidate, target, edit);
    }

    /// <inheritdoc />
    public EngineResult<IReadOnlyList<PluginSummary>> ListPlugins(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        CancellationToken cancellationToken)
    {
        if (sources is not SkyrimPluginSourceSet skyrimSources
            || !TryGetOutput(output, out var skyrimOutput))
        {
            return WrongState<IReadOnlyList<PluginSummary>>("source or output");
        }

        if (skyrimOutput is null)
        {
            return skyrimSources.ListPlugins(cancellationToken);
        }

        var result = PluginSummaryBuilder.Build(
            skyrimSources.GetMutagenMods(),
            skyrimSources.BorrowInputs().Plugins,
            skyrimOutput.GetMutableMod(),
            skyrimOutput.Association,
            cancellationToken);
        return EngineResult<IReadOnlyList<PluginSummary>>.Success(
            result.Value!,
            warnings: result.Warnings);
    }

    /// <inheritdoc />
    public EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        RecordScope scope,
        CancellationToken cancellationToken)
    {
        if (sources is not SkyrimPluginSourceSet skyrimSources
            || !TryGetOutput(output, out var skyrimOutput))
        {
            return WrongState<IReadOnlyList<FormListSummary>>("source or output");
        }

        return skyrimSources.ListFormLists(skyrimOutput, scope, cancellationToken);
    }

    /// <inheritdoc />
    public EngineResult<RecordRead> ReadFormListContext(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        if (sources is not SkyrimPluginSourceSet skyrimSources
            || !TryGetOutput(output, out var skyrimOutput))
        {
            return WrongState<RecordRead>("source or output");
        }

        return skyrimSources.ReadFormListContext(request, skyrimOutput, cancellationToken);
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
        if (sources is not SkyrimPluginSourceSet skyrimSources
            || !TryGetOutput(output, out var skyrimOutput))
        {
            return WrongState<ReferenceSearchPage>("source or output");
        }

        return skyrimSources.Search(
            request,
            skyrimOutput,
            workspaceId,
            revision,
            cancellationToken);
    }

    /// <inheritdoc />
    public EngineResult<ReferenceResolution> ResolveReference(
        IPluginSourceSet sources,
        IPluginOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        if (sources is not SkyrimPluginSourceSet skyrimSources
            || !TryGetOutput(output, out var skyrimOutput))
        {
            return WrongState<ReferenceResolution>("source or output");
        }

        return skyrimSources.Resolve(request, skyrimOutput, cancellationToken);
    }

    /// <inheritdoc />
    public EngineResult<WorkspacePreview> Preview(
        IPluginSourceSet sources,
        IPluginOutputState output)
    {
        if (sources is not SkyrimPluginSourceSet skyrimSources
            || output is not SkyrimPluginOutputState skyrimOutput)
        {
            return WrongState<WorkspacePreview>("source or output");
        }

        return _editService.Preview(skyrimSources, skyrimOutput);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<StagedPluginOutputSet>> WriteAndValidateAsync(
        IPluginSourceSet sources,
        IPluginOutputState output,
        PluginWriteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (sources is not SkyrimPluginSourceSet skyrimSources
            || output is not SkyrimPluginOutputState skyrimOutput)
        {
            return WrongState<StagedPluginOutputSet>("source or output");
        }

        return await _outputService.WriteAndValidateAsync(
            skyrimSources,
            skyrimOutput,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Requires a complete Skyrim output for the synchronous clone boundary.</summary>
    /// <param name="output">The plugin output supplied by the shared engine.</param>
    /// <returns>The same output through its exact Skyrim state type.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="output"/> belongs to another adapter.</exception>
    private static SkyrimPluginOutputState RequireOutput(IPluginOutputState output)
    {
        ArgumentNullException.ThrowIfNull(output);
        return output as SkyrimPluginOutputState
            ?? throw new ArgumentException("The Skyrim adapter requires a Skyrim plugin output state.", nameof(output));
    }

    /// <summary>Validates an optional output without changing the meaning of no selected output.</summary>
    /// <param name="output">The optional plugin output supplied by the shared engine.</param>
    /// <param name="skyrimOutput">The exact Skyrim output, or <see langword="null"/> when none was supplied.</param>
    /// <returns><see langword="true"/> when the value is absent or belongs to Skyrim.</returns>
    private static bool TryGetOutput(
        IPluginOutputState? output,
        out SkyrimPluginOutputState? skyrimOutput)
    {
        skyrimOutput = output as SkyrimPluginOutputState;
        return output is null || skyrimOutput is not null;
    }

    /// <summary>Creates a stable typed failure for cross-game Mutagen state passed to this adapter.</summary>
    /// <typeparam name="T">The attempted adapter operation result type.</typeparam>
    /// <param name="stateName">The incompatible state description used in the diagnostic.</param>
    /// <returns>A typed invalid-request failure.</returns>
    private static EngineResult<T> WrongState<T>(string stateName)
    {
        return EngineResult<T>.Failure(new EngineError(
            EngineErrorCode.InvalidRequest,
            $"The Skyrim adapter requires Skyrim Mutagen {stateName} state."));
    }
}
