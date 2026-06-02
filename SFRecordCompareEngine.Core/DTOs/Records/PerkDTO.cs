using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;
using Perk = SFRecordCompareEngine.Core.Models.Database.Perk;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class PerkDTO : IHasScriptingAdaptersRecordDTO
{
    public PerkDTO()
    { }

    [SetsRequiredMembers]
    public PerkDTO(Perk model)
    {
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        FormKey = new FormKey(ModKey, (uint)model.FormKeyId);
        EditorID = model.EditorId;
        FormVersion = model.FormVersion;
        StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)model.StarfieldMajorRecordFlags;
        Version2 = model.Version2;
        VersionControl = model.VersionControl;
        ImportedAtUTC = model.ImportedAtUTC;
        Name = model.Name;
        Description = model.Description;
        Flags = model.Flags;
        SkillGroup = model.SkillGroup;
        CrewAssignment = model.CrewAssignment;
        PerkIcon = model.PerkIcon;
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
    public string? Description { get; set; }
    public required string Flags { get; set; }
    public string? SkillGroup { get; set; }
    public string? CrewAssignment { get; set; }
    public string? PerkIcon { get; set; }
    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
