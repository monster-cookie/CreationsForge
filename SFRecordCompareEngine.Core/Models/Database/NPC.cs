using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("NPC")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class NPC
{
    public NPC()
    { }

    public NPC(NPCDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyId = (int)dto.FormKey.ID;
        EditorId = dto.EditorID;
        FormVersion = dto.FormVersion;
        StarfieldMajorRecordFlags = (int)dto.StarfieldMajorRecordFlags;
        Version2 = dto.Version2;
        VersionControl = dto.VersionControl;
        ImportedAtUTC = dto.ImportedAtUTC;
        Name = dto.Name;
        ShortName = dto.ShortName;
        LongName = dto.LongName;
        DispositionBase = dto.DispositionBase;
        Aggression = dto.Aggression;
        Confidence = dto.Confidence;
        EnergyLevel = dto.EnergyLevel;
        Responsibility = dto.Responsibility;
        Assistance = dto.Assistance;
        GearedUpWeapons = dto.GearedUpWeapons;
        HeightMin = dto.HeightMin;
        HeightMax = dto.HeightMax;
        SkinToneIndex = dto.SkinToneIndex;
        Pronoun = dto.Pronoun;
        VoiceFormKey = dto.VoiceFormKey?.ToString();
        RaceFormKey = dto.RaceFormKey?.ToString();
        CombatOverridePackageListFormKey = dto.CombatOverridePackageListFormKey?.ToString();
        CombatStyleFormKey = dto.CombatStyleFormKey?.ToString();
        DefaultPackageListFormKey = dto.DefaultPackageListFormKey?.ToString();
        CrimeFactionFormKey = dto.CrimeFactionFormKey?.ToString();
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("EditorID")] public string EditorId { get; set; } = string.Empty;
    [Column("FormVersion")] public int FormVersion { get; set; }
    [Column("StarfieldMajorRecordFlags")] public int StarfieldMajorRecordFlags { get; set; }
    [Column("Version2")] public int Version2 { get; set; }
    [Column("VersionControl")] public int VersionControl { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
    [Column("Name")] public string? Name { get; set; }
    [Column("ShortName")] public string? ShortName { get; set; }
    [Column("LongName")] public string? LongName { get; set; }
    [Column("DispositionBase")] public int DispositionBase { get; set; }
    [Column("Aggression")] public string Aggression { get; set; } = string.Empty;
    [Column("Confidence")] public string Confidence { get; set; } = string.Empty;
    [Column("EnergyLevel")] public int EnergyLevel { get; set; }
    [Column("Responsibility")] public string Responsibility { get; set; } = string.Empty;
    [Column("Assistance")] public string Assistance { get; set; } = string.Empty;
    [Column("GearedUpWeapons")] public int GearedUpWeapons { get; set; }
    [Column("HeightMin")] public double HeightMin { get; set; }
    [Column("HeightMax")] public double HeightMax { get; set; }
    [Column("SkinToneIndex")] public int? SkinToneIndex { get; set; }
    [Column("Pronoun")] public string? Pronoun { get; set; }
    [Column("VoiceFormKey")] public string? VoiceFormKey { get; set; }
    [Column("RaceFormKey")] public string? RaceFormKey { get; set; }

    [Column("CombatOverridePackageListFormKey")]
    public string? CombatOverridePackageListFormKey { get; set; }

    [Column("CombatStyleFormKey")] public string? CombatStyleFormKey { get; set; }
    [Column("DefaultPackageListFormKey")] public string? DefaultPackageListFormKey { get; set; }
    [Column("CrimeFactionFormKey")] public string? CrimeFactionFormKey { get; set; }
}