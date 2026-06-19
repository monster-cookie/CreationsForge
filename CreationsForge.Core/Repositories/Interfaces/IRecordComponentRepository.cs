using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IRecordComponentRepository
{
    void ReplaceRecordComponents(IHasComponentsRecordDTO record, string recordType);

    IReadOnlyList<RecordComponentDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey);
}
