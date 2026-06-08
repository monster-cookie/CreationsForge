using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IGlobalRepository
{
    IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey);

    IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game);

    IReadOnlyList<GlobalDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey);

    void Save(GlobalDTO dto);

    void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC);
}
