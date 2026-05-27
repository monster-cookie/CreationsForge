using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.Enums;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginDTO
{
    public PluginDTO()
    { }

    public PluginDTO(Plugin model)
    {
        ArgumentNullException.ThrowIfNull(model);
        if (!Enum.IsDefined(typeof(ModType), model.ModKeyType)) throw new ArgumentOutOfRangeException(nameof(model), $"model.ModKeyType is not a valid value, the value received was {model.ModKeyType}");
        
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        LoadOrderIndex = model.LoadOrderIndex;
        Enabled = model.Enabled;
        ExistsOnDisk = model.ExistsOnDisk;
        ImportState = model.ImportState;
        HeaderFlags = (StarfieldModHeader.HeaderFlag)model.HeaderFlags;
        FormVersion = model.FormVersion;
        Author = model.Author;
        Branch = model.Branch;
    }
    
    public ModKey ModKey { get; set; }
    public int LoadOrderIndex { get; set; }
    public bool Enabled { get; set; } = true;
    public bool ExistsOnDisk { get; set; } = true;
    public string ImportState { get; set; } = nameof(PluginImportState.Current);
    public StarfieldModHeader.HeaderFlag HeaderFlags { get; set; }
    public int FormVersion { get; set; }
    public string Author { get; set; } = "UNKNOWN";
    public string Branch { get; set; } = "UNKNOWN";
    public int InteriorCellCount { get; set; }
    public long SourceLastWriteUtcTicks { get; set; }
    public long SourceFileSizeBytes { get; set; }
    public DateTime LastCheckedUtc { get; set; }
    public DateTime? LastImportedUtc { get; set; }
    public DateTime? InvalidatedAtUtc { get; set; }
}
