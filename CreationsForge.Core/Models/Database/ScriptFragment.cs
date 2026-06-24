using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("ScriptFragments")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, FragmentSlot, Fragment_Index", AutoIncrement = false)]
public class ScriptFragment
{
    public ScriptFragment()
    { }

    public ScriptFragment(ScriptFragmentDTO dto)
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
        FragmentSlot = dto.FragmentSlot;
        FragmentIndex = dto.FragmentIndex;
        MutagenObjectType = dto.MutagenObjectType;
        ScriptName = dto.ScriptName;
        FragmentName = dto.FragmentName;
        ExtraBindDataVersion = dto.ExtraBindDataVersion;
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

    [Column("FragmentSlot")] public string FragmentSlot { get; set; } = string.Empty;

    [Column("Fragment_Index")] public int FragmentIndex { get; set; }

    [Column("MutagenObjectType")] public string? MutagenObjectType { get; set; }

    [Column("ScriptName")] public string? ScriptName { get; set; }

    [Column("FragmentName")] public string? FragmentName { get; set; }

    [Column("ExtraBindDataVersion")] public int? ExtraBindDataVersion { get; set; }

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
