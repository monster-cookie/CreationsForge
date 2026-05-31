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
        if (!Enum.IsDefined(typeof(ModType), model.ModKeyType))
        {
            throw new ArgumentOutOfRangeException(nameof(model), $"model.ModKeyType is not a valid value, the value received was {model.ModKeyType}");
        }

        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        LoadOrderIndex = model.LoadOrderIndex;
        Enabled = model.Enabled;
        ExistsOnDisk = model.ExistsOnDisk;
        ImportState = model.ImportState;
        HeaderFlags = (StarfieldModHeader.HeaderFlag)model.HeaderFlags;
        FormVersion = model.FormVersion;
        Author = model.Author;
        Branch = model.Branch;
        InteriorCellCount = model.InteriorCellCount;
        SourceLastWriteUTCTicks = model.SourceLastWriteUTCTicks ?? 0;
        SourceFileSizeBytes = model.SourceFileSizeBytes ?? 0;
        LastCheckedUTC = model.LastCheckedUTC;
        LastImportedUTC = model.LastImportedUTC;
        InvalidatedAtUTC = model.InvalidatedAtUTC;
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
    public int? InteriorCellCount { get; set; }
    public long SourceLastWriteUTCTicks { get; set; }
    public long SourceFileSizeBytes { get; set; }
    public DateTime LastCheckedUTC { get; set; }
    public DateTime? LastImportedUTC { get; set; }
    public DateTime? InvalidatedAtUTC { get; set; }
}