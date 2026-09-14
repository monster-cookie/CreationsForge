using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.Persistence;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Persistence;

/// <summary>Verifies output-directory lease metadata, identity, contention, cancellation, and cleanup behavior.</summary>
public sealed class OutputDirectoryLeaseProviderTests
{
    /// <summary>The stable direct-child guard file owned by the provider.</summary>
    private const string GuardFileName = ".creationsforge-output.lock";

    /// <summary>Verifies that existing-only acquisition reports normal absence without creating any file-system object.</summary>
    [Fact]
    public async Task AcquireAsync_ExistingOnlyWithoutGuard_ReturnsGuardNotFoundWithoutCreatingMetadata()
    {
        using var tempDirectory = TemporaryDirectory.Create();
        var provider = new OutputDirectoryLeaseProvider();

        var result = await provider.AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.ExistingOnly,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        result.Value.ShouldNotBeNull();
        result.Value.Status.ShouldBe(OutputDirectoryLeaseAcquisitionStatus.GuardNotFound);
        result.Value.Lease.ShouldBeNull();
        Directory.GetFileSystemEntries(tempDirectory.FullName).ShouldBeEmpty();
    }

    /// <summary>Verifies that create-or-open creates one stable guard and existing-only can subsequently lease it.</summary>
    [Fact]
    public async Task AcquireAsync_CreateOrOpen_CreatesAndRetainsOneStableGuard()
    {
        using var tempDirectory = TemporaryDirectory.Create();
        var provider = new OutputDirectoryLeaseProvider();
        var canonicalPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(tempDirectory.FullName));

        var created = await provider.AcquireAsync(
            Path.Combine(tempDirectory.FullName, "."),
            OutputDirectoryLeaseMode.CreateOrOpen,
            TestContext.Current.CancellationToken);

        created.Succeeded.ShouldBeTrue(created.Error?.Message);
        created.Value.ShouldNotBeNull();
        created.Value.Status.ShouldBe(OutputDirectoryLeaseAcquisitionStatus.Acquired);
        created.Value.Lease.ShouldNotBeNull();
        created.Value.Lease.OutputDirectoryPath.ShouldBe(canonicalPath);
        await created.Value.Lease.DisposeAsync();

        Directory.GetFiles(tempDirectory.FullName).ShouldBe([Path.Combine(canonicalPath, GuardFileName)]);
        var reopened = await provider.AcquireAsync(
            canonicalPath,
            OutputDirectoryLeaseMode.ExistingOnly,
            TestContext.Current.CancellationToken);

        reopened.Succeeded.ShouldBeTrue(reopened.Error?.Message);
        reopened.Value!.Status.ShouldBe(OutputDirectoryLeaseAcquisitionStatus.Acquired);
        await reopened.Value.Lease!.DisposeAsync();
        File.Exists(Path.Combine(canonicalPath, GuardFileName)).ShouldBeTrue();
    }

    /// <summary>Verifies that no mode creates a missing output directory.</summary>
    [Theory]
    [InlineData(OutputDirectoryLeaseMode.CreateOrOpen)]
    [InlineData(OutputDirectoryLeaseMode.ExistingOnly)]
    public async Task AcquireAsync_WhenDirectoryIsMissing_ReturnsInvalidRequestWithoutCreation(
        OutputDirectoryLeaseMode mode)
    {
        using var tempDirectory = TemporaryDirectory.Create();
        var missingPath = Path.Combine(tempDirectory.FullName, "Missing");

        var result = await new OutputDirectoryLeaseProvider().AcquireAsync(
            missingPath,
            mode,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        Directory.Exists(missingPath).ShouldBeFalse();
    }

    /// <summary>Verifies that undefined acquisition modes fail before guard metadata is created.</summary>
    [Fact]
    public async Task AcquireAsync_WithUndefinedMode_ReturnsInvalidRequestWithoutCreation()
    {
        using var tempDirectory = TemporaryDirectory.Create();

        var result = await new OutputDirectoryLeaseProvider().AcquireAsync(
            tempDirectory.FullName,
            (OutputDirectoryLeaseMode)int.MaxValue,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        Directory.GetFileSystemEntries(tempDirectory.FullName).ShouldBeEmpty();
    }

    /// <summary>Verifies that independent provider instances serialize ownership of the same canonical directory.</summary>
    [Fact]
    public async Task AcquireAsync_AcrossProviderInstances_SerializesByCanonicalDirectory()
    {
        using var tempDirectory = TemporaryDirectory.Create();
        var firstProvider = new OutputDirectoryLeaseProvider();
        var secondProvider = new OutputDirectoryLeaseProvider();
        var first = await firstProvider.AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.CreateOrOpen,
            TestContext.Current.CancellationToken);
        first.Succeeded.ShouldBeTrue(first.Error?.Message);
        var firstLease = first.Value!.Lease!;

        var secondTask = secondProvider.AcquireAsync(
            Path.Combine(tempDirectory.FullName, "."),
            OutputDirectoryLeaseMode.ExistingOnly,
            TestContext.Current.CancellationToken).AsTask();
        await Task.Delay(TimeSpan.FromMilliseconds(100), TestContext.Current.CancellationToken);
        secondTask.IsCompleted.ShouldBeFalse();

        await firstLease.DisposeAsync();
        var second = await secondTask.WaitAsync(
            TimeSpan.FromSeconds(5),
            TestContext.Current.CancellationToken);
        second.Succeeded.ShouldBeTrue(second.Error?.Message);
        second.Value!.Status.ShouldBe(OutputDirectoryLeaseAcquisitionStatus.Acquired);
        await second.Value.Lease!.DisposeAsync();
    }

    /// <summary>Verifies that a canceled waiter leaves no stale process gate and a later caller can acquire normally.</summary>
    [Fact]
    public async Task AcquireAsync_WhenWaiterIsCanceled_CleansGateForLaterAcquisition()
    {
        using var tempDirectory = TemporaryDirectory.Create();
        var provider = new OutputDirectoryLeaseProvider();
        var first = await provider.AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.CreateOrOpen,
            TestContext.Current.CancellationToken);
        first.Succeeded.ShouldBeTrue(first.Error?.Message);

        using var canceledWaiter = CancellationTokenSource.CreateLinkedTokenSource(
            TestContext.Current.CancellationToken);
        var waitingTask = new OutputDirectoryLeaseProvider().AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.ExistingOnly,
            canceledWaiter.Token).AsTask();
        canceledWaiter.Cancel();
        await Should.ThrowAsync<OperationCanceledException>(async () => await waitingTask);

        await first.Value!.Lease!.DisposeAsync();
        var later = await provider.AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.ExistingOnly,
            TestContext.Current.CancellationToken);
        later.Succeeded.ShouldBeTrue(later.Error?.Message);
        await later.Value!.Lease!.DisposeAsync();
    }

    /// <summary>Verifies that cancellation observed before acquisition creates no stable guard metadata.</summary>
    [Fact]
    public async Task AcquireAsync_WhenAlreadyCanceled_ThrowsWithoutCreatingMetadata()
    {
        using var tempDirectory = TemporaryDirectory.Create();
        using var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(
            TestContext.Current.CancellationToken);
        cancellationSource.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(() =>
            new OutputDirectoryLeaseProvider().AcquireAsync(
                tempDirectory.FullName,
                OutputDirectoryLeaseMode.CreateOrOpen,
                cancellationSource.Token).AsTask());

        Directory.GetFileSystemEntries(tempDirectory.FullName).ShouldBeEmpty();
    }

    /// <summary>Verifies that an independently held guard is reported as typed output-directory contention.</summary>
    [Fact]
    public async Task AcquireAsync_WhenGuardIsAlreadyExclusivelyOpen_ReturnsOutputDirectoryBusy()
    {
        using var tempDirectory = TemporaryDirectory.Create();
        var guardPath = Path.Combine(tempDirectory.FullName, GuardFileName);
        await using var externalOwner = new FileStream(
            guardPath,
            FileMode.CreateNew,
            FileAccess.ReadWrite,
            FileShare.None,
            bufferSize: 1,
            FileOptions.Asynchronous);

        var result = await new OutputDirectoryLeaseProvider().AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.ExistingOnly,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(EngineErrorCode.OutputDirectoryBusy);
    }

    /// <summary>Verifies that lease disposal is idempotent and releases both file-system and process ownership.</summary>
    [Fact]
    public async Task DisposeAsync_WhenCalledTwice_ReleasesOwnershipExactlyOnce()
    {
        using var tempDirectory = TemporaryDirectory.Create();
        var provider = new OutputDirectoryLeaseProvider();
        var result = await provider.AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.CreateOrOpen,
            TestContext.Current.CancellationToken);
        result.Succeeded.ShouldBeTrue(result.Error?.Message);
        var lease = result.Value!.Lease!;

        await lease.DisposeAsync();
        await lease.DisposeAsync();

        var reacquired = await provider.AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.ExistingOnly,
            TestContext.Current.CancellationToken);
        reacquired.Succeeded.ShouldBeTrue(reacquired.Error?.Message);
        await reacquired.Value!.Lease!.DisposeAsync();
    }

    /// <summary>Verifies that a symbolic-link output directory is rejected before guard creation.</summary>
    [Fact]
    public async Task AcquireAsync_WithSymbolicLinkDirectory_ReturnsUnsupportedInput()
    {
        using var tempDirectory = TemporaryDirectory.Create();
        var targetPath = Directory.CreateDirectory(Path.Combine(tempDirectory.FullName, "Target")).FullName;
        var linkPath = Path.Combine(tempDirectory.FullName, "Alias");
        try
        {
            Directory.CreateSymbolicLink(linkPath, targetPath);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or PlatformNotSupportedException)
        {
            Assert.Skip($"The host could not create a directory symbolic-link fixture: {exception.Message}");
        }

        var result = await new OutputDirectoryLeaseProvider().AcquireAsync(
            linkPath,
            OutputDirectoryLeaseMode.CreateOrOpen,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(EngineErrorCode.UnsupportedInput);
        File.Exists(Path.Combine(targetPath, GuardFileName)).ShouldBeFalse();
    }

    /// <summary>Verifies that a symbolic-link guard is rejected without following its target.</summary>
    [Fact]
    public async Task AcquireAsync_WithSymbolicLinkGuard_ReturnsUnsupportedInput()
    {
        using var tempDirectory = TemporaryDirectory.Create();
        var targetPath = Path.Combine(tempDirectory.FullName, "Target.lock");
        await File.WriteAllBytesAsync(targetPath, [], TestContext.Current.CancellationToken);
        var guardPath = Path.Combine(tempDirectory.FullName, GuardFileName);
        try
        {
            File.CreateSymbolicLink(guardPath, targetPath);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or PlatformNotSupportedException)
        {
            Assert.Skip($"The host could not create a file symbolic-link fixture: {exception.Message}");
        }

        var result = await new OutputDirectoryLeaseProvider().AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.ExistingOnly,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(EngineErrorCode.UnsupportedInput);
    }

    /// <summary>Verifies that a hard-linked guard is rejected as ambiguous physical metadata on supported hosts.</summary>
    /// <returns>A task that completes after the hard-linked guard is rejected.</returns>
    [Fact]
    public async Task AcquireAsync_WithHardLinkedGuard_ReturnsUnsupportedInputOnSupportedHosts()
    {
        Assert.SkipUnless(
            OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS(),
            "Output-directory physical identity is supported only on Windows, Linux, and macOS.");

        using var tempDirectory = TemporaryDirectory.Create();
        var targetPath = Path.Combine(tempDirectory.FullName, "Target.lock");
        await File.WriteAllBytesAsync(targetPath, [], TestContext.Current.CancellationToken);
        var guardPath = Path.Combine(tempDirectory.FullName, GuardFileName);
        var linkCreated = OperatingSystem.IsWindows()
            ? CreateHardLinkWindows(guardPath, targetPath, IntPtr.Zero)
            : CreateHardLinkUnix(targetPath, guardPath) == 0;
        if (!linkCreated)
        {
            throw new InvalidOperationException(
                $"Could not create the output-guard hard-link fixture: {Marshal.GetLastPInvokeError()}.");
        }

        var result = await new OutputDirectoryLeaseProvider().AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.ExistingOnly,
            TestContext.Current.CancellationToken);

        result.Succeeded.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error.Code.ShouldBe(EngineErrorCode.UnsupportedInput);
    }

    /// <summary>Verifies that Unix rejects a guard replaced after its descriptor is opened and releases acquired resources.</summary>
    /// <returns>A task that completes after replacement detection and resource release are validated.</returns>
    [Fact]
    public async Task AcquireAsync_OnUnix_WhenOpenedGuardIsReplaced_ReturnsExternalChangeAndReleasesResources()
    {
        Assert.SkipUnless(
            OperatingSystem.IsLinux() || OperatingSystem.IsMacOS(),
            "This regression exercises Unix descriptor and path identity.");

        using var tempDirectory = TemporaryDirectory.Create();
        var movedGuardPath = Path.Combine(tempDirectory.FullName, "Moved.lock");
        var provider = CreateProviderWithGuardOpenedObserver(guardPath =>
        {
            File.Move(guardPath, movedGuardPath);
            File.WriteAllBytes(guardPath, []);
        });

        var replaced = await provider.AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.CreateOrOpen,
            TestContext.Current.CancellationToken);

        replaced.Succeeded.ShouldBeFalse();
        replaced.Error.ShouldNotBeNull();
        replaced.Error.Code.ShouldBe(EngineErrorCode.ExternalChangeDetected);

        await using (var releasedMovedGuard = new FileStream(
                         movedGuardPath,
                         FileMode.Open,
                         FileAccess.ReadWrite,
                         FileShare.None,
                         bufferSize: 1,
                         FileOptions.Asynchronous))
        {
        }

        var retry = await new OutputDirectoryLeaseProvider().AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.ExistingOnly,
            TestContext.Current.CancellationToken);
        retry.Succeeded.ShouldBeTrue(retry.Error?.Message);
        await retry.Value!.Lease!.DisposeAsync();
    }

    /// <summary>Verifies that a Unix FIFO substituted for an opened guard is rejected promptly and releases the lease gate.</summary>
    /// <returns>A task that completes after prompt special-file rejection and resource release are validated.</returns>
    [Fact(Timeout = 15000)]
    public async Task AcquireAsync_OnUnix_WhenOpenedGuardIsReplacedWithFifo_RejectsWithoutBlocking()
    {
        Assert.SkipUnless(
            OperatingSystem.IsLinux() || OperatingSystem.IsMacOS(),
            "This regression exercises Unix FIFO and descriptor behavior.");

        using var tempDirectory = TemporaryDirectory.Create();
        var guardPath = Path.Combine(tempDirectory.FullName, GuardFileName);
        var movedGuardPath = Path.Combine(tempDirectory.FullName, "Moved.lock");
        var provider = CreateProviderWithGuardOpenedObserver(openedGuardPath =>
        {
            File.Move(openedGuardPath, movedGuardPath);
            if (MakeFifo(openedGuardPath, 0x180) != 0)
            {
                throw new InvalidOperationException(
                    $"Could not create the FIFO replacement fixture: {Marshal.GetLastPInvokeError()}.");
            }
        });

        var acquisition = Task.Run(async () => await provider.AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.CreateOrOpen,
            TestContext.Current.CancellationToken));
        try
        {
            var replaced = await acquisition.WaitAsync(
                TimeSpan.FromSeconds(5),
                TestContext.Current.CancellationToken);
            replaced.Succeeded.ShouldBeFalse();
            replaced.Error.ShouldNotBeNull();
            replaced.Error.Code.ShouldBe(EngineErrorCode.UnsupportedInput);
        }
        finally
        {
            if (!acquisition.IsCompleted)
            {
                // Unix permits opening a FIFO for read/write without another peer, releasing a regressed blocking read.
                using var releaseBlockedOpen = new FileStream(guardPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                await acquisition.WaitAsync(TimeSpan.FromSeconds(5));
            }
        }

        File.Delete(guardPath);
        File.Move(movedGuardPath, guardPath);
        var retry = await new OutputDirectoryLeaseProvider().AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.ExistingOnly,
            TestContext.Current.CancellationToken);
        retry.Succeeded.ShouldBeTrue(retry.Error?.Message);
        await retry.Value!.Lease!.DisposeAsync();
    }

    /// <summary>Verifies with a child process that the guard blocks and then permits an independent FileShare.None open.</summary>
    [Fact]
    public async Task AcquireAsync_OnWindows_EnforcesActualCrossProcessExclusion()
    {
        Assert.SkipUnless(OperatingSystem.IsWindows(), "This fixture uses Windows FileShare enforcement.");

        var windowsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        var shellPath = Path.Combine(
            windowsDirectory,
            "System32",
            "WindowsPowerShell",
            "v1.0",
            "powershell.exe");
        Assert.SkipUnless(File.Exists(shellPath), "Windows PowerShell is unavailable for the cross-process probe.");

        using var tempDirectory = TemporaryDirectory.Create();
        var provider = new OutputDirectoryLeaseProvider();
        var acquired = await provider.AcquireAsync(
            tempDirectory.FullName,
            OutputDirectoryLeaseMode.CreateOrOpen,
            TestContext.Current.CancellationToken);
        acquired.Succeeded.ShouldBeTrue(acquired.Error?.Message);
        var lease = acquired.Value!.Lease!;
        var guardPath = Path.Combine(tempDirectory.FullName, GuardFileName);

        try
        {
            var contended = await ProbeGuardFromChildProcessAsync(
                shellPath,
                guardPath,
                TestContext.Current.CancellationToken);
            contended.ExitCode.ShouldBe(23, contended.StandardError);

            await lease.DisposeAsync();
            var released = await ProbeGuardFromChildProcessAsync(
                shellPath,
                guardPath,
                TestContext.Current.CancellationToken);
            released.ExitCode.ShouldBe(0, released.StandardError);
        }
        finally
        {
            await lease.DisposeAsync();
        }
    }

    /// <summary>Attempts one independent exclusive guard open from Windows PowerShell.</summary>
    /// <param name="shellPath">The absolute Windows PowerShell executable path.</param>
    /// <param name="guardPath">The stable guard path to open.</param>
    /// <param name="cancellationToken">The token that bounds child-process execution.</param>
    /// <returns>The child exit code and captured standard error.</returns>
    private static async Task<ChildProcessResult> ProbeGuardFromChildProcessAsync(
        string shellPath,
        string guardPath,
        CancellationToken cancellationToken)
    {
        const string script = "try { $stream = [System.IO.File]::Open($env:CF_LEASE_GUARD_PATH, [System.IO.FileMode]::Open, [System.IO.FileAccess]::ReadWrite, [System.IO.FileShare]::None); $stream.Dispose(); exit 0 } catch [System.IO.IOException] { exit 23 } catch { [Console]::Error.WriteLine($_.Exception.ToString()); exit 24 }";
        var startInfo = new ProcessStartInfo
        {
            FileName = shellPath,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        startInfo.ArgumentList.Add("-NoLogo");
        startInfo.ArgumentList.Add("-NoProfile");
        startInfo.ArgumentList.Add("-NonInteractive");
        startInfo.ArgumentList.Add("-Command");
        startInfo.ArgumentList.Add(script);
        startInfo.Environment["CF_LEASE_GUARD_PATH"] = guardPath;

        using var process = new Process { StartInfo = startInfo };
        process.Start().ShouldBeTrue();
        var standardErrorTask = process.StandardError.ReadToEndAsync(cancellationToken);
        await process.WaitForExitAsync(cancellationToken);
        return new ChildProcessResult(process.ExitCode, await standardErrorTask);
    }

    /// <summary>Creates a provider with a private deterministic observer for the guard-open identity race.</summary>
    /// <param name="guardOpenedObserver">The callback that replaces the opened guard before admission.</param>
    /// <returns>The provider configured with the private acquisition observer.</returns>
    private static OutputDirectoryLeaseProvider CreateProviderWithGuardOpenedObserver(
        Action<string> guardOpenedObserver)
    {
        var constructor = typeof(OutputDirectoryLeaseProvider).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            [typeof(Action<string>)],
            modifiers: null);
        constructor.ShouldNotBeNull();
        return (OutputDirectoryLeaseProvider)constructor!.Invoke([guardOpenedObserver]);
    }

    /// <summary>Creates a Unix FIFO used to prove guard verification cannot wait for a writer.</summary>
    /// <param name="path">The absent guard path to replace with a FIFO.</param>
    /// <param name="mode">The owner-only permission bits for the fixture.</param>
    /// <returns>Zero on success; otherwise minus one with the plugin error available through the runtime.</returns>
    [DllImport("libc", EntryPoint = "mkfifo", SetLastError = true)]
    private static extern int MakeFifo([MarshalAs(UnmanagedType.LPUTF8Str)] string path, uint mode);

    /// <summary>Creates a Windows hard link for guard identity validation.</summary>
    /// <param name="fileName">The new hard-link path.</param>
    /// <param name="existingFileName">The existing target path.</param>
    /// <param name="securityAttributes">Reserved security attributes, always zero.</param>
    /// <returns><see langword="true"/> when Windows creates the link.</returns>
    [DllImport("kernel32.dll", EntryPoint = "CreateHardLinkW", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool CreateHardLinkWindows(
        string fileName,
        string existingFileName,
        IntPtr securityAttributes);

    /// <summary>Creates a Unix hard link for guard identity validation.</summary>
    /// <param name="existingFileName">The existing target path.</param>
    /// <param name="fileName">The new hard-link path.</param>
    /// <returns>Zero when Unix creates the link; otherwise minus one with the native error retained.</returns>
    [DllImport("libc", EntryPoint = "link", SetLastError = true)]
    private static extern int CreateHardLinkUnix(
        [MarshalAs(UnmanagedType.LPUTF8Str)] string existingFileName,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string fileName);

    /// <summary>Captures one child-process guard probe result.</summary>
    private sealed class ChildProcessResult
    {
        /// <summary>Initializes a completed child-process result.</summary>
        /// <param name="exitCode">The process exit code.</param>
        /// <param name="standardError">The captured standard error.</param>
        internal ChildProcessResult(int exitCode, string standardError)
        {
            ExitCode = exitCode;
            StandardError = standardError;
        }

        /// <summary>Gets the process exit code.</summary>
        internal int ExitCode { get; }

        /// <summary>Gets the captured standard error.</summary>
        internal string StandardError { get; }
    }

    /// <summary>Owns one disposable temporary directory used by an isolated lease test.</summary>
    private sealed class TemporaryDirectory : IDisposable
    {
        /// <summary>The owned directory.</summary>
        private readonly DirectoryInfo directory;

        /// <summary>Initializes ownership of one temporary directory.</summary>
        /// <param name="directory">The newly created directory.</param>
        private TemporaryDirectory(DirectoryInfo directory)
        {
            this.directory = directory;
        }

        /// <summary>Gets the absolute temporary-directory path.</summary>
        internal string FullName => directory.FullName;

        /// <summary>Creates and owns one uniquely named temporary directory.</summary>
        /// <returns>The disposable temporary-directory owner.</returns>
        internal static TemporaryDirectory Create()
        {
            return new TemporaryDirectory(Directory.CreateTempSubdirectory());
        }

        /// <summary>Deletes the owned directory and all remaining test metadata.</summary>
        public void Dispose()
        {
            if (directory.Exists)
            {
                directory.Delete(recursive: true);
            }
        }
    }
}
