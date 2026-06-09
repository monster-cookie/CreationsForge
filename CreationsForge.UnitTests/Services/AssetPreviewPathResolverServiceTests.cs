using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using Moq;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class AssetPreviewPathResolverServiceTests
{
    [Fact]
    public void GetPreviewCandidates_ReturnsCandidatesForModelFiles()
    {
        var formKey = CreateFormKey();
        var repository = new Mock<IModelRepository>();
        repository.Setup(repo => repo.GetByFormKey(SupportedGame.Starfield, "MISC", formKey))
            .Returns([
                CreateModel(formKey, "Meshes\\Props\\Preview.nif", "Model", string.Empty),
                CreateModel(formKey, "Meshes/Props/Preview.glb", "World", "Male")
            ]);
        var service = new AssetPreviewPathResolverService(repository.Object);

        var candidates = service.GetPreviewCandidates(SupportedGame.Starfield, "MISC", formKey);

        candidates.Count.ShouldBe(2);
        candidates[0].MeshPath.ShouldBe($"Meshes{Path.DirectorySeparatorChar}Props{Path.DirectorySeparatorChar}Preview.nif");
        candidates[0].DisplayName.ShouldBe("Model: Preview.nif");
        candidates[0].CanPreview.ShouldBeTrue();
        candidates[0].CanOpenExternally.ShouldBeTrue();
        candidates[0].UnsupportedReason.ShouldBeNull();
        candidates[1].DisplayName.ShouldBe("World (Male): Preview.glb");
    }

    [Fact]
    public void GetPreviewCandidates_IgnoresModelsWithoutFilePaths()
    {
        var formKey = CreateFormKey();
        var repository = new Mock<IModelRepository>();
        repository.Setup(repo => repo.GetByFormKey(SupportedGame.Starfield, "MISC", formKey))
            .Returns([
                CreateModel(formKey, null, "Model", string.Empty),
                CreateModel(formKey, "   ", "World", string.Empty)
            ]);
        var service = new AssetPreviewPathResolverService(repository.Object);

        var candidates = service.GetPreviewCandidates(SupportedGame.Starfield, "MISC", formKey);

        candidates.ShouldBeEmpty();
    }

    [Theory]
    [InlineData("Meshes/Example.nif", true)]
    [InlineData("Meshes/Example.obj", true)]
    [InlineData("Meshes/Example.glb", true)]
    [InlineData("Meshes/Example.txt", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void CanPreviewPath_DetectsSupportedPreviewExtensions(string? meshPath, bool expected)
    {
        var service = new AssetPreviewPathResolverService(Mock.Of<IModelRepository>());

        service.CanPreviewPath(meshPath).ShouldBe(expected);
    }

    [Fact]
    public void GetPreviewCandidates_MarksUnsupportedExtensions()
    {
        var formKey = CreateFormKey();
        var repository = new Mock<IModelRepository>();
        repository.Setup(repo => repo.GetByFormKey(SupportedGame.Starfield, "MISC", formKey))
            .Returns([CreateModel(formKey, "Meshes/Props/readme.txt", "Model", string.Empty)]);
        var service = new AssetPreviewPathResolverService(repository.Object);

        var candidate = service.GetPreviewCandidates(SupportedGame.Starfield, "MISC", formKey).Single();

        candidate.CanPreview.ShouldBeFalse();
        candidate.CanOpenExternally.ShouldBeFalse();
        candidate.UnsupportedReason.ShouldNotBeNullOrWhiteSpace();
    }

    private static ModelDTO CreateModel(FormKeyDTO formKey, string? file, string modelSlot, string modelGender)
    {
        return new ModelDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = new ModKeyDTO
            {
                Name = "Starfield",
                Type = 0,
                FileName = "Starfield.esm"
            },
            RecordType = "MISC",
            FormKey = formKey,
            ModelSlot = modelSlot,
            ModelGender = modelGender,
            File = file,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static FormKeyDTO CreateFormKey()
    {
        return new FormKeyDTO
        {
            ModKey = new ModKeyDTO
            {
                Name = "Starfield",
                Type = 0,
                FileName = "Starfield.esm"
            },
            Id = 0x123456
        };
    }
}
