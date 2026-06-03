using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class NPCDTO : IHasScriptingAdaptersRecordDTO
{
    public NPCDTO()
    { }

    [SetsRequiredMembers]
    public NPCDTO(NPC model)
    {
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        var formKeyModKey = new ModKey(model.FormKeyModKeyName, (ModType)model.FormKeyModKeyType);
        FormKey = new FormKey(formKeyModKey, (uint)model.FormKeyId);
        EditorID = model.EditorId;
        FormVersion = model.FormVersion;
        StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)model.StarfieldMajorRecordFlags;
        Version2 = model.Version2;
        VersionControl = model.VersionControl;
        ImportedAtUTC = model.ImportedAtUTC;
        Name = model.Name;
        ShortName = model.ShortName;
        LongName = model.LongName;
        DispositionBase = model.DispositionBase;
        Aggression = model.Aggression;
        Confidence = model.Confidence;
        EnergyLevel = model.EnergyLevel;
        Responsibility = model.Responsibility;
        Assistance = model.Assistance;
        GearedUpWeapons = model.GearedUpWeapons;
        HeightMin = model.HeightMin;
        HeightMax = model.HeightMax;
        SkinToneIndex = model.SkinToneIndex;
        Pronoun = model.Pronoun;
        VoiceFormKey = ParseFormKey(model.VoiceFormKey);
        RaceFormKey = ParseFormKey(model.RaceFormKey);
        CombatOverridePackageListFormKey = ParseFormKey(model.CombatOverridePackageListFormKey);
        CombatStyleFormKey = ParseFormKey(model.CombatStyleFormKey);
        DefaultPackageListFormKey = ParseFormKey(model.DefaultPackageListFormKey);
        CrimeFactionFormKey = ParseFormKey(model.CrimeFactionFormKey);
    }

    public required ModKey ModKey { get; set; }
    public required FormKey FormKey { get; set; }
    public required string EditorID { get; set; }
    public required int FormVersion { get; set; }
    public required StarfieldMajorRecord.StarfieldMajorRecordFlag StarfieldMajorRecordFlags { get; set; }
    public required int Version2 { get; set; }
    public required int VersionControl { get; set; }
    public required DateTime ImportedAtUTC { get; set; }
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public string? LongName { get; set; }
    public int DispositionBase { get; set; }
    public required string Aggression { get; set; }
    public required string Confidence { get; set; }
    public int EnergyLevel { get; set; }
    public required string Responsibility { get; set; }
    public required string Assistance { get; set; }
    public int GearedUpWeapons { get; set; }
    public double HeightMin { get; set; }
    public double HeightMax { get; set; }
    public int? SkinToneIndex { get; set; }
    public string? Pronoun { get; set; }
    public FormKey? VoiceFormKey { get; set; }
    public FormKey? RaceFormKey { get; set; }
    public FormKey? CombatOverridePackageListFormKey { get; set; }
    public FormKey? CombatStyleFormKey { get; set; }
    public FormKey? DefaultPackageListFormKey { get; set; }
    public FormKey? CrimeFactionFormKey { get; set; }
    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    private static FormKey? ParseFormKey(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : FormKey.Factory(value);
    }
}
