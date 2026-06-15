using Avalonia.Media;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.ViewModels;

public class OpenPluginRowViewModel
{
    public OpenPluginRowViewModel(PluginDTO plugin)
    {
        Plugin = plugin;
        FileName = plugin.ModKey.FileName;
        LoadOrderText = plugin.LoadOrderIndex.ToString("X2");
        RecordCountText = plugin.RecordCount > 0 ? plugin.RecordCount.ToString("N0") : "-";
        ImportStateText = plugin.ExistsOnDisk ? plugin.ImportState.ToString() : "Missing";
        LastImportedText = plugin.LastImportedUTC?.ToLocalTime().ToString("g") ?? "-";
        LastCheckedText = plugin.LastCheckedUTC.ToLocalTime().ToString("g");
        SourceModifiedText = plugin.SourceLastWriteUTCTicks > 0
            ? new DateTime(plugin.SourceLastWriteUTCTicks, DateTimeKind.Utc).ToLocalTime().ToString("g")
            : "-";
        SourceSizeText = plugin.SourceFileSizeBytes > 0 ? $"{plugin.SourceFileSizeBytes:N0} bytes" : "-";
        StatusBrush = CreateStatusBrush(plugin.ImportState, plugin.ExistsOnDisk);
        DiagnosticSummary = CreateDiagnosticSummary(plugin);
        DiagnosticDetails = CreateDiagnosticDetails(plugin);
        DiagnosticTooltip = string.IsNullOrWhiteSpace(DiagnosticDetails)
            ? DiagnosticSummary
            : DiagnosticDetails;
        CanOpen = plugin.ExistsOnDisk &&
            plugin.ImportState is PluginImportState.Current or PluginImportState.Changed or PluginImportState.PartiallyImported or PluginImportState.Failed;
    }

    public PluginDTO Plugin { get; }

    public string FileName { get; }

    public string LoadOrderText { get; }

    public string RecordCountText { get; }

    public string ImportStateText { get; }

    public string LastImportedText { get; }

    public string LastCheckedText { get; }

    public string SourceModifiedText { get; }

    public string SourceSizeText { get; }

    public IBrush StatusBrush { get; }

    public string DiagnosticSummary { get; }

    public string DiagnosticDetails { get; }

    public string DiagnosticTooltip { get; }

    public bool CanOpen { get; }

    public bool HasDiagnostics => !string.IsNullOrWhiteSpace(DiagnosticSummary) ||
        !string.IsNullOrWhiteSpace(DiagnosticDetails);

    public override string ToString()
    {
        return FileName;
    }

    private static string CreateDiagnosticSummary(PluginDTO plugin)
    {
        if (!string.IsNullOrWhiteSpace(plugin.ImportMessage))
        {
            return plugin.ImportMessage;
        }

        if (!plugin.ExistsOnDisk)
        {
            return "Plugin source file is missing.";
        }

        return plugin.ImportState switch
        {
            PluginImportState.Changed => "Plugin source changed since the last import.",
            PluginImportState.Failed => "Plugin import failed.",
            PluginImportState.PartiallyImported => "Plugin imported with record failures.",
            PluginImportState.Unsupported => "Plugin is not supported for import.",
            PluginImportState.Missing => "Plugin source file is missing.",
            _ => string.Empty
        };
    }

    private static string CreateDiagnosticDetails(PluginDTO plugin)
    {
        if (!string.IsNullOrWhiteSpace(plugin.ImportDetails))
        {
            return plugin.ImportDetails;
        }

        if (plugin.ImportState == PluginImportState.Changed && plugin.InvalidatedAtUTC.HasValue)
        {
            return $"Invalidated at {plugin.InvalidatedAtUTC.Value.ToLocalTime():g}. Reimport to refresh cached data.";
        }

        return string.Empty;
    }

    private static IBrush CreateStatusBrush(PluginImportState importState, bool existsOnDisk)
    {
        if (!existsOnDisk)
        {
            return new SolidColorBrush(Color.FromRgb(118, 124, 132));
        }

        return importState switch
        {
            PluginImportState.Current => new SolidColorBrush(Color.FromRgb(64, 132, 76)),
            PluginImportState.Changed => new SolidColorBrush(Color.FromRgb(156, 125, 24)),
            PluginImportState.PartiallyImported => new SolidColorBrush(Color.FromRgb(156, 125, 24)),
            PluginImportState.Missing => new SolidColorBrush(Color.FromRgb(118, 124, 132)),
            PluginImportState.Failed => new SolidColorBrush(Color.FromRgb(162, 66, 66)),
            PluginImportState.Unsupported => new SolidColorBrush(Color.FromRgb(118, 124, 132)),
            _ => App.GetApplicationForegroundBrush()
        };
    }
}
