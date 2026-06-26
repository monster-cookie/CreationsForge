using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.Services.Interfaces;

public interface IConditionRuleImportService
{
    void ReplaceConditionRules(IHasConditionsDTO record, string recordType);
}
