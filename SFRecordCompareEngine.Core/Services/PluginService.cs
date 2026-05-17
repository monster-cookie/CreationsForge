using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginService : IPluginService
{
    private readonly ILogger Logger = Log.ForContext<PluginService>();
    private readonly IGameEnvironment Starfield = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);

    /// <inheritdoc />
    public IList<string> GetDatabases()
    {
        try
        {
            IList<string> databases = new List<string>();
            foreach (var database in Starfield.LoadOrder.ListedOrder)
            {
                // Exclude the Starfield.esm database as all other records automatically compare to it
                if (database.FileName.Equals("Starfield.esm", StringComparison.CurrentCultureIgnoreCase)) continue;
                // Debating whether to exclude the DLC and free Bethesda plugins
                databases.Add(database.FileName);
            }

            return databases;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load databases.");
            return new List<string>();
        }
    }

    public PluginHeaderDTO? GetPluginHeader(string pluginName)
    {
        try
        {
            var plugin = LoadPlugin(pluginName);
            return new PluginHeaderDTO(pluginName, plugin.ModHeader);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Unable to load plugin header for {pluginName}.");
            return null;
        }
    }
    
    private IStarfieldModDisposableGetter LoadPlugin(string pluginName)
    {
        var pluginPath = Path.Combine(Starfield.DataFolderPath.Path, pluginName);
        var modKey = ModKey.FromFileName(Path.GetFileName(pluginPath));
        var modPath = new ModPath(modKey, pluginPath);

        return StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(Starfield.DataFolderPath.Path)
            .Construct();
    }

    private static string CleanRecordTypeName(string typeName)
    {
        return typeName.Replace("BinaryOverlay", "").Replace("Getter", "").Replace("Setter", "");
    }
}