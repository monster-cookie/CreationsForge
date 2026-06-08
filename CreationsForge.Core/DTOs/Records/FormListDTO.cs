using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class FormListDTO : RecordDTO
{
    public FormKeyDTO? AddToListFormKey { get; set; }

    public IList<FormListItemDTO> Items { get; set; } = new List<FormListItemDTO>();
}
