using System.IO.Compression;
using System.Text;

namespace CreationsForge.Bethesda.Assets.Archives.Ba2;

public class Ba2ArchiveReader : IAssetArchiveReader
{
    private const uint Magic = 0x58445442;
    private const uint GeneralArchiveType = 0x4C524E47;
    private const uint FileRecordSentinel = 0xBAADF00D;
    private const int InitialHeaderSize = 12;
    private const int StandardHeaderSize = 24;
    private const int ExtendedHeaderSize = 32;
    private const int FileRecordSize = 36;

    public bool CanRead(string archivePath)
    {
        return string.Equals(Path.GetExtension(archivePath), ".ba2", StringComparison.OrdinalIgnoreCase);
    }

    public IReadOnlyList<AssetArchiveEntry> ListEntries(string archivePath)
    {
        using var stream = File.OpenRead(archivePath);
        using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: false);
        var header = ReadHeader(reader, stream.Length);
        var records = ReadFileRecords(reader, header, stream.Length);
        var names = ReadNameTable(reader, header, stream.Length);
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

    public AssetArchiveReadResult TryReadEntry(string archivePath, string entryPath)
    {
        try
        {
            var normalizedEntryPath = NormalizeEntryPath(entryPath);
            using var stream = File.OpenRead(archivePath);
            using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: false);
            var header = ReadHeader(reader, stream.Length);
            var records = ReadFileRecords(reader, header, stream.Length);
            var names = ReadNameTable(reader, header, stream.Length);

            for (var index = 0; index < names.Count; index++)
            {
                if (!string.Equals(names[index], normalizedEntryPath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var record = records[index];
                ValidateDataRange(record, stream.Length);
                stream.Position = checked((long)record.DataOffset);
                var data = ReadEntryData(reader, record);

                return new AssetArchiveReadResult
                {
                    IsSuccess = true,
                    ArchivePath = archivePath,
                    EntryPath = names[index],
                    Data = data,
                    StatusMessage = $"Read BA2 archive entry {names[index]} from {archivePath}."
                };
            }

            return CreateFailure(archivePath, entryPath, $"BA2 archive entry {entryPath} was not found.");
        }
        catch (Exception exception) when (exception is EndOfStreamException or IOException or InvalidDataException or OverflowException)
        {
            return CreateFailure(archivePath, entryPath, exception.Message);
        }
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

        var archiveType = reader.ReadUInt32();
        if (archiveType != GeneralArchiveType)
        {
            throw new InvalidDataException("Only BA2 general archives are supported.");
        }

        var headerSize = version is 2 or 3 ? ExtendedHeaderSize : StandardHeaderSize;
        RequireLength(archiveLength, headerSize, "BA2 archive is too small to contain a complete general header.");
        var fileCount = reader.ReadUInt32();
        var nameTableOffset = reader.ReadUInt64();
        if (headerSize > StandardHeaderSize)
        {
            reader.BaseStream.Position = headerSize;
        }

        var recordTableEnd = checked(headerSize + ((long)fileCount * FileRecordSize));
        if (nameTableOffset < (ulong)recordTableEnd || nameTableOffset > (ulong)archiveLength)
        {
            throw new InvalidDataException("BA2 name table offset is outside the archive.");
        }

        return new Ba2ArchiveHeader(version, headerSize, fileCount, nameTableOffset);
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
}
