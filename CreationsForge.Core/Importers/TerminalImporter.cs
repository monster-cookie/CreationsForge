using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class TerminalImporter : ITypedRecordImporter
{
    private readonly ITerminalRepository TerminalRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public TerminalImporter(ITerminalRepository terminalRepository, IRecordChildImportService recordChildImportService)
    {
        TerminalRepository = terminalRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.Terminal.RecordID;

    public string TableName => RecordTypeCatalog.Terminal.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not TerminalDTO terminal) throw new ArgumentException($"Expected {nameof(TerminalDTO)}.", nameof(recordDTO));

        terminal.ImportedAtUTC = importedAtUTC;
        TerminalRepository.Save(terminal);
        RecordChildImportService.ReplaceRecordChildren(terminal, RecordTypeCatalog.Terminal.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        TerminalRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
