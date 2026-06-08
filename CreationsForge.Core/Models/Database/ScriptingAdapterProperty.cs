using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("ScriptingAdapterProperties")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, ScriptingAdapter_Name, Property_Index", AutoIncrement = false)]
public class ScriptingAdapterProperty
{
    public ScriptingAdapterProperty()
    { }

    public ScriptingAdapterProperty(ScriptingAdapterPropertyDTO dto)
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
        Name = dto.Name;
        MutagenObjectType = dto.MutagenObjectType;
        DataBool = dto.DataBool;
        DataInt = dto.DataInt;
        DataFloat = dto.DataFloat;
        DataString = dto.DataString;
        ObjectModKeyName = dto.ObjectFormKey?.ModKey.Name;
        ObjectModKeyType = dto.ObjectFormKey?.ModKey.Type;
        ObjectModKeyFileName = dto.ObjectFormKey?.ModKey.FileName;
        ObjectFormKeyId = dto.ObjectFormKey?.Id;
        ObjectAlias = dto.ObjectAlias;
        ObjectUnused = dto.ObjectUnused;
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

    [Column("Name")] public string Name { get; set; } = string.Empty;

    [Column("MutagenObjectType")] public string MutagenObjectType { get; set; } = string.Empty;

    [Column("Data_Bool")] public bool? DataBool { get; set; }

    [Column("Data_Int")] public int? DataInt { get; set; }

    [Column("Data_Float")] public double? DataFloat { get; set; }

    [Column("Data_String")] public string? DataString { get; set; }

    [Column("Object_ModKey_Name")] public string? ObjectModKeyName { get; set; }

    [Column("Object_ModKey_Type")] public int? ObjectModKeyType { get; set; }

    [Column("Object_ModKey_FileName")] public string? ObjectModKeyFileName { get; set; }

    [Column("Object_FormKey_ID")] public long? ObjectFormKeyId { get; set; }

    [Column("Object_Alias")] public short? ObjectAlias { get; set; }

    [Column("Object_Unused")] public int? ObjectUnused { get; set; }

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
