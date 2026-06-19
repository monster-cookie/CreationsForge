using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IConditionRuleImportService
{
    void ReplaceConditionRules(IHasConditionsRecordDTO record, string recordType);
}
