using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32.SafeHandles;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Archives;
using System.IO.Abstractions;

namespace CreationsForge.Core.Engine.NativeInputs;

/// <summary>
/// Observes native source files through open handles and rejects path aliases whose identity cannot be trusted.
/// </summary>
internal static class NativeFileInspector
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
    /// <exception cref="NativeSourceInputException">Thrown when the directory is absent, aliased, or cannot be verified on the current platform.</exception>
    internal static void VerifyDirectory(string path, string description)
    {
        if (!Directory.Exists(path))
        {
            throw new NativeSourceInputException(
                EngineErrorCode.InvalidRequest,
                $"The {description} does not exist: '{path}'.");
        }

        VerifyPathComponents(path, description);
        var realPath = ResolveRealPath(path);
        if (!PathComparer.Equals(Path.TrimEndingDirectorySeparator(path), Path.TrimEndingDirectorySeparator(realPath)))
        {
            throw new NativeSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"The {description} resolves through an alias and cannot be used safely: '{path}' -> '{realPath}'.");
        }
    }

    /// <summary>Captures one native artifact fingerprint with platform file identity.</summary>
    /// <param name="path">The canonical absolute artifact path.</param>
    /// <param name="role">The artifact role.</param>
    /// <param name="language">The loose sidecar language, or <see langword="null"/> for plugins and archives.</param>
    /// <param name="mustExist">Whether absence is a failure rather than a recorded baseline state.</param>
    /// <param name="cancellationToken">The token checked while opening and hashing the artifact.</param>
    /// <returns>The complete association observed from a stable read handle.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="NativeSourceInputException">Thrown when an expected file is missing, changes during observation, is aliased, or cannot be inspected safely.</exception>
    internal static async Task<NativeArtifactAssociation> InspectAsync(
        string path,
        NativeArtifactRole role,
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
                throw new NativeSourceInputException(
                    EngineErrorCode.InvalidRequest,
                    $"The native {DescribeRole(role)} path identifies a directory instead of a file: '{path}'.");
            }

            if (mustExist)
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.SourceOpenFailed,
                    $"The required native {DescribeRole(role)} does not exist: '{path}'.");
            }

            return new NativeArtifactAssociation(
                path,
                role,
                language,
                new NativeArtifactFingerprint(false, 0, null));
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
                throw new NativeSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    $"The native {DescribeRole(role)} changed while it was being inspected: '{path}'.");
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
                throw new NativeSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    $"The native {DescribeRole(role)} path was replaced while it was being inspected: '{path}'.");
            }

            return new NativeArtifactAssociation(
                path,
                role,
                language,
                new NativeArtifactFingerprint(true, initialLength, Convert.ToHexString(digest)),
                initialIdentity);
        }
        catch (NativeSourceInputException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or Win32Exception)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                $"The native {DescribeRole(role)} could not be inspected safely: '{path}'.",
                exception);
        }
    }

    /// <summary>Fingerprints only localized-string entries from an applicable archive while retaining its physical identity and size.</summary>
    /// <param name="path">The canonical absolute archive path.</param>
    /// <param name="release">The native game release used to select the archive reader.</param>
    /// <param name="targetFileNames">The exact localized-string file names that the admitted plugins can request.</param>
    /// <param name="fileSystem">The filesystem adapter used by Mutagen's archive reader.</param>
    /// <param name="cancellationToken">The token checked while reading the archive directory and matching localized entries.</param>
    /// <returns>An archive association whose digest covers matching entry names, sizes, and uncompressed bytes.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    /// <exception cref="NativeSourceInputException">Thrown when the archive changes, is aliased, or cannot be inspected safely.</exception>
    internal static async Task<NativeArtifactAssociation> InspectArchiveStringsAsync(
        string path,
        GameRelease release,
        IReadOnlySet<string> targetFileNames,
        IFileSystem fileSystem,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(targetFileNames);
        ArgumentNullException.ThrowIfNull(fileSystem);
        cancellationToken.ThrowIfCancellationRequested();
        VerifyPathComponents(path, DescribeRole(NativeArtifactRole.StringsArchive));

        try
        {
            await using var identityStream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read | FileShare.Delete,
                1,
                FileOptions.Asynchronous);
            VerifyResolvedFilePath(path, identityStream.SafeFileHandle);
            var initialIdentity = ReadIdentity(identityStream.SafeFileHandle);
            var initialLength = identityStream.Length;
            var archive = Archive.CreateReader(release, path, fileSystem);
            using var combinedHash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            foreach (var file in archive.Files
                         .Where(file => targetFileNames.Contains(Path.GetFileName(file.Path)))
                         .OrderBy(file => file.Path, StringComparer.OrdinalIgnoreCase))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await using var entryStream = file.AsStream();
                var entryDigest = await SHA256.HashDataAsync(entryStream, cancellationToken).ConfigureAwait(false);
                var descriptor = Encoding.UTF8.GetBytes(
                    $"{file.Path.Replace('\\', '/').ToUpperInvariant()}\0{file.Size}\0{Convert.ToHexString(entryDigest)}\n");
                combinedHash.AppendData(descriptor);
            }

            cancellationToken.ThrowIfCancellationRequested();
            var finalHandleIdentity = ReadIdentity(identityStream.SafeFileHandle);
            if (!initialIdentity.Equals(finalHandleIdentity) || initialLength != identityStream.Length)
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    $"The native strings archive changed while it was being inspected: '{path}'.");
            }

            await using var currentPathStream = new FileStream(
                path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read | FileShare.Delete,
                1,
                FileOptions.Asynchronous);
            VerifyResolvedFilePath(path, currentPathStream.SafeFileHandle);
            if (!initialIdentity.Equals(ReadIdentity(currentPathStream.SafeFileHandle)))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.ExternalChangeDetected,
                    $"The native strings archive path was replaced while it was being inspected: '{path}'.");
            }

            return new NativeArtifactAssociation(
                path,
                NativeArtifactRole.StringsArchive,
                null,
                new NativeArtifactFingerprint(true, initialLength, Convert.ToHexString(combinedHash.GetHashAndReset())),
                initialIdentity);
        }
        catch (NativeSourceInputException)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception) when (
            exception is IOException or UnauthorizedAccessException or InvalidDataException or OverflowException or ArgumentException)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                $"The native strings archive could not be inspected safely: '{path}'.",
                exception);
        }
    }

    /// <summary>Verifies that no existing component in an explicit path is a symbolic link or Windows reparse point.</summary>
    /// <param name="path">The canonical absolute file or directory path.</param>
    /// <param name="description">The artifact description used in diagnostics.</param>
    /// <exception cref="NativeSourceInputException">Thrown when a path component is an alias.</exception>
    private static void VerifyPathComponents(string path, string description)
    {
        FileSystemInfo? current = File.Exists(path) ? new FileInfo(path) : new DirectoryInfo(path);
        while (current is not null)
        {
            if (current.LinkTarget is not null
                || (current.Exists && (current.Attributes & FileAttributes.ReparsePoint) != 0))
            {
                throw new NativeSourceInputException(
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
    /// <exception cref="NativeSourceInputException">Thrown when the resolved path differs or cannot be inspected.</exception>
    private static void VerifyResolvedFilePath(string path, SafeFileHandle handle)
    {
        var realPath = OperatingSystem.IsWindows()
            ? ResolveWindowsHandlePath(handle)
            : ResolveRealPath(path);
        if (!PathComparer.Equals(path, realPath))
        {
            throw new NativeSourceInputException(
                EngineErrorCode.UnsupportedInput,
                $"The native input resolves through an alias and cannot be used safely: '{path}' -> '{realPath}'.");
        }
    }

    /// <summary>Resolves an existing path through the current platform's native canonicalization API.</summary>
    /// <param name="path">The existing absolute path.</param>
    /// <returns>The absolute physical path with aliases resolved.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when the platform is unsupported or native resolution fails.</exception>
    private static string ResolveRealPath(string path)
    {
        if (OperatingSystem.IsWindows())
        {
            return Path.GetFullPath(path);
        }

        if (!OperatingSystem.IsLinux())
        {
            throw new NativeSourceInputException(
                EngineErrorCode.UnsupportedInput,
                "Native input identity verification is currently supported only on Windows and Linux.");
        }

        var pointer = RealPath(path, IntPtr.Zero);
        if (pointer == IntPtr.Zero)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                $"The native input path could not be resolved safely: '{path}'.",
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
    /// <exception cref="NativeSourceInputException">Thrown when Windows cannot resolve the handle path.</exception>
    private static string ResolveWindowsHandlePath(SafeFileHandle handle)
    {
        var requiredLength = GetFinalPathNameByHandle(handle, null, 0, FileNameNormalizedDos);
        if (requiredLength == 0)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                "Windows could not resolve a native input path from its open handle.",
                new Win32Exception(Marshal.GetLastPInvokeError()));
        }

        var buffer = new char[requiredLength + 1];
        var actualLength = GetFinalPathNameByHandle(handle, buffer, (uint)buffer.Length, FileNameNormalizedDos);
        if (actualLength == 0 || actualLength >= buffer.Length)
        {
            throw new NativeSourceInputException(
                EngineErrorCode.SourceOpenFailed,
                "Windows returned an invalid resolved path for a native input handle.",
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

    /// <summary>Reads stable platform identity from an open file handle.</summary>
    /// <param name="handle">The open source file handle.</param>
    /// <returns>The platform identity and hard-link count.</returns>
    /// <exception cref="NativeSourceInputException">Thrown when the platform is unsupported or identity cannot be read.</exception>
    private static NativeFileIdentity ReadIdentity(SafeFileHandle handle)
    {
        if (OperatingSystem.IsWindows())
        {
            if (!GetFileInformationByHandle(handle, out var information))
            {
                throw new NativeSourceInputException(
                    EngineErrorCode.SourceOpenFailed,
                    "Windows could not read native input file identity.",
                    new Win32Exception(Marshal.GetLastPInvokeError()));
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
                throw new NativeSourceInputException(
                    EngineErrorCode.UnsupportedInput,
                    "Linux could not provide statx identity for a native input file.",
                    new Win32Exception(Marshal.GetLastPInvokeError()));
            }

            return new NativeFileIdentity(
                "linux-statx-v1",
                $"{information.DeviceMajor:X8}:{information.DeviceMinor:X8}",
                information.Inode.ToString("X16"),
                information.LinkCount);
        }

        throw new NativeSourceInputException(
            EngineErrorCode.UnsupportedInput,
            "Native input identity verification is currently supported only on Windows and Linux.");
    }

    /// <summary>Returns a stable human-readable artifact role.</summary>
    /// <param name="role">The native artifact role.</param>
    /// <returns>A diagnostic role name.</returns>
    private static string DescribeRole(NativeArtifactRole role)
    {
        return role switch
        {
            NativeArtifactRole.Plugin => "plugin",
            NativeArtifactRole.Strings => "STRINGS sidecar",
            NativeArtifactRole.DlStrings => "DLSTRINGS sidecar",
            NativeArtifactRole.IlStrings => "ILSTRINGS sidecar",
            NativeArtifactRole.StringsArchive => "strings archive",
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

    /// <summary>Resolves a Linux path through libc.</summary>
    /// <param name="path">The existing path to resolve.</param>
    /// <param name="resolvedPath">A caller buffer, or zero for libc allocation.</param>
    /// <returns>A newly allocated UTF-8 path pointer, or zero on failure.</returns>
    [DllImport("libc", EntryPoint = "realpath", SetLastError = true)]
    private static extern IntPtr RealPath(string path, IntPtr resolvedPath);

    /// <summary>Releases memory allocated by libc.</summary>
    /// <param name="pointer">The native pointer to release.</param>
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
