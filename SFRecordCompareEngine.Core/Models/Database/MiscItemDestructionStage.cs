using Mutagen.Bethesda.Plugins;
using NPoco;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("MiscItemDestructionStages")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Stage_Index", AutoIncrement = false)]
public class MiscItemDestructionStage
{
    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; } = (int)ModType.Master;
    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("Stage_Index")] public int StageIndex { get; set; }
    [Column("HealthPercent")] public int? HealthPercent { get; set; }
    [Column("SourceIndex")] public int? SourceIndex { get; set; }
    [Column("ModelDamageStage")] public int? ModelDamageStage { get; set; }
    [Column("Flags")] public string? Flags { get; set; }
    [Column("SelfDamagePerSecond")] public int? SelfDamagePerSecond { get; set; }
    [Column("Explosion_FormKey")] public string? ExplosionFormKey { get; set; }
    [Column("Debris_FormKey")] public string? DebrisFormKey { get; set; }
    [Column("DebrisCount")] public int? DebrisCount { get; set; }
    [Column("SequenceName")] public string? SequenceName { get; set; }
    [Column("Model_File")] public string? ModelFile { get; set; }
    [Column("Model_LightLayer")] public long? ModelLightLayer { get; set; }
    [Column("Model_Flags")] public string? ModelFlags { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
