using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class ContainerImporter : ITypedRecordImporter
{
    private readonly IContainerRepository ContainerRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public ContainerImporter(
        IContainerRepository containerRepository,
        IRecordChildImportService recordChildImportService)
    {
        ContainerRepository = containerRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.Container.RecordID;

    public string TableName => RecordTypeCatalog.Container.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not ContainerDTO container) throw new ArgumentException($"Expected {nameof(ContainerDTO)}.", nameof(recordDTO));

        container.ImportedAtUTC = importedAtUTC;
        ContainerRepository.Save(container);
        RecordChildImportService.ReplaceRecordChildren(container, RecordTypeCatalog.Container.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        ContainerRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
