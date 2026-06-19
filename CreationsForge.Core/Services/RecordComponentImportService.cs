using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RecordComponentImportService : IRecordComponentImportService
{
    private readonly IRecordComponentRepository RecordComponentRepository;

    public RecordComponentImportService(IRecordComponentRepository recordComponentRepository)
    {
        RecordComponentRepository = recordComponentRepository;
    }

    public void ReplaceRecordComponents(IHasComponentsRecordDTO record, string recordType)
    {
        foreach (var component in record.Components)
        {
            component.RecordType = recordType;
            foreach (var item in component.Items)
            {
                item.RecordType = recordType;
            }
        }

        RecordComponentRepository.ReplaceRecordComponents(record, recordType);
    }
}
