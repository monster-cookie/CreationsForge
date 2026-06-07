using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class KeywordImporter : ITypedRecordImporter
{
    private readonly IKeywordRepository KeywordRepository;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;

    public KeywordImporter(
        IKeywordRepository keywordRepository,
        IScriptingAdapterImportService scriptingAdapterImportService)
    {
        KeywordRepository = keywordRepository;
        ScriptingAdapterImportService = scriptingAdapterImportService;
    }

    public string RecordType => RecordTypeCatalog.Keyword.RecordID;

    public string TableName => RecordTypeCatalog.Keyword.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not KeywordDTO keyword) throw new ArgumentException($"Expected {nameof(KeywordDTO)}.", nameof(recordDTO));

        keyword.ImportedAtUTC = importedAtUTC;
        KeywordRepository.Save(keyword);
        ScriptingAdapterImportService.ReplaceRecordScriptingAdapters(keyword, RecordTypeCatalog.Keyword.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        KeywordRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
