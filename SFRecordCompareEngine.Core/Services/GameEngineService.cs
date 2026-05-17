using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace SFRecordCompareEngine.Core.Services;

public class GameEngineService : IGameEngineService
{
    private readonly ILogger Logger = Log.ForContext<GameEngineService>();
    
    public bool ValidateStarfieldPluginHeaders(string dataFolderPath)
    {
        var pluginFiles = Directory
            .EnumerateFiles(dataFolderPath, "*.*", SearchOption.TopDirectoryOnly)
            .Where(p =>
                p.EndsWith(".esm", StringComparison.OrdinalIgnoreCase) ||
                p.EndsWith(".esp", StringComparison.OrdinalIgnoreCase) ||
                p.EndsWith(".esl", StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p)
            .ToList();

        foreach (var path in pluginFiles)
        {
            try
            {
                var header = ModHeaderFrame.FromPath(path, GameRelease.Starfield);
                Logger.Debug("OK: {GetFileName} | MasterStyle={HeaderMasterStyle}", Path.GetFileName(path), header.MasterStyle);
            }
            catch (ModHeaderMalformedException ex)
            {
                Logger.Error(ex, "Malformed Starfield plugin header in '{Path}': {Message}", path, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed while checking plugin header in '{Path}': {Message}", path, ex.Message);
                return false;
            }
        }
        
        Logger.Information("All plugins are valid");
        return true;
    }
}