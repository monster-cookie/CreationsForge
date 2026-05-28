using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginService : IPluginService
{
    private readonly IStarfieldPluginReaderService StarfieldPluginReaderService;

    public PluginService(IStarfieldPluginReaderService starfieldPluginReaderService)
    {
        StarfieldPluginReaderService = starfieldPluginReaderService;
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
}
