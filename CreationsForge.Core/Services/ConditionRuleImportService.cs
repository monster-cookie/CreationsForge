using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class ConditionRuleImportService : IConditionRuleImportService
{
    private readonly IConditionRuleRepository ConditionRuleRepository;

    public ConditionRuleImportService(IConditionRuleRepository conditionRuleRepository)
    {
        ConditionRuleRepository = conditionRuleRepository;
    }

    public void ReplaceConditionRules(IHasConditionsRecordDTO record, string recordType)
    {
        if (record is not RecordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        ConditionRuleRepository.ReplaceConditionRules(record, recordType);
    }
}
