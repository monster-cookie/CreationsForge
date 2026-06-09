using CreationsForge.Bethesda.Assets.Archives;
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
            if (CanRead(archivePath) && string.Equals(EntryPath, entryPath, StringComparison.OrdinalIgnoreCase))
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
                StatusMessage = "Fake archive entry missing."
            };
        }
    }
}
