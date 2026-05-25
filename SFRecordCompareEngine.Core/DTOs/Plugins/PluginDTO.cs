using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginDTO
{
    public PluginDTO()
    {}

    public PluginDTO(Plugin model)
    {
        if (!Enum.IsDefined(typeof(ModType), model.ModKeyType))
        {
            throw new ArgumentOutOfRangeException(nameof(model.ModKeyType), model.ModKeyType, "Invalid mod type value.");
        }
        
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        GameRelease = model.GameRelease;
        LoadOrderIndex = model.LoadOrderIndex;
        PluginFileName = model.PluginFileName;
        PluginPath = model.PluginPath;
        Enabled = model.Enabled;
        ExistsOnDisk = model.ExistsOnDisk;
        ImportState = model.ImportState;
        HeaderFlags = (StarfieldModHeader.HeaderFlag)model.HeaderFlags;
        FormVersion = model.FormVersion;
        Author = model.Author;
    }
    
    public ModKey ModKey { get; set; }
    public string GameRelease { get; set; }
    public int LoadOrderIndex { get; set; }
    public string PluginFileName { get; set; }
    public string PluginPath { get; set; }
    public bool Enabled { get; set; } = true;
    public bool ExistsOnDisk { get; set; } = true;
    public string ImportState { get; set; } = nameof(PluginImportState.Current);
    public StarfieldModHeader.HeaderFlag HeaderFlags { get; set; }
    public int FormVersion { get; set; }
    public string Author { get; set; }
    public string Branch { get; set; }
    public int InteriorCellCount { get; set; }
    public long SourceLastWriteUtcTicks { get; set; }
    public long SourceFileSizeBytes { get; set; }
    public DateTime LastCheckedUtc { get; set; }
    public DateTime? LastImportedUtc { get; set; }
    public DateTime? InvalidatedAtUtc { get; set; }
}
