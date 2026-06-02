using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("Plugins")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName", AutoIncrement = false)]
public class Plugin
{
    public Plugin()
    { }

    public Plugin(PluginDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyFileName = dto.ModKey.FileName;
        ModKeyType = (int)dto.ModKey.Type;
        LoadOrderIndex = dto.LoadOrderIndex;
        Enabled = dto.Enabled;
        ExistsOnDisk = dto.ExistsOnDisk;
        ImportState = dto.ImportState;
        HeaderFlags = (int)dto.HeaderFlags;
        FormVersion = dto.FormVersion;
        Author = dto.Author;
        Branch = dto.Branch;
        InteriorCellCount = dto.InteriorCellCount ?? 0;
        RecordCount = dto.RecordCount;
        SourceLastWriteUTCTicks = dto.SourceLastWriteUTCTicks;
        SourceFileSizeBytes = dto.SourceFileSizeBytes;
        LastCheckedUTC = dto.LastCheckedUTC;
        LastImportedUTC = dto.LastImportedUTC;
        InvalidatedAtUTC = dto.InvalidatedAtUTC;
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;

    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;

    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;

    [Column("LoadOrderIndex")] public int LoadOrderIndex { get; set; }

    [Column("Enabled")] public bool Enabled { get; set; } = true;

    [Column("ExistsOnDisk")] public bool ExistsOnDisk { get; set; } = true;

    [Column("ImportState")] public string ImportState { get; set; } = "Current";

    [Column("HeaderFlags")] public int HeaderFlags { get; set; }

    [Column("FormVersion")] public int FormVersion { get; set; }

    [Column("Author")] public string Author { get; set; } = "UNKNOWN";

    [Column("Branch")] public string Branch { get; set; } = "UNKNOWN";

    [Column("InteriorCellCount")] public int InteriorCellCount { get; set; }

    [Column("RecordCount")] public long RecordCount { get; set; }

    [Column("SourceLastWriteUTCTicks")] public long? SourceLastWriteUTCTicks { get; set; }

    [Column("SourceFileSizeBytes")] public long? SourceFileSizeBytes { get; set; }

    [Column("LastCheckedUTC")] public DateTime LastCheckedUTC { get; set; }

    [Column("LastImportedUTC")] public DateTime? LastImportedUTC { get; set; }

    [Column("InvalidatedAtUTC")] public DateTime? InvalidatedAtUTC { get; set; }
}
