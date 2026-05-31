using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class StarfieldPluginMetadataDTO
{
    public ModKey ModKey { get; set; }
    public StarfieldModHeader.HeaderFlag HeaderFlags { get; set; }
    public int FormVersion { get; set; }
    public string Author { get; set; } = "Unknown";
    public int? InteriorCellCount { get; set; }
    public IReadOnlyList<ModKey> MasterReferences { get; set; } = new List<ModKey>();
}
