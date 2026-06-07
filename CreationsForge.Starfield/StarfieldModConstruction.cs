using CreationsForge.Core.DTOs.Plugins;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Starfield;

namespace CreationsForge.Starfield;

internal static class StarfieldModConstruction
{
    public static IStarfieldModGetter Load(ModKeyDTO modKey)
    {
        return Load(modKey.FileName);
    }

    public static IStarfieldModGetter Load(string fileName)
    {
        var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
        var dataFolderPath = environment.DataFolderPath;
        var loadOrderMods = environment.LoadOrder.ListedOrder
            .Where(listing => listing.Mod is not null)
            .Select(listing => (IModMasterStyledGetter)listing.Mod!)
            .ToArray();

        var builder = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(Path.Combine(dataFolderPath, fileName));

        if (loadOrderMods.Length > 0)
        {
            return builder
                .WithLoadOrder(loadOrderMods)
                .WithDataFolder(dataFolderPath)
                .Construct();
        }

        return builder
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(dataFolderPath)
            .Construct();
    }

    public static string GetDataFolderPath()
    {
        var environment = GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield);
        return environment.DataFolderPath;
    }
}
