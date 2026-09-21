using System.Runtime.InteropServices;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.PluginInputs;

/// <summary>Provides platform-specific assertions for workspace source-file locks.</summary>
internal static class SourceFileLockAssertions
{
    /// <summary>The nonblocking exclusive lock operation used by Linux and macOS.</summary>
    private const int LockExclusiveNonBlocking = 0x02 | 0x04;

    /// <summary>The unlock operation used by Linux and macOS.</summary>
    private const int LockUnlock = 0x08;

    /// <summary>Verifies a retained workspace source lock denies a cooperating writer.</summary>
    /// <param name="path">The locked source path.</param>
    internal static void AssertWriteDenied(string path)
    {
        if (OperatingSystem.IsWindows())
        {
            Should.Throw<IOException>(() =>
            {
                using var ignored = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Write,
                    FileShare.Read);
            });
            return;
        }

        using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.ReadWrite,
            FileShare.ReadWrite | FileShare.Delete);
        var result = Flock(stream.SafeFileHandle.DangerousGetHandle().ToInt32(), LockExclusiveNonBlocking);
        if (result == 0)
        {
            _ = Flock(stream.SafeFileHandle.DangerousGetHandle().ToInt32(), LockUnlock);
        }

        result.ShouldBe(-1);
    }

    /// <summary>Verifies disposal releases the source path for a cooperating writer.</summary>
    /// <param name="path">The released source path.</param>
    internal static void AssertWriteAllowed(string path)
    {
        using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.ReadWrite,
            FileShare.ReadWrite | FileShare.Delete);
        if (OperatingSystem.IsWindows())
        {
            stream.CanWrite.ShouldBeTrue();
            return;
        }

        var result = Flock(stream.SafeFileHandle.DangerousGetHandle().ToInt32(), LockExclusiveNonBlocking);
        result.ShouldBe(0);
        Flock(stream.SafeFileHandle.DangerousGetHandle().ToInt32(), LockUnlock).ShouldBe(0);
    }

    /// <summary>Applies or releases a BSD-style whole-file advisory lock on Linux and macOS.</summary>
    /// <param name="fileDescriptor">The open file descriptor.</param>
    /// <param name="operation">The exclusive, nonblocking, or unlock operation flags.</param>
    /// <returns>Zero on success or minus one on failure.</returns>
    [DllImport("libc", EntryPoint = "flock", SetLastError = true)]
    private static extern int Flock(int fileDescriptor, int operation);
}
