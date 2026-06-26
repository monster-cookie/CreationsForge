using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IClassRepository : IRecordTreeRepository
{
    void Save(ClassDTO dto);

    IReadOnlyList<ClassDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey);

    void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC);
}
