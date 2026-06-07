using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IRecordTreeRepository
{
    string RecordType { get; }

    IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey);

    IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game);
}
