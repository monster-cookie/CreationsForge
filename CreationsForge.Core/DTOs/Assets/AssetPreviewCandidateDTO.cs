using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Assets;

public class AssetPreviewCandidateDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required string RecordType { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required string ModelSlot { get; set; }

    public string ModelGender { get; set; } = string.Empty;

    public required string MeshPath { get; set; }

    public required string DisplayName { get; set; }

    public required bool CanPreview { get; set; }

    public required bool CanOpenExternally { get; set; }

    public string? UnsupportedReason { get; set; }
}
