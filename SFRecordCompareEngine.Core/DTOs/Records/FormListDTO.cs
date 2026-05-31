using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using FormList = SFRecordCompareEngine.Core.Models.Database.FormList;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FormListDTO
{
    public FormListDTO()
    { }

    [SetsRequiredMembers]
    public FormListDTO(FormList model)
    {
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        FormKey = new FormKey(ModKey, (uint)model.FormKeyId);
        EditorID = model.EditorId ?? string.Empty;
        FormVersion = model.FormVersion;
        StarfieldMajorRecordFlags = (StarfieldMajorRecord.StarfieldMajorRecordFlag)model.StarfieldMajorRecordFlags;
        Version2 = model.Version2;
        VersionControl = model.VersionControl;
        ImportedAtUTC = model.ImportedAtUTC;
        AddToListFormKey = string.IsNullOrWhiteSpace(model.AddToListFormKey) ? null : FormKey.Factory(model.AddToListFormKey);
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

    public FormKey? AddToListFormKey { get; set; }
    public IReadOnlyList<FormListItemDataDTO> Items { get; set; } = new List<FormListItemDataDTO>();
}