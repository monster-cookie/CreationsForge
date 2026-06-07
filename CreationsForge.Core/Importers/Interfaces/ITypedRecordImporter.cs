using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.Importers.Interfaces;

public interface ITypedRecordImporter
{
    string RecordType { get; }

    string TableName { get; }

    IReadOnlySet<SupportedGame> SupportedGames { get; }

    void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC);

    void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC);
}
