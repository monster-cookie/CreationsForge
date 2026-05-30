using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Services.Interfaces;

public interface IActivePluginSelectionService
{
    event EventHandler? ActivePluginChanged;

    PluginDTO? ActivePlugin { get; }

    void SetActivePlugin(PluginDTO plugin);
}
