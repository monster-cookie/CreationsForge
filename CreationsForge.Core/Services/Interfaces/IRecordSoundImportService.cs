using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IRecordSoundImportService
{
    void ReplaceRecordSounds(IHasSoundsRecordDTO record, string recordType);
}
