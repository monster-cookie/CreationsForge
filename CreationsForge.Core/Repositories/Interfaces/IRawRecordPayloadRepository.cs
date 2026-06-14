using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IRawRecordPayloadRepository
{
    void Save(RawRecordPayloadDTO dto);

    IReadOnlyList<RawRecordPayloadDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey);

    void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey);
}
