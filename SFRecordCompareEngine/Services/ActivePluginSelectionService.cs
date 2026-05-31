using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Services.Interfaces;

namespace SFRecordCompareEngine.Services;

public class ActivePluginSelectionService : IActivePluginSelectionService
{
    public event EventHandler? ActivePluginChanged;

    public PluginDTO? ActivePlugin { get; private set; }

    public void SetActivePlugin(PluginDTO plugin)
    {
        ArgumentNullException.ThrowIfNull(plugin);
        ActivePlugin = plugin;
        ActivePluginChanged?.Invoke(this, EventArgs.Empty);
    }
}
