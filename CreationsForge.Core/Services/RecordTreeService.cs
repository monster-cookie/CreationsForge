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
            .SelectMany(repository => ApplyPluginCounts(
                repository.GetRecordTreeEntriesByPlugin(game, modKey),
                repository.GetRecordPluginCountsByGame(game)))
            .ToList();
    }

    private static IEnumerable<RecordTreeEntryDTO> ApplyPluginCounts(
        IEnumerable<RecordTreeEntryDTO> entries,
        IReadOnlyDictionary<string, int> pluginCounts)
    {
        foreach (var entry in entries)
        {
            entry.PluginCount = pluginCounts.TryGetValue(GetFormKeyKey(entry.FormKey), out var pluginCount)
                ? pluginCount
                : entry.PluginCount;
            yield return entry;
        }
    }

    private static string GetFormKeyKey(FormKeyDTO formKey)
    {
        return $"{formKey.ModKey.Name}|{formKey.ModKey.Type}|{formKey.ModKey.FileName}|{formKey.Id}";
    }
}
