using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class ScriptingAdapterPropertyListItemDTO
{
    public ScriptingAdapterPropertyListItemDTO()
    { }

    [SetsRequiredMembers]
    public ScriptingAdapterPropertyListItemDTO(ScriptingAdapterPropertyListItem model)
    {
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        RecordType = model.RecordType;
        FormKey = new FormKey(ModKey, (uint)model.FormKeyId);
        ScriptingAdapterName = model.ScriptingAdapterName;
        PropertyIndex = model.PropertyIndex;
        ListItemIndex = model.ListItemIndex;
        MutagenObjectType = model.MutagenObjectType;
        DataBool = model.DataBool;
        DataInt = model.DataInt;
        DataFloat = model.DataFloat;
        DataString = model.DataString;
        ObjectFormKey = TryCreateFormKey(model.ObjectModKeyName, model.ObjectModKeyType, model.ObjectModKeyFileName, model.ObjectFormKeyId);
        ObjectAlias = model.ObjectAlias;
        ObjectUnused = model.ObjectUnused;
        ImportedAtUTC = model.ImportedAtUTC;
    }

    public required ModKey ModKey { get; set; }
    public required string RecordType { get; set; }
    public required FormKey FormKey { get; set; }
    public required string ScriptingAdapterName { get; set; }
    public required int PropertyIndex { get; set; }
    public required int ListItemIndex { get; set; }
    public required string MutagenObjectType { get; set; }
    public bool? DataBool { get; set; }
    public int? DataInt { get; set; }
    public double? DataFloat { get; set; }
    public string? DataString { get; set; }
    public FormKey? ObjectFormKey { get; set; }
    public short? ObjectAlias { get; set; }
    public ushort? ObjectUnused { get; set; }
    public required DateTime ImportedAtUTC { get; set; }

    private static FormKey? TryCreateFormKey(string? modKeyName, int? modKeyType, string? modKeyFileName, int? formKeyId)
    {
        if (string.IsNullOrWhiteSpace(modKeyName)
            || !modKeyType.HasValue
            || string.IsNullOrWhiteSpace(modKeyFileName)
            || !formKeyId.HasValue)
        {
            return null;
        }

        var modKey = new ModKey(modKeyName, (ModType)modKeyType.Value);
        return new FormKey(modKey, (uint)formKeyId.Value);
    }
}
