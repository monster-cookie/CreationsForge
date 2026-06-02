using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;
using Keyword = SFRecordCompareEngine.Core.Models.Database.Keyword;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class KeywordDTO : IHasScriptingAdaptersRecordDTO
{
    public KeywordDTO()
    { }

    [SetsRequiredMembers]
    public KeywordDTO(Keyword model)
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
        Color = model.Color;
        Type = model.Type;
        Notes = model.Notes;
        FlashLinkageName = model.FlashLinkageName;
        AttractionRuleFormKey = string.IsNullOrWhiteSpace(model.AttractionRuleFormKey) ? null : FormKey.Factory(model.AttractionRuleFormKey);
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
    public required string Color { get; set; }
    public required string Type { get; set; }
    public string? Notes { get; set; }
    public string? FlashLinkageName { get; set; }
    public FormKey? AttractionRuleFormKey { get; set; }
    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
