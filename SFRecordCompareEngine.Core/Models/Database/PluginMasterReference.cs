using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("PluginMasterReferences")]
[PrimaryKey("Master_ModKey_Name, Master_ModKey_Type, Master_ModKey_FileName, Plugin_ModKey_Name, Plugin_ModKey_Type, Plugin_ModKey_FileName", AutoIncrement = false)]
public class PluginMasterReference
{
    public PluginMasterReference()
    { }

    public PluginMasterReference(PluginMasterReferenceDTO dto)
    {
        MasterModKeyName = dto.MasterModKey.Name;
        MasterModKeyType = (int)dto.MasterModKey.Type;
        MasterModKeyFileName = dto.MasterModKey.FileName;
        PluginModKeyName = dto.PluginModKey.Name;
        PluginModKeyType = (int)dto.PluginModKey.Type;
        PluginModKeyFileName = dto.PluginModKey.FileName;
        ImportedAtUTC = dto.ImportedAtUTC;
    }

    [Column("Master_ModKey_Name")] public string MasterModKeyName { get; set; } = string.Empty;

    [Column("Master_ModKey_Type")] public int MasterModKeyType { get; set; } = (int)ModType.Master;

    [Column("Master_ModKey_FileName")] public string MasterModKeyFileName { get; set; } = string.Empty;

    [Column("Plugin_ModKey_Name")] public string PluginModKeyName { get; set; } = string.Empty;

    [Column("Plugin_ModKey_Type")] public int PluginModKeyType { get; set; } = (int)ModType.Master;

    [Column("Plugin_ModKey_FileName")] public string PluginModKeyFileName { get; set; } = string.Empty;

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}