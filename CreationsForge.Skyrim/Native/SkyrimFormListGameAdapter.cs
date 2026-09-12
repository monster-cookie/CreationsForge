using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Skyrim.Native;

/// <summary>
/// Composes Skyrim Special Edition native source, output, edit, inspection, and staged-write services for the shared FormList engine.
/// </summary>
public sealed class SkyrimFormListGameAdapter : IFormListGameAdapter
{
    /// <summary>The complete Skyrim source loader.</summary>
    private readonly SkyrimNativeSourceLoader _sourceLoader;

    /// <summary>The complete Skyrim output and staged-write service.</summary>
    private readonly SkyrimNativeOutputService _outputService;

    /// <summary>The immutable-preparation and transactional Skyrim edit service.</summary>
    private readonly SkyrimNativeEditService _editService;

    /// <summary>Initializes the complete Skyrim Special Edition engine adapter.</summary>
    /// <param name="sourceLoader">The native source acquisition service.</param>
    /// <param name="outputService">The native output acquisition and staged-write service.</param>
    /// <param name="editService">The typed FormList edit and preview service.</param>
    /// <exception cref="ArgumentNullException">Thrown when any required service is <see langword="null"/>.</exception>
    public SkyrimFormListGameAdapter(
        SkyrimNativeSourceLoader sourceLoader,
        SkyrimNativeOutputService outputService,
        SkyrimNativeEditService editService)
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
    public IFormListNativeInspector Inspector => _outputService.Inspector;

    /// <inheritdoc />
    public bool SupportsRelease(GameRelease release)
    {
        return release == GameRelease.SkyrimSE;
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
        if (sources is not SkyrimNativeSourceSet skyrimSources)
        {
            return WrongState<NativeOutputOpenResult>("source");
        }

        return await _outputService.OpenAsync(skyrimSources, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<NativeOutputOpenResult>> ReopenOutputAsync(
        INativeSourceSet sources,
        OutputAssociation association,
        OutputArtifactSetBaseline expectedBaseline,
        CancellationToken cancellationToken = default)
    {
        if (sources is not SkyrimNativeSourceSet skyrimSources)
        {
            return WrongState<NativeOutputOpenResult>("source");
        }

        return await _outputService.ReopenAsync(
            skyrimSources,
            association,
            expectedBaseline,
            cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public INativeOutputState CloneOutput(
        INativeOutputState output,
        CancellationToken cancellationToken)
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
        if (sources is not SkyrimNativeSourceSet skyrimSources
            || candidate is not SkyrimNativeOutputState skyrimCandidate)
        {
            return WrongState<NativeEditIdentity>("source or output");
        }

        return _outputService.BeginEdit(skyrimSources, skyrimCandidate, request, cancellationToken);
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
        Mutagen.Bethesda.Plugins.FormKey target,
        PreparedFormListEdit edit)
    {
        if (sources is not SkyrimNativeSourceSet skyrimSources
            || candidate is not SkyrimNativeOutputState skyrimCandidate)
        {
            return WrongState<NativeEditMutationResult>("source or output");
        }

        return _editService.ApplyEdit(skyrimSources, skyrimCandidate, target, edit);
    }

    /// <inheritdoc />
    public EngineResult<IReadOnlyList<PluginSummary>> ListPlugins(
        INativeSourceSet sources,
        INativeOutputState? output,
        CancellationToken cancellationToken)
    {
        if (sources is not SkyrimNativeSourceSet skyrimSources
            || !TryGetOutput(output, out var skyrimOutput))
        {
            return WrongState<IReadOnlyList<PluginSummary>>("source or output");
        }

        var sourceResult = skyrimSources.ListPlugins(cancellationToken);
        if (!sourceResult.Succeeded || skyrimOutput is null)
        {
            return sourceResult;
        }

        var plugins = sourceResult.Value!.ToList();
        plugins.Add(new PluginSummary(
            skyrimOutput.Association.ModKey,
            skyrimOutput.Association.PluginPath,
            plugins.Count,
            PluginRole.Output));
        return EngineResult<IReadOnlyList<PluginSummary>>.Success(
            Array.AsReadOnly(plugins.ToArray()),
            warnings: sourceResult.Warnings);
    }

    /// <inheritdoc />
    public EngineResult<IReadOnlyList<FormListSummary>> ListFormLists(
        INativeSourceSet sources,
        INativeOutputState? output,
        RecordScope scope,
        CancellationToken cancellationToken)
    {
        if (sources is not SkyrimNativeSourceSet skyrimSources
            || !TryGetOutput(output, out var skyrimOutput))
        {
            return WrongState<IReadOnlyList<FormListSummary>>("source or output");
        }

        return skyrimSources.ListFormLists(skyrimOutput, scope, cancellationToken);
    }

    /// <inheritdoc />
    public EngineResult<NativeRecordRead> ReadFormListContext(
        INativeSourceSet sources,
        INativeOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        if (sources is not SkyrimNativeSourceSet skyrimSources
            || !TryGetOutput(output, out var skyrimOutput))
        {
            return WrongState<NativeRecordRead>("source or output");
        }

        return skyrimSources.ReadFormListContext(request, skyrimOutput, cancellationToken);
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
        if (sources is not SkyrimNativeSourceSet skyrimSources
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
        INativeSourceSet sources,
        INativeOutputState? output,
        ReferenceRequest request,
        CancellationToken cancellationToken)
    {
        if (sources is not SkyrimNativeSourceSet skyrimSources
            || !TryGetOutput(output, out var skyrimOutput))
        {
            return WrongState<ReferenceResolution>("source or output");
        }

        return skyrimSources.Resolve(request, skyrimOutput, cancellationToken);
    }

    /// <inheritdoc />
    public EngineResult<WorkspacePreview> Preview(
        INativeSourceSet sources,
        INativeOutputState output)
    {
        if (sources is not SkyrimNativeSourceSet skyrimSources
            || output is not SkyrimNativeOutputState skyrimOutput)
        {
            return WrongState<WorkspacePreview>("source or output");
        }

        return _editService.Preview(skyrimSources, skyrimOutput);
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<NativeStagedOutputSet>> WriteAndValidateAsync(
        INativeSourceSet sources,
        INativeOutputState output,
        NativeWriteRequest request,
        CancellationToken cancellationToken = default)
    {
        if (sources is not SkyrimNativeSourceSet skyrimSources
            || output is not SkyrimNativeOutputState skyrimOutput)
        {
            return WrongState<NativeStagedOutputSet>("source or output");
        }

        return await _outputService.WriteAndValidateAsync(
            skyrimSources,
            skyrimOutput,
            request,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Requires a complete Skyrim output for the synchronous clone boundary.</summary>
    /// <param name="output">The native output supplied by the shared engine.</param>
    /// <returns>The same output through its exact Skyrim state type.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="output"/> belongs to another adapter.</exception>
    private static SkyrimNativeOutputState RequireOutput(INativeOutputState output)
    {
        ArgumentNullException.ThrowIfNull(output);
        return output as SkyrimNativeOutputState
            ?? throw new ArgumentException("The Skyrim adapter requires a Skyrim native output state.", nameof(output));
    }

    /// <summary>Validates an optional output without changing the meaning of no selected output.</summary>
    /// <param name="output">The optional native output supplied by the shared engine.</param>
    /// <param name="skyrimOutput">The exact Skyrim output, or <see langword="null"/> when none was supplied.</param>
    /// <returns><see langword="true"/> when the value is absent or belongs to Skyrim.</returns>
    private static bool TryGetOutput(
        INativeOutputState? output,
        out SkyrimNativeOutputState? skyrimOutput)
    {
        skyrimOutput = output as SkyrimNativeOutputState;
        return output is null || skyrimOutput is not null;
    }

    /// <summary>Creates a stable typed failure for cross-game native state passed to this adapter.</summary>
    /// <typeparam name="T">The attempted adapter operation result type.</typeparam>
    /// <param name="stateName">The incompatible state description used in the diagnostic.</param>
    /// <returns>A typed invalid-request failure.</returns>
    private static EngineResult<T> WrongState<T>(string stateName)
    {
        return EngineResult<T>.Failure(new EngineError(
            EngineErrorCode.InvalidRequest,
            $"The Skyrim adapter requires Skyrim native {stateName} state."));
    }
}
