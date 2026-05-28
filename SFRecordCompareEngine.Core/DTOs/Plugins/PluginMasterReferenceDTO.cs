using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginMasterReferenceDTO
{
    public PluginMasterReferenceDTO()
    { }

    public PluginMasterReferenceDTO(PluginMasterReference model)
    {
        if (!Enum.IsDefined(typeof(ModType), model.ModKeyType))
        {
            throw new ArgumentOutOfRangeException(nameof(model.ModKeyType), model.ModKeyType, "Invalid mod type value.");
        }

        if (!Enum.IsDefined(typeof(ModType), model.ParentModKeyType))
        {
            throw new ArgumentOutOfRangeException(nameof(model.ParentModKeyType), model.ParentModKeyType, "Invalid parent mod type value.");
        }

        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        ParentModKey = new ModKey(model.ParentModKeyName, (ModType)model.ParentModKeyType);
        MasterReferenceIndex = model.MasterReferenceIndex;
        ParentLoadOrderIndex = model.ParentLoadOrderIndex;
        ImportedAtUTC = model.ImportedAtUTC;
    }

    public ModKey ModKey { get; set; }
    public ModKey ParentModKey { get; set; }
    public int MasterReferenceIndex { get; set; }
    public int ParentLoadOrderIndex { get; set; }
    public DateTime ImportedAtUTC { get; set; }
}
