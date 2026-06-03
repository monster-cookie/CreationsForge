using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;
using ActorValueInformation = SFRecordCompareEngine.Core.Models.Database.ActorValueInformation;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class ActorValueInformationDTO : IHasScriptingAdaptersRecordDTO
{
    public ActorValueInformationDTO()
    { }

    [SetsRequiredMembers]
    public ActorValueInformationDTO(ActorValueInformation model)
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
        Abbreviation = model.Abbreviation;
        ContextNotes = model.ContextNotes;
        DefaultValue = model.DefaultValue;
        Flags = model.Flags;
        Type = model.Type;
        Min = model.Min;
        Max = model.Max;
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
    public string? Abbreviation { get; set; }
    public string? ContextNotes { get; set; }
    public double? DefaultValue { get; set; }
    public string? Flags { get; set; }
    public string? Type { get; set; }
    public double? Min { get; set; }
    public double? Max { get; set; }
    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
