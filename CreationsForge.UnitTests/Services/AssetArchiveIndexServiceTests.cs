using CreationsForge.Bethesda.Assets.Archives;
using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class AssetArchiveIndexServiceTests
{
    [Fact]
    public void IndexGameArchives_IndexesArchivesAndReportsProgress()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Starfield - Textures03.ba2");
            File.WriteAllBytes(archivePath, [1, 2, 3]);
            var repository = new TestAssetArchiveIndexRepository();
            var reader = new TestAssetArchiveReader(
                [
                    new AssetArchiveEntry
                    {
                        ArchivePath = archivePath,
                        EntryPath = "textures/cinimatics/digipic/digipick_material_color.dds",
                        PackedSize = 10,
                        UnpackedSize = 20
                    }
                ],
                [9]);
            var progressReports = new List<GameImportProgressDTO>();
            var service = new AssetArchiveIndexService(repository, [reader]);

            var result = service.IndexGameArchives(
                SupportedGame.Starfield,
                tempDirectory.FullName,
                new TestProgress<GameImportProgressDTO>(progressReports));

            result.ArchivesDiscovered.ShouldBe(1);
            result.ArchivesIndexed.ShouldBe(1);
            result.ArchivesSkippedCurrent.ShouldBe(0);
            result.ArchivesFailed.ShouldBe(0);
            result.EntriesIndexed.ShouldBe(1);
            repository.Entries.Count.ShouldBe(1);
            progressReports.Count.ShouldBe(1);
            progressReports[0].StatusText.ShouldBe("Indexing Starfield asset archives");
            progressReports[0].DetailText.ShouldBe("Starfield - Textures03.ba2");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void IndexGameArchives_IndexesLargeArchiveEntryListAndReportsInsertedCount()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Starfield - Meshes01.ba2");
            File.WriteAllBytes(archivePath, [1, 2, 3]);
            var entries = Enumerable.Range(0, 1200)
                .Select(index => new AssetArchiveEntry
                {
                    ArchivePath = archivePath,
                    EntryPath = $"meshes/generated/model{index}.nif",
                    PackedSize = index,
                    UnpackedSize = index + 1
                })
                .ToList();
            var repository = new TestAssetArchiveIndexRepository();
            var reader = new TestAssetArchiveReader(entries, [9]);
            var service = new AssetArchiveIndexService(repository, [reader]);

            var result = service.IndexGameArchives(SupportedGame.Starfield, tempDirectory.FullName);

            result.ArchivesIndexed.ShouldBe(1);
            result.EntriesIndexed.ShouldBe(1200);
            repository.Entries.Count.ShouldBe(1200);
            repository.LastReplaceEntryCount.ShouldBe(1200);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void IndexGameArchives_ReplacesOldArchiveEntries()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            File.WriteAllBytes(archivePath, [1, 2, 3]);
            var repository = new TestAssetArchiveIndexRepository();
            repository.ReplaceArchiveEntries(
                SupportedGame.Skyrim,
                archivePath,
                [
                    new AssetArchiveEntryDTO
                    {
                        Game = SupportedGame.Skyrim,
                        ArchivePath = archivePath,
                        NormalizedEntryPath = "meshes/old.nif",
                        RootFolder = "meshes",
                        Extension = ".nif",
                        PackedSize = 1,
                        UnpackedSize = 1
                    }
                ]);
            var reader = new TestAssetArchiveReader(
                [
                    new AssetArchiveEntry
                    {
                        ArchivePath = archivePath,
                        EntryPath = "meshes/new.nif",
                        PackedSize = 2,
                        UnpackedSize = 2
                    }
                ],
                [2]);
            var service = new AssetArchiveIndexService(repository, [reader]);

            var result = service.IndexGameArchives(SupportedGame.Skyrim, tempDirectory.FullName);

            result.EntriesIndexed.ShouldBe(1);
            repository.Entries.Count.ShouldBe(1);
            repository.Entries[0].NormalizedEntryPath.ShouldBe("meshes/new.nif");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void IndexGameArchives_SkipsCurrentArchivesWithoutListingEntries()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            File.WriteAllBytes(archivePath, [1, 2, 3]);
            var fileInfo = new FileInfo(archivePath);
            var repository = new TestAssetArchiveIndexRepository();
            repository.SaveArchiveFile(new AssetArchiveFileDTO
            {
                Game = SupportedGame.Skyrim,
                DataFolder = tempDirectory.FullName,
                ArchivePath = archivePath,
                ArchiveFileName = fileInfo.Name,
                ArchiveExtension = fileInfo.Extension,
                ArchiveType = "BSA",
                SourceLastWriteUTCTicks = fileInfo.LastWriteTimeUtc.Ticks,
                SourceFileSizeBytes = fileInfo.Length,
                IndexedAtUTC = DateTime.UtcNow
            });
            var reader = new TestAssetArchiveReader([], [1]);
            var service = new AssetArchiveIndexService(repository, [reader]);

            var result = service.IndexGameArchives(SupportedGame.Skyrim, tempDirectory.FullName);

            result.ArchivesDiscovered.ShouldBe(1);
            result.ArchivesIndexed.ShouldBe(0);
            result.ArchivesSkippedCurrent.ShouldBe(1);
            result.ArchivesFailed.ShouldBe(0);
            reader.ListEntriesCallCount.ShouldBe(0);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void IndexGameArchives_WhenCancelled_Throws()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            File.WriteAllBytes(Path.Combine(tempDirectory.FullName, "Fallout4 - Meshes.ba2"), [1]);
            using var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var service = new AssetArchiveIndexService(new TestAssetArchiveIndexRepository(), [new TestAssetArchiveReader([], [1])]);

            Should.Throw<OperationCanceledException>(() => service.IndexGameArchives(
                SupportedGame.Fallout4,
                tempDirectory.FullName,
                cancellationToken: cancellationTokenSource.Token));
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadArchiveAsset_IndexesArchiveAndReadsMatchingEntry()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Starfield - Textures03.ba2");
            File.WriteAllBytes(archivePath, [1, 2, 3]);
            var repository = new TestAssetArchiveIndexRepository();
            var reader = new TestAssetArchiveReader(
                [
                    new AssetArchiveEntry
                    {
                        ArchivePath = archivePath,
                        EntryPath = "textures/cinimatics/digipic/digipick_material_color.dds",
                        PackedSize = 10,
                        UnpackedSize = 20
                    }
                ],
                [9, 8, 7]);
            var service = new AssetArchiveIndexService(repository, [reader]);

            var result = service.TryReadArchiveAsset(
                SupportedGame.Starfield,
                tempDirectory.FullName,
                "Data\\Textures\\Cinimatics\\DigiPic\\DigiPick_Material_Color.DDS");

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            result.Data.ShouldBe([9, 8, 7]);
            result.SourceArchivePath.ShouldBe(archivePath);
            result.NormalizedEntryPath.ShouldBe("textures/cinimatics/digipic/digipick_material_color.dds");
            reader.ListEntriesCallCount.ShouldBe(1);
            repository.Entries.Count.ShouldBe(1);
            repository.Entries[0].RootFolder.ShouldBe("textures");
            repository.Entries[0].Extension.ShouldBe(".dds");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadArchiveAsset_ReusesCurrentIndexWithoutListingArchive()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Skyrim - Meshes.bsa");
            File.WriteAllBytes(archivePath, [1, 2, 3, 4]);
            var fileInfo = new FileInfo(archivePath);
            var repository = new TestAssetArchiveIndexRepository();
            repository.SaveArchiveFile(new AssetArchiveFileDTO
            {
                Game = SupportedGame.Skyrim,
                DataFolder = tempDirectory.FullName,
                ArchivePath = archivePath,
                ArchiveFileName = fileInfo.Name,
                ArchiveExtension = fileInfo.Extension,
                ArchiveType = "BSA",
                SourceLastWriteUTCTicks = fileInfo.LastWriteTimeUtc.Ticks,
                SourceFileSizeBytes = fileInfo.Length,
                IndexedAtUTC = DateTime.UtcNow
            });
            repository.ReplaceArchiveEntries(
                SupportedGame.Skyrim,
                archivePath,
                [
                    new AssetArchiveEntryDTO
                    {
                        Game = SupportedGame.Skyrim,
                        ArchivePath = archivePath,
                        NormalizedEntryPath = "meshes/clutter/basket04.nif",
                        RootFolder = "meshes",
                        Extension = ".nif",
                        PackedSize = 4,
                        UnpackedSize = 4
                    }
                ]);
            var reader = new TestAssetArchiveReader([], [4, 5, 6]);
            var service = new AssetArchiveIndexService(repository, [reader]);

            var result = service.TryReadArchiveAsset(SupportedGame.Skyrim, tempDirectory.FullName, "Meshes\\Clutter\\Basket04.NIF");

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            result.Data.ShouldBe([4, 5, 6]);
            reader.ListEntriesCallCount.ShouldBe(0);
            reader.TryReadEntryCallCount.ShouldBe(1);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadArchiveAsset_WhenDirectIndexEntryIsStale_FallsBackToReindexArchive()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Starfield - Meshes01.ba2");
            File.WriteAllBytes(archivePath, [1, 2, 3, 4]);
            var fileInfo = new FileInfo(archivePath);
            var repository = new TestAssetArchiveIndexRepository();
            repository.SaveArchiveFile(new AssetArchiveFileDTO
            {
                Game = SupportedGame.Starfield,
                DataFolder = tempDirectory.FullName,
                ArchivePath = archivePath,
                ArchiveFileName = fileInfo.Name,
                ArchiveExtension = fileInfo.Extension,
                ArchiveType = "BA2",
                SourceLastWriteUTCTicks = fileInfo.LastWriteTimeUtc.Ticks - 1,
                SourceFileSizeBytes = fileInfo.Length,
                IndexedAtUTC = DateTime.UtcNow
            });
            repository.ReplaceArchiveEntries(
                SupportedGame.Starfield,
                archivePath,
                [
                    new AssetArchiveEntryDTO
                    {
                        Game = SupportedGame.Starfield,
                        ArchivePath = archivePath,
                        NormalizedEntryPath = "geometries/preview.mesh",
                        RootFolder = "geometries",
                        Extension = ".mesh",
                        PackedSize = 4,
                        UnpackedSize = 4
                    }
                ]);
            var reader = new TestAssetArchiveReader(
                [
                    new AssetArchiveEntry
                    {
                        ArchivePath = archivePath,
                        EntryPath = "geometries/preview.mesh",
                        PackedSize = 4,
                        UnpackedSize = 4
                    }
                ],
                [7, 8, 9]);
            var service = new AssetArchiveIndexService(repository, [reader]);

            var result = service.TryReadArchiveAsset(SupportedGame.Starfield, tempDirectory.FullName, "geometries\\preview.mesh");

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            result.Data.ShouldBe([7, 8, 9]);
            reader.ListEntriesCallCount.ShouldBe(1);
            reader.TryReadEntryCallCount.ShouldBe(1);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void TryReadArchiveAsset_ReplacesStaleArchiveEntries()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archivePath = Path.Combine(tempDirectory.FullName, "Fallout4 - Meshes.ba2");
            File.WriteAllBytes(archivePath, [1, 2, 3, 4, 5]);
            var fileInfo = new FileInfo(archivePath);
            var repository = new TestAssetArchiveIndexRepository();
            repository.SaveArchiveFile(new AssetArchiveFileDTO
            {
                Game = SupportedGame.Fallout4,
                DataFolder = tempDirectory.FullName,
                ArchivePath = archivePath,
                ArchiveFileName = fileInfo.Name,
                ArchiveExtension = fileInfo.Extension,
                ArchiveType = "BA2",
                SourceLastWriteUTCTicks = fileInfo.LastWriteTimeUtc.Ticks - 1,
                SourceFileSizeBytes = fileInfo.Length,
                IndexedAtUTC = DateTime.UtcNow
            });
            repository.ReplaceArchiveEntries(
                SupportedGame.Fallout4,
                archivePath,
                [
                    new AssetArchiveEntryDTO
                    {
                        Game = SupportedGame.Fallout4,
                        ArchivePath = archivePath,
                        NormalizedEntryPath = "meshes/old.nif",
                        RootFolder = "meshes",
                        Extension = ".nif",
                        PackedSize = 1,
                        UnpackedSize = 1
                    }
                ]);
            var reader = new TestAssetArchiveReader(
                [
                    new AssetArchiveEntry
                    {
                        ArchivePath = archivePath,
                        EntryPath = "meshes/new.nif",
                        PackedSize = 5,
                        UnpackedSize = 5
                    }
                ],
                [5]);
            var service = new AssetArchiveIndexService(repository, [reader]);

            var result = service.TryReadArchiveAsset(SupportedGame.Fallout4, tempDirectory.FullName, "Meshes\\New.nif");

            result.Status.ShouldBe(BethesdaAssetReadStatus.ReadArchiveEntry);
            repository.Entries.Count.ShouldBe(1);
            repository.Entries[0].NormalizedEntryPath.ShouldBe("meshes/new.nif");
            reader.ListEntriesCallCount.ShouldBe(1);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    private class TestAssetArchiveReader : IAssetArchiveReader
    {
        private readonly IReadOnlyList<AssetArchiveEntry> Entries;
        private readonly byte[] Data;

        public TestAssetArchiveReader(IReadOnlyList<AssetArchiveEntry> entries, byte[] data)
        {
            Entries = entries;
            Data = data;
        }

        public int ListEntriesCallCount { get; private set; }

        public int TryReadEntryCallCount { get; private set; }

        public bool CanRead(string archivePath)
        {
            return true;
        }

        public IReadOnlyList<AssetArchiveEntry> ListEntries(string archivePath)
        {
            ListEntriesCallCount++;
            return Entries;
        }

        public AssetArchiveReadResult TryReadEntry(string archivePath, string entryPath)
        {
            TryReadEntryCallCount++;
            return new AssetArchiveReadResult
            {
                IsSuccess = true,
                ArchivePath = archivePath,
                EntryPath = entryPath.Replace('\\', '/').ToLowerInvariant(),
                Data = Data,
                StatusMessage = "Read test archive entry."
            };
        }
    }

    private class TestProgress<T> : IProgress<T>
    {
        private readonly IList<T> Reports;

        public TestProgress(IList<T> reports)
        {
            Reports = reports;
        }

        public void Report(T value)
        {
            Reports.Add(value);
        }
    }

    private class TestAssetArchiveIndexRepository : IAssetArchiveIndexRepository
    {
        private readonly List<AssetArchiveFileDTO> ArchiveFiles = new();

        public List<AssetArchiveEntryDTO> Entries { get; } = new();

        public long LastReplaceEntryCount { get; private set; }

        public AssetArchiveFileDTO? GetArchiveFile(SupportedGame game, string archivePath)
        {
            return ArchiveFiles.FirstOrDefault(archive =>
                archive.Game == game &&
                string.Equals(archive.ArchivePath, archivePath, StringComparison.OrdinalIgnoreCase));
        }

        public AssetArchiveEntryDTO? FindEntry(SupportedGame game, string archivePath, IReadOnlyList<string> normalizedEntryPaths)
        {
            return Entries.FirstOrDefault(entry =>
                entry.Game == game &&
                string.Equals(entry.ArchivePath, archivePath, StringComparison.OrdinalIgnoreCase) &&
                normalizedEntryPaths.Contains(entry.NormalizedEntryPath, StringComparer.OrdinalIgnoreCase));
        }

        public IReadOnlyList<AssetArchiveEntryDTO> FindEntries(SupportedGame game, string dataFolder, IReadOnlyList<string> normalizedEntryPaths)
        {
            var fullDataFolder = Path.GetFullPath(dataFolder);
            var archivePaths = ArchiveFiles
                .Where(archive => archive.Game == game && string.Equals(Path.GetFullPath(archive.DataFolder), fullDataFolder, StringComparison.OrdinalIgnoreCase))
                .Select(archive => archive.ArchivePath)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            return Entries
                .Where(entry =>
                    entry.Game == game &&
                    archivePaths.Contains(entry.ArchivePath) &&
                    normalizedEntryPaths.Contains(entry.NormalizedEntryPath, StringComparer.OrdinalIgnoreCase))
                .ToList();
        }

        public void SaveArchiveFile(AssetArchiveFileDTO archiveFile)
        {
            DeleteArchive(archiveFile.Game, archiveFile.ArchivePath);
            ArchiveFiles.Add(archiveFile);
        }

        public long ReplaceArchiveEntries(SupportedGame game, string archivePath, IEnumerable<AssetArchiveEntryDTO> entries)
        {
            Entries.RemoveAll(entry =>
                entry.Game == game &&
                string.Equals(entry.ArchivePath, archivePath, StringComparison.OrdinalIgnoreCase));
            var currentEntries = entries.ToList();
            Entries.AddRange(currentEntries);
            LastReplaceEntryCount = currentEntries.Count;
            return currentEntries.Count;
        }

        public void DeleteArchive(SupportedGame game, string archivePath)
        {
            ArchiveFiles.RemoveAll(archive =>
                archive.Game == game &&
                string.Equals(archive.ArchivePath, archivePath, StringComparison.OrdinalIgnoreCase));
            Entries.RemoveAll(entry =>
                entry.Game == game &&
                string.Equals(entry.ArchivePath, archivePath, StringComparison.OrdinalIgnoreCase));
        }
    }
}
