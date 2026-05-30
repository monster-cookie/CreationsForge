using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginService : IPluginService
{
    private readonly IStarfieldPluginReaderService StarfieldPluginReaderService;
    private readonly IPluginRepository PluginRepository;

    public PluginService(
        IStarfieldPluginReaderService starfieldPluginReaderService,
        IPluginRepository pluginRepository)
    {
        StarfieldPluginReaderService = starfieldPluginReaderService;
        PluginRepository = pluginRepository;
    }

    /// <inheritdoc />
    public IList<string> GetRecordTypes()
    {
        return MajorRecordTypeEnumerator
            .GetMajorRecordTypesFor(GameCategory.Starfield)
            .OrderBy(x => x.ClassType.Name)
            .Select(x => x.ClassType.Name)
            .ToList();
    }

    /// <inheritdoc />
    public IList<PluginLoadOrderEntryDTO> GetLoadOrder()
    {
        return StarfieldPluginReaderService.GetLoadOrder();
    }

    /// <inheritdoc />
    public IList<PluginDTO> GetImportedPlugins()
    {
        return PluginRepository.GetImportedPlugins();
    }

    /// <inheritdoc />
    public IList<PluginDTO> GetOpenablePlugins()
    {
        return PluginRepository.GetOpenablePlugins();
    }

    /// <inheritdoc />
    public IList<PluginDTO> SearchOpenablePluginsByFilename(string searchFilename)
    {
        return PluginRepository.SearchOpenablePluginsByFilename(searchFilename);
    }
}