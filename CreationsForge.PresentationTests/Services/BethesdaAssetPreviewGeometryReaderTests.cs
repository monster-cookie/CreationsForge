using CreationsForge.Bethesda.Assets.Files;
using CreationsForge.Bethesda.Assets.Nif;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Services;

public class BethesdaAssetPreviewGeometryReaderTests
{
    [Fact]
    public void TryRead_ReportsTexturePathsSeparatelyWhenTextureBytesDoNotLoad()
    {
        var assetFileResolver = new FakeAssetFileResolverService();
        assetFileResolver.Resolutions[@"Meshes\Preview.nif"] = CreateResolution(@"Meshes\Preview.nif", AssetFileResolutionStatus.ResolvedLooseFile, [1]);
        assetFileResolver.Resolutions[@"textures\Preview\Preview_color.dds"] = CreateResolution(@"textures\Preview\Preview_color.dds", AssetFileResolutionStatus.MissingLooseFile, null);
        var nifReader = new FakeNifPreviewModelReader
        {
            Result = CreateNifResult(texturePath: @"textures\Preview\Preview_color.dds")
        };
        var reader = CreateReader(assetFileResolver, nifReader);

        var read = reader.TryRead(CreateCandidate(), out var previewModel, out var statusMessage);

        read.ShouldBeTrue();
        previewModel.ShouldNotBeNull();
        statusMessage.ShouldContain("Texture paths were found, but preview texture bytes were not loaded.");
        statusMessage.ShouldNotContain("Textures were found.");
        statusMessage.ShouldNotContain("Loaded 1 preview texture");
    }

    [Fact]
    public void TryRead_ReportsLoadedTextureOnlyWhenTextureBytesLoad()
    {
        var assetFileResolver = new FakeAssetFileResolverService();
        assetFileResolver.Resolutions[@"Meshes\Preview.nif"] = CreateResolution(@"Meshes\Preview.nif", AssetFileResolutionStatus.ResolvedLooseFile, [1]);
        assetFileResolver.Resolutions[@"textures\Preview\Preview_color.dds"] = CreateResolution(@"textures\Preview\Preview_color.dds", AssetFileResolutionStatus.ResolvedArchiveEntryInMemory, [2, 3, 4]);
        var nifReader = new FakeNifPreviewModelReader
        {
            Result = CreateNifResult(texturePath: @"textures\Preview\Preview_color.dds")
        };
        var reader = CreateReader(assetFileResolver, nifReader);

        var read = reader.TryRead(CreateCandidate(), out var previewModel, out var statusMessage);

        read.ShouldBeTrue();
        previewModel.ShouldNotBeNull();
        previewModel.Meshes[0].Texture.ShouldNotBeNull();
        statusMessage.ShouldContain("Loaded 1 preview texture(s).");
        statusMessage.ShouldNotContain("Textures were found.");
        statusMessage.ShouldNotContain("Texture paths were found");
    }

    [Fact]
    public void TryRead_ReturnsNonFallbackModelWhenAssetIsMissing()
    {
        var assetFileResolver = new FakeAssetFileResolverService();
        assetFileResolver.Resolutions[@"Meshes\Preview.nif"] = CreateResolution(@"Meshes\Preview.nif", AssetFileResolutionStatus.MissingLooseFile, null);
        var nifReader = new FakeNifPreviewModelReader
        {
            Result = CreateNifResult(texturePath: string.Empty)
        };
        var reader = CreateReader(assetFileResolver, nifReader);

        var read = reader.TryRead(CreateCandidate(), out var previewModel, out var statusMessage);

        read.ShouldBeTrue();
        previewModel.ShouldNotBeNull();
        previewModel.Meshes.ShouldBeEmpty();
        previewModel.AllowFallbackRender.ShouldBeFalse();
        statusMessage.ShouldBe(@"Could not read Meshes\Preview.nif.");
    }

    private static BethesdaAssetPreviewGeometryReader CreateReader(
        IAssetFileResolverService assetFileResolver,
        INifPreviewModelReader nifReader)
    {
        return new BethesdaAssetPreviewGeometryReader(
            assetFileResolver,
            nifReader,
            new LoggerConfiguration().CreateLogger());
    }

    private static AssetPreviewCandidateDTO CreateCandidate()
    {
        return new AssetPreviewCandidateDTO
        {
            Game = SupportedGame.Fallout4,
            ModKey = new ModKeyDTO
            {
                Name = "Fallout4",
                Type = 0,
                FileName = "Fallout4.esm"
            },
            RecordType = "MISC",
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = "Fallout4",
                    Type = 0,
                    FileName = "Fallout4.esm"
                },
                Id = 0x123
            },
            ModelSlot = "Model",
            MeshPath = @"Meshes\Preview.nif",
            DisplayName = "Preview",
            CanPreview = true,
            CanOpenExternally = true
        };
    }

    private static AssetFileResolutionDTO CreateResolution(string path, AssetFileResolutionStatus status, byte[]? data)
    {
        return new AssetFileResolutionDTO
        {
            OriginalPath = path,
            ResolvedPath = status == AssetFileResolutionStatus.ResolvedLooseFile ? path : null,
            NormalizedEntryPath = status == AssetFileResolutionStatus.ResolvedArchiveEntryInMemory ? path : null,
            Data = data,
            Status = status,
            StatusMessage = status is AssetFileResolutionStatus.ResolvedLooseFile or AssetFileResolutionStatus.ResolvedArchiveEntryInMemory
                ? $"Read {path}."
                : $"Could not read {path}."
        };
    }

    private static NifPreviewReadResult CreateNifResult(string texturePath)
    {
        return new NifPreviewReadResult
        {
            IsSuccess = true,
            StatusMessage = "Loaded 1 preview mesh(es).",
            Model = new NifPreviewModel
            {
                DisplayName = "Preview",
                SourcePath = @"Meshes\Preview.nif",
                Meshes =
                {
                    new NifPreviewMesh
                    {
                        Name = "Mesh 1",
                        MaterialName = "Material",
                        TexturePath = texturePath
                    }
                }
            }
        };
    }

    private class FakeAssetFileResolverService : IAssetFileResolverService
    {
        public Dictionary<string, AssetFileResolutionDTO> Resolutions { get; } = new(StringComparer.OrdinalIgnoreCase);

        public AssetFileResolutionDTO ResolveAssetFile(AssetPreviewCandidateDTO candidate)
        {
            return Resolutions.TryGetValue(candidate.MeshPath, out var resolution)
                ? resolution
                : CreateResolution(candidate.MeshPath, AssetFileResolutionStatus.MissingLooseFile, null);
        }
    }

    private class FakeNifPreviewModelReader : INifPreviewModelReader
    {
        public required NifPreviewReadResult Result { get; set; }

        public NifPreviewReadResult TryRead(NifPreviewReadRequest request)
        {
            return Result;
        }
    }
}
