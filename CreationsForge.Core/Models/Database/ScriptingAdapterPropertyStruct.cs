using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

/// <summary>
/// Database row for one VMAD script property struct-list entry.
/// </summary>
[TableName("ScriptingAdapterPropertyStructs")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name, Property_Index, Struct_Index", AutoIncrement = false)]
public class ScriptingAdapterPropertyStruct
{
    /// <summary>
    /// Initializes a blank row for NPoco materialization.
    /// </summary>
    public ScriptingAdapterPropertyStruct()
    { }

    /// <summary>
    /// Initializes a row from a VMAD script property struct DTO.
    /// </summary>
    /// <param name="dto">The DTO to persist.</param>
    public ScriptingAdapterPropertyStruct(ScriptingAdapterPropertyStructDTO dto)
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
        ScriptingAdapterName = dto.ScriptingAdapterName;
        PropertyIndex = dto.PropertyIndex;
        StructIndex = dto.StructIndex;
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

    [Column("ScriptingAdapter_Name")] public string ScriptingAdapterName { get; set; } = string.Empty;

    [Column("Property_Index")] public int PropertyIndex { get; set; }

    [Column("Struct_Index")] public int StructIndex { get; set; }

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
