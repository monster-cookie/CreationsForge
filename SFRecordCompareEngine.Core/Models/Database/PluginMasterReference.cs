using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("PluginMasterReferences")]
[PrimaryKey("ModKeyName, ModKeyType, ModKeyFileName, ParentModKeyName, ParentModKeyType, ParentModKeyFileName", AutoIncrement = false)]
public class PluginMasterReference
{
    public PluginMasterReference()
    { }

    public PluginMasterReference(PluginMasterReferenceDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        ParentModKeyName = dto.ParentModKey.Name;
        ParentModKeyType = (int)dto.ParentModKey.Type;
        ParentModKeyFileName = dto.ParentModKey.FileName;
        MasterReferenceIndex = dto.MasterReferenceIndex;
        ParentLoadOrderIndex = dto.ParentLoadOrderIndex;
        ImportedAtUtc = dto.ImportedAtUtc;
    }

    [Column("ModKey_Name")]
    public string ModKeyName { get; set; } = string.Empty;

    [Column("ModKey_Type")]
    public int ModKeyType { get; set; } = (int)ModType.Master;

    [Column("ModKey_FileName")]
    public string ModKeyFileName { get; set; } = string.Empty;

    [Column("Parent_ModKey_Name")]
    public string ParentModKeyName { get; set; } = string.Empty;

    [Column("Parent_ModKey_Type")]
    public int ParentModKeyType { get; set; } = (int)ModType.Master;

    [Column("Parent_ModKey_FileName")]
    public string ParentModKeyFileName { get; set; } = string.Empty;

    [Column("MasterReferenceIndex")]
    public int MasterReferenceIndex { get; set; }

    [Column("ParentLoadOrderIndex")]
    public int ParentLoadOrderIndex { get; set; }

    [Column("ImportedAtUtc")]
    public DateTime ImportedAtUtc { get; set; }
}
