using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("Models")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ModelSlot, ModelGender", AutoIncrement = false)]
public class Model
{
    public Model()
    { }

    public Model(ModelDTO dto)
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
        File = dto.File;
        TextureFileHashes = dto.TextureFileHashes;
        LightLayer = dto.LightLayer;
        Flags = dto.Flags;
        ColorRemappingIndex = dto.ColorRemappingIndex;
        FlagsVestigial = dto.FlagsVestigial;
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

    [Column("File")] public string? File { get; set; }

    [Column("TextureFileHashes")] public string? TextureFileHashes { get; set; }

    [Column("LightLayer")] public long? LightLayer { get; set; }

    [Column("Flags")] public string? Flags { get; set; }

    [Column("ColorRemappingIndex")] public float? ColorRemappingIndex { get; set; }

    [Column("FlagsVestigial")] public string? FlagsVestigial { get; set; }

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
