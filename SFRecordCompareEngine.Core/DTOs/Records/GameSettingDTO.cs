using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using GameSetting = SFRecordCompareEngine.Core.Models.Database.GameSetting;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class GameSettingDTO
{
    public GameSettingDTO()
    { }

    [SetsRequiredMembers]
    public GameSettingDTO(GameSetting model)
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
        SettingType = model.SettingType;
        Data = model.Data;
        RawData = model.RawData;
        XALG = model.XALG;
        IsCompressed = model.IsCompressed;
        IsDeleted = model.IsDeleted;
    }

    public required ModKey ModKey { get; set; }
    public required FormKey FormKey { get; set; }
    public required string EditorID { get; set; }
    public required int FormVersion { get; set; }
    public required StarfieldMajorRecord.StarfieldMajorRecordFlag StarfieldMajorRecordFlags { get; set; }
    public required int Version2 { get; set; }
    public required int VersionControl { get; set; }
    public required DateTime ImportedAtUTC { get; set; }

    // END HEADER

    public string? SettingType { get; set; }
    public string? Data { get; set; }
    public double? RawData { get; set; }
    public int? XALG { get; set; }
    public int IsCompressed { get; set; }
    public int IsDeleted { get; set; }
}