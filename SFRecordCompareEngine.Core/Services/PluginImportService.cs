using System.Globalization;
using System.IO;
using Mutagen.Bethesda.Plugins;
using Serilog;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginImportService : IPluginImportService
{
    private readonly ILogger Logger = Log.ForContext<PluginImportService>();

    public PluginImportService()
    {
        // TODO: Reimplement as needed. During refactor.
    }

    public Task<PluginImportResultDTO> InitializeAndImportAsync(IProgress<PluginImportProgressDTO>? progress, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        // TODO: Reimplement uings mutagen safe and multi-threaded.
    }

    private static bool IsUnsupportedPlugin(PluginLoadOrderEntryDTO loadOrderEntry)
    {
        var pluginFileName = string.IsNullOrWhiteSpace(loadOrderEntry.PluginFileName) ? loadOrderEntry.ModKey.ToString() : loadOrderEntry.PluginFileName;
        return pluginFileName.StartsWith("BlueprintShips", StringComparison.OrdinalIgnoreCase) && pluginFileName.EndsWith(".esm", StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatUtc(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.UtcDateTime.ToString("O", CultureInfo.InvariantCulture);
    }
}
