using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Services.Interfaces;

public interface IRecordComparisonService
{
    RecordComparisonDTO GetRecordComparison(SupportedGame game, string recordType, FormKeyDTO formKey);
}
