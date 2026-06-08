using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("PluginMasterReferences")]
[PrimaryKey("Game, Master_ModKey_Name, Master_ModKey_Type, Master_ModKey_FileName, Plugin_ModKey_Name, Plugin_ModKey_Type, Plugin_ModKey_FileName", AutoIncrement = false)]
public class PluginMasterReference
{
    public PluginMasterReference()
    { }

    public PluginMasterReference(PluginMasterReferenceDTO dto)
    {
        Game = dto.Game.ToString();
        MasterModKeyName = dto.MasterModKey.Name;
        MasterModKeyType = dto.MasterModKey.Type;
        MasterModKeyFileName = dto.MasterModKey.FileName;
        PluginModKeyName = dto.PluginModKey.Name;
        PluginModKeyType = dto.PluginModKey.Type;
        PluginModKeyFileName = dto.PluginModKey.FileName;
        ImportedAtUTC = dto.ImportedAtUTC;
    }

    [Column("Game")] public string Game { get; set; } = string.Empty;

    [Column("Master_ModKey_Name")] public string MasterModKeyName { get; set; } = string.Empty;

    [Column("Master_ModKey_Type")] public int MasterModKeyType { get; set; }

    [Column("Master_ModKey_FileName")] public string MasterModKeyFileName { get; set; } = string.Empty;

    [Column("Plugin_ModKey_Name")] public string PluginModKeyName { get; set; } = string.Empty;

    [Column("Plugin_ModKey_Type")] public int PluginModKeyType { get; set; }

    [Column("Plugin_ModKey_FileName")] public string PluginModKeyFileName { get; set; } = string.Empty;

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }

    public PluginMasterReferenceDTO ToDTO()
    {
        if (!Enum.TryParse<SupportedGame>(Game, out var supportedGame))
        {
            throw new ArgumentOutOfRangeException(nameof(Game), Game, "Invalid supported game value.");
        }

        return new PluginMasterReferenceDTO
        {
            Game = supportedGame,
            MasterModKey = new ModKeyDTO
            {
                Name = MasterModKeyName,
                Type = MasterModKeyType,
                FileName = MasterModKeyFileName
            },
            PluginModKey = new ModKeyDTO
            {
                Name = PluginModKeyName,
                Type = PluginModKeyType,
                FileName = PluginModKeyFileName
            },
            ImportedAtUTC = ImportedAtUTC
        };
    }
}
