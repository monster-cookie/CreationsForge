using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.Internal;

namespace CreationsForge.Core.Engine.PluginInputs;

/// <summary>
/// Observes plugin source files through open handles and rejects path aliases whose identity cannot be trusted.
/// </summary>
internal static class PluginFileInspector
{
    /// <summary>Windows final-path query format that returns a normalized DOS path.</summary>
    private const uint FileNameNormalizedDos = 0;

    /// <summary>Linux statx flag that inspects the file referenced by a descriptor when the path is empty.</summary>
    private const int AtEmptyPath = 0x1000;

    /// <summary>Linux statx mask requesting the basic identity and metadata fields.</summary>
    private const uint StatxBasicStats = 0x07ff;

    /// <summary>Compares paths according to the current platform's file-name semantics.</summary>
    private static readonly StringComparer PathComparer = OperatingSystem.IsWindows()
        ? StringComparer.OrdinalIgnoreCase
        : StringComparer.Ordinal;

    /// <summary>Verifies that an explicit directory exists and does not traverse an alias.</summary>
    /// <param name="path">The canonical absolute directory path.</param>
    /// <param name="description">The directory description used in diagnostic messages.</param>
    /// <exception cref="PluginSourceInputException">Thrown when the directory is absent, aliased, or cannot be verified on the current platform.</exception>
    internal static void VerifyDirectory(string path, string description)
    {
        if (!Directory.Exists(path))
        {
            throw new PluginSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The {description} does not exist: '{path}'.");
        }

        VerifyPathComponents(path, description);
        var realPath = ResolveRealPath(path);
        if (!PathComparer.Equals(Path.TrimEndingDirectorySeparator(path), Path.TrimEndingDirectorySeparator(realPath)))
        {
            throw new PluginSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"The {description} resolves through an alias and cannot be used safely: '{path}' -> '{realPath}'.");
        }
    }

    /// <summary>Captures one plugin artifact fingerprint with platform file identity.</summary>
    /// <param name="path">The canonical absolute artifact path.</param>
    /// <param name="role">The artifact role.</param>
    /// <param name="language">The loose sidecar language, or <see langword="null"/> for plugins and archives.</param>
    /// <param name="mustExist">Whether absence is a failure rather than a recorded baseline state.</param>
    /// <param name="cancellationToken">The token checked while opening and hashing the artifact.</param>
    /// <returns>The complete association observed from a stable read handle.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="PluginSourceInputException">Thrown when an expected file is missing, changes during observation, is aliased, or cannot be inspected safely.</exception>
    internal static async Task<PluginArtifactAssociation> InspectAsync(
        string path,
        PluginArtifactRole role,
        string? language,
        bool mustExist,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        VerifyPathComponents(path, DescribeRole(role));
        if (!File.Exists(path))
        {
            if (Directory.Exists(path))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"The plugin {DescribeRole(role)} path identifies a directory instead of a file: '{path}'.");
            }

            if (mustExist)
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.SourceOpenFailed,
                    $"The required plugin {DescribeRole(role)} does not exist: '{path}'.");
            }

            return new PluginArtifactAssociation(
                path,
                role,
                language,
                new PluginArtifactFingerprint(false, 0, null));
        }

        try
        {
            await using var stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read | FileShare.Delete,
                131072,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            VerifyResolvedFilePath(path, stream.SafeFileHandle);
            var initialIdentity = ReadIdentity(stream.SafeFileHandle);
            var initialLength = stream.Length;
            var digest = await SHA256.HashDataAsync(stream, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            var finalHandleIdentity = ReadIdentity(stream.SafeFileHandle);
            if (!initialIdentity.Equals(finalHandleIdentity) || initialLength != stream.Length)
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    $"The plugin {DescribeRole(role)} changed while it was being inspected: '{path}'.");
            }

            await using var currentPathStream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read | FileShare.Delete,
                1,
                FileOptions.Asynchronous);
            VerifyResolvedFilePath(path, currentPathStream.SafeFileHandle);
            var currentPathIdentity = ReadIdentity(currentPathStream.SafeFileHandle);
            if (!initialIdentity.Equals(currentPathIdentity))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    $"The plugin {DescribeRole(role)} path was replaced while it was being inspected: '{path}'.");
            }

            return new PluginArtifactAssociation(
                path,
                role,
                language,
                new PluginArtifactFingerprint(true, initialLength, Convert.ToHexString(digest)),
                initialIdentity);
        }
        catch (PluginSourceInputException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or Win32Exception)
        {
            throw new PluginSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                $"The plugin {DescribeRole(role)} could not be inspected safely: '{path}'.",
                exception);
        }
    }

    /// <summary>Opens and retains a read lock for one source artifact without hashing its content.</summary>
    /// <param name="path">The canonical absolute artifact path.</param>
    /// <param name="role">The artifact role.</param>
    /// <param name="language">The loose sidecar language, or <see langword="null"/> for plugins and archives.</param>
    /// <param name="mustExist">Whether absence is a failure rather than a recorded inventory state.</param>
    /// <param name="cancellationToken">The token checked before the handle is acquired.</param>
    /// <returns>The metadata observation and its retained read stream, or an absent observation with no stream.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="PluginSourceInputException">Thrown when an expected file is missing, aliased, locked for writing, or cannot be opened safely.</exception>
    internal static (PluginArtifactAssociation Artifact, FileStream? Stream) OpenReadLock(
        string path,
        PluginArtifactRole role,
        string? language,
        bool mustExist,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        VerifyPathComponents(path, DescribeRole(role));
        if (!File.Exists(path))
        {
            if (Directory.Exists(path))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"The plugin {DescribeRole(role)} path identifies a directory instead of a file: '{path}'.");
            }

            if (mustExist)
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.SourceOpenFailed,
                    $"The required plugin {DescribeRole(role)} does not exist: '{path}'.");
            }

            return (new PluginArtifactAssociation(
                path,
                role,
                language,
                new PluginArtifactFingerprint(false, 0, null)), null);
        }

        FileStream? stream = null;
        try
        {
            stream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                1,
                FileOptions.RandomAccess);
            VerifyResolvedFilePath(path, stream.SafeFileHandle);
            var identity = ReadIdentity(stream.SafeFileHandle);
            var artifact = new PluginArtifactAssociation(
                path,
                role,
                language,
                new PluginArtifactFingerprint(true, stream.Length, null),
                identity);
            return (artifact, stream);
        }
        catch (PluginSourceInputException)
        {
            stream?.Dispose();
            throw;
        }
        catch (OperationCanceledException)
        {
            stream?.Dispose();
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or Win32Exception)
        {
            stream?.Dispose();
            throw new PluginSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                $"The plugin {DescribeRole(role)} could not be locked for read-only workspace use: '{path}'.",
                exception);
        }
    }

    /// <summary>Verifies that no existing component in an explicit path is a symbolic link or Windows reparse point.</summary>
    /// <param name="path">The canonical absolute file or directory path.</param>
    /// <param name="description">The artifact description used in diagnostics.</param>
    /// <exception cref="PluginSourceInputException">Thrown when a path component is an alias.</exception>
    private static void VerifyPathComponents(string path, string description)
    {
        FileSystemInfo? current = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path);
        while (current is not null)
        {
            if (current.LinkTarget is not null
                || (current.Exists && (current.Attributes & FileAttributes.ReparsePoint) != 0))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.UnsupportedInput,
                    $"The {description} traverses a symbolic link or reparse point: '{current.FullName}'.");
            }

            current = current switch
            {
                FileInfo file => file.Directory,
                DirectoryInfo directory => directory.Parent,
                _ => null
            };
        }
    }

    /// <summary>Verifies that an opened file resolves to its supplied canonical path.</summary>
    /// <param name="path">The supplied canonical path.</param>
    /// <param name="handle">The open file handle.</param>
    /// <exception cref="PluginSourceInputException">Thrown when the resolved path differs or cannot be inspected.</exception>
    private static void VerifyResolvedFilePath(string path, SafeFileHandle handle)
    {
        var realPath = OperatingSystem.IsWindows()
            ? ResolveWindowsHandlePath(handle)
            : OperatingSystem.IsMacOS()
                ? ResolveDarwinHandlePath(handle)
                : ResolveRealPath(path);
        if (!PathComparer.Equals(path, realPath))
        {
            throw new PluginSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"The plugin input resolves through an alias and cannot be used safely: '{path}' -> '{realPath}'.");
        }
    }

    /// <summary>Resolves an existing path through the current platform's plugin canonicalization API.</summary>
    /// <param name="path">The existing absolute path.</param>
    /// <returns>The absolute physical path with aliases resolved.</returns>
    /// <exception cref="PluginSourceInputException">Thrown when the platform is unsupported or plugin resolution fails.</exception>
    private static string ResolveRealPath(string path)
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.GetFullPath(path);
        }

        if (!OperatingSystem.IsLinux() && !OperatingSystem.IsMacOS())
        {
            throw new PluginSourceInputException(
                EngineErrorCode.UnsupportedInput,
                "Plugin input identity verification is currently supported only on Windows, Linux, and macOS.");
        }

        var pointer = RealPath(path, IntPtr.Zero);
        if (pointer == IntPtr.Zero)
        {
            throw new PluginSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                $"The plugin input path could not be resolved safely: '{path}'.",
                new Win32Exception(Marshal.GetLastPInvokeError()));
        }

        try
        {
            return Path.GetFullPath(Marshal.PtrToStringUTF8(pointer)!);
        }
        finally
        {
            Free(pointer);
        }
    }

    /// <summary>Resolves the normalized DOS path represented by an open Windows file handle.</summary>
    /// <param name="handle">The open file handle.</param>
    /// <returns>The canonical absolute Windows path.</returns>
    /// <exception cref="PluginSourceInputException">Thrown when Windows cannot resolve the handle path.</exception>
    private static string ResolveWindowsHandlePath(SafeFileHandle handle)
    {
        var requiredLength = GetFinalPathNameByHandle(handle, null, 0, FileNameNormalizedDos);
        if (requiredLength == 0)
        {
            throw new PluginSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                "Windows could not resolve a plugin input path from its open handle.",
                new Win32Exception(Marshal.GetLastPInvokeError()));
        }

        var buffer = new char[requiredLength + 1];
        var actualLength = GetFinalPathNameByHandle(handle, buffer, (uint)buffer.Length, FileNameNormalizedDos);
        if (actualLength == 0 || actualLength >= buffer.Length)
        {
            throw new PluginSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                "Windows returned an invalid resolved path for a plugin input handle.",
                new Win32Exception(Marshal.GetLastPInvokeError()));
        }

        var resolved = new string(buffer, 0, (int)actualLength);
        if (resolved.StartsWith(@"\\?\UNC\", StringComparison.OrdinalIgnoreCase))
        {
            resolved = @"\\" + resolved[8..];
        }
        else if (resolved.StartsWith(@"\\?\", StringComparison.OrdinalIgnoreCase))
        {
            resolved = resolved[4..];
        }

        return Path.GetFullPath(resolved);
    }

    /// <summary>Resolves the physical path represented by an open Darwin file handle.</summary>
    /// <param name="handle">The open Darwin file handle.</param>
    /// <returns>The canonical absolute macOS path.</returns>
    /// <exception cref="PluginSourceInputException">Thrown when Darwin cannot resolve the handle path.</exception>
    private static string ResolveDarwinHandlePath(SafeFileHandle handle)
    {
        try
        {
            return DarwinFileSystemInterop.ResolvePath(handle);
        }
        catch (Exception exception) when (exception is Win32Exception or IOException or PlatformNotSupportedException)
        {
            throw new PluginSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                "macOS could not resolve a plugin input path from its open handle.",
                exception);
        }
    }

    /// <summary>Reads stable platform identity from an open file handle.</summary>
    /// <param name="handle">The open source file handle.</param>
    /// <returns>The platform identity and hard-link count.</returns>
    /// <exception cref="PluginSourceInputException">Thrown when the platform is unsupported or identity cannot be read.</exception>
    private static ArtifactFileIdentity ReadIdentity(SafeFileHandle handle)
    {
        if (OperatingSystem.IsWindows())
        {
            if (!GetFileInformationByHandle(handle, out var information))
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.SourceOpenFailed,
                    "Windows could not read plugin input file identity.",
                    new Win32Exception(Marshal.GetLastPInvokeError()));
            }

            var fileIndex = ((ulong)information.FileIndexHigh << 32) | information.FileIndexLow;
            return new ArtifactFileIdentity(
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
                throw new PluginSourceInputException(
                    EngineErrorCode.UnsupportedInput,
                    "Linux could not provide statx identity for a plugin input file.",
                    new Win32Exception(Marshal.GetLastPInvokeError()));
            }

            return new ArtifactFileIdentity(
                "linux-statx-v1",
                $"{information.DeviceMajor:X8}:{information.DeviceMinor:X8}",
                information.Inode.ToString("X16"),
                information.LinkCount);
        }

        if (OperatingSystem.IsMacOS())
        {
            try
            {
                return DarwinFileSystemInterop.ReadStatus(handle).CreateIdentity();
            }
            catch (Exception exception) when (exception is Win32Exception or PlatformNotSupportedException)
            {
                throw new PluginSourceInputException(
                    EngineErrorCode.UnsupportedInput,
                    "macOS could not provide fstat identity for a plugin input file.",
                    exception);
            }
        }

        throw new PluginSourceInputException(
            EngineErrorCode.UnsupportedInput,
            "Plugin input identity verification is currently supported only on Windows, Linux, and macOS.");
    }

    /// <summary>Returns a stable human-readable artifact role.</summary>
    /// <param name="role">The plugin artifact role.</param>
    /// <returns>A diagnostic role name.</returns>
    private static string DescribeRole(PluginArtifactRole role)
    {
        return role switch
        {
            PluginArtifactRole.Plugin => "plugin",
            PluginArtifactRole.Strings => "STRINGS sidecar",
            PluginArtifactRole.DlStrings => "DLSTRINGS sidecar",
            PluginArtifactRole.IlStrings => "ILSTRINGS sidecar",
            PluginArtifactRole.StringsArchive => "strings archive",
            _ => "artifact"
        };
    }

    /// <summary>Windows file metadata returned for an open handle.</summary>
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

        /// <summary>The number of hard links to the file.</summary>
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
    /// <param name="filePath">The destination character buffer, or <see langword="null"/> to query its required length.</param>
    /// <param name="filePathLength">The destination buffer length.</param>
    /// <param name="flags">The Windows path normalization flags.</param>
    /// <returns>The required or written character count, or zero on failure.</returns>
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern uint GetFinalPathNameByHandle(
        SafeFileHandle fileHandle,
        [Out] char[]? filePath,
        uint filePathLength,
        uint flags);

    /// <summary>Resolves a Unix path through libc.</summary>
    /// <param name="path">The existing path to resolve.</param>
    /// <param name="resolvedPath">A caller buffer, or zero for libc allocation.</param>
    /// <returns>A newly allocated UTF-8 path pointer, or zero on failure.</returns>
    [DllImport("libc", EntryPoint = "realpath", SetLastError = true)]
    private static extern IntPtr RealPath(string path, IntPtr resolvedPath);

    /// <summary>Releases memory allocated by libc.</summary>
    /// <param name="pointer">The plugin pointer to release.</param>
    [DllImport("libc", EntryPoint = "free")]
    private static extern void Free(IntPtr pointer);

    /// <summary>Reads Linux file identity from a descriptor through statx.</summary>
    /// <param name="directoryFileDescriptor">The open file descriptor.</param>
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
