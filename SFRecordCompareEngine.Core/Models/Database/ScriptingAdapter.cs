using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("ScriptingAdapters")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ID, Name", AutoIncrement = false)]
public class ScriptingAdapter
{
    public ScriptingAdapter()
    { }

    public ScriptingAdapter(ScriptingAdapterDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        RecordType = dto.RecordType;
        FormKeyId = (int)dto.FormKey.ID;
        Name = dto.Name;
        ScriptIndex = dto.ScriptIndex;
        ImportedAtUTC = dto.ImportedAtUTC;
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("RecordType")] public string RecordType { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("Name")] public string Name { get; set; } = string.Empty;
    [Column("Script_Index")] public int ScriptIndex { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
