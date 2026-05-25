using System;
using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("Plugins")]
[PrimaryKey("ModKeyName, ModKeyFileName, ModKeyType", AutoIncrement = false)]
public class Plugin
{
    public Plugin()
    { }
    
    public Plugin(PluginDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyFileName = dto.ModKey.FileName;
        ModKeyType = (int)dto.ModKey.Type;
        GameRelease = dto.GameRelease;
        LoadOrderIndex = dto.LoadOrderIndex;
        PluginFileName = dto.PluginFileName;
        PluginPath = dto.PluginPath;
        Enabled = dto.Enabled;
        ExistsOnDisk = dto.ExistsOnDisk;
        ImportState = dto.ImportState;
        HeaderFlags = (int)dto.HeaderFlags;
        FormVersion = dto.FormVersion;
        Author = dto.Author;
    }
    
    [Column("ModKey_Name")]
    public string ModKeyName { get; set; } = string.Empty;
    
    [Column("ModKey_FileName")]
    public string ModKeyFileName { get; set; } = string.Empty;
    
    [Column("ModKey_Type")]
    public int ModKeyType { get; set; } = (int)ModType.Master;

    [Column("GameRelease")]
    public string GameRelease { get; set; } = string.Empty;

    [Column("LoadOrderIndex")]
    public int LoadOrderIndex { get; set; }

    [Column("PluginFileName")]
    public string PluginFileName { get; set; } = string.Empty;

    [Column("PluginPath")]
    public string? PluginPath { get; set; }

    [Column("Enabled")]
    public bool Enabled { get; set; } = true;

    [Column("ExistsOnDisk")]
    public bool ExistsOnDisk { get; set; } = true;

    [Column("ImportState")]
    public string ImportState { get; set; } = "Current";

    [Column("HeaderFlags")]
    public int HeaderFlags { get; set; }

    [Column("FormVersion")]
    public int FormVersion { get; set; }

    [Column("Author")]
    public string Author { get; set; } = string.Empty;

    [Column("Branch")]
    public string Branch { get; set; } = string.Empty;

    [Column("InteriorCellCount")] public int InteriorCellCount { get; set; } = 0;

    [Column("SourceLastWriteUtcTicks")]
    public long? SourceLastWriteUtcTicks { get; set; }

    [Column("SourceFileSizeBytes")]
    public long? SourceFileSizeBytes { get; set; }

    [Column("LastCheckedUtc")]
    public DateTime LastCheckedUtc { get; set; }

    [Column("LastImportedUtc")]
    public DateTime LastImportedUtc { get; set; }

    [Column("InvalidatedAtUtc")]
    public DateTime? InvalidatedAtUtc { get; set; }
}
