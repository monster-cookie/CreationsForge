using System.Text;
using CreationsForge.Bethesda.Assets.Archives.Ba2;
using Shouldly;

namespace CreationsForge.UnitTests.Archives;

public class Ba2ArchiveReaderTests
{
    private const uint Magic = 0x58445442;
    private const uint GeneralArchiveType = 0x4C524E47;
    private const uint FileRecordSentinel = 0xBAADF00D;
    private const int HeaderSize = 24;
    private const int FileRecordSize = 36;

    [Fact]
    public void TryReadEntry_AfterPayloadBytesChange_ReturnsLatestPayloadBytes()
    {
        var archivePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.ba2");
        try
        {
            var entryOffset = WriteGeneralArchive(archivePath, [new TestArchiveEntry("geometries/test.mesh", [1, 2, 3])]);
            var originalLastWriteTime = File.GetLastWriteTimeUtc(archivePath);
            var reader = new Ba2ArchiveReader();

            var firstRead = reader.TryReadEntry(archivePath, "geometries/test.mesh");

            firstRead.IsSuccess.ShouldBeTrue();
            firstRead.Data.ShouldBe([1, 2, 3]);

            using (var stream = File.Open(archivePath, FileMode.Open, FileAccess.Write, FileShare.Read))
            {
                stream.Position = entryOffset;
                stream.Write([4, 5, 6]);
            }

            File.SetLastWriteTimeUtc(archivePath, originalLastWriteTime);

            var secondRead = reader.TryReadEntry(archivePath, "geometries/test.mesh");

            secondRead.IsSuccess.ShouldBeTrue();
            secondRead.Data.ShouldBe([4, 5, 6]);
        }
        finally
        {
            DeleteArchive(archivePath);
        }
    }

    [Fact]
    public void TryReadEntry_AfterArchiveDirectoryChanges_InvalidatesCachedDirectory()
    {
        var archivePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.ba2");
        try
        {
            WriteGeneralArchive(archivePath, [new TestArchiveEntry("geometries/old.mesh", [1, 2, 3])]);
            var reader = new Ba2ArchiveReader();

            var firstRead = reader.TryReadEntry(archivePath, "geometries/old.mesh");

            firstRead.IsSuccess.ShouldBeTrue();
            firstRead.Data.ShouldBe([1, 2, 3]);

            WriteGeneralArchive(archivePath, [new TestArchiveEntry("geometries/new.mesh", [4, 5, 6])]);
            File.SetLastWriteTimeUtc(archivePath, DateTime.UtcNow.AddMinutes(1));

            var secondRead = reader.TryReadEntry(archivePath, "geometries/new.mesh");

            secondRead.IsSuccess.ShouldBeTrue();
            secondRead.Data.ShouldBe([4, 5, 6]);
        }
        finally
        {
            DeleteArchive(archivePath);
        }
    }

    private static long WriteGeneralArchive(string archivePath, IReadOnlyList<TestArchiveEntry> entries)
    {
        var normalizedEntries = entries
            .Select(entry => new TestArchiveEntry(NormalizeEntryPath(entry.EntryPath), entry.Data))
            .ToList();
        var nameTableSize = normalizedEntries.Sum(entry => sizeof(ushort) + Encoding.UTF8.GetByteCount(entry.EntryPath));
        var nameTableOffset = HeaderSize + (FileRecordSize * normalizedEntries.Count);
        var nextDataOffset = nameTableOffset + nameTableSize;
        var firstDataOffset = nextDataOffset;
        Directory.CreateDirectory(Path.GetDirectoryName(archivePath)!);

        using var stream = File.Create(archivePath);
        using var writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: false);
        writer.Write(Magic);
        writer.Write(1U);
        writer.Write(GeneralArchiveType);
        writer.Write((uint)normalizedEntries.Count);
        writer.Write((ulong)nameTableOffset);

        foreach (var entry in normalizedEntries)
        {
            writer.Write(0U);
            writer.Write(0U);
            writer.Write(0U);
            writer.Write(0U);
            writer.Write((ulong)nextDataOffset);
            writer.Write(0U);
            writer.Write((uint)entry.Data.Length);
            writer.Write(FileRecordSentinel);
            nextDataOffset += entry.Data.Length;
        }

        foreach (var entry in normalizedEntries)
        {
            var nameBytes = Encoding.UTF8.GetBytes(entry.EntryPath);
            writer.Write((ushort)nameBytes.Length);
            writer.Write(nameBytes);
        }

        foreach (var entry in normalizedEntries)
        {
            writer.Write(entry.Data);
        }

        return firstDataOffset;
    }

    private static string NormalizeEntryPath(string entryPath)
    {
        return entryPath.Trim().Replace('\\', '/').ToLowerInvariant();
    }

    private static void DeleteArchive(string archivePath)
    {
        if (File.Exists(archivePath))
        {
            File.Delete(archivePath);
        }
    }

    private sealed class TestArchiveEntry
    {
        public TestArchiveEntry(string entryPath, byte[] data)
        {
            EntryPath = entryPath;
            Data = data;
        }

        public string EntryPath { get; }

        public byte[] Data { get; }
    }
}
