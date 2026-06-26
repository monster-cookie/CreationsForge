using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class BookImporter : ITypedRecordImporter
{
    private readonly IBookRepository BookRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public BookImporter(IBookRepository bookRepository, IRecordChildImportService recordChildImportService)
    {
        BookRepository = bookRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.Book.RecordID;

    public string TableName => RecordTypeCatalog.Book.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame> { SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not BookDTO book) throw new ArgumentException($"Expected {nameof(BookDTO)}.", nameof(recordDTO));

        book.ImportedAtUTC = importedAtUTC;
        BookRepository.Save(book);
        RecordChildImportService.ReplaceRecordChildren(book, RecordTypeCatalog.Book.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        BookRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
