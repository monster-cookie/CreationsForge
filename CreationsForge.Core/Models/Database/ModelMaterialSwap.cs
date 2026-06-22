using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("ModelMaterialSwaps")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ModelSlot, ModelGender, MaterialSwap_Index", AutoIncrement = false)]
public class ModelMaterialSwap
{
    public ModelMaterialSwap()
    { }

    public ModelMaterialSwap(ModelMaterialSwapDTO dto)
    {
        Game = dto.Game.ToString();
        ModKeyName = dto.ModKey.Name;
        ModKeyType = dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        RecordType = dto.RecordType;
        FormKeyModKeyName = dto.FormKey.ModKey.Name;
        FormKeyModKeyType = dto.FormKey.ModKey.Type;
        FormKeyModKeyFileName = dto.FormKey.ModKey.FileName;
        FormKeyId = dto.FormKey.Id;
        ModelSlot = dto.ModelSlot;
        ModelGender = dto.ModelGender;
        Name = dto.Name;
        MaterialSwapModKeyName = dto.MaterialSwapFormKey.ModKey.Name;
        MaterialSwapModKeyType = dto.MaterialSwapFormKey.ModKey.Type;
        MaterialSwapModKeyFileName = dto.MaterialSwapFormKey.ModKey.FileName;
        MaterialSwapFormKeyId = dto.MaterialSwapFormKey.Id;
        MaterialSwapIndex = dto.MaterialSwapIndex;
        ImportedAtUTC = dto.ImportedAtUTC;
    }

    [Column("Game")] public string Game { get; set; } = string.Empty;

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;

    [Column("ModKey_Type")] public int ModKeyType { get; set; }

    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;

    [Column("RecordType")] public string RecordType { get; set; } = string.Empty;

    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;

    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; }

    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;

    [Column("FormKey_ID")] public long FormKeyId { get; set; }

    [Column("ModelSlot")] public string ModelSlot { get; set; } = string.Empty;

    [Column("ModelGender")] public string ModelGender { get; set; } = string.Empty;

    [Column("Name")] public string? Name { get; set; }

    [Column("MaterialSwap_ModKey_Name")] public string MaterialSwapModKeyName { get; set; } = string.Empty;

    [Column("MaterialSwap_ModKey_Type")] public int MaterialSwapModKeyType { get; set; }

    [Column("MaterialSwap_ModKey_FileName")] public string MaterialSwapModKeyFileName { get; set; } = string.Empty;

    [Column("MaterialSwap_FormKey_ID")] public long MaterialSwapFormKeyId { get; set; }

    [Column("MaterialSwap_Index")] public int MaterialSwapIndex { get; set; }

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
