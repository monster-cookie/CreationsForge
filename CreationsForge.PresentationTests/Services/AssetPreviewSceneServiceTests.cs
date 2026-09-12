using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using Serilog;
using Shouldly;

namespace CreationsForge.PresentationTests.Services;

/// <summary>
/// Verifies the retained asset-preview scene service depends only on the standalone mesh-preview contract.
/// </summary>
public sealed class AssetPreviewSceneServiceTests
{
    /// <summary>
    /// Verifies a geometry reader receives the two-field preview candidate and can provide the returned model.
    /// </summary>
    [Fact]
    public void CreatePreview_WhenReaderAcceptsCandidate_ReturnsReaderModel()
    {
        var expectedModel = new AssetPreviewModelDTO
        {
            DisplayName = "Reader model",
            SourcePath = "Meshes/ReaderModel.nif"
        };
        var reader = new SuccessfulGeometryReader(expectedModel);
        var service = new AssetPreviewSceneService([reader], new LoggerConfiguration().CreateLogger());
        var candidate = new AssetPreviewCandidateDTO
        {
            MeshPath = "Meshes/Example.nif",
            DisplayName = "Example mesh"
        };

        var result = service.CreatePreview(candidate, out var statusMessage);

        result.ShouldBeSameAs(expectedModel);
        reader.CapturedCandidate.ShouldBeSameAs(candidate);
        statusMessage.ShouldBe("Geometry loaded");
    }

    /// <summary>
    /// Verifies the fallback scene preserves the candidate's display name and source path when no reader handles it.
    /// </summary>
    [Fact]
    public void CreatePreview_WhenNoReaderHandlesCandidate_PreservesCandidateIdentityInFallback()
    {
        var service = new AssetPreviewSceneService([], new LoggerConfiguration().CreateLogger());
        var candidate = new AssetPreviewCandidateDTO
        {
            MeshPath = "Meshes/ArchiveOnly.nif",
            DisplayName = "Archive-only mesh"
        };

        var result = service.CreatePreview(candidate, out var statusMessage);

        result.DisplayName.ShouldBe(candidate.DisplayName);
        result.SourcePath.ShouldBe(candidate.MeshPath);
        result.Meshes.ShouldHaveSingleItem();
        statusMessage.ShouldContain(candidate.MeshPath);
    }

    /// <summary>
    /// Supplies a deterministic preview model while capturing the candidate passed by the scene service.
    /// </summary>
    private sealed class SuccessfulGeometryReader : IAssetPreviewGeometryReader
    {
        /// <summary>
        /// Stores the preview model returned from <see cref="TryRead" />.
        /// </summary>
        private readonly AssetPreviewModelDTO PreviewModel;

        /// <summary>
        /// Initializes the reader with the preview model it should return.
        /// </summary>
        /// <param name="previewModel">The model returned for the captured candidate.</param>
        public SuccessfulGeometryReader(AssetPreviewModelDTO previewModel)
        {
            PreviewModel = previewModel;
        }

        /// <summary>
        /// Gets the candidate most recently supplied by the scene service, or <see langword="null" /> before a read.
        /// </summary>
        public AssetPreviewCandidateDTO? CapturedCandidate { get; private set; }

        /// <inheritdoc />
        public bool TryRead(
            AssetPreviewCandidateDTO candidate,
            out AssetPreviewModelDTO? previewModel,
            out string statusMessage)
        {
            CapturedCandidate = candidate;
            previewModel = PreviewModel;
            statusMessage = "Geometry loaded";
            return true;
        }
    }
}
