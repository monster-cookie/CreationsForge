using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class ActorValueInformationImporter : ITypedRecordImporter
{
    private readonly IActorValueInformationRepository ActorValueInformationRepository;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;

    public ActorValueInformationImporter(
        IActorValueInformationRepository actorValueInformationRepository,
        IScriptingAdapterImportService scriptingAdapterImportService)
    {
        ActorValueInformationRepository = actorValueInformationRepository;
        ScriptingAdapterImportService = scriptingAdapterImportService;
    }

    public string RecordType => RecordTypeCatalog.ActorValueInformation.RecordID;

    public string TableName => RecordTypeCatalog.ActorValueInformation.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not ActorValueInformationDTO actorValueInformation) throw new ArgumentException($"Expected {nameof(ActorValueInformationDTO)}.", nameof(recordDTO));

        actorValueInformation.ImportedAtUTC = importedAtUTC;
        ActorValueInformationRepository.Save(actorValueInformation);
        ScriptingAdapterImportService.ReplaceRecordScriptingAdapters(actorValueInformation, RecordTypeCatalog.ActorValueInformation.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        ActorValueInformationRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
