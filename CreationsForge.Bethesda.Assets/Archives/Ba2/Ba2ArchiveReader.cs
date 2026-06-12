using System.IO.Compression;
using System.Text;
using CreationsForge.Bethesda.Assets.Resources;
using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;

namespace CreationsForge.Bethesda.Assets.Archives.Ba2;

public class Ba2ArchiveReader : IAssetArchiveReader
{
    private const int MaximumCachedArchiveDirectories = 8;
    private const uint Magic = 0x58445442;
    private const uint GeneralArchiveType = 0x4C524E47;
    private const uint TextureArchiveType = 0x30315844;
    private const uint FileRecordSentinel = 0xBAADF00D;
    private const uint DdsMagic = 0x20534444;
    private const uint DdsCaps = 0x00000001;
    private const uint DdsHeight = 0x00000002;
    private const uint DdsWidth = 0x00000004;
    private const uint DdsPixelFormat = 0x00001000;
    private const uint DdsMipMapCount = 0x00020000;
    private const uint DdsLinearSize = 0x00080000;
    private const uint DdsPixelFormatFourCc = 0x00000004;
    private const uint DdsCapsTexture = 0x00001000;
    private const uint DdsCapsComplex = 0x00000008;
    private const uint DdsCapsMipMap = 0x00400000;
    private const uint Dxt1FourCc = 0x31545844;
    private const uint Dxt3FourCc = 0x33545844;
    private const uint Dxt5FourCc = 0x35545844;
    private const int InitialHeaderSize = 12;
    private const int StandardHeaderSize = 24;
    private const int ExtendedHeaderSize = 32;
    private const int Fallout76HeaderSize = 36;
    private const int FileRecordSize = 36;
    private const int TextureRecordSize = 24;
    private const int TextureChunkSize = 24;
    private const int DdsHeaderSize = 128;
    private readonly object DirectoryCacheLock = new();
    private readonly Dictionary<string, Ba2ArchiveDirectoryCacheEntry> DirectoryCache = new(StringComparer.OrdinalIgnoreCase);
    private long DirectoryCacheAccessCounter;

    public bool CanRead(string archivePath)
    {
        return string.Equals(Path.GetExtension(archivePath), ".ba2", StringComparison.OrdinalIgnoreCase);
    }

    public IReadOnlyList<AssetArchiveEntry> ListEntries(string archivePath)
    {
        var directory = GetArchiveDirectory(archivePath);
        return directory.Header.ArchiveType == Ba2ArchiveType.Texture
            ? ListTextureEntries(archivePath, directory)
            : ListGeneralEntries(archivePath, directory);
    }

    public AssetArchiveReadResult TryReadEntry(string archivePath, string entryPath)
    {
        try
        {
            var normalizedEntryPath = NormalizeEntryPath(entryPath);
            var directory = GetArchiveDirectory(archivePath);
            using var stream = File.OpenRead(archivePath);
            using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: false);
            if (directory.Header.ArchiveType == Ba2ArchiveType.Texture)
            {
                return TryReadTextureEntry(archivePath, entryPath, normalizedEntryPath, reader, directory, stream.Length);
            }

            return TryReadGeneralEntry(archivePath, entryPath, normalizedEntryPath, reader, directory, stream.Length);
        }
        catch (Exception exception) when (exception is EndOfStreamException or IOException or InvalidDataException or OverflowException)
        {
            return exception is AssetTooLargeException
                ? CreateTooLargeFailure(archivePath, entryPath, exception.Message)
                : CreateFailure(archivePath, entryPath, exception.Message);
        }
    }

    private Ba2ArchiveDirectory GetArchiveDirectory(string archivePath)
    {
        var fullPath = Path.GetFullPath(archivePath);
        var fileInfo = new FileInfo(fullPath);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException("BA2 archive was not found.", fullPath);
        }

        var cacheKey = new Ba2ArchiveDirectoryCacheKey(fullPath, fileInfo.Length, fileInfo.LastWriteTimeUtc.Ticks);
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
            DirectoryCache[cacheKey.ArchivePath] = new Ba2ArchiveDirectoryCacheEntry(cacheKey, directory, ++DirectoryCacheAccessCounter);
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

    private static Ba2ArchiveDirectory ReadArchiveDirectory(string archivePath)
    {
        using var stream = File.OpenRead(archivePath);
        using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: false);
        var header = ReadHeader(reader, stream.Length);
        if (header.ArchiveType == Ba2ArchiveType.Texture)
        {
            var records = ReadTextureRecords(reader, header, stream.Length);
            var names = ReadNameTable(reader, header, stream.Length);
            return Ba2ArchiveDirectory.CreateTexture(header, names, records);
        }

        var fileRecords = ReadFileRecords(reader, header, stream.Length);
        var fileNames = ReadNameTable(reader, header, stream.Length);
        return Ba2ArchiveDirectory.CreateGeneral(header, fileNames, fileRecords);
    }

    private static IReadOnlyList<AssetArchiveEntry> ListGeneralEntries(string archivePath, Ba2ArchiveDirectory directory)
    {
        var records = directory.FileRecords;
        var names = directory.Names;
        var entries = new List<AssetArchiveEntry>();

        for (var index = 0; index < names.Count; index++)
        {
            entries.Add(new AssetArchiveEntry
            {
                ArchivePath = archivePath,
                EntryPath = names[index],
                PackedSize = records[index].PackedSize,
                UnpackedSize = records[index].UnpackedSize
            });
        }

        return entries;
    }

    private static IReadOnlyList<AssetArchiveEntry> ListTextureEntries(string archivePath, Ba2ArchiveDirectory directory)
    {
        var records = directory.TextureRecords;
        var names = directory.Names;
        var entries = new List<AssetArchiveEntry>();

        for (var index = 0; index < names.Count; index++)
        {
            entries.Add(new AssetArchiveEntry
            {
                ArchivePath = archivePath,
                EntryPath = names[index],
                PackedSize = records[index].PackedSize,
                UnpackedSize = checked((uint)(DdsHeaderSize + records[index].UnpackedSize))
            });
        }

        return entries;
    }

    private static AssetArchiveReadResult TryReadGeneralEntry(string archivePath, string entryPath, string normalizedEntryPath, BinaryReader reader, Ba2ArchiveDirectory directory, long archiveLength)
    {
        if (directory.EntryIndexes.TryGetValue(normalizedEntryPath, out var index))
        {
            var record = directory.FileRecords[index];
            ValidateDataRange(record, archiveLength);
            reader.BaseStream.Position = checked((long)record.DataOffset);
            var data = ReadEntryData(reader, record);

            return new AssetArchiveReadResult
            {
                IsSuccess = true,
                ArchivePath = archivePath,
                EntryPath = directory.Names[index],
                Data = data,
                StatusMessage = $"Read BA2 archive entry {directory.Names[index]} from {archivePath}."
            };
        }

        return CreateFailure(archivePath, entryPath, $"BA2 archive entry {entryPath} was not found.");
    }

    private static AssetArchiveReadResult TryReadTextureEntry(string archivePath, string entryPath, string normalizedEntryPath, BinaryReader reader, Ba2ArchiveDirectory directory, long archiveLength)
    {
        if (directory.EntryIndexes.TryGetValue(normalizedEntryPath, out var index))
        {
            var record = directory.TextureRecords[index];
            ValidateTextureDataRanges(record, archiveLength);
            var data = ReadTextureEntryData(reader, directory.Header, record);

            return new AssetArchiveReadResult
            {
                IsSuccess = true,
                ArchivePath = archivePath,
                EntryPath = directory.Names[index],
                Data = data,
                StatusMessage = $"Read BA2 texture archive entry {directory.Names[index]} from {archivePath}."
            };
        }

        return CreateFailure(archivePath, entryPath, $"BA2 archive entry {entryPath} was not found.");
    }

    private static Ba2ArchiveHeader ReadHeader(BinaryReader reader, long archiveLength)
    {
        RequireLength(archiveLength, StandardHeaderSize, "BA2 archive is too small to contain a valid header.");
        var magic = reader.ReadUInt32();
        if (magic != Magic)
        {
            throw new InvalidDataException("Archive is not a BA2 file.");
        }

        var version = reader.ReadUInt32();
        if (!IsSupportedVersion(version))
        {
            throw new InvalidDataException($"BA2 version {version} is not supported.");
        }

        var archiveTypeValue = reader.ReadUInt32();
        if (archiveTypeValue != GeneralArchiveType && archiveTypeValue != TextureArchiveType)
        {
            throw new InvalidDataException("Only BA2 general and texture archives are supported.");
        }

        var archiveType = archiveTypeValue == TextureArchiveType ? Ba2ArchiveType.Texture : Ba2ArchiveType.General;
        var headerSize = archiveType == Ba2ArchiveType.Texture && version is 3 or 7 or 8
            ? Fallout76HeaderSize
            : version is 2 or 3
                ? ExtendedHeaderSize
                : StandardHeaderSize;
        RequireLength(archiveLength, headerSize, "BA2 archive is too small to contain a complete header.");
        var fileCount = reader.ReadUInt32();
        var nameTableOffset = reader.ReadUInt64();
        if (headerSize > StandardHeaderSize)
        {
            reader.BaseStream.Position = headerSize;
        }

        var minimumRecordSize = archiveType == Ba2ArchiveType.Texture ? TextureRecordSize : FileRecordSize;
        var recordTableEnd = checked(headerSize + ((long)fileCount * minimumRecordSize));
        if (nameTableOffset < (ulong)recordTableEnd || nameTableOffset > (ulong)archiveLength)
        {
            throw new InvalidDataException("BA2 name table offset is outside the archive.");
        }

        return new Ba2ArchiveHeader(version, archiveType, headerSize, fileCount, nameTableOffset);
    }

    private static IReadOnlyList<Ba2ArchiveFileRecord> ReadFileRecords(BinaryReader reader, Ba2ArchiveHeader header, long archiveLength)
    {
        var records = new List<Ba2ArchiveFileRecord>();
        reader.BaseStream.Position = header.HeaderSize;
        for (var index = 0; index < header.FileCount; index++)
        {
            RequireLength(archiveLength - reader.BaseStream.Position, FileRecordSize, "BA2 file record table ended early.");
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            var dataOffset = reader.ReadUInt64();
            var packedSize = reader.ReadUInt32();
            var unpackedSize = reader.ReadUInt32();
            var sentinel = reader.ReadUInt32();
            if (sentinel != FileRecordSentinel)
            {
                throw new InvalidDataException("BA2 file record sentinel is invalid.");
            }

            records.Add(new Ba2ArchiveFileRecord(dataOffset, packedSize, unpackedSize));
        }

        return records;
    }

    private static IReadOnlyList<Ba2ArchiveTextureRecord> ReadTextureRecords(BinaryReader reader, Ba2ArchiveHeader header, long archiveLength)
    {
        var records = new List<Ba2ArchiveTextureRecord>();
        reader.BaseStream.Position = header.HeaderSize;
        for (var index = 0; index < header.FileCount; index++)
        {
            RequireLength(archiveLength - reader.BaseStream.Position, TextureRecordSize, "BA2 texture record table ended early.");
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadByte();
            var chunkCount = reader.ReadByte();
            var chunkHeaderSize = reader.ReadUInt16();
            var height = reader.ReadUInt16();
            var width = reader.ReadUInt16();
            var mipCount = reader.ReadByte();
            var dxgiFormat = reader.ReadByte();
            var flags = reader.ReadUInt16();
            if (chunkCount == 0)
            {
                throw new InvalidDataException("BA2 texture record has no chunks.");
            }

            if (chunkHeaderSize != TextureChunkSize)
            {
                throw new InvalidDataException($"BA2 texture chunk header size {chunkHeaderSize} is not supported.");
            }

            var chunks = new List<Ba2ArchiveTextureChunk>();
            for (var chunkIndex = 0; chunkIndex < chunkCount; chunkIndex++)
            {
                RequireLength(archiveLength - reader.BaseStream.Position, TextureChunkSize, "BA2 texture chunk table ended early.");
                var dataOffset = reader.ReadUInt64();
                var packedSize = reader.ReadUInt32();
                var unpackedSize = reader.ReadUInt32();
                var startMip = reader.ReadUInt16();
                var endMip = reader.ReadUInt16();
                var sentinel = reader.ReadUInt32();
                if (sentinel != FileRecordSentinel)
                {
                    throw new InvalidDataException("BA2 texture chunk sentinel is invalid.");
                }

                chunks.Add(new Ba2ArchiveTextureChunk(dataOffset, packedSize, unpackedSize, startMip, endMip));
            }

            records.Add(new Ba2ArchiveTextureRecord(height, width, mipCount, dxgiFormat, flags, chunks));
        }

        return records;
    }

    private static IReadOnlyList<string> ReadNameTable(BinaryReader reader, Ba2ArchiveHeader header, long archiveLength)
    {
        var names = new List<string>();
        reader.BaseStream.Position = checked((long)header.NameTableOffset);
        for (var index = 0; index < header.FileCount; index++)
        {
            RequireLength(archiveLength - reader.BaseStream.Position, sizeof(ushort), "BA2 name table ended before a name length could be read.");
            var nameLength = reader.ReadUInt16();
            RequireLength(archiveLength - reader.BaseStream.Position, nameLength, "BA2 name table ended before a complete name could be read.");
            var nameBytes = reader.ReadBytes(nameLength);
            names.Add(NormalizeEntryPath(Encoding.UTF8.GetString(nameBytes)));
        }

        return names;
    }

    private static byte[] ReadEntryData(BinaryReader reader, Ba2ArchiveFileRecord record)
    {
        if (record.PackedSize > 0)
        {
            return ReadCompressedEntryData(reader, record);
        }

        EnsurePreviewSize(record.UnpackedSize);
        var data = reader.ReadBytes(checked((int)record.UnpackedSize));
        if (data.Length != record.UnpackedSize)
        {
            throw new InvalidDataException("BA2 entry ended before all bytes could be read.");
        }

        return data;
    }

    private static byte[] ReadCompressedEntryData(BinaryReader reader, Ba2ArchiveFileRecord record)
    {
        var packedData = reader.ReadBytes(checked((int)record.PackedSize));
        if (packedData.Length != record.PackedSize)
        {
            throw new InvalidDataException("BA2 compressed entry ended before all packed bytes could be read.");
        }

        EnsurePreviewSize(record.UnpackedSize);
        var data = new byte[checked((int)record.UnpackedSize)];
        using var packedStream = new MemoryStream(packedData);
        using var zlibStream = new ZLibStream(packedStream, CompressionMode.Decompress);
        zlibStream.ReadExactly(data);
        if (zlibStream.ReadByte() != -1)
        {
            throw new InvalidDataException("BA2 compressed entry produced more bytes than expected.");
        }

        return data;
    }

    private static byte[] ReadTextureEntryData(BinaryReader reader, Ba2ArchiveHeader header, Ba2ArchiveTextureRecord record)
    {
        EnsurePreviewSize(checked(DdsHeaderSize + (long)record.UnpackedSize));
        var data = new byte[checked(DdsHeaderSize + (int)record.UnpackedSize)];
        WriteDdsHeader(data, record);
        var outputOffset = DdsHeaderSize;
        foreach (var chunk in record.Chunks)
        {
            reader.BaseStream.Position = checked((long)chunk.DataOffset);
            var chunkData = ReadTextureChunkData(reader, header, chunk);
            Buffer.BlockCopy(chunkData, 0, data, outputOffset, chunkData.Length);
            outputOffset += chunkData.Length;
        }

        return data;
    }

    private static byte[] ReadTextureChunkData(BinaryReader reader, Ba2ArchiveHeader header, Ba2ArchiveTextureChunk chunk)
    {
        if (chunk.PackedSize == 0)
        {
            EnsurePreviewSize(chunk.UnpackedSize);
            var data = reader.ReadBytes(checked((int)chunk.UnpackedSize));
            if (data.Length != chunk.UnpackedSize)
            {
                throw new InvalidDataException("BA2 texture chunk ended before all bytes could be read.");
            }

            return data;
        }

        var packedData = reader.ReadBytes(checked((int)chunk.PackedSize));
        if (packedData.Length != chunk.PackedSize)
        {
            throw new InvalidDataException("BA2 packed texture chunk ended before all bytes could be read.");
        }

        return header.Version is 3 or 7 or 8
            ? DecompressLz4TextureChunk(packedData, chunk.UnpackedSize)
            : DecompressZlibTextureChunk(packedData, chunk.UnpackedSize);
    }

    private static byte[] DecompressZlibTextureChunk(byte[] packedData, uint unpackedSize)
    {
        EnsurePreviewSize(unpackedSize);
        var data = new byte[checked((int)unpackedSize)];
        using var packedStream = new MemoryStream(packedData);
        using var zlibStream = new ZLibStream(packedStream, CompressionMode.Decompress);
        zlibStream.ReadExactly(data);
        if (zlibStream.ReadByte() != -1)
        {
            throw new InvalidDataException("BA2 texture chunk produced more bytes than expected.");
        }

        return data;
    }

    private static byte[] DecompressLz4TextureChunk(byte[] packedData, uint unpackedSize)
    {
        EnsurePreviewSize(unpackedSize);
        var data = new byte[checked((int)unpackedSize)];
        var rawFailure = TryDecompressRawLz4TextureChunk(packedData, data);
        if (rawFailure is null)
        {
            return data;
        }

        var frameFailure = TryDecompressFramedLz4TextureChunk(packedData, data);
        if (frameFailure is null)
        {
            return data;
        }

        throw new InvalidDataException($"BA2 texture chunk was compressed using an unsupported LZ4 layout. Raw failed: {rawFailure}; frame failed: {frameFailure}.");
    }

    private static string? TryDecompressRawLz4TextureChunk(byte[] packedData, byte[] data)
    {
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

    private static string? TryDecompressFramedLz4TextureChunk(byte[] packedData, byte[] data)
    {
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

    private static void WriteDdsHeader(byte[] data, Ba2ArchiveTextureRecord record)
    {
        var fourCc = GetDdsFourCc(record.DxgiFormat, out var blockSize);
        var mipCount = Math.Max(1, (int)record.MipCount);
        var flags = DdsCaps | DdsHeight | DdsWidth | DdsPixelFormat | DdsLinearSize;
        var caps = DdsCapsTexture;
        if (mipCount > 1)
        {
            flags |= DdsMipMapCount;
            caps |= DdsCapsComplex | DdsCapsMipMap;
        }

        using var stream = new MemoryStream(data, writable: true);
        using var writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true);
        writer.Write(DdsMagic);
        writer.Write(124U);
        writer.Write(flags);
        writer.Write((uint)record.Height);
        writer.Write((uint)record.Width);
        writer.Write(CalculateTopMipLinearSize(record.Width, record.Height, blockSize));
        writer.Write(0U);
        writer.Write((uint)mipCount);
        for (var index = 0; index < 11; index++)
        {
            writer.Write(0U);
        }

        writer.Write(32U);
        writer.Write(DdsPixelFormatFourCc);
        writer.Write(fourCc);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(caps);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(0U);
    }

    private static uint GetDdsFourCc(byte dxgiFormat, out uint blockSize)
    {
        if (dxgiFormat is 70 or 71 or 72)
        {
            blockSize = 8;
            return Dxt1FourCc;
        }

        if (dxgiFormat is 73 or 74 or 75)
        {
            blockSize = 16;
            return Dxt3FourCc;
        }

        if (dxgiFormat is 76 or 77 or 78)
        {
            blockSize = 16;
            return Dxt5FourCc;
        }

        throw new InvalidDataException($"BA2 texture DXGI format {dxgiFormat} is not supported by the preview reader.");
    }

    private static uint CalculateTopMipLinearSize(ushort width, ushort height, uint blockSize)
    {
        var blockCountX = ((uint)width + 3) / 4;
        var blockCountY = ((uint)height + 3) / 4;
        return blockCountX * blockCountY * blockSize;
    }

    private static void ValidateDataRange(Ba2ArchiveFileRecord record, long archiveLength)
    {
        var dataOffset = checked((long)record.DataOffset);
        var storedSize = record.PackedSize > 0 ? record.PackedSize : record.UnpackedSize;
        var dataEnd = checked(dataOffset + storedSize);
        if (dataOffset < 0 || dataEnd > archiveLength)
        {
            throw new InvalidDataException("BA2 entry data range is outside the archive.");
        }
    }

    private static void ValidateTextureDataRanges(Ba2ArchiveTextureRecord record, long archiveLength)
    {
        if (record.Width == 0 || record.Height == 0)
        {
            throw new InvalidDataException("BA2 texture record has invalid dimensions.");
        }

        foreach (var chunk in record.Chunks)
        {
            var dataOffset = checked((long)chunk.DataOffset);
            var storedSize = chunk.PackedSize > 0 ? chunk.PackedSize : chunk.UnpackedSize;
            var dataEnd = checked(dataOffset + storedSize);
            if (dataOffset < 0 || dataEnd > archiveLength)
            {
                throw new InvalidDataException("BA2 texture chunk data range is outside the archive.");
            }
        }
    }

    private static bool IsSupportedVersion(uint version)
    {
        return version is 1 or 2 or 3 or 7 or 8;
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

    private static void EnsurePreviewSize(long byteCount)
    {
        if (byteCount > BethesdaAssetProvider.MaximumPreviewAssetBytes)
        {
            throw new AssetTooLargeException($"BA2 archive entry is {byteCount} bytes, which exceeds the {BethesdaAssetProvider.MaximumPreviewAssetBytes} byte preview read limit.");
        }
    }

    private static string NormalizeEntryPath(string entryPath)
    {
        return new string(entryPath.Trim().Select(NormalizeEntryCharacter).ToArray());
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
