using System.IO.Compression;
using System.Text;
using CreationsForge.Bethesda.Assets.Archives;
using CreationsForge.Bethesda.Assets.Archives.Ba2;
using CreationsForge.Bethesda.Assets.Archives.Bsa;
using CreationsForge.Bethesda.Assets.Resources;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class BethesdaAssetProviderTests
{
    [Fact]
    public void TryReadAsset_ReturnsLooseFileBytes()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var filePath = Path.Combine(tempDirectory.FullName, "Preview.nif");
            File.WriteAllBytes(filePath, [1, 2, 3]);
            var provider = new BethesdaAssetProvider([]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                AssetPath = filePath
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadLooseFile);
            result.SourceType.ShouldBe(BethesdaAssetSourceType.LooseFile);
            result.ResolvedPath.ShouldBe(filePath);
            result.Data.ShouldBe([1, 2, 3]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_TriesMeshesPrefixForRelativeLooseFiles()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var meshDirectory = Path.Combine(tempDirectory.FullName, "Meshes", "SetDressing", "BabyBottle");
            Directory.CreateDirectory(meshDirectory);
            var filePath = Path.Combine(meshDirectory, "BabyBottleDirty02.nif");
            File.WriteAllBytes(filePath, [4, 5, 6]);
            var provider = new BethesdaAssetProvider([]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = "SetDressing\\BabyBottle\\BabyBottleDirty02.nif"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadLooseFile);
            result.ResolvedPath.ShouldBe(filePath);
            result.Data.ShouldBe([4, 5, 6]);
            result.SearchedPaths.ShouldContain(Path.Combine(tempDirectory.FullName, "SetDressing", "BabyBottle", "BabyBottleDirty02.nif"));
            result.SearchedPaths.ShouldContain(filePath);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_DispatchesToArchiveReader()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Fallout4 - Meshes.ba2");
            File.WriteAllText(archivePath, "archive");
            var archiveReader = new FakeArchiveReader(archivePath, "Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif", [7, 8, 9]);
            var provider = new BethesdaAssetProvider([archiveReader]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = "SetDressing\\BabyBottle\\BabyBottleDirty02.nif"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            result.SourceType.ShouldBe(BethesdaAssetSourceType.Archive);
            result.SourceArchivePath.ShouldBe(archivePath);
            result.NormalizedEntryPath.ShouldBe("Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif");
            result.Data.ShouldBe([7, 8, 9]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_ReadsCompressedBa2ArchiveEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Fallout4 - Meshes.ba2");
            WriteSingleCompressedBa2Archive(
                archivePath,
                "Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif",
                [10, 11, 12, 13]);
            var provider = new BethesdaAssetProvider([new Ba2ArchiveReader()]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = "SetDressing\\BabyBottle\\BabyBottleDirty02.nif"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            result.SourceType.ShouldBe(BethesdaAssetSourceType.Archive);
            result.SourceArchivePath.ShouldBe(archivePath);
            result.NormalizedEntryPath.ShouldBe("meshes/setdressing/babybottle/babybottledirty02.nif");
            result.Data.ShouldBe([10, 11, 12, 13]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_ReadsBsaArchiveEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            WriteSingleBsaArchiveEntry(
                archivePath,
                "Meshes\\Clutter",
                "Basket04.NIF",
                [14, 15, 16]);
            var provider = new BethesdaAssetProvider([new BsaArchiveReader()]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = "Meshes\\Clutter\\Basket04.NIF"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            result.SourceType.ShouldBe(BethesdaAssetSourceType.Archive);
            result.SourceArchivePath.ShouldBe(archivePath);
            result.NormalizedEntryPath.ShouldBe("meshes/clutter/basket04.nif");
            result.Data.ShouldBe([14, 15, 16]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_ReadsArchiveEntryStoredWithoutAssetRoot()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            File.WriteAllText(archivePath, "archive");
            var archiveReader = new FakeArchiveReader(archivePath, "Clutter/Basket04.NIF", [17, 18, 19]);
            var provider = new BethesdaAssetProvider([archiveReader]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = "Meshes\\Clutter\\Basket04.NIF"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            result.SourceType.ShouldBe(BethesdaAssetSourceType.Archive);
            result.SourceArchivePath.ShouldBe(archivePath);
            result.NormalizedEntryPath.ShouldBe("Clutter/Basket04.NIF");
            result.Data.ShouldBe([17, 18, 19]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_ReadsArchiveEntryStoredWithDataRoot()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Starfield - Textures01.ba2");
            File.WriteAllText(archivePath, "archive");
            var archiveReader = new FakeArchiveReader(archivePath, "Data/Textures/Cinimatics/DigiPic/DigiPick_Material_color.DDS", [20, 21, 22]);
            var provider = new BethesdaAssetProvider([archiveReader]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = @"Textures\Cinimatics\DigiPic\DigiPick_Material_color.DDS"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            result.SourceType.ShouldBe(BethesdaAssetSourceType.Archive);
            result.SourceArchivePath.ShouldBe(archivePath);
            result.NormalizedEntryPath.ShouldBe("Data/Textures/Cinimatics/DigiPic/DigiPick_Material_color.DDS");
            result.Data.ShouldBe([20, 21, 22]);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_PrefersMeshArchivesForMeshPaths()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var soundsArchivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Sounds.bsa");
            var meshesArchivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes0.bsa");
            File.WriteAllText(soundsArchivePath, "archive");
            File.WriteAllText(meshesArchivePath, "archive");
            var archiveReader = new RecordingArchiveReader(
                meshesArchivePath,
                "Meshes/Clutter/Basket04.NIF",
                [20, 21, 22]);
            var provider = new BethesdaAssetProvider([archiveReader]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = "Meshes\\Clutter\\Basket04.NIF"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            result.SourceArchivePath.ShouldBe(meshesArchivePath);
            archiveReader.Attempts.ShouldNotBeEmpty();
            archiveReader.Attempts[0].ArchivePath.ShouldBe(meshesArchivePath);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_PrefersTextureArchivesForDataRootedTexturePaths()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var meshesArchivePath = Path.Combine(tempDirectory.FullName, "Starfield - Meshes01.ba2");
            var texturesArchivePath = Path.Combine(tempDirectory.FullName, "Starfield - Textures01.ba2");
            File.WriteAllText(meshesArchivePath, "archive");
            File.WriteAllText(texturesArchivePath, "archive");
            var archiveReader = new RecordingArchiveReader(
                texturesArchivePath,
                "Data/Textures/Cinimatics/DigiPic/DigiPick_Material_color.DDS",
                [23, 24, 25]);
            var provider = new BethesdaAssetProvider([archiveReader]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = @"Data\Textures\Cinimatics\DigiPic\DigiPick_Material_color.DDS"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            result.SourceArchivePath.ShouldBe(texturesArchivePath);
            archiveReader.Attempts.ShouldNotBeEmpty();
            archiveReader.Attempts[0].ArchivePath.ShouldBe(texturesArchivePath);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_ReturnsArchiveAttemptSummaryWhenEntryIsMissing()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var soundsArchivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Sounds.bsa");
            var meshesArchivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes0.bsa");
            File.WriteAllText(soundsArchivePath, "archive");
            File.WriteAllText(meshesArchivePath, "archive");
            var archiveReader = new RecordingArchiveReader(
                "Meshes/Clutter/Missing.NIF",
                [23, 24, 25]);
            var provider = new BethesdaAssetProvider([archiveReader]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = "Meshes\\Clutter\\Basket04.NIF"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ArchiveEntryMissing);
            result.StatusMessage.ShouldNotBeNull();
            result.StatusMessage.ShouldContain("Archive attempts:");
            result.StatusMessage.ShouldContain("Skyrim - Meshes0.bsa");
            result.StatusMessage.ShouldContain("Skyrim - Sounds.bsa");
            result.StatusMessage.ShouldContain("Meshes/Clutter/Basket04.NIF");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_ReturnsArchiveReaderUnavailableWhenArchiveExistsWithoutReader()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            File.WriteAllText(Path.Combine(tempDirectory.FullName, "Fallout4 - Meshes.ba2"), "archive");
            var provider = new BethesdaAssetProvider([]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = "SetDressing\\BabyBottle\\BabyBottleDirty02.nif"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ArchiveReaderUnavailable);
            result.SourceType.ShouldBe(BethesdaAssetSourceType.Archive);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadAsset_ReturnsArchiveReaderFailureMessageWhenEntryCannotBeRead()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Fallout4 - Meshes.ba2");
            File.WriteAllText(archivePath, "archive");
            var archiveReader = new FakeArchiveReader(archivePath, "Meshes/SetDressing/BabyBottle/BabyBottleDirty02.nif", [7, 8, 9])
            {
                FailureMessage = "Archive reader could not read the entry."
            };
            var provider = new BethesdaAssetProvider([archiveReader]);

            var result = provider.TryReadAsset(new BethesdaAssetReadRequest
            {
                DataFolder = tempDirectory.FullName,
                AssetPath = "SetDressing\\BabyBottle\\BabyBottleDirty02.nif"
            });

            result.Status.ShouldBe(BethesdaAssetReadStatus.ArchiveEntryMissing);
            result.StatusMessage.ShouldNotBeNull();
            result.StatusMessage.ShouldContain("Archive attempts:");
            result.StatusMessage.ShouldContain("Fallout4 - Meshes.ba2");
            result.StatusMessage.ShouldContain("Archive reader could not read the entry.");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    private class RecordingArchiveReadAttempt
    {
        public RecordingArchiveReadAttempt(string archivePath, string entryPath)
        {
            ArchivePath = archivePath;
            EntryPath = entryPath;
        }

        public string ArchivePath { get; }

        public string EntryPath { get; }
    }

    private class RecordingArchiveReader : IAssetArchiveReader
    {
        private readonly string? SuccessArchivePath;
        private readonly string EntryPath;
        private readonly byte[] Data;

        public RecordingArchiveReader(string successArchivePath, string entryPath, byte[] data)
        {
            SuccessArchivePath = successArchivePath;
            EntryPath = entryPath;
            Data = data;
        }

        public RecordingArchiveReader(string entryPath, byte[] data)
        {
            EntryPath = entryPath;
            Data = data;
        }

        public List<RecordingArchiveReadAttempt> Attempts { get; } = [];

        public bool CanRead(string archivePath)
        {
            return string.Equals(Path.GetExtension(archivePath), ".bsa", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(Path.GetExtension(archivePath), ".ba2", StringComparison.OrdinalIgnoreCase);
        }

        public IReadOnlyList<AssetArchiveEntry> ListEntries(string archivePath)
        {
            return [];
        }

        public AssetArchiveReadResult TryReadEntry(string archivePath, string entryPath)
        {
            Attempts.Add(new RecordingArchiveReadAttempt(archivePath, entryPath));
            if ((SuccessArchivePath == null || string.Equals(SuccessArchivePath, archivePath, StringComparison.OrdinalIgnoreCase)) &&
                string.Equals(EntryPath, entryPath, StringComparison.OrdinalIgnoreCase))
            {
                return new AssetArchiveReadResult
                {
                    IsSuccess = true,
                    ArchivePath = archivePath,
                    EntryPath = entryPath,
                    Data = Data,
                    StatusMessage = "Read recording archive entry."
                };
            }

            return new AssetArchiveReadResult
            {
                IsSuccess = false,
                ArchivePath = archivePath,
                EntryPath = entryPath,
                StatusMessage = $"Recording archive entry {entryPath} was not found."
            };
        }
    }

    private class FakeArchiveReader : IAssetArchiveReader
    {
        private readonly string ArchivePath;
        private readonly string EntryPath;
        private readonly byte[] Data;

        public FakeArchiveReader(string archivePath, string entryPath, byte[] data)
        {
            ArchivePath = archivePath;
            EntryPath = entryPath;
            Data = data;
        }

        public string? FailureMessage { get; set; }

        public bool CanRead(string archivePath)
        {
            return string.Equals(ArchivePath, archivePath, StringComparison.OrdinalIgnoreCase);
        }

        public IReadOnlyList<AssetArchiveEntry> ListEntries(string archivePath)
        {
            return
            [
                new AssetArchiveEntry
                {
                    ArchivePath = archivePath,
                    EntryPath = EntryPath,
                    PackedSize = Data.Length,
                    UnpackedSize = Data.Length
                }
            ];
        }

        public AssetArchiveReadResult TryReadEntry(string archivePath, string entryPath)
        {
            if (string.IsNullOrWhiteSpace(FailureMessage) &&
                CanRead(archivePath) &&
                string.Equals(EntryPath, entryPath, StringComparison.OrdinalIgnoreCase))
            {
                return new AssetArchiveReadResult
                {
                    IsSuccess = true,
                    ArchivePath = archivePath,
                    EntryPath = entryPath,
                    Data = Data,
                    StatusMessage = "Read fake archive entry."
                };
            }

            return new AssetArchiveReadResult
            {
                IsSuccess = false,
                ArchivePath = archivePath,
                EntryPath = entryPath,
                StatusMessage = FailureMessage ?? "Fake archive entry missing."
            };
        }
    }

    private static void WriteSingleCompressedBa2Archive(string archivePath, string entryName, byte[] unpackedData)
    {
        const int headerSize = 24;
        const int fileRecordSize = 36;
        var entryNameBytes = Encoding.UTF8.GetBytes(entryName);
        var nameTableOffset = headerSize + fileRecordSize;
        var dataOffset = nameTableOffset + sizeof(ushort) + entryNameBytes.Length;
        using var storedStream = new MemoryStream();
        using (var zlibStream = new ZLibStream(storedStream, CompressionMode.Compress, leaveOpen: true))
        {
            zlibStream.Write(unpackedData);
        }

        var storedData = storedStream.ToArray();
        using var stream = File.Create(archivePath);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        writer.Write(Encoding.ASCII.GetBytes("BTDX"));
        writer.Write(1U);
        writer.Write(Encoding.ASCII.GetBytes("GNRL"));
        writer.Write(1U);
        writer.Write((ulong)nameTableOffset);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write(0U);
        writer.Write((ulong)dataOffset);
        writer.Write((uint)storedData.Length);
        writer.Write((uint)unpackedData.Length);
        writer.Write(0xBAADF00DU);
        writer.Write((ushort)entryNameBytes.Length);
        writer.Write(entryNameBytes);
        writer.Write(storedData);
    }

    private static void WriteSingleBsaArchiveEntry(string archivePath, string folderName, string fileName, byte[] data)
    {
        const int headerSize = 36;
        const int folderRecordSize = 24;
        const int fileRecordSize = 16;
        var folderNameBytes = Encoding.UTF8.GetBytes(folderName);
        var fileNameBytes = Encoding.UTF8.GetBytes(fileName);
        var folderRecordOffset = headerSize;
        var folderBlockOffset = folderRecordOffset + folderRecordSize;
        var fileNameTableSize = fileNameBytes.Length + 1;
        var folderBlockSize = 1 + folderNameBytes.Length + 1 + fileRecordSize;
        var dataOffset = folderBlockOffset + folderBlockSize + fileNameTableSize;
        using var stream = File.Create(archivePath);
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: false);
        writer.Write(Encoding.ASCII.GetBytes("BSA\0"));
        writer.Write(105U);
        writer.Write((uint)folderRecordOffset);
        writer.Write(0x1U | 0x2U);
        writer.Write(1U);
        writer.Write(1U);
        writer.Write((uint)(folderNameBytes.Length + 2));
        writer.Write((uint)fileNameTableSize);
        writer.Write(0U);
        writer.Write(0UL);
        writer.Write(1U);
        writer.Write(0U);
        writer.Write((ulong)folderBlockOffset);
        writer.Write((byte)(folderNameBytes.Length + 1));
        writer.Write(folderNameBytes);
        writer.Write((byte)0);
        writer.Write(0UL);
        writer.Write(((ulong)(uint)dataOffset << 32) | (uint)data.Length);
        writer.Write(fileNameBytes);
        writer.Write((byte)0);
        writer.Write(data);
    }
}
