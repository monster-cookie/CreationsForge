using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IPluginImportService
{
    Task<PluginImportResultDTO> InitializeAndImportAsync(IProgress<PluginImportProgressDTO>? progress, CancellationToken cancellationToken);
}
