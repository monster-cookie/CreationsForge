using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RecordLocalizedStringImportService : IRecordLocalizedStringImportService
{
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;

    public RecordLocalizedStringImportService(IRecordLocalizedStringRepository recordLocalizedStringRepository)
    {
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
    }

    public void ReplaceRecordLocalizedStrings(IHasLocalizedStringsRecordDTO record, string recordType)
    {
        if (record is not RecordDTO recordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        RecordLocalizedStringRepository.DeleteByRecord(recordDTO.Game, recordDTO.ModKey, recordType, recordDTO.FormKey);

        foreach (var localizedString in record.LocalizedStrings)
        {
            localizedString.Game = recordDTO.Game;
            localizedString.ModKey = recordDTO.ModKey;
            localizedString.RecordType = recordType;
            localizedString.FormKey = recordDTO.FormKey;
            localizedString.ImportedAtUTC = recordDTO.ImportedAtUTC;
            RecordLocalizedStringRepository.Save(localizedString);
        }
    }
}
