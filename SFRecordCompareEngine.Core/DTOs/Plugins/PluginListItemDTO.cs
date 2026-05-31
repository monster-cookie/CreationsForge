using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Enums;

namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginListItemDTO
{
    public required ModKey ModKey { get; set; }

    public string ImportState { get; set; } = nameof(PluginImportState.Current);

    public bool IsFailed => string.Equals(ImportState, nameof(PluginImportState.Failed), StringComparison.OrdinalIgnoreCase);
}