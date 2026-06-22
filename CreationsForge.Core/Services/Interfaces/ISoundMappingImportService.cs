using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface ISoundMappingImportService
{
    void ReplaceSoundMappings(ISounds record, string recordType);
}
