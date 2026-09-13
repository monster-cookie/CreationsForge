using System.ComponentModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.PluginInputs;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Provides serialized in-process and cooperating cross-process leases for existing output directories.</summary>
public sealed class OutputDirectoryLeaseProvider : IOutputDirectoryLeaseProvider
{
    /// <summary>The stable direct-child file used for cooperating cross-process exclusion.</summary>
    private const string GuardFileName = ".creationsforge-output.lock";

    /// <summary>Observes the narrow interval after the guard is opened and before its identity is admitted.</summary>
    private readonly Action<string>? guardOpenedObserver;

    /// <summary>Initializes an output-directory lease provider backed by process-wide and file-system exclusion.</summary>
    public OutputDirectoryLeaseProvider()
        : this(null)
    {
    }

    /// <summary>Initializes a provider with an acquisition observer used by deterministic identity-race validation.</summary>
    /// <param name="guardOpenedObserver">The callback invoked after exclusive guard open and before final identity admission.</param>
    private OutputDirectoryLeaseProvider(Action<string>? guardOpenedObserver)
    {
        this.guardOpenedObserver = guardOpenedObserver;
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<OutputDirectoryLeaseAcquisition>> AcquireAsync(
        string outputDirectoryPath,
        OutputDirectoryLeaseMode mode,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var canonicalResult = Canonicalize(outputDirectoryPath, mode);
        if (!canonicalResult.Succeeded)
        {
            return EngineResult<OutputDirectoryLeaseAcquisition>.Failure(canonicalResult.Error!);
        }

        var canonicalDirectoryPath = canonicalResult.Value!;
        OutputDirectoryInProcessGate.Releaser? processGate = null;
        OutputDirectoryIdentityHandle? directoryHandle = null;
        FileStream? guardStream = null;
        try
        {
            processGate = await OutputDirectoryInProcessGate.EnterAsync(
                canonicalDirectoryPath,
                cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            PluginFileInspector.VerifyDirectory(canonicalDirectoryPath, "output directory");
            directoryHandle = OutputDirectoryIdentityInspector.OpenDirectory(canonicalDirectoryPath);

            var guardPath = Path.GetFullPath(Path.Combine(canonicalDirectoryPath, GuardFileName));
            RejectGuardAliasIfPresent(guardPath);
            guardStream = OpenGuard(guardPath, mode);
            if (guardStream is null)
            {
                return EngineResult<OutputDirectoryLeaseAcquisition>.Success(
                    new OutputDirectoryLeaseAcquisition(
                        OutputDirectoryLeaseAcquisitionStatus.GuardNotFound,
                        null));
            }

            guardOpenedObserver?.Invoke(guardPath);
            cancellationToken.ThrowIfCancellationRequested();
            RejectGuardAliasIfPresent(guardPath);
            OutputDirectoryIdentityInspector.VerifyGuard(guardPath, guardStream.SafeFileHandle);
            OutputDirectoryIdentityInspector.VerifyDirectoryMatches(canonicalDirectoryPath, directoryHandle);
            cancellationToken.ThrowIfCancellationRequested();

            var lease = new OutputDirectoryLease(
                canonicalDirectoryPath,
                guardStream,
                directoryHandle,
                processGate);
            guardStream = null;
            directoryHandle = null;
            processGate = null;
            return EngineResult<OutputDirectoryLeaseAcquisition>.Success(
                new OutputDirectoryLeaseAcquisition(
                    OutputDirectoryLeaseAcquisitionStatus.Acquired,
                    lease));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (PluginSourceInputException exception)
        {
            var code = exception.Code == EngineErrorCode.SourceOpenFailed
                ? EngineErrorCode.OutputOpenFailed
                : exception.Code;
            return Failure(code, exception.Message);
        }
        catch (OutputDirectoryLeaseException exception)
        {
            return Failure(exception.Code, exception.Message);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or Win32Exception)
        {
            return Failure(
                EngineErrorCode.OutputOpenFailed,
                $"The output-directory lease could not be acquired safely: '{canonicalDirectoryPath}'.");
        }
        catch (Exception)
        {
            return Failure(
                EngineErrorCode.UnexpectedFailure,
                $"An unexpected failure prevented output-directory lease acquisition: '{canonicalDirectoryPath}'.");
        }
        finally
        {
            guardStream?.Dispose();
            directoryHandle?.Dispose();
            processGate?.Dispose();
        }
    }

    /// <summary>Validates the request and returns a normalized absolute directory path.</summary>
    /// <param name="outputDirectoryPath">The caller-supplied directory path.</param>
    /// <param name="mode">The requested stable-guard policy.</param>
    /// <returns>The canonical path or a typed invalid-request failure.</returns>
    private static EngineResult<string> Canonicalize(
        string outputDirectoryPath,
        OutputDirectoryLeaseMode mode)
    {
        if (string.IsNullOrWhiteSpace(outputDirectoryPath))
        {
            return EngineResult<string>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                "An output-directory path is required for lease acquisition."));
        }

        if (!Enum.IsDefined(mode))
        {
            return EngineResult<string>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                "The output-directory lease mode is invalid."));
        }

        try
        {
            return EngineResult<string>.Success(
                Path.TrimEndingDirectorySeparator(Path.GetFullPath(outputDirectoryPath)));
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return EngineResult<string>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                $"The output-directory path is invalid: '{outputDirectoryPath}'."));
        }
    }

    /// <summary>Rejects an existing guard that is a directory, symbolic link, or Windows reparse point.</summary>
    /// <param name="guardPath">The canonical stable guard path.</param>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when the existing guard is unsafe or cannot be inspected.</exception>
    private static void RejectGuardAliasIfPresent(string guardPath)
    {
        try
        {
            var guard = new FileInfo(guardPath);
            if (guard.LinkTarget is not null
                || (guard.Exists && (guard.Attributes & FileAttributes.ReparsePoint) != 0))
            {
                throw new OutputDirectoryLeaseException(
                    EngineErrorCode.UnsupportedInput,
                    $"The output-directory guard is a symbolic link or reparse point: '{guardPath}'.");
            }

            if (Directory.Exists(guardPath))
            {
                throw new OutputDirectoryLeaseException(
                    EngineErrorCode.UnsupportedInput,
                    $"The output-directory guard path identifies a directory instead of a regular file: '{guardPath}'.");
            }
        }
        catch (OutputDirectoryLeaseException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or NotSupportedException)
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.OutputOpenFailed,
                $"The output-directory guard could not be inspected safely: '{guardPath}'.",
                exception);
        }
    }

    /// <summary>Opens the stable guard with exclusive sharing or reports normal existing-only absence.</summary>
    /// <param name="guardPath">The canonical stable guard path.</param>
    /// <param name="mode">Whether absent guard metadata may be created.</param>
    /// <returns>The exclusively opened stream, or <see langword="null"/> for normal existing-only absence.</returns>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when contention or another file-system failure prevents opening the guard.</exception>
    private static FileStream? OpenGuard(string guardPath, OutputDirectoryLeaseMode mode)
    {
        var fileMode = mode == OutputDirectoryLeaseMode.ExistingOnly
            ? FileMode.Open
            : FileMode.OpenOrCreate;
        try
        {
            return new FileStream(
                guardPath,
                fileMode,
                FileAccess.ReadWrite,
                FileShare.None,
                bufferSize: 1,
                FileOptions.Asynchronous);
        }
        catch (FileNotFoundException) when (mode == OutputDirectoryLeaseMode.ExistingOnly)
        {
            return null;
        }
        catch (DirectoryNotFoundException exception)
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.ExternalChangeDetected,
                $"The output directory disappeared while its lease was being acquired: '{Path.GetDirectoryName(guardPath)}'.",
                exception);
        }
        catch (IOException exception) when (IsSharingContention(exception))
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.OutputDirectoryBusy,
                $"Another process or workspace currently owns the output-directory guard: '{guardPath}'.",
                exception);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.OutputOpenFailed,
                $"The output-directory guard could not be opened safely: '{guardPath}'.",
                exception);
        }
    }

    /// <summary>Recognizes platform sharing and non-blocking lock failures without parsing error messages.</summary>
    /// <param name="exception">The file-open failure.</param>
    /// <returns><see langword="true"/> when the plugin error identifies active contention.</returns>
    private static bool IsSharingContention(IOException exception)
    {
        var platformErrorCode = exception.HResult & 0xffff;
        return platformErrorCode is 11 or 13 or 32 or 33;
    }

    /// <summary>Creates a typed lease-acquisition failure.</summary>
    /// <param name="code">The stable error category.</param>
    /// <param name="message">The diagnostic failure description.</param>
    /// <returns>A failed lease-acquisition result.</returns>
    private static EngineResult<OutputDirectoryLeaseAcquisition> Failure(
        EngineErrorCode code,
        string message)
    {
        return EngineResult<OutputDirectoryLeaseAcquisition>.Failure(new EngineError(code, message));
    }
}
