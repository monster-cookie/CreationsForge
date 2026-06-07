using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class MagicEffectImporter : ITypedRecordImporter
{
    private readonly IMagicEffectRepository MagicEffectRepository;
    private readonly IRecordKeywordImportService RecordKeywordImportService;
    private readonly IRecordSoundImportService RecordSoundImportService;
    private readonly IScriptingAdapterImportService ScriptingAdapterImportService;

    public MagicEffectImporter(
        IMagicEffectRepository magicEffectRepository,
        IScriptingAdapterImportService scriptingAdapterImportService,
        IRecordKeywordImportService recordKeywordImportService,
        IRecordSoundImportService recordSoundImportService)
    {
        MagicEffectRepository = magicEffectRepository;
        ScriptingAdapterImportService = scriptingAdapterImportService;
        RecordKeywordImportService = recordKeywordImportService;
        RecordSoundImportService = recordSoundImportService;
    }

    public string RecordType => RecordTypeCatalog.MagicEffect.RecordID;

    public string TableName => RecordTypeCatalog.MagicEffect.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not MagicEffectDTO magicEffect) throw new ArgumentException($"Expected {nameof(MagicEffectDTO)}.", nameof(recordDTO));

        magicEffect.ImportedAtUTC = importedAtUTC;
        MagicEffectRepository.Save(magicEffect);
        RecordKeywordImportService.ReplaceRecordKeywords(magicEffect, RecordTypeCatalog.MagicEffect.RecordID);
        RecordSoundImportService.ReplaceRecordSounds(magicEffect, RecordTypeCatalog.MagicEffect.RecordID);
        ScriptingAdapterImportService.ReplaceRecordScriptingAdapters(magicEffect, RecordTypeCatalog.MagicEffect.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        MagicEffectRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
