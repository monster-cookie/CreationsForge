using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

/// <summary>
/// Replaces component reflection rows for imported parent records.
/// </summary>
public class ReflectionImportService : IReflectionImportService
{
    private readonly IReflectionRepository ReflectionRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReflectionImportService"/> class.
    /// </summary>
    /// <param name="reflectionRepository">The repository used to replace reflection rows.</param>
    public ReflectionImportService(IReflectionRepository reflectionRepository)
    {
        ReflectionRepository = reflectionRepository;
    }

    /// <inheritdoc />
    public void ReplaceReflections(IHasReflectionDTO record, string recordType)
    {
        if (record is not RecordDTO recordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        ReflectionRepository.DeleteByRecord(recordDTO.Game, recordDTO.ModKey, recordType, recordDTO.FormKey);

        foreach (var reflection in record.Reflections)
        {
            reflection.Game = recordDTO.Game;
            reflection.ModKey = recordDTO.ModKey;
            reflection.RecordType = recordType;
            reflection.FormKey = recordDTO.FormKey;
            reflection.ImportedAtUTC = recordDTO.ImportedAtUTC;
            ReflectionRepository.Save(reflection);
        }
    }
}
