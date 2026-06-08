using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IRecordSoundRepository
{
    void Save(RecordSoundDTO dto);

    IReadOnlyList<RecordSoundDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey);

    void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey);

    void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC);
}
