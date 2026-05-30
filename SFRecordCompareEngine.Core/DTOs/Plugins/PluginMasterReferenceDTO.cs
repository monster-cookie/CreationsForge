using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginMasterReferenceDTO
{
    public PluginMasterReferenceDTO()
    { }

    public PluginMasterReferenceDTO(PluginMasterReference model)
    {
        if (!Enum.IsDefined(typeof(ModType), model.MasterModKeyType))
        {
            throw new ArgumentOutOfRangeException(nameof(model.MasterModKeyType), model.MasterModKeyType, "Invalid master mod type value.");
        }

        if (!Enum.IsDefined(typeof(ModType), model.PluginModKeyType))
        {
            throw new ArgumentOutOfRangeException(nameof(model.PluginModKeyType), model.PluginModKeyType, "Invalid plugin mod type value.");
        }

        MasterModKey = new ModKey(model.MasterModKeyName, (ModType)model.MasterModKeyType);
        PluginModKey = new ModKey(model.PluginModKeyName, (ModType)model.PluginModKeyType);
        ImportedAtUTC = model.ImportedAtUTC;
    }

    public ModKey MasterModKey { get; set; }
    public ModKey PluginModKey { get; set; }
    public DateTime ImportedAtUTC { get; set; }
}