using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class KeywordMappingImportService : IKeywordMappingImportService
{
    private readonly IKeywordMappingRepository KeywordMappingRepository;

    public KeywordMappingImportService(IKeywordMappingRepository keywordMappingRepository)
    {
        KeywordMappingRepository = keywordMappingRepository;
    }

    public void ReplaceKeywordMappings(IKeywords record, string recordType)
    {
        if (record is not RecordDTO recordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        KeywordMappingRepository.DeleteByRecord(recordDTO.Game, recordDTO.ModKey, recordType, recordDTO.FormKey);

        foreach (var keyword in record.Keywords)
        {
            keyword.Game = recordDTO.Game;
            keyword.ModKey = recordDTO.ModKey;
            keyword.RecordType = recordType;
            keyword.FormKey = recordDTO.FormKey;
            keyword.ImportedAtUTC = recordDTO.ImportedAtUTC;
            KeywordMappingRepository.Save(keyword);
        }
    }
}
