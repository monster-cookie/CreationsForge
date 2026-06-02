using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Services.Interfaces;

public interface IActivePluginSelectionService
{
    PluginDTO? ActivePlugin { get; }
    event EventHandler? ActivePluginChanged;

    void SetActivePlugin(PluginDTO plugin);
    void ClearActivePlugin();
}
