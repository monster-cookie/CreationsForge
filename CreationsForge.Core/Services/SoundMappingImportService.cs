using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class SoundMappingImportService : ISoundMappingImportService
{
    private readonly ISoundMappingRepository SoundMappingRepository;

    public SoundMappingImportService(ISoundMappingRepository soundMappingRepository)
    {
        SoundMappingRepository = soundMappingRepository;
    }

    public void ReplaceSoundMappings(ISounds record, string recordType)
    {
        if (record is not RecordDTO recordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        SoundMappingRepository.DeleteByRecord(recordDTO.Game, recordDTO.ModKey, recordType, recordDTO.FormKey);

        foreach (var sound in record.Sounds)
        {
            sound.Game = recordDTO.Game;
            sound.ModKey = recordDTO.ModKey;
            sound.RecordType = recordType;
            sound.FormKey = recordDTO.FormKey;
            sound.ImportedAtUTC = recordDTO.ImportedAtUTC;
            SoundMappingRepository.Save(sound);
        }
    }
}
