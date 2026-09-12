using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using CreationsForge.Core.Engine.Contracts;
using Microsoft.Win32.SafeHandles;

namespace CreationsForge.Core.Engine.Persistence;

/// <summary>Retains and compares native directory identity while validating stable guard-file identity.</summary>
internal static class OutputDirectoryIdentityInspector
{
    /// <summary>Requests Windows file attributes without content access.</summary>
    private const uint FileReadAttributes = 0x00000080;

    /// <summary>Allows other handles to retain read access to the directory itself.</summary>
    private const uint FileShareRead = 0x00000001;

    /// <summary>Allows other handles to retain write access to the directory itself.</summary>
    private const uint FileShareWrite = 0x00000002;

    /// <summary>Opens an existing Windows file-system object.</summary>
    private const uint OpenExisting = 3;

    /// <summary>Permits opening a Windows directory handle.</summary>
    private const uint FileFlagBackupSemantics = 0x02000000;

    /// <summary>Opens the final Windows reparse point itself so it can be rejected.</summary>
    private const uint FileFlagOpenReparsePoint = 0x00200000;

    /// <summary>Identifies a Windows directory in handle metadata.</summary>
    private const uint FileAttributeDirectory = 0x00000010;

    /// <summary>Identifies a Windows reparse point in handle metadata.</summary>
    private const uint FileAttributeReparsePoint = 0x00000400;

    /// <summary>Requests a normalized DOS path from a Windows handle.</summary>
    private const uint FileNameNormalizedDos = 0;

    /// <summary>Opens a Linux object for read access.</summary>
    private const int OpenReadOnly = 0;

    /// <summary>Prevents opening an unvalidated Linux guard path from waiting on a FIFO or device.</summary>
    private const int OpenNonBlocking = 0x00000800;

    /// <summary>Requires a Linux directory rather than a regular file.</summary>
    private const int OpenDirectoryFlag = 0x00010000;

    /// <summary>Rejects a symbolic link in the final Linux path component.</summary>
    private const int OpenNoFollow = 0x00020000;

    /// <summary>Prevents a retained Linux descriptor from leaking across process execution.</summary>
    private const int OpenCloseOnExec = 0x00080000;

    /// <summary>Reads Linux descriptor metadata when the supplied path is empty.</summary>
    private const int AtEmptyPath = 0x00001000;

    /// <summary>Requests Linux basic identity and metadata fields.</summary>
    private const uint StatxBasicStats = 0x000007ff;

    /// <summary>Masks the Linux file type from its mode.</summary>
    private const ushort LinuxFileTypeMask = 0xf000;

    /// <summary>Identifies a Linux directory.</summary>
    private const ushort LinuxDirectoryType = 0x4000;

    /// <summary>Identifies a Linux regular file.</summary>
    private const ushort LinuxRegularFileType = 0x8000;

    /// <summary>Compares canonical paths with the host file system's casing rules.</summary>
    private static readonly StringComparer PathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    /// <summary>Opens and retains one verified output directory without allowing Windows deletion or rename.</summary>
    /// <param name="canonicalDirectoryPath">The canonical existing output directory.</param>
    /// <returns>The retained native handle and captured stable identity.</returns>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when the platform cannot retain and verify the directory identity.</exception>
    internal static OutputDirectoryIdentityHandle OpenDirectory(string canonicalDirectoryPath)
    {
        if (OperatingSystem.IsWindows())
        {
            return OpenWindowsDirectory(canonicalDirectoryPath);
        }

        if (OperatingSystem.IsLinux())
        {
            return OpenLinuxDirectory(canonicalDirectoryPath);
        }

        throw new OutputDirectoryLeaseException(
            EngineErrorCode.UnsupportedInput,
            "Output-directory lease identity is currently supported only on Windows and Linux.");
    }

    /// <summary>Verifies that the canonical path still identifies the retained directory.</summary>
    /// <param name="canonicalDirectoryPath">The canonical output-directory path.</param>
    /// <param name="retainedDirectory">The directory handle retained before the guard was opened.</param>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when the path is replaced or can no longer be verified.</exception>
    internal static void VerifyDirectoryMatches(
        string canonicalDirectoryPath,
        OutputDirectoryIdentityHandle retainedDirectory)
    {
        using var currentDirectory = OpenDirectory(canonicalDirectoryPath);
        if (!HasSameStableIdentity(retainedDirectory.Identity, currentDirectory.Identity))
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.ExternalChangeDetected,
                $"The output directory path was replaced while its lease was being acquired: '{canonicalDirectoryPath}'.");
        }
    }

    /// <summary>Verifies the opened guard is a canonical single-link regular file.</summary>
    /// <param name="canonicalGuardPath">The expected canonical guard path.</param>
    /// <param name="guardHandle">The exclusively opened guard handle.</param>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when the guard is aliased, linked, or not a regular file.</exception>
    internal static void VerifyGuard(string canonicalGuardPath, SafeFileHandle guardHandle)
    {
        var identity = ReadIdentity(guardHandle, requireDirectory: false);
        if (identity.LinkCount != 1)
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.UnsupportedInput,
                $"The output-directory guard must have exactly one physical link: '{canonicalGuardPath}'.");
        }

        if (OperatingSystem.IsLinux())
        {
            VerifyLinuxGuardPathIdentity(canonicalGuardPath, identity);
        }

        VerifyResolvedPath(canonicalGuardPath, guardHandle, "output-directory guard");
    }

    /// <summary>Opens a Windows directory while denying delete sharing and retaining its physical identity.</summary>
    /// <param name="canonicalDirectoryPath">The canonical existing directory.</param>
    /// <returns>The retained Windows directory handle.</returns>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when Windows cannot open or verify the directory.</exception>
    private static OutputDirectoryIdentityHandle OpenWindowsDirectory(string canonicalDirectoryPath)
    {
        var handle = CreateFile(
            canonicalDirectoryPath,
            FileReadAttributes,
            FileShareRead | FileShareWrite,
            IntPtr.Zero,
            OpenExisting,
            FileFlagBackupSemantics | FileFlagOpenReparsePoint,
            IntPtr.Zero);
        if (handle.IsInvalid)
        {
            var exception = new Win32Exception(Marshal.GetLastPInvokeError());
            handle.Dispose();
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.OutputOpenFailed,
                $"Windows could not retain the output directory for lease acquisition: '{canonicalDirectoryPath}'.",
                exception);
        }

        try
        {
            VerifyResolvedPath(canonicalDirectoryPath, handle, "output directory");
            var identity = ReadIdentity(handle, requireDirectory: true);
            return new OutputDirectoryIdentityHandle(handle, identity);
        }
        catch
        {
            handle.Dispose();
            throw;
        }
    }

    /// <summary>Opens a Linux directory descriptor without following its final path component.</summary>
    /// <param name="canonicalDirectoryPath">The canonical existing directory.</param>
    /// <returns>The retained Linux directory descriptor.</returns>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when Linux cannot open or verify the directory.</exception>
    private static OutputDirectoryIdentityHandle OpenLinuxDirectory(string canonicalDirectoryPath)
    {
        var descriptor = Open(
            canonicalDirectoryPath,
            OpenReadOnly | OpenDirectoryFlag | OpenNoFollow | OpenCloseOnExec);
        if (descriptor < 0)
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.OutputOpenFailed,
                $"Linux could not retain the output directory for lease acquisition: '{canonicalDirectoryPath}'.",
                new Win32Exception(Marshal.GetLastPInvokeError()));
        }

        var handle = new SafeFileHandle(new IntPtr(descriptor), ownsHandle: true);
        try
        {
            VerifyResolvedPath(canonicalDirectoryPath, handle, "output directory");
            var identity = ReadIdentity(handle, requireDirectory: true);
            return new OutputDirectoryIdentityHandle(handle, identity);
        }
        catch
        {
            handle.Dispose();
            throw;
        }
    }

    /// <summary>Verifies that an opened handle resolves to the exact canonical path supplied by the caller.</summary>
    /// <param name="canonicalPath">The expected canonical path.</param>
    /// <param name="handle">The opened native handle.</param>
    /// <param name="description">The object description used in diagnostics.</param>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when path resolution fails or reveals an alias.</exception>
    private static void VerifyResolvedPath(
        string canonicalPath,
        SafeFileHandle handle,
        string description)
    {
        var resolvedPath = OperatingSystem.IsWindows()
            ? ResolveWindowsHandlePath(handle)
            : ResolveLinuxHandlePath(handle);
        var expectedPath = Path.TrimEndingDirectorySeparator(canonicalPath);
        var actualPath = Path.TrimEndingDirectorySeparator(resolvedPath);
        if (!PathComparer.Equals(expectedPath, actualPath))
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.UnsupportedInput,
                $"The {description} resolves through an alias and cannot be leased safely: '{canonicalPath}' -> '{resolvedPath}'.");
        }
    }

    /// <summary>Reads stable identity and validates the expected native object type.</summary>
    /// <param name="handle">The opened file or directory handle.</param>
    /// <param name="requireDirectory">Whether the handle must identify a directory rather than a regular file.</param>
    /// <returns>The native physical identity and hard-link count.</returns>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when identity is unavailable or the native object type is unsafe.</exception>
    private static NativeFileIdentity ReadIdentity(SafeFileHandle handle, bool requireDirectory)
    {
        if (OperatingSystem.IsWindows())
        {
            if (!GetFileInformationByHandle(handle, out var information))
            {
                throw new OutputDirectoryLeaseException(
                    EngineErrorCode.OutputOpenFailed,
                    "Windows could not read output-directory lease identity.",
                    new Win32Exception(Marshal.GetLastPInvokeError()));
            }

            if ((information.FileAttributes & FileAttributeReparsePoint) != 0)
            {
                throw new OutputDirectoryLeaseException(
                    EngineErrorCode.UnsupportedInput,
                    "An output-directory lease object is a Windows reparse point and cannot be used safely.");
            }

            var isDirectory = (information.FileAttributes & FileAttributeDirectory) != 0;
            if (isDirectory != requireDirectory)
            {
                throw new OutputDirectoryLeaseException(
                    EngineErrorCode.UnsupportedInput,
                    requireDirectory
                        ? "The output-directory lease path does not identify a directory."
                        : "The output-directory guard does not identify a regular file.");
            }

            var fileIndex = ((ulong)information.FileIndexHigh << 32) | information.FileIndexLow;
            return new NativeFileIdentity(
                "windows-file-id-v1",
                information.VolumeSerialNumber.ToString("X8"),
                fileIndex.ToString("X16"),
                information.NumberOfLinks);
        }

        if (OperatingSystem.IsLinux())
        {
            var descriptor = handle.DangerousGetHandle().ToInt32();
            if (Statx(descriptor, string.Empty, AtEmptyPath, StatxBasicStats, out var information) != 0)
            {
                throw new OutputDirectoryLeaseException(
                    EngineErrorCode.UnsupportedInput,
                    "Linux could not provide statx identity for an output-directory lease object.",
                    new Win32Exception(Marshal.GetLastPInvokeError()));
            }

            var fileType = (ushort)(information.Mode & LinuxFileTypeMask);
            var expectedType = requireDirectory ? LinuxDirectoryType : LinuxRegularFileType;
            if (fileType != expectedType)
            {
                throw new OutputDirectoryLeaseException(
                    EngineErrorCode.UnsupportedInput,
                    requireDirectory
                        ? "The output-directory lease path does not identify a directory."
                        : "The output-directory guard does not identify a regular file.");
            }

            return new NativeFileIdentity(
                "linux-statx-v1",
                $"{information.DeviceMajor:X8}:{information.DeviceMinor:X8}",
                information.Inode.ToString("X16"),
                information.LinkCount);
        }

        throw new OutputDirectoryLeaseException(
            EngineErrorCode.UnsupportedInput,
            "Output-directory lease identity is currently supported only on Windows and Linux.");
    }

    /// <summary>Compares only immutable volume and object identifiers, excluding mutable link counts.</summary>
    /// <param name="left">The retained identity.</param>
    /// <param name="right">The identity observed through the current path.</param>
    /// <returns><see langword="true"/> when both values identify the same physical object.</returns>
    private static bool HasSameStableIdentity(NativeFileIdentity left, NativeFileIdentity right)
    {
        return string.Equals(left.Provider, right.Provider, StringComparison.Ordinal)
            && string.Equals(left.VolumeId, right.VolumeId, StringComparison.Ordinal)
            && string.Equals(left.FileId, right.FileId, StringComparison.Ordinal);
    }

    /// <summary>Verifies that the current Linux guard path still identifies the exclusively opened descriptor.</summary>
    /// <param name="canonicalGuardPath">The canonical stable guard path.</param>
    /// <param name="openedIdentity">The identity captured from the exclusively opened descriptor.</param>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when the guard path was replaced or can no longer be verified.</exception>
    private static void VerifyLinuxGuardPathIdentity(
        string canonicalGuardPath,
        NativeFileIdentity openedIdentity)
    {
        var descriptor = Open(canonicalGuardPath, OpenReadOnly | OpenNonBlocking | OpenNoFollow | OpenCloseOnExec);
        if (descriptor < 0)
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.ExternalChangeDetected,
                $"The output-directory guard path changed while its lease was being acquired: '{canonicalGuardPath}'.",
                new Win32Exception(Marshal.GetLastPInvokeError()));
        }

        using var currentHandle = new SafeFileHandle(new IntPtr(descriptor), ownsHandle: true);
        VerifyResolvedPath(canonicalGuardPath, currentHandle, "output-directory guard");
        var currentIdentity = ReadIdentity(currentHandle, requireDirectory: false);
        if (!HasSameStableIdentity(openedIdentity, currentIdentity))
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.ExternalChangeDetected,
                $"The output-directory guard path was replaced while its lease was being acquired: '{canonicalGuardPath}'.");
        }

        if (currentIdentity.LinkCount != 1)
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.UnsupportedInput,
                $"The output-directory guard must have exactly one physical link: '{canonicalGuardPath}'.");
        }
    }

    /// <summary>Resolves a normalized Windows DOS path from an open handle.</summary>
    /// <param name="handle">The opened Windows handle.</param>
    /// <returns>The canonical absolute path.</returns>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when Windows cannot resolve the path.</exception>
    private static string ResolveWindowsHandlePath(SafeFileHandle handle)
    {
        var requiredLength = GetFinalPathNameByHandle(handle, null, 0, FileNameNormalizedDos);
        if (requiredLength == 0)
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.OutputOpenFailed,
                "Windows could not resolve an output-directory lease path from its open handle.",
                new Win32Exception(Marshal.GetLastPInvokeError()));
        }

        var buffer = new char[requiredLength + 1];
        var actualLength = GetFinalPathNameByHandle(handle, buffer, (uint)buffer.Length, FileNameNormalizedDos);
        if (actualLength == 0 || actualLength >= buffer.Length)
        {
            throw new OutputDirectoryLeaseException(
                EngineErrorCode.OutputOpenFailed,
                "Windows returned an invalid resolved path for an output-directory lease handle.",
                new Win32Exception(Marshal.GetLastPInvokeError()));
        }

        var resolvedPath = new string(buffer, 0, (int)actualLength);
        if (resolvedPath.StartsWith(@"\\?\UNC\", StringComparison.OrdinalIgnoreCase))
        {
            resolvedPath = @"\\" + resolvedPath[8..];
        }
        else if (resolvedPath.StartsWith(@"\\?\", StringComparison.OrdinalIgnoreCase))
        {
            resolvedPath = resolvedPath[4..];
        }

        return Path.GetFullPath(resolvedPath);
    }

    /// <summary>Resolves the object held by a Linux descriptor through the process descriptor table.</summary>
    /// <param name="handle">The retained Linux handle.</param>
    /// <returns>The absolute physical path currently associated with the descriptor.</returns>
    /// <exception cref="OutputDirectoryLeaseException">Thrown when Linux cannot resolve the held descriptor.</exception>
    private static string ResolveLinuxHandlePath(SafeFileHandle handle)
    {
        var descriptorPath = $"/proc/self/fd/{handle.DangerousGetHandle().ToInt32()}";
        var buffer = new byte[4096];
        while (true)
        {
            var length = ReadLink(descriptorPath, buffer, (nuint)buffer.Length);
            if (length < 0)
            {
                throw new OutputDirectoryLeaseException(
                    EngineErrorCode.UnsupportedInput,
                    "Linux could not resolve an output-directory lease object from its retained descriptor.",
                    new Win32Exception(Marshal.GetLastPInvokeError()));
            }

            if (length < buffer.Length)
            {
                return Path.GetFullPath(Encoding.UTF8.GetString(buffer, 0, (int)length));
            }

            if (buffer.Length >= 65536)
            {
                throw new OutputDirectoryLeaseException(
                    EngineErrorCode.UnsupportedInput,
                    "Linux returned an output-directory lease path longer than the supported descriptor-path limit.");
            }

            buffer = new byte[buffer.Length * 2];
        }
    }

    /// <summary>Windows metadata returned for an opened file-system object.</summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct ByHandleFileInformation
    {
        /// <summary>The file-system attribute flags.</summary>
        internal uint FileAttributes;

        /// <summary>The file creation timestamp.</summary>
        internal System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;

        /// <summary>The last access timestamp.</summary>
        internal System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;

        /// <summary>The last write timestamp.</summary>
        internal System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;

        /// <summary>The containing volume serial number.</summary>
        internal uint VolumeSerialNumber;

        /// <summary>The high 32 bits of the file length.</summary>
        internal uint FileSizeHigh;

        /// <summary>The low 32 bits of the file length.</summary>
        internal uint FileSizeLow;

        /// <summary>The physical hard-link count.</summary>
        internal uint NumberOfLinks;

        /// <summary>The high 32 bits of the file identifier.</summary>
        internal uint FileIndexHigh;

        /// <summary>The low 32 bits of the file identifier.</summary>
        internal uint FileIndexLow;
    }

    /// <summary>The stable leading fields of Linux's fixed-size statx result.</summary>
    [StructLayout(LayoutKind.Sequential, Size = 256)]
    private struct LinuxStatx
    {
        /// <summary>The fields supplied by the kernel.</summary>
        internal uint Mask;

        /// <summary>The preferred block size.</summary>
        internal uint BlockSize;

        /// <summary>The extended attribute flags.</summary>
        internal ulong Attributes;

        /// <summary>The observed hard-link count.</summary>
        internal uint LinkCount;

        /// <summary>The owning user identifier.</summary>
        internal uint UserId;

        /// <summary>The owning group identifier.</summary>
        internal uint GroupId;

        /// <summary>The file type and mode.</summary>
        internal ushort Mode;

        /// <summary>Reserved alignment bytes.</summary>
        internal ushort Reserved;

        /// <summary>The inode identity.</summary>
        internal ulong Inode;

        /// <summary>The file length.</summary>
        internal ulong Size;

        /// <summary>The allocated block count.</summary>
        internal ulong Blocks;

        /// <summary>The supported attribute mask.</summary>
        internal ulong AttributesMask;

        /// <summary>The last access timestamp.</summary>
        internal LinuxStatxTimestamp AccessTime;

        /// <summary>The creation timestamp.</summary>
        internal LinuxStatxTimestamp BirthTime;

        /// <summary>The last metadata-change timestamp.</summary>
        internal LinuxStatxTimestamp ChangeTime;

        /// <summary>The last content-write timestamp.</summary>
        internal LinuxStatxTimestamp ModificationTime;

        /// <summary>The special-device major number.</summary>
        internal uint DeviceTypeMajor;

        /// <summary>The special-device minor number.</summary>
        internal uint DeviceTypeMinor;

        /// <summary>The containing-device major number.</summary>
        internal uint DeviceMajor;

        /// <summary>The containing-device minor number.</summary>
        internal uint DeviceMinor;
    }

    /// <summary>A Linux statx timestamp.</summary>
    [StructLayout(LayoutKind.Sequential)]
    private struct LinuxStatxTimestamp
    {
        /// <summary>The whole seconds since the Unix epoch.</summary>
        internal long Seconds;

        /// <summary>The fractional nanoseconds.</summary>
        internal uint Nanoseconds;

        /// <summary>Reserved alignment bytes.</summary>
        internal int Reserved;
    }

    /// <summary>Opens a Windows file-system object.</summary>
    /// <param name="fileName">The canonical path.</param>
    /// <param name="desiredAccess">The requested access rights.</param>
    /// <param name="shareMode">The allowed sharing rights.</param>
    /// <param name="securityAttributes">Optional security attributes.</param>
    /// <param name="creationDisposition">Whether the object must already exist.</param>
    /// <param name="flagsAndAttributes">The object-opening flags.</param>
    /// <param name="templateFile">An optional template handle.</param>
    /// <returns>The opened safe handle, or an invalid handle on failure.</returns>
    [DllImport("kernel32.dll", EntryPoint = "CreateFileW", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern SafeFileHandle CreateFile(
        string fileName,
        uint desiredAccess,
        uint shareMode,
        IntPtr securityAttributes,
        uint creationDisposition,
        uint flagsAndAttributes,
        IntPtr templateFile);

    /// <summary>Reads Windows file identity from an open handle.</summary>
    /// <param name="fileHandle">The open file handle.</param>
    /// <param name="information">The returned handle metadata.</param>
    /// <returns><see langword="true"/> when Windows supplied the metadata.</returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetFileInformationByHandle(
        SafeFileHandle fileHandle,
        out ByHandleFileInformation information);

    /// <summary>Reads the normalized path represented by an open Windows handle.</summary>
    /// <param name="fileHandle">The open file handle.</param>
    /// <param name="filePath">The destination buffer, or <see langword="null"/> to query its required length.</param>
    /// <param name="filePathLength">The destination buffer length.</param>
    /// <param name="flags">The Windows path-normalization flags.</param>
    /// <returns>The required or written character count, or zero on failure.</returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetFinalPathNameByHandle(
        SafeFileHandle fileHandle,
        [Out] char[]? filePath,
        uint filePathLength,
        uint flags);

    /// <summary>Opens a Linux file-system object.</summary>
    /// <param name="path">The canonical path.</param>
    /// <param name="flags">The native open flags.</param>
    /// <returns>The opened descriptor, or minus one on failure.</returns>
    [DllImport("libc", EntryPoint = "open", SetLastError = true)]
    private static extern int Open(string path, int flags);

    /// <summary>Reads the path associated with a Linux symbolic link without following it.</summary>
    /// <param name="path">The process descriptor-link path.</param>
    /// <param name="buffer">The destination path buffer.</param>
    /// <param name="bufferSize">The destination buffer size.</param>
    /// <returns>The path byte count, or minus one on failure.</returns>
    [DllImport("libc", EntryPoint = "readlink", SetLastError = true)]
    private static extern nint ReadLink(string path, byte[] buffer, nuint bufferSize);

    /// <summary>Reads Linux identity from an open descriptor.</summary>
    /// <param name="directoryFileDescriptor">The open descriptor.</param>
    /// <param name="path">The empty path used with <see cref="AtEmptyPath"/>.</param>
    /// <param name="flags">The statx lookup flags.</param>
    /// <param name="mask">The requested statx fields.</param>
    /// <param name="information">The returned identity and metadata.</param>
    /// <returns>Zero on success or minus one on failure.</returns>
    [DllImport("libc", EntryPoint = "statx", SetLastError = true)]
    private static extern int Statx(
        int directoryFileDescriptor,
        string path,
        int flags,
        uint mask,
        out LinuxStatx information);
}
