using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using CreationsForge.Core.Engine.Contracts;
using Microsoft.Win32.SafeHandles;

namespace CreationsForge.Core.Engine.Internal;

/// <summary>Provides isolated Darwin descriptor opens, stable identity, object-type metadata, and physical path resolution.</summary>
internal static class DarwinFileSystemInterop
{
    /// <summary>The stable identity provider shared by all macOS file-system inspectors.</summary>
    private const string IdentityProvider = "macos-fstat-v1";

    /// <summary>Opens a Darwin object for read access.</summary>
    private const int OpenReadOnly = 0x00000000;

    /// <summary>Prevents a Darwin guard reopen from waiting on a FIFO or device.</summary>
    private const int OpenNonBlocking = 0x00000004;

    /// <summary>Rejects a symbolic link in the final Darwin path component.</summary>
    private const int OpenNoFollow = 0x00000100;

    /// <summary>Requires a Darwin directory rather than another file-system object.</summary>
    private const int OpenDirectoryFlag = 0x00100000;

    /// <summary>Prevents a retained Darwin descriptor from leaking across process execution.</summary>
    private const int OpenCloseOnExec = 0x01000000;

    /// <summary>The fixed libproc flavor that returns vnode metadata and the path for one descriptor.</summary>
    private const int ProcPidFdVnodePathInformation = 2;

    /// <summary>The LP64 size of Darwin's fixed-width <c>proc_fileinfo</c> structure.</summary>
    private const int ProcFileInformationSize = 24;

    /// <summary>The LP64 size of Darwin's fixed-width <c>vnode_info</c> structure.</summary>
    private const int VnodeInformationSize = 152;

    /// <summary>The fixed path-buffer length embedded in Darwin's <c>vnode_info_path</c> structure.</summary>
    private const int MaximumPathLength = 1024;

    /// <summary>The LP64 offset of <c>vip_path</c> within Darwin's <c>vnode_fdinfowithpath</c>.</summary>
    private const int VnodePathOffset = ProcFileInformationSize + VnodeInformationSize;

    /// <summary>The exact LP64 size of Darwin's <c>vnode_fdinfowithpath</c> structure.</summary>
    private const int VnodePathInformationSize = VnodePathOffset + MaximumPathLength;

    /// <summary>Masks the POSIX file type from a Darwin mode value.</summary>
    private const ushort FileTypeMask = 0xf000;

    /// <summary>Identifies a Darwin directory in a mode value.</summary>
    private const ushort DirectoryType = 0x4000;

    /// <summary>Identifies a Darwin regular file in a mode value.</summary>
    private const ushort RegularFileType = 0x8000;

    /// <summary>Opens a Darwin directory descriptor without following its final path component.</summary>
    /// <param name="path">The canonical existing directory path.</param>
    /// <returns>The owned descriptor handle, which is invalid when Darwin rejects the open.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty.</exception>
    internal static SafeFileHandle OpenDirectory(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return OpenHandle(path, OpenReadOnly | OpenDirectoryFlag | OpenNoFollow | OpenCloseOnExec);
    }

    /// <summary>Reopens a Darwin guard without following a symbolic link or waiting on a special file.</summary>
    /// <param name="path">The canonical existing guard path.</param>
    /// <returns>The owned descriptor handle, which is invalid when Darwin rejects the open.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty.</exception>
    internal static SafeFileHandle OpenGuardForIdentityVerification(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        return OpenHandle(path, OpenReadOnly | OpenNonBlocking | OpenNoFollow | OpenCloseOnExec);
    }

    /// <summary>Reads the stable identity and file type held by a Darwin descriptor.</summary>
    /// <param name="handle">The open Darwin file-system handle.</param>
    /// <returns>The identity-bearing fields returned by Darwin <c>fstat</c>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle"/> is invalid.</exception>
    /// <exception cref="PlatformNotSupportedException">Thrown when the current Darwin architecture is not x64 or arm64.</exception>
    /// <exception cref="Win32Exception">Thrown when Darwin cannot read descriptor metadata.</exception>
    internal static DarwinFileStatus ReadStatus(SafeFileHandle handle)
    {
        if (handle.IsInvalid)
        {
            throw new ArgumentException("A valid Darwin descriptor is required.", nameof(handle));
        }

        EnsureSupportedArchitecture();
        var descriptor = handle.DangerousGetHandle().ToInt32();
        DarwinStat information;
        int result;
        if (RuntimeInformation.ProcessArchitecture == Architecture.X64)
        {
            result = FStatX64(descriptor, out information);
        }
        else
        {
            result = FStatArm64(descriptor, out information);
        }

        if (result != 0)
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError(), "Darwin could not read descriptor identity with fstat.");
        }

        return new DarwinFileStatus(
            information.Device,
            information.Inode,
            information.LinkCount,
            information.Mode);
    }

    /// <summary>Resolves the physical path held by a Darwin descriptor through fixed-signature libproc metadata.</summary>
    /// <param name="handle">The open Darwin file-system handle.</param>
    /// <returns>The normalized absolute physical path represented by the descriptor.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="handle"/> is invalid.</exception>
    /// <exception cref="PlatformNotSupportedException">Thrown when the current Darwin architecture is not x64 or arm64.</exception>
    /// <exception cref="Win32Exception">Thrown when Darwin cannot resolve the descriptor path.</exception>
    /// <exception cref="IOException">Thrown when Darwin returns an invalid or unterminated path.</exception>
    internal static string ResolvePath(SafeFileHandle handle)
    {
        if (handle.IsInvalid)
        {
            throw new ArgumentException("A valid Darwin descriptor is required.", nameof(handle));
        }

        EnsureSupportedArchitecture();
        var buffer = new byte[VnodePathInformationSize];
        var bytesWritten = ProcPidFdInfo(
            Environment.ProcessId,
            handle.DangerousGetHandle().ToInt32(),
            ProcPidFdVnodePathInformation,
            buffer,
            buffer.Length);
        if (bytesWritten <= 0)
        {
            throw new Win32Exception(
                Marshal.GetLastPInvokeError(),
                "Darwin could not resolve a descriptor path with proc_pidfdinfo.");
        }

        if (bytesWritten != VnodePathInformationSize)
        {
            throw new IOException(
                $"Darwin returned {bytesWritten} descriptor-information bytes instead of the expected {VnodePathInformationSize} bytes.");
        }

        var terminator = Array.IndexOf(buffer, (byte)0, VnodePathOffset, MaximumPathLength);
        if (terminator <= VnodePathOffset)
        {
            throw new IOException("Darwin returned an empty or unterminated descriptor path.");
        }

        return Path.GetFullPath(Encoding.UTF8.GetString(buffer, VnodePathOffset, terminator - VnodePathOffset));
    }

    /// <summary>Rejects Darwin architectures whose native layouts or fstat symbols are not represented by this helper.</summary>
    /// <exception cref="PlatformNotSupportedException">Thrown when the current Darwin process is not x64 or arm64.</exception>
    private static void EnsureSupportedArchitecture()
    {
        if (RuntimeInformation.ProcessArchitecture is not Architecture.X64 and not Architecture.Arm64)
        {
            throw new PlatformNotSupportedException(
                $"Darwin file identity is not supported for {RuntimeInformation.ProcessArchitecture} processes.");
        }
    }

    /// <summary>Opens one Darwin path with caller-selected Darwin-only flags.</summary>
    /// <param name="path">The canonical existing path.</param>
    /// <param name="flags">The Darwin open flags.</param>
    /// <returns>The owned descriptor handle, which is invalid when Darwin rejects the open.</returns>
    private static SafeFileHandle OpenHandle(string path, int flags)
    {
        var descriptor = Open(path, flags);
        return new SafeFileHandle(new IntPtr(descriptor), ownsHandle: true);
    }

    /// <summary>Opens a Darwin file-system object.</summary>
    /// <param name="path">The canonical path.</param>
    /// <param name="flags">The Darwin open flags.</param>
    /// <returns>The opened descriptor, or minus one on failure.</returns>
    [DllImport("libc", EntryPoint = "open", SetLastError = true)]
    private static extern int Open([MarshalAs(UnmanagedType.LPUTF8Str)] string path, int flags);

    /// <summary>Reads x64 Darwin descriptor identity using the 64-bit inode symbol selected by Apple headers.</summary>
    /// <param name="descriptor">The open descriptor.</param>
    /// <param name="information">The returned 64-bit inode metadata.</param>
    /// <returns>Zero on success or minus one on failure.</returns>
    [DllImport("libc", EntryPoint = "fstat$INODE64", SetLastError = true)]
    private static extern int FStatX64(int descriptor, out DarwinStat information);

    /// <summary>Reads arm64 Darwin descriptor identity through the unsuffixed 64-bit-only inode symbol.</summary>
    /// <param name="descriptor">The open descriptor.</param>
    /// <param name="information">The returned 64-bit inode metadata.</param>
    /// <returns>Zero on success or minus one on failure.</returns>
    [DllImport("libc", EntryPoint = "fstat", SetLastError = true)]
    private static extern int FStatArm64(int descriptor, out DarwinStat information);

    /// <summary>Reads fixed-width vnode path information for one descriptor owned by a process.</summary>
    /// <param name="processId">The process that owns the descriptor.</param>
    /// <param name="descriptor">The open descriptor.</param>
    /// <param name="flavor">The fixed libproc descriptor-information flavor.</param>
    /// <param name="buffer">The exact output buffer for the selected flavor.</param>
    /// <param name="bufferSize">The output buffer size in bytes.</param>
    /// <returns>The number of bytes written, or zero on failure with the native error retained.</returns>
    [DllImport("/usr/lib/libproc.dylib", EntryPoint = "proc_pidfdinfo", SetLastError = true)]
    private static extern int ProcPidFdInfo(
        int processId,
        int descriptor,
        int flavor,
        [Out] byte[] buffer,
        int bufferSize);

    /// <summary>The stable leading fields of Darwin's 64-bit-inode <c>struct stat</c>.</summary>
    [StructLayout(LayoutKind.Sequential, Size = 144)]
    private struct DarwinStat
    {
        /// <summary>The containing device identifier.</summary>
        internal int Device;

        /// <summary>The file type and permission mode.</summary>
        internal ushort Mode;

        /// <summary>The observed hard-link count.</summary>
        internal ushort LinkCount;

        /// <summary>The 64-bit inode identifier.</summary>
        internal ulong Inode;
    }

    /// <summary>Represents stable identity and object-type fields captured from a Darwin descriptor.</summary>
    internal readonly struct DarwinFileStatus
    {
        /// <summary>Initializes one captured Darwin descriptor status.</summary>
        /// <param name="device">The containing device identifier.</param>
        /// <param name="inode">The file-system inode identifier.</param>
        /// <param name="linkCount">The observed physical hard-link count.</param>
        /// <param name="mode">The file type and permission mode.</param>
        internal DarwinFileStatus(int device, ulong inode, ushort linkCount, ushort mode)
        {
            Device = device;
            Inode = inode;
            LinkCount = linkCount;
            Mode = mode;
        }

        /// <summary>Gets the containing device identifier.</summary>
        internal int Device { get; }

        /// <summary>Gets the file-system inode identifier.</summary>
        internal ulong Inode { get; }

        /// <summary>Gets the observed physical hard-link count.</summary>
        internal ushort LinkCount { get; }

        /// <summary>Gets the file type and permission mode.</summary>
        internal ushort Mode { get; }

        /// <summary>Gets whether the descriptor identifies a directory.</summary>
        internal bool IsDirectory => (Mode & FileTypeMask) == DirectoryType;

        /// <summary>Gets whether the descriptor identifies a regular file.</summary>
        internal bool IsRegularFile => (Mode & FileTypeMask) == RegularFileType;

        /// <summary>Projects the captured device, inode, and link count into the shared artifact identity contract.</summary>
        /// <returns>The stable macOS physical identity.</returns>
        internal ArtifactFileIdentity CreateIdentity()
        {
            return new ArtifactFileIdentity(
                IdentityProvider,
                unchecked((uint)Device).ToString("X8"),
                Inode.ToString("X16"),
                LinkCount);
        }
    }
}
