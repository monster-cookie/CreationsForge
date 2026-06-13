using System.IO.Compression;
using System.Text;
using CreationsForge.Bethesda.Assets.Resources;
using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;

namespace CreationsForge.Bethesda.Assets.Archives.Bsa;

public class BsaArchiveReader : IAssetArchiveReader, IAssetArchiveCache
{
    private const int MaximumCachedArchiveDirectories = 8;
    private const uint Magic = 0x00415342;
    private const int HeaderSize = 36;
    private const int LegacyFolderRecordSize = 16;
    private const int WideFolderRecordSize = 24;
    private const int FileRecordSize = 16;
    private const uint SizeCompressionToggle = 0x40000000;
    private const uint CompressedSizeMask = 0x3FFFFFFF;
    private const uint SizePayloadMask = 0x7FFFFFFF;
    private readonly object DirectoryCacheLock = new();
    private readonly Dictionary<string, BsaArchiveDirectoryCacheEntry> DirectoryCache = new(StringComparer.OrdinalIgnoreCase);
    private long DirectoryCacheAccessCounter;

    public bool CanRead(string archivePath)
    {
        return string.Equals(Path.GetExtension(archivePath), ".bsa", StringComparison.OrdinalIgnoreCase);
    }

    public IReadOnlyList<AssetArchiveEntry> ListEntries(string archivePath)
    {
        var directory = GetArchiveDirectory(archivePath);

        return directory.Records
            .Select(record => new AssetArchiveEntry
            {
                ArchivePath = archivePath,
                EntryPath = record.EntryPath,
                PackedSize = record.StoredSize,
                UnpackedSize = record.StoredSize
            })
            .ToList();
    }

    public AssetArchiveReadResult TryReadEntry(string archivePath, string entryPath)
    {
        try
        {
            var normalizedEntryPath = NormalizeEntryPath(entryPath);
            var directory = GetArchiveDirectory(archivePath);
            using var stream = File.OpenRead(archivePath);
            using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: false);

            var match = FindMatchingRecord(directory.Records, normalizedEntryPath);
            if (match.StatusMessage is not null)
            {
                return CreateFailure(archivePath, entryPath, match.StatusMessage);
            }

            if (match.Record.HasValue)
            {
                var record = match.Record.Value;
                stream.Position = record.DataOffset;
                var data = ReadEntryData(reader, directory.Header, record, stream.Length);
                return new AssetArchiveReadResult
                {
                    IsSuccess = true,
                    ArchivePath = archivePath,
                    EntryPath = record.EntryPath,
                    Data = data,
                    StatusMessage = $"Read BSA archive entry {record.EntryPath} from {archivePath}."
                };
            }

            return CreateFailure(archivePath, entryPath, BuildMissingEntryStatusMessage(directory.Records, entryPath, normalizedEntryPath));
        }
        catch (Exception exception) when (exception is EndOfStreamException or IOException or InvalidDataException or OverflowException)
        {
            return exception is AssetTooLargeException
                ? CreateTooLargeFailure(archivePath, entryPath, exception.Message)
                : CreateFailure(archivePath, entryPath, exception.Message);
        }
    }

    public void ClearCache()
    {
        lock (DirectoryCacheLock)
        {
            DirectoryCache.Clear();
            DirectoryCacheAccessCounter = 0;
        }
    }

    private BsaArchiveDirectory GetArchiveDirectory(string archivePath)
    {
        var fullPath = Path.GetFullPath(archivePath);
        var fileInfo = new FileInfo(fullPath);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException("BSA archive was not found.", fullPath);
        }

        var cacheKey = new BsaArchiveDirectoryCacheKey(fullPath, fileInfo.Length, fileInfo.LastWriteTimeUtc.Ticks);
        lock (DirectoryCacheLock)
        {
            if (DirectoryCache.TryGetValue(cacheKey.ArchivePath, out var cacheEntry) &&
                cacheEntry.CacheKey.Length == cacheKey.Length &&
                cacheEntry.CacheKey.LastWriteTimeUtcTicks == cacheKey.LastWriteTimeUtcTicks)
            {
                cacheEntry.LastAccess = ++DirectoryCacheAccessCounter;
                return cacheEntry.Directory;
            }
        }

        var directory = ReadArchiveDirectory(fullPath);
        lock (DirectoryCacheLock)
        {
            DirectoryCache[cacheKey.ArchivePath] = new BsaArchiveDirectoryCacheEntry(cacheKey, directory, ++DirectoryCacheAccessCounter);
            TrimArchiveDirectoryCache();
        }

        return directory;
    }

    private void TrimArchiveDirectoryCache()
    {
        while (DirectoryCache.Count > MaximumCachedArchiveDirectories)
        {
            var oldest = DirectoryCache.OrderBy(pair => pair.Value.LastAccess).First();
            DirectoryCache.Remove(oldest.Key);
        }
    }

    private static BsaArchiveDirectory ReadArchiveDirectory(string archivePath)
    {
        using var stream = File.OpenRead(archivePath);
        using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: false);
        var header = ReadHeader(reader, stream.Length);
        var records = ReadFileRecords(reader, header, stream.Length);
        return new BsaArchiveDirectory(header, records);
    }

    private static BsaArchiveHeader ReadHeader(BinaryReader reader, long archiveLength)
    {
        RequireLength(archiveLength, HeaderSize, "BSA archive is too small to contain a valid header.");
        var magic = reader.ReadUInt32();
        if (magic != Magic)
        {
            throw new InvalidDataException("Archive is not a BSA file.");
        }

        var version = reader.ReadUInt32();
        if (version is < 103 or > 105)
        {
            throw new InvalidDataException($"BSA version {version} is not supported.");
        }

        var folderRecordOffset = reader.ReadUInt32();
        var archiveFlags = reader.ReadUInt32();
        var folderCount = reader.ReadUInt32();
        var fileCount = reader.ReadUInt32();
        var totalFolderNameLength = reader.ReadUInt32();
        var totalFileNameLength = reader.ReadUInt32();
        reader.ReadUInt32();

        var minimumFolderRecordTableEnd = checked((long)folderRecordOffset + ((long)folderCount * LegacyFolderRecordSize));
        if (folderRecordOffset < HeaderSize || minimumFolderRecordTableEnd > archiveLength)
        {
            throw new InvalidDataException("BSA folder record table is outside the archive.");
        }

        return new BsaArchiveHeader(
            version,
            folderRecordOffset,
            archiveFlags,
            folderCount,
            fileCount,
            totalFolderNameLength,
            totalFileNameLength);
    }

    private static IReadOnlyList<BsaArchiveFileRecord> ReadFileRecords(BinaryReader reader, BsaArchiveHeader header, long archiveLength)
    {
        var folderRecordSize = GetFolderRecordSize(header);
        var folderFileCounts = ReadFolderFileCounts(reader, header, archiveLength, folderRecordSize);
        var folderFiles = new List<(string FolderName, uint Size, uint Offset, bool CompressionToggled)>();
        reader.BaseStream.Position = checked((long)header.FolderRecordOffset + ((long)header.FolderCount * folderRecordSize));

        foreach (var fileCount in folderFileCounts)
        {
            var folderName = ReadFolderName(reader, header, archiveLength);
            for (var fileIndex = 0; fileIndex < fileCount; fileIndex++)
            {
                RequireLength(archiveLength - reader.BaseStream.Position, FileRecordSize, "BSA file record table ended early.");
                reader.ReadUInt64();
                var packedFileRecord = reader.ReadUInt64();
                var rawSize = (uint)packedFileRecord;
                var dataOffset = checked((uint)(packedFileRecord >> 32));
                var compressionToggled = (rawSize & SizeCompressionToggle) != 0;
                var storedSize = compressionToggled ? rawSize & CompressedSizeMask : rawSize & SizePayloadMask;
                folderFiles.Add((folderName, storedSize, dataOffset, compressionToggled));
            }
        }

        if (!header.HasFileNames)
        {
            throw new InvalidDataException("BSA archives without file name tables are not supported for preview lookup.");
        }

        var fileNames = ReadFileNames(reader, header, archiveLength);
        if (fileNames.Count != folderFiles.Count)
        {
            throw new InvalidDataException("BSA file name count does not match the file record count.");
        }

        var records = new List<BsaArchiveFileRecord>();
        for (var index = 0; index < folderFiles.Count; index++)
        {
            var file = folderFiles[index];
            var entryPath = NormalizeEntryPath(string.IsNullOrWhiteSpace(file.FolderName)
                ? fileNames[index]
                : $"{file.FolderName}/{fileNames[index]}");
            records.Add(new BsaArchiveFileRecord(entryPath, file.Size, file.Offset, file.CompressionToggled));
        }

        return records;
    }

    private static int GetFolderRecordSize(BsaArchiveHeader header)
    {
        return header.Version >= 105 ? WideFolderRecordSize : LegacyFolderRecordSize;
    }

    private static IReadOnlyList<uint> ReadFolderFileCounts(BinaryReader reader, BsaArchiveHeader header, long archiveLength, int folderRecordSize)
    {
        var folderFileCounts = new List<uint>();
        var folderRecordTableEnd = checked((long)header.FolderRecordOffset + ((long)header.FolderCount * folderRecordSize));
        if (folderRecordTableEnd > archiveLength)
        {
            throw new InvalidDataException($"BSA folder record table is outside the archive for {folderRecordSize}-byte folder records.");
        }

        reader.BaseStream.Position = header.FolderRecordOffset;
        for (var index = 0; index < header.FolderCount; index++)
        {
            RequireLength(archiveLength - reader.BaseStream.Position, folderRecordSize, "BSA folder record table ended early.");
            reader.ReadUInt64();
            var fileCount = reader.ReadUInt32();
            reader.BaseStream.Position += folderRecordSize - sizeof(ulong) - sizeof(uint);
            folderFileCounts.Add(fileCount);
        }

        return folderFileCounts;
    }

    private static string ReadFolderName(BinaryReader reader, BsaArchiveHeader header, long archiveLength)
    {
        if (!header.HasDirectoryNames)
        {
            return string.Empty;
        }

        RequireLength(archiveLength - reader.BaseStream.Position, sizeof(byte), "BSA folder block ended before folder name length.");
        var folderNameLength = reader.ReadByte();
        RequireLength(archiveLength - reader.BaseStream.Position, folderNameLength, "BSA folder block ended before folder name.");
        var bytes = reader.ReadBytes(folderNameLength);
        var nameLength = bytes.Length > 0 && bytes[^1] == 0 ? bytes.Length - 1 : bytes.Length;
        return NormalizeEntryPath(Encoding.UTF8.GetString(bytes, 0, nameLength));
    }

    private static IReadOnlyList<string> ReadFileNames(BinaryReader reader, BsaArchiveHeader header, long archiveLength)
    {
        RequireLength(archiveLength - reader.BaseStream.Position, header.TotalFileNameLength, "BSA file name table is outside the archive.");
        var tableBytes = reader.ReadBytes(checked((int)header.TotalFileNameLength));
        var names = new List<string>();
        var start = 0;
        for (var index = 0; index < tableBytes.Length; index++)
        {
            if (tableBytes[index] != 0)
            {
                continue;
            }

            if (index > start)
            {
                names.Add(NormalizeEntryPath(Encoding.UTF8.GetString(tableBytes, start, index - start)));
            }

            start = index + 1;
        }

        return names;
    }

    private static byte[] ReadEntryData(BinaryReader reader, BsaArchiveHeader header, BsaArchiveFileRecord record, long archiveLength)
    {
        var storedSize = record.StoredSize;
        if (record.DataOffset >= archiveLength || checked((long)record.DataOffset + storedSize) > archiveLength)
        {
            throw new InvalidDataException("BSA entry data range is outside the archive.");
        }

        if (header.HasEmbeddedFileNames)
        {
            SkipEmbeddedFileName(reader, ref storedSize);
        }

        var isCompressed = header.IsCompressedByDefault ^ record.CompressionToggled;
        if (!isCompressed)
        {
            return ReadStoredBytes(reader, storedSize);
        }

        if (storedSize < sizeof(uint))
        {
            throw new InvalidDataException("BSA compressed entry is too small to contain its unpacked size.");
        }

        var unpackedSize = reader.ReadUInt32();
        var packedSize = checked(storedSize - sizeof(uint));
        var packedData = ReadStoredBytes(reader, packedSize);
        return DecompressEntryData(packedData, unpackedSize);
    }

    private static byte[] DecompressEntryData(byte[] packedData, uint unpackedSize)
    {
        var zlibFailure = TryDecompressZlib(packedData, unpackedSize, out var data);
        if (zlibFailure is null)
        {
            return data;
        }

        var lz4Failure = TryDecompressLz4(packedData, unpackedSize, out data);
        if (lz4Failure is null)
        {
            return data;
        }

        throw new InvalidDataException($"The archive entry was compressed using an unsupported compression method. ZLib failed: {zlibFailure}; LZ4 failed: {lz4Failure}.");
    }

    private static string? TryDecompressZlib(byte[] packedData, uint unpackedSize, out byte[] data)
    {
        EnsurePreviewSize(unpackedSize);
        data = new byte[checked((int)unpackedSize)];
        try
        {
            using var packedStream = new MemoryStream(packedData);
            using var zlibStream = new ZLibStream(packedStream, CompressionMode.Decompress);
            zlibStream.ReadExactly(data);
            if (zlibStream.ReadByte() != -1)
            {
                return "produced more bytes than expected";
            }

            return null;
        }
        catch (Exception exception) when (exception is InvalidDataException or EndOfStreamException or IOException)
        {
            return exception.Message;
        }
    }

    private static string? TryDecompressLz4(byte[] packedData, uint unpackedSize, out byte[] data)
    {
        var frameFailure = TryDecompressLz4Frame(packedData, unpackedSize, out data);
        if (frameFailure is null)
        {
            return null;
        }

        var rawFailure = TryDecompressLz4RawBlock(packedData, unpackedSize, out data);
        return $"frame failed: {frameFailure}; raw block failed: {rawFailure}";
    }

    private static string? TryDecompressLz4Frame(byte[] packedData, uint unpackedSize, out byte[] data)
    {
        EnsurePreviewSize(unpackedSize);
        data = new byte[checked((int)unpackedSize)];
        try
        {
            using var packedStream = new MemoryStream(packedData);
            using var lz4Stream = LZ4Stream.Decode(packedStream, leaveOpen: false);
            lz4Stream.ReadExactly(data);
            if (lz4Stream.ReadByte() != -1)
            {
                return "produced more bytes than expected";
            }

            return null;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or EndOfStreamException or IOException)
        {
            return exception.Message;
        }
    }

    private static string? TryDecompressLz4RawBlock(byte[] packedData, uint unpackedSize, out byte[] data)
    {
        EnsurePreviewSize(unpackedSize);
        data = new byte[checked((int)unpackedSize)];
        try
        {
            var decodedLength = LZ4Codec.Decode(packedData, data);
            if (decodedLength != data.Length)
            {
                return $"decoded {decodedLength} bytes, expected {data.Length}";
            }

            return null;
        }
        catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
        {
            return exception.Message;
        }
    }

    private static byte[] ReadStoredBytes(BinaryReader reader, uint storedSize)
    {
        EnsurePreviewSize(storedSize);
        var data = reader.ReadBytes(checked((int)storedSize));
        if (data.Length != storedSize)
        {
            throw new InvalidDataException("BSA entry ended before all bytes could be read.");
        }

        return data;
    }

    private static void SkipEmbeddedFileName(BinaryReader reader, ref uint storedSize)
    {
        var nameLength = reader.ReadByte();
        if (storedSize < nameLength + 1)
        {
            throw new InvalidDataException("BSA embedded file name exceeds the stored entry size.");
        }

        reader.ReadBytes(nameLength);
        storedSize -= (uint)nameLength + 1;
    }

    private static void RequireLength(long availableLength, long requiredLength, string message)
    {
        if (availableLength < requiredLength)
        {
            throw new InvalidDataException(message);
        }
    }

    private static AssetArchiveReadResult CreateFailure(string archivePath, string entryPath, string statusMessage)
    {
        return new AssetArchiveReadResult
        {
            IsSuccess = false,
            ArchivePath = archivePath,
            EntryPath = entryPath,
            StatusMessage = statusMessage
        };
    }

    private static AssetArchiveReadResult CreateTooLargeFailure(string archivePath, string entryPath, string statusMessage)
    {
        return new AssetArchiveReadResult
        {
            IsSuccess = false,
            IsTooLarge = true,
            ArchivePath = archivePath,
            EntryPath = entryPath,
            StatusMessage = statusMessage
        };
    }

    private static void EnsurePreviewSize(uint byteCount)
    {
        if (byteCount > BethesdaAssetProvider.MaximumPreviewAssetBytes)
        {
            throw new AssetTooLargeException($"BSA archive entry is {byteCount} bytes, which exceeds the {BethesdaAssetProvider.MaximumPreviewAssetBytes} byte preview read limit.");
        }
    }

    private static string NormalizeEntryPath(string entryPath)
    {
        return new string(entryPath.Trim().Select(NormalizeEntryCharacter).ToArray());
    }

    private static (BsaArchiveFileRecord? Record, string? StatusMessage) FindMatchingRecord(
        IReadOnlyList<BsaArchiveFileRecord> records,
        string normalizedEntryPath)
    {
        foreach (var record in records)
        {
            if (string.Equals(record.EntryPath, normalizedEntryPath, StringComparison.OrdinalIgnoreCase))
            {
                return (record, null);
            }
        }

        var suffixMatches = records
            .Where(record => IsPathSuffixMatch(record.EntryPath, normalizedEntryPath))
            .ToList();
        if (suffixMatches.Count == 1)
        {
            return (suffixMatches[0], null);
        }

        if (suffixMatches.Count > 1)
        {
            var examples = string.Join(", ", suffixMatches.Select(record => record.EntryPath).Take(5));
            return (null, $"Multiple BSA archive entries matched {normalizedEntryPath}; refusing to choose between {examples}.");
        }

        return (null, null);
    }

    private static bool IsPathSuffixMatch(string entryPath, string requestedPath)
    {
        return entryPath.EndsWith($"/{requestedPath}", StringComparison.OrdinalIgnoreCase)
            || requestedPath.EndsWith($"/{entryPath}", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildMissingEntryStatusMessage(
        IReadOnlyList<BsaArchiveFileRecord> records,
        string entryPath,
        string normalizedEntryPath)
    {
        var message = new StringBuilder();
        message.Append("BSA archive entry ");
        message.Append(entryPath);
        message.Append(" was not found after indexing ");
        message.Append(records.Count);
        message.Append(" entries. Normalized request: ");
        message.Append(normalizedEntryPath);
        message.Append('.');

        var fileName = GetArchiveFileName(normalizedEntryPath);
        var nearMatches = records
            .Select(record => record.EntryPath)
            .Where(recordPath => string.Equals(GetArchiveFileName(recordPath), fileName, StringComparison.OrdinalIgnoreCase))
            .Take(5)
            .ToList();

        if (nearMatches.Count == 0)
        {
            message.Append(" No indexed entries with the same file name were found.");
        }
        else
        {
            message.Append(" Near matches: ");
            message.Append(string.Join(", ", nearMatches));
            message.Append('.');
        }

        return message.ToString();
    }

    private static string GetArchiveFileName(string entryPath)
    {
        var separatorIndex = entryPath.LastIndexOf('/');
        return separatorIndex < 0 ? entryPath : entryPath[(separatorIndex + 1)..];
    }

    private static char NormalizeEntryCharacter(char character)
    {
        if (character == '\\')
        {
            return '/';
        }

        if (character == ':' || character < 0x20 || character >= 0x7F)
        {
            return '_';
        }

        return char.ToLowerInvariant(character);
    }

    private sealed class AssetTooLargeException : IOException
    {
        public AssetTooLargeException(string message) : base(message)
        {
        }
    }
}
