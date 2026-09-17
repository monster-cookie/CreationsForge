using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Serilog;

namespace CreationsForge.Core.Engine;

/// <summary>
/// Opens independently owned plugin workspaces from complete caller-supplied plugin inputs.
/// </summary>
public sealed class PluginWorkspaceFactory : IPluginWorkspaceFactory
{
    /// <summary>Compares canonical paths according to the host file system's case semantics.</summary>
    private static readonly StringComparer CanonicalPathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    /// <summary>Registered game adapters available for explicit release selection.</summary>
    private readonly IReadOnlyList<IFormListGameAdapter> Adapters;

    /// <summary>Coordinator shared by workspaces for guarded output saves.</summary>
    private readonly IWorkspaceSaveCoordinator SaveCoordinator;

    /// <summary>Provider shared by workspaces for exclusive output-directory leases.</summary>
    private readonly IOutputDirectoryLeaseProvider OutputDirectoryLeaseProvider;

    /// <summary>Structured logger used for unexpected open and cleanup failures.</summary>
    private readonly ILogger Logger;

    /// <summary>
    /// Initializes a workspace factory with externally supplied engine adapters and infrastructure.
    /// </summary>
    /// <param name="adapters">The game adapters eligible to open explicit plugin sources.</param>
    /// <param name="saveCoordinator">The coordinator used by each returned workspace for guarded saves.</param>
    /// <param name="outputDirectoryLeaseProvider">The lease provider used by each returned workspace for output admission and recovery.</param>
    /// <param name="logger">The structured logger used for workflow diagnostics.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="adapters"/> contains a <see langword="null"/> entry.</exception>
    public PluginWorkspaceFactory(
        IEnumerable<IFormListGameAdapter> adapters,
        IWorkspaceSaveCoordinator saveCoordinator,
        IOutputDirectoryLeaseProvider outputDirectoryLeaseProvider,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(adapters);
        ArgumentNullException.ThrowIfNull(saveCoordinator);
        ArgumentNullException.ThrowIfNull(outputDirectoryLeaseProvider);
        ArgumentNullException.ThrowIfNull(logger);

        var adapterSnapshot = adapters.ToArray();
        if (adapterSnapshot.Any(adapter => adapter is null))
        {
            throw new ArgumentException("The game adapter collection cannot contain null entries.", nameof(adapters));
        }

        Adapters = Array.AsReadOnly(adapterSnapshot);
        SaveCoordinator = saveCoordinator;
        OutputDirectoryLeaseProvider = outputDirectoryLeaseProvider;
        Logger = logger;
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<IPluginWorkspace>> OpenAsync(
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        PluginSourceOpenResult? sourceOpenResult = null;
        PluginWorkspace? workspace = null;
        var diagnosticProgress = new LoggingWorkspaceOpenProgress(Logger, request?.WorkspaceId, request?.Progress);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            diagnosticProgress.Report(new WorkspaceOpenProgress(
                WorkspaceOpenStage.Validating,
                "Validating explicit workspace inputs."));

            var canonicalRequestResult = ValidateAndCanonicalize(request);
            if (!canonicalRequestResult.Succeeded)
            {
                return EngineResult<IPluginWorkspace>.Failure(
                    canonicalRequestResult.Error!,
                    workspaceId: request?.WorkspaceId,
                    warnings: canonicalRequestResult.Warnings);
            }

            var canonicalRequest = canonicalRequestResult.Value!.WithProgress(diagnosticProgress);
            cancellationToken.ThrowIfCancellationRequested();
            canonicalRequest.Progress?.Report(new WorkspaceOpenProgress(
                WorkspaceOpenStage.SelectingAdapter,
                "Selecting a game adapter."));

            var adapterResult = SelectAdapter(canonicalRequest);
            if (!adapterResult.Succeeded)
            {
                return EngineResult<IPluginWorkspace>.Failure(
                    adapterResult.Error!,
                    workspaceId: canonicalRequest.WorkspaceId,
                    warnings: adapterResult.Warnings);
            }

            var adapter = adapterResult.Value!;
            cancellationToken.ThrowIfCancellationRequested();
            canonicalRequest.Progress?.Report(new WorkspaceOpenProgress(
                WorkspaceOpenStage.PreparingInputs,
                "Preparing explicit plugin sources."));
            cancellationToken.ThrowIfCancellationRequested();

            EngineResult<PluginSourceOpenResult> openResult;
            using var slowOpenCancellation = new CancellationTokenSource();
            var slowOpenTask = LogSlowOpenAsync(diagnosticProgress, slowOpenCancellation.Token);
            try
            {
                openResult = await Task.Run(async () =>
                    await adapter.OpenSourcesAsync(canonicalRequest, cancellationToken).ConfigureAwait(false))
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Logger.Error(
                    exception,
                    "The game adapter failed while opening plugin sources for workspace {WorkspaceId}.",
                    canonicalRequest.WorkspaceId);
                return EngineResult<IPluginWorkspace>.Failure(
                    new EngineError(
                        EngineErrorCode.SourceOpenFailed,
                        "The selected game adapter could not open the explicit plugin sources."),
                    workspaceId: canonicalRequest.WorkspaceId);
            }
            finally
            {
                slowOpenCancellation.Cancel();
                await ObserveSlowOpenTaskAsync(slowOpenTask).ConfigureAwait(false);
            }

            if (!openResult.Succeeded)
            {
                Logger.Warning(
                    "Workspace {WorkspaceId} source acquisition failed after {ElapsedMilliseconds} ms; error code: {ErrorCode}; message: {ErrorMessage}",
                    canonicalRequest.WorkspaceId,
                    diagnosticProgress.Elapsed.TotalMilliseconds,
                    openResult.Error?.Code,
                    openResult.Error?.Message ?? "No error description was returned.");
                return EngineResult<IPluginWorkspace>.Failure(
                    openResult.Error ?? new EngineError(
                        EngineErrorCode.SourceOpenFailed,
                        "The selected game adapter did not return a source-open failure reason."),
                    workspaceId: canonicalRequest.WorkspaceId,
                    warnings: openResult.Warnings);
            }

            sourceOpenResult = openResult.Value;
            if (sourceOpenResult is null)
            {
                return EngineResult<IPluginWorkspace>.Failure(
                    new EngineError(
                        EngineErrorCode.UnexpectedFailure,
                        "The selected game adapter returned no plugin source state."),
                    workspaceId: canonicalRequest.WorkspaceId,
                    warnings: openResult.Warnings);
            }

            cancellationToken.ThrowIfCancellationRequested();
            workspace = new PluginWorkspace(
                canonicalRequest,
                adapter,
                sourceOpenResult,
                SaveCoordinator,
                OutputDirectoryLeaseProvider,
                Logger);
            sourceOpenResult = null;

            canonicalRequest.Progress?.Report(new WorkspaceOpenProgress(
                WorkspaceOpenStage.Completed,
                "Workspace opened."));
            cancellationToken.ThrowIfCancellationRequested();

            var openedWorkspace = workspace;
            workspace = null;
            return EngineResult<IPluginWorkspace>.Success(
                openedWorkspace,
                workspaceId: canonicalRequest.WorkspaceId,
                warnings: openResult.Warnings);
        }
        catch (OperationCanceledException)
        {
            diagnosticProgress.LogCancellation();
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(
                exception,
                "An unexpected failure prevented workspace {WorkspaceId} from opening.",
                request?.WorkspaceId);
            return EngineResult<IPluginWorkspace>.Failure(
                new EngineError(
                    EngineErrorCode.UnexpectedFailure,
                    "An unexpected failure prevented the workspace from opening."),
                workspaceId: request?.WorkspaceId);
        }
        finally
        {
            await DisposeFailedOpenStateAsync(workspace, sourceOpenResult, request?.WorkspaceId);
        }
    }

    /// <summary>Reports an incomplete plugin source open after ten seconds and every thirty seconds thereafter.</summary>
    /// <param name="progress">The observer holding the most recent engine acquisition phase.</param>
    /// <param name="cancellationToken">A token canceled when the adapter finishes.</param>
    /// <returns>A task that completes when source acquisition finishes or is canceled.</returns>
    private static async Task LogSlowOpenAsync(
        LoggingWorkspaceOpenProgress progress,
        CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);
        while (true)
        {
            progress.LogStillRunning();
            await Task.Delay(TimeSpan.FromSeconds(30), cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>Observes the slow-open monitor's expected internal cancellation without hiding unexpected failures.</summary>
    /// <param name="slowOpenTask">The monitor task canceled after engine acquisition returns.</param>
    /// <returns>A task that completes after the monitor has stopped.</returns>
    private static async Task ObserveSlowOpenTaskAsync(Task slowOpenTask)
    {
        try
        {
            await slowOpenTask.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
        }
    }

    /// <summary>Validates explicit inputs and returns a caller-independent request containing canonical paths.</summary>
    /// <param name="request">The caller-supplied request.</param>
    /// <returns>A canonical request or a stable invalid-request failure.</returns>
    private static EngineResult<WorkspaceOpenRequest> ValidateAndCanonicalize(WorkspaceOpenRequest? request)
    {
        if (request is null)
        {
            return InvalidRequest("A workspace-open request is required.");
        }

        if (request.WorkspaceId == Guid.Empty)
        {
            return InvalidRequest("A non-empty workspace identifier is required.");
        }

        if (!Enum.IsDefined(request.Game))
        {
            return InvalidRequest("The requested game is invalid.");
        }

        if (!Enum.IsDefined(request.Release))
        {
            return InvalidRequest("The requested engine game release is invalid.");
        }

        if (!TryCanonicalizeFile(request.SourcePluginPath, "source plugin", out var sourcePluginPath, out var pathError))
        {
            return InvalidRequest(pathError);
        }

        if (!TryReadModKey(sourcePluginPath, out _, out var sourceModKeyError))
        {
            return InvalidRequest(sourceModKeyError);
        }

        if (request.LoadOrderPluginPaths.Count == 0)
        {
            return InvalidRequest("At least one explicit load-order plugin path is required.");
        }

        var canonicalLoadOrderPaths = new List<string>(request.LoadOrderPluginPaths.Count);
        var canonicalPathSet = new HashSet<string>(CanonicalPathComparer);
        var modKeySet = new HashSet<ModKey>();
        foreach (var pluginPath in request.LoadOrderPluginPaths)
        {
            if (!TryCanonicalizeFile(pluginPath, "load-order plugin", out var canonicalPluginPath, out pathError))
            {
                return InvalidRequest(pathError);
            }

            if (!canonicalPathSet.Add(canonicalPluginPath))
            {
                return InvalidRequest($"The load order contains the canonical plugin path more than once: '{canonicalPluginPath}'.");
            }

            if (!TryReadModKey(canonicalPluginPath, out var modKey, out var modKeyError))
            {
                return InvalidRequest(modKeyError);
            }

            if (!modKeySet.Add(modKey))
            {
                return InvalidRequest($"The load order contains the plugin identity more than once: '{modKey.FileName}'.");
            }

            canonicalLoadOrderPaths.Add(canonicalPluginPath);
        }

        if (!canonicalPathSet.Contains(sourcePluginPath))
        {
            return InvalidRequest("The source plugin must be included in the explicit ordered load order by canonical path.");
        }

        if (!TryCanonicalizeDirectory(request.DataDirectoryPath, "data directory", out var dataDirectoryPath, out pathError))
        {
            return InvalidRequest(pathError);
        }

        var canonicalStringDirectoryPaths = new List<string>(request.StringDirectoryPaths.Count);
        var stringDirectoryPathSet = new HashSet<string>(CanonicalPathComparer);
        foreach (var stringDirectoryPath in request.StringDirectoryPaths)
        {
            if (!TryCanonicalizeDirectory(stringDirectoryPath, "localized-string directory", out var canonicalStringDirectoryPath, out pathError))
            {
                return InvalidRequest(pathError);
            }

            if (!stringDirectoryPathSet.Add(canonicalStringDirectoryPath))
            {
                return InvalidRequest($"The localized-string directories contain the canonical path more than once: '{canonicalStringDirectoryPath}'.");
            }

            canonicalStringDirectoryPaths.Add(canonicalStringDirectoryPath);
        }

        return EngineResult<WorkspaceOpenRequest>.Success(request.WithCanonicalPaths(
            sourcePluginPath,
            canonicalLoadOrderPaths,
            dataDirectoryPath,
            canonicalStringDirectoryPaths));
    }

    /// <summary>Selects the single adapter matching the request's exact game and engine release pair.</summary>
    /// <param name="request">The validated canonical request.</param>
    /// <returns>The unique matching adapter or a stable selection failure.</returns>
    private EngineResult<IFormListGameAdapter> SelectAdapter(WorkspaceOpenRequest request)
    {
        var matchingAdapters = Adapters
            .Where(adapter => adapter.Game == request.Game && adapter.SupportsRelease(request.Release))
            .ToArray();

        if (matchingAdapters.Length == 0)
        {
            return EngineResult<IFormListGameAdapter>.Failure(new EngineError(
                EngineErrorCode.UnsupportedGameRelease,
                $"No registered game adapter supports {request.Game} with engine release {request.Release}."));
        }

        if (matchingAdapters.Length > 1)
        {
            return EngineResult<IFormListGameAdapter>.Failure(new EngineError(
                EngineErrorCode.UnexpectedFailure,
                $"Multiple registered game adapters support {request.Game} with engine release {request.Release}."));
        }

        return EngineResult<IFormListGameAdapter>.Success(matchingAdapters[0]);
    }

    /// <summary>Canonicalizes and verifies one required existing file.</summary>
    /// <param name="path">The path supplied by the caller.</param>
    /// <param name="description">A stable description used in failures.</param>
    /// <param name="canonicalPath">The canonical absolute path when successful.</param>
    /// <param name="error">The validation failure when unsuccessful.</param>
    /// <returns><see langword="true"/> when the path is valid and names an existing file.</returns>
    private static bool TryCanonicalizeFile(
        string? path,
        string description,
        out string canonicalPath,
        out string error)
    {
        if (!TryCanonicalizePath(path, description, out canonicalPath, out error))
        {
            return false;
        }

        if (!File.Exists(canonicalPath))
        {
            error = $"The {description} does not exist: '{canonicalPath}'.";
            return false;
        }

        return true;
    }

    /// <summary>Canonicalizes and verifies one required existing directory.</summary>
    /// <param name="path">The path supplied by the caller.</param>
    /// <param name="description">A stable description used in failures.</param>
    /// <param name="canonicalPath">The canonical absolute path when successful.</param>
    /// <param name="error">The validation failure when unsuccessful.</param>
    /// <returns><see langword="true"/> when the path is valid and names an existing directory.</returns>
    private static bool TryCanonicalizeDirectory(
        string? path,
        string description,
        out string canonicalPath,
        out string error)
    {
        if (!TryCanonicalizePath(path, description, out canonicalPath, out error))
        {
            return false;
        }

        canonicalPath = Path.TrimEndingDirectorySeparator(canonicalPath);
        if (!Directory.Exists(canonicalPath))
        {
            error = $"The {description} does not exist: '{canonicalPath}'.";
            return false;
        }

        return true;
    }

    /// <summary>Converts a non-empty path to an absolute normalized path without consulting configuration.</summary>
    /// <param name="path">The path supplied by the caller.</param>
    /// <param name="description">A stable description used in failures.</param>
    /// <param name="canonicalPath">The canonical absolute path when successful.</param>
    /// <param name="error">The validation failure when unsuccessful.</param>
    /// <returns><see langword="true"/> when canonicalization succeeds.</returns>
    private static bool TryCanonicalizePath(
        string? path,
        string description,
        out string canonicalPath,
        out string error)
    {
        canonicalPath = string.Empty;
        error = string.Empty;
        if (string.IsNullOrWhiteSpace(path))
        {
            error = $"A {description} path is required.";
            return false;
        }

        try
        {
            canonicalPath = Path.GetFullPath(path);
            return true;
        }
        catch (Exception exception) when (
            exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            error = $"The {description} path is invalid: '{path}'.";
            return false;
        }
    }

    /// <summary>Parses a plugin identity from the canonical plugin file name.</summary>
    /// <param name="canonicalPath">The canonical existing plugin path.</param>
    /// <param name="modKey">The parsed plugin identity when successful.</param>
    /// <param name="error">The validation failure when unsuccessful.</param>
    /// <returns><see langword="true"/> when the file name is a valid plugin identity.</returns>
    private static bool TryReadModKey(string canonicalPath, out ModKey modKey, out string error)
    {
        var pluginFileName = Path.GetFileName(canonicalPath);
        if (ModKey.TryFromNameAndExtension(pluginFileName, out modKey, out var parsingError))
        {
            error = string.Empty;
            return true;
        }

        error = $"The plugin file name '{pluginFileName}' is invalid: {parsingError}";
        return false;
    }

    /// <summary>Creates a stable invalid-request result.</summary>
    /// <param name="message">The validation failure description.</param>
    /// <returns>A failed canonical-request result.</returns>
    private static EngineResult<WorkspaceOpenRequest> InvalidRequest(string message)
    {
        return EngineResult<WorkspaceOpenRequest>.Failure(new EngineError(
            EngineErrorCode.InvalidRequest,
            message));
    }

    /// <summary>Disposes engine state that was acquired but not returned to the caller.</summary>
    /// <param name="workspace">A constructed workspace whose ownership was not returned.</param>
    /// <param name="sourceOpenResult">A source result whose ownership was not transferred to a workspace.</param>
    /// <param name="workspaceId">The requested workspace identifier for cleanup diagnostics.</param>
    /// <returns>A task that completes after acquired state is released or a cleanup failure is logged.</returns>
    private async ValueTask DisposeFailedOpenStateAsync(
        PluginWorkspace? workspace,
        PluginSourceOpenResult? sourceOpenResult,
        Guid? workspaceId)
    {
        try
        {
            if (workspace is not null)
            {
                await workspace.DisposeAsync();
            }
            else if (sourceOpenResult is not null)
            {
                await sourceOpenResult.Sources.DisposeAsync();
            }
        }
        catch (Exception exception)
        {
            Logger.Error(
                exception,
                "Failed to dispose engine state after workspace {WorkspaceId} did not open.",
                workspaceId);
        }
    }
}
