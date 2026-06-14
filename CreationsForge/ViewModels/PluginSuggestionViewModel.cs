using Avalonia.Media;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.ViewModels;

public class PluginSuggestionViewModel
{
    public PluginSuggestionViewModel(PluginDTO plugin)
    {
        Plugin = plugin;
        FileName = plugin.ModKey.FileName;
        ImportState = plugin.ImportState;
        StatusBrush = CreateStatusBrush(plugin.ImportState);
    }

    public PluginDTO Plugin { get; }

    public string FileName { get; }

    public PluginImportState ImportState { get; }

    public IBrush StatusBrush { get; }

    public override string ToString()
    {
        return FileName;
    }

    private static IBrush CreateStatusBrush(PluginImportState importState)
    {
        return importState switch
        {
            PluginImportState.Changed => new SolidColorBrush(Color.FromRgb(255, 168, 74)),
            PluginImportState.PartiallyImported => new SolidColorBrush(Color.FromRgb(238, 190, 82)),
            PluginImportState.Missing => new SolidColorBrush(Color.FromRgb(178, 144, 255)),
            PluginImportState.Failed => new SolidColorBrush(Color.FromRgb(255, 112, 112)),
            PluginImportState.Unsupported => new SolidColorBrush(Color.FromRgb(178, 186, 196)),
            _ => App.GetApplicationForegroundBrush()
        };
    }
}
