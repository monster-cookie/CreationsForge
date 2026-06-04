using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using SFRecordCompareEngine.Core.DTOs.Records.Interfaces;
using MiscItem = SFRecordCompareEngine.Core.Models.Database.MiscItem;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class MiscItemDTO : IHasScriptingAdaptersRecordDTO
{
    public MiscItemDTO()
    { }

    [SetsRequiredMembers]
    public MiscItemDTO(MiscItem model)
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
        Value = model.Value;
        Weight = model.Weight;
        DirtinessScale = model.DirtinessScale;
        FeaturedItemMessageFormKey = CreateNullableFormKey(model.FeaturedItemMessageModKeyName, model.FeaturedItemMessageModKeyType, model.FeaturedItemMessageFormKeyId);
        Flag = model.Flag;
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
    public int? Value { get; set; }
    public double? Weight { get; set; }
    public float? DirtinessScale { get; set; }
    public FormKey? FeaturedItemMessageFormKey { get; set; }
    public string? Flag { get; set; }
    public MiscItemObjectBoundsDTO? ObjectBounds { get; set; }
    public MiscItemObjectPaletteDefaultsDTO? ObjectPaletteDefaults { get; set; }
    public MiscItemTransformsDTO? Transforms { get; set; }
    public MiscItemModelDTO? Model { get; set; }
    public MiscItemSoundDTO? CraftingSound { get; set; }
    public MiscItemSoundDTO? PickupSound { get; set; }
    public MiscItemSoundDTO? DropdownSound { get; set; }
    public IList<FormKey> Keywords { get; set; } = new List<FormKey>();
    public MiscItemDestructibleDTO? Destructible { get; set; }
    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    private static FormKey? CreateNullableFormKey(string? modKeyName, int? modKeyType, int? formKeyId)
    {
        return modKeyName == null || !modKeyType.HasValue || !formKeyId.HasValue
            ? null
            : new FormKey(new ModKey(modKeyName, (ModType)modKeyType.Value), (uint)formKeyId.Value);
    }
}
