using CreationsForge.Bethesda.Assets.Files;
using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using Moq;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class AssetFileResolverServiceTests
{
    [Fact]
    public void ResolveAssetFile_ReturnsAbsoluteLooseFile()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var filePath = Path.Combine(tempDirectory.FullName, "Preview.nif");
            File.WriteAllText(filePath, "nif");
            var service = new AssetFileResolverService([], new BethesdaAssetProvider([]), new TestAssetArchiveIndexService());

            var result = service.ResolveAssetFile(CreateCandidate(filePath));

            result.Status.ShouldBe(AssetFileResolutionStatus.ResolvedLooseFile);
            result.ResolvedPath.ShouldBe(filePath);
            result.IsResolved.ShouldBeTrue();
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void ResolveAssetFile_TriesMeshesPrefixForRelativePaths()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var meshDirectory = Path.Combine(tempDirectory.FullName, "Meshes", "SetDressing", "BabyBottle");
            Directory.CreateDirectory(meshDirectory);
            var filePath = Path.Combine(meshDirectory, "BabyBottleDirty02.nif");
            File.WriteAllText(filePath, "nif");
            var service = new AssetFileResolverService(
                [CreateGameMetadataService(SupportedGame.Fallout4, tempDirectory.FullName)],
                new BethesdaAssetProvider([]),
                new TestAssetArchiveIndexService());

            var result = service.ResolveAssetFile(CreateCandidate("SetDressing\\BabyBottle\\BabyBottleDirty02.nif", SupportedGame.Fallout4));

            result.Status.ShouldBe(AssetFileResolutionStatus.ResolvedLooseFile);
            result.ResolvedPath.ShouldBe(filePath);
            result.SearchedPaths.ShouldContain(Path.Combine(tempDirectory.FullName, "SetDressing", "BabyBottle", "BabyBottleDirty02.nif"));
            result.SearchedPaths.ShouldContain(filePath);
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void ResolveAssetFile_ReturnsIndexedArchiveMissingWhenArchivesExist()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            File.WriteAllText(Path.Combine(tempDirectory.FullName, "Fallout4 - Meshes.ba2"), "archive");
            var service = new AssetFileResolverService(
                [CreateGameMetadataService(SupportedGame.Fallout4, tempDirectory.FullName)],
                new BethesdaAssetProvider([]),
                new TestAssetArchiveIndexService());

            var result = service.ResolveAssetFile(CreateCandidate("SetDressing\\BabyBottle\\BabyBottleDirty02.nif", SupportedGame.Fallout4));

            result.Status.ShouldBe(AssetFileResolutionStatus.ArchiveExtractionUnsupported);
            result.IsResolved.ShouldBeFalse();
            result.StatusMessage.ShouldContain("Missing indexed asset");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void ResolveAssetFile_ReturnsMissingDataFolderWhenMetadataIsUnavailable()
    {
        var service = new AssetFileResolverService([], new BethesdaAssetProvider([]), new TestAssetArchiveIndexService());

        var result = service.ResolveAssetFile(CreateCandidate("Meshes\\Props\\Preview.nif"));

        result.Status.ShouldBe(AssetFileResolutionStatus.MissingDataFolder);
        result.IsResolved.ShouldBeFalse();
    }

    [Fact]
    public void ResolveAssetFile_UsesArchiveIndexWhenLooseFileIsMissing()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var archiveIndexService = new TestAssetArchiveIndexService
            {
                Result = new BethesdaAssetReadResult
                {
                    OriginalPath = "Meshes\\Props\\Preview.nif",
                    DataFolder = tempDirectory.FullName,
                    SourceType = BethesdaAssetSourceType.Archive,
                    Status = BethesdaAssetReadStatus.ReadArchiveEntry,
                    Data = [1, 2, 3],
                    SourceArchivePath = Path.Combine(tempDirectory.FullName, "Starfield - Meshes.ba2"),
                    NormalizedEntryPath = "meshes/props/preview.nif",
                    StatusMessage = "Read indexed archive asset."
                }
            };
            var service = new AssetFileResolverService(
                [CreateGameMetadataService(SupportedGame.Starfield, tempDirectory.FullName)],
                new BethesdaAssetProvider([]),
                archiveIndexService);

            var result = service.ResolveAssetFile(CreateCandidate("Meshes\\Props\\Preview.nif"));

            result.Status.ShouldBe(AssetFileResolutionStatus.ResolvedArchiveEntryInMemory);
            result.Data.ShouldBe([1, 2, 3]);
            result.SourceArchivePath.ShouldBe(Path.Combine(tempDirectory.FullName, "Starfield - Meshes.ba2"));
            archiveIndexService.RequestedAssetPath.ShouldBe("Meshes\\Props\\Preview.nif");
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void ResolveAssetFile_MapsAssetTooLargeStatus()
    {
        var service = new AssetFileResolverService([], new BethesdaAssetProvider([]), new TestAssetArchiveIndexService());

        var result = service.ResolveAssetFile(CreateCandidate(CreateOversizedLooseFile()));

        result.Status.ShouldBe(AssetFileResolutionStatus.AssetTooLarge);
        result.IsResolved.ShouldBeFalse();
        File.Delete(result.OriginalPath);
    }

    private static IGameMetadataService CreateGameMetadataService(SupportedGame game, string dataFolder)
    {
        var metadataService = new Mock<IGameMetadataService>();
        metadataService.SetupGet(service => service.Game).Returns(game);
        metadataService.Setup(service => service.GetGame())
            .Returns(new GameDTO
            {
                Game = game,
                DisplayName = game.ToString(),
                DataFolder = dataFolder,
                ImportedAtUTC = DateTime.UtcNow
            });
        return metadataService.Object;
    }

    private static AssetPreviewCandidateDTO CreateCandidate(string meshPath, SupportedGame game = SupportedGame.Starfield)
    {
        return new AssetPreviewCandidateDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = "Plugin",
                Type = 0,
                FileName = "Plugin.esm"
            },
            RecordType = "MISC",
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = "Plugin",
                    Type = 0,
                    FileName = "Plugin.esm"
                },
                Id = 0x123456
            },
            ModelSlot = "Model",
            MeshPath = meshPath,
            DisplayName = Path.GetFileName(meshPath),
            CanPreview = true,
            CanOpenExternally = true
        };
    }

    private static string CreateOversizedLooseFile()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.nif");
        using var stream = File.Create(filePath);
        stream.SetLength(BethesdaAssetProvider.MaximumPreviewAssetBytes + 1L);
        return filePath;
    }

    private class TestAssetArchiveIndexService : IAssetArchiveIndexService
    {
        public BethesdaAssetReadResult? Result { get; set; }

        public string? RequestedAssetPath { get; private set; }

        public AssetArchiveIndexResultDTO IndexGameArchives(
            SupportedGame game,
            string? dataFolder,
            IProgress<GameImportProgressDTO>? progress = null,
            CancellationToken cancellationToken = default)
        {
            return new AssetArchiveIndexResultDTO();
        }

        public BethesdaAssetReadResult TryReadArchiveAsset(SupportedGame game, string dataFolder, string assetPath)
        {
            RequestedAssetPath = assetPath;
            return Result ?? new BethesdaAssetReadResult
            {
                OriginalPath = assetPath,
                DataFolder = dataFolder,
                SourceType = BethesdaAssetSourceType.Archive,
                Status = BethesdaAssetReadStatus.ArchiveEntryMissing,
                StatusMessage = "Missing indexed asset."
            };
        }
    }
}
