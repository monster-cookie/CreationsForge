using System;
using System.Threading;
using System.Threading.Tasks;
using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IPluginImportService
{
    /// <summary>
    /// Initialize the plugin import process.
    /// </summary>
    /// <param name="progress">DTO for returning progress updates during the import process.</param>
    /// <param name="cancellationToken">Token to cancel the import operation.</param>
    /// <returns>Result of the plugin import operation.</returns>
    Task<PluginImportResultDTO> InitializeAndImportAsync(IProgress<PluginImportProgressDTO>? progress, CancellationToken cancellationToken);
}
