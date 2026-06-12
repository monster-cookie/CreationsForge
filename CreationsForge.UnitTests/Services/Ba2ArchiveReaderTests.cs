using System.IO.Compression;
using System.Text;
using CreationsForge.Bethesda.Assets.Archives.Ba2;
using K4os.Compression.LZ4;
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
    public void ListEntries_ReturnsTextureArchiveEntriesWithDdsHeaderSize()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Textures.ba2");
            WriteBa2TextureArchive(
                archivePath,
                [
                    new TestBa2TextureEntry("Textures/SetDressing/BabyBottle/BabyBottleDirty01_d.dds", 4, 4, 71, [1, 2, 3, 4, 5, 6, 7, 8])
                ]);
            var reader = new Ba2ArchiveReader();

            var entries = reader.ListEntries(archivePath);

            entries.Count.ShouldBe(1);
            entries[0].ArchivePath.ShouldBe(archivePath);
            entries[0].EntryPath.ShouldBe("textures/setdressing/babybottle/babybottledirty01_d.dds");
            entries[0].PackedSize.ShouldBe(0);
            entries[0].UnpackedSize.ShouldBe(136U);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsUncompressedTextureEntryAsDds()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Textures.ba2");
            var textureData = new byte[] { 10, 11, 12, 13, 14, 15, 16, 17 };
            WriteBa2TextureArchive(
                archivePath,
                [
                    new TestBa2TextureEntry("Textures/SetDressing/BabyBottle/BabyBottleDirty01_d.dds", 4, 4, 71, textureData)
                ]);
            var reader = new Ba2ArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Textures\\SetDressing\\BabyBottle\\BabyBottleDirty01_d.dds");

            result.IsSuccess.ShouldBeTrue();
            var data = result.Data.ShouldNotBeNull();
            data.Length.ShouldBe(136);
            Encoding.ASCII.GetString(data, 0, 4).ShouldBe("DDS ");
            BitConverter.ToUInt32(data, 12).ShouldBe(4U);
            BitConverter.ToUInt32(data, 16).ShouldBe(4U);
            Encoding.ASCII.GetString(data, 84, 4).ShouldBe("DXT1");
            data.Skip(128).ToArray().ShouldBe(textureData);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsCompressedTextureEntryAsDds()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Textures.ba2");
            var textureData = new byte[] { 20, 21, 22, 23, 24, 25, 26, 27 };
            WriteBa2TextureArchive(
                archivePath,
                [
                    TestBa2TextureEntry.CreateCompressed("Textures/SetDressing/BabyBottle/BabyBottleDirty01_d.dds", 4, 4, 77, textureData)
                ]);
            var reader = new Ba2ArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Textures/SetDressing/BabyBottle/BabyBottleDirty01_d.dds");

            result.IsSuccess.ShouldBeTrue();
            var data = result.Data.ShouldNotBeNull();
            data.Length.ShouldBe(136);
            Encoding.ASCII.GetString(data, 0, 4).ShouldBe("DDS ");
            Encoding.ASCII.GetString(data, 84, 4).ShouldBe("DXT5");
            data.Skip(128).ToArray().ShouldBe(textureData);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReadsStarfieldVersion3CompressedTextureEntryAsDds()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Starfield - Textures03.ba2");
            var textureData = new byte[] { 30, 31, 32, 33, 34, 35, 36, 37 };
            WriteBa2TextureArchive(
                archivePath,
                [
                    TestBa2TextureEntry.CreateLz4Compressed("Textures/Cinimatics/DigiPic/DigiPick_Material_color.DDS", 4, 4, 72, textureData)
                ],
                version: 3,
                headerSize: 36);
            var reader = new Ba2ArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Textures\\Cinimatics\\DigiPic\\DigiPick_Material_color.DDS");

            result.IsSuccess.ShouldBeTrue();
            result.EntryPath.ShouldBe("textures/cinimatics/digipic/digipick_material_color.dds");
            var data = result.Data.ShouldNotBeNull();
            data.Length.ShouldBe(136);
            Encoding.ASCII.GetString(data, 0, 4).ShouldBe("DDS ");
            Encoding.ASCII.GetString(data, 84, 4).ShouldBe("DXT1");
            data.Skip(128).ToArray().ShouldBe(textureData);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadEntry_ReturnsFailureForUnsupportedTextureFormat()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Textures.ba2");
            WriteBa2TextureArchive(
                archivePath,
                [
                    new TestBa2TextureEntry("Textures/Unsupported.dds", 4, 4, 98, [1, 2, 3, 4])
                ]);
            var reader = new Ba2ArchiveReader();

            var result = reader.TryReadEntry(archivePath, "Textures/Unsupported.dds");

            result.IsSuccess.ShouldBeFalse();
            result.StatusMessage.ShouldNotBeNull();
            result.StatusMessage.ShouldContain("DXGI format 98");
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

    private static void WriteBa2TextureArchive(
        string archivePath,
        IReadOnlyList<TestBa2TextureEntry> entries,
        uint version = 1,
        int headerSize = 24)
    {
        const int textureRecordSize = 24;
        const int textureChunkSize = 24;
        var nameTableSize = entries.Sum(entry => sizeof(ushort) + Encoding.UTF8.GetByteCount(entry.Name));
        var nameTableOffset = headerSize + entries.Sum(entry => textureRecordSize + (entry.Chunks.Count * textureChunkSize));
        var dataOffset = nameTableOffset + nameTableSize;
        using var stream = File.Create(archivePath);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);

        writer.Write(Encoding.ASCII.GetBytes("BTDX"));
        writer.Write(version);
        writer.Write(Encoding.ASCII.GetBytes("DX10"));
        writer.Write((uint)entries.Count);
        writer.Write((ulong)nameTableOffset);
        if (headerSize > 24)
        {
            writer.Write(new byte[headerSize - 24]);
        }

        var nextDataOffset = dataOffset;
        foreach (var entry in entries)
        {
            writer.Write(0U);
            writer.Write(Encoding.ASCII.GetBytes("dds\0"));
            writer.Write(0U);
            writer.Write((byte)0);
            writer.Write((byte)entry.Chunks.Count);
            writer.Write((ushort)textureChunkSize);
            writer.Write((ushort)entry.Height);
            writer.Write((ushort)entry.Width);
            writer.Write((byte)1);
            writer.Write(entry.DxgiFormat);
            writer.Write((ushort)0);

            foreach (var chunk in entry.Chunks)
            {
                writer.Write((ulong)nextDataOffset);
                writer.Write(chunk.IsCompressed ? (uint)chunk.StoredData.Length : 0U);
                writer.Write((uint)chunk.UnpackedSize);
                writer.Write((ushort)0);
                writer.Write((ushort)0);
                writer.Write(0xBAADF00DU);
                nextDataOffset += chunk.StoredData.Length;
            }
        }

        foreach (var entry in entries)
        {
            var nameBytes = Encoding.UTF8.GetBytes(entry.Name);
            writer.Write((ushort)nameBytes.Length);
            writer.Write(nameBytes);
        }

        foreach (var entry in entries)
        {
            foreach (var chunk in entry.Chunks)
            {
                writer.Write(chunk.StoredData);
            }
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

    private class TestBa2TextureEntry
    {
        public TestBa2TextureEntry(string name, int width, int height, byte dxgiFormat, byte[] storedData)
            : this(name, width, height, dxgiFormat, [new TestBa2TextureChunk(storedData, storedData.Length, false)])
        {
        }

        private TestBa2TextureEntry(string name, int width, int height, byte dxgiFormat, IReadOnlyList<TestBa2TextureChunk> chunks)
        {
            Name = name;
            Width = width;
            Height = height;
            DxgiFormat = dxgiFormat;
            Chunks = chunks;
        }

        public static TestBa2TextureEntry CreateCompressed(string name, int width, int height, byte dxgiFormat, byte[] unpackedData)
        {
            using var storedStream = new MemoryStream();
            using (var zlibStream = new ZLibStream(storedStream, CompressionMode.Compress, leaveOpen: true))
            {
                zlibStream.Write(unpackedData);
            }

            return new TestBa2TextureEntry(name, width, height, dxgiFormat, [new TestBa2TextureChunk(storedStream.ToArray(), unpackedData.Length, true)]);
        }

        public static TestBa2TextureEntry CreateLz4Compressed(string name, int width, int height, byte dxgiFormat, byte[] unpackedData)
        {
            var maximumSize = LZ4Codec.MaximumOutputSize(unpackedData.Length);
            var storedData = new byte[maximumSize];
            var storedSize = LZ4Codec.Encode(unpackedData, storedData, LZ4Level.L00_FAST);
            storedSize.ShouldBeGreaterThan(0);
            Array.Resize(ref storedData, storedSize);
            return new TestBa2TextureEntry(name, width, height, dxgiFormat, [new TestBa2TextureChunk(storedData, unpackedData.Length, true)]);
        }

        public string Name { get; }

        public int Width { get; }

        public int Height { get; }

        public byte DxgiFormat { get; }

        public IReadOnlyList<TestBa2TextureChunk> Chunks { get; }
    }

    private class TestBa2TextureChunk
    {
        public TestBa2TextureChunk(byte[] storedData, int unpackedSize, bool isCompressed)
        {
            StoredData = storedData;
            UnpackedSize = unpackedSize;
            IsCompressed = isCompressed;
        }

        public byte[] StoredData { get; }

        public int UnpackedSize { get; }

        public bool IsCompressed { get; }
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
