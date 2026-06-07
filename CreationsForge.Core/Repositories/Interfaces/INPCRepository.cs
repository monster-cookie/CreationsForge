using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface INPCRepository : IRecordTreeRepository
{
    void Save(NPCDTO dto);

    IReadOnlyList<NPCDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey);

    void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC);
}
