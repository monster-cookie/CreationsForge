using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RecordTreeService : IRecordTreeService
{
    private readonly IReadOnlyList<IRecordTreeRepository> RecordTreeRepositories;

    public RecordTreeService(IEnumerable<IRecordTreeRepository> recordTreeRepositories)
    {
        RecordTreeRepositories = recordTreeRepositories
            .ToList();
    }

    public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntries(SupportedGame game, ModKeyDTO modKey)
    {
        return RecordTreeRepositories
            .SelectMany(repository => repository.GetRecordTreeEntriesByPlugin(game, modKey))
            .ToList();
    }
}
