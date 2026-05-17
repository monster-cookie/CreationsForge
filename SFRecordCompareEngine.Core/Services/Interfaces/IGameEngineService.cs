namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IGameEngineService
{
    /// <summary>
    /// Validate the Starfield plugin headers in the specified data folder path. Logs any issues found.
    /// </summary>
    /// <param name="dataFolderPath">The path to the Starfield data folder</param>
    /// <returns>True if all plugins are valid, false otherwise</returns>
    bool ValidateStarfieldPluginHeaders(string dataFolderPath);
}