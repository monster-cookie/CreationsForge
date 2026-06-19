using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IConditionRuleRepository
{
    void ReplaceConditionRules(IHasConditionsRecordDTO record, string recordType);

    IReadOnlyList<ConditionFormConditionDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey);
}
