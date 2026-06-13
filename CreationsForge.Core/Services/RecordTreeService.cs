using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RecordTreeService : IRecordTreeService
{
    private readonly IRecordInstanceRepository RecordInstanceRepository;

    public RecordTreeService(IRecordInstanceRepository recordInstanceRepository)
    {
        RecordInstanceRepository = recordInstanceRepository;
    }

    public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntries(SupportedGame game, ModKeyDTO modKey)
    {
        return RecordInstanceRepository.GetRecordTreeEntriesByPlugin(game, modKey);
    }
}
