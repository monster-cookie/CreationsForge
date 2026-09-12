namespace CreationsForge.Core.DTOs.Assets;

/// <summary>
/// Identifies a mesh that can be rendered by the retained asset-preview pipeline.
/// </summary>
public class AssetPreviewCandidateDTO
{
    /// <summary>
    /// Gets or sets the mesh path supplied to preview geometry readers.
    /// </summary>
    public required string MeshPath { get; set; }

    /// <summary>
    /// Gets or sets the human-readable name shown for the preview.
    /// </summary>
    public required string DisplayName { get; set; }
}
