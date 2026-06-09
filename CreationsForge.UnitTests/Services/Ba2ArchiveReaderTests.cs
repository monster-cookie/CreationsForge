using System.IO.Compression;
using System.Text;
using CreationsForge.Bethesda.Assets.Archives.Ba2;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class Ba2ArchiveReaderTests
{
    [Fact]
    public void CanRead_ReturnsTrueForBa2Archive()
    {
        var reader = new Ba2ArchiveReader();

        var canRead = reader.CanRead("Fallout4 - Meshes.ba2");

        canRead.ShouldBeTrue();
    }

    [Fact]
    public void CanRead_ReturnsFalseForBsaArchive()
    {
        var reader = new Ba2ArchiveReader();

        var canRead = reader.CanRead("Skyrim - Meshes.bsa");

        canRead.ShouldBeFalse();
    }

    [Fact]
    public void ListEntries_ReturnsGeneralArchiveEntries()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Meshes.ba2");
            WriteBa2Archive(
                archivePath,
                [
                    new TestBa2Entry("Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif", [1, 2, 3])
                ]);
            var reader = new Ba2ArchiveReader();

            var entries = reader.ListEntries(archivePath);

            entries.Count.ShouldBe(1);
            entries[0].ArchivePath.ShouldBe(archivePath);
            entries[0].EntryPath.ShouldBe("meshes/setdressing/babybottle/babybottledirty02.nif");
            entries[0].PackedSize.ShouldBe(0);
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
            var archivePath = Path.Combine(tempDirectory.FullName, "Meshes.ba2");
            WriteBa2Archive(
                archivePath,
                [
                    new TestBa2Entry("Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif", [4, 5, 6])
                ]);
            var reader = new Ba2ArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif");

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe([4, 5, 6]);
            result.EntryPath.ShouldBe("meshes/setdressing/babybottle/babybottledirty02.nif");
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
            var archivePath = Path.Combine(tempDirectory.FullName, "Meshes.ba2");
            WriteBa2Archive(
                archivePath,
                [
                    new TestBa2Entry("Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif", [7, 8, 9])
                ]);
            var reader = new Ba2ArchiveReader();

            var result = reader.TryReadEntry(archivePath, "MESHES\\SETDRESSING\\BABYBOTTLE\\BABYBOTTLEDIRTY02.NIF");

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe([7, 8, 9]);
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
            var archivePath = Path.Combine(tempDirectory.FullName, "Meshes.ba2");
            WriteBa2Archive(
                archivePath,
                [
                    new TestBa2Entry("Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif", [1])
                ]);
            var reader = new Ba2ArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes/Missing.nif");

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
    public void TryReadEntry_ReadsCompressedEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Meshes.ba2");
            WriteBa2Archive(
                archivePath,
                [
                    TestBa2Entry.CreateCompressed("Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif", [10, 11, 12, 13])
                ]);
            var reader = new Ba2ArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif");

            result.IsSuccess.ShouldBeTrue();
            result.Data.ShouldBe([10, 11, 12, 13]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReturnsFailureForInvalidCompressedEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Meshes.ba2");
            WriteBa2Archive(
                archivePath,
                [
                    new TestBa2Entry("Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif", [1, 2], 4)
                ]);
            var reader = new Ba2ArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif");

            result.IsSuccess.ShouldBeFalse();
            result.StatusMessage.ShouldNotBeNull();
            string.IsNullOrWhiteSpace(result.StatusMessage).ShouldBeFalse();
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
            var archivePath = Path.Combine(tempDirectory.FullName, "Meshes.ba2");
            File.WriteAllBytes(archivePath, Encoding.ASCII.GetBytes("NOPE"));
            var reader = new Ba2ArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif");

            result.IsSuccess.ShouldBeFalse();
            result.StatusMessage.ShouldNotBeNull();
            result.StatusMessage.ShouldContain("too small");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    private static void WriteBa2Archive(string archivePath, IReadOnlyList<TestBa2Entry> entries)
    {
        const int headerSize = 24;
        const int fileRecordSize = 36;
        var nameTableSize = entries.Sum(entry => sizeof(ushort) + Encoding.UTF8.GetByteCount(entry.Name));
        var nameTableOffset = headerSize + (entries.Count * fileRecordSize);
        var dataOffset = nameTableOffset + nameTableSize;
        using var stream = File.Create(archivePath);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);

        writer.Write(Encoding.ASCII.GetBytes("BTDX"));
        writer.Write(1U);
        writer.Write(Encoding.ASCII.GetBytes("GNRL"));
        writer.Write((uint)entries.Count);
        writer.Write((ulong)nameTableOffset);

        var nextDataOffset = dataOffset;
        foreach (var entry in entries)
        {
            var packedSize = entry.IsCompressed ? entry.StoredData.Length : 0;
            var unpackedSize = entry.UnpackedSize;
            writer.Write(0U);
            writer.Write(0U);
            writer.Write(0U);
            writer.Write(0U);
            writer.Write((ulong)nextDataOffset);
            writer.Write((uint)packedSize);
            writer.Write((uint)unpackedSize);
            writer.Write(0xBAADF00DU);
            nextDataOffset += entry.StoredData.Length;
        }

        foreach (var entry in entries)
        {
            var nameBytes = Encoding.UTF8.GetBytes(entry.Name);
            writer.Write((ushort)nameBytes.Length);
            writer.Write(nameBytes);
        }

        foreach (var entry in entries)
        {
            writer.Write(entry.StoredData);
        }
    }

    private class TestBa2Entry
    {
        public TestBa2Entry(string name, byte[] storedData, int? unpackedSizeOverride = null)
        {
            Name = name;
            StoredData = storedData;
            UnpackedSize = unpackedSizeOverride ?? storedData.Length;
            IsCompressed = unpackedSizeOverride.HasValue;
        }

        public static TestBa2Entry CreateCompressed(string name, byte[] unpackedData)
        {
            using var storedStream = new MemoryStream();
            using (var zlibStream = new ZLibStream(storedStream, CompressionMode.Compress, leaveOpen: true))
            {
                zlibStream.Write(unpackedData);
            }

            return new TestBa2Entry(name, storedStream.ToArray(), unpackedData.Length);
        }

        public string Name { get; }

        public byte[] StoredData { get; }

        public int UnpackedSize { get; }

        public bool IsCompressed { get; }
    }
}
