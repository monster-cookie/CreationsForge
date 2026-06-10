using System.IO.Compression;
using System.Text;
using CreationsForge.Bethesda.Assets.Archives.Bsa;
using K4os.Compression.LZ4;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class BsaArchiveReaderTests
{
    [Fact]
    public void CanRead_ReturnsTrueForBsaArchive()
    {
        var reader = new BsaArchiveReader();

        var canRead = reader.CanRead("Skyrim - Meshes.bsa");

        canRead.ShouldBeTrue();
    }

    [Fact]
    public void CanRead_ReturnsFalseForBa2Archive()
    {
        var reader = new BsaArchiveReader();

        var canRead = reader.CanRead("Fallout4 - Meshes.ba2");

        canRead.ShouldBeFalse();
    }

    [Fact]
    public void ListEntries_ReturnsArchiveEntries()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    new TestBsaEntry("Meshes\\Clutter", "Basket04.NIF", [1, 2, 3])
                ],
                compressedByDefault: false);
            var reader = new BsaArchiveReader();

            var entries = reader.ListEntries(archivePath);

            entries.Count.ShouldBe(1);
            entries[0].ArchivePath.ShouldBe(archivePath);
            entries[0].EntryPath.ShouldBe("meshes/clutter/basket04.nif");
            entries[0].PackedSize.ShouldBe(3);
            entries[0].UnpackedSize.ShouldBe(3);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsUncompressedEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    new TestBsaEntry("Meshes\\Clutter", "Basket04.NIF", [4, 5, 6])
                ],
                compressedByDefault: false);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes\\Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.EntryPath.ShouldBe("meshes/clutter/basket04.nif");
            result.Data.ShouldBe([4, 5, 6]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsWideFolderRecordEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    new TestBsaEntry("Meshes\\Clutter", "Basket04.NIF", [4, 5, 6])
                ],
                compressedByDefault: false,
                useWideFolderRecords: true);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes\\Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.EntryPath.ShouldBe("meshes/clutter/basket04.nif");
            result.Data.ShouldBe([4, 5, 6]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsLegacyFolderRecordEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Legacy Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    new TestBsaEntry("Meshes\\Clutter", "Basket04.NIF", [4, 5, 6])
                ],
                compressedByDefault: false,
                useWideFolderRecords: false);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes\\Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.EntryPath.ShouldBe("meshes/clutter/basket04.nif");
            result.Data.ShouldBe([4, 5, 6]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_NormalizesPathCaseAndSeparators()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    new TestBsaEntry("Meshes\\Clutter", "Basket04.NIF", [7, 8, 9])
                ],
                compressedByDefault: false);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "MESHES/CLUTTER/BASKET04.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe([7, 8, 9]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsUniqueSuffixMatch()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    new TestBsaEntry("Meshes\\Clutter", "Basket04.NIF", [11, 12, 13])
                ],
                compressedByDefault: false);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.EntryPath.ShouldBe("meshes/clutter/basket04.nif");
            result.Data.ShouldBe([11, 12, 13]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReturnsFailureForAmbiguousSuffixMatch()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    new TestBsaEntry("Meshes\\Clutter", "Basket04.NIF", [11]),
                    new TestBsaEntry("Textures\\Clutter", "Basket04.NIF", [12])
                ],
                compressedByDefault: false);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeFalse();
            result.StatusMessage.ShouldNotBeNull();
            result.StatusMessage.ShouldContain("Multiple BSA archive entries matched");
            result.StatusMessage.ShouldContain("meshes/clutter/basket04.nif");
            result.StatusMessage.ShouldContain("textures/clutter/basket04.nif");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsCompressedEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    TestBsaEntry.CreateCompressed("Meshes\\Clutter", "Basket04.NIF", [10, 11, 12, 13])
                ],
                compressedByDefault: true);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes\\Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe([10, 11, 12, 13]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsUncompressedEntryWithEmbeddedFileName()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    new TestBsaEntry("Meshes\\Clutter", "Basket04.NIF", [14, 15, 16])
                ],
                compressedByDefault: false,
                embedFileNames: true);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes\\Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe([14, 15, 16]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsCompressedEntryWithEmbeddedFileName()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    TestBsaEntry.CreateCompressed("Meshes\\Clutter", "Basket04.NIF", [17, 18, 19, 20])
                ],
                compressedByDefault: true,
                embedFileNames: true);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes\\Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe([17, 18, 19, 20]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsLz4CompressedEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    TestBsaEntry.CreateLz4Compressed("Meshes\\Clutter", "Basket04.NIF", [21, 22, 23, 24])
                ],
                compressedByDefault: true);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes\\Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe([21, 22, 23, 24]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsLz4CompressedEntryWithEmbeddedFileName()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    TestBsaEntry.CreateLz4Compressed("Meshes\\Clutter", "Basket04.NIF", [25, 26, 27, 28])
                ],
                compressedByDefault: true,
                embedFileNames: true);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes\\Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe([25, 26, 27, 28]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReturnsFailureForMissingEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    new TestBsaEntry("Meshes\\Clutter", "Basket04.NIF", [1])
                ],
                compressedByDefault: false);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes\\Clutter\\Missing.NIF");

            result.IsSuccess.ShouldBeFalse();
            result.StatusMessage.ShouldNotBeNull();
            result.StatusMessage.ShouldContain("was not found");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReturnsNearMatchesForMissingEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteBsaArchive(
                archivePath,
                [
                    new TestBsaEntry("Meshes\\Clutter", "Basket04.NIF", [1])
                ],
                compressedByDefault: false);
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Props\\Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeFalse();
            result.StatusMessage.ShouldNotBeNull();
            result.StatusMessage.ShouldContain("after indexing 1 entries");
            result.StatusMessage.ShouldContain("Normalized request: props/clutter/basket04.nif");
            result.StatusMessage.ShouldContain("Near matches: meshes/clutter/basket04.nif");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReturnsFailureForInvalidMagic()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            File.WriteAllBytes(archivePath, Encoding.ASCII.GetBytes("NOPE"));
            var reader = new BsaArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes\\Clutter\\Basket04.NIF");

            result.IsSuccess.ShouldBeFalse();
            result.StatusMessage.ShouldNotBeNull();
            result.StatusMessage.ShouldContain("too small");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    private static void WriteBsaArchive(
        string archivePath,
        IReadOnlyList<TestBsaEntry> entries,
        bool compressedByDefault,
        bool useWideFolderRecords = true,
        bool embedFileNames = false)
    {
        const int headerSize = 36;
        const int fileRecordSize = 16;
        var folderRecordSize = useWideFolderRecords ? 24 : 16;
        var folders = entries
            .GroupBy(entry => entry.FolderName, StringComparer.OrdinalIgnoreCase)
            .Select(group => new TestBsaFolder(group.Key, group.ToList()))
            .ToList();
        var storedDataByEntry = entries.ToDictionary(entry => entry, entry => CreateStoredData(entry, embedFileNames));
        var folderRecordOffset = headerSize;
        var folderBlocksOffset = folderRecordOffset + (folders.Count * folderRecordSize);
        var fileNameTableSize = entries.Sum(entry => Encoding.UTF8.GetByteCount(entry.FileName) + 1);
        var folderBlocksSize = folders.Sum(folder => 1 + Encoding.UTF8.GetByteCount(folder.Name) + 1 + (folder.Entries.Count * fileRecordSize));
        var dataOffset = folderBlocksOffset + folderBlocksSize + fileNameTableSize;
        var archiveFlags = 0x1U | 0x2U;
        if (compressedByDefault)
        {
            archiveFlags |= 0x4U;
        }

        if (embedFileNames)
        {
            archiveFlags |= 0x100U;
        }

        using var stream = File.Create(archivePath);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        writer.Write(Encoding.ASCII.GetBytes("BSA\0"));
        writer.Write(useWideFolderRecords ? 105U : 104U);
        writer.Write((uint)folderRecordOffset);
        writer.Write(archiveFlags);
        writer.Write((uint)folders.Count);
        writer.Write((uint)entries.Count);
        writer.Write((uint)folders.Sum(folder => Encoding.UTF8.GetByteCount(folder.Name) + 2));
        writer.Write((uint)fileNameTableSize);
        writer.Write(0U);

        var nextFolderBlockOffset = folderBlocksOffset;
        foreach (var folder in folders)
        {
            writer.Write(0UL);
            writer.Write((uint)folder.Entries.Count);
            if (useWideFolderRecords)
            {
                writer.Write(0U);
                writer.Write((ulong)nextFolderBlockOffset);
            }
            else
            {
                writer.Write((uint)nextFolderBlockOffset);
            }

            nextFolderBlockOffset += 1 + Encoding.UTF8.GetByteCount(folder.Name) + 1 + (folder.Entries.Count * fileRecordSize);
        }

        var nextDataOffset = dataOffset;
        foreach (var folder in folders)
        {
            var folderNameBytes = Encoding.UTF8.GetBytes(folder.Name);
            writer.Write((byte)(folderNameBytes.Length + 1));
            writer.Write(folderNameBytes);
            writer.Write((byte)0);

            foreach (var entry in folder.Entries)
            {
                var storedData = storedDataByEntry[entry];
                writer.Write(0UL);
                writer.Write(CreatePackedFileRecord((uint)storedData.Length, (uint)nextDataOffset));
                nextDataOffset += storedData.Length;
            }
        }

        foreach (var entry in entries)
        {
            writer.Write(Encoding.UTF8.GetBytes(entry.FileName));
            writer.Write((byte)0);
        }

        foreach (var entry in entries)
        {
            writer.Write(storedDataByEntry[entry]);
        }
    }

    private static byte[] CreateStoredData(TestBsaEntry entry, bool embedFileName)
    {
        if (!embedFileName)
        {
            return entry.StoredData;
        }

        var embeddedFileName = Encoding.UTF8.GetBytes(entry.FileName);
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        writer.Write((byte)embeddedFileName.Length);
        writer.Write(embeddedFileName);
        writer.Write(entry.StoredData);
        return stream.ToArray();
    }

    private static ulong CreatePackedFileRecord(uint storedSize, uint dataOffset)
    {
        return ((ulong)dataOffset << 32) | storedSize;
    }

    private class TestBsaFolder
    {
        public TestBsaFolder(string name, IReadOnlyList<TestBsaEntry> entries)
        {
            Name = name;
            Entries = entries;
        }

        public string Name { get; }

        public IReadOnlyList<TestBsaEntry> Entries { get; }
    }

    private class TestBsaEntry
    {
        public TestBsaEntry(string folderName, string fileName, byte[] storedData)
        {
            FolderName = folderName;
            FileName = fileName;
            StoredData = storedData;
        }

        public static TestBsaEntry CreateCompressed(string folderName, string fileName, byte[] unpackedData)
        {
            using var storedStream = new MemoryStream();
            using (var writer = new BinaryWriter(storedStream, Encoding.UTF8, leaveOpen: true))
            {
                writer.Write((uint)unpackedData.Length);
            }

            using (var zlibStream = new ZLibStream(storedStream, CompressionMode.Compress, leaveOpen: true))
            {
                zlibStream.Write(unpackedData);
            }

            return new TestBsaEntry(folderName, fileName, storedStream.ToArray());
        }

        public static TestBsaEntry CreateLz4Compressed(string folderName, string fileName, byte[] unpackedData)
        {
            using var storedStream = new MemoryStream();
            using (var writer = new BinaryWriter(storedStream, Encoding.UTF8, leaveOpen: true))
            {
                writer.Write((uint)unpackedData.Length);
            }

            var packedData = new byte[LZ4Codec.MaximumOutputSize(unpackedData.Length)];
            var packedLength = LZ4Codec.Encode(unpackedData, packedData);
            if (packedLength <= 0)
            {
                throw new InvalidOperationException("Test LZ4 compression failed.");
            }

            storedStream.Write(packedData, 0, packedLength);
            return new TestBsaEntry(folderName, fileName, storedStream.ToArray());
        }

        public string FolderName { get; }

        public string FileName { get; }

        public byte[] StoredData { get; }
    }
}
