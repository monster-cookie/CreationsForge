using Serilog;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class RecordService(
    ISqliteConnectionFactory connectionFactory,
    IRecordHeaderRepository recordHeaderRepository) : IRecordService
{
    private readonly ILogger Logger = Log.ForContext<RecordService>();

    public string? ResolveReferenceDisplayValue(string referenceValue)
    {
        if (string.IsNullOrWhiteSpace(referenceValue)) return null;

            var normalizedReferenceValue = FormKeyTextNormalizer.NormalizeReferenceValue(referenceValue);
            try
            {
                using var database = connectionFactory.OpenDatabase();
            var recordHeader = recordHeaderRepository.GetCurrentByFormKey(database, normalizedReferenceValue);
            return string.IsNullOrWhiteSpace(recordHeader?.EditorID)
                ? normalizedReferenceValue
                : recordHeader.EditorID;
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Unable to resolve record reference display value for {ReferenceValue}", normalizedReferenceValue);
            return normalizedReferenceValue;
        }
    }
}
