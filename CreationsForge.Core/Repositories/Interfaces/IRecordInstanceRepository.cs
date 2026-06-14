using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IRecordInstanceRepository
{
    IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey);

    void Save(RecordInstanceDTO dto);

    void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, string recordType, DateTime importedAtUTC);
}
