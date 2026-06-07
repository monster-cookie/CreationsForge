using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RecordSoundImportService : IRecordSoundImportService
{
    private readonly IRecordSoundRepository RecordSoundRepository;

    public RecordSoundImportService(IRecordSoundRepository recordSoundRepository)
    {
        RecordSoundRepository = recordSoundRepository;
    }

    public void ReplaceRecordSounds(IHasSoundsRecordDTO record, string recordType)
    {
        if (record is not RecordDTO recordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        RecordSoundRepository.DeleteByRecord(recordDTO.Game, recordDTO.ModKey, recordType, recordDTO.FormKey);

        foreach (var sound in record.Sounds)
        {
            sound.Game = recordDTO.Game;
            sound.ModKey = recordDTO.ModKey;
            sound.RecordType = recordType;
            sound.FormKey = recordDTO.FormKey;
            sound.ImportedAtUTC = recordDTO.ImportedAtUTC;
            RecordSoundRepository.Save(sound);
        }
    }
}
